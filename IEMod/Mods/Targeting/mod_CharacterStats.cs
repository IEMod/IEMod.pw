using System;
using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEMod.Mods.Targeting {

	[ModifiesType]
	public class mod_CharacterStats : CharacterStats {
		/// <summary>
		/// Code is inserted into the middle of method.
		/// </summary>
		/// <param name="enemy"></param>
		/// <param name="damage"></param>
		/// <param name="testing"></param>
		[ModifiesMember(nameof(AdjustDamageDealt))]
		public void mod_AdjustDamageDealt(GameObject enemy, DamageInfo damage, bool testing) {
            float statDamageHealMultiplier;
            if ((damage.Attack != null) && damage.Attack.IgnoreCharacterStats)
            {
                statDamageHealMultiplier = 1f;
            }
            else
            {
                statDamageHealMultiplier = this.StatDamageHealMultiplier;
            }
            damage.DamageMult(statDamageHealMultiplier);
            if (!testing && this.OnPreDamageDealt != null)
            {
                this.OnPreDamageDealt(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
            }
            if (!testing && this.OnAddDamage != null)
            {
                this.OnAddDamage(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
            }
            int attackerToHitRollOverride = OEIRandom.DieRoll(100);
            CharacterStats component = enemy.GetComponent<CharacterStats>();
            if (component == null)
            {
                return;
            }
            attackerToHitRollOverride = component.GetAttackerToHitRollOverride(attackerToHitRollOverride);
            int num = this.CalculateAccuracy(damage.Attack, enemy);
            bool flag = component.CalculateIsImmune(damage.DefendedBy, damage.Attack, base.gameObject);
            int num1 = component.CalculateDefense(damage.DefendedBy, damage.Attack, base.gameObject);
            if (damage.DefendedBy != CharacterStats.DefenseType.None)
            {
                this.ComputeHitAdjustment(attackerToHitRollOverride + num - num1, component, damage);

                //!+ ADDED CODE
                if (IEModOptions.DisableFriendlyFire)
                {
                    var faction = enemy.Component<Faction>();
                    if (mod_AttackBase.FriendlyRightNowAndAlsoWhenConfused(enemy, this.gameObject))
                    {
                        damage.IsCriticalHit = false;
                        damage.Interrupts = false;
                        damage.IsGraze = false;
                        damage.IsKillingBlow = false;
                        damage.IsMiss = true;
                    }
                }
                //!+ END ADD


                if (!testing && this.OnAttackRollCalculated != null)
                {
                    this.OnAttackRollCalculated(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
                }
                if (damage.IsCriticalHit)
                {
                    float criticalHitMultiplier = this.CriticalHitMultiplier;
                    Health health = enemy.GetComponent<Health>();
                    if (health != null && health.StaminaPercentage < 0.1f)
                    {
                        criticalHitMultiplier = criticalHitMultiplier + this.CritHitDamageMultiplierBonusEnemyBelow10Percent;
                    }
                    damage.DamageMult(criticalHitMultiplier);
                }
                else if (damage.IsGraze)
                {
                    damage.DamageMult(CharacterStats.GrazeMultiplier);
                }
                else if (damage.IsMiss)
                {
                    damage.DamageMult(0f);
                }
            }
            WeaponSpecializationData.AddWeaponSpecialization(this, damage);
            damage.AccuracyRating = num;
            damage.DefenseRating = num1;
            damage.Immune = flag;
            damage.RawRoll = attackerToHitRollOverride;
            if (!testing && damage.Immune)
            {
                UIHealthstringManager.Instance.ShowNotice(GUIUtils.GetText(2188), enemy, 1f);
                if (this.IsPartyMember)
                {
                    SoundSet.TryPlayVoiceEffectWithLocalCooldown(base.gameObject, SoundSet.SoundAction.TargetImmune, SoundSet.s_LongVODelay, false);
                }
            }
            if (!testing && this.OnAdjustCritGrazeMiss != null)
            {
                this.OnAdjustCritGrazeMiss(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
            }
            if (!damage.IsMiss)
            {
                for (int i = 0; i < this.ActiveStatusEffects.Count; i++)
                {
                    if (this.ActiveStatusEffects[i].Applied)
                    {
                        damage.DamageAdd(this.ActiveStatusEffects[i].AdjustDamage(base.gameObject, enemy, damage.Attack) * statDamageHealMultiplier);
                        damage.DamageMult(this.ActiveStatusEffects[i].AdjustDamageMultiplier(base.gameObject, enemy, damage.Attack));
                    }
                }
                for (int j = 0; j < (int)this.BonusDamage.Length; j++)
                {
                    if (this.BonusDamage[j] != 0f)
                    {
                        DamagePacket.DamageProcType damageProcType = new DamagePacket.DamageProcType((DamagePacket.DamageType)j, this.BonusDamage[j]);
                        damage.Damage.DamageProc.Add(damageProcType);
                    }
                }
                this.AddBonusDamagePerType(damage);
                this.AddBonusDamagePerRace(damage, component);
                if (damage.Attack != null)
                {
                    Equippable equippable = damage.Attack.GetComponent<Equippable>();
                    if (equippable)
                    {
                        if (equippable is Weapon && !(damage.Attack is AttackMelee) && enemy != null && !this.IsEnemyDistant(enemy))
                        {
                            damage.DamageMult(this.BonusRangedWeaponCloseEnemyDamageMult);
                        }
                        equippable.ApplyItemModDamageProcs(damage);
                    }
                }
            }
            this.ComputeInterrupt(component, damage);
            if (!testing && this.IsPartyMember)
            {
                if (component)
                {
                    component.RevealDefense(damage.DefendedBy);
                    component.RevealDT(damage.Damage.Type);
                    foreach (DamagePacket.DamageProcType damageProc in damage.Damage.DamageProc)
                    {
                        component.RevealDT(damageProc.Type);
                    }
                }
                if (damage.DefenseRating >= damage.AccuracyRating + 50 || damage.Immune)
                {
                    GameState.AutoPause(AutoPauseOptions.PauseEvent.ExtraordinaryDefence, base.gameObject, enemy, null);
                    TutorialManager.STriggerTutorialsOfTypeFast(TutorialManager.ExclusiveTriggerType.PARTYMEM_GETS_DEFENSE_TOO_HIGH);
                }
            }
            if (!testing && this.OnPostDamageDealt != null)
            {
                this.OnPostDamageDealt(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
            }
        }
    }
}