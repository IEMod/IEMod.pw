using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using IEMod.Helpers;
using IEMod.Mods.PartyBar;
using IEMod.Mods.UICustomization;
using IEMod.QuickControls;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Options {

	[NewType]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class SaveAttribute : Attribute {
		
	}

	/// <summary>
	/// Nullable types don't seem to be easily serialized to XML.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[NewType]
	public class XmlNullable<T> {
		public XmlNullable(T value) {
			Value = value;
		}

		public XmlNullable() {
		}

		public T Value {
			get;
			set;
		}

		public static implicit operator XmlNullable<T>(T value) {
			return new XmlNullable<T>(value);
		}
	}

	[NewType]
	public static class IEModOptions {
		[NewType]
		[XmlInclude(typeof(Vector3))]
		public class LayoutOptions {

			public bool BuffsSideLeft;

			public Vector3 FormationPosition;

			public Vector3 PartyBarPosition;

			public Vector3 PartySolidHudPosition;

			public Vector3 HudPosition;

			public Vector3 AbilitiesBarPosition;

			public Vector3 LeftHudBarPosition;

			public Vector3 RightHudBarPosition;

			public Vector3 ClockPosition;

			public Vector3 LogPosition;

			public Vector3 CustomizeButtonPosition;

			public bool HudHorizontal;

			public bool HudTextureHidden;

			public bool LogButtonsLeft;

			public bool ButtonsBackground;

			public bool UsingCustomTextures;

			public bool PortraitHighlightsDisabled;

			public bool PartyBarHorizontal;

			public bool TooltipOffset;

			public string FramePath;

			public LayoutOptions Clone() {
				return (LayoutOptions)this.MemberwiseClone();
			}

            public override bool Equals(object obj)
            {
                if (!(obj is LayoutOptions))
                {
                    return false;
                }

                LayoutOptions other = obj as LayoutOptions;

                return this.AbilitiesBarPosition == other.AbilitiesBarPosition &&
                    this.BuffsSideLeft == other.BuffsSideLeft &&
                    this.ButtonsBackground == other.ButtonsBackground &&
                    this.ClockPosition == other.ClockPosition &&
                    this.CustomizeButtonPosition == other.CustomizeButtonPosition &&
                    this.FormationPosition == other.FormationPosition &&
                    this.FramePath == other.FramePath &&
                    this.HudHorizontal == other.HudHorizontal &&
                    this.HudPosition == other.HudPosition &&
                    this.HudTextureHidden == other.HudTextureHidden &&
                    this.LeftHudBarPosition == other.LeftHudBarPosition &&
                    this.LogButtonsLeft == other.LogButtonsLeft &&
                    this.LogPosition == other.LogPosition &&
                    this.PartyBarHorizontal == other.PartyBarHorizontal &&
                    this.PartyBarPosition == other.PartyBarPosition &&
                    this.PartySolidHudPosition == other.PartySolidHudPosition &&
                    this.PortraitHighlightsDisabled == other.PortraitHighlightsDisabled &&
                    this.RightHudBarPosition == other.RightHudBarPosition &&
                    this.TooltipOffset == other.TooltipOffset &&
                    this.UsingCustomTextures == other.UsingCustomTextures;
            }
        }




		private static bool _enableCustomUi;

		//GR 28/8/15 - this was out of good intentions, but... yeah... it doesn't work. I hadn't noticed on my machine circumstances made it seem like it did.
		//GR 30/8 - I'm just gonna disable this for now.
		[Save]
		public static XmlNullable<float> SelectionCircleWidth = null;

		[Save]
		public static  float DefaultZoom;
        
		[Save]
		public static LayoutOptions Layout = new LayoutOptions();

		[Save]
		[Description("Display selection circles for neutral NPCs at all times.")]
		[Label("Always show circles")]
		public static bool AlwaysShowCircles = false;

		[Save]
		[XmlElement]
		[Label("One tooltip at a time")]
		[Description("When holding down TAB, displays only one tooltip - for the hovered character.")]
		public static bool OneTooltip = false;

		[Save]
		[Label("Blue selection circles")]
		[Description("Make selection circles for neutral NPCs blue. \n(colorblind mode must be disabled)")]
		public static bool BlueCircles;

		[Save]
		[Label("IE-Like blue")]
		[Description("Blue selection circles become cyan, like in IE games. (requires exit to main menu)")
		]
		public static bool BlueCirclesBG;

		[Save]
		[Label("Unlock combat inventory/loot")]
		[Description(
			"Allows looting containers during combat, transfering items between party members, as well as equipping and unequipping all gear, except body armor."
			)]
		public static bool UnlockCombatInv;

		[Save]
		[Label("Fantasy names for backers")]
		[Description(
			"Some backer names can be immersion breaking, so this mod replaces them with random fantasy names based on their race and gender. Takes effect after reloading or transitionning."
			)]
		public static bool FixBackerNames;

		public static bool SaveBeforeTransition;

		public static int SaveInterval;

		[Save]
		[Label("Autosave setting")]
		[Description("Auto save setting")]
		public static AutoSaveSetting AutosaveSetting {
			get {
				return _autoSaveSetting;
			}
			set {
				_autoSaveSetting = value;
				OnAutosaveSettingChanged();
			}
		}

		private static void OnAutosaveSettingChanged() {
			switch (AutosaveSetting) {
				case AutoSaveSetting.SaveAfter15:
				case AutoSaveSetting.SaveBefore15:
					SaveInterval = 15;
					break;
				case AutoSaveSetting.SaveAfter30:
				case AutoSaveSetting.SaveBefore30:
					SaveInterval = 30;
					break;
				case AutoSaveSetting.Default:
				case AutoSaveSetting.SaveBefore:
					SaveInterval = 0;
					break;
				case AutoSaveSetting.DisableAutosave:
					SaveInterval = -1;
					break;
				default:
					throw IEDebug.Exception(null, $"Invalid AutoSaveSetting: {AutosaveSetting}");
			}
			SaveBeforeTransition = AutosaveSetting.ToString().Contains("Before");
		}

		[Save]
		[Label("Fix moving recovery rate")]
		[Description("This mod removes additional recovery rate penalty for moving characters.")]
		public static bool RemoveMovingRecovery;

		[Save]
		[Label("Fast scouting mode")]
		[Description(
			"This mod makes Scouting Mode move at normal running speed (instead of walking speed).  Note: when enemies are visible, your scouting movement speed is reduced to walking speed"
			)]
		public static FastSneakOptions FastSneak;

		[Save]
		[Label("Improved AI")]
		[Description("Some improvements to the combat AI.")]
		public static bool ImprovedAI;

		[Save]
		[Label("Nerfed XP table")]
		[Description(
			"Increases experience needed. Note: You may need to use ChangeClass to de-level if enabling/increasing this setting midgame."
			)]
		public static NerfedXpTable NerfedXPTableSetting;

		[Save]
		[Label("Loot shuffler")]
		[Description("Random loot will change on every reload. (Loot is set when opening a container.)")]
		public static bool LootShuffler;

		[Save]
		[Label("Game speed mod")]
		[Description("Holding control when toggling fast or slow mode will use more extreme speeds.")]
		public static bool GameSpeedMod;

		[Save]
		[Label("Remove Combat-Only Restrictions")]
		[Description("Allows most spells, abilities, items, and consumables to function outside of combat. (Warning: this can significantly affect game balance, and will cause bugs. Be careful while saving with combat-only effects active.)"
			)]
		public static bool CombatOnlyMod;

		[Save]
		[Label("Per-Encounter Spells Mod")]
		[Description(
			"Modifies which levels of Wizard, Priest and Druid spells are treated as per-encounter.")]
		public static PerEncounterSpells PerEncounterSpellsSetting;

        [Save]
        [Label("Cipher Base Focus")]
        [Description(
            "Modifies the amount of Focus Ciphers begin combat with.")]
        public static CipherStartingFocus CipherStartingFocusSetting;



        [Save]
		[Label("NPC Disposition Fix")]
		[Description(
			"Applies disposition-based bonuses to NPC paladins and priests. Patches in favored and disfavored dispositions for Pallegina's order."
			)]
		public static bool NPCDispositionFix;

		[Save]
		[Label("Pallegina's Favored Dispositions")]
		[Description("Favored dispositions for Pallegina's Order - Brotherhood of the Five Suns.")]
		public static Disposition.Axis PalleginaFavored1;

		[Save]
		public static Disposition.Axis PalleginaFavored2;

		[Save]
		[Label("Disable Backer Dialogs")]
		[Description("Disables talking to backer NPCs.")]
		public static bool DisableBackerDialogs;

		[Save]
		[Label("Minimize tombstones")]
		[Description("Minimizes the interaction and visibility of tombstones.")]
		public static bool MinimizeTombstones;

		[Save]
		[Label("Pallegina's Disfavored Dispositions")]
		[Description("Disfavored dispositions for Pallegina's Order - Brotherhood of the Five Suns.")]
		public static Disposition.Axis PalleginaDisfavored1;

		[Save]
		public static Disposition.Axis PalleginaDisfavored2;

		[Save]
		[Label("Disable friendly fire")]
		[Description("Disable Friendly Fire")]
		public static bool DisableFriendlyFire;

		[Save]
		[Label("Bonus spells")]
		[Description(
			"Calculates bonus spells per rest based on a caster's Intellect. The first bonus spell for a given spell level is gained at an Intellect score of 14 + [Spell Level].  Another bonus spell for that level is added for every 4 additional Intellect points."
			)]
		public static bool BonusSpellsPerDay;

		[Save]
		[Label("Target turned enemies")]
		[Description(
			"Enemies that have temporarily switched to your side are still considered hostile against your abilities, like area attacks. Doesn't change how turned allies are targeted."
			)]
		public static bool TargetTurnedEnemies;

		[Save]
		[Label("Extra Wizard Preparation Slots")]
		[Description(
			"Adds extra spell slots to a wizard's grimoire.  If you change this option in game, you will need to return to the main menu and reload before the change takes effect."
			)]
		public static ExtraSpellsInGrimoire ExtraWizardSpells;

        private static MaxAdventurersOptions _maxAdventurersCount;
        [Save]
        [Label("Max adventurers you can hire")]
        [Description(
            "Changes the maximum count of adventurers you can hire in tavern."
            )]
        public static MaxAdventurersOptions MaxAdventurersCount {
            get {
                return _maxAdventurersCount;
            }
            set {
                _maxAdventurersCount = value;
                MaxAdventurers.mod_PartyMemberAI.newUpdateMaxAdventurers();
            }
        }

        private static Dictionary<string, FieldInfo> _fieldCache;

		public static Dictionary<string, FieldInfo> FieldCache {
			get {
				if (_fieldCache == null) {
					_fieldCache =
						typeof (IEModOptions).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance)
						.Where(x => x.GetCustomAttribute<SaveAttribute>() != null)
							.ToDictionary(x => x.Name, x => x);
				}
				return _fieldCache;
			}
		}

		public static Dictionary<string, PropertyInfo> PropertyCache {
			get {
				if (_propertyCache == null) {
					_propertyCache =
						typeof (IEModOptions).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance)
						.Where(x => x.GetCustomAttribute<SaveAttribute>() != null)
							.ToDictionary(x => x.Name, x => x);
				}
				return _propertyCache;
			}
		}

		[Save]
		[Label("UI Customization")]
		[Description("Enables UI customization. You need to exit to main menu and reload to toggle this option properly.")]
		public static bool EnableCustomUi {
			get {
				return _enableCustomUi;
			}
			set {
				_enableCustomUi = value;
			}
		}

		[Save]
		[Label("Disable engagement")]
		[Description("Engagement begone. You need to exit to main menu and reload to reenable engagement.")]
		public static bool DisableEngagement;

        [Save]
        [Label("Hide Anticlass Spells from Ability Bar")]
        [Description("Hides Anticlass spells from showing up on the ability bar and adds them to a spell submenu of an existing class.  For example, if you add Druid spells to your Wizard, the added Druid spells will show up under the Wizard spells submenu of the appropriate level.")]
        public static bool HideAnticlassSpells;

        private static AutoSaveSetting _autoSaveSetting;
		private static Dictionary<string, PropertyInfo> _propertyCache;

		public static void LoadFromPrefs() {
			foreach (var field in FieldCache.Values) {
				var fieldType = field.FieldType;
				var value = PlayerPrefsHelper.GetObject(field.Name, fieldType);
				field.SetValue(null, value);
			}
			foreach (var property in PropertyCache.Values) {
				var fieldType = property.PropertyType;
				var value = PlayerPrefsHelper.GetObject(property.Name, fieldType);
				property.SetValue(null, value, null);
			}
		}

		public static string GetSettingName(string memberName) {
			return memberName;
		}

		public static void DeleteAllSettings() {
			foreach (var field in FieldCache.Values) {
				PlayerPrefs.DeleteKey(GetSettingName(field.Name));
			}
		}

		public static void SaveToPrefs() {
			foreach (var field in FieldCache.Values) {
				var fieldType = field.FieldType;
				var value = field.GetValue(null);
				PlayerPrefsHelper.SetObject(GetSettingName(field.Name), fieldType, value);
			}
			foreach (var field in PropertyCache.Values) {
				var fieldType = field.PropertyType;
				var value = field.GetValue(null, null);
				PlayerPrefsHelper.SetObject(GetSettingName(field.Name), fieldType, value);
			}
		}

		public static bool IsIdenticalToPrefs() {
			foreach (var field in FieldCache.Values) {
				var myValue = field.GetValue(null);
				var prefValue = PlayerPrefsHelper.GetObject(GetSettingName(field.Name), field.FieldType);
				if (!Equals(myValue, prefValue)) {
                    return false;
				}
			}
			foreach (var field in PropertyCache.Values) {
				var myValue = field.GetValue(null, null);
				var prefValue = PlayerPrefsHelper.GetObject(GetSettingName(field.Name), field.PropertyType);
				if (!Equals(myValue, prefValue)) {
                    return false;
				}
			}
			return true;
		}

		[NewType]
		public enum AutoSaveSetting {
			[Description("Save after every area transition (standard)")]
			Default,

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
			[Description("No Change")]
			NoChange,

			[Description("Per-encounter spells gained at levels 9 and 12")]
			Levels_9_12,

			[Description("Per-encounter spells gained at levels 6, 9 and 12")]
			Levels_6_9_12,

            [Description("Per-encounter spells gained at levels 8, 10, 12, and 14")]
            Levels_8_10_12_14,

            [Description("Per-encounter spells gained at levels 6, 9, 12 and 14")]
            Levels_6_9_12_14,

            [Description("Per-encounter spells gained at levels 6, 8, 10, 12 and 14")]
			Levels_6_8_10_12_14,

			[Description("Per-encounter spells gained at levels 4, 6, 8, 10, 12 and 14")]
			Levels_4_6_8_10_12_14,

			[Description("All spells per-encounter")]
			AllPerEncounter,

			[Description("All spells per-rest")]
			AllPerRest
		}

        [NewType]
        public enum CipherStartingFocus
        {
            [Description("No Change (1/4 Max Focus)")]
            Quarter,

            [Description("1/2 Max Focus")]
            Half,

            [Description("3/4 Max Focus")]
            ThreeQuarter,

            [Description("Max Focus")]
            Max,

            [Description("No Focus")]
            None
        }

        [NewType]
		public enum ExtraSpellsInGrimoire {
			[Description("No extra preparation slots.")]
			None = 0,

			[Description("One (1) extra preparation slot.")]
			One = 1,

			[Description("Two (2) extra preparation slots.")]
			Two = 2,

			[Description("Three (3) extra preparation slots.")]
			Three = 3
		}

        [NewType]
        public enum FastSneakOptions
        {
            [Description("Normal")]
            Normal = 0,

            [Description("Fast Scouting, Party LoS")]
            FastScoutingAllLOS = 1,

            [Description("Fast Scouting, Individual LoS")]
            FastScoutingSingleLOS = 2
        }

        [NewType]
        public enum MaxAdventurersOptions
        {
            [Description("Normal (8)")]
            Normal_8,
            [Description("Double (16)")]
            Double_16,
            [Description("Triple (24)")]
            Triple_24,
            [Description("Quadra (32)")]
            Quadra_32,
            [Description("One hundred twenty eight (128)")]
            OneHundredTwentyEight
        }
    }
}