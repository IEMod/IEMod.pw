/*
 * Friendly AOE was not affecting caster regardles of other intentions... 
 * Since the damage is done in mod_AdjustDamageDealt of mod_CharacterSats this will only affect the UI but messes up the whole thing otherwise... 
 * I think the virtual property is messing this up somehow, in the meantime I'm taking it out

using System.Collections.Generic;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Targeting {

	[ModifiesType]
	public class mod_AttackAOE : AttackAOE {

		[ModifiesMember("FindAoeTargets")]
        public virtual List<GameObject> FindAoeTargetsNew(GameObject caster, Vector3 parentForward, Vector3 hitPosition, bool forUi)
        {
            bool flag;
            bool flag1;
            List<GameObject> gameObjects = new List<GameObject>();
            float adjustedBlastRadius = this.AdjustedBlastRadius;
            float single = adjustedBlastRadius;
            if (this.ValidTargets == AttackBase.TargetType.All && this.BlastRadius < this.AdjustedBlastRadius)
            {
                adjustedBlastRadius = this.BlastRadius;
            }
            foreach (Faction activeFactionComponent in Faction.ActiveFactionComponents)
            {
                if (activeFactionComponent != null)
                {
                    if (!(activeFactionComponent.gameObject == base.Owner) || this.DamageAngleDegrees <= 0f || this.DamageAngleDegrees >= 360f)
                    {
                        Vector3 vector3 = activeFactionComponent.transform.position - hitPosition;
                        float single1 = vector3.sqrMagnitude;
                        if (this.ExcludeTarget && single1 <= 1.401298E-45f)
                        {
                            continue;
                        }
                        float cachedRadius = activeFactionComponent.CachedRadius;
                        float single2 = (adjustedBlastRadius + cachedRadius) * (adjustedBlastRadius + cachedRadius);
                        float single3 = (single + cachedRadius) * (single + cachedRadius);
                        Vector3 vector31 = vector3.normalized;
                        if (this.TargetAngle > 0f)
                        {
                            vector31 = Quaternion.Euler(0f, this.TargetAngle, 0f) * vector31;
                        }
                        float single4 = Vector3.Angle(parentForward, vector31);
                        bool damageAngleDegrees = single4 <= this.DamageAngleDegrees * 0.5f;
                        if (single1 < 1.401298E-45f)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = (!damageAngleDegrees ? false : single1 <= single3);
                        }
                        bool flag2 = flag;
                        if (single1 < 1.401298E-45f)
                        {
                            flag1 = true;
                        }
                        else
                        {
                            flag1 = (!damageAngleDegrees ? false : single1 <= single2);
                        }
                        bool flag3 = flag1;
                        if (!flag2 || !GameUtilities.LineofSight(hitPosition, activeFactionComponent.gameObject.transform.position, 1f, false, true))
                        {
                            continue;
                        }
                        if (this.ValidTargets != AttackBase.TargetType.All || !base.IsValidTarget(activeFactionComponent.gameObject, caster, AttackBase.TargetType.Hostile))
                        {
                            if (!flag3 || !base.IsValidTarget(activeFactionComponent.gameObject, caster))
                            {
                                continue;
                            }

                            var casterFaction = caster.GetComponent<Faction>();
                            if (IEModOptions.DisableFriendlyFire && ValidTargets == TargetType.All && mod_AttackBase.FriendlyRightNowAndAlsoWhenConfused(activeFactionComponent.gameObject, caster))
                            {
                                continue;
                            }

                            gameObjects.Add(activeFactionComponent.gameObject);
                        }
                        else
                        {
                            gameObjects.Add(activeFactionComponent.gameObject);
                        }
                    }
                }
            }
            int num = gameObjects.IndexOf(base.Owner);
            if (num > 0)
            {
                gameObjects[num] = gameObjects[0];
                gameObjects[0] = base.Owner;
            }
            return gameObjects;
        }

    }

}

Leftover from 2.0, from another modder
        /*
        public virtual List<GameObject> FindAoeTargetsNew(GameObject caster, Vector3 parentForward, Vector3 hitPosition, bool forUI) {
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
        */