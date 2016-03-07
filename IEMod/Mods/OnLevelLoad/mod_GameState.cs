using System;
using System.IO;
using System.Xml;
using IEMod.Mods.BackerStuff;
using IEMod.Mods.DropButtonMod;
using IEMod.Mods.Options;
using IEMod.Mods.UICustomization;
using IEMod.Helpers;
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
		public  static void mod_ChangeLevel(MapData map)
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
				Debug.LogException(exception);
			}
		}

		[ModifiesMember("FinalizeLevelLoad")]
		public void mod_FinalizeLevelLoad() {
            try
            {
                if (this.CurrentMap != null && !this.CurrentMap.HasBeenVisited && BonusXpManager.Instance && this.CurrentMap.GivesExplorationXp)
                {
                    this.CurrentMap.HasBeenVisited = true;
                    int mapExplorationXp = 0;
                    if (BonusXpManager.Instance != null)
                    {
                        mapExplorationXp = BonusXpManager.Instance.MapExplorationXp;
                    }
                    Console.AddMessage(string.Concat("[", NGUITools.EncodeColor(Color.yellow), "]", Console.Format(GUIUtils.GetTextWithLinks(1633), new object[] { this.CurrentMap.DisplayName, mapExplorationXp * PartyHelper.NumPartyMembers })));
                    PartyHelper.AssignXPToParty(mapExplorationXp, false);
                }
                if (GameState.OnLevelLoaded != null)
                {
                    GameState.OnLevelLoaded(Application.loadedLevelName, EventArgs.Empty);
                }
                if (GameState.NewGame && this.Difficulty == GameDifficulty.Easy)
                {
                    GameState.Option.AutoPause.SetSlowEvent(AutoPauseOptions.PauseEvent.CombatStart, true);
                }
                ScriptEvent.BroadcastEvent(ScriptEvent.ScriptEvents.OnLevelLoaded);
                GameState.IsLoading = false;
                if (GameState.s_playerCharacter != null && !GameState.LoadedGame && !GameState.NewGame && GameState.NumSceneLoads > 0)
                {
                    if (!IEModOptions.SaveBeforeTransition) // added this line
                    {
                        if (FogOfWar.Instance)
                        {
                            FogOfWar.Instance.WaitForFogUpdate();
                        }
                        GameState.Autosave();
                    }
                }
                GameState.NewGame = false;
                if (this.CurrentMap != null && this.CouldAccessStashOnLastMap != this.CurrentMap.GetCanAccessStash() && !GameState.Option.GetOption(GameOption.BoolOption.DONT_RESTRICT_STASH))
                {
                    if (!this.CurrentMap.GetCanAccessStash())
                    {
                        UISystemMessager.Instance.PostMessage(GUIUtils.GetText(1566), Color.white);
                    }
                    else
                    {
                        UISystemMessager.Instance.PostMessage(GUIUtils.GetText(1565), Color.white);
                    }
                }
                GameState.NumSceneLoads = GameState.NumSceneLoads + 1;
                FatigueCamera.CreateCamera();
                GammaCamera.CreateCamera();
                WinCursor.Clip(true);
                if (this.CurrentMap != null)
                {
                    TutorialManager.TutorialTrigger tutorialTrigger = new TutorialManager.TutorialTrigger(TutorialManager.TriggerType.ENTERED_MAP)
                    {
                        Map = this.CurrentMap.SceneName
                    };
                    TutorialManager.STriggerTutorialsOfType(tutorialTrigger);
                }
                if (this.CurrentMap != null && this.CurrentMap.IsValidOnMap("px1"))
                {
                    GameState.Instance.HasEnteredPX1 = true;
                    if (GameGlobalVariables.HasStartedPX2())
                    {
                        this.HasEnteredPX2 = true;
                }
                }
                // in here you can place something like if (CurrentMap.SceneName == "AR_0011_Dyrford_Tavern_02") make_an_NPC; or change_NPC's_stats;
                // added this code

                // Addition of autoload custom NPC stats if enabled
                if (IEModOptions.AutoLoadCustomStats)
                {
                   ImportStats();
                }

                DropButton.InjectDropInvButton();
                if (IEModOptions.EnableCustomUi)
                {
                    if (IEModOptions.Layout == null)
                    {
                        UICustomizer.Initialize();
                        IEModOptions.Layout = UICustomizer.DefaultLayout.Clone();
                    }
                    else
                    {
                        UICustomizer.LoadLayout(IEModOptions.Layout);
                    }
                }

                BackerNamesMod.FixBackerNames(IEModOptions.FixBackerNames);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                GameState.ReturnToMainMenuFromError();
            }
            if (!this.RetroactiveSpellMasteryChecked)
            {
                for (int i = 0; i < (int)PartyMemberAI.PartyMembers.Length; i++)
                {
                    if (PartyMemberAI.PartyMembers[i] != null)
                    {
                        CharacterStats component = PartyMemberAI.PartyMembers[i].GetComponent<CharacterStats>();
                        if (component)
                        {
                            if (component.MaxMasteredAbilitiesAllowed() > component.GetNumMasteredAbilities())
                            {
                                UIWindowManager.ShowMessageBox(UIMessageBox.ButtonStyle.OK, GUIUtils.GetText(2252), GUIUtils.GetText(2303));
                                break;
                            }
                        }
                    }
                }
                this.RetroactiveSpellMasteryChecked = true;
            }
            if (GameUtilities.HasPX2() && GameState.LoadedGame)
            {
                if (GameGlobalVariables.HasFinishedPX1())
                {
                    QuestManager.Instance.StartPX2Umbrella();
                }
                else if (!this.HasNotifiedPX2Installation)
                {
                    UIWindowManager.ShowMessageBox(UIMessageBox.ButtonStyle.OK, string.Empty, GUIUtils.GetText(2438));
                    this.HasNotifiedPX2Installation = true;
                }
            }
		}


        /// <summary>
        /// Method serving as a band-aid fix for the stats not sticking after reloads in 3.01
        /// Looks in an xml file located in Managed/iemod/customStats and extracts the values for NPC stats     
        /// located inside it. Inspired heavily by the FindCharacter and AttributeScore methods in the vanilla dll.
        /// </summary>
        [NewMember]
        public static void ImportStats()
        {
            // Load document
            XmlDocument doc = new XmlDocument();
            string xmlPath = PathHelper.Combine(Application.dataPath, "Managed/iemod/customStats", "custom.xml");
            try
            {
                doc.Load(xmlPath);
            }
            catch (System.IO.FileNotFoundException e)
            {
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
                        // Break out of inner loop once that NPC has been adjusted... ugly but probably quicker than anything else I know....
                        break;
                    }
                }
            }
        }
	}
}