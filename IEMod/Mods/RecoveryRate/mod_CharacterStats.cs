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
                CharacterStats mWeaponSwitchingTimer = this;
                mWeaponSwitchingTimer.m_weaponSwitchingTimer = mWeaponSwitchingTimer.m_weaponSwitchingTimer - Time.deltaTime;
            }
            if (this.m_interruptTimer >= 0f)
            {
                CharacterStats mInterruptTimer = this;
                mInterruptTimer.m_interruptTimer = mInterruptTimer.m_interruptTimer - Time.deltaTime;
            }
            if (this.CurrentGrimoireCooldown > 0f)
            {
                CharacterStats currentGrimoireCooldown = this;
                currentGrimoireCooldown.CurrentGrimoireCooldown = currentGrimoireCooldown.CurrentGrimoireCooldown - Time.deltaTime;
                if (this.CurrentGrimoireCooldown < 0f)
                {
                    this.CurrentGrimoireCooldown = 0f;
                }
            }
            if (!this.HasStatusEffectThatPausesRecoveryTimer())
            {
                float movingRecoveryMult = 1f;
                if (this.IsMoving && !IEModOptions.RemoveMovingRecovery) //IEMOD - Modded line
                {
                    movingRecoveryMult = AttackData.Instance.MovingRecoveryMult;
                    if (this.m_equipment != null && this.m_equipment.PrimaryAttack != null && this.m_equipment.PrimaryAttack is AttackRanged)
                    {
                        movingRecoveryMult = movingRecoveryMult + this.RangedMovingRecoveryReductionPct;
                    }
                }
                float single = Time.deltaTime * movingRecoveryMult;
                if (this.m_recoveryTimer > 0f)
                {
                    CharacterStats mRecoveryTimer = this;
                    mRecoveryTimer.m_recoveryTimer = mRecoveryTimer.m_recoveryTimer - single;
                }
                for (GenericAbility.ActivationGroup i = GenericAbility.ActivationGroup.None; i < GenericAbility.ActivationGroup.Count; i = (GenericAbility.ActivationGroup)((int)i + (int)GenericAbility.ActivationGroup.A))
                {
                    if (this.m_modalCooldownTimer[(int)i] > 0f)
                    {
                        this.m_modalCooldownTimer[(int)i] = this.m_modalCooldownTimer[(int)i] - single;
                    }
                }
            }
            for (int j = this.m_statusEffects.Count - 1; j >= 0; j--)
            {
                if (this.m_statusEffects[j].Expired)
                {
                    StatusEffect item = this.m_statusEffects[j];
                    this.m_statusEffects.RemoveAt(j);
                    this.m_updateTracker = true;
                    if (this.OnClearStatusEffect != null)
                    {
                        this.OnClearStatusEffect(item);
                    }
                    item.Reset();
                }
            }
            for (int k = this.m_abilities.Count - 1; k >= 0; k--)
            {
                if (this.m_abilities[k] != null)
                {
                    GenericAbility genericAbility = this.m_abilities[k];
                    if (genericAbility.Passive && !genericAbility.Activated && genericAbility.Ready && genericAbility.IsLoaded)
                    {
                        genericAbility.Activate();
                        this.m_updateTracker = true;
                    }
                }
                else
                {
                    this.m_abilities.RemoveAt(k);
                }
            }
            if (this.m_updateTracker)
            {
                this.m_updateTracker = false;
                this.ClearStackTracker();
                for (int l = 0; l < this.m_statusEffects.Count; l++)
                {
                    StatusEffect statusEffect = this.m_statusEffects[l];
                    if (!statusEffect.IsSuspended)
                    {
                        bool isSuppressed = statusEffect.IsSuppressed;
                        bool flag = false;
                        for (int m = 0; m < this.m_statusEffects.Count; m++)
                        {
                            if (l != m)
                            {
                                StatusEffect item1 = this.m_statusEffects[m];
                                if (!item1.IsSuspended)
                                {
                                    if (item1.Suppresses(statusEffect, l > m))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (isSuppressed && !flag)
                        {
                            statusEffect.Unsuppress();
                        }
                        else if (!isSuppressed && flag)
                        {
                            statusEffect.Suppress();
                        }
                    }
                }
            }
            for (int n = 0; n < this.m_statusEffects.Count; n++)
            {
                StatusEffect statusEffect1 = this.m_statusEffects[n];
                if (statusEffect1.Stackable && !statusEffect1.HasBeenApplied)
                {
                    statusEffect1.ApplyEffect(base.gameObject);
                }
                if (!statusEffect1.Stackable && !statusEffect1.IsSuspended && !statusEffect1.IsSuppressed)
                {
                    StatusEffect trackedEffect = this.GetTrackedEffect(statusEffect1.NonstackingEffectType, statusEffect1.GetStackingKey());
                    int num = this.m_statusEffects.IndexOf(trackedEffect);
                    if (trackedEffect == null || trackedEffect.IsSuspended || statusEffect1.Suppresses(trackedEffect, num > n))
                    {
                        if (trackedEffect != null && trackedEffect.Applied)
                        {
                            trackedEffect.Suppress();
                        }
                        this.AddTrackedEffect(statusEffect1);
                    }
                }
            }
            if (CharacterStats.s_PlayFatigueSoundWhenNotLoading && UIInterstitialManager.Instance != null && !UIInterstitialManager.Instance.WindowActive() && !GameState.IsLoading)
            {
                IEnumerable<PartyMemberAI> onlyPrimaryPartyMembers = PartyMemberAI.OnlyPrimaryPartyMembers;
                if (onlyPrimaryPartyMembers != null)
                {
                    List<PartyMemberAI> partyMemberAIs = new List<PartyMemberAI>();
                    CharacterStats component = null;
                    IEnumerator<PartyMemberAI> enumerator = onlyPrimaryPartyMembers.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            PartyMemberAI current = enumerator.Current;
                            if (current != null)
                            {
                                component = current.GetComponent<CharacterStats>();
                                if (!(component != null) || component.GetFatigueLevel() == CharacterStats.FatigueLevel.None)
                                {
                                    continue;
                                }
                                partyMemberAIs.Add(current);
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
                    while (partyMemberAIs.Count > 0 && AfflictionData.Instance.TravelFatigueSoundTimer <= 0f)
                    {
                        PartyMemberAI partyMemberAI = partyMemberAIs[Random.Range(0, partyMemberAIs.Count)];
                        this.PlayPartyMemberFatigueSound(partyMemberAI);
                        partyMemberAIs.Remove(partyMemberAI);
                    }
                    if (partyMemberAIs != null)
                    {
                        partyMemberAIs.Clear();
                        partyMemberAIs = null;
                    }
                }
                CharacterStats.s_PlayFatigueSoundWhenNotLoading = false;
            }
            if (this.m_stackTracker != null)
            {
                foreach (KeyValuePair<int, Dictionary<int, StatusEffect>> mStackTracker in this.m_stackTracker)
                {
                    foreach (KeyValuePair<int, StatusEffect> value in mStackTracker.Value)
                    {
                        StatusEffect value1 = value.Value;
                        if (value1 != null)
                        {
                            if (!value1.HasBeenApplied)
                            {
                                value1.Unsuppress();
                                value1.ApplyEffect(base.gameObject);
                            }
                        }
                    }
                }
            }
            for (int o = 0; o < this.m_statusEffects.Count; o++)
            {
                this.m_statusEffects[o].Update();
            }
            if (this.IsPartyMember && GameCursor.CharacterUnderCursor && this.m_equipment)
            {
                PartyMemberAI component1 = base.GetComponent<PartyMemberAI>();
                if (component1 && component1.Selected)
                {
                    int num1 = 0;
                    while (num1 < this.m_abilities.Count)
                    {
                        FlankingAbility flankingAbility = this.m_abilities[num1] as FlankingAbility;
                        if (!flankingAbility || !flankingAbility.CanSneakAttackEnemy(GameCursor.CharacterUnderCursor, this.m_equipment.PrimaryAttack))
                        {
                            num1++;
                        }
                        else
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
            if (this.IsPartyMember && !GameState.InCombat && !TimeController.Instance.Paused)
            {
                int maxLevelCanLevelUpTo = this.GetMaxLevelCanLevelUpTo();
                if (maxLevelCanLevelUpTo > this.Level && maxLevelCanLevelUpTo > this.m_NotifiedLevel)
                {
                    GameUtilities.LaunchEffect(InGameHUD.Instance.LevelUpVfx, 1f, base.transform, null);
                    UIHealthstringManager.Instance.ShowNotice(GUIUtils.GetText(807), base.gameObject, 2.5f);
                    this.m_NotifiedLevel = this.GetMaxLevelCanLevelUpTo();
                }
            }
            if (CharacterStats.DebugStats)
            {
                Faction faction = base.GetComponent<Faction>();
                if (faction != null && faction.MousedOver)
                {
                    UIDebug.Instance.SetText("Character Stats Debug", this.GetCharacterStatsDebugOutput(), Color.cyan);
                    UIDebug.Instance.SetTextPosition("Character Stats Debug", 0.95f, 0.95f, UIWidget.Pivot.TopRight);
                }
            }           
		}
	}
}
