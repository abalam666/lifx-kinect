using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Net;
using System.Xml;

using Newtonsoft.Json.Linq;

namespace jarvis{

    static class Program {
        [STAThread]
        static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Bot());
        }
    }

    public partial class Bot : Form{
        string botname = "viki";
        string voice = "FR, Hortense";
        string recognitionLanguage = "fr-FR";
        string _wakeupnow = "réveille-toi s'il te plait";
        string _sleepnow = "endors-toi s'il te plait";
        string _turnon = "allume";
        string _turnoff = "éteint";
        string _colorize = "colore";
        string _slowly = "lentement";
        string _confidence = "0,6";
        string lifxUrl = "http://localhost:1234/";
        string _errorMessageWebService = "";

        Boolean __standby = false;
        SpeechSynthesizer synth;
        SpeechSynthesizer synthEN;
        WebClient http;
        SpeechRecognitionEngine sre;
        XmlDocument doc;

        IDictionary<string, string> quotes = new Dictionary<string, string>();
        IDictionary<string, string> urls = new Dictionary<string, string>();

        Choices choicesColors = new Choices();
        Choices choicesZones = new Choices();
        Choices choicesInfo = new Choices();

        public Bot(){
            init();
            while (true) sre.Recognize();
        }

        // Initialize words, http, synths and grammars
        void init(){
            doc = new XmlDocument();
            doc.Load("lifx-kinect.xml");
            XmlElement root = doc.DocumentElement;

            // Main parameters
            botname = root.GetAttribute("botname");
            _wakeupnow = root.GetAttribute("wakeupnow");
            _sleepnow = root.GetAttribute("sleepnow");
            _turnon = root.GetAttribute("turnon");
            _turnoff = root.GetAttribute("turnoff");
            voice = root.GetAttribute("voice");
            recognitionLanguage = root.GetAttribute("recognitionLanguage");
            _colorize = root.GetAttribute("colorize");
            _slowly = root.GetAttribute("slowly");
            lifxUrl = root.GetAttribute("lifxUrl");
            _errorMessageWebService = root.GetAttribute("errorMessageWebService");

            http = new WebClient();
            http.Encoding = System.Text.Encoding.UTF8;

            synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            synth.SelectVoiceByHints(VoiceGender.Female);
            synth.SelectVoice("Microsoft Server Speech Text to Speech Voice (fr-" + voice + ")");
            foreach (InstalledVoice v in synth.GetInstalledVoices()){
                VoiceInfo info = v.VoiceInfo;
                Console.WriteLine("== Voices available on this system ==");
                Console.WriteLine(" Name:          " + info.Name);
                Console.WriteLine(" Culture:       " + info.Culture);
                Console.WriteLine(" Age:           " + info.Age);
                Console.WriteLine(" Gender:        " + info.Gender);
                Console.WriteLine(" Description:   " + info.Description);
                Console.WriteLine(" ID:            " + info.Id);
            }

            sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo(recognitionLanguage));
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizedProcedure);

            initWordsCustom();
            initWordsLIFX();
            initGrammars();
        }

        // Available grammars
        void initGrammars(){
            // Standby
            Choices choicesStandby = new Choices();
            choicesStandby.Add(new string[] { _wakeupnow, _sleepnow });
            GrammarBuilder Standby = new GrammarBuilder();
            Standby.Append(botname);
            Standby.Append(choicesStandby);
            Grammar gStandby = new Grammar(Standby);
            sre.LoadGrammarAsync(gStandby);

            // On/Off
            Choices choicesOnOff = new Choices();
            choicesOnOff.Add(new string[] { _turnoff, _turnon });
            GrammarBuilder OnOff = new GrammarBuilder();
            OnOff.Append(botname);
            OnOff.Append(choicesOnOff);
            OnOff.Append(choicesZones);
            Grammar gOnOff = new Grammar(OnOff);
            sre.LoadGrammarAsync(gOnOff);

            // Couleurs
            Choices choicesColoriser = new Choices();
            choicesColoriser.Add(new string[] { _colorize });
            GrammarBuilder colorize = new GrammarBuilder();
            colorize.Append(botname);
            colorize.Append(choicesColoriser);
            colorize.Append(choicesZones);
            colorize.Append(choicesColors);
            colorize.Append(_slowly, 0, 1);
            Grammar gColorize = new Grammar(colorize);
            sre.LoadGrammarAsync(gColorize);

            // Infos maison
            GrammarBuilder quelleEst = new GrammarBuilder();
            quelleEst.Append(botname);
            quelleEst.Append(choicesInfo);
            Grammar gquelleTemp = new Grammar(quelleEst);
            sre.LoadGrammarAsync(gquelleTemp);
       }

        // Custom calls
        void initWordsCustom(){
            Console.WriteLine("");
            Console.WriteLine("Please start your command with '" + botname + "', here are all possible commands :");
            foreach (XmlElement item in doc.SelectNodes(@"/lifx_kinect/custom_grammars/grammar")){
                Console.WriteLine(botname + " " + item.GetAttribute("words"));
                choicesInfo.Add(item.GetAttribute("words"));
                urls[botname + " " + item.GetAttribute("words")] = item.GetAttribute("url");
            }
       }

        // LIFX calls to the provided daemon
        void initWordsLIFX(){
            Console.WriteLine(botname + " " + _colorize + " {zone} {color} [" + _slowly+"]");

            Console.WriteLine("");
            Console.WriteLine("> Zones available :");
            IDictionary<string, string> zones = new Dictionary<string, string>();
            foreach (XmlElement item in doc.SelectNodes(@"/lifx_kinect/lifx_grammars/zones/zone")) {
                Console.Write(item.GetAttribute("words") + ", ");
                zones[item.GetAttribute("words")] = item.GetAttribute("path");
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" > Colors available :");
            IDictionary<string, string> colors = new Dictionary<string, string>();
            foreach (XmlElement item in doc.SelectNodes(@"/lifx_kinect/lifx_grammars/colors/color")){
                choicesColors.Add(item.GetAttribute("words"));
                Console.Write(item.GetAttribute("words")+", ");
                colors[item.GetAttribute("words")] = item.GetAttribute("code");
            }

            foreach (var z in zones){
                choicesZones.Add(z.Key);
                urls[botname + " " + _turnoff + " " + z.Key] = lifxUrl+"off/" + z.Value;
                urls[botname + " " + _turnon + " " + z.Key] = lifxUrl + "on/" + z.Value;
                foreach (var c in colors) {
                    urls[botname + " " + _colorize + " " + z.Key + " " + c.Key] = lifxUrl + "color/" + z.Value + "/" + c.Value;
                    urls[botname + " " + _colorize + " " + z.Key + " " + c.Key + " " + _slowly] = lifxUrl + "color/" + z.Value + "/" + c.Value + "/slow";

                    if (c.Value == "random"){
                        quotes[lifxUrl + "color/" + z.Value + "/" + c.Value] = quotes["color/" + z.Value + "/" + c.Value + "/slow"] = "random color";
                    }
                }
            }

            quotes[lifxUrl + "off/tag/Chambre"] = "Oké, j'éteins la chambre !";
        }

        void recognizedProcedure(object sender, SpeechRecognizedEventArgs e) {
            Console.WriteLine("\nSpeech recognized: " + e.Result.Text + ", " + e.Result.Confidence);
            Console.WriteLine("Number of Alternates from Recognizer: {0}", e.Result.Alternates.Count.ToString());
            foreach (RecognizedPhrase phrase in e.Result.Alternates)
                Console.WriteLine(phrase.Text + ", " + phrase.Confidence);

            if (e.Result.Confidence < float.Parse(_confidence))
            {
                Console.WriteLine("Confidence threshold is higher...");
                return;
            }

            // stand-by
            if (e.Result.Text == botname + " " + _sleepnow){
                __standby = true; 
            } else if (e.Result.Text == botname + " " + _wakeupnow){
                __standby = false;
            }
            if (__standby) {
                Console.WriteLine("zzzZZZZZzzzzzzzzzZZZZZ...");
                return;
            }

            if (urls.ContainsKey(e.Result.Text)) {
                string url = urls[e.Result.Text];
                string result = get(url);
                if (result=="-1") {
                    // Erreur
                } else {
                    if (result.Length > 0){
                        Console.WriteLine("Result " + result);
                        synth.Speak(result);
                    }
                    if (quotes.ContainsKey(url)) {
                        string quote = quotes[url];
                        if (quote=="random color") {
                            string[] quotesRandom = { "Je choisis cette couleur, est-ce que cela vous plait ?", "Voilà une autre couleur !", "Est-ce que cela vous va ?", "Et hop !", "En voilà une spéciale pour vous." };
                            Random rnd = new Random();
                            rnd.Shuffle(quotesRandom);
                            quote = quotesRandom[0];
                        }
                        synth.Speak(quote);
                    }
                }
            }
        }

        string get(string url) {
            Console.WriteLine("getting this webservice : "+url);
            string result = "";
            try {
                result = http.DownloadString(url);
                if (result.Length > 1 && result.Substring(0, 1)==">"){
                    result = result.Substring(1, result.Length-1);
                } else {
                    result = "";
                }
                return result;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                synth.Speak(_errorMessageWebService + ex.Message);
            }
            return "-1";
        }
        /*
        bool post(string message) {
            NameValueCollection values = new NameValueCollection();
            values.Add("id", "toto");
            byte[] responseArray = http.UploadValues("http://192.168.66.6/emmet.php", "POST", values);
            string res = Encoding.ASCII.GetString(responseArray);
            dynamic d = JObject.Parse(res);
            Console.WriteLine(d.result);
            return true;
        }*/
    }

    static class RandomExtensions
    {
        public static void Shuffle<T>(this Random rnd, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rnd.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
