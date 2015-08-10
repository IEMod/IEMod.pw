using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.NoEngagement {
	[ModifiesType]
	public abstract class Mod_NoEngagement_AIController : AIController
	{
		[ModifiesMember("EngageEnemy")]
		public void EngageEnemyNew(GameObject enemy)
		{
			AIController component = enemy.GetComponent<AIController>();
			if (!component || component == this)
			{
				return;
			}
			if (!this.EngagedEnemies.Contains(enemy))
			{
				this.EngagedEnemies.Add(enemy);
			}
			GameObject owner = this.StateManager.CurrentState.Owner;
			CharacterStats component2 = owner.GetComponent<CharacterStats>();
			if (component2 != null)
			{
				component2.NotifyEngagement(enemy);
			}
			GameEventArgs gameEventArgs = new GameEventArgs();
			gameEventArgs.Type = GameEventType.MeleeEngaged;
			gameEventArgs.GameObjectData = new GameObject[1];
			gameEventArgs.GameObjectData[0] = owner;
			component.OnEvent(gameEventArgs);
			if (FogOfWar.Instance.PointVisible(owner.transform.position) && (!IEModOptions.DisableEngagement)) // added && ((Mod_GameOptions_GameMode)GameState.Mode).DisableEngagement == 0
			{
				Console.AddMessage(Console.Format(GUIUtils.GetTextWithLinks(100), new object[]
				{
					CharacterStats.NameColored(owner),
					CharacterStats.NameColored(enemy)
				}));
			}
			GameState.AutoPause(AutoPauseOptions.PauseEvent.CharacterAttacked, enemy, owner, null);
			component.AddEngagedBy(owner);
		}

		[ModifiesMember("DisengageEnemy")]
		public void DisengageEnemyNew(GameObject enemy, AttackBase attack)
		{
			CharacterStats component = enemy.GetComponent<CharacterStats>();
			if ((component != null) && !component.ImmuneToEngagement && !IEModOptions.DisableEngagement) // added && (Mod_GameOptions_GameMode)GameState.Mode).DisableEngagement == 0
			{
				attack.IsDisengagementAttack = true;
				attack.Launch(enemy, -1);
			}
			GameObject owner = this.StateManager.CurrentState.Owner;
			this.m_disengagementTrackers.Add(new DisengagementTracker(enemy, false));
			this.StopEnagagement(enemy);
			if (this.m_stats != null)
			{
				this.m_stats.NotifyEngagementBreak(enemy);
			}
			this.EnemyBreaksEngagement(enemy);
			AIController controller = enemy.GetComponent<AIController>();
			if (controller != null)
			{
				GameEventArgs args = new GameEventArgs {
					Type = GameEventType.MeleeEngageBroken,
					GameObjectData = new GameObject[] { owner }
				};
				controller.OnEvent(args);
				controller.EnemyBreaksEngagement(owner);
			}
		}
	}

	[ModifiesType]
	public class Mod_NoEngagement_HudEngagementIndicator : HudEngagementIndicator
	{
		[ModifiesMember("Update")]
		protected void UpdateNew()
		{
			if (!this.Target || !this.Source)
			{
				return;
			}
			this.LineRenderer.renderer.enabled = (GameState.Paused || GameCursor.CharacterUnderCursor == this.m_Source || GameCursor.CharacterUnderCursor == this.m_Target);
			this.LineRenderer.renderer.enabled &= InGameHUD.Instance.ShowHUD;
			this.LineRenderer.renderer.enabled &= IEModOptions.DisableEngagement; // added this line
			if (!this.LineRenderer.renderer.enabled)
			{
				return;
			}
			Vector3 vector = this.Target.transform.position - this.Source.transform.position;
			vector.y = 0f;
			base.transform.rotation = Quaternion.FromToRotation(Vector3.forward, vector.normalized);
			float vertScale = HudEngagementManager.Instance.ArrowScaleY;
			if (vector.sqrMagnitude > HudEngagementManager.Instance.ArrowMaxRange)
			{
				vertScale = HudEngagementManager.Instance.ArrowScaleY + (1f - HudEngagementManager.Instance.ArrowScaleY) * (vector.magnitude / HudEngagementManager.Instance.ArrowMaxRange);
			}
			float num = vector.magnitude - (this.m_SourceFaction.Mover.Radius + this.m_TargetFaction.Mover.Radius) / 3f;
			this.m_xrot += HudEngagementManager.Instance.ArrowRotSpeed * TimeController.sUnscaledDelta / num;
			this.GenerateVertexData(this.m_Mesh, num, vertScale, 0.0174532924f * this.m_xrot);
			base.transform.position = (this.Source.transform.position + this.Target.transform.position) / 2f;
			base.transform.position += new Vector3(0f, HudEngagementManager.Instance.ArrowElevation, 0f);
			if (this.TwoWay)
			{
				Quaternion rotation = base.transform.rotation * Quaternion.AngleAxis(90f, Vector3.up);
				base.transform.position += rotation * (Vector3.forward * HudEngagementManager.Instance.TwoWayOffset);
			}
			this.UpdateMaterial();
		}
	}
	[ModifiesType]
	public class Mod_NoEngagement_UIChantEditor : UIChantEditor
	{
		[ModifiesMember("Awake")]
		private void AwakeNew()
		{
			UIChantEditor.Instance = this;
			this.ToggleKey = MappedControl.NONE; // prevent it from opening when Deprecated_CHANT_EDITOR is pressed.
		}
	}
	[ModifiesType]
	public class Mod_NoEngagement_Player : Player
	{
		[NewMember]
		public static bool WalkMode = false;

		[NewMember]
		public static void ToggleWalkMode()
		{
			if (UIWindowManager.KeyInputAvailable)
			{
				if (GameInput.GetControlUp(MappedControl.Deprecated_CHANT_EDITOR))
				{
					WalkMode = !WalkMode;
					string msg;
					Color color;
					if (WalkMode)
					{
						msg = "It is a nice day for a walk";
						color = Color.green;
					}
					else
					{
						msg = "Time to move.  We can sight see later!";
						color = Color.red;
					}
					Console.AddMessage(msg, color);
				}
			}
		}
		[ModifiesMember("UpdateCursor")]
		protected void UpdateCursorNew()
		{
			ToggleWalkMode();
			if (this.m_isCasting && this.m_castAbility != null)
			{
				if (!this.m_castAbility.ReadyForUI)
				{
					this.CancelModes(true);
				}
				if (this.m_castAbility != null && !this.m_castAbility.IsValidTarget(GameCursor.CharacterUnderCursor))
				{
					GameCursor.DesiredCursor = GameCursor.CursorType.NoWalk;
					return;
				}
			}
			if (GameCursor.UiObjectUnderCursor != null)
			{
				GameCursor.DesiredCursor = GameCursor.CursorType.Normal;
				if (this.m_isCasting && GameCursor.OverrideCharacterUnderCursor != null)
				{
					GameCursor.DesiredCursor = this.GetCastingCursor();
				}
				return;
			}
			bool flag = PartyMemberAI.IsPrimaryPartyMemberSelected();
			if (this.m_isSelecting)
			{
				GameCursor.DesiredCursor = GameCursor.CursorType.Normal;
			}
			else
			{
				if (this.IsInForceAttackMode)
				{
					if (GameCursor.CharacterUnderCursor != null)
					{
						Health component = GameCursor.CharacterUnderCursor.GetComponent<Health>();
						PartyMemberAI component2 = GameCursor.CharacterUnderCursor.GetComponent<PartyMemberAI>();
						if (component == null || !component.CanBeTargeted || (component2 != null && component2.Selected))
						{
							GameCursor.DesiredCursor = GameCursor.CursorType.NoWalk;
						}
						else
						{
							GameCursor.DesiredCursor = this.GetAttackCursor();
						}
					}
					else
					{
						GameCursor.DesiredCursor = this.GetAttackCursor();
					}
				}
				else
				{
					if (this.m_isCasting)
					{
						GameCursor.DesiredCursor = this.GetCastingCursor();
					}
					else
					{
						if (this.RotatingFormation)
						{
							GameCursor.DesiredCursor = GameCursor.CursorType.RotateFormation;
						}
						else
						{
							if (GameCursor.ObjectUnderCursor != null)
							{
								Faction component3 = GameCursor.ObjectUnderCursor.GetComponent<Faction>();
								Health component4 = GameCursor.ObjectUnderCursor.GetComponent<Health>();
								Trap component5 = GameCursor.ObjectUnderCursor.GetComponent<Trap>();
								Container component6 = GameCursor.ObjectUnderCursor.GetComponent<Container>();
								if (component3 != null && (component5 == null || component5.Visible) && (!component4 || component4.CurrentHealth > 0f) && component6 == null)
								{
									if (component5 != null)
									{
										if (component3.RelationshipToPlayer == Faction.Relationship.Hostile && !GameCursor.OverrideCharacterUnderCursor && !flag)
										{
											GameCursor.DesiredCursor = GameCursor.CursorType.NoWalk;
										}
										else
										{
											Collider2D component7 = GameCursor.ObjectUnderCursor.GetComponent<Collider2D>();
											if (component7 != null && component5.CanDisarm)
											{
												GameCursor.DesiredCursor = GameCursor.CursorType.Disarm;
											}
											else
											{
												GameCursor.DesiredCursor = GameCursor.CursorType.Normal;
											}
										}
									}
									else
									{
										if (component3.RelationshipToPlayer == Faction.Relationship.Hostile && !GameCursor.OverrideCharacterUnderCursor)
										{
											GameCursor.DesiredCursor = this.GetAttackCursor();
										}
										else
										{
											NPCDialogue component8 = GameCursor.ObjectUnderCursor.GetComponent<NPCDialogue>();
											PartyMemberAI component9 = GameCursor.ObjectUnderCursor.GetComponent<PartyMemberAI>();
											AIController component10 = GameCursor.ObjectUnderCursor.GetComponent<AIController>();
											if (flag && (!component10 || (!component10.IsBusy && !component10.IsFactionSwapped())) && component8 && (component9 == null || !component9.IsInSlot))
											{
												GameCursor.DesiredCursor = GameCursor.CursorType.Talk;
											}
											else
											{
												GameCursor.DesiredCursor = GameCursor.CursorType.Normal;
											}
										}
									}
								}
								else
								{
									TrapTriggerGeneric component11 = GameCursor.ObjectUnderCursor.GetComponent<TrapTriggerGeneric>();
									if (component11)
									{
										GameCursor.DesiredCursor = GameCursor.CursorType.AreaTransition;
										return;
									}
									Door component12 = GameCursor.ObjectUnderCursor.GetComponent<Door>();
									if (component12)
									{
										if (component12.CurrentState == OCL.State.Closed)
										{
											GameCursor.DesiredCursor = GameCursor.CursorType.OpenDoor;
										}
										else
										{
											if (component12.CurrentState == OCL.State.Open)
											{
												if (component12.IsAnyMoverIntersectingNavMeshObstacle())
												{
													GameCursor.DesiredCursor = GameCursor.CursorType.NoWalk;
												}
												else
												{
													GameCursor.DesiredCursor = GameCursor.CursorType.CloseDoor;
												}
											}
											else
											{
												if (component12.CurrentState == OCL.State.Locked)
												{
													GameCursor.DesiredCursor = GameCursor.CursorType.LockedDoor;
												}
											}
										}
										return;
									}
									Container component13 = GameCursor.ObjectUnderCursor.GetComponent<Container>();
									if (component13 != null)
									{
										if (!flag)
										{
											GameCursor.DesiredCursor = GameCursor.CursorType.NoWalk;
											return;
										}
										if (component5 != null && !component5.Disarmed && component5.Visible && component5.CanDisarm && GameState.InStealthMode)
										{
											GameCursor.DesiredCursor = GameCursor.CursorType.Disarm;
											return;
										}
										if (!GameState.InCombat)
										{
											if (component13.CurrentState == OCL.State.Closed)
											{
												if (component13.StealingFactionID != FactionName.None)
												{
													GameCursor.DesiredCursor = GameCursor.CursorType.Stealing;
												}
												else
												{
													GameCursor.DesiredCursor = GameCursor.CursorType.Loot;
												}
											}
											else
											{
												if (component13.CurrentState == OCL.State.Locked)
												{
													if (component13.StealingFactionID != FactionName.None)
													{
														GameCursor.DesiredCursor = GameCursor.CursorType.StealingLocked;
													}
													else
													{
														GameCursor.DesiredCursor = GameCursor.CursorType.LockedDoor;
													}
												}
											}
										}
										return;
									}
									else
									{
										AutoLootContainer component14 = GameCursor.ObjectUnderCursor.GetComponent<AutoLootContainer>();
										if (component14 && flag && !GameState.InCombat)
										{
											GameCursor.DesiredCursor = GameCursor.CursorType.Interact;
											return;
										}
										RestInteraction component15 = GameCursor.ObjectUnderCursor.GetComponent<RestInteraction>();
										if (component15 && flag && !GameState.InCombat)
										{
											GameCursor.DesiredCursor = GameCursor.CursorType.Interact;
											return;
										}
										Animate component16 = GameCursor.ObjectUnderCursor.GetComponent<Animate>();
										if (component16 && flag)
										{
											GameCursor.DesiredCursor = GameCursor.CursorType.Interact;
											return;
										}
										SceneTransition component17 = GameCursor.ObjectUnderCursor.GetComponent<SceneTransition>();
										if (component17)
										{
											if (GameCursor.CursorOverride != GameCursor.CursorType.None)
											{
												GameCursor.DesiredCursor = GameCursor.CursorOverride;
											}
											else
											{
												GameCursor.DesiredCursor = GameCursor.CursorType.Normal;
											}
											return;
										}
										ScriptedInteraction component18 = GameCursor.ObjectUnderCursor.GetComponent<ScriptedInteraction>();
										if (component18 && (!component18.IsUsable || PartyMemberAI.IsPartyMemberUnconscious()))
										{
											GameCursor.DesiredCursor = GameCursor.CursorType.NoWalk;
											return;
										}
										Collider2D component19 = GameCursor.ObjectUnderCursor.GetComponent<Collider2D>();
										if (component19 && (component5 == null || component5.Visible))
										{
											if (!flag)
											{
												GameCursor.DesiredCursor = GameCursor.CursorType.NoWalk;
												return;
											}
											if (GameCursor.CursorOverride != GameCursor.CursorType.None)
											{
												GameCursor.DesiredCursor = GameCursor.CursorOverride;
											}
											else
											{
												GameCursor.DesiredCursor = GameCursor.CursorType.Normal;
											}
											return;
										}
									}
								}
							}
							else
							{
								if (this.IsMouseOnWalkMesh())
								{
									if (GameState.InCombat && this.IsSelectedPartyMemberEngaged())
									{
										GameCursor.DesiredCursor = GameCursor.CursorType.Disengage;
									}
									else
									{
										GameCursor.DesiredCursor = GameCursor.CursorType.Walk;
									}
								}
								else
								{
									GameCursor.DesiredCursor = GameCursor.CursorType.NoWalk;
								}
							}
						}
					}
				}
			}
			this.WantsAttackAdvantageCursor = false;
		}
	}
}