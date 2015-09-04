# IE Mod
**IEMod Version:** 5.0.1-beta<br/>
**PoE Version:** 2.0.0706<br/>
**License:** [CC BY-SA 2.0](https://creativecommons.org/licenses/by-sa/2.0/).<br/>
_**Patchwork Version:** 0.6.0_

This is the most recent repository for [IEMod](https://bitbucket.org/Bester/poe-modding-framework), which is a mod for [Pillars of Eternity](http://eternity.obsidian.net/). The previous repository may be found [here](https://bitbucket.org/Bester/poe-modding-framework). A repository for the fork which became the current version can be found [here](https://bitbucket.org/GregRoss/patchwork-iemod).

It uses the [Patchwork](https://github.com/GregRos/Patchwork) assembly modification framework.

Original descriptions of IE Mod (some may be outdated):

* [IEMod Nexus Page](http://www.nexusmods.com/pillarsofeternity/mods/1/?)
* [IEMod Website](http://rien-ici.com/iemod/)

## 5.0.1-beta Patch Notes
1. Fixed **Disable Friendly Fire** option.
2. Fixed **AutoSave** options.
3. Fixed **BSC** command (partial).
4. Fixed **Combad Only Mod** (partial)
5. Fixed **ReenableAchievements**

## New Additions in 5.0.0+

Most of these are optional.

### Target Turned Enemies
I've added an option called *"target turned enemies"* that makes enemies that have switched allegiance due to e.g. dominate, confusion, etc. to be considered hostile against your attacks. I always hated how being a little confused gave your enemies miraculous defensive benefits.

### Disable Backer Dialogs
If this option is chosen, you can no longer "talk" to backer NPCs (experience their backstory). This option existed in the mod before, but was kind of hidden.

### Cipher Base Focus
Modifies cipher's start focus in the beginning of combat.

### Fast Scouting Mode
You can choose from a number of options, depending on the behavior you want.

### UI Customization
1. You can access the UI customization from a button in the UI. This button is draggable as well in UI customization mode.
3. Turned the frame selection feature into a dropdown instead of many separate buttons. Also easier to add your own frames, since no changes to the code are necessary (you just add the appropriate files to the `iemod/frames` folder, and that's it).
2. Added a checkbox for enabling tooltip offset. This nudges the enemy tooltip that appears in the upper left hand corner to the right, so it's not blocked by other UI elements. It was an already present but somewhat hidden feature.
4. Restoring defaults happens instantly. You don't leave the UI customization menu or need to reload.
5. A few other small changes.

### Console Commands
1. `ShowMouseDebug` toggles debug information for what's under the cursor (this is something that was already in the game's code). There isn't much information. I'll probably expand it later on. You can use it to get the IDs of characters instead of using `FindCharacter`.
2. `ResetCustomUI` resets the custom UI settings to the default. Use this if you somehow manage to screw them up and can't fix them. You need to reload/area transition after this is done.

## Development Information
The options/settings of the mod are handled in the folder `IEMod\Mods\Options`.

Previously, UI creation was pretty convoluted and involved *a lot* of repetition. It also involved working around rather inconvenient (for us) features, such as language-specific string tables, and a pretty confusing GameObject and Component hierarchy. 

The new setup works using `QuickControls`. These are thin wrappers around the game's UI controls that are geared towards easy manipulation through code, rather than an editor. 

They're called "quick" because you can use them with little effort on your part. Also, as is implied, they expose pretty limited functionality.

Of course, some of that still happens, but somewhere far away from you.

The new setup uses the *static* class `IEModOptions` for your options. Each option is just a field (it could also be a property). Add the `[Label]` attribute to the member to set the label of the control that option is bound to, and `[Description]` to set its tooltip.

You can put the `[Description]` attribute on each value in the enum definition to set its label in the combo box. 

Before creating a control, you have to get an example of that control from the game UI. This is done automatically for you, usually.

The UI is created in the file `mod_UIOptionsManager`. 
		
Some methods for creating controls take an `Expression` parameter. An expression is a bit like a lambda, but you can freely inspect it, so I just pull the  property/field you want to bind to the control from the lambda you write there. It's extremely convenient.

By the way, binding means that once the control is changed, the property/field is changed as well. Unfortunately, it doesn't work the other way around, at least not at this point.

### Modding Tips

#### General Advice
Debugging mods like this one is hard. It's possible to debug Unity applications (attach a debugger to them, I mean), it's possible to debug modified assemblies, and it's possible to debug assemblies you don't have any debug symbols for. Unfortunately, there is no tool that allows you to debug all three, and I have no idea how to make one.

Debugging involves printouts and using the logs. The game's default debug log is in `PillarsOfEternity_Data\output_log.txt`. Crash reports, which include additional data like memory dumps, are filed in the main folder. The game only crashed when something really nasty happens, and most exceptions (including things like `NullReferenceException` and even `InvalidProgramException`) are just logged, and the show goes on. Usually, things like weird graphical glitches, disappearing text, and so on, indicate that an exception has been thrown.

In addition to this, you can use `IEDebug.Log` to write to a special log dedicated for `IEMod` that this class creates. It appears in the main folder of the game.

Because debugging is so difficult, your best bet is to check things carefully. Check if things are `null`, if an expression is a valid type before casting, etc, and then throw an exception that tells you something about the problem. Handling exceptions, if only to rethrow more meaningful ones, is also a good idea. Assertions are also great.

Nulls in particular are your worst enemy. Try to avoid using them as much as possible.

#### Features

It's best to throw `IEModException`, because you'll be able to find the exception in the log later, and it narrows down the place where it was thrown. Also, `IEDebug.Exception` creates a new `IEModException` and also logs it to the IE mod log, which is handy.

You can use `UnityPrinter` to print unity object graphs. This is extremely helpful, especially when working with UI.

When navigating the Component/GameObject graph, use the extension methods in `UnityObjectExtensions`. For example, instead of `GetComponent<T>` (provided by Unity), use `Component<T>`. This is because the `GetX` methods can easily return `null`, and leave you to figure out where the error occurred. Extension methods in `UnityObjectExtensions` don't return nulls and throw informative exceptions if an error has occurred, including the names of the objects involved, etc. 

Seriously, avoid nulls *at all costs*. 

Use the `QuickControl` system instead of working with raw GameObjects, where this is possible. These are thin wrappers around UI GameObjects that give you access to common components, as well as improved error detection and data binding.

#### Building

Note: this is a "how-to" that assumes you're a programmer, but not necessarily a Visual Studio dev. These steps should be performed at your own risk, this will obviously not result in a tested or supported build.

You need Microsoft Visual Studio (VS).  The [Community edition](https://www.visualstudio.com/products/visual-studio-community-vs) is sufficient.

1. Open `IEMod.pw.sln` in VS.
1. Edit the file `PathConsts.DoNotPush.cs` and set `YourGameFolderPath` to the path to your POE install.
1. Edit the file `NuGet.targets` and set `DownloadNuGetExe` to `true` since you probably don't have it.
1. Update the `PoE Dll Source` subfolders with the `Assembly-CSharp.dll` from your POE install.
    * Note: this is a temporary step, future versions should do this automatically.
1. Change the "startup" project to `PrepareEnvironment`, then run.
1. Change the "startup" project to `_Start!`, then run.
1. Make sure the `iemod` (the lowercase one!) folder is installed at `PillarsOfEternity_Data/Managed`.

Your `Assembly-CSharp.dll` will be patched directly during the build. Simply launch POE when done.
