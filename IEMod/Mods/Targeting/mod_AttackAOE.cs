using System.Collections.Generic;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Targeting {

	[ModifiesType]
	public class mod_AttackAOE : AttackAOE {

		[ModifiesMember("FindAoeTargets")]
		public virtual List<GameObject> FindAoeTargetsNew(GameObject caster, Vector3 parentForward, Vector3 hitPosition)
		{
			bool disableFriendlyFire = PlayerPrefs.GetInt("DisableFriendlyFire", 0) == 1;
			bool confusedArentFriends = PlayerPrefs.GetInt("TargetTurnedEnemies", 0) == 1;
			List<GameObject> list = new List<GameObject>();
			float num = this.AdjustedBlastRadius;
			float num2 = num;
			if (this.ValidTargets == AttackBase.TargetType.All && this.BlastRadius < this.AdjustedBlastRadius)
			{
				num = this.BlastRadius;
			}
			foreach (Faction current in Faction.ActiveFactionComponents)
			{
				if (!(current == null))
				{
					if (!(current.gameObject == base.Owner) || this.DamageAngleDegrees <= 0f || this.DamageAngleDegrees >= 360f)
					{
						Vector3 vector = current.transform.position - hitPosition;
						float sqrMagnitude = vector.sqrMagnitude;
						if (!this.ExcludeTarget || sqrMagnitude > 1.401298E-45f)
						{
							float cachedRadius = current.CachedRadius;
							float num3 = (num + cachedRadius) * (num + cachedRadius);
							float num4 = (num2 + cachedRadius) * (num2 + cachedRadius);
							Vector3 vector2 = vector.normalized;
							if (this.TargetAngle > 0f)
							{
								vector2 = Quaternion.Euler(0f, this.TargetAngle, 0f) * vector2;
							}
							float num5 = Vector3.Angle(parentForward, vector2);
							bool flag = num5 <= this.DamageAngleDegrees * 0.5f;
							bool flag2 = sqrMagnitude < 1.401298E-45f || (flag && sqrMagnitude <= num4);
							bool flag3 = sqrMagnitude < 1.401298E-45f || (flag && sqrMagnitude <= num3);
							if (flag2 && GameUtilities.LineofSight(hitPosition, current.gameObject.transform.position, 1f, false, true))
							{
								if ((this.ValidTargets == TargetType.Friendly || this.ValidTargets == TargetType.FriendlyIncludingCharmed) && !base.IsValidTarget(current.gameObject, caster, AttackBase.TargetType.Hostile) && base.IsValidTarget(current.gameObject, caster))
								{
									if (confusedArentFriends) {
										Faction casterFaction = caster.GetComponent<Faction>();
										var aiController = GameUtilities.FindActiveAIController(current.gameObject);
										if (aiController == null
											|| aiController.GetOriginalTeam().GetRelationship(casterFaction.CurrentTeam) != Faction.Relationship.Hostile) {
											list.Add(current.gameObject);
										}
									} else {
										list.Add(current.gameObject);
									}
								}
								else if (
									(this.ValidTargets == AttackBase.TargetType.Hostile && !base.IsValidTarget(current.gameObject, caster)) || (!flag3 && this.ValidTargets == TargetType.All && !base.IsValidTarget(current.gameObject, caster, TargetType.Hostile))) {
									if (IsValidTarget(current.gameObject, caster, TargetType.All) && confusedArentFriends) {
										Faction casterFaction = caster.GetComponent<Faction>();
										var aiController = GameUtilities.FindActiveAIController(current.gameObject);
										if (aiController != null
											&& aiController.GetOriginalTeam().GetRelationship(casterFaction.CurrentTeam) == Faction.Relationship.Hostile) {
											list.Add(current.gameObject);
										}
									}
								}
	
								else if (this.ValidTargets == AttackBase.TargetType.All && base.IsValidTarget(current.gameObject, caster, AttackBase.TargetType.Hostile))
								{
									Faction casterFaction = caster.GetComponent<Faction>();
								
									if (disableFriendlyFire)
									{
										if (!(current.isPartyMember || casterFaction.IsFriendly(current)))
										{
											list.Add(current.gameObject);
										}
									}
									else
									{
										list.Add(current.gameObject);
									}
								}
								else if (flag3 && base.IsValidTarget(current.gameObject, caster))
								{
									if (disableFriendlyFire)
									{
										if (this.ValidTargets == TargetType.All)
										{
											Faction casterFaction = caster.GetComponent<Faction>();
										
											if (!(current.isPartyMember || casterFaction.IsFriendly(current)))
											{
												list.Add(current.gameObject);
											}
										}
										else
										{
											list.Add(current.gameObject);
										}

									}
									else
									{
										list.Add(current.gameObject);
									}
								}
							}
						}
					}
				}
			}
			int num6 = list.IndexOf(base.Owner);
			if (num6 > 0)
			{
				list[num6] = list[0];
				list[0] = base.Owner;
			}
			return list;
		}
	}

}