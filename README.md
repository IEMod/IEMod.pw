# IEMod<sub>pw</sub> <a href="https://gitter.im/GregRos/Patchwork"><img style="float: right" src="https://badges.gitter.im/Join%20Chat.svg"/></a>

**IEMod<sub>pw</sub> Version/Patchwork Version:** 0.7.5/0.5


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

### UI Customization
There is a button on the upper right hand side of your screen that lets you customize the UI. Customizing the UI also allows you to drag this button somewhere else if you like.

Changed the interface somewhat.

Most of the code has been changed, though this isn't visible at this point. Largely changed the code so it's easier to fix later on, and it's also more readable. 

### Console
1. `ShowMouseDebug` enables a basic debug display for what is under the cursor.
2. `DeleteIEModSettings` deletes all of the current IEMod version's custom settings. In order to reset everything to default, you'll have to restart the game.

## Code Injection
See the [Patchwork library](https://github.com/GregRos/Patchwork) for more information. Patchwork is made part of this repository as a sub-module (basically a kind of sub-project).

## UI Creation
The options/settings of the mod are handled in the folder `IEMod\Mods\Options`.

Previously, UI creation was pretty convoluted and involved *a lot* of repetition. It also involved working around rather inconvenient (for us) features, such as language-specific string tables, and a pretty confusing GameObject and Component hierarchy. 

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

## Debugging Tips
Debugging mods like this one is hard. It's possible to debug Unity applications (attach a debugger to them, I mean), it's possible to debug modified assemblies, and it's possible to debug assemblies you don't have any debug symbols for. Unfortunately, there is no tool that allows you to debug all three, and I have no idea how to make one.

Debugging involves printouts and using the logs. The game's default debug log is in `PillarsOfEternity_Data\output_log.txt`. Crash reports, which include additional data like memory dumps, are filed in the main folder. The game only crashed when something really nasty happens, and most exceptions (including things like `NullReferenceException` and even `InvalidProgramException`) are just logged, and the show goes on. Usually, things like weird graphical glitches, disappearing text, and so on, indicate that an exception has been thrown.

In addition to this, you can use `IEDebug.Log` to write to a special log dedicated for `IEMod` that this class creates. It appears in the main folder of the game.

Because debugging is so difficult, your best bet is to check things carefully. Check if things are `null`, if an expression is a valid type before casting, etc, and then throw an exception that tells you something about the problem. Handling exceptions, if only to rethrow more meaningful ones, is also a good idea. Assertions are also great.

Nulls in particular are your worst enemy. Try to avoid using them as much as possible.

It's best to throw `IEModException`, because you'll be able to find the exception in the log later, and it narrows down the place where it was thrown. Also, `IEDebug.Exception` creates a new `IEModException` and also logs it to the IE mod log, which is handy.

You can use `UnityPrinter` to print unity object graphs. This is extremely helpful, especially when working with UI.

When navigating the Component/GameObject graph, use the extension methods in `UnityObjectExtensions`. For example, instead of `GetComponent<T>` (provided by Unity), use `Component<T>`. This is because the `GetX` methods can easily return `null`, and leave you to figure out where the error occurred. Extension methods in `UnityObjectExtensions` don't return nulls and throw informative exceptions if an error has occurred.

*More to come later*

## Modding Intro
*Here I'll talk about how modding unity games works in general (what I've managed to figure out, anyway), and Pillars of Eternity in particular.*

## License
[CC BY-SA 2.0](https://creativecommons.org/licenses/by-sa/2.0/), same as the original. Though this license is implied, rather than clearly stated.