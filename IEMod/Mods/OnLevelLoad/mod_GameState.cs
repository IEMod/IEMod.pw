using System;
using System.IO;
using IEMod.Mods.BackerStuff;
using IEMod.Mods.DropButtonMod;
using IEMod.Mods.Options;
using IEMod.Mods.UICustomization;
//using IEMod.Mods.UICustomization;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.OnLevelLoad {
	[ModifiesType]
	public  class mod_GameState : global::GameState
	{
		[NewMember]
		public  static DateTime LastAutoSaveTime;

		[NewMember]
		public  static void AutosaveIfAllowed()
		{
			var now = DateTime.Now;
			var minutes = IEModOptions.SaveInterval;
			if ((minutes >= 0) && ((now - LastAutoSaveTime).TotalMinutes >= minutes))
			{
				LastAutoSaveTime = now;
				Autosave();
			}
		}

		[ModifiesMember("ChangeLevel")]
		public  static void ChangeLevelNew(MapData map)
		{
			if (IEModOptions.SaveBeforeTransition) // added this block
			{
				AutosaveIfAllowed();
			}
			try
			{
				foreach (PartyMemberAI rai in PartyMemberAI.PartyMembers)
				{
					if (rai != null)
					{
						if (rai.StateManager != null)
						{
							AIState currentState = rai.StateManager.CurrentState;
							if (currentState != null)
							{
								currentState.StopMover();
							}
							rai.StateManager.AbortStateStack();
						}
						Stealth component = rai.GetComponent<Stealth>();
						if (component)
						{
							component.ClearAllSuspicion();
						}
					}
				}
				StartPoint.s_ChosenStartPoint = null;
				BeginLevelUnload(map.SceneName);
				ConditionalToggleManager.Instance.ResetBetweenSceneLoads();
				PersistenceManager.SaveGame();
				FogOfWar.Save();
				string levelFilePath = PersistenceManager.GetLevelFilePath(map.SceneName);
				GameState.IsRestoredLevel = File.Exists(levelFilePath);
				if (GameUtilities.Instance != null)
				{
					bool loadFromSaveFile = false;
					GameUtilities.Instance.StartCoroutine(GameResources.LoadScene(map.SceneName, loadFromSaveFile));
				}
				GameState.Instance.CurrentNextMap = map;
			}
			catch (Exception exception)
			{
				Debug.LogError(exception.ToString());
			}
		}

		[ModifiesMember("FinalizeLevelLoad")]
		public void FinalizeLevelLoadNew() {
			if (this.CurrentMap != null && !this.CurrentMap.HasBeenVisited && BonusXpManager.Instance
				&& this.CurrentMap.GivesExplorationXp) {
				this.CurrentMap.HasBeenVisited = true;
				int xp = 0;
				if (BonusXpManager.Instance != null) {
					xp = BonusXpManager.Instance.MapExplorationXp;
				}
				object[] parameters = new object[] {
					this.CurrentMap.DisplayName,
					xp * PartyHelper.NumPartyMembers
				};
				global::Console.AddMessage("[" + NGUITools.EncodeColor(Color.yellow) + "]"
					+ global::Console.Format(GUIUtils.GetTextWithLinks(0x661), parameters));
				PartyHelper.AssignXPToParty(xp, false);
			}
			if (OnLevelLoaded != null) {
				OnLevelLoaded(Application.loadedLevelName, EventArgs.Empty);
			}
			if (NewGame && (this.Difficulty == GameDifficulty.Easy)) {
				Option.AutoPause.SetSlowEvent(AutoPauseOptions.PauseEvent.CombatStart, true);
			}
			ScriptEvent.BroadcastEvent(ScriptEvent.ScriptEvents.OnLevelLoaded);
			GameState.IsLoading = false;
			if (GameState.s_playerCharacter != null && !GameState.LoadedGame && !GameState.NewGame && GameState.NumSceneLoads > 0) {
				if (!IEModOptions.SaveBeforeTransition) // added this line
				{
					if (FogOfWar.Instance) {
						FogOfWar.Instance.WaitForFogUpdate();
					}
					//AutosaveIfAllowed();
					GameState.Autosave();
				}
			}
			NewGame = false;
			if (((this.CurrentMap != null) && (this.CouldCampOnLastMap != this.CurrentMap.CanCamp))
				&& !Option.GetOption(GameOption.BoolOption.DONT_RESTRICT_STASH)) {
				if (this.CurrentMap.CanCamp) {
					UISystemMessager.Instance.PostMessage(GUIUtils.GetText(0x61e), Color.white);
				} else {
					UISystemMessager.Instance.PostMessage(GUIUtils.GetText(0x61d), Color.white);
				}
			}
			NumSceneLoads++;
			FatigueCamera.CreateCamera();
			GammaCamera.CreateCamera();
			WinCursor.Clip(true);
			if (this.CurrentMap != null) {
				TutorialManager.TutorialTrigger trigger =
					new TutorialManager.TutorialTrigger(TutorialManager.TriggerType.ENTERED_MAP);
				trigger.Map = this.CurrentMap.SceneName;
				TutorialManager.STriggerTutorialsOfType(trigger);
			}
			if (this.CurrentMap != null && this.CurrentMap.IsValidOnMap("px1")) {
				GameState.Instance.HasEnteredPX1 = true;
			}
			// in here you can place something like if (CurrentMap.SceneName == "AR_0011_Dyrford_Tavern_02") make_an_NPC; or change_NPC's_stats;
			// added this code
			DropButton.InjectDropInvButton();
			if (IEModOptions.EnableCustomUI) {
				if (IEModOptions.Layout == null) {
					UICustomizer.Initialize();
					IEModOptions.Layout = UICustomizer.DefaultLayout.Clone();
				} else {
					UICustomizer.LoadLayout(IEModOptions.Layout);	
				}
			}
		


		BackerNamesMod.FixBackerNames(IEModOptions.FixBackerNames);
			// end of added code
		}
	}
}