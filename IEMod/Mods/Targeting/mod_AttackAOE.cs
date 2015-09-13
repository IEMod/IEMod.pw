using System.Collections.Generic;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Targeting {

	[ModifiesType]
	public class mod_AttackAOE : AttackAOE {

		//[ModifiesMember("FindAoeTargets")]
		public virtual List<GameObject> FindAoeTargetsNew(GameObject caster, Vector3 parentForward, Vector3 hitPosition) {
			List<GameObject> gameObjects = new List<GameObject>();
			float blastRadius = AdjustedBlastRadius;
			float adjBlastRadius = blastRadius;
			if (ValidTargets == TargetType.All && BlastRadius < AdjustedBlastRadius) {
				blastRadius = BlastRadius;
			}
			foreach (Faction activeFactionComponent in Faction.ActiveFactionComponents) {
				if (activeFactionComponent != null) {
					if (!(activeFactionComponent.gameObject == Owner) || DamageAngleDegrees <= 0f || DamageAngleDegrees >= 360f) {
						Vector3 distVector = activeFactionComponent.transform.position - hitPosition;
						float distSqr = distVector.sqrMagnitude;
						if (ExcludeTarget && distSqr <= float.Epsilon) {
							continue;
						}
						float cachedRadius = activeFactionComponent.CachedRadius;
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
							|| !GameUtilities.LineofSight(hitPosition, activeFactionComponent.gameObject.transform.position, 1f, false, true)) {
							continue;
						}
						if (ValidTargets != TargetType.All
							|| !IsValidTarget(activeFactionComponent.gameObject, caster, TargetType.Hostile)) {
							if (!isWithinPureRadius || !IsValidTarget(activeFactionComponent.gameObject, caster)) {
								continue;
							}
							gameObjects.Add(activeFactionComponent.gameObject);
						} else {
							gameObjects.Add(activeFactionComponent.gameObject);
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