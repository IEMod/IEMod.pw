# IEMod<sub>pw</sub>
**IEMod Version:** 0.7<sub>pw</sub>

This is a **fork** of the [IEMod](https://bitbucket.org/Bester/poe-modding-framework). I ported the mod to my new assembly modification framework, [Patchwork](https://github.com/GregRos/Patchwork). Originally I hoped to merge my fork into the original mod (and I still hope to do that), but I couldn't contact any active developers, so I decided to continue development by myself for now.

I'd introduce IEMod, but I'll just let the original documentation do the talking:
[IEMod Nexus Page](http://www.nexusmods.com/pillarsofeternity/mods/1/?)

Currently my only modifications have been:

1. Directly ported the mod from the old framework to Patchwork.
2. I've added an option called *"target turned enemies"* that makes enemies that have switched allegiance due to e.g. dominate, confusion, etc. to be considered hostile against your attacks. I always hated how being a little confused gave your enemies miraculous defensive benefits.
2. My most major change has been an almost complete overhaul of the UI part, using the additional features of my framework. Adding new checkboxes and combobxes now requires a few easily readable lines of code, and there will be no issues with string tables. This isn't user-visible; it just makes continuing development that much easier.

## License
[CC BY-SA 2.0](https://creativecommons.org/licenses/by-sa/2.0/), same as the original. Though this license is implied, rather than clearly stated.