using System.Collections.Generic;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Targeting {

	[ModifiesType]
	public class mod_AttackAOE : AttackAOE {

		[ModifiesMember("FindAoeTargets")]
		public virtual List<GameObject> FindAoeTargetsNew(GameObject caster, Vector3 parentForward, Vector3 hitPosition) {
			List<GameObject> gameObjects = new List<GameObject>();
			float blastRadius = AdjustedBlastRadius;
			float adjBlastRadius = blastRadius;
			if (ValidTargets == TargetType.All && BlastRadius < AdjustedBlastRadius) {
				blastRadius = BlastRadius;
			}
			foreach (Faction targetFaction in Faction.ActiveFactionComponents) {
				if (targetFaction != null) {
					if (!(targetFaction.gameObject == Owner) || DamageAngleDegrees <= 0f || DamageAngleDegrees >= 360f) {
						Vector3 distVector = targetFaction.transform.position - hitPosition;
						float distSqr = distVector.sqrMagnitude;
						if (ExcludeTarget && distSqr <= float.Epsilon) {
							continue;
						}
						float cachedRadius = targetFaction.CachedRadius;
						float sqrBlastRadius = (blastRadius + cachedRadius) * (blastRadius + cachedRadius);
						float sqrAdjBlastRadius = (adjBlastRadius + cachedRadius) * (adjBlastRadius + cachedRadius);
						Vector3 vector31 = distVector.normalized;
						if (TargetAngle > 0f) {
							vector31 = Quaternion.Euler(0f, TargetAngle, 0f) * vector31;
						}
						float angle = Vector3.Angle(parentForward, vector31);
						bool isWithinAngle = angle <= DamageAngleDegrees * 0.5f;
						bool isWithinAdjRadius = distSqr < float.Epsilon || (isWithinAngle && distSqr <= sqrAdjBlastRadius);
						bool isWithinPureRadius = distSqr < float.Epsilon || (isWithinAngle && distSqr <= sqrBlastRadius);
						if (!isWithinAdjRadius
							|| !GameUtilities.LineofSight(hitPosition, targetFaction.gameObject.transform.position, 1f, false, true)) {
							continue;
						}
						if (ValidTargets != TargetType.All
							|| !IsValidTarget(targetFaction.gameObject, caster, TargetType.Hostile)) {
							if (!isWithinPureRadius || !IsValidTarget(targetFaction.gameObject, caster)) {
								continue;
							}
							var casterFaction = caster.GetComponent<Faction>();
							if (IEModOptions.DisableFriendlyFire && ValidTargets == TargetType.All && mod_AttackBase.FriendlyRightNowAndAlsoWhenConfused(targetFaction.gameObject, caster)) {
								continue;
							}
							//if isWithinPureRadius && IsValidTarget(...)
							gameObjects.Add(targetFaction.gameObject);
						} else {
							//if ValidTargets == TargetType.All && IsValidTarget(...,Hostile)
							//basically, if the faction is hostile 
							gameObjects.Add(targetFaction.gameObject);
						}
					}
				}
			}
			int num = gameObjects.IndexOf(Owner);
			if (num > 0) {
				gameObjects[num] = gameObjects[0];
				gameObjects[0] = Owner;
			}
			return gameObjects;
		}
	}

}