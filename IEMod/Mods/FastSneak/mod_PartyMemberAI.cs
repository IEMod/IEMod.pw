using System;
using AI.Achievement;
using AI.Player;
using IEMod.Mods.NoEngagement;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;
using Attack = AI.Achievement.Attack;

namespace IEMod.Mods.FastSneak {
	[ModifiesType]
	public class mod_PartyMemberAI : PartyMemberAI
	{
		[NewMember]
		[MemberAlias("Update", typeof(AIController), AliasCallMode.NonVirtual)]
		private void BaseUpdate()
		{
			//A call to this method will be translated to a "base.Update()" call in the target assembly.
		}

		[ModifiesMember("Update")]
		// PartyMemberAI
		public void UpdateNew()
		{
			if (this.m_mover != null && this.m_mover.AIController == null)
			{
				this.m_mover.AIController = this;
			}
			if (GameState.s_playerCharacter != null && base.gameObject == GameState.s_playerCharacter.gameObject && PartyMemberAI.DebugParty)
			{
				UIDebug.Instance.SetText("Party Debug", PartyMemberAI.GetPartyDebugOutput(), Color.cyan);
				UIDebug.Instance.SetTextPosition("Party Debug", 0.95f, 0.95f, UIWidget.Pivot.TopRight);
			}
			if (this.m_destinationCircleState != null)
			{
				if (base.StateManager.IsStateInStack(this.m_destinationCircleState))
				{
					this.ShowDestination(this.m_destinationCirclePosition);
				}
				else
				{
					this.m_destinationCircleState = null;
					this.HideDestination();
				}
			}
			if (GameState.s_playerCharacter != null && GameState.s_playerCharacter.RotatingFormation && this.Selected)
			{
				this.ShowDestinationTarget(this.m_desiredFormationPosition);
			}
			else
			{
				this.HideDestinationTarget();
			}
			if (this.m_revealer != null)
			{
				this.m_revealer.WorldPos = base.gameObject.transform.position;
				this.m_revealer.RequiresRefresh = false;
			}
			else
			{
				this.CreateFogRevealer();
			}
			if (GameState.Paused)
			{
				base.CheckForNullEngagements();
				if (this.m_ai != null)
				{
					this.m_ai.Update();
				}
				base.DrawDebugText();
				return;
			}
			if (this.m_ai == null)
			{
				return;
			}
			if (GameState.Option.AutoPause.IsEventSet(AutoPauseOptions.PauseEvent.EnemySpotted))
			{
				this.UpdateEnemySpotted();
			}
			if (this.QueuedAbility != null && this.QueuedAbility.Ready)
			{
				AIState currentState = this.m_ai.CurrentState;
				Consumable component = this.QueuedAbility.GetComponent<Consumable>();
				if (component != null && component.Type == Consumable.ConsumableType.Ingestible)
				{
					ConsumePotion consumePotion = this.m_ai.QueuedState as ConsumePotion;
					if (!(currentState is ConsumePotion) && (consumePotion == null || currentState.Priority < 1))
					{
						ConsumePotion consumePotion2 = AIStateManager.StatePool.Allocate<ConsumePotion>();
						base.StateManager.PushState(consumePotion2);
						consumePotion2.Ability = this.QueuedAbility;
						AttackBase primaryAttack = this.GetPrimaryAttack();
						if (!(primaryAttack is AttackMelee) || !(primaryAttack as AttackMelee).Unarmed)
						{
							consumePotion2.HiddenObjects = primaryAttack.GetComponentsInChildren<Renderer>();
						}
					}
				}
				else
				{
					Attack attack = currentState as Attack;
					if (this.QueuedAbility.Passive || attack == null)
					{
						this.QueuedAbility.Activate(currentState.Owner);
					}
					else
					{
						Ability ability = AIStateManager.StatePool.Allocate<Ability>();
						ability.QueuedAbility = this.QueuedAbility;
						if (attack != null)
						{
							if (attack.CanCancel)
							{
								attack.OnCancel();
								base.StateManager.PopCurrentState();
								base.StateManager.PushState(ability);
							}
							else
							{
								base.StateManager.QueueStateAtTop(ability);
							}
						}
						else
						{
							base.StateManager.PushState(ability);
						}
					}
				}
				this.QueuedAbility = null;
			}
			BaseUpdate(); // modified
			if (GameState.IsLoading || GameState.s_playerCharacter == null)
			{
				return;
			}
			if (this.m_alphaControl != null && this.m_alphaControl.Alpha < 1.401298E-45f)
			{
				this.m_alphaControl.Alpha = 1f;
			}
			if (this.m_mover != null && !GameState.InCombat)
			{
				var walk = false; // modified
				if (GameState.InStealthMode)
				{
					// If FastSneak is not enabled we walk
					if (!IEModOptions.FastSneak)
					{
						walk = true;
					}
					else
					{
						// if the fastSneak mod is active, then check if any enemies are spotted
						// if so...we walk
						for (int i = 0; i < PartyMemberAI.PartyMembers.Length; ++i)
						{
							var p = PartyMemberAI.PartyMembers[i];
							if (p != null && p.m_enemySpotted)
							{
								walk = true;
								break;
							}
						}
					}
				}
            
				// walk mode overrides fast sneak mode
				if (Mod_NoEngagement_Player.WalkMode)
				{
					walk = true;
				}

				float num = (!walk) ? this.m_mover.GetRunSpeed() : this.m_mover.GetWalkSpeed(); // modified
				GameObject[] selectedPartyMembers = PartyMemberAI.SelectedPartyMembers;
				for (int i = 0; i < selectedPartyMembers.Length; i++)
				{
					GameObject gameObject = selectedPartyMembers[i];
					if (!(gameObject == null) && !(gameObject == base.gameObject))
					{
						Mover component2 = gameObject.GetComponent<Mover>();
						float num2 = (!walk) ? component2.GetRunSpeed() : component2.GetWalkSpeed(); // modified
						if (num2 < num)
						{
							num = component2.DesiredSpeed;
						}
					}
				}
				if (num < this.m_mover.GetWalkSpeed() * 0.75f)
				{
					num = this.m_mover.GetWalkSpeed();
				}
				this.m_mover.UseCustomSpeed(num);
			}
			if (this.m_suspicionDecayTimer > 0f)
			{
				this.m_suspicionDecayTimer -= Time.deltaTime;
			}
			if (this.m_suspicionDecayTimer <= 0f)
			{
				for (int j = this.m_detectingMe.Count - 1; j >= 0; j--)
				{
					this.m_detectingMe[j].m_time -= (float)AttackData.Instance.StealthDecayRate * Time.deltaTime;
					if (this.m_detectingMe[j].m_time <= 0f)
					{
						this.m_detectingMe.RemoveAt(j);
					}
				}
			}
		}
	}
}