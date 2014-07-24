# lifx-kinect
Drive **lifx bulbs with your voice**.

[![ScreenShot](http://img4.hostingpics.net/pics/325982ScreenShot241.jpg)](http://youtu.be/uEPNLfaTRe4)

## Requirements
* a **Kinect** plugged on **Windows 8+** *(in fact, a Kinect is not a must, but if you want a casual experience at home without an headset, or a desk microphone you really should use a Kinect)*
* **Ruby 2.0+** with [official LIFX gem](https://github.com/LIFX/lifx-gem)
* No skills in development

## Recommended hardware
* one (for each room) fanless NUC ( like [Intel DE3815TYKHE](http://www.intel.com/content/www/us/en/nuc/nuc-kit-de3815tykhe.html), low power consumption ) with Windows 8 and a Kinect v2
* one Raspberry as LIFX daemon server and other webservices to hack (informations grabbing, etc...)
* Small experiences in procedural programmation if you want to customize the logic

## How does it work ?

LIFX team provide a lot of work ex-nihilo in only two years, but there is only 3 official languages API : [Obj-C](https://github.com/LIFX/LIFXKit) for iOS and [Java](https://github.com/LIFX/lifx-sdk-android) for Android and [Ruby](https://github.com/LIFX/lifx-gem) for everything else.

Microsoft created the Kinect for XBox experience, and after trying every voice recognition tools found ([independant opensource projects](http://en.wikipedia.org/wiki/List_of_speech_recognition_software), android, ios hacks...) but the problem is not really the software. Microphone has to be very good to catch words without paying too attention to the noise around in rooms of life in a common house. And if you look at the price on those hardware, it's a real pain.

I finally tried a Kinect bought 15 EUR on ebay.
And my conclusion if that **the Kinect is truly at this time the best association of good but cheap hardware and good software development kit provided**.
Yes it's C#, but it's easy enough to make small orders to drive LIFX bulbs.

Now i have two working kinects, one for each floor, and the source code is only 140 lines in Ruby, and 300 lines in C#.

## Installation

### Kinect

You'll have to be sure to have installed every software development toolkits for the binary program to work. Here is a list of the "only" files you need to get things work, i recommend to install every files with this order :

* [KinectSDK-v2.0-PublicPreview1407-Setup.exe](http://www.microsoft.com/en-ie/download/details.aspx?id=43661) (even if you have a Kinect v1, it improves recognition)
* [KinectRuntime-v1.8-Setup.exe](http://www.microsoft.com/en-us/download/details.aspx?id=40277)
* [MSSpeech_SR_en-US_TELE.msi and MSSpeech_TTS_en-GB_Hazel.msi...and all localisation you want](http://www.microsoft.com/en-us/download/details.aspx?id=3971)...
* [MicrosoftSpeechPlatformSDK.msi (x86 and x64)](http://www.microsoft.com/en-us/download/details.aspx?id=27226)
* [SpeechPlatformRuntime.msi (x86 and x64)](http://www.microsoft.com/en-us/download/details.aspx?id=27225)
* [KinectSpeechLanguagePack_fr-FR.exe and all you want](http://www.microsoft.com/en-ie/download/details.aspx?id=34809)

Facultative :
* [wdexpress_full.exe](http://www.microsoft.com/france/visual-studio/essayez/express.aspx) (if you want to quickly customize logic with visual studio express)

1. Get the sourcecode. For example with the command `git clone https://github.com/abalam666/lifx-kinect`
2. Edit the configuration file `/lifx-kinect/kinect-recognizer/lifx-kinect.xml`
3. Check up that you have a microphone and speakers plugged on your PC
4. Double-click on the program `/kinect-recognizer/bin/x64/Release/jarvis.exe` *Yeah i know... Jarvis was the first botname i used ^^*

If everthing is fine, the program should be waiting for a command.
As a first check you can try now to say *viki allume toute la maison* and see if it writes your words on the screen.

### Ruby LIFX daemon

The aim of **this daemon** is to checkup every minutes and to **keep updated the list of living bulbs** ready to accept orders immediately.

LIFX bulbs have to be discovered to get addresses, status and to be able to send an order to them. This is a problem if you need a light to power on AS SOON AS you want to, and not 30 seconds after you are in the room (we'll talk later about Z-Wave or 433Mhz interface).

1) First you can checkup your ruby version with `ruby -v` and something like this should appear :
```
ruby 2.1.2p95 (2014-05-08 revision 45877) [armv6l-linux-eabihf]
```

2) If you already have downloaded the [official LIFX gem](https://github.com/LIFX/lifx-gem) you can install this two viki files into the examples already provided : 

```
/lifx-gem/examples/
├── auto-off
│   ├── auto-off.rb
│   └── Gemfile
├── identify
│   ├── Gemfile
│   └── identify.rb
├── travis-build-light
│   ├── build-light.rb
│   └── Gemfile
└── viki
    ├── Gemfile
    └── viki.rb
```

3) And you just have to run the command `bundle` into the folder `/lifx-gem/examples/viki`.

4) As soon as the bundle is completed, you just have to start the daemon with the command : `ruby viki.rb`

5) The program should display the number of bulbs discovered in the 10 first seconds, then it will rediscover every 60 seconds.

6) You should be able to connect to the tiny synthesis of your bulbs here : http://localhost:1234/



## Customization

### Robot name : how can i change the name *viki* to something else and the language ?

I'm using this name for my bot, from the movie "I, Robot" ([Virtual Interactive Kinetic Intelligence](http://en.wikipedia.org/wiki/I,_Robot_(film))). But you can change the name with the parameter `botname`.

I personnaly use to change the name of my bot when i want, we could create themes to speak with samples instead of microsoft text-to-speech. These days, it is named McFly, and it calls me Doc ^^

You just have to setup the parameter *botname* in the main parameters in the XML file named *lifx-kinect.xml* :

```
<lifx_kinect
    botname="viki"
    voice="FR, Hortense"
    recognitionLanguage="fr-FR"
    wakeupnow="réveille-toi s'il te plait"
    sleepnow="endors-toi s'il te plait"
    turnon="allume"
    turnoff="éteint"
    colorize="colore"
    slowly="lentement"
    lifxUrl="http://192.168.66.150:1234/"
    >
```

### Grammar : how can i setup the zones in my house, and map spoken words to particulary LIFX bulbs ?

The daemon handles few commands, but it represente a lot of possible combinations.
First, it can handle several types of orders to bulbs :
* color
* on
* off

Second, you can call :
* All once (all)
* All with a tag (tag/Bedroom)
* One bulb with its id (d073d500cd62)

This mapping is done with the life-kinect.xml :
```
<zones>
  <zone path="all" words="toute la maison" />
  <zone path="tag/RDC" words="le rez-de-chaussée" />
  <zone path="tag/Etage" words="l'étage" />
  <zone path="tag/Salle de bain" words="la salle de bain" />
  <zone path="d073d500d6ca" words="le couloir" />
</zones>
```

Third, you have a color, prefixed by *dark* or *light* :
* red
* orange
* yellow
* green
* cyan
* blue
* pink
* purple
* white

Fourth, a last optionnal argument *slow* to do the transition slowly.

Then you could call for example those URLs to immediately execute orders :

```
http://localhost:1234/off/all
http://localhost:1234/color/tag/Bedroom/cyan
http://localhost:1234/color/d073d500cd62/darkpurple
...etc...
```

### Trigger events : how can i make *viki* to call other *not-LIFX* webervices and talk back the answer ?

In the same XML, you can customize any web service, and the text-to-speech will say the result.

Here's the custom grammar :

```
<custom_grammars>
    <grammar url="http://your.web.service.com/api/getTemperature/cuisine" words="quelle est la température de la cuisine" />
    <grammar url="http://your.web.service.com/api/getTemperature/chambre" words="quelle est la température de la chambre" />
    <grammar url="http://your.web.service.com/api/getTemperature/salon" words="quelle est la température du salon" />
    <grammar url="http://your.web.service.com/api/getVent" words="quelle est la température de la cuisine" />
    <grammar url="http://your.web.service.com/api/getConso" words="combien consomme la maison" />
    <grammar url="http://your.web.service.com/api/getConso" words="quelle est la consommation actuelle" />
  </custom_grammars>
```

#What is LIFX ?

LIFX are Wi-Fi RGB bulbs with an awesome API to make everything you want to do.
You can see those official presentation videos on kickstarter :

[![ScreenShot](http://img.youtube.com/vi/cRaPQDzkJcQ/0.jpg)](http://youtu.be/cRaPQDzkJcQ)

And the official website : http://www.lifx.co/
