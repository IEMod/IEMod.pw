using System;
using System.Collections.Generic;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEMod.Mods.AI {
	[ModifiesType]
	public class Mod_AITargetScanner : TargetScanner
	{
		[NewMember]
		public bool DeprioritizeCurrentTarget;

		[ModifiesMember("ScanForTargetToAttack")]
		protected GameObject ScanForTargetToAttackNew(List<GameObject> potentialTargets, GameObject owner, AIController aiController)
		{
			if (IEModOptions.ImprovedAI) // added this if statement
			{
				return ScanForTargetToAttackImproved(potentialTargets, owner, aiController);
			}

			float num = 3.40282347E+38f;
			GameObject result = null;
			Faction component = owner.GetComponent<Faction>();
			if (component == null)
			{
				Debug.LogError(owner.name + " doesn't have a faction.", owner);
				return null;
			}
			float num2 = aiController.PerceptionDistance;
			if (aiController.InCombat)
			{
				num2 += 6f;
			}
			for (int i = 0; i < potentialTargets.Count; i++)
			{
				GameObject gameObject = potentialTargets[i];
				if (component.IsHostile(gameObject))
				{
					Health component2 = gameObject.GetComponent<Health>();
					if (!(component2 == null) && component2.Targetable)
					{
						AIController component3 = gameObject.GetComponent<AIController>();
						if (!(component3 == null))
						{
							CharacterStats component4 = gameObject.GetComponent<CharacterStats>();
							if (!(component4 == null))
							{
								float num3 = GameUtilities.V3Distance2D(owner.transform.position, gameObject.transform.position);
								float num4 = num2;
								if (component4.NoiseLevelRadius > num3)
								{
									result = gameObject;
									GameState.InStealthMode = false;
								}
								if (num3 <= num4)
								{
									if (GameUtilities.LineofSight(owner.transform.position, component3.transform.position, 1f, false))
									{
										float num5 = 0f;
										num5 += num3 / num4 * 2f;
										num5 += component2.CurrentStamina / component2.MaxStamina * 0.5f;
										num5 += ((!aiController.HasEngaged(gameObject)) ? 0f : -3f);
										if (num5 < num && !GameState.InStealthMode)
										{
											num = num5;
											result = gameObject;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		[NewMember]
		private GameObject ScanForTargetToAttackImproved(List<GameObject> potentialTargets, GameObject owner, AIController aiController)
		{
			Faction ownerFaction = owner.GetComponent<Faction>();
			if (ownerFaction == null)
			{
				Debug.LogError(owner.name + " doesn't have a faction.", owner);
				return null;
			}
			float perceptionDistance = aiController.PerceptionDistance;
			if (aiController.InCombat)
			{
				perceptionDistance *= 2; // was += 6f;
			}

			var targets = new List<GameObject>(potentialTargets.Count);
			var targetDistances = new List<float>(potentialTargets.Count);
			var targetStaminas = new List<float>(potentialTargets.Count);
			var targetIsEngageds = new List<bool>(potentialTargets.Count);
			var maxStamina = float.MinValue;
			for (int i = 0; i < potentialTargets.Count; i++)
			{
				GameObject potentialTarget = potentialTargets[i];
				if (ownerFaction.IsHostile(potentialTarget))
				{
					Health targetHealth = potentialTarget.GetComponent<Health>();
					if (!(targetHealth == null) && targetHealth.Targetable)
					{
						AIController targetAI = potentialTarget.GetComponent<AIController>();
						if (!(targetAI == null))
						{
							CharacterStats targetStats = potentialTarget.GetComponent<CharacterStats>();
							if (!(targetStats == null))
							{
								float targetDistance = GameUtilities.V3Distance2D(owner.transform.position, potentialTarget.transform.position);
								bool isValid = false;
								if (targetStats.NoiseLevelRadius > targetDistance)
								{
									isValid = true;
									GameState.InStealthMode = false;
								}
								else if (targetDistance <= perceptionDistance && GameUtilities.LineofSight(owner.transform.position, targetAI.transform.position, 1f, false))
								{
									isValid = true;
								}

								if (isValid)
								{
									targets.Add(potentialTarget);
									targetDistances.Add(targetDistance);
									targetStaminas.Add(targetHealth.CurrentStamina);
									targetIsEngageds.Add(aiController.HasEngaged(potentialTarget));
									maxStamina = Math.Max(targetHealth.CurrentStamina, maxStamina);
								}
							}
						}
					}
				}
			}

			if (GameState.InStealthMode)
			{
				return null;
			}

			const float DISTANCE_WEIGHT = 1f;
			const float STAMINA_WEIGHT = 1f;
			const float FICKLENESS_WEIGHT = 0.5f;
			const float ENGAGED_WEIGHT = -10f; // must be more than the other weights combined
			const float CURRENT_TARGET_WEIGHT = 2f;
			var bestWeight = float.MaxValue;
			GameObject result = null;

			for (int i = 0; i < targets.Count; ++i)
			{
				var distanceValue = (targetDistances[i] / perceptionDistance) * DISTANCE_WEIGHT;
				var staminaValue = (targetStaminas[i] / maxStamina) * STAMINA_WEIGHT; // targets with less stamina are more attractive
				var isCurrentTarget = ((Mod_AITargetScanner)(object)this).DeprioritizeCurrentTarget && (targets[i] == aiController.CurrentTarget);
				var currentTargetValue = isCurrentTarget ? CURRENT_TARGET_WEIGHT : 0;
				var engagedValue = (targetIsEngageds[i] ? 1 : 0) * ENGAGED_WEIGHT;
				var fickleValue = Random.Range(0f, 1f) * FICKLENESS_WEIGHT;  // a little randomness is useful

				var value = distanceValue + staminaValue + currentTargetValue + engagedValue + fickleValue;
				if (value < bestWeight)
				{
					bestWeight = value;
					result = targets[i];
				}
			}

			return result;
		}
	}
}