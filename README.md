# lifx-kinect
Drive **lifx bulbs with your voice**.

## Requirements
* a **Kinect** plugged on **Windows 8+**
* **Ruby 2.0+** with [official LIFX gem](https://github.com/LIFX/lifx-gem)
* No particulary skills in development

## Recommended
* one (for each room) fanless NUC ( like [Intel DE3815TYKHE](http://www.intel.com/content/www/us/en/nuc/nuc-kit-de3815tykhe.html), low power consumption ) with Windows 8 and a Kinect v2
* one Raspberry as LIFX daemon server
* Small experiences in procedural programmation if you want to customize the Grammar, Voices, or Language you want (i will create a small configuration file later)

## How does it work ?

LIFX team provide a lot of work ex-nihilo in only two years, but there is only 3 official languages API : [Obj-C](https://github.com/LIFX/LIFXKit) for iOS and [Java](https://github.com/LIFX/lifx-sdk-android) for Android and [Ruby](https://github.com/LIFX/lifx-gem) for everything else.

Microsoft created the Kinect for XBox experience, and after trying every voice recognition tools found ([independant opensource projects](http://en.wikipedia.org/wiki/List_of_speech_recognition_software), android, ios hacks...) but the problem is not really the software. Microphone has to be very good to catch words without paying too attention to the noise around in rooms of life in a common house. And if you look at the price on those hardware, it's a real pain.

I finally tried a Kinect bought 15 EUR on ebay.
And my conclusion if that **the Kinect is truly at this time the best association of good but cheap hardware and good software development kit provided**.
Yes it's C#, but it's easy enough to make small orders to drive LIFX bulbs.

Now i have two working kinects, one for each floor, and the source code is only 140 lines in Ruby, and 100 lines in C#.

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
* wdexpress_full.exe (if you want to quickly customize logic with visual studio express)

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

## Customization

### Robot name : how can i change the name 'viki' to something else ?

### Grammar : how can i make the words  ?

### Trigger events : how can i make 'viki' to call other webervices and talk back the answer ?

#What is LIFX ?

LIFX are Wi-Fi RGB bulbs with an awesome API to make everything you want to do.
You can see those official presentation videos on kickstarter :

[![ScreenShot](http://img.youtube.com/vi/cRaPQDzkJcQ/0.jpg)](http://youtu.be/cRaPQDzkJcQ)

And the official website : http://www.lifx.co/
