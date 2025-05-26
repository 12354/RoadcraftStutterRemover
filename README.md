Roadcraft Stutter Remover
==============
TL;DR:
==============
I fixed the stuttering issue where your throttle cuts off:

[Download under Assets](https://github.com/12354/RoadcraftStutterRemover/releases/latest)

Start RoadcraftStutterRemover.exe and use the first option.

It will ask you for the location of your Roadcraft - Retail.exe, patch the game for you and fix the issue. 
This needs to be redone after every new Roadcraft Patch.

There's also the second option, which will not modify any files on your disk.
This will instead modify the memory of a running roadcraft instance.
Use this for a microsoft store or future game pass version(if it ever comes out), if you dont want to mess around with protected game pass files.
To run this, .NET 8.0 is needed.


Whats the issue
==============

[I originally fixed this issue in SnowRunner](https://old.reddit.com/r/snowrunner/comments/pt78i8/i_fixed_the_stuttering_issues_where_your_throttle/):
After playing SnowRunner for a while I encountered the issue where your game stutters for a second and you lose your throttle([example post](https://old.reddit.com/r/snowrunner/comments/gk38ez/stutter_and_throttle_cut_off/)).
Unfortunately this issues still exists in Roadcraft.

Other types of stuttering will not be fixed by this, but this one was the most annoying and long stutter I had in this game.


What causes it?
==============

Windows send a notification to the game that some device has changed (maybe new device plugged in, maybe some device has issues where it reconnects every minute). When Roadcraft receives this notification it enumerates all input devices(takes a long time => stutter) and recreates it's direct input device(this loses keypresses => cut off throttle). You can try this by plugging in a usb device.

How did you fix it?
==============

By preventing the game from receiving the "device changed notification", we can prevent this issue altogether. This is done by hooking into game internals and preventing the notification from reaching the game loop. This means you will probably not be able to plug in a new usb controller while the game is running, and you need to restart the game. 


Is this safe?
==============
I've uploaded[ the source code to github](https://github.com/12354/RoadcraftStutterRemover) and let github compile it. This means everyone can check if the source code is safe and I can not hide code.

Microsoft Store version
==============

Patching the Microsoft Store Version on disk is not easy, [fight9 posted a complicated way](https://old.reddit.com/r/snowrunner/comments/pt78i8/i_fixed_the_stuttering_issues_where_your_throttle/hp8w88y/) to circumvent this protection.

I've added an option to patch the memory of a running Roadcraft instance instead to RoadcraftStutterRemover, to hopefully bypass this issue.


Just open the game first then open RoadcraftStutterRemover.exe select the 2. option. 

As this does not modify the game you will need to run this every time you restart the game. Try running the patcher as administrator if you encounter any issues while patching.
