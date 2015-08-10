using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IEMod.Mods.ObjectBrowser;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.ConsoleMod {
	[ModifiesType()]
	public class mod_CommandLine : CommandLine
	{
		[ModifiesType()]
		public class Mod5_GameState : GameState {
			[ModifiesAccessibility]
			public new bool s_playerCharacter;
		}
		[ModifiesType()]
		public class Mod5_Loot : Loot {

			[ModifiesAccessibility]
			public new bool GetInventoryComponent() {
				return false;
			}
		}
		[ModifiesType()]
		public class Mod5_Health : Health {
			public new bool m_isAnimalCompanion {
				[ModifiesAccessibility()] get;
				[ModifiesAccessibility()] set; 
			}

			public new bool CanDie
			{
				[ModifiesAccessibility()]
				get;
				set;
			}
		}
		[ModifiesType]
		public class Mod5_QuestManager : QuestManager {

			[ModifiesAccessibility("get_ActiveQuests")]
			public void ActiveGetter() {
				
			}
		}
		[ModifiesType("StringTableManager")]
		public static class Mod5_StringTableManager {
			[ModifiesAccessibility]
			public static bool StringTables;
			[ModifiesAccessibility]
			public static bool StringTableLookup;
		}

		[ModifiesType]
		public class Mod5_UIOptionsManager : UIOptionsManager {
			[ModifiesAccessibility]
			public new bool m_GameMode;
		}
		[ModifiesType]
		public class Mod5_CharacterStats : global::CharacterStats {
			[ModifiesAccessibility]
			public new bool m_stronghold;
		}

				[NewMember]
		public  string modname = "";
		[NewMember]
		public  string description = "";
		[NewMember]
		public  string author = "";

		[NewMember]
		public  static GameObject modelViewerBackground;
		[NewMember]
		public  static UITexture GameBrowserBackground;

		[NewMember]
		public  static GameObject DragBtn1;
		[NewMember]
		public  static GameObject DragBtn2;
		[NewMember]
		public  static GameObject DragBtn3;
		[NewMember]
		public  static GameObject DragBtn4;
		[NewMember]
		public  static GameObject DragBtn5;
		[NewMember]
		public  static GameObject SaveBtn;
		[NewMember]
		public  static GameObject CancelBtn;
		[NewMember]
		public  static GameObject UseDefaultUIBtn;
		[NewMember]
		public  static GameObject DragBtn6;
		[NewMember]
		public  static GameObject DragBtn7;
		[NewMember]
		public  static GameObject DragBtn8;
		[NewMember]
		public  static GameObject DragBtn9;
		[NewMember]
		public  static GameObject DragBtn10;
		[NewMember]
		public  static GameObject DragBtn11;
		[NewMember]
		public  static GameObject DragBtn12;
		[NewMember]
		public  static GameObject DragBtn13;

		[NewMember]
		public  static GameObject DragBtn14;
		[NewMember]
		public  static GameObject DragBtn15;
		[NewMember]
		public  static GameObject DragBtn16;
		[NewMember]
		public  static GameObject DragBtn17;
		[NewMember]
		public  static GameObject DragBtn18;
		[NewMember]
		public  static GameObject DragBtn19;
		[NewMember]
		public  static GameObject DragBtn20;
		[NewMember]
		public  static GameObject DragBtn21;
		[NewMember]
		public  static GameObject DragBtn22;
		[NewMember]
		public  static GameObject DragBtn23;
		[NewMember]
		public  static GameObject DragBtn24;
		[NewMember]
		public  static GameObject DragBtn25;
		[NewMember]
		public  static GameObject DragBtn26;
		[NewMember]
		public  static GameObject DragBtn27;
		[NewMember]
		public  static GameObject DragBtn28;
		[NewMember]
		public  static GameObject DragBtn29;
		[NewMember]
		public  static GameObject DragBtn30;
		[NewMember]
		public  static GameObject DragBtn31;
		[NewMember]
		public  static GameObject DragBtn32;
		[NewMember]
		public  static GameObject DragBtn33;

		[NewMember]
		public  static Texture DefaultActionBarAtlas;
		[NewMember]
		public  static Texture2D AlternateActionBarAtlas;

		[NewMember]
		public  static Texture DefaultLeftCornerTexture;
		
		[NewMember]
		public  static void ReplaceAtlas(bool state)
		{
			//			// THIS IS WHAT YOU WOULD DO if you wanted to use a custom atlas
			//
			//			UIAtlas copy = (UIAtlas) UIAtlas.Instantiate (attackspr.atlas);
			//			copy.name = "Pizda";
			//			attackspr.atlas = copy;
			//			copy.spriteMaterial =  new Material(UIAtlasManager.Instance.Inventory.spriteMaterial.shader);
			//			copy.spriteMaterial.mainTexture = atlasTexture;
			//
			//			copy.spriteList.Clear ();
			//
			//			UIAtlas.Sprite sprite1 = new UIAtlas.Sprite ();
			//			sprite1.inner = new Rect (283, 18, 42, 42);
			//			sprite1.outer = new Rect (0, 0, 0, 0);
			//			sprite1.paddingLeft = 0;
			//			sprite1.paddingTop = 0;
			//			sprite1.paddingRight = 0;
			//			sprite1.paddingBottom = 0;
			//			sprite1.name = "test1";
			//
			//			copy.spriteList.Add (sprite1);
			//
			//			// END
			if (state)
			{
				if (AlternateActionBarAtlas == null)
				{
					string alternateAtlas = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/buttons"), "ActionBarAlternate.png");
					
					if (File.Exists(alternateAtlas))
					{
						Texture2D atlasTexture = new Texture2D(512, 256, TextureFormat.RGB24, false);
						byte[] bytes = File.ReadAllBytes(alternateAtlas);
						atlasTexture.LoadImage(bytes);
						atlasTexture.name = "AlternateAtlas";
						AlternateActionBarAtlas = atlasTexture;
						UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<UISprite>().atlas.spriteMaterial.mainTexture = atlasTexture; //attack icon

					}
					else
					{
						global::Console.AddMessage("Couldn't read file at path: " + alternateAtlas, Color.red);
					}
				}
				else
				{
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<UISprite>().atlas.spriteMaterial.mainTexture = AlternateActionBarAtlas;
				}
			}
			else
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<UISprite>().atlas.spriteMaterial.mainTexture = DefaultActionBarAtlas;
			}
		}

		[NewMember]
		public  static void ResetPortraitHighlight(bool state)
		{
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(10).gameObject.SetActive(state);

			for (int i = 3; i < UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).childCount; i++)
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(i).GetChild(11).gameObject.SetActive(state);
			}
		}

		[NewMember]
		public  static void TogglePortraitHighlight(GameObject go)
		{
			if (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(10).gameObject.activeSelf)
				ResetPortraitHighlight(false);
			else
				ResetPortraitHighlight(true);
		}

		[NewMember]
		public  static void ExtractMemorials()
		{
			string xmlPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod"), "MemorialEntries.xml");
			string xml = Resources.Load("Data/UI/BackerMemorials").ToString();

			File.WriteAllText(xmlPath, xml);

			global::Console.AddMessage("Extraction: done.");
		}

		[NewMember]
		public  static void CC()
		{
			GameState.s_playerCharacter.GetComponent<Mover>().UseWalkSpeed();
		}

		[NewMember]
		public  static void SetDefaultZoom(float value)
		{
			PlayerPrefs.SetFloat("DefaultZoom", value);
			global::Console.AddMessage("Default zoom set to: " + value + ". Reminder: game's vanilla value is 1.");
		}

		[NewMember]
		public  static void DisableBackerDialogues(bool state)
		{
			if (state)
			{
				PlayerPrefs.SetInt("DisableBackerDialogues", 1);
				global::Console.AddMessage("If you're using the \"Rename backers\" mod, backer dialogues will now be DISABLED as soon as you transition to another area or reload a save.");
			}
			else
			{
				PlayerPrefs.SetInt("DisableBackerDialogues", 0);
				global::Console.AddMessage("Backer dialogues will now be ENABLED as soon as you transition to another area or reload a save.");
			}
		}

		[NewMember]
		public  static void TS()
		{
			Vector3 test = new Vector3() { x = 20f };
			Vector3 second = new Vector3() { x = 30f };
			//test.x = 20;
			Debug.DrawLine(test, second);
		}

		[NewMember]
		public  static void DD()
		{
			QualitySettings.IncreaseLevel();
		}

		[NewMember]
		public  static void CheckAchievements()
		{
			if (AchievementTracker.Instance.DisableAchievements == true)
			{
				global::Console.AddMessage("Your achievements were previously disabled for this playthrough.", Color.red);
				global::Console.AddMessage("To reactivate them, type: ReenableAchievements");
			}
			else
				global::Console.AddMessage("Your achievements are doing fine.", Color.green);
		}

		[NewMember]
		public  static void AssignClericalGod(string charname, string godname)
		{
			charname = charname.Replace("_", " ");

			GameObject npc = null;

			foreach (var partymember in PartyMemberAI.PartyMembers)
			{
				if (partymember != null && RemoveDiacritics(partymember.gameObject.GetComponent<CharacterStats>().Name()).Contains(charname))
					npc = partymember.gameObject;
			}

			if (npc != null)
			{
				bool goOn = false;
				try
				{
					if (Enum.Parse(typeof(global::Religion.Deity), godname) != null)
						goOn = true;
				}
				catch
				{
					global::Console.AddMessage(godname + " - not found as a Deity.");
				}
				if (goOn)
				{
					object newclassobj = Enum.Parse(typeof(global::Religion.Deity), godname);
					int newGodId = Convert.ToInt32(newclassobj);

					npc.GetComponent<CharacterStats>().Deity = (global::Religion.Deity)newGodId;

					global::Console.AddMessage("Deity assigned.", Color.green);
				}
			}
		}

		[NewMember]
		public  static void AssignPaladinOrder(string charname, string ordername)
		{
			charname = charname.Replace("_", " ");

			GameObject npc = null;

			foreach (var partymember in PartyMemberAI.PartyMembers)
			{
				if (partymember != null && RemoveDiacritics(partymember.gameObject.GetComponent<CharacterStats>().Name()).Contains(charname))
					npc = partymember.gameObject;
			}

			if (npc != null)
			{
				bool goOn = false;
				try
				{
					if (Enum.Parse(typeof(global::Religion.PaladinOrder), ordername) != null)
						goOn = true;
				}
				catch
				{
					global::Console.AddMessage(ordername + " - not found as a Palandin Order.");
				}
				if (goOn)
				{
					object newclassobj = Enum.Parse(typeof(global::Religion.PaladinOrder), ordername);
					int newOrderId = Convert.ToInt32(newclassobj);

					npc.GetComponent<CharacterStats>().PaladinOrder = (global::Religion.PaladinOrder)newOrderId;

					global::Console.AddMessage("Paladin order assigned.", Color.green);
				}
			}
		}

		//code by Michael Kaplan
		[NewMember]
		static string RemoveDiacritics(string text)
		{
			var normalizedString = text.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			foreach (var c in normalizedString)
			{
				var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}

		[NewMember]
		public  static void IERemove(string charname, string abilname)
		{

			charname = charname.Replace("_", " ");

			GameObject npc = null;

			foreach (var partymember in PartyMemberAI.PartyMembers)
			{
				if (partymember != null && RemoveDiacritics(partymember.gameObject.GetComponent<CharacterStats>().Name()).Contains(charname))
					npc = partymember.gameObject;
			}

			if (npc != null)
			{
				bool removedSomething = false;

				CharacterStats stats = npc.GetComponent<CharacterStats>();
				for (int i = stats.ActiveTalents.Count - 1; i > -1; i--)
				{
					if (stats.ActiveTalents[i].gameObject.name.Contains(abilname))
					{
						global::Console.AddMessage("Removed active talent: " + stats.ActiveTalents[i].gameObject.name);
						AbilityProgressionTable.RemoveAbilityFromCharacter(stats.ActiveTalents[i].gameObject, stats);
						removedSomething = true;
						break;
					}
				}

				for (int i = stats.Talents.Count - 1; i > -1; i--)
				{
					if (stats.Talents[i].gameObject.name.Contains(abilname))
					{
						global::Console.AddMessage("Removed talent: " + stats.Talents[i].gameObject.name);
						AbilityProgressionTable.RemoveAbilityFromCharacter(stats.Talents[i].gameObject, stats);
						removedSomething = true;
						break;
					}
				}

				for (int i = stats.ActiveAbilities.Count - 1; i > -1; i--)
				{
					if (stats.ActiveAbilities[i].gameObject.name.Contains(abilname))
					{
						global::Console.AddMessage("Removed active ability: " + stats.ActiveAbilities[i].gameObject.name);
						stats.ActiveAbilities[i].ForceDeactivate(npc);
						AbilityProgressionTable.RemoveAbilityFromCharacter(stats.ActiveAbilities[i].gameObject, stats);
						removedSomething = true;
						break;
					}
				}

				for (int i = stats.Abilities.Count - 1; i > -1; i--)
				{
					if (stats.Abilities[i].gameObject.name.Contains(abilname))
					{
						global::Console.AddMessage("Removed ability: " + stats.Abilities[i].gameObject.name);
						stats.Abilities[i].ForceDeactivate(npc);
						AbilityProgressionTable.RemoveAbilityFromCharacter(stats.Abilities[i].gameObject, stats);
						removedSomething = true;
						break;
					}
				}

				if (!removedSomething)
				{
					global::Console.AddMessage("Nothing was removed. Talent wasn't found.");
				}

			}
			else
				global::Console.AddMessage("Party memeber not found.");
		}

		[NewMember]
		public  static void SwitchPOTD()
		{
			if (GameState.Instance.Difficulty != GameDifficulty.PathOfTheDamned)
			{
				GameState.Instance.Difficulty = GameDifficulty.PathOfTheDamned;
				global::Console.AddMessage("The difficulty is now: Path of the Damned.");
			}
			else
			{
				GameState.Instance.Difficulty = GameDifficulty.Hard;
				global::Console.AddMessage("The difficulty is now: Hard.");
			}
		}

		[NewMember]
		public  static void ReenableAchievements()
		{
			if (AchievementTracker.Instance.DisableAchievements == true)
			{
				AchievementTracker.Instance.DisableAchievements = false;
				global::Console.AddMessage("Achievements have been reenabled for this playthrough.", Color.green);
			}
			else
				global::Console.AddMessage("Nothing was done, since achievements were already active.");
		}

		[NewMember]
		public  static void FixSagani(string guid)
		{
			GameObject sagani = UnityEngine.GameObject.Find(guid);
			if (sagani != null)
			{
				for (int i = sagani.GetComponent<CharacterStats>().ActiveAbilities.Count - 1; i > -1; i--)
				{
					if (sagani.GetComponent<CharacterStats>().ActiveAbilities[i].gameObject.name.Contains("SummonCompanion") && !sagani.GetComponent<CharacterStats>().ActiveAbilities[i].gameObject.name.Contains("ArcticFox"))
					{
						sagani.GetComponent<CharacterStats>().ActiveAbilities[i].ForceDeactivate(sagani);
						AbilityProgressionTable.RemoveAbilityFromCharacter(sagani.GetComponent<CharacterStats>().ActiveAbilities[i].gameObject, sagani.GetComponent<CharacterStats>());
					}
				}
			}
			else
				global::Console.AddMessage("Character not found.");
		}

		[NewMember]
		public  static void ChangeClass(string guid, string charclass)
		{
			GameObject npc = null;
			string charname = guid.Replace("_", " ");
			foreach (var partymember in PartyMemberAI.PartyMembers)
			{
				if (partymember != null && RemoveDiacritics(partymember.gameObject.GetComponent<CharacterStats>().Name()).Contains(charname))
					npc = partymember.gameObject;
			}
			if (npc == null)
				npc = UnityEngine.GameObject.Find(guid);
			if (npc != null)
			{
				bool goOn = false;
				try
				{
					if (Enum.Parse(typeof(CharacterStats.Class), charclass) != null)
						goOn = true;
				}
				catch
				{
					global::Console.AddMessage(charclass + " - not found as a class.");
				}
				if (goOn)
				{
					object newclassobj = Enum.Parse(typeof(CharacterStats.Class), charclass);
					int newclassId = Convert.ToInt32(newclassobj);

					List<string> Innates = new List<string>();

					//Put all innate non-racial talents and abilities here (case insensitive):
					Innates.Add("crucible_of_the_soul");
					Innates.Add("armed_to_the_teeth");
					Innates.Add("speaker_to_the_restless");
					Innates.Add("dominion_of_the_sleepers");
					Innates.Add("steps_to_the_wheel");
					Innates.Add("Beraths_Boon");
					Innates.Add("Hyleas_Boon");
					Innates.Add("Waels_Boon");
					Innates.Add("Galawains_Boon");
					Innates.Add("Rymrgands_Boon");
					Innates.Add("Skaens_Boon");
					Innates.Add("Second_Skin");
					Innates.Add("The_Merciless_Hand");
					Innates.Add("Mob_Justice");
					Innates.Add("Mob Justice"); //the ability has a space in place of an underscore...
					Innates.Add("Blooded_Hunter");
					Innates.Add("Song_of_the_Heavens");
					Innates.Add("Wild_Running");
					Innates.Add("Dungeon_Delver");
					Innates.Add("Scale-Breaker");
					Innates.Add("Gift_from_the_Machine");
					Innates.Add("Effigys_Resentment"); //should work for all types

					if (npc.GetComponent<CharacterStats>().name.Contains("Sagani"))
					{
						Innates.Add("SummonCompanionArcticFox");
					}

					//==========================================================================
					//REMOVE TALENTS
					//==========================================================================
					List<GenericTalent> talentRemoveList = new List<GenericTalent>();
					foreach (GenericTalent activeTalent in npc.GetComponent<CharacterStats>().ActiveTalents)
					{
						bool saveMe = false;
						foreach (string innate in Innates)
						{
							if (activeTalent.gameObject.name.IndexOf(innate, StringComparison.OrdinalIgnoreCase) >= 0) //look for substring
							{
								saveMe = true;
								break;
							}
						}
						if (!saveMe)
							talentRemoveList.Add(activeTalent);
					}
					foreach (GenericTalent talentToRemove in talentRemoveList)
						AbilityProgressionTable.RemoveAbilityFromCharacter(talentToRemove.gameObject, npc.GetComponent<CharacterStats>());

					talentRemoveList.Clear();
					foreach (GenericTalent talent in npc.GetComponent<CharacterStats>().Talents)
					{
						bool saveMe = false;
						foreach (string innate in Innates)
						{
							if (talent.gameObject.name.IndexOf(innate, StringComparison.OrdinalIgnoreCase) >= 0) //look for substring
							{
								saveMe = true;
								break;
							}
						}
						if (!saveMe)
							talentRemoveList.Add(talent);
					}
					foreach (GenericTalent talentToRemove in talentRemoveList)
						AbilityProgressionTable.RemoveAbilityFromCharacter(talentToRemove.gameObject, npc.GetComponent<CharacterStats>());
					//==========================================================================

					//==========================================================================
					//REMOVE ABILITIES
					//==========================================================================
					List<GenericAbility> abilRemoveList = new List<GenericAbility>();
					foreach (GenericAbility activeAbility in npc.GetComponent<CharacterStats>().ActiveAbilities)
					{
						if (activeAbility.EffectType == GenericAbility.AbilityType.Racial)
							continue;
						bool saveMe = false;
						foreach (string innate in Innates)
						{
							if (activeAbility.gameObject.name.IndexOf(innate, StringComparison.OrdinalIgnoreCase) >= 0) //look for substring
							{
								saveMe = true;
								break;
							}
						}
						if (!saveMe)
							abilRemoveList.Add(activeAbility);
					}
					foreach (GenericAbility abilToRemove in abilRemoveList)
					{
						abilToRemove.ForceDeactivate(npc);
						AbilityProgressionTable.RemoveAbilityFromCharacter(abilToRemove.gameObject, npc.GetComponent<CharacterStats>());
					}
					abilRemoveList.Clear();
					foreach (GenericAbility ability in npc.GetComponent<CharacterStats>().Abilities)
					{
						if (ability.EffectType == GenericAbility.AbilityType.Racial)
							continue;
						bool saveMe = false;
						foreach (string innate in Innates)
						{
							if (ability.gameObject.name.IndexOf(innate, StringComparison.OrdinalIgnoreCase) >= 0) //look for substring
							{
								saveMe = true;
								break;
							}
						}
						if (!saveMe)
							abilRemoveList.Add(ability);
					}
					foreach (GenericAbility abilToRemove in abilRemoveList)
					{
						abilToRemove.ForceDeactivate(npc);
						AbilityProgressionTable.RemoveAbilityFromCharacter(abilToRemove.gameObject, npc.GetComponent<CharacterStats>());
					}
					//==========================================================================

					// remove ranger's pet
					if (npc.GetComponent<CharacterStats>().CharacterClass == CharacterStats.Class.Ranger && !npc.GetComponent<CharacterStats>().name.Contains("Sagani"))
					{
						foreach (var cre in npc.GetComponent<AIController>().SummonedCreatureList)
						{
							if (GameUtilities.IsAnimalCompanion(cre.gameObject))
							{
								PartyMemberAI.RemoveFromActiveParty(cre.GetComponent<PartyMemberAI>(), true);
								cre.GetComponent<Persistence>().UnloadsBetweenLevels = true;
								cre.GetComponent<Health>().m_isAnimalCompanion = false;
								cre.GetComponent<Health>().ApplyDamageDirectly(1000);
								cre.GetComponent<Health>().ApplyDamageDirectly(1000);
								global::Console.AddMessage(cre.GetComponent<CharacterStats>().Name() + " is free from its bonds and returns to the wilds to be with its own kind.", Color.green);
								cre.SetActive(false);
							}
						}
						//npc.GetComponent<AIController> ().SummonedCreatureList.Clear ();
					}

					// remove or give grimoire
					if (npc.GetComponent<CharacterStats>().CharacterClass != (CharacterStats.Class)newclassId)
					{
						if (npc.GetComponent<CharacterStats>().CharacterClass == CharacterStats.Class.Wizard)
						{
							npc.GetComponent<Equipment>().UnEquip(Equippable.EquipmentSlot.Grimoire);
						}

						npc.GetComponent<CharacterStats>().CharacterClass = (CharacterStats.Class)newclassId;

						if (npc.GetComponent<CharacterStats>().CharacterClass == CharacterStats.Class.Wizard)
						{
							// equip an empty grimoire...?
							Equippable grim = GameResources.LoadPrefab<Equippable>("empty_grimoire_01", true);
							if (grim != null)
							{
								grim.GetComponent<Grimoire>().PrimaryOwnerName = npc.GetComponent<CharacterStats>().Name();
								npc.GetComponent<Equipment>().Equip(grim);
							}
						}
					}

					//BaseDeflection,BaseFortitude,BaseReflexes,BaseWill,MeleeAccuracyBonus,RangedAccuracyBonus,MaxHealth,MaxStamina,HealthStaminaPerLevel,ClassHealthMultiplier
					object comp = (object)npc.GetComponent<CharacterStats>();
					DataManager.AdjustFromData(ref comp);

					npc.GetComponent<CharacterStats>().Level = 0;

					npc.GetComponent<CharacterStats>().StealthSkill = 0;
					npc.GetComponent<CharacterStats>().StealthBonus = 0;
					npc.GetComponent<CharacterStats>().AthleticsSkill = 0;
					npc.GetComponent<CharacterStats>().AthleticsBonus = 0;
					npc.GetComponent<CharacterStats>().LoreSkill = 0;
					npc.GetComponent<CharacterStats>().LoreBonus = 0;
					npc.GetComponent<CharacterStats>().MechanicsSkill = 0;
					npc.GetComponent<CharacterStats>().MechanicsBonus = 0;
					npc.GetComponent<CharacterStats>().SurvivalSkill = 0;
					npc.GetComponent<CharacterStats>().SurvivalBonus = 0;

					npc.GetComponent<CharacterStats>().RemainingSkillPoints = 0;

					string HeOrShe = npc.GetComponent<CharacterStats>().Gender.ToString();
					global::Console.AddMessage(npc.GetComponent<CharacterStats>().Name() + " has reformed into a " + charclass + ". " + (HeOrShe == "Male" ? "He" : "She") + " lost all " + (HeOrShe == "Male" ? "his" : "her") + " previous abilities and talents.", Color.green);
				}
			}
			else
				global::Console.AddMessage("Couldn't find: " + guid, Color.yellow);
		}

		[ModifiesMember("Skill")]
		public  static void SkillNew(string name, string skillScore, string value)
		{
			int num;
			if (!int.TryParse(value, out num))
			{
				Debug.Log("Skill: ERROR - value not in integer format.");
			}
			else
			{
				CharacterStats component = null;
				if (name.ToLower() == "player")
				{
					if (GameState.s_playerCharacter == null)
					{
						Debug.Log("Skill: Error - player character not found.");
						return;
					}
					component = GameState.s_playerCharacter.GetComponent<CharacterStats>();
				}
				else
				{
					CharacterStats[] statsArray = UnityEngine.Object.FindObjectsOfType<CharacterStats>();
					for (int i = 0; i < statsArray.Length; i++)
					{
						if (statsArray[i].gameObject.name.ToLower().Contains(name.ToLower()))
						{
							component = statsArray[i];
							break;
						}
					}
				}
				if (component == null)
				{
					Debug.Log("Skill: Error - stats component not found for " + name);
				}
				else
				{
					skillScore = skillScore.ToUpper();
					int length = Enum.GetNames(typeof(CharacterStats.SkillType)).Length;
					for (int j = 0; j < length; j++)
					{
						if (!(skillScore == Enum.GetNames(typeof(CharacterStats.SkillType))[j].ToUpper()))
						{
							continue;
						}
						switch (((CharacterStats.SkillType)j))
						{
							case CharacterStats.SkillType.Stealth:
								component.StealthSkill = num;
								break;

							case CharacterStats.SkillType.Athletics:
								component.AthleticsSkill = num;
								break;

							case CharacterStats.SkillType.Lore:
								component.LoreSkill = num;
								break;

							case CharacterStats.SkillType.Mechanics:
								component.MechanicsSkill = num;
								break;

							case CharacterStats.SkillType.Survival:
								component.SurvivalSkill = num;
								break;

							case CharacterStats.SkillType.Crafting:
								component.CraftingSkill = num;
								break;
						}
						global::Console.AddMessage(name + "'s " + skillScore + " is now " + value.ToString());
						return;
					}
					Debug.Log("Skill Score: Error - could not find skill " + skillScore);
				}
			}
		}

		[NewMember]
		public  static void RenameCreature(string guid, string newname)
		{
			GameObject npc = UnityEngine.GameObject.Find(guid);
			if (npc != null && npc.GetComponent<CharacterStats>() != null)
				npc.GetComponent<CharacterStats>().OverrideName = newname;
		}

		[NewMember]
		public  static void AssignRandomName(int seed, ref string[] names, CharacterStats backer)
		{
			UnityEngine.Random.seed = seed;
			int randomId = 0;
			for (int i = 0; i < names.Length - 1; i++)
			{
				if (UnityEngine.Random.value > 0.5f) // UnityEngine.Random.value is between 0.0 and 1.0, it's based on Random.Seed
					randomId++;
			}
			backer.OverrideName = names[randomId];
			//Console.AddMessage ("Using seed " + seed + ", " + backer.DisplayName.ToString () + " rolled: " + randomId);
		}

		[NewMember]
		public  static void FixBackerNames(bool state)
		{
			// finding all objects
			GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

			// finding all backer npcs
			List<GameObject> allBackers = new List<GameObject>();
			for (int i = allObjects.Length - 1; i > 0; i--)
			{
				if ((allObjects[i].name.StartsWith("NPC_BACKER") || allObjects[i].name.StartsWith("NPC_Visceris")) && allObjects[i].GetComponent<CharacterStats>() != null)
				{
					allBackers.Add(allObjects[i]);
				}
			}

			if (state)
			{
				string humanMalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "human-males.txt");
				string humanFemalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "human-females.txt");

				string dwarfMalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "dwarf-males.txt");
				string dwarfFemalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "dwarf-females.txt");

				string elfMalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "elf-males.txt");
				string elfFemalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "elf-females.txt");

				string orlanMalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "orlan-males.txt");
				string orlanFemalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "orlan-females.txt");

				string aumauaMalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "aumaua-males.txt");
				string aumauaFemalesPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/names"), "aumaua-females.txt");


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
					&& File.Exists(aumauaFemalesPath))
				{
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

					for (int z = 0; z < allBackers.Count; z++)
					{
						// disabling dialogues for backers
						if (PlayerPrefs.GetInt("DisableBackerDialogues", 0) == 1)
							GameUtilities.Destroy(allBackers[z].GetComponent<NPCDialogue>());

						CharacterStats backer = allBackers[z].GetComponent<CharacterStats>();

						if (backer != null)
						{
							string originalName = backer.DisplayName.ToString();
							int seedForThisNpc = originalName.GetHashCode();
							if (backer.RacialBodyType == CharacterStats.Race.Human)
							{
								if (backer.Gender == Gender.Male)
									AssignRandomName(seedForThisNpc, ref humanMaleNames, backer);
								else if (backer.Gender == Gender.Female)
									AssignRandomName(seedForThisNpc, ref humanFemaleNames, backer);
							}
							else if (backer.RacialBodyType == CharacterStats.Race.Dwarf)
							{
								if (backer.Gender == Gender.Male)
									AssignRandomName(seedForThisNpc, ref dwarfMaleNames, backer);
								else if (backer.Gender == Gender.Female)
									AssignRandomName(seedForThisNpc, ref dwarfFemaleNames, backer);
							}
							else if (backer.RacialBodyType == CharacterStats.Race.Elf)
							{
								if (backer.Gender == Gender.Male)
									AssignRandomName(seedForThisNpc, ref elfMaleNames, backer);
								else if (backer.Gender == Gender.Female)
									AssignRandomName(seedForThisNpc, ref elfFemaleNames, backer);
							}
							else if (backer.RacialBodyType == CharacterStats.Race.Aumaua)
							{
								if (backer.Gender == Gender.Male)
									AssignRandomName(seedForThisNpc, ref aumauaMaleNames, backer);
								else if (backer.Gender == Gender.Female)
									AssignRandomName(seedForThisNpc, ref aumauaFemaleNames, backer);
							}
							else if (backer.RacialBodyType == CharacterStats.Race.Orlan)
							{
								if (backer.Gender == Gender.Male)
									AssignRandomName(seedForThisNpc, ref orlanMaleNames, backer);
								else if (backer.Gender == Gender.Female)
									AssignRandomName(seedForThisNpc, ref orlanFemaleNames, backer);
							}
						}
					}
				}
				else
				{
					global::Console.AddMessage("IEMod: At least one of the .txt files with backer names is missing at path: " + Path.Combine(Application.dataPath, "Managed/iemod/names"), Color.red);
				}
			}
			else
			{
				for (int z = 0; z < allBackers.Count; z++)
				{
					GameObject npc = allBackers[z];

					CharacterStats backer = npc.GetComponent<CharacterStats>();
					if (backer != null)
					{
						string originalName = backer.DisplayName.ToString();
						if (backer.OverrideName != "" && backer.OverrideName != originalName)
						{
							backer.OverrideName = originalName;
						}
					}
				}
			}
		}

		[NewMember]
		public  static void DisableTooltipOffset(bool state)
		{
			if (state)
			{
				UIOptionsManager.Instance.transform.parent.GetChild(5).GetChild(2).GetChild(1).GetComponent<UIAnchor>().pixelOffset = new Vector2(25f, -25f);
				PlayerPrefs.SetInt("DisableTooltipOffset", 1);
			}
			else
			{
				PlayerPrefs.SetInt("DisableTooltipOffset", 0);
				if (PlayerPrefs.GetString("Uframe", "FRM_botCornerLft") != "FRM_botCornerLft")
					UIOptionsManager.Instance.transform.parent.GetChild(5).GetChild(2).GetChild(1).GetComponent<UIAnchor>().pixelOffset = new Vector2(150f, -25f);
			}
		}

		[NewMember]
		public  static void DisableRedCursor(bool state)
		{
			if (state)
				PlayerPrefs.SetInt("DisableRedCursor", 1);
			else
				PlayerPrefs.SetInt("DisableRedCursor", 0);
		}

		[NewMember]
		public  static void ToggleButtonsBackground()
		{
			bool toSet = true;
			if (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
				toSet = false;

			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(toSet);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(toSet);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(2).GetChild(1).gameObject.SetActive(toSet);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(3).GetChild(1).gameObject.SetActive(toSet);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(4).GetChild(1).gameObject.SetActive(toSet);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(5).GetChild(1).gameObject.SetActive(toSet);

			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(toSet);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(toSet);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetChild(2).GetChild(1).gameObject.SetActive(toSet);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetChild(3).GetChild(1).gameObject.SetActive(toSet);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetChild(4).GetChild(2).gameObject.SetActive(toSet);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetChild(5).GetChild(1).gameObject.SetActive(toSet);
		}

		[NewMember]
		public  static void ToggleCustomTextures(GameObject go)
		{
			if (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<UISprite>().atlas.spriteMaterial.mainTexture == DefaultActionBarAtlas)
			{
				ReplaceAtlas(true);
			}
			else
			{
				ReplaceAtlas(false);
			}
		}

		[NewMember]
		public  static void ButtonsBackground(GameObject go)
		{
			ToggleButtonsBackground();
		}

		[NewMember]
		public  static void DropInventory(GameObject go)
		{
			PartyMemberAI selChar = UIInventoryManager.Instance.SelectedCharacter.gameObject.GetComponent<PartyMemberAI>();

			GameObject dropObject = null;
			if (dropObject == null)
			{
				Container container = GameResources.LoadPrefab<Container>("DefaultDropItem", false);

				//assign another mesh, less ugly, less giant, less looking like a fucking 3 year old baby? ugh..................

				dropObject = GameResources.Instantiate<GameObject>(container.gameObject, selChar.gameObject.transform.position, Quaternion.identity);

				dropObject.tag = "DropItem";
				dropObject.layer = LayerUtility.FindLayerValue("Dynamics");
				dropObject.transform.localRotation = Quaternion.AngleAxis(UnityEngine.Random.Range((float)0f, (float)360f), Vector3.up);
				InstanceID eid = dropObject.GetComponent<InstanceID>();
				eid.Guid = Guid.NewGuid();
				eid.UniqueID = eid.Guid.ToString();
				dropObject.GetComponent<Persistence>().TemplateOnly = true;
			}
			Container component = dropObject.GetComponent<Container>();
			component.ManualLabelName = "Drop Items";
			if (component != null)
			{
				component.DeleteMeIfEmpty = true;
			}
			AlphaControl control = dropObject.AddComponent<AlphaControl>();
			if (control != null)
			{
				control.Alpha = 0f;
				control.FadeIn(1f);
			}

			// opening created container... code taken from Container.Open()
			UILootManager.Instance.SetData(selChar, dropObject.GetComponent<Inventory>(), dropObject.gameObject);
			UILootManager.Instance.ShowWindow();
			dropObject.GetComponent<Inventory>().CloseInventoryCB = new BaseInventory.closeInventoryDelegate(component.CloseInventoryCB);
		}



		[NewMember]
		public  static void InjectDropInvButton()
		{
			if (UIInventoryManager.Instance.transform.GetChild(0).GetChild(9).GetChild(3).childCount < 5) // check to do it only once
			{
				GameObject craftButton = UIInventoryManager.Instance.transform.GetChild(0).GetChild(9).GetChild(3).GetChild(3).gameObject;
				craftButton.transform.localPosition += new Vector3(0, 25, 0);
				craftButton.transform.localScale = new Vector3(0.82f, 0.82f, 0.82f);

				GameObject myButton = GameObject.Instantiate(craftButton) as GameObject;
				myButton.transform.parent = craftButton.transform.parent;
				myButton.name = "DropItems";
				myButton.transform.localScale = craftButton.transform.localScale;
				myButton.transform.localPosition = craftButton.transform.localPosition;
				myButton.transform.localPosition += new Vector3(0, -43, 0);
				myButton.GetComponent<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Drop Items";
				myButton.GetComponent<UIMultiSpriteImageButton>().Label.multiLine = false;
				myButton.GetComponent<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				myButton.GetComponent<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(DropInventory);
			}
		}

		// makes it possible to use some console commands without having to type in "IRoll20s" first
		[ModifiesMember("RunCommand")]
		public  static void RunCommandNew(string command)
		{
			if (!string.IsNullOrEmpty(command) && (command.ToLower() != "runcommand"))
			{
				IEnumerable<System.Reflection.MethodInfo> methods;
				List<string> list = new List<string>();
				char[] separator = new char[] { ' ' };
				list.AddRange(command.Split(separator));

				methods = typeof(Scripts).GetMethods().Concat<System.Reflection.MethodInfo>(typeof(CommandLine).GetMethods());

				IEnumerator<System.Reflection.MethodInfo> enumerator = methods.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						System.Reflection.MethodInfo current = enumerator.Current;
						bool alwaysCheat = true; // added this line and put it in the if below
						if (((current.GetCustomAttributes(typeof(CheatAttribute), true).Length <= 0) || alwaysCheat) && (string.Compare(current.Name, list[0], true) == 0))
						{
							ParameterInfo[] parameters = current.GetParameters();
							if ((parameters != null) && (parameters.Length == (list.Count - 1)))
							{
								object[] objArray = new object[parameters.Length];
								for (int i = 0; i < parameters.Length; i++)
								{
									Type attributeType = (Type)s_ScriptAttributeTypes[i];
									object[] customAttributes = current.GetCustomAttributes(attributeType, false);
									Scripts.BrowserType none = Scripts.BrowserType.None;
									if (customAttributes.Length > 0)
									{
										none = (Scripts.BrowserType)((int)attributeType.GetProperty("Browser").GetValue(customAttributes[0], null));
									}
									ParameterInfo info2 = parameters[i];
									if (info2.ParameterType.IsEnum)
									{
										try
										{
											objArray[i] = Enum.Parse(info2.ParameterType, list[i + 1]);
										}
										catch
										{
											objArray[i] = Enum.GetValues(info2.ParameterType).GetValue(0);
										}
									}
									else if ((none == Scripts.BrowserType.ObjectGuid) || (info2.ParameterType == typeof(Guid)))
									{
										try
										{
											objArray[i] = new Guid(list[i + 1]);
										}
										catch
										{
											GameObject obj2 = GameObject.Find(list[i + 1]);
											if (obj2 != null)
											{
												InstanceID component = obj2.GetComponent<InstanceID>();
												if (component != null)
												{
													objArray[i] = component.Guid;
												}
												else
												{
													objArray[i] = Guid.Empty;
												}
											}
											else
											{
												objArray[i] = Guid.Empty;
											}
										}
									}
									else if (none == Scripts.BrowserType.Quest)
									{
										List<string> list2 = QuestManager.Instance.FindLoadedQuests(list[i + 1]);
										if (list2.Count != 0)
										{
											if (list2.Count != 1)
											{
												global::Console.AddMessage("Multiple quests matched search '" + list[i + 1] + "':", Color.yellow);
												foreach (string str in list2)
												{
													global::Console.AddMessage(Path.GetFileNameWithoutExtension(str), Color.yellow);
												}
												return;
											}
											objArray[i] = list2[0];
										}
										else
										{
											objArray[i] = list[i + 1];
										}
									}
									else if (none == Scripts.BrowserType.Conversation)
									{
										List<string> list3 = ConversationManager.Instance.FindConversations(list[i + 1]);
										if (list3.Count != 0)
										{
											if (list3.Count != 1)
											{
												global::Console.AddMessage("Multiple conversations matched search '" + list[i + 1] + "':", Color.yellow);
												foreach (string str2 in list3)
												{
													global::Console.AddMessage(Path.GetFileNameWithoutExtension(str2), Color.yellow);
												}
												return;
											}
											objArray[i] = list3[0];
										}
										else
										{
											objArray[i] = list[i + 1];
										}
									}
									else if (info2.ParameterType.IsValueType)
									{
										try
										{
											objArray[i] = Convert.ChangeType(list[i + 1], info2.ParameterType, CultureInfo.InvariantCulture);
										}
										catch (Exception)
										{
											global::Console.AddMessage(string.Concat(new object[] { "Script error: could not convert parameter ", i + 1, " ('", list[i], "') into type ", info2.ParameterType.Name, ". " }), Color.yellow);
											return;
										}
									}
									else
									{
										objArray[i] = list[i + 1];
									}
								}
								current.Invoke(null, objArray);
								return;
							}
						}
					}
				}
				finally
				{
					if (enumerator == null)
					{
					}
					else
						enumerator.Dispose(); // seems there was an "else" lacking here
				}
				global::Console.AddMessage(string.Concat(new object[] { "No command or script '", list[0], "' accepting ", list.Count - 1, " parameters exists." }), Color.yellow);
			}
		}



		[NewMember]
		public  static void ListActiveQuests()
		{
			foreach (var quest in QuestManager.Instance.ActiveQuests)
				global::Console.AddMessage(quest.Key);
		}

		[NewMember]
		public  static void ForceAdvanceQuest(string name)
		{
			QuestManager.Instance.AdvanceQuest(name, true);
		}

		[NewMember]
		public  static void OpenContainer(string objectGuid)
		{
			GameObject container = GameObject.Find(objectGuid);
			if (container != null)
			{
				Container chest = container.GetComponent<Container>();
				if (chest != null)
					chest.Open(GameState.s_playerCharacter.gameObject, true);
				else
				{
					global::Console.AddMessage("Object is not a container.");
				}
				//oCLComponent.SealOpen ();
			}
			else
			{
				global::Console.AddMessage("Container not found");
			}
		}

		[NewMember]
		public  static void ShowAIState()
		{
			if (GameCursor.CharacterUnderCursor)
			{
				var ai = GameCursor.CharacterUnderCursor.GetComponent<AIController>();
				if (ai)
				{
					var stateManager = ai.StateManager;
					var str = new System.Text.StringBuilder();
					stateManager.BuildDebugText(str);
					global::Console.AddMessage(str.ToString());
				}
				else
				{
					global::Console.AddMessage("no AI available");
				}
			}
			else
			{
				global::Console.AddMessage("no one under the cursor");
			}
		}

		[NewMember]
		public  static void Jump()
		{
			if (GameState.s_playerCharacter.IsMouseOnWalkMesh())
			{
				foreach (var partymember in PartyMemberAI.GetSelectedPartyMembers())
				{
					partymember.transform.position = GameInput.WorldMousePosition;
				}
			}
			else
			{
				global::Console.AddMessage("Mouse is not on navmesh.");
			}
		}

		// spawns a creature... param1 is the creature, param2 is a bool: 0 for friendly, 1 for hostile
		// for instance: BSC cre_druid_cat01 1
		static void BSC(string param1, string param2)
		{
			if (GameState.s_playerCharacter.IsMouseOnWalkMesh())
			{
				if (param2 == "1")
					global::Console.AddMessage("Spawning a hostile: " + param1, UnityEngine.Color.green);
				else
					global::Console.AddMessage("Spawning a friendly: " + param1, UnityEngine.Color.green);
				UnityEngine.GameObject besterCreature;
				besterCreature = GameResources.LoadPrefab<UnityEngine.GameObject>(param1, true);
				if (besterCreature != null)
				{
					besterCreature.transform.position = GameInput.WorldMousePosition;
					besterCreature.transform.rotation = GameState.s_playerCharacter.transform.rotation;

					bool isHostile = false;
					if (param2 == "1")
						isHostile = true;

					if (isHostile == false)
					{
						if (besterCreature.GetComponent<Faction>() != null)
						{
							besterCreature.GetComponent<Faction>().RelationshipToPlayer = Faction.Relationship.Neutral;
							besterCreature.GetComponent<Faction>().CurrentTeamInstance = Team.GetTeamByTag("player");
							besterCreature.GetComponent<Faction>().UnitHostileToPlayer = false;
						}
						if (besterCreature.GetComponent<AIPackageController>() != null)
						{
							// some monsters might come without AI, which would make them always stand idly, so we give them DefaultAI
							besterCreature.GetComponent<AIPackageController>().ChangeBehavior(AIPackageController.PackageType.DefaultAI);
							besterCreature.GetComponent<AIPackageController>().InitAI();
						}
					}
					else
					{
						if (besterCreature.GetComponent<Faction>() != null)
						{
							besterCreature.GetComponent<Faction>().CurrentTeamInstance = Team.GetTeamByTag("monster");
							besterCreature.GetComponent<Faction>().RelationshipToPlayer = Faction.Relationship.Hostile;

						}
						if (besterCreature.GetComponent<AIPackageController>() != null)
						{
							besterCreature.GetComponent<AIPackageController>().ChangeBehavior(AIPackageController.PackageType.DefaultAI);
							besterCreature.GetComponent<AIPackageController>().InitAI();
						}
					}
					global::CameraControl.Instance.FocusOnPoint(besterCreature.transform.position);
				}
				else
					global::Console.AddMessage("Failed to spawn " + param1 + " - probably bad naming.", UnityEngine.Color.red);
			}
			else
				global::Console.AddMessage("Mouse is not on navmesh, move mouse elsewhere and try again.", UnityEngine.Color.red);
		}

		[NewMember]
		public  static void UseUframe(string uframeName)
		{

			if (PlayerPrefs.GetInt("DisableTooltipOffset", 0) != 1)
				UIOptionsManager.Instance.transform.parent.GetChild(5).GetChild(2).GetChild(1).GetComponent<UIAnchor>().pixelOffset = new Vector2(150f, -25f); // enemy tooltip in the left upper corner

			Texture2D SolidUFrame;

			string path = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/frames"), uframeName + "-" + Screen.width.ToString() + "x" + Screen.height.ToString() + ".png");
			string textpath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/frames"), uframeName + "-" + Screen.width.ToString() + "x" + Screen.height.ToString() + ".txt");

			float leftBarWidthPixels = 0f;
			float bottomBarHeightPixels = 0f;
			float rightBarPixels = 0f;

			float leftBarWidth = 0f;
			float bottomBarHeight = 0f;
			float rightBarWidth = 0f;

			if (File.Exists(textpath))
			{
				string[] threeLines = File.ReadAllLines(textpath);
				leftBarWidthPixels = (float)int.Parse(threeLines[0]);
				bottomBarHeightPixels = (float)int.Parse(threeLines[1]);
				rightBarPixels = (float)int.Parse(threeLines[2]);

				leftBarWidth = (leftBarWidthPixels * 2) / Screen.width;
				bottomBarHeight = (bottomBarHeightPixels * 2) / Screen.height;
				rightBarWidth = (rightBarPixels * 2) / Screen.width;
			}
			else
			{
				global::Console.AddMessage("Couldn't read file at path: " + textpath, Color.red);
			}



			if (File.Exists(path))
			{
				SolidUFrame = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
				byte[] bytes = File.ReadAllBytes(path);
				SolidUFrame.LoadImage(bytes);
				SolidUFrame.name = uframeName;
				//Console.AddMessage ("Texture loaded: "+uframeName);


				// Displaying U-frame
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(1).gameObject.SetActive(false);

				// we load it only once
				if (DefaultLeftCornerTexture == null)
					DefaultLeftCornerTexture = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetComponent<UITexture>().mainTexture;

				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetComponent<UITexture>().mainTexture = SolidUFrame;
				float width = Screen.width;
				float height = Screen.height;
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).transform.localScale = new Vector3(width, height, 1f);

				UIResolutionScaler scaler;
				if (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).gameObject.GetComponent<UIResolutionScaler>() == null)
					scaler = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).gameObject.AddComponent<UIResolutionScaler>();
				else
					scaler = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).gameObject.GetComponent<UIResolutionScaler>();
				scaler.DesignedWidth = (int)width;
				scaler.DesignedHeight = (int)height;
				scaler.UseMaximumScale = true;
				scaler.Apply();
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetComponent<UITexture>().MakePixelPerfect();
				// end of U-frame

				// destroying the 3 colliders?

				if (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).childCount != 0)
				{
					UnityEngine.Object.Destroy(UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetChild(2).gameObject);
					UnityEngine.Object.Destroy(UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetChild(1).gameObject);
					UnityEngine.Object.Destroy(UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetChild(0).gameObject);
				}


				// if we hadn't previously created 3 colliders, we create them, otherwise we just activate them

				// no-click collider for the left bar
				UITexture leftBarTexture = NGUITools.AddWidget<UITexture>(UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).gameObject);
				GameObject leftbar = leftBarTexture.gameObject;
				leftbar.transform.localScale = new Vector3(leftBarWidth, 2f, 1f); // i seriously have no idea why those values have to be used, i just guessed them after a few hours of trying... you'd think that you need to use (126, 1080, 1), but apperently not... WHY???
				leftBarTexture.mainTexture = new Texture2D((int)leftBarWidthPixels, Screen.height);
				BoxCollider box = NGUITools.AddWidgetCollider(leftbar.gameObject); // adding a box collider, it's required for the UINoClick component
				box.gameObject.AddComponent<UINoClick>(); // this prevents clicks from going through the U-frame
				UIAnchor ank = leftbar.gameObject.AddComponent<UIAnchor>();
				ank.side = UIAnchor.Side.BottomLeft;
				leftBarTexture.depth = 1;
				// end of left bar collider

				// no click collider for the right bar
				UITexture rightBarTexture = NGUITools.AddWidget<UITexture>(UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).gameObject);
				GameObject righttbar = rightBarTexture.gameObject;
				righttbar.transform.localScale = new Vector3(rightBarWidth, 2f, 1f); // i seriously have no idea why those values have to be used, i just guessed them after a few hours of trying... you'd think that you need to use (126, 1080, 1), but apperently not... WHY???
				rightBarTexture.mainTexture = new Texture2D((int)rightBarPixels, Screen.height);
				BoxCollider boxRight = NGUITools.AddWidgetCollider(righttbar.gameObject); // adding a box collider, it's required for the UINoClick component
				boxRight.gameObject.AddComponent<UINoClick>(); // this prevents clicks from going through the U-frame
				UIAnchor ankRight = righttbar.gameObject.AddComponent<UIAnchor>();
				ankRight.side = UIAnchor.Side.BottomRight;
				rightBarTexture.depth = 1;
				// end of right bar collider

				// no click collider for the bottom
				UITexture bottomBarTexture = NGUITools.AddWidget<UITexture>(UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).gameObject);
				GameObject bottombar = bottomBarTexture.gameObject;
				bottombar.transform.localScale = new Vector3(1f, bottomBarHeight, 1f); // i seriously have no idea why those values have to be used, i just guessed them after a few hours of trying... you'd think that you need to use (126, 1080, 1), but apperently not... WHY???
				bottomBarTexture.mainTexture = new Texture2D(Screen.width, (int)bottomBarHeightPixels);
				BoxCollider boxBottom = NGUITools.AddWidgetCollider(bottombar.gameObject); // adding a box collider, it's required for the UINoClick component
				boxBottom.gameObject.AddComponent<UINoClick>(); // this prevents clicks from going through the U-frame
				UIAnchor ankBottom = bottombar.gameObject.AddComponent<UIAnchor>();
				ankBottom.side = UIAnchor.Side.Bottom;
				bottomBarTexture.depth = 1;
				// end of bottom bar collider

			}
			else global::Console.AddMessage("Couldn't read file at path: " + path, Color.red);
		}

		[NewMember]
		public  static void UseNoFrame()
		{
			UIOptionsManager.Instance.transform.parent.GetChild(5).GetChild(2).GetChild(1).GetComponent<UIAnchor>().pixelOffset = new Vector2(25f, -25f);

			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(1).gameObject.SetActive(true);

			if (DefaultLeftCornerTexture != null)
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetComponent<UITexture>().mainTexture = DefaultLeftCornerTexture;

			if (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).childCount > 0)
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetChild(0).gameObject.SetActive(false);
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetChild(1).gameObject.SetActive(false);
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetChild(2).gameObject.SetActive(false);
			}

			if (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetComponent<UIResolutionScaler>() != null)
				UnityEngine.Object.Destroy(UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetComponent<UIResolutionScaler>());

			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).localScale = new Vector3(248f, 160f, 1);
		}
		// this method gives your maincharacter all existing mage spells... it was just to test something, but someone might want to use some bits of it
		[NewMember]
		public  static void AdAb()
		{
			CharacterStats firstparam = GameState.s_playerCharacter.GetComponent<CharacterStats>();
			AbilityProgressionTable wizardsProgressionTable = AbilityProgressionTable.LoadAbilityProgressionTable("Wizard");
			global::Console.AddMessage("Wizard abilities in game: " + wizardsProgressionTable.AbilityUnlocks.Length);
			global::Console.AddMessage("This wizard has abilities: " + GameState.s_playerCharacter.GetComponent<CharacterStats>().GetCopyOfCoreData().KnownSkills.Count());
			foreach (var abil in wizardsProgressionTable.AbilityUnlocks)
			{
				bool hasSpell = false;

				foreach (var spell in firstparam.GetCopyOfCoreData().KnownSkills)
					if (abil.Ability.name == spell.name.Replace("(Clone)", ""))
						hasSpell = true;

				if (hasSpell)
					global::Console.AddMessage("The wizard already knows: " + abil.Ability.name);
				else
					CommandLine.AddAbility(firstparam, abil.Ability.name);
			}
		}
		[ModifiesMember(".cctor")]
		private void ConstructorNew()
		{
			// added code
			modelViewerBackground = null;
			GameBrowserBackground = null;
			// end of added code
			s_ScriptAttributeTypes = new object[] { typeof(ScriptParam0Attribute), typeof(ScriptParam1Attribute), typeof(ScriptParam2Attribute), typeof(ScriptParam3Attribute), typeof(ScriptParam4Attribute), typeof(ScriptParam5Attribute), typeof(ScriptParam6Attribute), typeof(ScriptParam7Attribute) };
		}

		[NewMember]
		public  static void SelectCircles(float width)
		{
			global::Console.AddMessage("Setting selection circle width to: " + width, Color.green);
			InGameHUD.Instance.SelectionCircleWidth = width;
			InGameHUD.Instance.EngagedCircleWidth = width;
			PlayerPrefs.SetFloat("SelectCircWidth", width);
		}

		[NewMember]
		public  static void ToggleLogButtons(GameObject go) // toggles log buttons position between topright and topleft
		{
			if (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetChild(0).GetChild(0).GetComponent<UIAnchor>().side == UIAnchor.Side.TopRight)
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetChild(0).GetChild(0).GetComponent<UIAnchor>().side = UIAnchor.Side.TopLeft; // log icon moved to the left
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetChild(0).GetChild(0).GetChild(0).localPosition = new Vector3(144, 0, 0);
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetChild(0).GetChild(0).GetChild(1).localPosition = new Vector3(200, 0, 0);
			}
			else
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetChild(0).GetChild(0).GetComponent<UIAnchor>().side = UIAnchor.Side.TopRight; // log icon moved to the left
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetChild(0).GetChild(0).GetChild(0).localPosition = new Vector3(0, 0, 0);
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetChild(0).GetChild(0).GetChild(1).localPosition = new Vector3(-56, 0, 0);
			}
		}

		[NewMember]
		public  static void ToggleHudHoriz(GameObject go)
		{
			if (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetComponent<UIGrid>().arrangement == UIGrid.Arrangement.Vertical)
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetComponent<UIGrid>().arrangement = UIGrid.Arrangement.Horizontal;
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetComponent<UIGrid>().Reposition();

				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetComponent<UIGrid>().arrangement = UIGrid.Arrangement.Vertical;
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetComponent<UIGrid>().Reposition();
			}
			else
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetComponent<UIGrid>().arrangement = UIGrid.Arrangement.Vertical;
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetComponent<UIGrid>().Reposition();

				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetComponent<UIGrid>().arrangement = UIGrid.Arrangement.Horizontal;
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetComponent<UIGrid>().Reposition();
			}
		}

		[NewMember]
		public  static void HideHudBg(GameObject go)
		{
			if (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(3).gameObject.activeSelf)
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(3).gameObject.SetActive(false);
			}
			else
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(3).gameObject.SetActive(true);
			}
		}

		[NewMember]
		public  static void TogglePartyBar(GameObject go)
		{
			int toggled = PlayerPrefs.GetInt("PartyBarToggled", 0);
			PlayerPrefs.SetInt("PartyBarToggled", toggled == 0 ? 1 : 0);
		}

		[NewMember]
		public  static void UframeNone(GameObject go)
		{
			UseNoFrame();
		}

		[NewMember]
		public  static void UframeOne(GameObject go)
		{
			UseUframe("scratched-stone-blue");
		}

		[NewMember]
		public  static void UframeTwo(GameObject go)
		{
			UseUframe("scratched-stone");
		}

		[NewMember]
		public  static void UframeThree(GameObject go)
		{
			UseUframe("scratched-granite");
		}

		[NewMember]
		public  static void UframeFour(GameObject go)
		{
			UseUframe("polished-granite");
		}

		[NewMember]
		public  static void UframeFive(GameObject go)
		{
			UseUframe("medium-stones");
		}

		[NewMember]
		public  static void UframeSix(GameObject go)
		{
			UseUframe("big-stones");
		}

		[NewMember]
		public  static void UframeSeven(GameObject go)
		{
			UseUframe("wood-planks-brown");
		}

		[NewMember]
		public  static void UframeEight(GameObject go)
		{
			UseUframe("wood-planks");
		}

		[NewMember]
		public  static void UframeNine(GameObject go)
		{
			UseUframe("generic-wood-brown");
		}

		[NewMember]
		public  static void UframeTen(GameObject go)
		{
			UseUframe("dry-wood-brown");
		}

		[NewMember]
		public  static void UframeEleven(GameObject go)
		{
			UseUframe("dry-wood");
		}

		[NewMember]
		public  static void UframeTwelve(GameObject go)
		{
			UseUframe("bg2-mosaic");
		}

		[NewMember]
		public  static void UframeThirteen(GameObject go)
		{
			UseUframe("karkarovA");
		}

		[NewMember]
		public  static void UframeFourteen(GameObject go)
		{
			UseUframe("karkarovB");
		}

		[NewMember]
		public  static void UframeFifteen(GameObject go)
		{
			UseUframe("karkarovC");
		}

		[NewMember]
		public  static void UframeSixteen(GameObject go)
		{
			UseUframe("karkarovD");
		}

		[NewMember]
		public  static void ToggleBuffSide(GameObject go)
		{
			if (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIAnchor>().side == UIAnchor.Side.TopRight)
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIAnchor>().side = UIAnchor.Side.TopLeft;
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIAnchor>().pixelOffset = new Vector2(-27f, 0f); // default is (3,0)
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIGrid>().Reposition();

				for (int i = 0; i < 5; i++)
				{
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIAnchor>().side = UIAnchor.Side.TopLeft;
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIAnchor>().pixelOffset = new Vector2(-27f, 0f); // default is (3,0)
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIGrid>().Reposition();
				}
			}
			else
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIAnchor>().side = UIAnchor.Side.TopRight;
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIAnchor>().pixelOffset = new Vector2(3f, 0f); // default is (3,0)
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIGrid>().Reposition();

				for (int i = 0; i < 5; i++)
				{
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIAnchor>().side = UIAnchor.Side.TopRight;
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIAnchor>().pixelOffset = new Vector2(3f, 0f); // default is (3,0)
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIGrid>().Reposition();
				}
			}
		}

		[NewMember]
		public  static void DraggableUiToggle(bool state)
		{
			DragBtn1.SetActive(state);
			DragBtn2.SetActive(state);
			DragBtn3.SetActive(state);
			DragBtn4.SetActive(state);
			DragBtn5.SetActive(state);
			DragBtn6.SetActive(state);
			DragBtn7.SetActive(state);
			DragBtn8.SetActive(state);
			DragBtn9.SetActive(state);
			DragBtn10.SetActive(state);
			DragBtn11.SetActive(state);
			DragBtn12.SetActive(state);
			DragBtn13.SetActive(state);
			DragBtn14.SetActive(state);
			DragBtn15.SetActive(state);
			DragBtn16.SetActive(state);
			DragBtn17.SetActive(state);
			DragBtn18.SetActive(state);
			DragBtn19.SetActive(state);
			DragBtn20.SetActive(state);
			DragBtn21.SetActive(state);
			DragBtn22.SetActive(state);
			DragBtn23.SetActive(state);
			DragBtn24.SetActive(state);
			DragBtn25.SetActive(state);
			DragBtn26.SetActive(state);
			DragBtn27.SetActive(state);
			DragBtn28.SetActive(state);
			DragBtn29.SetActive(state);
			DragBtn30.SetActive(state);
			DragBtn31.SetActive(state);
			DragBtn32.SetActive(state);
			DragBtn33.SetActive(state);
			SaveBtn.SetActive(state);
			CancelBtn.SetActive(state);
			UseDefaultUIBtn.SetActive(state);
		}

		[NewMember]
		public  static void SaveUiCoords()
		{

			//Trim
			PlayerPrefs.SetString("Uframe", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(6).GetChild(0).GetComponent<UITexture>().mainTexture.name);

			PlayerPrefs.SetFloat("FormationX", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(5).localPosition.x);
			PlayerPrefs.SetFloat("FormationY", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(5).localPosition.y);

			PlayerPrefs.SetInt("BuffsSideChanged", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIAnchor>().side == UIAnchor.Side.TopRight ? 0 : 1);

			PlayerPrefs.SetInt("TempPartyToggled?", PlayerPrefs.GetInt("PartyBarToggled", 0));

			PlayerPrefs.SetFloat("PartyBarX", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).localPosition.x);
			PlayerPrefs.SetFloat("PartyBarY", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).localPosition.y);

			PlayerPrefs.SetFloat("PartySolidHudX", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(2).localPosition.x);
			PlayerPrefs.SetFloat("PartySolidHudY", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(2).localPosition.y);

			PlayerPrefs.SetInt("LogButtonsChanged", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetChild(0).GetChild(0).GetComponent<UIAnchor>().side == UIAnchor.Side.TopRight ? 0 : 1);

			PlayerPrefs.SetFloat("HudX", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(3).localPosition.x);
			PlayerPrefs.SetFloat("HudY", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(3).localPosition.y);

			PlayerPrefs.SetFloat("AbilitiesBarX", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(0).localPosition.x);
			PlayerPrefs.SetFloat("AbilitiesBarY", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(0).localPosition.y);

			PlayerPrefs.SetFloat("LeftHudBarX", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).localPosition.x);
			PlayerPrefs.SetFloat("LeftHudBarY", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).localPosition.y);

			PlayerPrefs.SetFloat("RightHudBarX", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).localPosition.x);
			PlayerPrefs.SetFloat("RightHudBarY", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).localPosition.y);

			PlayerPrefs.SetFloat("ClockX", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(2).localPosition.x);
			PlayerPrefs.SetFloat("ClockY", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(2).localPosition.y);

			PlayerPrefs.SetInt("AbilsHorizontal", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetComponent<UIGrid>().arrangement == UIGrid.Arrangement.Vertical ? 0 : 1);

			PlayerPrefs.SetInt("HudTextureHidden", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(3).gameObject.activeSelf ? 0 : 1);

			PlayerPrefs.SetFloat("LogX", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).localPosition.x);
			PlayerPrefs.SetFloat("LogY", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).localPosition.y);

			// this changed in the update
			PlayerPrefs.SetFloat("LogWidth", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetChild(0).GetChild(2).GetChild(1).localPosition.x);
			PlayerPrefs.SetFloat("LogHeight", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetChild(0).GetChild(2).GetChild(1).localPosition.y);

			PlayerPrefs.SetInt("ButtonsBackground", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf ? 1 : 0);

			PlayerPrefs.SetInt("UsingCustomTextures", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<UISprite>().atlas.spriteMaterial.mainTexture == DefaultActionBarAtlas ? 0 : 1);

			PlayerPrefs.SetInt("PortraitHighlights", UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(10).gameObject.activeSelf ? 1 : 0);
		}

		[NewMember]
		public  static void SaveUi(GameObject go)
		{
			PlayerPrefs.SetInt("UseCustomUi", 1);

			mod_CommandLine.SaveUiCoords();

			global::Console.AddMessage("UI Layout Saved.", Color.green);
			DraggableUiToggle(false);
		}

		[NewMember]
		public  static void CustomUiApplyOnce()
		{
			//FixPortraitHoles ();    
			DefaultActionBarAtlas = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<UISprite>().atlas.spriteMaterial.mainTexture;
			UIOptionsManager.Instance.transform.parent.GetChild(8).gameObject.SetActive(false); // turning off BB-version label in the upper right corner, cause it's annoying when you want to move portraits there
			// UIAnchors on the ActionBarWindow that prevent it from being moved... (it's related to the partybar somehow?)
			// HUD -> Bottom -> ActionBarWindow -> destroy 3 UIAnchors
			foreach (UIAnchor comp in UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetComponents<UIAnchor>())
				GameUtilities.DestroyComponent(comp);

			//		// These also prevent the log from being moved...
			//		// HUD -> Bottom -> ActionBarWindow -> ActionBarContractedAnchor
			//		// HUD -> Bottom -> ActionBarWindow -> ActionBarExpandedAnchor
			//		UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(1).gameObject.SetActive (false);
			//		UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(2).gameObject.SetActive (false);
			//		// these, actually... but more importantly, maybe we should let it stay there and only move its children?
			//		UnityEngine.Object.Destroy (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(1).gameObject);
			//		UnityEngine.Object.Destroy (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(2).gameObject);
			//		// we don't need it now, cause we stopped moving the log, we only move its children... moving the log became buggy in v480

			// Prevented the log from being moved
			// HUD -> Bottom -> ConsoleWindow -> destroy UIAnchor
			GameUtilities.DestroyComponent(UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetComponent<UIAnchor>());

			//GameUtilities.DestroyComponent (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetComponent<UIAnchor>());

			// disable the minimize buttons for the log and the actionbar
			// HUD -> Bottom -> disable ConsoleMinimize
			// HUD -> Bottom -> disable ActionBarMinimize
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(0).gameObject.SetActive(false);

			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(3).gameObject.SetActive(false);

			// this UIPanel used to hide the clock when it was moved from far away from its original position
			// HUD -> Bottom -> ActionBarWindow -> ActionBarExpandedAnchor -> UIPanel
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetComponent<UIPanel>().clipping = UIDrawCall.Clipping.None;

			// detaches the "GAME PAUSED" and "SLOW MO" from the Clock panel, to which it was attached for some reason...
			UIOptionsManager.Instance.transform.parent.GetChild(3).GetComponent<UIAnchor>().widgetContainer = UIOptionsManager.Instance.transform.parent.GetChild(6).GetComponent<UIPanel>().widgets[0];
			UIOptionsManager.Instance.transform.parent.GetChild(3).GetComponents<UIAnchor>()[1].DisableY = true;
			UIOptionsManager.Instance.transform.parent.GetChild(9).GetComponent<UIAnchor>().widgetContainer = UIOptionsManager.Instance.transform.parent.GetChild(6).GetComponent<UIPanel>().widgets[0];
			UIOptionsManager.Instance.transform.parent.GetChild(9).GetComponents<UIAnchor>()[1].DisableY = true;

			Debug.Log("P! end CustomUiApplyOnce");
		}

		[NewMember]
		public  static void RepositionUi()
		{

			if (PlayerPrefs.GetInt("BuffsSideChanged", 0) == 1)
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIAnchor>().side = UIAnchor.Side.TopLeft;
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIAnchor>().pixelOffset = new Vector2(-27f, 0f); // default is (3,0)
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIGrid>().Reposition();

				for (int i = 0; i < 5; i++)
				{
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIAnchor>().side = UIAnchor.Side.TopLeft;
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIAnchor>().pixelOffset = new Vector2(-27f, 0f); // default is (3,0)
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIGrid>().Reposition();
				}
			}
			else
			{
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIAnchor>().side = UIAnchor.Side.TopRight;
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIAnchor>().pixelOffset = new Vector2(3f, 0f); // default is (3,0)
				UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(5).GetComponent<UIGrid>().Reposition();

				for (int i = 0; i < 5; i++)
				{
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIAnchor>().side = UIAnchor.Side.TopRight;
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIAnchor>().pixelOffset = new Vector2(3f, 0f); // default is (3,0)
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).GetChild(3 + i).GetChild(5).GetComponent<UIGrid>().Reposition();
				}
			}

			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(5).localPosition = new Vector3(PlayerPrefs.GetFloat("FormationX", -488f), PlayerPrefs.GetFloat("FormationY", -390f), 0f);

			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(1).GetChild(0).localPosition = new Vector3(PlayerPrefs.GetFloat("PartyBarX", 0f), PlayerPrefs.GetFloat("PartyBarY", 0f), -3f);

			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(2).localPosition = new Vector3(PlayerPrefs.GetFloat("PartySolidHudX", -22f), PlayerPrefs.GetFloat("PartySolidHudY", 0f), 2f);

			if (PlayerPrefs.GetInt("LogButtonsChanged", 0) != (UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).GetChild(0).GetChild(0).GetComponent<UIAnchor>().side == UIAnchor.Side.TopRight ? 0 : 1))
			{
				GameObject blop = new GameObject();
				mod_CommandLine.ToggleLogButtons(blop);
			}

			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(3).localPosition = new Vector3(PlayerPrefs.GetFloat("HudX", -374f), PlayerPrefs.GetFloat("HudY", -517.5f), 0f);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(0).localPosition = new Vector3(PlayerPrefs.GetFloat("AbilitiesBarX", 0f), PlayerPrefs.GetFloat("AbilitiesBarY", 150f), 0);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).localPosition = new Vector3(PlayerPrefs.GetFloat("LogX", 927f), PlayerPrefs.GetFloat("LogY", -519f), -3f);

			float LogWidth = PlayerPrefs.GetFloat("LogWidth", 400f);
			float LogHeight = PlayerPrefs.GetFloat("LogHeight", 106f);

			UIConsole.Instance.CornerHandle.gameObject.transform.localPosition = new Vector3(LogWidth, LogHeight, UIConsole.Instance.CornerHandle.gameObject.transform.localPosition.z);
			UIConsole.Instance.Background.transform.localScale = new Vector3(Mathf.Abs(LogWidth), LogHeight, 1f);

			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).localPosition = new Vector3(PlayerPrefs.GetFloat("LeftHudBarX", -204f), PlayerPrefs.GetFloat("LeftHudBarY", 49f), 0f);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).localPosition = new Vector3(PlayerPrefs.GetFloat("RightHudBarX", 155f), PlayerPrefs.GetFloat("RightHudBarY", 49f), 0f);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(2).localPosition = new Vector3(PlayerPrefs.GetFloat("ClockX", 0f), PlayerPrefs.GetFloat("ClockY", 0f), -2f);
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetComponent<UIGrid>().arrangement = PlayerPrefs.GetInt("AbilsHorizontal", 0) == 1 ? UIGrid.Arrangement.Horizontal : UIGrid.Arrangement.Vertical;
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetComponent<UIGrid>().Reposition();
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetComponent<UIGrid>().arrangement = PlayerPrefs.GetInt("AbilsHorizontal", 0) == 1 ? UIGrid.Arrangement.Vertical : UIGrid.Arrangement.Horizontal;
			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).GetComponent<UIGrid>().Reposition();

			UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(3).gameObject.SetActive(PlayerPrefs.GetInt("HudTextureHidden", 0) == 0);

			string uframe = PlayerPrefs.GetString("Uframe");
			if (uframe == "FRM_botCornerLft" || uframe == "")
			{
				UseNoFrame();
			}
			else
			{
				UseUframe(uframe);
			}

			if (Convert.ToInt32(UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf) != PlayerPrefs.GetInt("ButtonsBackground", 1))
				ToggleButtonsBackground();

			ReplaceAtlas(PlayerPrefs.GetInt("UsingCustomTextures", 0) == 1 ? true : false);

			ResetPortraitHighlight(PlayerPrefs.GetInt("PortraitHighlights", 1) == 1 ? true : false);
		}

		[NewMember]
		public  static void CancelUi(GameObject go)
		{
			global::Console.AddMessage("Cancelling changes.");

			PlayerPrefs.SetInt("PartyBarToggled", PlayerPrefs.GetInt("TempPartyToggled?", 0));

			mod_CommandLine.RepositionUi();

			DraggableUiToggle(false);
		}

		[NewMember]
		public  static void UseDefaultUi(GameObject go)
		{
			PlayerPrefs.SetInt("PartyBarToggled", 0);
			PlayerPrefs.SetInt("TempPartyToggled?", 0);

			global::Console.AddMessage("In order to start using the default UI, save game if you want, then exit to the main menu and reload.", Color.green);
			PlayerPrefs.SetInt("UseCustomUi", 0);
			DraggableUiToggle(false);
		}

		[NewMember]
		public  static void TT()
		{
			if (((Mod_OnGUI_Player)GameState.s_playerCharacter).showGameObjectBrowser == false)
			{
				if (((Mod_OnGUI_Player)GameState.s_playerCharacter).inspecting == null)
					((Mod_OnGUI_Player)GameState.s_playerCharacter).inspecting = UIOptionsManager.Instance.transform.parent;

				if (GameBrowserBackground == null)
				{
					UIMultiSpriteImageButton portraitLast = null;

					foreach (UIMultiSpriteImageButton btn in UnityEngine.Object.FindObjectsOfType<UIMultiSpriteImageButton>())
						if (btn.ToString() == "PartyPortrait(Clone) (UIMultiSpriteImageButton)" && portraitLast == null)
							portraitLast = btn;

					GameBrowserBackground = NGUITools.AddWidget<UITexture>(portraitLast.transform.parent.parent.gameObject);
					GameBrowserBackground.mainTexture = new Texture2D(500, 500);

					GameBrowserBackground.transform.localScale = new Vector3(1100f, 600f, 1f); // those values are for 1920*1080...
					//GameBrowserBackground.transform.localScale = new Vector3 (1600f, 800f, 1f); // those values are for 1280*1024

					// rescaler
					if (Screen.width != 1920)
					{
						float addwidth = (1920 - Screen.width) * 0.78f;
						float addheight = (1080 - Screen.height) * 0.55f;
						GameBrowserBackground.transform.localScale += new Vector3(addwidth, addheight, 0f);
					}

					BoxCollider boxColl = NGUITools.AddWidgetCollider(GameBrowserBackground.gameObject); // adding a box collider, it's required for the UINoClick component
					boxColl.gameObject.AddComponent<UINoClick>(); // this prevents clicks from going through the U-frame
					UIAnchor ank = GameBrowserBackground.gameObject.AddComponent<UIAnchor>();
					ank.side = UIAnchor.Side.Center;

				}
				else
					GameBrowserBackground.gameObject.SetActive(true);

				((Mod_OnGUI_Player)GameState.s_playerCharacter).showGameObjectBrowser = true; // temporarily turned off
			}
			else
			{
				GameBrowserBackground.gameObject.SetActive(false);
				((Mod_OnGUI_Player)GameState.s_playerCharacter).showGameObjectBrowser = false;
			}
		}

		[NewMember]
		public  static void BB()
		{
			if (DragBtn1 == null)
			{
				if (PlayerPrefs.GetInt("AppliedCustomUi", 0) == 0)
					mod_CommandLine.CustomUiApplyOnce();

				mod_CommandLine.SaveUiCoords();

				////////////////////// finding a button's texture
				/// 
				UISprite closeButton = null;

				UISprite[] mdo = Resources.FindObjectsOfTypeAll(typeof(UISprite)) as UISprite[];
				foreach (UISprite brr in mdo)
				{
					if (brr.name == "CloseButton (UISprite)" && closeButton == null)
					{
						closeButton = brr;
						break;
					}
				}
				/// end

				DragBtn1 = new GameObject();
				DragBtn1.name = "DragPartyBar";
				DragBtn1.transform.parent = UIPartyPortraitBar.Instance.transform.parent;
				DragBtn1.transform.localScale = new Vector3(1f, 1f, 1f);
				UIMultiSpriteImageButton dpbImageButton = NGUITools.AddChild(DragBtn1.gameObject, UIOptionsManager.Instance.PageButtonPrefab.gameObject).GetComponent<UIMultiSpriteImageButton>();

				if (dpbImageButton.transform.childCount == 5) // sometimes the pagebuttonprefab seems to already have a collider, sometimes not. if it has one, we destroy it and just always manually create one.
					UnityEngine.Object.DestroyImmediate(DragBtn1.transform.GetChild(0).GetChild(4).gameObject);

				GameObject bx = new GameObject("Collider");
				bx.transform.parent = dpbImageButton.transform;
				bx.transform.localScale = new Vector3(269f, 56f, 1f);
				bx.transform.localPosition = new Vector3(0f, 0, -2f);
				bx.layer = 14;
				bx.AddComponent<BoxCollider>().size = new Vector3(1, 1, 1);
				bx.AddComponent<UINoClick>().BlockClicking = true;
				bx.AddComponent<UIEventListener>();
				bx.AddComponent<UIDragObject>().target = UIPartyPortraitBar.Instance.transform;
				dpbImageButton.Label.GetComponent<GUIStringLabel>().FormatString = "Drag Party Bar";
				// this button will have a second component UIDragObject to drag the party solid background, but since it would also be duplicated on other buttons, we'll add this component at the very end of the method

				DragBtn2 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn2.name = "DragActionbarButton";
				DragBtn2.transform.parent = DragBtn1.transform.parent;
				DragBtn2.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn2.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn2.transform.localPosition += new Vector3(0f, 50f, 0f);
				DragBtn2.GetComponentInChildren<UIDragObject>().target = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(3).transform;
				DragBtn2.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Drag Hud Bgr";

				DragBtn3 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn3.name = "DragLog";
				DragBtn3.transform.parent = DragBtn1.transform.parent;
				DragBtn3.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn3.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn3.transform.localPosition += new Vector3(0f, 100f, 0f);
				DragBtn3.GetComponentInChildren<UIDragObject>().target = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(1).transform;
				DragBtn3.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Drag Log";

				DragBtn4 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn4.name = "DragAbilities";
				DragBtn4.transform.parent = DragBtn1.transform.parent;
				DragBtn4.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn4.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn4.transform.localPosition += new Vector3(0f, -50f, 0f);
				DragBtn4.GetComponentInChildren<UIDragObject>().target = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(0).transform;
				DragBtn4.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Drag Abilities";

				DragBtn5 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn5.name = "ToggleLogBtns";
				DragBtn5.transform.parent = DragBtn1.transform.parent;
				DragBtn5.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn5.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn5.transform.localPosition += new Vector3(400f, -50f, 0f);
				DragBtn5.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "T. Log Buttons Side";
				UnityEngine.Object.Destroy(DragBtn5.GetComponentInChildren<UIDragObject>());
				DragBtn5.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn5.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn5.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(ToggleLogButtons);

				DragBtn6 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn6.name = "ToggleHudHoriz";
				DragBtn6.transform.parent = DragBtn1.transform.parent;
				DragBtn6.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn6.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn6.transform.localPosition += new Vector3(400f, -0f, 0f);
				DragBtn6.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "T. Hud Horiz/Vert";
				UnityEngine.Object.Destroy(DragBtn6.GetComponentInChildren<UIDragObject>());
				DragBtn6.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn6.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn6.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(ToggleHudHoriz);

				DragBtn7 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn7.name = "DragHudLeft";
				DragBtn7.transform.parent = DragBtn1.transform.parent;
				DragBtn7.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn7.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn7.transform.localPosition += new Vector3(0f, -150f, 0f);
				DragBtn7.GetComponentInChildren<UIDragObject>().target = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(0).transform;
				DragBtn7.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Drag Hud Panel 1";
				DragBtn7.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn7.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;

				DragBtn8 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn8.name = "DragHudRight";
				DragBtn8.transform.parent = DragBtn1.transform.parent;
				DragBtn8.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn8.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn8.transform.localPosition += new Vector3(0f, -200f, 0f);
				DragBtn8.GetComponentInChildren<UIDragObject>().target = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(1).transform;
				DragBtn8.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Drag Hud Panel 2";
				DragBtn8.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn8.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;

				DragBtn9 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn9.name = "DragClock";
				DragBtn9.transform.parent = DragBtn1.transform.parent;
				DragBtn9.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn9.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn9.transform.localPosition += new Vector3(0f, -100f, 0f);
				DragBtn9.GetComponentInChildren<UIDragObject>().target = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(2).transform;
				DragBtn9.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Drag Clock";
				DragBtn9.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn9.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;

				DragBtn10 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn10.name = "HideHudBg";
				DragBtn10.transform.parent = DragBtn1.transform.parent;
				DragBtn10.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn10.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn10.transform.localPosition += new Vector3(400f, 50f, 0f);
				DragBtn10.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "T. Hud Background";
				UnityEngine.Object.Destroy(DragBtn10.GetComponentInChildren<UIDragObject>());
				DragBtn10.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn10.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn10.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(HideHudBg);

				DragBtn11 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn11.name = "TogglePartyBar";
				DragBtn11.transform.parent = DragBtn1.transform.parent;
				DragBtn11.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn11.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn11.transform.localPosition += new Vector3(400f, 100f, 0f);
				DragBtn11.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "T. Party Horiz/Vert";
				UnityEngine.Object.Destroy(DragBtn11.GetComponentInChildren<UIDragObject>());
				DragBtn11.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn11.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn11.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(TogglePartyBar);

				DragBtn12 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn12.name = "SwitchBuffSide";
				DragBtn12.transform.parent = DragBtn1.transform.parent;
				DragBtn12.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn12.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn12.transform.localPosition += new Vector3(400f, 150f, 0f);
				DragBtn12.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Toggle Buffs Side";
				UnityEngine.Object.Destroy(DragBtn12.GetComponentInChildren<UIDragObject>());
				DragBtn12.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn12.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn12.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(ToggleBuffSide);

				DragBtn13 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn13.name = "DragFormation";
				DragBtn13.transform.parent = DragBtn1.transform.parent;
				DragBtn13.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn13.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn13.transform.localPosition += new Vector3(0f, -250f, 0f);
				DragBtn13.GetComponentInChildren<UIDragObject>().target = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(4).GetChild(0).GetChild(5).transform;
				DragBtn13.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Drag Formation Bar";
				DragBtn13.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn13.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;

				DragBtn14 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn14.name = "UframeNone";
				DragBtn14.transform.parent = DragBtn1.transform.parent;
				DragBtn14.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn14.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn14.transform.localPosition += new Vector3(-400f, -290f, 0f);
				DragBtn14.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Frame: None";
				UnityEngine.Object.Destroy(DragBtn14.GetComponentInChildren<UIDragObject>());
				DragBtn14.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn14.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn14.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeNone);

				DragBtn15 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn15.name = "Uframe1";
				DragBtn15.transform.parent = DragBtn1.transform.parent;
				DragBtn15.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn15.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn15.transform.localPosition += new Vector3(-400f, -210f, 0f);
				DragBtn15.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: Scr. Stone Blue";
				UnityEngine.Object.Destroy(DragBtn15.GetComponentInChildren<UIDragObject>());
				DragBtn15.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn15.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn15.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeOne);

				DragBtn16 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn16.name = "Uframe2";
				DragBtn16.transform.parent = DragBtn1.transform.parent;
				DragBtn16.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn16.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn16.transform.localPosition += new Vector3(-400f, -170f, 0f);
				DragBtn16.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: Scratched Stone";
				UnityEngine.Object.Destroy(DragBtn16.GetComponentInChildren<UIDragObject>());
				DragBtn16.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn16.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn16.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeTwo);

				DragBtn17 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn17.name = "Uframe3";
				DragBtn17.transform.parent = DragBtn1.transform.parent;
				DragBtn17.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn17.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn17.transform.localPosition += new Vector3(-400f, -130f, 0f);
				DragBtn17.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: Scratched Granite";
				UnityEngine.Object.Destroy(DragBtn17.GetComponentInChildren<UIDragObject>());
				DragBtn17.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn17.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn17.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeThree);

				DragBtn18 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn18.name = "Uframe4";
				DragBtn18.transform.parent = DragBtn1.transform.parent;
				DragBtn18.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn18.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn18.transform.localPosition += new Vector3(-400f, -90f, 0f);
				DragBtn18.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: Polished Granite";
				UnityEngine.Object.Destroy(DragBtn18.GetComponentInChildren<UIDragObject>());
				DragBtn18.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn18.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn18.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeFour);

				DragBtn19 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn19.name = "Uframe5";
				DragBtn19.transform.parent = DragBtn1.transform.parent;
				DragBtn19.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn19.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn19.transform.localPosition += new Vector3(-400f, -50f, 0f);
				DragBtn19.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: Medium Stones";
				UnityEngine.Object.Destroy(DragBtn19.GetComponentInChildren<UIDragObject>());
				DragBtn19.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn19.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn19.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeFive);

				DragBtn20 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn20.name = "Uframe6";
				DragBtn20.transform.parent = DragBtn1.transform.parent;
				DragBtn20.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn20.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn20.transform.localPosition += new Vector3(-400f, -10f, 0f);
				DragBtn20.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: Big Stones";
				UnityEngine.Object.Destroy(DragBtn20.GetComponentInChildren<UIDragObject>());
				DragBtn20.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn20.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn20.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeSix);

				DragBtn21 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn21.name = "Uframe7";
				DragBtn21.transform.parent = DragBtn1.transform.parent;
				DragBtn21.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn21.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn21.transform.localPosition += new Vector3(-400f, 30f, 0f);
				DragBtn21.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: W. Planks Brown";
				UnityEngine.Object.Destroy(DragBtn21.GetComponentInChildren<UIDragObject>());
				DragBtn21.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn21.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn21.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeSeven);

				DragBtn22 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn22.name = "Uframe8";
				DragBtn22.transform.parent = DragBtn1.transform.parent;
				DragBtn22.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn22.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn22.transform.localPosition += new Vector3(-400f, 70f, 0f);
				DragBtn22.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: Wood Planks";
				UnityEngine.Object.Destroy(DragBtn22.GetComponentInChildren<UIDragObject>());
				DragBtn22.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn22.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn22.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeEight);

				DragBtn23 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn23.name = "Uframe9";
				DragBtn23.transform.parent = DragBtn1.transform.parent;
				DragBtn23.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn23.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn23.transform.localPosition += new Vector3(-400f, 110f, 0f);
				DragBtn23.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: Generic Wood";
				UnityEngine.Object.Destroy(DragBtn23.GetComponentInChildren<UIDragObject>());
				DragBtn23.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn23.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn23.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeNine);

				DragBtn24 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn24.name = "Uframe10";
				DragBtn24.transform.parent = DragBtn1.transform.parent;
				DragBtn24.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn24.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn24.transform.localPosition += new Vector3(-400f, 150f, 0f);
				DragBtn24.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: Dry Wood Brown";
				UnityEngine.Object.Destroy(DragBtn24.GetComponentInChildren<UIDragObject>());
				DragBtn24.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn24.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn24.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeTen);

				DragBtn25 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn25.name = "Uframe11";
				DragBtn25.transform.parent = DragBtn1.transform.parent;
				DragBtn25.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn25.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn25.transform.localPosition += new Vector3(-400f, -250f, 0f);
				DragBtn25.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: Dry Wood";
				UnityEngine.Object.Destroy(DragBtn25.GetComponentInChildren<UIDragObject>());
				DragBtn25.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn25.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn25.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeEleven);

				DragBtn26 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn26.name = "Uframe12";
				DragBtn26.transform.parent = DragBtn1.transform.parent;
				DragBtn26.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn26.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn26.transform.localPosition += new Vector3(-400f, 190f, 0f);
				DragBtn26.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "U: BG2 Mosaic";
				UnityEngine.Object.Destroy(DragBtn26.GetComponentInChildren<UIDragObject>());
				DragBtn26.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn26.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn26.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeTwelve);

				DragBtn27 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn27.name = "Uframe13";
				DragBtn27.transform.parent = DragBtn1.transform.parent;
				DragBtn27.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn27.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn27.transform.localPosition += new Vector3(-400f, 230f, 0f);
				DragBtn27.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "_: Karkarov A";
				UnityEngine.Object.Destroy(DragBtn27.GetComponentInChildren<UIDragObject>());
				DragBtn27.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn27.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn27.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeThirteen);

				DragBtn28 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn28.name = "Uframe14";
				DragBtn28.transform.parent = DragBtn1.transform.parent;
				DragBtn28.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn28.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn28.transform.localPosition += new Vector3(-400f, 270f, 0f);
				DragBtn28.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "_: Karkarov B";
				UnityEngine.Object.Destroy(DragBtn28.GetComponentInChildren<UIDragObject>());
				DragBtn28.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn28.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn28.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeFourteen);

				DragBtn32 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn32.name = "Uframe15";
				DragBtn32.transform.parent = DragBtn1.transform.parent;
				DragBtn32.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn32.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn32.transform.localPosition += new Vector3(-400f, 310f, 0f);
				DragBtn32.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "_: Karkarov C";
				UnityEngine.Object.Destroy(DragBtn32.GetComponentInChildren<UIDragObject>());
				DragBtn32.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn32.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn32.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeFifteen);

				DragBtn33 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn33.name = "Uframe16";
				DragBtn33.transform.parent = DragBtn1.transform.parent;
				DragBtn33.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				DragBtn33.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn33.transform.localPosition += new Vector3(-400f, 350f, 0f);
				DragBtn33.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "_: Karkarov D";
				UnityEngine.Object.Destroy(DragBtn33.GetComponentInChildren<UIDragObject>());
				DragBtn33.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn33.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn33.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UframeSixteen);

				DragBtn29 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn29.name = "TglBtnBackground";
				DragBtn29.transform.parent = DragBtn1.transform.parent;
				DragBtn29.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn29.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn29.transform.localPosition += new Vector3(400f, -100f, 0f);
				DragBtn29.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "T. Buttons Background";
				UnityEngine.Object.Destroy(DragBtn29.GetComponentInChildren<UIDragObject>());
				DragBtn29.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn29.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn29.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(ButtonsBackground);

				DragBtn30 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn30.name = "TglPortHighlight";
				DragBtn30.transform.parent = DragBtn1.transform.parent;
				DragBtn30.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn30.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn30.transform.localPosition += new Vector3(400f, -150f, 0f);
				DragBtn30.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "T. Portrait Highlight";
				UnityEngine.Object.Destroy(DragBtn30.GetComponentInChildren<UIDragObject>());
				DragBtn30.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn30.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn30.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(TogglePortraitHighlight);

				DragBtn31 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn31.name = "TglCustomTextures";
				DragBtn31.transform.parent = DragBtn1.transform.parent;
				DragBtn31.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn31.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn31.transform.localPosition += new Vector3(400f, -200f, 0f);
				DragBtn31.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "T. Custom Textures";
				UnityEngine.Object.Destroy(DragBtn31.GetComponentInChildren<UIDragObject>());
				DragBtn31.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				DragBtn31.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				DragBtn31.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(ToggleCustomTextures);


				SaveBtn = GameObject.Instantiate(DragBtn1) as GameObject;
				SaveBtn.name = "SaveUiBtn";
				SaveBtn.transform.parent = DragBtn1.transform.parent;
				SaveBtn.transform.localScale = new Vector3(1f, 1f, 1f);
				SaveBtn.transform.localPosition = DragBtn1.transform.localPosition;
				SaveBtn.transform.localPosition += new Vector3(-140f, 250f, 0f);
				SaveBtn.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Save UI & Close";
				UnityEngine.Object.Destroy(SaveBtn.GetComponentInChildren<UIDragObject>());
				//SaveBtn.GetComponentInChildren<UIDragObject> ().target = SaveBtn.transform;
				SaveBtn.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				SaveBtn.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				SaveBtn.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(SaveUi);

				CancelBtn = GameObject.Instantiate(DragBtn1) as GameObject;
				CancelBtn.name = "CancelUiBtn";
				CancelBtn.transform.parent = DragBtn1.transform.parent;
				CancelBtn.transform.localScale = new Vector3(1f, 1f, 1f);
				CancelBtn.transform.localPosition = DragBtn1.transform.localPosition;
				CancelBtn.transform.localPosition += new Vector3(140f, 250f, 0f);
				CancelBtn.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Cancel & Close";
				UnityEngine.Object.Destroy(CancelBtn.GetComponentInChildren<UIDragObject>());
				//SaveBtn.GetComponentInChildren<UIDragObject> ().target = SaveBtn.transform;
				CancelBtn.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				CancelBtn.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				CancelBtn.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(CancelUi);

				UseDefaultUIBtn = GameObject.Instantiate(DragBtn1) as GameObject;
				UseDefaultUIBtn.name = "UseDefaultUIBtn";
				UseDefaultUIBtn.transform.parent = DragBtn1.transform.parent;
				UseDefaultUIBtn.transform.localScale = new Vector3(1f, 1f, 1f);
				UseDefaultUIBtn.transform.localPosition = DragBtn1.transform.localPosition;
				UseDefaultUIBtn.transform.localPosition += new Vector3(400f, 350f, 0f);
				UseDefaultUIBtn.GetComponentInChildren<UIMultiSpriteImageButton>().Label.GetComponent<GUIStringLabel>().FormatString = "Use Default UI & Close";
				UnityEngine.Object.Destroy(UseDefaultUIBtn.GetComponentInChildren<UIDragObject>());
				//SaveBtn.GetComponentInChildren<UIDragObject> ().target = SaveBtn.transform;
				UseDefaultUIBtn.GetComponentInChildren<UIMultiSpriteImageButton>().Label.multiLine = false;
				UseDefaultUIBtn.GetComponentInChildren<UIMultiSpriteImageButton>().Label.shrinkToFit = true;
				UseDefaultUIBtn.GetComponentInChildren<UIMultiSpriteImageButton>().onClick = new UIEventListener.VoidDelegate(UseDefaultUi);

				// adding the second drag component to the DragPartyBar button that will drag the partybar solid background
				bx.AddComponent<UIDragObject>().target = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild(12).GetChild(2).GetChild(2);



			}
			else
			{
				if (DragBtn1.activeSelf == false)
				{
					mod_CommandLine.SaveUiCoords();
					DraggableUiToggle(true);
				}
				else
				{
					DraggableUiToggle(false);
				}
			}
		}

		[NewMember]
		public  static void ShowModelViewer()
		{
			if (((Mod_OnGUI_Player)GameState.s_playerCharacter).showModelViewer)
			{
				global::Console.AddMessage("Closing ModelViewer.");
				((Mod_OnGUI_Player)GameState.s_playerCharacter).showModelViewer = false;
				modelViewerBackground.SetActive(false);
			}
			else
			{
				global::Console.AddMessage("Opening ModelViewer. Type the same command to hide it.");
				if (modelViewerBackground == null)
				{
					////////////////////// finding various buttons and textures
					UISprite closeButton = null;

					UISprite[] mdo = Resources.FindObjectsOfTypeAll(typeof(UISprite)) as UISprite[];
					foreach (UISprite brr in mdo)
					{
						if (brr.ToString() == "CloseButton (UISprite)" && closeButton == null)
						{
							closeButton = brr;
							//Console.AddMessage ("Assigning CloseButton.");
						}
					}

					UIMultiSpriteImageButton portraitLast = null;

					foreach (UIMultiSpriteImageButton btn in UnityEngine.Object.FindObjectsOfType<UIMultiSpriteImageButton>())
						if (btn.ToString() == "PartyPortrait(Clone) (UIMultiSpriteImageButton)" && portraitLast == null)
						{
							portraitLast = btn;
							//Console.AddMessage ("Assigning optioButton.");
						}
					///////////////////////////////

					modelViewerBackground = new GameObject("ModelViewer");
					modelViewerBackground.transform.parent = portraitLast.transform.parent;
					modelViewerBackground.layer = portraitLast.gameObject.layer;
					modelViewerBackground.transform.localScale = new Vector3(1f, 1f, 1f);

					UISprite spr = NGUITools.AddSprite(modelViewerBackground, closeButton.atlas, "BUT_genericBGMid");
					spr.transform.localScale = new Vector3(Screen.width / 4, Screen.height * 1.6f, 1f);
					spr.type = UISprite.Type.Sliced;
					spr.color = new Color(0f, 0f, 0f, 0.3f);

					BoxCollider cold = spr.gameObject.AddComponent<BoxCollider>();
					cold.size = new Vector3(1f, 1f, 1f);

					spr.gameObject.AddComponent<UINoClick>().BlockClicking = true;

					modelViewerBackground.transform.localPosition = new Vector3(Screen.width / 10, Screen.height * 0.8f, 0);
				}
				else
					modelViewerBackground.SetActive(true);

				((Mod_OnGUI_Player)GameState.s_playerCharacter).showModelViewer = true;
			}
		}


	}

}