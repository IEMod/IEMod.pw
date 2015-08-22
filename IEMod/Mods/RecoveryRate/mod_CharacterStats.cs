using System.Collections.Generic;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEMod.Mods.RecoveryRate {
	[ModifiesType]
	public class mod_CharacterStats : CharacterStats
	{
		[ModifiesMember("Update")]
		private void UpdateNew()
		{
			if (this.m_bestiaryReference)
			{
				return;
			}
			this.FatigueUpdate(Time.deltaTime * (float)WorldTime.Instance.GameSecondsPerRealSecond, this.IsMoving);
			this.NoiseUpdate(Time.deltaTime);
			this.DetectUpdate(Time.deltaTime);
			this.TrapCooldownTimerUpdate(Time.deltaTime);
			if (this.m_weaponSwitchingTimer >= 0f)
			{
				this.m_weaponSwitchingTimer -= Time.deltaTime;
			}
			if (this.m_interruptTimer >= 0f)
			{
				this.m_interruptTimer -= Time.deltaTime;
			}
			if (this.CurrentGrimoireCooldown > 0f)
			{
				this.CurrentGrimoireCooldown -= Time.deltaTime;
				if (this.CurrentGrimoireCooldown < 0f)
				{
					this.CurrentGrimoireCooldown = 0f;
				}
			}
			if (!this.HasStatusEffectThatPausesRecoveryTimer())
			{
				float num = 1f;
				if (this.IsMoving && !IEModOptions.RemoveMovingRecovery) //MOD
				{
					num = AttackData.Instance.MovingRecoveryMult;
					if (this.m_equipment != null && this.m_equipment.PrimaryAttack != null && this.m_equipment.PrimaryAttack is AttackRanged)
					{
						num += this.RangedMovingRecoveryReductionPct;
					}
				}
				float num2 = Time.deltaTime * num;
				if (this.m_recoveryTimer > 0f)
				{
					this.m_recoveryTimer -= num2;
				}
				for (GenericAbility.ActivationGroup activationGroup = GenericAbility.ActivationGroup.None; activationGroup < GenericAbility.ActivationGroup.Count; activationGroup++)
				{
					if (this.m_modalCooldownTimer[(int)activationGroup] > 0f)
					{
						this.m_modalCooldownTimer[(int)activationGroup] -= num2;
					}
				}
			}
			for (int i = this.m_statusEffects.Count - 1; i >= 0; i--)
			{
				if (this.m_statusEffects[i].Expired)
				{
					StatusEffect statusEffect = this.m_statusEffects[i];
					this.m_statusEffects.RemoveAt(i);
					this.m_updateTracker = true;
					if (this.OnClearStatusEffect != null)
					{
						this.OnClearStatusEffect(statusEffect);
					}
					statusEffect.Reset();
				}
			}
			for (int j = this.m_abilities.Count - 1; j >= 0; j--)
			{
				if (this.m_abilities[j] == null)
				{
					this.m_abilities.RemoveAt(j);
				}
				else
				{
					GenericAbility genericAbility = this.m_abilities[j];
					if (genericAbility.Passive && !genericAbility.Activated && genericAbility.Ready && genericAbility.IsLoaded)
					{
						genericAbility.Activate();
						this.m_updateTracker = true;
					}
				}
			}
			if (this.m_updateTracker)
			{
				this.m_updateTracker = false;
				this.ClearStackTracker();
				for (int k = 0; k < this.m_statusEffects.Count; k++)
				{
					StatusEffect statusEffect2 = this.m_statusEffects[k];
					if (!statusEffect2.IsSuspended)
					{
						bool isSuppressed = statusEffect2.IsSuppressed;
						bool flag = false;
						for (int l = 0; l < this.m_statusEffects.Count; l++)
						{
							if (k != l)
							{
								StatusEffect statusEffect3 = this.m_statusEffects[l];
								if (!statusEffect3.IsSuspended)
								{
									if (statusEffect3.Suppresses(statusEffect2, k > l))
									{
										flag = true;
										break;
									}
								}
							}
						}
						if (isSuppressed && !flag)
						{
							statusEffect2.Unsuppress();
						}
						else
						{
							if (!isSuppressed && flag)
							{
								statusEffect2.Suppress();
							}
						}
					}
				}
			}
			for (int m = 0; m < this.m_statusEffects.Count; m++)
			{
				StatusEffect statusEffect4 = this.m_statusEffects[m];
				if (statusEffect4.Stackable && !statusEffect4.HasBeenApplied)
				{
					statusEffect4.ApplyEffect(base.gameObject);
				}
				if (!statusEffect4.Stackable && !statusEffect4.IsSuspended && !statusEffect4.IsSuppressed)
				{
					StatusEffect trackedEffect = this.GetTrackedEffect(statusEffect4.NonstackingEffectType, statusEffect4.GetStackingKey());
					int num3 = this.m_statusEffects.IndexOf(trackedEffect);
					if (trackedEffect == null || trackedEffect.IsSuspended || statusEffect4.Suppresses(trackedEffect, num3 > m))
					{
						if (trackedEffect != null && trackedEffect.Applied)
						{
							trackedEffect.Suppress();
						}
						this.AddTrackedEffect(statusEffect4);
					}
				}
			}
			if (CharacterStats.s_PlayFatigueSoundWhenNotLoading && UIInterstitialManager.Instance != null && !UIInterstitialManager.Instance.WindowActive() && !GameState.IsLoading)
			{
				IEnumerable<PartyMemberAI> onlyPrimaryPartyMembers = PartyMemberAI.OnlyPrimaryPartyMembers;
				if (onlyPrimaryPartyMembers != null)
				{
					List<PartyMemberAI> list = new List<PartyMemberAI>();
					foreach (PartyMemberAI current in onlyPrimaryPartyMembers)
					{
						if (!(current == null))
						{
							CharacterStats component = current.GetComponent<CharacterStats>();
							if (component != null && component.GetFatigueLevel() != CharacterStats.FatigueLevel.None)
							{
								list.Add(current);
							}
						}
					}
					while (list.Count > 0 && AfflictionData.Instance.TravelFatigueSoundTimer <= 0f)
					{
						PartyMemberAI partyMemberAI = list[Random.Range(0, list.Count)];
						this.PlayPartyMemberFatigueSound(partyMemberAI);
						list.Remove(partyMemberAI);
					}
					if (list != null)
					{
						list.Clear();
						list = null;
					}
				}
				CharacterStats.s_PlayFatigueSoundWhenNotLoading = false;
			}
			if (this.m_stackTracker != null)
			{
				foreach (KeyValuePair<int, Dictionary<int, StatusEffect>> current2 in this.m_stackTracker)
				{
					foreach (KeyValuePair<int, StatusEffect> current3 in current2.Value)
					{
						StatusEffect value = current3.Value;
						if (value != null)
						{
							if (!value.HasBeenApplied)
							{
								value.Unsuppress();
								value.ApplyEffect(base.gameObject);
							}
						}
					}
				}
			}
			for (int n = 0; n < this.m_statusEffects.Count; n++)
			{
				this.m_statusEffects[n].Update();
			}
			if (this.m_isPartyMember && GameCursor.CharacterUnderCursor && this.m_equipment)
			{
				PartyMemberAI component2 = base.GetComponent<PartyMemberAI>();
				if (component2 && component2.Selected)
				{
					for (int num4 = 0; num4 < this.m_abilities.Count; num4++)
					{
						FlankingAbility flankingAbility = this.m_abilities[num4] as FlankingAbility;
						if (flankingAbility && flankingAbility.CanSneakAttackEnemy(GameCursor.CharacterUnderCursor, this.m_equipment.PrimaryAttack))
						{
							if (GameCursor.DesiredCursor == GameCursor.CursorType.Attack)
							{
								GameCursor.DesiredCursor = GameCursor.CursorType.AttackAdvantage;
							}
							GameState.s_playerCharacter.WantsAttackAdvantageCursor = true;
							break;
						}
					}
				}
			}
			if (CharacterStats.DebugStats)
			{
				Faction component3 = base.GetComponent<Faction>();
				if (component3 != null && component3.MousedOver)
				{
					UIDebug.Instance.SetText("Character Stats Debug", this.GetCharacterStatsDebugOutput(), Color.cyan);
					UIDebug.Instance.SetTextPosition("Character Stats Debug", 0.95f, 0.95f, UIWidget.Pivot.TopRight);
				}
			}
		}
	}
}
