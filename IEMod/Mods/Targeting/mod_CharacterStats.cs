using System;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Targeting {

	[ModifiesType]
	public class mod_CharacterStats : CharacterStats
	{
		[NewMember]
		[DuplicatesBody(nameof(AdjustDamageDealt))]
		private void orig_AdjustDamageDealt(GameObject enemy, DamageInfo damage, bool testing) {
			
		}

		[ModifiesMember(nameof(AdjustDamageDealt))]
		public void mod_AdjustDamageDealt(GameObject enemy, DamageInfo damage, bool testing) {
			if (IEModOptions.DisableFriendlyFire && gameObject && enemy?.GetComponent<Faction>()?.IsFriendly(gameObject) == true && base.IsPartyMember) {
				damage.IsCriticalHit = damage.Interrupts = damage.IsGraze =damage.IsKillingBlow = false;
				damage.IsMiss = true;
				return;
			}
			orig_AdjustDamageDealt(enemy, damage, testing);
		}

		//[ModifiesMember("AdjustDamageDealt")]
		public void AdjustDamageDealtNew(GameObject enemy, DamageInfo damage)
		{
			bool disableFriendlyFire = IEModOptions.DisableFriendlyFire;
			float statDamageHealMultiplier = this.StatDamageHealMultiplier;
			damage.DamageMult(statDamageHealMultiplier);
			var z = this.OnPreDamageDealt;
			if (this.OnPreDamageDealt != null)
			{
				this.OnPreDamageDealt(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
			}
			if (this.OnAddDamage != null)
			{
				this.OnAddDamage(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
			}
			int num = UnityEngine.Random.Range(1, 101);
			CharacterStats component = enemy.GetComponent<CharacterStats>();
			if (component == null)
			{
				return;
			}
			num = component.GetAttackerToHitRollOverride(num);
			if (this.OnAttackRollCalculated != null)
			{
				this.OnAttackRollCalculated(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
			}
			int num2 = this.CalculateAccuracy(damage.Attack, enemy);
			int num3 = component.CalculateDefense(damage.DefendedBy, damage.Attack, base.gameObject);
			if (damage.DefendedBy != CharacterStats.DefenseType.None)
			{
				int hitValue = num + num2 - num3;
				//GR 27/8 - computeHitAdjustment used to be passed with ref, but this was changed in 2.0
				this.ComputeHitAdjustment(hitValue, component, damage);

				if (disableFriendlyFire)
				{

					if (enemy.GetComponent<Faction>().IsFriendly(base.gameObject) && base.IsPartyMember)
					{
						damage.IsCriticalHit = false;
						damage.Interrupts = false;
						damage.IsGraze = false;
						damage.IsKillingBlow = false;
						damage.IsMiss = true;
					}
				}

				if (damage.IsCriticalHit)
				{
					float num4 = this.CriticalHitMultiplier;
					Health component2 = enemy.GetComponent<Health>();
					if (component2 != null && component2.StaminaPercentage < 0.1f)
					{
						num4 += this.CritHitDamageMultiplierBonusEnemyBelow10Percent;
					}
					damage.DamageMult(num4);
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
			damage.AccuracyRating = num2;
			damage.DefenseRating = num3;
			damage.RawRoll = num;
			if (this.OnAdjustCritGrazeMiss != null)
			{
				this.OnAdjustCritGrazeMiss(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
			}
			if (!damage.IsMiss)
			{
				if (damage.Attack.IsDisengagementAttack)
				{
					damage.DamageAdd(this.DisengagementDamageBonus * statDamageHealMultiplier);
				}
				if (damage.Attack is AttackMelee)
				{
					damage.DamageMult(this.BonusMeleeDamageMult);
					damage.DamageAdd(this.BonusMeleeDamage * statDamageHealMultiplier);
					if ((damage.Attack as AttackMelee).Unarmed)
					{
						damage.DamageAdd(this.BonusUnarmedDamage * statDamageHealMultiplier);
					}
				}
				for (int i = 0; i < this.BonusDamage.Length; i++)
				{
					if (this.BonusDamage[i] != 0f)
					{
						DamagePacket.DamageProcType item = new DamagePacket.DamageProcType((DamagePacket.DamageType)i, this.BonusDamage[i]);
						damage.Damage.DamageProc.Add(item);
					}
				}
				this.AddBonusDamagePerType(damage);
				this.AddBonusDamagePerRace(damage, component);
				if (damage.Attack != null)
				{
					Equippable component3 = damage.Attack.GetComponent<Equippable>();
					if (component3 != null)
					{
						if (component3 is Weapon)
						{
							if (damage.Attack is AttackMelee)
							{
								damage.DamageMult(this.BonusMeleeWeaponDamageMult);
								if (component3.BothPrimaryAndSecondarySlot)
								{
									damage.DamageMult(this.BonusTwoHandedMeleeWeaponDamageMult);
								}
							}
							else
							{
								damage.DamageMult(this.BonusRangedWeaponDamageMult);
								if (enemy != null && !this.IsEnemyDistant(enemy))
								{
									damage.DamageMult(this.BonusRangedWeaponCloseEnemyDamageMult);
								}
							}
						}
						component3.ApplyItemModDamageProcs(damage);
					}
				}
			}
			this.ComputeInterrupt(component, damage);
			if (this.IsPartyMember)
			{
				if (component)
				{
					component.RevealDefense(damage.DefendedBy);
					component.RevealDT(damage.Damage.Type);
					foreach (DamagePacket.DamageProcType current in damage.Damage.DamageProc)
					{
						component.RevealDT(current.Type);
					}
				}
				if (damage.DefenseRating >= damage.AccuracyRating + 50)
				{
					global::GameState.AutoPause(AutoPauseOptions.PauseEvent.ExtraordinaryDefence, base.gameObject, enemy, null);
					TutorialManager.STriggerTutorialsOfTypeFast(TutorialManager.ExclusiveTriggerType.PARTYMEM_GETS_DEFENSE_TOO_HIGH);
				}
			}
			if (this.OnPostDamageDealt != null)
			{
				this.OnPostDamageDealt(base.gameObject, new CombatEventArgs(damage, base.gameObject, enemy));
			}
		}
	}
}