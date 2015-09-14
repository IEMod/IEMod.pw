using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Targeting {
	[ModifiesType]
	internal class mod_AttackBase : AttackBase {
		[NewMember]
		public static bool HostileEvenIfConfused(GameObject target, GameObject caster) {
			var targetFaction = target.GetComponent<Faction>();
			var casterFaction = caster.GetComponent<Faction>();
			var targetAiController = GameUtilities.FindActiveAIController(target);
			if (targetFaction.IsHostile(caster) == true || casterFaction.IsHostile(target) == true) {
				//they're actually hostile
				return true;
			}
			if (targetAiController == null) {
				//not AI controller. Can never be confused.
				return false;
			}
			if (!IEModOptions.TargetTurnedEnemies) {
				//no more checks if the option is disabled.
				return false;
			}
	
			var targetOriginal = targetAiController.GetOriginalTeam().GetRelationship(casterFaction.CurrentTeam)
				== Faction.Relationship.Hostile;

			return targetOriginal;
		}

		[ModifiesMember("IsValidTarget")]
		public bool mod_IsValidTarget(GameObject target, GameObject caster, TargetType validType) {
			//+ UNTOUCHED ORIGINAL CODE
			if (IgnoreValidTargetCheck()) {
				return true;
			}
			if (target == null) {
				return false;
			}
			if (caster == null) {
				caster = Owner;
			}
			Health component = target.GetComponent<Health>();
			if (component == null || !component.CanBeTargeted || !target.activeInHierarchy) {
				return false;
			}
			if (component.Dead) {
				if (!TargetTypeUtils.ValidTargetDead(validType)) {
					return false;
				}
			} else if (component.Unconscious && !TargetTypeUtils.ValidTargetUnconscious(validType)) {
				return false;
			}
			AIController aIController = GameUtilities.FindActiveAIController(target);
			if (aIController != null && aIController.IsBusy) {
				return false;
			}
			CharacterStats characterStat = target.GetComponent<CharacterStats>();
			if (ApplyOnceOnly && m_ability != null && characterStat != null
				&& characterStat.CountStatusEffects(m_ability) > 0) {
				return false;
			}
			Faction casterFaction = caster.GetComponent<Faction>();
			Faction targetFaction = target.GetComponent<Faction>();
			//+ END ORIGINAL CODE
			//!+ MODIFICATIONS
			//Replaced all appearances of 'faction.IsHostile(target) || component1 != null && component1.IsHostile(caster)' with 'HostileEvenIfConfused(target,caster)'
			switch (validType) {
				case TargetType.All: {
					return true;
				}
				case TargetType.Hostile: {
					if (HostileEvenIfConfused(target, caster)) {
						return true;
					}
					break;
				}
				case TargetType.Friendly: {
					if (!HostileEvenIfConfused(target, caster)) {
						return true;
					}
					break;
				}
				case TargetType.FriendlyUnconscious: {
					if (casterFaction != null && component.Unconscious && !component.Dead && !casterFaction.IsHostile(target)
						&& (targetFaction == null || !targetFaction.IsHostile(caster))) {
						return true;
					}
					break;
				}
				case TargetType.AllDeadOrUnconscious: {
					if (component.Unconscious || component.Dead) {
						return true;
					}
					break;
				}
				case TargetType.HostileWithGrimoire: {
					if (HostileEvenIfConfused(target, caster)) {
						Equipment equipment = target.GetComponent<Equipment>();
						if (equipment != null && equipment.CurrentItems != null && equipment.CurrentItems.Grimoire != null) {
							return true;
						}
					}
					break;
				}
				case TargetType.HostileVessel: {
					if (HostileEvenIfConfused(target, caster)
						&& characterStat != null && characterStat.CharacterRace == CharacterStats.Race.Vessel) {
						return true;
					}
					break;
				}
				case TargetType.HostileBeast: {
					if (HostileEvenIfConfused(target, caster)
						&& characterStat != null && characterStat.CharacterRace == CharacterStats.Race.Beast) {
						return true;
					}
					break;
				}
				case TargetType.Dead: {
					if (component.Dead) {
						return true;
					}
					break;
				}
				case TargetType.Ally: {
					PartyMemberAI partyMemberAI = caster.GetComponent<PartyMemberAI>();
					PartyMemberAI partyMemberAI1 = target.GetComponent<PartyMemberAI>();
					if (partyMemberAI != null && partyMemberAI.gameObject.activeInHierarchy) {
						if (partyMemberAI1 != null && partyMemberAI1.gameObject.activeInHierarchy) {
							return true;
						}
					} else if (partyMemberAI1 == null || !partyMemberAI1.gameObject.activeInHierarchy) {
						return true;
					}
					break;
				}
				case TargetType.AllyNotSelf: {
					if (target != caster && IsValidTarget(target, caster, TargetType.Ally)) {
						return true;
					}
					break;
				}
				case TargetType.AllyNotSelfOrHostile: {
					if (IsValidTarget(target, caster, TargetType.Hostile)
						|| IsValidTarget(target, caster, TargetType.AllyNotSelf)) {
						return true;
					}
					break;
				}
				case TargetType.NotSelf: {
					if (target != caster) {
						return true;
					}
					break;
				}
				case TargetType.DragonOrDrake: {
					if (characterStat != null
						&& (characterStat.CharacterClass == CharacterStats.Class.Drake
							|| characterStat.CharacterClass == CharacterStats.Class.AdraDragon
							|| characterStat.CharacterClass == CharacterStats.Class.SkyDragon)) {
						return true;
					}
					break;
				} case TargetType.FriendlyIncludingCharmed: {
					if (!HostileEvenIfConfused(target, caster)) {
						return true;
					}
					if (caster != null) {
						Faction faction1 = caster.GetComponent<Faction>();
						if (faction1 != null && aIController != null) {
							Team originalTeam = aIController.GetOriginalTeam();
							if (originalTeam != null && originalTeam.GetRelationship(faction1.CurrentTeam) != Faction.Relationship.Hostile) {
								return true;
							}
						}
					}
					break;
				}
				case TargetType.Self: {
					if (target == caster) {
						return true;
					}
					break;
				}
				case TargetType.FriendlyNotVessel: {
					if (characterStat != null && characterStat.CharacterRace != CharacterStats.Race.Vessel && !HostileEvenIfConfused(target, caster)) {
						return true;
					}
					break;
				}
			}
			//!+END MODIFICATIONS
			return false;
		}
	}
}