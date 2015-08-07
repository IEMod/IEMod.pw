using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using IEMod.Helpers;
using Patchwork.Attributes;

namespace IEMod.Mods.Options {
	[NewType]
	public static class IEModOptions {
		[Description("Display selection circles for neutral NPCs at all times.")]
		[Label("Always show circles")]
		public static bool AlwaysShowCircles = false;

		[Label("One tooltip at a time")]
		[Description("When holding down TAB, displays only one tooltip - for the hovered character.")]
		public static bool OneTooltip = false;

		[Label("Disable engagement")]
		[Description("Engagement begone.")]
		public static bool DisableEngagement;

		[Label("Blue selection circles")]
		[Description("Make selection circles for neutral NPCs blue. \n(colorblind mode must be disabled)")
		]
		public static bool BlueCircles;

		[Label("IE-Like blue")]
		[Description("Blue selection circles become cyan, like in IE games. (requires exit to main menu)")
		]
		public static bool BlueCirclesBG;

		[Label("Unlock combat inventory/loot")]
		[Description(
			"Allows looting containers during combat, transfering items between party members, as well as equipping and unequipping all gear, except body armor."
			)]
		public static bool UnlockCombatInv;

		[Label("Fantasy names for backers")]
		[Description(
			"Some backer names can be immersion breaking, so this mod replaces them with random fantasy names based on their race and gender. Takes effect after reloading or transitionning."
			)]
		public static bool FixBackerNames;

		public static bool SaveBeforeTransition;

		public static int SaveInterval;

		[Label("Autosave setting")]
		public static AutoSaveSetting AutosaveSetting;

		[Label("Fix moving recovery rate")]
		[Description("This mod removes additional recovery rate penalty for moving characters.")]
		public static bool RemoveMovingRecovery;

		[Label("Fast scouting mode")]
		[Description(
			"This mod makes Scouting Mode move at normal running speed (instead of walking speed).  Note: when enemies are visible, your scouting movement speed is reduced to walking speed"
			)]
		public static bool FastSneak;

		[Label("Improved AI")]
		[Description("Some improvements to the combat AI.")]
		public static bool ImprovedAI;

		[Label("Nerfed XP table")]
		[Description(
			"Increases experience needed. Note: You may need to use ChangeClass to de-level if enabling/increasing this setting midgame."
			)]
		public static NerfedXpTable NerfedXPTableSetting;

		[Label("Loot shuffler")]
		[Description("Random loot will change on every reload. (Loot is set when opening a container.)")]
		public static bool LootShuffler;

		[Label("Game speed mod")]
		[Description("Holding control when toggling fast or slow mode will use more extreme speeds.")]
		public static bool GameSpeedMod;

		[Label("Remove Combat-Only Restrictions")]
		[Description(
			"Allows all spells and abilities to function outside of combat. (Warning: this can significantly affect game balance, and possibly could cause bugs.)"
			)]
		public static bool CombatOnlyMod;

		[Label("Per-Encounter Spells Mod")]
		[Description(
			"Modifies which levels of Wizard, Priest and Druid spells are treated as per-encounter.")]
		public static PerEncounterSpells PerEncounterSpellsSetting;

		[Label("NPC Disposition Fix")]
		[Description(
			"Applies disposition-based bonuses to NPC paladins and priests. Patches in favored and disfavored dispositions for Pallegina's order."
			)]
		public static bool NPCDispositionFix;

		[Label("Pallegina's Favored Dispositions")]
		[Description("Favored dispositions for Pallegina's Order - Brotherhood of the Five Suns.")]
		public static Disposition.Axis PalleginaFavored1;

		public static Disposition.Axis PalleginaFavored2;

		[Label("Pallegina's Disfavored Dispositions")]
		[Description("Disfavored dispositions for Pallegina's Order - Brotherhood of the Five Suns.")]
		public static Disposition.Axis PalleginaDisfavored1;

		public static Disposition.Axis PalleginaDisfavored2;

		[Label("Disable friendly fire")]
		[Description("Disable Friendly Fire")]
		public static bool DisableFriendlyFire;

		[Label("Bonus spells")]
		[Description(
			"Calculates bonus spells per rest based on a caster's Intellect. The first bonus spell for a given spell level is gained at an Intellect score of 14 + [Spell Level].  Another bonus spell for that level is added for every 4 additional Intellect points."
			)]
		public static bool BonusSpellsPerDay;

		[Label("Target turned enemies")]
		[Description(
			"Enemies that have temporarily switched to your side are still considered hostile against your abilities, like area attacks. Doesn't change how turned allies are targeted."
			)]
		public static bool TargetTurnedEnemies;

		[Label("Extra Wizard Preparation Slots")]
		[Description(
			"Adds extra spell slots to a wizard's grimoire.  If you change this option in game, you will need to return to the main menu and reload before the change takes effect."
			)]
		public static ExtraSpellsInGrimoire ExtraWizardSpells;

		private static Dictionary<string, FieldInfo> _fieldCache;

		public static Dictionary<string, FieldInfo> FieldCache {
			get {
				if (_fieldCache == null) {
					_fieldCache =
						typeof (IEModOptions).GetFields(BindingFlags.Public | BindingFlags.Static)
							.ToDictionary(x => x.Name, x => x);
				}
				return _fieldCache;
			}
		}

		public static void LoadFromPrefs() {
			foreach (var field in FieldCache.Values) {
				var fieldType = field.FieldType;
				var value = PlayerPrefsHelper.GetObject(field.Name, fieldType);
				field.SetValue(null, value);
			}
		}

		public static void SaveToPrefs() {
			foreach (var field in FieldCache.Values) {
				var fieldType = field.FieldType;
				var value = field.GetValue(null);
				PlayerPrefsHelper.SetObject(field.Name, fieldType, value);
			}
		}

		public static bool IsIdenticalToPrefs() {
			foreach (var field in FieldCache.Values) {
				var myValue = field.GetValue(null);
				var prefValue = PlayerPrefsHelper.GetObject(field.Name, field.FieldType);
				if (!Equals(myValue, prefValue)) {
					return false;
				}
			}
			return true;
		}

		[NewType]
		public enum AutoSaveSetting {
			[Description("Save after every area transition (standard)")]
			SaveAfter,

			[Description("Save after area transitions, but only once per 15 minutes")]
			SaveAfter15,

			[Description("Save after area transitions, but only once per 30 minutes")]
			SaveAfter30,

			[Description("Save before every area transition")]
			SaveBefore,

			[Description("Save before area transitions, but only once per 15 minutes")]
			SaveBefore15,

			[Description("Save before area transitions, but only once per 30 minutes")]
			SaveBefore30,

			[Description("Disable autosave")]
			DisableAutosave
		}

		[NewType]
		public enum NerfedXpTable {
			[Description("Disabled")]
			Disabled,

			[Description("25% increase: 1250,3750,...82,500")]
			Increase25,

			[Description("50% increase: 1500,4500,...99,000")]
			Increase50,

			[Description("Square progression: 1000,4000,...121,000")]
			Square
		}

		[NewType]
		public enum PerEncounterSpells {
			[Description("No Change (Per-encounter spell level gained at level 9)")]
			NoChange,

			[Description("Per-encounter spell levels gained at levels 9 and 12")]
			Levels_9_12,

			[Description("Per-encounter spell levels gained at levels 6, 9 and 12")]
			Levels_6_9_12,

			[Description("Per-encounter spell levels gained at levels 6, 8, 10 and 12")]
			Levels_6_8_10_12,

			[Description("Per-encounter spell levels gained at levels 4, 6, 8, 10 and 12")]
			Levels_4_6_8_10_12,

			[Description("All spells per-encounter")]
			AllPerEncounter,

			[Description("All spells per-rest")]
			AllPerRest
		}

		[NewType]
		public enum ExtraSpellsInGrimoire {
			[Description("No extra preparation slots.")]
			None,

			[Description("One (1) extra preparation slot.")]
			One,

			[Description("Two (2) extra preparation slots.")]
			Two,

			[Description("Three (3) extra preparation slots.")]
			Three
		}
	}
}