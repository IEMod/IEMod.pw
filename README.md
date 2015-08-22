# IEMod<sub>pw</sub> <a href="https://gitter.im/GregRos/Patchwork"><img style="float: right" src="https://badges.gitter.im/Join%20Chat.svg"/></a>

**IEMod<sub>pw</sub> Version/Patchwork Version:** 0.7.5/0.5.5

*Also stylized __IEMod.pw__ in  places that don't support subscripts*.

This is a **fork** of [IEMod](https://bitbucket.org/Bester/poe-modding-framework) mod for [Pillars of Eternity](http://eternity.obsidian.net/). I ported the mod to my new assembly modification framework, [Patchwork](https://github.com/GregRos/Patchwork). 

I've transferred this project from [my old Bitbucket repository](https://bitbucket.org/GregRoss/patchwork-iemod).

I'll introduce IEMod myself at some point, but for now I'll just let the original website do the talking:

* [IEMod Nexus Page](http://www.nexusmods.com/pillarsofeternity/mods/1/?)
* [IEMod Website](http://rien-ici.com/iemod/)

## New Additions
These are additional mods to the game, beyond the functionality of the original IEMod.

### Target Turned Enemies
I've added an option called *"target turned enemies"* that makes enemies that have switched allegiance due to e.g. dominate, confusion, etc. to be considered hostile against your attacks. I always hated how being a little confused gave your enemies miraculous defensive benefits.

### Minimize Intrusive Backer Elements
This is probably the most requested feature I've seen. Many people find some backer NPCs and Tombstones to be intrusive. I've provided a few options to fix that.

1. **Disable Backer Dialogs:** If this option is chosen, you can no longer "talk" to backer NPCs (experience their backstory). This option existed in the mod before, but was kind of hidden. Also, backer NPCs will no longer have gold nameplates.


### UI Customization
1. You can access the UI customization from a button in the UI. This button is draggable as well in UI customization mode.
3. Turned the frame selection feature into a dropdown instead of many separate buttons. Also easier to add your own frames, since no changes to the code are necessary (you just add the appropriate files to the `iemod/frames` folder, and that's it).
2. Added a checkbox for enabling tooltip offset. This nudges the enemy tooltip that appears in the upper left hand corner to the right, so it's not blocked by other UI elements. It was an already present but somewhat hidden feature.
4. Restoring defaults happens instantly. You don't leave the UI customization menu or need to reload.
5. Fixed various small bugs people have had with the system. 
6. A few other small changes.

### Console Commands
1. `ShowMouseDebug` toggles debug information for what's under the cursor (this is something that was already in the game's code). There isn't much information. I'll probably expand it later on. You can use it to get the IDs of characters instead of using `FindCharacter`.
2. `ClearAllSettings true` clears all settings, including mod and game settings. 

## Code Injection
See the [Patchwork library](https://github.com/GregRos/Patchwork) for more information. Patchwork is made part of this repository as a sub-module (basically a kind of sub-project).

## UI Creation
UI Creation uses a system I call `QuickControls`. These are thin wrappers around the game's Unity controls (generally implemented as complicated `GameObjects`). I've implemented a checkbox, a dropdown, and a button. There are several reasons to use these controls:

1. They let you 


The options/settings of the mod are handled in the folder `IEMod\Mods\Options`.

Previously, UI creation was pretty convoluted and involved *a lot* of repetition. It also involved working around rather inconvenient (for us) features, such as language-specific string tables, and a pretty confusing GameObject and Component hierarchy. 

The new setup works using `QuickControls`. These are thin wrappers around the game's UI controls that are geared towards easy manipulation through code, rather than an editor. 

They're called "quick" because you can use them with little effort on your part. Also, as is implied, they expose pretty limited functionality.

Of course, some of that still happens, but somewhere far away from you.

The new setup uses the *static* class `IEModOptions` for your options. Each option is just a field (it could also be a property). Add the `[Label]` attribute to the member to set the label of the control that option is bound to, and `[Description]` to set its tooltip.

These options can be bound to controls created using the class `IEControlFactory`. Currently, the class can only create checkboxes bound to boolean fields/properties, and combo boxes bound to enum-typed fields/properties (each enum value represents an option).

You can put the `[Description]` attribute on each value in the enum definition to set its label in the combo box. 

Before creating a control, you have to get an example of that control from the UI. It's used as a prototype. Then you need the `Example` property for that control in the control factory. 

The UI is created in the file `mod_UIOptionsManager`. Here is an example of how simple the process is:

		//this is how you create a control factory:
		var controlFactory = new IEControlFactory {
			ExampleCheckbox = exampleCheckbox.gameObject,
			CurrentParent = Pages[5],
			ExampleComboBox = exampleDropdown.gameObject
		};
		//now you can just create the controls and they are automatically added to the page:
		//one check box
		_blueCirclesBg = controlFactory.Checkbox(() => IEModOptions.BlueCirclesBG);
		_blueCirclesBg.transform.localPosition = new Vector3(-180, 240, 0);

		//another checkbox
		_alwaysShowCircles = controlFactory.Checkbox(() => IEModOptions.AlwaysShowCircles);
		_alwaysShowCircles.transform.localPosition = new Vector3(-210, 210, 0);

		//this is a dropdown:
		_nerfedXpCmb = controlFactory.EnumBoundDropdown(() => IEModOptions.NerfedXPTableSetting, 515, 300);
		_nerfedXpCmb.transform.localPosition = new Vector3(-80, -70, 0);
		
The methods for creating controls take an `Expression` parameter. An expression is a bit like a lambda, but you can freely inspect it, so I just pull the  property/field you want to bind to the control from the lambda you write there. It's extremely convenient.

By the way, binding means that once the control is changed, the property/field is changed as well. Unfortunately, it doesn't work the other way around, at least not at this point.

## Data Binding
The UI toolkit supports a simple form of data binding. Data binding is when the values of two members are bound together, so that a change in one affects the other. Typically, it is used to bind properties of UI elements (such as a checkbox's `IsChecked` property) to application settings.

Data binding makes use the of `Binder` abstract class, which supplies data binding services. 

## Modding Tips

### Debugging

#### General Advice
Debugging mods like this one is hard. It's possible to debug Unity applications (attach a debugger to them, I mean), it's possible to debug modified assemblies, and it's possible to debug assemblies you don't have any debug symbols for. Unfortunately, there is no tool that allows you to debug all three, and I have no idea how to make one.

Debugging involves printouts and using the logs. The game's default debug log is in `PillarsOfEternity_Data\output_log.txt`. Crash reports, which include additional data like memory dumps, are filed in the main folder. The game only crashed when something really nasty happens, and most exceptions (including things like `NullReferenceException` and even `InvalidProgramException`) are just logged, and the show goes on. Usually, things like weird graphical glitches, disappearing text, and so on, indicate that an exception has been thrown.

In addition to this, you can use `IEDebug.Log` to write to a special log dedicated for `IEMod` that this class creates. It appears in the main folder of the game.

Because debugging is so difficult, your best bet is to check things carefully. Check if things are `null`, if an expression is a valid type before casting, etc, and then throw an exception that tells you something about the problem. Handling exceptions, if only to rethrow more meaningful ones, is also a good idea. Assertions are also great.

Nulls in particular are your worst enemy. Try to avoid using them as much as possible.

#### Features

It's best to throw `IEModException`, because you'll be able to find the exception in the log later, and it narrows down the place where it was thrown. Also, `IEDebug.Exception` creates a new `IEModException` and also logs it to the IE mod log, which is handy.

You can use `UnityPrinter` to print unity object graphs. This is extremely helpful, especially when working with UI.

When navigating the Component/GameObject graph, use the extension methods in `UnityObjectExtensions`. For example, instead of `GetComponent<T>` (provided by Unity), use `Component<T>`. This is because the `GetX` methods can easily return `null`, and leave you to figure out where the error occurred. Extension methods in `UnityObjectExtensions` don't return nulls and throw informative exceptions if an error has occurred.

Use the `QuickControl` system instead of working with raw GameObjects. These are thin wrappers around UI GameObjects that give you access to common components, as well as improved error detection.

*More to come later*

## Modding Intro
*Here I'll talk about how modding unity games works in general (what I've managed to figure out, anyway), and Pillars of Eternity in particular.*

## License
[CC BY-SA 2.0](https://creativecommons.org/licenses/by-sa/2.0/), same as the original. Though this license is implied, rather than clearly stated.