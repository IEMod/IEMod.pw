using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IEMod.Mods.ConsoleMod;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.BackerStuff {
	[NewType]
	internal class BackerNamesMod {
		[NewMember]
		public static void FixBackerNames(bool state) {
			// finding all objects
			GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

			// finding all backer npcs
			List<GameObject> allBackers = new List<GameObject>();
			for (int i = allObjects.Length - 1; i > 0; i--) {
				if ((allObjects[i].name.StartsWith("NPC_BACKER") || allObjects[i].name.StartsWith("NPC_Visceris"))
					&& allObjects[i].GetComponent<CharacterStats>() != null) {
					allBackers.Add(allObjects[i]);
				}
			}

			if (state) {
				string humanMalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "human-males.txt");
				string humanFemalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"),
					"human-females.txt");

				string dwarfMalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "dwarf-males.txt");
				string dwarfFemalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"),
					"dwarf-females.txt");

				string elfMalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "elf-males.txt");
				string elfFemalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "elf-females.txt");

				string orlanMalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "orlan-males.txt");
				string orlanFemalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"),
					"orlan-females.txt");

				string aumauaMalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "aumaua-males.txt");
				string aumauaFemalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"),
					"aumaua-females.txt");


				string[] humanMaleNames;
				string[] humanFemaleNames;
				string[] dwarfMaleNames;
				string[] dwarfFemaleNames;
				string[] elfMaleNames;
				string[] elfFemaleNames;
				string[] orlanMaleNames;
				string[] orlanFemaleNames;
				string[] aumauaMaleNames;
				string[] aumauaFemaleNames;

				if (File.Exists(humanMalesPath)
					&& File.Exists(humanFemalesPath)
					&& File.Exists(dwarfMalesPath)
					&& File.Exists(dwarfFemalesPath)
					&& File.Exists(elfMalesPath)
					&& File.Exists(elfFemalesPath)
					&& File.Exists(orlanMalesPath)
					&& File.Exists(orlanFemalesPath)
					&& File.Exists(aumauaMalesPath)
					&& File.Exists(aumauaFemalesPath)) {
					humanMaleNames = File.ReadAllLines(humanMalesPath);
					humanFemaleNames = File.ReadAllLines(humanFemalesPath);
					dwarfMaleNames = File.ReadAllLines(dwarfMalesPath);
					dwarfFemaleNames = File.ReadAllLines(dwarfFemalesPath);
					elfMaleNames = File.ReadAllLines(elfMalesPath);
					elfFemaleNames = File.ReadAllLines(elfFemalesPath);
					orlanMaleNames = File.ReadAllLines(orlanMalesPath);
					orlanFemaleNames = File.ReadAllLines(orlanFemalesPath);
					aumauaMaleNames = File.ReadAllLines(aumauaMalesPath);
					aumauaFemaleNames = File.ReadAllLines(aumauaFemalesPath);

					for (int z = 0; z < allBackers.Count; z++) {
						// disabling dialogues for backers
						if (IEModOptions.DisableBackerDialogs)
							GameUtilities.Destroy(allBackers[z].GetComponent<NPCDialogue>());

						CharacterStats backer = allBackers[z].GetComponent<CharacterStats>();

						if (backer != null) {
							string originalName = backer.DisplayName.ToString();
							int seedForThisNpc = originalName.GetHashCode();
							if (backer.RacialBodyType == CharacterStats.Race.Human) {
								if (backer.Gender == Gender.Male)
									AssignRandomName(seedForThisNpc, ref humanMaleNames, backer);
								else if (backer.Gender == Gender.Female)
									AssignRandomName(seedForThisNpc, ref humanFemaleNames, backer);
							} else if (backer.RacialBodyType == CharacterStats.Race.Dwarf) {
								if (backer.Gender == Gender.Male)
									AssignRandomName(seedForThisNpc, ref dwarfMaleNames, backer);
								else if (backer.Gender == Gender.Female)
									AssignRandomName(seedForThisNpc, ref dwarfFemaleNames, backer);
							} else if (backer.RacialBodyType == CharacterStats.Race.Elf) {
								if (backer.Gender == Gender.Male)
									AssignRandomName(seedForThisNpc, ref elfMaleNames, backer);
								else if (backer.Gender == Gender.Female)
									AssignRandomName(seedForThisNpc, ref elfFemaleNames, backer);
							} else if (backer.RacialBodyType == CharacterStats.Race.Aumaua) {
								if (backer.Gender == Gender.Male)
									AssignRandomName(seedForThisNpc, ref aumauaMaleNames, backer);
								else if (backer.Gender == Gender.Female)
									AssignRandomName(seedForThisNpc, ref aumauaFemaleNames, backer);
							} else if (backer.RacialBodyType == CharacterStats.Race.Orlan) {
								if (backer.Gender == Gender.Male)
									AssignRandomName(seedForThisNpc, ref orlanMaleNames, backer);
								else if (backer.Gender == Gender.Female)
									AssignRandomName(seedForThisNpc, ref orlanFemaleNames, backer);
							}
						}
					}
				} else {
					Console.AddMessage(
						"IEMod: At least one of the .txt files with backer names is missing at path: "
							+ Path.Combine(Application.dataPath, "Managed/iemod/names"), Color.red);
				}
			} else {
				for (int z = 0; z < allBackers.Count; z++) {
					GameObject npc = allBackers[z];

					CharacterStats backer = npc.GetComponent<CharacterStats>();
					if (backer != null) {
						string originalName = backer.DisplayName.ToString();
						if (backer.OverrideName != "" && backer.OverrideName != originalName) {
							backer.OverrideName = originalName;
						}
					}
				}
			}
		}

		[NewMember]
		public static void AssignRandomName(int seed, ref string[] names, CharacterStats backer) {
			UnityEngine.Random.seed = seed;
			int randomId = 0;
			for (int i = 0; i < names.Length - 1; i++) {
				if (UnityEngine.Random.value > 0.5f) // UnityEngine.Random.value is between 0.0 and 1.0, it's based on Random.Seed
					randomId++;
			}
			backer.OverrideName = names[randomId];
			//Console.AddMessage ("Using seed " + seed + ", " + backer.DisplayName.ToString () + " rolled: " + randomId);
		}
	}
}
