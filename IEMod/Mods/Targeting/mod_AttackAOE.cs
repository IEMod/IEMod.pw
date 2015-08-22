using System.Collections.Generic;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Targeting {

	[ModifiesType]
	public class mod_AttackAOE : AttackAOE {

		[ModifiesMember("FindAoeTargets")]
		public virtual List<GameObject> FindAoeTargetsNew(GameObject caster, Vector3 parentForward, Vector3 hitPosition) {
			/*
				The purpose of this method is to return a collection of the targets of a given area attack, according to the rules of the game.
			*/
			bool disableFriendlyFire = IEModOptions.DisableFriendlyFire;
			;
			bool confusedArentFriends = IEModOptions.TargetTurnedEnemies;
			List<GameObject> list = new List<GameObject>();
			float blastRadius = AdjustedBlastRadius;
			//note that ajdBlastRadius is the wider blast radius.
			float adjBlastRadius = blastRadius;
			if (ValidTargets == TargetType.All && BlastRadius < AdjustedBlastRadius) {
				blastRadius = BlastRadius;
			}
			foreach (Faction current in Faction.ActiveFactionComponents) {
				if (current == null) { continue; }
				if (current.gameObject == Owner && !(DamageAngleDegrees <= 0f) && !(DamageAngleDegrees >= 360f)) {
					continue;
				}
				Vector3 vector = current.transform.position - hitPosition;
				float sqrMagnitude = vector.sqrMagnitude;
				if (ExcludeTarget && !(sqrMagnitude > float.Epsilon)) { continue; }
				float cachedRadius = current.CachedRadius;
				float sqrBlastRadiusPlus = (blastRadius + cachedRadius) * (blastRadius + cachedRadius);
				float sqrAdjBlastRadiusPlus = (adjBlastRadius + cachedRadius) * (adjBlastRadius + cachedRadius);
				Vector3 vector2 = vector.normalized;
				if (TargetAngle > 0f) {
					vector2 = Quaternion.Euler(0f, TargetAngle, 0f) * vector2;
				}
				bool isWithinAngle = Vector3.Angle(parentForward, vector2) <= DamageAngleDegrees * 0.5f;
				bool isWithinAdjRadius = sqrMagnitude < float.Epsilon || (isWithinAngle && sqrMagnitude <= sqrAdjBlastRadiusPlus);
				bool isWithinPureRadius = sqrMagnitude < float.Epsilon || (isWithinAngle && sqrMagnitude <= sqrBlastRadiusPlus);
				if (!isWithinAdjRadius || !GameUtilities.LineofSight(hitPosition, current.gameObject.transform.position, 1f, false, true)) {
					//if the target isn't even within the adj (wider) radius OR isn't within line of sight (there is cover)
					//it's not in the target list.
					continue;
				}
				Faction casterFaction = caster.GetComponent<Faction>();
				//if isValidForHostile, that means that the target is valid when it is hostile. This means that the AOE is a negative effect
				var isValidForHostile = IsValidTarget(current.gameObject, caster, TargetType.Hostile);
				var isValidTarget = IsValidTarget(current.gameObject, caster);
				var isCurrentFriendly = (current.isPartyMember || casterFaction.IsFriendly(current));
				var aiController = GameUtilities.FindActiveAIController(current.gameObject);
				var relationshipToCasterFaction = aiController?.GetOriginalTeam()?.GetRelationship(casterFaction.CurrentTeam);
				if ((ValidTargets == TargetType.Friendly || ValidTargets == TargetType.FriendlyIncludingCharmed)
					&& !isValidForHostile
					&& isValidTarget) {
					if (!confusedArentFriends || relationshipToCasterFaction != Faction.Relationship.Hostile) {
						list.Add(current.gameObject);
					}
				} else if (
					(ValidTargets == TargetType.Hostile && !isValidTarget)
						|| (!isWithinPureRadius && ValidTargets == TargetType.All
							&& !isValidForHostile)) {
					if (IsValidTarget(current.gameObject, caster, TargetType.All) && confusedArentFriends && relationshipToCasterFaction == Faction.Relationship.Hostile) {
						list.Add(current.gameObject);
					}
				} else if (ValidTargets == TargetType.All && isValidForHostile) {
					
					if (!disableFriendlyFire || !isCurrentFriendly) {
						list.Add(current.gameObject);
					}
				} else if (isWithinPureRadius && isValidTarget) {
					//in this case, 
					if (!disableFriendlyFire || ValidTargets != TargetType.All || !isCurrentFriendly) {
						list.Add(current.gameObject);
					}
				}
			}
			int num6 = list.IndexOf(Owner);
			if (num6 > 0) {
				list[num6] = list[0];
				list[0] = Owner;
			}
			return list;
		}
	}

}