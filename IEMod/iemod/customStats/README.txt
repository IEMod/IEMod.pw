Welcome to the custom stats portion of the mod !

The way this works is closer to a macro than anything else, but essentially it will allow you to apply stat values entered in the xml files
found in Managed/iemod/customStats folder of your game file. You can either do so manually with a console command or with the help
of a checkbox in the IEmod options. 

IEmod Checkbox:
	You can create as many xml files as you want, but for the time being, only the one called custom.xml will be autoloaded by IEmod (subject to change if a lot of people request it).
	
	The automatic stat adjustment by the IEmod is for the moment only applied on map loads, therefore you will need to travel once to trigger it, or save and reload.
	
	This also means that changing out your companions at an inn/Stronghold will reset their stats to default until you travel. You can use the console command (explained below) if for some reason you absolutely need their stats restored right away.


Manually with the console command:
	
	This is a bit more involved, but maybe it can be usefull to someone. 

	You can manually import stats using the following command:  ImportStats filename           (do not include the .xml at the end, just the name will do)
	For example: ImportStats default         will re-adjust your companion's stats to their default values
	
	You can ''stack'' your modified stats that way, if you want to have a specific set of stats for a certain encounter. Simply make a new file and load it manually.
	On the next loading screen the regular custom stats will be automatically reloaded by the mod if the option is enabled, otherwise they will persist until you reload a saved game or
	switch out your party members.



In order for this feature to work properly, you will need to respect the following rules:

1) You HAVE to respect the general xml structure, otherwise it will not work properly so make sure the opening and closing tags are properly placed. 

2) You can include all the NPCs in the xml file or just the ones you want to respec, only the recognized names will have their stats changed.

3) If you respec an NPC, you will have to write down the values for ALL of their stats, even if you only intend to change a single one.

4) You can add any NPC you want to the file, including your watcher, his pet and whatever henchmen you have, as long as you know their name. In theory, this should also work with named ennemies but I have not tested it and unless it is requested it will not be officially supported by this mod.

5)Racial/Background bonuses are NOT included in the stats entered in the xml files, they will be added  on top automatically by the game.
Here are the  cumulated stat bonuses that the game will add from race/background choices:

	Aloth: +1 Dexterity, +1 Perception, +1 Resolve
	Eder:  +1 Might, +2 Resolve
	Kana: +2 Might, +1 Constitution
	Sagani: +2 Might, +1 Constitution, -1 Dexterity +1 Perception
	Itumaak: None
	Durance: +1 Might, +2 Resolve
	Pallegina: +1 Dexterity, + 2 Intellect
	Hiravias: -1 Might, +2 Perception, +1 Resolve, +1 Constitution
	Grieving Mother: +1 Might, +2 Resolve
	Devil of Caroc: +1 Resolve
	Zahua: +1 Might, +2 Resolve
	Maneha: +2 Might, +1 Constitution






