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
			float statDamageHealMultiplier = this.StatDamageHealMultiplier;
			damage.DamageMult(statDamageHealMultiplier);
			if (!testing && this.OnPreDamageDealt != null) {
				this.OnPreDamageDealt(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
			}
			if (!testing && this.OnAddDamage != null) {
				this.OnAddDamage(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
			}
			int attackerToHitRollOverride = Random.Range(1, 101);
			CharacterStats component = enemy.GetComponent<CharacterStats>();
			if (component == null) {
				return;
			}
			attackerToHitRollOverride = component.GetAttackerToHitRollOverride(attackerToHitRollOverride);
			int num = this.CalculateAccuracy(damage.Attack, enemy);
			bool flag = component.CalculateIsImmune(damage.DefendedBy, damage.Attack, base.gameObject);
			int num1 = component.CalculateDefense(damage.DefendedBy, damage.Attack, base.gameObject);
			if (damage.DefendedBy != CharacterStats.DefenseType.None) {
				this.ComputeHitAdjustment(attackerToHitRollOverride + num - num1, component, damage);
				//!+ ADDED CODE
				if (IEModOptions.DisableFriendlyFire) {

					var faction = enemy.Component<Faction>();
					if (faction.IsFriendly(base.gameObject) && base.IsPartyMember && faction.isPartyMember) {
						damage.IsCriticalHit = false;
						damage.Interrupts = false;
						damage.IsGraze = false;
						damage.IsKillingBlow = false;
						damage.IsMiss = true;
					}
				}
				//!+ END ADD
				if (!testing && this.OnAttackRollCalculated != null) {
					this.OnAttackRollCalculated(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
				}
				if (damage.IsCriticalHit) {
					float criticalHitMultiplier = this.CriticalHitMultiplier;
					Health health = enemy.GetComponent<Health>();
					if (health != null && health.StaminaPercentage < 0.1f) {
						criticalHitMultiplier = criticalHitMultiplier + this.CritHitDamageMultiplierBonusEnemyBelow10Percent;
					}
					damage.DamageMult(criticalHitMultiplier);
				} else if (damage.IsGraze) {
					damage.DamageMult(CharacterStats.GrazeMultiplier);
				} else if (damage.IsMiss) {
					damage.DamageMult(0f);
				}
			}
			WeaponSpecializationData.AddWeaponSpecialization(this, damage);
			damage.AccuracyRating = num;
			damage.DefenseRating = num1;
			damage.Immune = flag;
			damage.RawRoll = attackerToHitRollOverride;
			if (!testing && damage.Immune) {
				UIHealthstringManager.Instance.ShowNotice(GUIUtils.GetText(2188), enemy, 1f);
			}
			if (!testing && this.OnAdjustCritGrazeMiss != null) {
				this.OnAdjustCritGrazeMiss(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
			}
			if (!damage.IsMiss) {
				if (damage.Attack.IsDisengagementAttack) {
					damage.DamageAdd(this.DisengagementDamageBonus * statDamageHealMultiplier);
				}
				if (damage.Attack is AttackMelee) {
					damage.DamageMult(this.BonusMeleeDamageMult);
					damage.DamageAdd(this.BonusMeleeDamage * statDamageHealMultiplier);
					if ((damage.Attack as AttackMelee).Unarmed) {
						damage.DamageAdd(this.BonusUnarmedDamage * statDamageHealMultiplier);
					}
				}
				for (int i = 0; i < (int) this.BonusDamage.Length; i++) {
					if (this.BonusDamage[i] != 0f) {
						DamagePacket.DamageProcType damageProcType = new DamagePacket.DamageProcType((DamagePacket.DamageType) i,
							this.BonusDamage[i]);
						damage.Damage.DamageProc.Add(damageProcType);
					}
				}
				this.AddBonusDamagePerType(damage);
				this.AddBonusDamagePerRace(damage, component);
				if (damage.Attack != null) {
					Equippable equippable = damage.Attack.GetComponent<Equippable>();
					if (equippable != null) {
						if (equippable is Weapon) {
							if (!(damage.Attack is AttackMelee)) {
								damage.DamageMult(this.BonusRangedWeaponDamageMult);
								if (enemy != null && !this.IsEnemyDistant(enemy)) {
									damage.DamageMult(this.BonusRangedWeaponCloseEnemyDamageMult);
								}
							} else {
								damage.DamageMult(this.BonusMeleeWeaponDamageMult);
								if (equippable.BothPrimaryAndSecondarySlot) {
									damage.DamageMult(this.BonusTwoHandedMeleeWeaponDamageMult);
								}
							}
						}
						equippable.ApplyItemModDamageProcs(damage);
					}
				}
			}
			this.ComputeInterrupt(component, damage);
			if (!testing && this.IsPartyMember) {
				if (component) {
					component.RevealDefense(damage.DefendedBy);
					component.RevealDT(damage.Damage.Type);
					foreach (DamagePacket.DamageProcType damageProc in damage.Damage.DamageProc) {
						component.RevealDT(damageProc.Type);
					}
				}
				if (damage.DefenseRating >= damage.AccuracyRating + 50 || damage.Immune) {
					GameState.AutoPause(AutoPauseOptions.PauseEvent.ExtraordinaryDefence, base.gameObject, enemy, null);
					TutorialManager.STriggerTutorialsOfTypeFast(TutorialManager.ExclusiveTriggerType.PARTYMEM_GETS_DEFENSE_TOO_HIGH);
				}
			}
			if (!testing && this.OnPostDamageDealt != null) {
				this.OnPostDamageDealt(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
			}
		}
	}
}