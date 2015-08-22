using System;
using System.Collections.Generic;
using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace IEMod.Mods.Targeting {
	[ModifiesType]
	internal class mod_AttackBase : AttackBase {
		[NewMember]
		public static string OnImpactDebugRegister;

		[ModifiesMember]
		[PatchworkDebugRegister(nameof(OnImpactDebugRegister))]
		private new GameObject CheckRedirect(GameObject enemy) {
			try {
				if (this is AttackMelee && enemy.GetComponent<CharacterStats>().RedirectMeleeAttacks) {
					GameObject[] gameObjectArray = GameUtilities.CreaturesInRange(this.Owner.transform.position,
						this.TotalAttackDistance, enemy, false);
					if (gameObjectArray != null) {
						int num = Random.Range(0, (int) gameObjectArray.Length);
						enemy = gameObjectArray[num];
					}
				}
				return enemy;
			}
			catch (Exception ex) {
				IEDebug.PrintException(ex);
				IEDebug.Log($"Register: {OnImpactDebugRegister}");
				throw;
			}
		}

		[ModifiesMember]
		[PatchworkDebugRegister(nameof(OnImpactDebugRegister))]
		public new virtual void OnImpact(GameObject self, GameObject enemy, bool isMainTarget) {
			try {
				DamageInfo dTBypass;
				if (this.m_cancelled || enemy == null) {
					this.m_can_cancel = false;
					this.TriggerNoise();
					if (isMainTarget) {
						AttackBase mNumImpacts = this;
						mNumImpacts.m_numImpacts = mNumImpacts.m_numImpacts - 1;
						if (this.m_numImpacts <= 0) {
							this.CleanUpAttack(enemy);
							if (this.m_ability != null) {
								this.m_ability.AttackComplete = true;
							}
						}
					}
					return;
				}
				this.m_can_cancel = false;
				this.TriggerNoise();
				this.m_impactFrameHit = true;
				//the following is a block that's supposed to fix an issue with various Wizard trap attacks having a null parent.
				//I don't know what causes it.
				GameObject owner = this.Owner;
				var parent = this.transform.parent;
				if (owner == null) {
					if (parent.HasComponent<Trap>()) {
						var foundOwner = parent.Component<Trap>().Owner;
						if (foundOwner == null) {
							IEDebug.Log(
								$"Trying to fix {this.name}'s null owner, but parent trap {parent.name}'s owner is null too. This means this ability probably won't work properly.");
						}
						if (this.m_ability != null) {
							this.m_ability.Owner = foundOwner;
						}

						this.Owner = foundOwner;
						owner = foundOwner;
					} else {
						UnityPrinter.FullPrinter.Print(this);
						UnityPrinter.ComponentPrinter.Print(parent);
					}
				}	
				CharacterStats component = owner.GetComponent<CharacterStats>(); //no owner??
				CharacterStats characterStat = enemy.GetComponent<CharacterStats>();
				if (this.GetBounceCount(self) == 0 && this.m_ability != null && !(this is AttackRanged) && !(this is AttackBeam)) {
					this.m_ability.Activate(enemy);
				}
				if (owner.GetComponent<Equipment>() == null) {
					Debug.LogError(string.Concat("Parent object ", owner.name, " has no equipment component! What is this insanity!?"),
						owner);
					return;
				}
				Console.Instance.ResetMessageDelta();
				List<StatusEffect> statusEffects = new List<StatusEffect>();
				List<StatusEffect> statusEffects1 = new List<StatusEffect>();
				DamageInfo damageInfo = null;
				this.CalcDamage(enemy, self, out dTBypass);
				if (component) {
					dTBypass.AttackerDTBypass = component.DTBypass;
					if (this is AttackRanged) {
						DamageInfo attackerDTBypass = dTBypass;
						attackerDTBypass.AttackerDTBypass = attackerDTBypass.AttackerDTBypass + component.RangedDTBypass;
					}
					if (this is AttackMelee) {
						DamageInfo attackerDTBypass1 = dTBypass;
						attackerDTBypass1.AttackerDTBypass = attackerDTBypass1.AttackerDTBypass + component.MeleeDTBypass;
					}
				}
				if (component != null) {
					if (!dTBypass.IsMiss) {
						component.TriggerWhenHits(enemy, ref dTBypass);
					} else {
						component.TriggerWhenMisses(enemy, ref dTBypass);
					}
				}
				if (!dTBypass.IsMiss && characterStat) {
					characterStat.TriggerWhenHit(owner, ref dTBypass);
				}
				if (!dTBypass.IsMiss) {
					this.PushEnemy(enemy, this.PushDistance, null, dTBypass);
					if (characterStat) {
						if (this.HasKeyword("poison") || this.HasAfflictionWithKeyword("poison")) {
							if (
								!characterStat.HasStatusEffectWithSearchFunction(
									(StatusEffect effect) => (effect == null ? false : effect.IsPoisonEffect))) {
								SoundSet.TryPlayVoiceEffectWithLocalCooldown(characterStat.gameObject, SoundSet.SoundAction.Poisoned,
									SoundSet.s_LongVODelay, false);
							}
						}
						this.ApplyStatusEffects(enemy, dTBypass, true, statusEffects);
						damageInfo = this.ApplyAfflictions(enemy, dTBypass, true, statusEffects, statusEffects1);
						Equippable equippable = base.gameObject.GetComponent<Equippable>();
						if (equippable != null) {
							equippable.ApplyItemModAttackEffects(base.gameObject, characterStat, dTBypass, statusEffects);
						}
					}
					Transform transform = AttackBase.GetTransform(owner, this.OnHitAttackerAttach);
					GameUtilities.LaunchEffect(this.OnHitAttackerVisualEffect, transform.position, transform.rotation, 1f, transform,
						this.m_ability);
					GameUtilities.LaunchEffect(this.OnHitGroundVisualEffect, 1f, enemy.transform.position, this.m_ability);
					if (!(this is AttackRanged)) {
						Transform hitTransform = this.GetHitTransform(enemy);
						GameUtilities.LaunchEffect(this.OnHitVisualEffect, hitTransform.position, hitTransform.rotation, 1f, hitTransform,
							this.m_ability);
					} else {
						GameUtilities.LaunchEffect(this.OnHitVisualEffect, self.transform.position, Quaternion.identity, 1f,
							enemy.transform, this.m_ability);
					}
					Weapon weapon = base.gameObject.GetComponent<Weapon>();
					if (weapon != null) {
						weapon.Deteriorate();
					}
					Equipment equipment = enemy.GetComponent<Equipment>();
					if (equipment && equipment.CurrentItems != null) {
						Equippable chest = equipment.CurrentItems.Chest;
						if (chest) {
							chest.Deteriorate();
						}
						Shield equippedShield = equipment.EquippedShield;
						if (equippedShield != null) {
							chest = equippedShield.gameObject.GetComponent<Equippable>();
							if (chest) {
								chest.Deteriorate();
							}
						}
					}
					if (this.IsDisengagementAttack && AttackData.Instance.DefaultDisengagementFx != null) {
						Vector3 vector3 = owner.transform.position - enemy.transform.position;
						Quaternion quaternion = Quaternion.LookRotation(vector3.normalized);
						Vector3 vector31 = owner.transform.position;
						vector31.y = vector31.y;
						GameUtilities.LaunchEffect(AttackData.Instance.DefaultDisengagementFx, vector31, quaternion, 1f, null,
							this.m_ability);
					}
					AudioBank audioBank = owner.GetComponent<AudioBank>();
					if (audioBank != null) {
						audioBank.PlayFrom("HitEnemy");
					}
				} else {
					if (!(this is AttackRanged)) {
						GameUtilities.LaunchEffect(this.OnMissVisualEffect, enemy.transform.position, Quaternion.identity, 1f,
							enemy.transform, this.m_ability);
					} else {
						GameUtilities.LaunchEffect(this.OnMissVisualEffect, 1f, self.transform.position, this.m_ability);
					}
					int accuracyRating = dTBypass.AccuracyRating - dTBypass.DefenseRating;
					if (dTBypass.RawRoll + accuracyRating < 0) {
						this.PlayCriticalMiss();
					}
					AudioBank component1 = owner.GetComponent<AudioBank>();
					if (component1 != null) {
						bool flag = false;
						Weapon weapon1 = base.gameObject.GetComponent<Weapon>();
						if (weapon1 != null) {
							flag = weapon1.PlayMissSound();
						}
						if (!flag) {
							component1.PlayFrom("Miss");
						}
					}
				}
				Health health = enemy.GetComponent<Health>();
				if (health) {
					if (!dTBypass.IsMiss && !health.Unconscious && !health.Dead) {
						Weapon component2 = base.gameObject.GetComponent<Weapon>();
						if (component2 != null) {
							component2.PlayHitSound();
						} else if (this is AttackMelee && ((AttackMelee)(object) this).Unarmed) {
							AudioBank audioBank1 = owner.GetComponent<AudioBank>();
							ClipBankSet weaponHitSoundSet =
								GlobalAudioPlayer.Instance.GetWeaponHitSoundSet(WeaponSpecializationData.WeaponType.Unarmed);
							if (audioBank1 && weaponHitSoundSet != null) {
								audioBank1.PlayFrom(weaponHitSoundSet);
							}
						}
					}
					bool flag1 = true;
					if (!this.IsFakeAttack) {
						flag1 = health.DoDamage(dTBypass, owner) > 0f;
					}
					GameObject hitEffect = this.GetHitEffect(dTBypass.Damage.Type, enemy);
					if (hitEffect != null && flag1) {
						if (!(this is AttackRanged)) {
							Transform transforms = this.GetHitTransform(enemy);
							GameUtilities.LaunchEffect(hitEffect, transforms.position, transforms.rotation, 1f, transforms, this.m_ability);
						} else {
							GameUtilities.LaunchEffect(hitEffect, self.transform.position, Quaternion.identity, 1f, enemy.transform,
								this.m_ability);
						}
					}
				}
				
				if (dTBypass.IsCriticalHit && this.OnCriticalHit != null || Health.BloodyMess) {
					this.OnCriticalHit(owner, new CombatEventArgs(dTBypass, owner, enemy));
				}
				if (FogOfWar.Instance == null || FogOfWar.Instance.PointVisible(owner.transform.position)
					|| FogOfWar.Instance.PointVisible(enemy.transform.position)) {
					AttackBase.s_PostUseDelta = true;
					AttackBase.PostAttackMessages(enemy, dTBypass, statusEffects, true);
					if (damageInfo != null) {
						AttackBase.PostAttackMessagesSecondaryEffect(enemy, damageInfo, statusEffects1);
					}
					AttackBase.s_PostUseDelta = false;
				}
				this.ResetAttackVars();
				this.CheckBouncing(self, enemy);
				if (!dTBypass.IsMiss && this.ExtraAOE != null
					&& (!(this is AttackRanged) || !((AttackRanged)(object) this).ExtraAOEOnBounce)) {
					AttackAOE mAbility = Object.Instantiate(this.ExtraAOE) as AttackAOE;
					mAbility.DestroyAfterImpact = true;
					mAbility.Owner = owner;
					mAbility.transform.parent = owner.transform;
					mAbility.m_skipAnimation = true;
					mAbility.ParentAttack = this;
					mAbility.m_ability = this.m_ability;
					mAbility.ShowImpactEffect(enemy.transform.position);
					GameObject gameObject = null;
					if (mAbility.ExcludeTarget) {
						gameObject = enemy;
					}
					mAbility.OnImpactShared(null, enemy.transform.position, gameObject);
				}
				if (isMainTarget) {
					AttackBase attackBase = this;
					attackBase.m_numImpacts = attackBase.m_numImpacts - 1;
					if (this.m_numImpacts <= 0) {
						this.CleanUpAttack(enemy);
						if (this.OnAttackComplete != null) {
							this.OnAttackComplete(owner, new CombatEventArgs(dTBypass, owner, enemy));
						}
						if (this.m_ability != null) {
							this.m_ability.AttackComplete = true;
						}
					}
				}
				if (dTBypass.TargetPreviouslyDead || health.Dead != true && health.Unconscious != true) {
					GameState.AutoPause(AutoPauseOptions.PauseEvent.PartyMemberAttacked, enemy, owner, null);
				} else {
					GameState.AutoPause(AutoPauseOptions.PauseEvent.TargetDestroyed, owner, enemy, null);
				}
			}
			catch (Exception ex) {
				IEDebug.Log(IEDebug.PrintException(ex));
				IEDebug.Log($"Debug Register: {OnImpactDebugRegister}");
				throw;
			}
		}

		[ModifiesMember("IsValidTarget")]
		public bool IsValidTargetNew(GameObject target, GameObject caster, TargetType validType) {
			var confusedArentFriends = IEModOptions.TargetTurnedEnemies;

			//(original decompiled code:)

			if (IgnoreValidTargetCheck()) {
				return true;
			}
			if (target == null) {
				return false;
			}
			if (caster == null) {
				caster = Owner;
			}
			var component1 = target.GetComponent<Health>();
			if (component1 == null || !component1.CanBeTargeted || !target.activeInHierarchy) {
				return false;
			}
			var activeAiController = GameUtilities.FindActiveAIController(target);
			if (activeAiController != null && activeAiController.IsBusy) {
				return false;
			}
			var component2 = target.GetComponent<CharacterStats>();
			if (ApplyOnceOnly && m_ability != null
				&& (component2 != null && component2.CountStatusEffects(m_ability) > 0)) {
				return false;
			}
			var component3 = caster.GetComponent<Faction>();
			var component4 = target.GetComponent<Faction>();
			switch (validType) {
				case TargetType.All:
					if (!component1.Unconscious && !component1.Dead) {
						return true;
					}
					break;
				case TargetType.Hostile:
					//(this part is modified)
					if (component1.Unconscious || component1.Dead) { break; }
					if ((component3 != null && component3.IsHostile(target))
						|| (component4 != null && component4.IsHostile(caster))) {
						return true;
					}

					if (confusedArentFriends && activeAiController != null && component3 != null
						&& activeAiController.GetOriginalTeam().GetRelationship(component3.CurrentTeam) == Faction.Relationship.Hostile) {
						return true;
					}
					break;
				case TargetType.Friendly:
					if (component1.Unconscious || component1.Dead) { break; }
					if (component3 != null && !component3.IsHostile(target)
						&& (component4 == null || !component4.IsHostile(caster))) {
						if (!confusedArentFriends || activeAiController == null
							|| activeAiController.GetOriginalTeam().GetRelationship(component3.CurrentTeam) != Faction.Relationship.Hostile) {
							return true;
						}
					}
					break;
				case TargetType.FriendlyUnconscious:
					if (component3 != null && component1.Unconscious && !component3.IsHostile(target)
						&& (component4 == null || !component4.IsHostile(caster))) {
						return true;
					}
					break;
				case TargetType.AllDeadOrUnconscious:
					if (component1.Unconscious || component1.Dead) {
						return true;
					}
					break;
				case TargetType.HostileWithGrimoire:
					if (component3 != null && component3.IsHostile(target)
						|| component4 != null && component4.IsHostile(caster)) {
						var component5 = target.GetComponent<Equipment>();
						if (component5 != null && component5.CurrentItems != null
							&& component5.CurrentItems.Grimoire != null) {
							return true;
						}
					}
					break;
				case TargetType.HostileVessel:
					if ((component3 != null && component3.IsHostile(target)
						|| component4 != null && component4.IsHostile(caster))
						&& (component2 != null && component2.CharacterRace == CharacterStats.Race.Vessel)) {
						return true;
					}
					break;
				case TargetType.HostileBeast:
					if ((component3 != null && component3.IsHostile(target)
						|| component4 != null && component4.IsHostile(caster))
						&& (component2 != null && component2.CharacterRace == CharacterStats.Race.Beast)) {
						return true;
					}
					break;
				case TargetType.Dead:
					if (component1.Dead) {
						return true;
					}
					break;
				case TargetType.Ally:
					var component6 = caster.GetComponent<PartyMemberAI>();
					var component7 = target.GetComponent<PartyMemberAI>();
					if (component6 != null && component6.gameObject.activeInHierarchy) {
						if (component7 != null && component7.gameObject.activeInHierarchy) {
							return true;
						}
						break;
					}
					if (component7 == null || !component7.gameObject.activeInHierarchy) {
						return true;
					}
					break;
				case TargetType.AllyNotSelf:
					if (target != caster && IsValidTarget(target, caster, TargetType.Ally)) {
						return true;
					}
					break;
				case TargetType.AllyNotSelfOrHostile:
					if (IsValidTarget(target, caster, TargetType.Hostile) || IsValidTarget(target, caster, TargetType.AllyNotSelf)) {
						return true;
					}
					break;
				case TargetType.NotSelf:
					if (target != caster && !component1.Unconscious && !component1.Dead) {
						return true;
					}
					break;
				case TargetType.DragonOrDrake:
					if (component2 != null
						&& (component2.CharacterClass == CharacterStats.Class.Drake
							|| component2.CharacterClass == CharacterStats.Class.AdraDragon
							|| component2.CharacterClass == CharacterStats.Class.SkyDragon)) {
						return true;
					}
					break;
				case TargetType.FriendlyIncludingCharmed:
					if (!component1.Unconscious && !component1.Dead) {
						if (component3 != null && !component3.IsHostile(target)) {
							return true;
						}
						if (caster != null) {
							var component5 = caster.GetComponent<Faction>();
							if (component5 != null && activeAiController != null) {
								var originalTeam = activeAiController.GetOriginalTeam();
								if (originalTeam != null
									&& originalTeam.GetRelationship(component5.CurrentTeam) != Faction.Relationship.Hostile) {
									return true;
								}
							}
						}
					}
					break;
				case TargetType.Self:
					if (target == caster && !component1.Unconscious && !component1.Dead) {
						return true;
					}
					break;
			}
			return false;
		}
	}
}