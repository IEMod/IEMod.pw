using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Targeting {
	[ModifiesType]
	internal class mod_AttackBase : AttackBase {
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
			var targetFaction = target.GetComponent<Faction>();
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
						|| (targetFaction != null && targetFaction.IsHostile(caster))) {
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
						&& (targetFaction == null || !targetFaction.IsHostile(caster))) {
						if (!confusedArentFriends || activeAiController == null
							|| activeAiController.GetOriginalTeam().GetRelationship(component3.CurrentTeam) != Faction.Relationship.Hostile) {
							return true;
						}
					}
					break;
				case TargetType.FriendlyUnconscious:
					if (component3 != null && component1.Unconscious && !component3.IsHostile(target)
						&& (targetFaction == null || !targetFaction.IsHostile(caster))) {
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
						|| targetFaction != null && targetFaction.IsHostile(caster)) {
						var component5 = target.GetComponent<Equipment>();
						if (component5 != null && component5.CurrentItems != null
							&& component5.CurrentItems.Grimoire != null) {
							return true;
						}
					}
					break;
				case TargetType.HostileVessel:
					if ((component3 != null && component3.IsHostile(target)
						|| targetFaction != null && targetFaction.IsHostile(caster))
						&& (component2 != null && component2.CharacterRace == CharacterStats.Race.Vessel)) {
						return true;
					}
					break;
				case TargetType.HostileBeast:
					if ((component3 != null && component3.IsHostile(target)
						|| targetFaction != null && targetFaction.IsHostile(caster))
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