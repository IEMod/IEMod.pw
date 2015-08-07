# IEMod<sub>pw</sub>
**IEMod<sub>pw</sub> Version:** 0.7
*Also stylized **IEMod.pw** in places that don't support subscripts*

This is a **fork** of the [IEMod](https://bitbucket.org/Bester/poe-modding-framework). I ported the mod to my new assembly modification framework, [Patchwork](https://github.com/GregRos/Patchwork). Originally I hoped to merge my fork into the original mod (and I still hope to do that), but I couldn't contact any active developers, so I decided to continue development by myself for now.

I'd introduce IEMod, but for now I'll just let the original documentation do the talking:
[IEMod Nexus Page](http://www.nexusmods.com/pillarsofeternity/mods/1/?)

My only modifications so far have been mostly to the implementation, and I've kept the user-visible parts completely unchanged. 

I've focused on making further modding both convenient and expedient, so I've only worked on the parts that every modder would want to use: the code injection features (implemented in Patchwork) and the UI creation.

## Code Injection
See the Patchwork library for more information. Patchwork is made part of this repository as a sub-module (basically a kind of sub-project), so you can work on both repositories side by side.

## UI Creation
Previously, UI creation was pretty convoluted and involved *a lot* of repetition. It also involved working around rather inconvenient (for us) features, such as language-specific string tables, and a pretty confusing GameObject and Component hierarchy. 

Of course, that still happens, but somewhere far away from you.

The new setup uses the *static* class `IEModOptions` for your options. Each option is just a field (it could also be a property). Add the `[Label]` attribute to the member to set the label of the control that option is bound to, and `[Description]` to set its tooltip.

The actual optios 

Creating a simple UI is simple. There is a class called `IEControlCreator` that you instantiate. It has methods like `Checkbox` and `EnumBoundComboBox`. You just need to supply 


I've ported the mod to my 

Currently my only modifications have been:

1. Directly ported the mod from the old framework to Patchwork.
2. I've added an option called *"target turned enemies"* that makes enemies that have switched allegiance due to e.g. dominate, confusion, etc. to be considered hostile against your attacks. I always hated how being a little confused gave your enemies miraculous defensive benefits.
2. My most major change has been an almost complete overhaul of the UI part, using the additional features of my framework. Adding new checkboxes and combobxes now requires a few easily readable lines of code, and there will be no issues with string tables. This isn't user-visible; it just makes continuing development that much easier.

## License
[CC BY-SA 2.0](https://creativecommons.org/licenses/by-sa/2.0/), same as the original. Though this license is implied, rather than clearly stated.