using System;
using System.IO;
using System.Linq;
using IEMod.Helpers;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Options {



	[ModifiesType]
	public class mod_UIOptionsManager : UIOptionsManager
	{
		[ReplaceType("UIOptionsManager/OptionsPage")]
		public new enum OptionsPage
		{
			MENU = 1,
			GAME = 2,
			AUTOPAUSE = 4,
			DIFFICULTY = 8,
			GRAPHICS = 16,
			SOUND = 32,
			CONTROLS = 64,
			CUSTOM = 128,
			CUSTOM2 = 256
		}
		[ModifiesType("MappedInput")]
		public class Mod4_MappedInput {
			[ModifiesAccessibility]
			public static bool m_ControlNames;
			[ModifiesAccessibility]
			public static bool ReadOnlyControls;
		}
		[NewMember]
		private GameObject _blueCircles;
		[NewMember]
		private GameObject _blueCirclesBg;
		[NewMember]
		private GameObject _alwaysShowCircles;
		[NewMember]
		private GameObject _unlockCombatInv;
		[NewMember]
		private GameObject _fixBackerNames;
		[NewMember]
		private GameObject _removeMovingRecovery;
		[NewMember]
		private GameObject _fastSneak;
		[NewMember]
		private GameObject _improvedAi;
		[NewMember]
		private GameObject _disableFfCb;
		[NewMember]
		private GameObject _oneTooltip;
		[NewMember]
		private GameObject _disableEngagement;
		[NewMember]
		private GameObject _nerfedXpCmb;
		[NewMember]
		private GameObject _lootShuffler;
		[NewMember]
		private GameObject _gameSpeed;
		[NewMember]
		private GameObject _combatOnly;
		[NewMember]
		private GameObject _bonusSpellsPerDay;
		[NewMember]
		private GameObject _targetTurnedEnemies;
		[NewMember]
		private GameObject _npcDispositionFix;
		[NewMember]
		private GameObject _perEncounterSpellsCmb;
		[NewMember]
		private GameObject _extraGrimoireSpellsCmb;
		[NewMember]
		private GameObject _autosaveCmb;

		
		[NewMember]
		[DuplicatesBody("Start")]
		public void StartOrig() {
			throw new DeadEndException("StartOrig");
		}

		[ModifiesMember("Start")]
		private void StartNew() {
			var exampleCheckbox =
				base.GetComponentsInChildren<UIOptionsTag>(true).Single(
					opt => opt.Checkbox && opt.BoolSuboption == GameOption.BoolOption.SCREEN_EDGE_SCROLLING)
					.transform;

			var exampleDropdown = this.ResolutionDropdown.transform.parent;
			var pageParent = Pages[5].transform.parent;

			var controlFactory = new IEControlFactory {
				ExampleCheckbox = exampleCheckbox.gameObject,
				ExamplePage = Pages[5],
				CurrentParent = pageParent,
				ExampleComboBox = exampleDropdown.gameObject
			};

			var ieModOptions = controlFactory.Page("IEModOptions");
			var ieModDisposition = controlFactory.Page("IEModOptions_Disposition");
			Pages = Pages.Concat(new[] {
				ieModOptions,
				ieModDisposition
			}).ToArray();

			//don't touch this
			this.SetMenuLayout(OptionsMenuLayout.InGame);
			this.QuitButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.QuitButton.onClick, new UIEventListener.VoidDelegate(this.OnQuitClicked));
			this.SaveButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.SaveButton.onClick, new UIEventListener.VoidDelegate(this.OnSaveClicked));
			this.LoadButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.LoadButton.onClick, new UIEventListener.VoidDelegate(this.OnLoadClicked));
        
			this.m_Options = base.GetComponentsInChildren<UIOptionsTag>(true);
			// end
			//File.WriteAllText("ComboboxDump.txt", UnityObjectDumper.PrintUnityGameObject(exampleDropdown.gameObject, null, x => false));
			controlFactory.CurrentParent = Pages[7].transform;
			
			//The following are the controls that appear in the GUI of the mod.
			_oneTooltip = controlFactory.Checkbox(() => IEModOptions.OneTooltip);
			_oneTooltip.transform.localPosition = new Vector3(-210, 330, 0);

			_disableEngagement = controlFactory.Checkbox(() => IEModOptions.DisableEngagement);
			_disableEngagement.transform.localPosition = new Vector3(-210, 300, 0);

			_blueCircles = controlFactory.Checkbox(() => IEModOptions.BlueCircles);
			_blueCircles.transform.localPosition = new Vector3(-210, 270, 0);
		    
			_blueCirclesBg = controlFactory.Checkbox(() => IEModOptions.BlueCirclesBG);
			_blueCirclesBg.transform.localPosition = new Vector3(-180, 240, 0);

			_alwaysShowCircles = controlFactory.Checkbox(() => IEModOptions.AlwaysShowCircles);
			_alwaysShowCircles.transform.localPosition = new Vector3(-210, 210, 0);

			_unlockCombatInv = controlFactory.Checkbox(() => IEModOptions.UnlockCombatInv);
			_unlockCombatInv.transform.localPosition = new Vector3(-210, 180, 0);

			_fixBackerNames = controlFactory.Checkbox(() => IEModOptions.FixBackerNames);
			_fixBackerNames.transform.localPosition = new Vector3(-210, 150, 0);

			_removeMovingRecovery = controlFactory.Checkbox(() => IEModOptions.RemoveMovingRecovery);
			_removeMovingRecovery.transform.localPosition = new Vector3(-210, 120, 0);

			_fastSneak = controlFactory.Checkbox(() => IEModOptions.FastSneak);
			_fastSneak.transform.localPosition = new Vector3(-210, 90, 0);

			_improvedAi = controlFactory.Checkbox(() => IEModOptions.ImprovedAI);
			_improvedAi.transform.localPosition = new Vector3(-210, 60, 0);

			_disableFfCb = controlFactory.Checkbox(() => IEModOptions.DisableFriendlyFire);
			_disableFfCb.transform.localPosition = new Vector3(-210, 30, 0);

			_lootShuffler = controlFactory.Checkbox(() => IEModOptions.LootShuffler);
			_lootShuffler.transform.localPosition = new Vector3(210, 300, 0);

			_gameSpeed = controlFactory.Checkbox(() => IEModOptions.GameSpeedMod);
			_gameSpeed.transform.localPosition = new Vector3(210, 270, 0);

			_combatOnly = controlFactory.Checkbox(() => IEModOptions.CombatOnlyMod);
			_combatOnly.transform.localPosition = new Vector3(210, 240, 0);

			_bonusSpellsPerDay = controlFactory.Checkbox(() => IEModOptions.BonusSpellsPerDay);
			_bonusSpellsPerDay.transform.localPosition = new Vector3(210, 210, 0);

			_targetTurnedEnemies = controlFactory.Checkbox(() => IEModOptions.TargetTurnedEnemies);
			_targetTurnedEnemies.transform.localPosition = new Vector3(210, 180, 0);
			
			_npcDispositionFix = controlFactory.Checkbox(() => IEModOptions.NPCDispositionFix);
			_npcDispositionFix.transform.localPosition = new Vector3(210, 150, 0);
			
			_nerfedXpCmb = controlFactory.EnumBoundDropdown(() => IEModOptions.NerfedXPTableSetting, 515, 300);
			_nerfedXpCmb.transform.localPosition = new Vector3(-80, -70, 0);

			_perEncounterSpellsCmb = controlFactory.EnumBoundDropdown(() => IEModOptions.PerEncounterSpellsSetting, 515, 300);
			_perEncounterSpellsCmb.transform.localPosition = new Vector3(-80, -110, 0);

			_extraGrimoireSpellsCmb = controlFactory.EnumBoundDropdown(() => IEModOptions.ExtraWizardSpells, 515, 300);
			_extraGrimoireSpellsCmb.transform.localPosition = new Vector3(-80, -150, 0);

			_autosaveCmb = controlFactory.EnumBoundDropdown(() => IEModOptions.AutosaveSetting, 515, 300);
			_autosaveCmb.transform.localPosition = new Vector3(-80, -30, 0);

			// add autosave settings
			// end of adding checkbox

			// Pallegina dispositions mod page

			controlFactory.CurrentParent = ieModDisposition.transform;
			var favoredDisposition1 = controlFactory.EnumBoundDropdown(() => IEModOptions.PalleginaFavored1,
				150,
				300);
			favoredDisposition1.transform.localPosition = new Vector3(-60, 300, 0);

			var favoredDisposition2 = controlFactory.EnumBoundDropdown(() => IEModOptions.PalleginaFavored2, 150, 0);
			favoredDisposition2.transform.localPosition = new Vector3(100, 300, 0);

			var disDisposition1 = controlFactory.EnumBoundDropdown(() => IEModOptions.PalleginaDisfavored1,150, 300);
			disDisposition1.transform.localPosition = new Vector3(-60, 250, 0);

			var disDisposition2 = controlFactory.EnumBoundDropdown(() => IEModOptions.PalleginaDisfavored2, 150, 0);
			disDisposition2.transform.localPosition = new Vector3(100, 250, 0);
			
			//END OF CONTROL DEFINITIONS. The rest is built in stuff that needs to happen after creating all of our controls.
			this.PageButtonGroup.OnRadioSelectionChangedEvent += new UIRadioButtonGroup.RadioSelectionChanged(this.OnChangePage); // changed to Event
			this.m_PageButtonGrid = this.PageButtonGroup.GetComponent<UIGrid>();
			this.m_PageButtons = new UIMultiSpriteImageButton[9];
			this.m_PageButtons[0] = this.PageButtonPrefab;
			for (int i = 0; i < 9; i++)
			{
				if (i > 0)
				{
					this.m_PageButtons[i] = NGUITools.AddChild(this.PageButtonPrefab.transform.parent.gameObject, this.PageButtonPrefab.gameObject).GetComponent<UIMultiSpriteImageButton>();
				}
				if (i < this.PageTitleStringIds.Length)
				{
					GUIStringLabel.Get(this.m_PageButtons[i].Label).SetString(this.PageTitleStringIds[i]);
				}
				else if(i == this.PageTitleStringIds.Length) // added this line
				{
					GUIStringLabel.Get (this.m_PageButtons [i].Label).FormatString = "IE Mod"; // added this line
					//Debug.LogWarning("Not enough strings provided for every options tab in OptionsManager.");
				}
				else if (i == this.PageTitleStringIds.Length + 1)
				{
					GUIStringLabel.Get(this.m_PageButtons[i].Label).FormatString = "Dispositions"; // added this line
					//Debug.LogWarning("Not enough strings provided for every options tab in OptionsManager.");
				}
				this.m_PageButtons[i].name = this.PageOrder[i] + "." + this.m_PageButtons[i].name;
			}
			this.m_PageButtonGrid.Reposition();
			foreach (UIOptionsTag tag in this.m_Options)
			{
				if (tag.Checkbox != null)
				{
					tag.Checkbox.onStateChange = (UICheckbox.OnStateChange) Delegate.Combine(tag.Checkbox.onStateChange, new UICheckbox.OnStateChange(this.OnCheckChanged));
				}
			}

			this.CombatTimerSlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.CombatTimerSlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnCombatTimerChanged));
			this.AutoslowThresholdSlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.AutoslowThresholdSlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnAutoslowThresholdChanged));
			this.TooltipDelay.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.TooltipDelay.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnTooltipDelayChanged));
			this.FontSize.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.FontSize.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnFontSizeChanged));
			this.GammaSlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.GammaSlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnGammaSliderChanged));
			this.AreaLootRangeSlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.AreaLootRangeSlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnAreaLootSliderChanged));
			this.VoiceFrequency.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.VoiceFrequency.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnVoiceFrequencyChanged));
			this.ScrollSpeed.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.ScrollSpeed.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnScrollSpeedChanged));

			this.LanguageDropdown.OnDropdownOptionChangedEvent += new UIDropdownMenu.DropdownOptionChanged(this.OnLanguageChanged); // changed to Event
			this.ResolutionDropdown.OnDropdownOptionChangedEvent += new UIDropdownMenu.DropdownOptionChanged(this.OnResolutionChanged); // changed to Event
			this.QualitySlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.QualitySlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnQualityChanged));
			this.FrameRateMaxSlider.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged) Delegate.Combine(this.FrameRateMaxSlider.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnMaxFPSChanged));

			this.m_VolumeSliders = new UIOptionsSliderGroup[4];
			UIOptionsVolumeSlider[] componentsInChildren = base.GetComponentsInChildren<UIOptionsVolumeSlider>(true);
			UIOptionsVolumeSlider[] array = componentsInChildren;
			for (int k = 0; k < array.Length; k++)
			{
				UIOptionsVolumeSlider uIOptionsVolumeSlider = array[k];
				if (this.m_VolumeSliders[(int)uIOptionsVolumeSlider.Category] == null)
				{
					UIOptionsSliderGroup component = uIOptionsVolumeSlider.GetComponent<UIOptionsSliderGroup>();
					this.m_VolumeSliders[(int)uIOptionsVolumeSlider.Category] = component;
					component.Slider.OnChanged = (UIOptionsSlider.OnSettingChanged)Delegate.Combine(component.Slider.OnChanged, new UIOptionsSlider.OnSettingChanged(this.OnVolumeChanged));
				}
			}
			this.AcceptButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.AcceptButton.onClick, new UIEventListener.VoidDelegate(this.OnAcceptClick));
			this.DefControlsButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.DefControlsButton.onClick, new UIEventListener.VoidDelegate(this.OnRestoreDefaultControls));
			this.ApplyResolutionButton.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.ApplyResolutionButton.onClick, new UIEventListener.VoidDelegate(this.OnApplyResolution));
		}

		[ModifiesMember(".ctor")]
		public void ConstructorNew() {
			PageOrder = new[] {
				0,
				3,
				1,
				4,
				5,
				2,
				6,
				7,
				8
			};
			m_ValidOptionPages = (UIOptionsManager.OptionsPage)OptionsPage.CONTROLS | (UIOptionsManager.OptionsPage)OptionsPage.SOUND | (UIOptionsManager.OptionsPage)OptionsPage.GRAPHICS | (UIOptionsManager.OptionsPage)OptionsPage.GAME | (UIOptionsManager.OptionsPage)OptionsPage.AUTOPAUSE | (UIOptionsManager.OptionsPage)OptionsPage.MENU | (UIOptionsManager.OptionsPage)OptionsPage.CUSTOM | (UIOptionsManager.OptionsPage)OptionsPage.CUSTOM2;
			m_GameMode = new GameMode();
			m_Controls = new ControlMapping();
		}



	}

}