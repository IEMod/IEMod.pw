using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using IEMod.Helpers;
using IEMod.Mods.ObjectBrowser;
using IEMod.Mods.Options;
using IEMod.Mods.UICustomization;
//using IEMod.Mods.UICustomization;
using Patchwork.Attributes;
using UnityEngine;


namespace IEMod.Mods.ConsoleMod
{
    [ModifiesType("CommandLine")]
    public class mod_CommandLine
    {
        [NewMember]
        public static void CheckAchievements()
        {
            if (AchievementTracker.Instance.DisableAchievements == true)
            {
                global::Console.AddMessage("Your achievements were previously disabled for this playthrough.", Color.red);
               // global::Console.AddMessage("To reactivate them, type: ReenableAchievements");
            }
            else
                global::Console.AddMessage("Your achievements are doing fine.", Color.green);
        }
    }
    /*
        [ModifiesType("CommandLineRun")]
        public class mod_CommandLineRun
        {
            // TJH 8/26/2015 - It's no longer necessary to override RunCommand. We can just make sure all methods are always
            // available and not treated as cheats

            [ModifiesMember("MethodIsAvailable")]
            public static bool MethodIsAvailable(MethodInfo method)
            {
                return true;
            }

        }
    */

    [ModifiesType("CommandLineRun")]
    public class mod_CommandLineRun
    {
        [ModifiesMember("RunCommand")]
        public static void RunCommand(string command)
        {
            object[] objArray;
            if (string.IsNullOrEmpty(command))
            {
                return;
            }
            if (command.ToLower() == "runcommand")
            {
                return;
            }
            IList<string> strs = StringUtility.CommandLineStyleSplit(command);
            bool flag = false;
            bool flag1 = false;
            string empty = string.Empty;
            IEnumerator<MethodInfo> enumerator = CommandLineRun.GetAllMethods().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    MethodInfo current = enumerator.Current;
                    if (string.Compare(current.Name, strs[0], true) != 0)
                    {
                        continue;
                    }
                    /*
                    if (!CommandLineRun.MethodIsAvailable(current))
                    {
                        flag = true;
                    }
                    */
                    else if (!CommandLineRun.FillMethodParams(current, strs, out objArray, out empty))
                    {
                        flag1 = true;
                    }
                    else
                    {
                        current.Invoke(null, objArray);
                        return;
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            if (flag1)
            {
                Console.AddMessage(string.Concat("Command or script '", strs[0], "' parameter error: ", empty), Color.yellow);
            }
            else if (!flag)
            {
                Console.AddMessage(string.Concat("No command or script named '", strs[0], "' exists."), Color.yellow);
            }
            else
            {
                Console.AddMessage(string.Concat("The command or script '", strs[0], "' is not available at this time."), Color.yellow);
            }
        }
    }
}

/*
 * 
 * Old contents of this file, caused combat crashes ''randomly''
 * Some of the commands might be restorable though
 * 
 * 
 [ModifiesType("CommandLine")]
    public class mod_CommandLine 
	{
		[ModifiesType()]
		public class mod_GameState : GameState {
			[ModifiesAccessibility]
			public new bool s_playerCharacter;
		}

	    [ModifiesType]
	    private class mod_AchievementsTracker : AchievementTracker {
			[ModifiesAccessibility()]
		    public new bool m_disableAchievements;
	    }

		[ModifiesType()]
		public class mod_Loot : Loot {

			[ModifiesAccessibility]
			public new bool GetInventoryComponent() {
				return false;
			}
		}
		[ModifiesType]
		public class mod_Health : Health {

			public new bool m_isAnimalCompanion {
				[ModifiesAccessibility()] get {
					return false;
				}
				[ModifiesAccessibility()] set { } 
			}

			public new bool CanDie
			{
				[ModifiesAccessibility()]
				get {
					return false;
				}
				set {
					
				}
			}
		}
		[ModifiesType]
		public class mod_QuestManager : QuestManager {

			[ModifiesAccessibility("get_ActiveQuests")]
			public void ActiveGetter() {
				
			}
		}
		[ModifiesType("StringTableManager")]
		public static class mod_StringTableManager {
			[ModifiesAccessibility]
			public static bool StringTables;
			[ModifiesAccessibility]
			public static bool StringTableLookup;
		}

		[ModifiesType]
		public class mod_UIOptionsManager : UIOptionsManager {
			[ModifiesAccessibility]
			public new bool m_GameMode;
		}
		[ModifiesType]
		public class mod_CharacterStats : global::CharacterStats {
			[ModifiesAccessibility]
			public new bool m_stronghold;
		}

		[NewMember]
		public  static GameObject modelViewerBackground;
		[NewMember]
		public  static UITexture GameBrowserBackground;

		[NewMember]
		public  static void ExtractMemorials()
		{
			string xmlPath = PathHelper.Combine(Application.dataPath, "Managed/iemod", "MemorialEntries.xml");
			string xml = Resources.Load("Data/UI/BackerMemorials").ToString();

			File.WriteAllText(xmlPath, xml);
			Console.AddMessage("Extraction: done.");
		}

		[NewMember]
		public  static void CC()
		{
			GameState.s_playerCharacter.GetComponent<Mover>().UseWalkSpeed();
		}

		[NewMember]
		public  static void SetDefaultZoom(float value)
		{
			IEModOptions.DefaultZoom = value;
			global::Console.AddMessage("Default zoom set to: " + value + ". Reminder: game's vanilla value is 1.");
		}

		[NewMember]
		public  static void DisableBackerDialogues(bool state)
		{
			if (state) {
				IEModOptions.DisableBackerDialogs = true;
				global::Console.AddMessage("If you're using the \"Rename backers\" mod, backer dialogues will now be DISABLED as soon as you transition to another area or reload a save.");
			}
			else {
				IEModOptions.DisableBackerDialogs = false;
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
        public static void UnlockSoulbound(Guid character)
        {
            try
            {
                EquipmentSoulbind component;
                Equipment componentByGuid = Scripts.GetComponentByGuid<Equipment>(character);
                if (!componentByGuid)
                {
                    global::Console.AddMessage("Found no soulbound weapon.", Color.red);
                    return;
                }
                Equippable itemInSlot = componentByGuid.CurrentItems.GetItemInSlot(Equippable.EquipmentSlot.PrimaryWeapon);
                if (!itemInSlot)
                {
                    global::Console.AddMessage("Found no soulbound weapon.", Color.red);
                    return;
                }
                else
                {
                    component = itemInSlot.GetComponent<EquipmentSoulbind>();

                    if(component == null)
                    {
                        global::Console.AddMessage("Found no soulbound weapon.", Color.red);
                        return;
                    }
                }

                IEDebug.Log(string.Format("Found item {0}", component.name));
                mod_EquipmentSoulbind componentAsSoulbind = (mod_EquipmentSoulbind)component;

                componentAsSoulbind.ForceUnlock();
               
            }
            catch (Exception ex)
            {
                IEDebug.Log(ex.ToString());
                throw new IEModException("Failed to unlock soulbound", ex);
            }
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
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
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
			if (AchievementTracker.Instance.DisableAchievements)
			{
				AchievementTracker.Instance.m_disableAchievements = false;
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
		public static void PlayerPrefs_DeleteAll(bool confirmation) {
			if (!confirmation) {
				Console.AddMessage("You need to supply a 'true' argument if you're sure you want to clear all preferences.");
				return;
			}
			
			PlayerPrefs.DeleteAll();
			Console.AddMessage("All preferences cleared. Please restart the game so that no errors occur.");
		}

	    [NewMember]
	    public static void PlayerPrefs_Delete(string name) {
		    if (!PlayerPrefs.HasKey(name)) {
			    Console.AddMessage("A key with this name was not found in PlayerPrefs.");
		    }
		    PlayerPrefs.DeleteKey(name);
	    }

		[NewMember]
	    public static void ResetCustomUI() {
			IEModOptions.Layout = UICustomizer.DefaultLayout.Clone();
			IEModOptions.SaveToPrefs();
			Console.AddMessage("Successfuly reset the UI to default.");
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

        /// <summary>
        /// This method fixes a bug in the original Skill command (that only applied the new value as your current bonus,
        ///     not actually replacing the score). This bug was still present in v2.0 of PoE.
        /// </summary>
        /// <param name="character">The character to modify. The Guid will be filled in by CommandLineRun before we get here</param>
        /// <param name="skill">The skill to modify. This will be converted from string to Enum by CommandLineRun</param>
        /// <param name="score">The new score value to assign. This will be validated as a number by CommandLineRun. Note
        ///     that this is NOT the actual score, it's the "points invested" in score. Thus to attain a score of 9, you
        ///     would need to pass in 45 (the sum of 1 to 9). </param>
        [ModifiesMember("Skill")]
        public static void Skill(Guid character, CharacterStats.SkillType skill, int score)
        {
            CharacterStats characterStatsComponent = Scripts.GetCharacterStatsComponent(character);
            if (characterStatsComponent == null)
            {
                Debug.Log(string.Concat("Skill: Error - stats component not found for '", character, "'."));
                return;
            }
			
            switch (skill)
            {
                case CharacterStats.SkillType.Stealth:
                    {
                        characterStatsComponent.StealthSkill = score;
                        break;
                    }
                case CharacterStats.SkillType.Athletics:
                    {
                        characterStatsComponent.AthleticsSkill = score;
                        break;
                    }
                case CharacterStats.SkillType.Lore:
                    {
                        characterStatsComponent.LoreSkill = score;
                        break;
                    }
                case CharacterStats.SkillType.Mechanics:
                    {
                        characterStatsComponent.MechanicsSkill = score;
                        break;
                    }
                case CharacterStats.SkillType.Survival:
                    {
                        characterStatsComponent.SurvivalSkill = score;
                        break;
                    }
                case CharacterStats.SkillType.Crafting:
                    {
                        characterStatsComponent.CraftingSkill = score;
                        break;
                    }
            }
            Console.AddMessage(string.Concat(new object[] { characterStatsComponent.name, "'s ", skill, " is now ", score.ToString() }));
        }


        [NewMember]
		public  static void RenameCreature(string guid, string newname)
		{
			GameObject npc = UnityEngine.GameObject.Find(guid);
			if (npc != null && npc.GetComponent<CharacterStats>() != null)
				npc.GetComponent<CharacterStats>().OverrideName = newname;
		}


		[NewMember]
		[Cheat]
		public static void ShowMouseDebug() {
			GameCursor.ShowDebug = !GameCursor.ShowDebug;
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

		// for instance: BSC cre_druid_cat01 true
		[NewMember]
		public static void BSC(string prefabName, int intIsHostile)
		{
			if (GameState.s_playerCharacter.IsMouseOnWalkMesh()) {
				var isHostile = intIsHostile > 0;
				Console.AddMessage($"Spawning ${(isHostile  ? "Hostile" : "Friendly")}: ${prefabName}", Color.green);
				var newCreature = GameResources.LoadPrefab<UnityEngine.GameObject>(prefabName, true);
				if (newCreature != null)
				{
					newCreature.transform.position = GameInput.WorldMousePosition;
					newCreature.transform.rotation = GameState.s_playerCharacter.transform.rotation;
					var faction = newCreature.Component<Faction>();
					faction.RelationshipToPlayer = isHostile ? Faction.Relationship.Hostile : Faction.Relationship.Neutral;
					faction.UnitHostileToPlayer = isHostile;
					var teamTag = isHostile ? "monster" : "player";
					faction.CurrentTeamInstance = Team.GetTeamByTag(teamTag);
					var aiPackage = newCreature.Component<AIPackageController>();
					aiPackage.ChangeBehavior(AIPackageController.PackageType.DefaultAI);
					aiPackage.InitAI();
					global::CameraControl.Instance.FocusOnPoint(newCreature.transform.position);
				}
				else
					global::Console.AddMessage("Failed to spawn " + prefabName + " - probably bad naming.", UnityEngine.Color.red);
			}
			else
				global::Console.AddMessage("Mouse is not on navmesh, move mouse elsewhere and try again.", UnityEngine.Color.red);
		}



		// this method gives your maincharacter all existing mage spells... it was just to test something, but someone might want to use some bits of it
		[NewMember]
		public static void AdAb()
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
					AbilityProgressionTable.AddAbilityToCharacter(abil.Ability.name,firstparam, false);
			}
		}


		[NewMember]
		public static void DeleteIEModSettings(bool areYouSure) {
			if (!areYouSure) {
				Console.AddMessage("You need to pass 'true' if you really want to delete all settings.", Color.red);
				return;
			}
			IEModOptions.DeleteAllSettings();
			Console.AddMessage("All settings have been deleted.", Color.green);
		}

		[NewMember]
		public  static void SelectCircles(float width)
		{
			global::Console.AddMessage("Setting selection circle width to: " + width, Color.green);
			InGameHUD.Instance.SelectionCircleWidth = width;
			InGameHUD.Instance.EngagedCircleWidth = width;
			IEModOptions.SelectionCircleWidth = width;
		}

		[NewMember]
		public static void BB()
		{
			UICustomizer.ShowInterface(true);
		}

		[NewMember]
		public static void TT()
		{
			if (((Mod_OnGUI_Player)GameState.s_playerCharacter).showGameObjectBrowser == false)
			{
				if (((Mod_OnGUI_Player)GameState.s_playerCharacter).inspecting == null)
					((Mod_OnGUI_Player)GameState.s_playerCharacter).inspecting = UICustomizer.UiCamera.transform;

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
		
		/// <summary>
        /// Method serving as a band-aid fix for the stats not sticking after reloads in 3.01
        /// Looks in an xml file located in Managed/iemod/customStats and extracts the values for NPC stats     
        /// located inside it. Inspired heavily by the FindCharacter and AttributeScore methods in the vanilla dll.
        /// </summary>
        /// <param name="filename">Name of the .xml file containing the stats to be applied to all NPCs</param>
        [NewMember]
        public static void ImportStats(string fileName)
        {
            // Load document
            XmlDocument doc = new XmlDocument();
            string xmlPath = PathHelper.Combine(Application.dataPath, "Managed/iemod/customStats", (fileName+".xml"));
            try
            {
                doc.Load(xmlPath);
            }
            catch (FileNotFoundException e)
            {
                Console.AddMessage("File not found");
                return;
            }

            // Retrieve all CharStats in current game
            CharacterStats[] characterStatsArray = UnityEngine.Object.FindObjectsOfType<CharacterStats>();
            foreach (XmlNode node in doc.DocumentElement)
            {
               
                string npcName = node.Attributes[0].Value;
                //Find Character sheet associated with that name, heavily insipired by the orignial FindCharacter command
                for (int i = 0; i < (int)characterStatsArray.Length; i++)
                {
                    CharacterStats characterStat = characterStatsArray[i];
                    if (characterStat.name.ToLower().Contains(npcName.ToLower()) || CharacterStats.Name(characterStat).ToLower().Contains(npcName.ToLower()))
                    {
                        // Switch stats arround accordingly
                        characterStat.BaseMight = int.Parse(node["Might"].InnerText);
                        characterStat.BaseConstitution = int.Parse(node["Constitution"].InnerText);
                        characterStat.BaseDexterity = int.Parse(node["Dexterity"].InnerText);
                        characterStat.BasePerception = int.Parse(node["Perception"].InnerText);
                        characterStat.BaseIntellect = int.Parse(node["Intellect"].InnerText);
                        characterStat.BaseResolve = int.Parse(node["Resolve"].InnerText);

                        //  Display change in console... Mostly for testing and paranoia 
                        Console.AddMessage(string.Concat(new string[] { characterStat.name, "'s stats have been imported from ", fileName }));

                        // Break out of inner loop once that NPC has been adjusted... ugly but probably quicker than anything else I know....
                        break;
                    }
                }
            }
        }

	}

    [ModifiesType("CommandLineRun")]
    public class mod_CommandLineRun
    {
        /* * * TJH 8/26/2015 - It's no longer necessary to override RunCommand. We can just make sure all methods are always
            available and not treated as cheats * * */
/*
[ModifiesMember("MethodIsAvailable")]
public static bool MethodIsAvailable(MethodInfo method)
{
    return true;
}

    }

    [ModifiesType]
public class mod_EquipmentSoulbind : EquipmentSoulbind
{

    [NewMember]
    public void ForceUnlock()
    {
        if (this.AreUnlocksComplete)
        {
            return;
        }

        this.UnlockLevel = this.m_NextUnlockLevel;
        this.UnlockProgress = 0f;
        this.DegenerateUnlockProgress = 0f;
        ItemMod[] modsToApply = this.Unlocks[this.UnlockLevel].ModsToApply;
        for (int i = 0; i < (int)modsToApply.Length; i++)
        {
            ItemMod itemMod = modsToApply[i];
            this.m_Equippable.AttachItemMod(itemMod);
        }
        ItemMod[] modsToRemove = this.Unlocks[this.UnlockLevel].ModsToRemove;
        for (int j = 0; j < (int)modsToRemove.Length; j++)
        {
            ItemMod itemMod1 = modsToRemove[j];
            this.m_Equippable.DestroyFirstMod(itemMod1);
        }
        if (!UIItemInspectManager.ReloadWindowsForObject(base.gameObject, true))
        {
            UIItemInspectManager.ExamineSoulbindUnlock(this, this.m_Equippable.EquippedOwner);
        }
        this.TryUnlockNext();
    }

}
*/