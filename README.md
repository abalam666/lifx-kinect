# lifx-kinect
Drive all your lifx bulbs in your house with your voice.

## Requirements
* a Kinect plugged on Windows 8+
* Ruby 2.0+
* No particulary skills in development

## Recommended
* one (for each room) fanless NUC ( like [Intel DE3815TYKHE](http://www.intel.com/content/www/us/en/nuc/nuc-kit-de3815tykhe.html), low power consumption ) with Windows 8 and a Kinect v2
* one Raspberry as LIFX daemon server
* Small experiences in procedural programmation if you want to customize the Grammar, Voices, or Language you want (i will create a small configuration file later)

## How does it work ?

LIFX team provide a lot of work ex-nihilo in only two years, but there is only 3 official languages API : [Obj-C](https://github.com/LIFX/LIFXKit) for iOS and [Java](https://github.com/LIFX/lifx-sdk-android) for Android and [Ruby](https://github.com/LIFX/lifx-gem) for everything else.

Microsoft created the Kinect for XBox experience, and after trying every voice recognition possible (independant opensource projects, android, ios hacks...) but the problem is not really the software. Microphone has to be very good to catch words without paying too attention to the noise around in rooms of life in a common house.

I finally tried a Kinect bought 15 EUR on ebay.
And my conclusion if that the Kinect is truly at this time the best association of good hardware and good software development kit provided.
Yes it's C#, but it's easy enough to make small orders to drive LIFX bulbs.

Now i have two working kinects, one for each floor, and the source code is only 140 lines in Ruby, and 100 lines in C#.

#What is LIFX ?

LIFX are Wi-Fi RGB bulbs with an awesome API to make everything you want to do.
You can see those official presentation videos on kickstarter :

[![ScreenShot](http://img.youtube.com/vi/cRaPQDzkJcQ/0.jpg)](http://youtu.be/cRaPQDzkJcQ)

And the official website : http://www.lifx.co/
