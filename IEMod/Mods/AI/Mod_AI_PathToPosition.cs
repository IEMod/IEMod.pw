using System;
using System.Collections.Generic;
using AI;
using AI.Achievement;
using AI.Plan;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEMod.Mods.AIMod {

		[ModifiesType]
	public class Mod_AI_PreMover : Mover {
		[ModifiesAccessibility]
		public new Mover m_pivotObstacle;

		[ModifiesAccessibility]
		public new bool m_hasSteeredAroundPivotObstacle;

	}

	[ModifiesType("TargetScanner")]
	public class Mod_AI_PreTargetScanner : TargetScanner
	{
		[NewMember]
		public bool DeprioritizeCurrentTarget;
	}



	[ModifiesType]
	public class Mod_AI_PathToPosition : PathToPosition
	{
		[NewMember]
		[DuplicatesBody(methodName:"Update", sourceType:typeof(Mod_AIGameAIState))]
		private void GameAIStateUpdate()
		{
			// will be filled with Mod_AIGameAIState.Update() IL generated below
		}

		[NewMember]
		public bool TargetIsHardToReach()
		{
			var mover = this.m_pathingController.Mover;
			return (mover != null && mover.m_pivotObstacle != null && !mover.m_hasSteeredAroundPivotObstacle);
		}

		[NewMember]
		public bool TryToPickNewTarget(bool targetIsHardToReach)
		{
			AIState currentState = base.Manager.CurrentState;

			AIState aIState = base.Manager.FindState(typeof(ApproachTarget));
			if (targetIsHardToReach)
			{
				this.m_ai.CancelAllEngagements();
			}

			((Mod_AI_PreTargetScanner)this.m_params.TargetScanner).DeprioritizeCurrentTarget = targetIsHardToReach;
			var foundNewTarget = this.m_params.TargetScanner.ScanForTarget(this.m_owner, this.m_ai, -1f, true);
			((Mod_AI_PreTargetScanner)this.m_params.TargetScanner).DeprioritizeCurrentTarget = false;

			if (foundNewTarget)
			{
				base.Manager.PopState(currentState);
				if (aIState != null)
				{
					base.Manager.PopState(aIState);
				}
			}

			return foundNewTarget;
		}

		[ModifiesMember("Update")]
		public void UpdateNew()
		{
			GameAIStateUpdate(); // was base.Update()
			if (this.m_params.Target == null && this.m_params.Destination.sqrMagnitude < 1.401298E-45f)
			{
				base.Manager.PopCurrentState();
				return;
			}
			if (this.m_ai.BeingKited())
			{
				this.m_ai.StateManager.PopCurrentState();
				this.m_ai.StopKiting();
				return;
			}
			if (this.m_params.TargetScanner != null)
			{
				if (this.m_params.Target != null)
				{
					var improvedAI = IEModOptions.ImprovedAI; // added
					var targetIsHardToReach = improvedAI && TargetIsHardToReach();
					this.m_scanTimer -= targetIsHardToReach ? (6 * Time.deltaTime) : Time.deltaTime; // changed
					if (this.m_scanTimer <= 0f)
					{
						this.m_scanTimer = Random.Range(AttackData.Instance.MinTargetReevaluationTime, AttackData.Instance.MaxTargetReevaluationTime);
						if (this.TryToPickNewTarget(improvedAI))
						{
							return;
						}

						if (base.Manager.CurrentState != this)
						{
							return;
						}
					}
				}
				else
				{
					AIState currentState = base.Manager.CurrentState;
					if (this.m_params.TargetScanner.ScanForTarget(this.m_owner, this.m_ai, -1f, true))
					{
						base.Manager.PopState(currentState);
						return;
					}
				}
			}
			this.m_hasBeenUpdated = true;
			if (this.m_frozen)
			{
				if (!this.m_ai.Mover.Frozen)
				{
					this.m_pathingController.Init(this, this.m_params, true);
					this.m_frozen = false;
				}
			}
			else
			{
				this.m_frozen = this.m_ai.Mover.Frozen;
			}
			this.m_pathingController.Update();
			if (this.m_ai is PartyMemberAI)
			{
				this.m_pathingController.UpdateStealth();
			}
			this.m_ai.UpdateEngagement(base.Owner, AIController.GetPrimaryAttack(base.Owner));
			if (this.m_pathingController.ReachedDestination())
			{
				base.Manager.PopCurrentState();
			}
			this.m_pathingController.UpdatePreviousPosition();
		}
	}

// Used to generate IL to call GameAIState.Update() without virtual dispatch
// (i.e. the equivalent of base.Update() from a subclass's overridden Update() method)
	public class Mod_AIGameAIState : GameAIState
	{
		public override void Update()
		{
			base.Update();
		}
	}

	[ModifiesType]
	public class Mod_AITargetScanner : TargetScanner
	{
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
				var isCurrentTarget = ((Mod_AI_PreTargetScanner)(object)this).DeprioritizeCurrentTarget && (targets[i] == aiController.CurrentTarget);
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

	[ModifiesType]
	public class Mod_AIMover : Mover
	{
		[ModifiesMember("GetObstaclesToAvoid")]
		private List<Mover> GetObstaclesToAvoidNew(float range)
		{
			Mover.s_potentialObstacles.Clear();
			float num = range * range;
			GameObject currentTarget = this.m_aiController.CurrentTarget;
			Mover y = null;
			if (currentTarget != null)
			{
				y = currentTarget.GetComponent<Mover>();
				if (IEModOptions.ImprovedAI) // added this if statement
				{
					// reset range to the distance between us and our target
					num = Math.Min(num, GameUtilities.V3Subtract2D(y.transform.position, base.transform.position).sqrMagnitude);
				}
			}
			for (int i = 0; i < Mover.s_moverList.Count; i++)
			{
				Mover mover = Mover.s_moverList[i];
				if (!(mover == this) && !(mover == y) && mover.gameObject.activeInHierarchy)
				{
					if (mover.IsPathingObstacle())
					{
						if (!mover.IsMoving() || Vector2.Dot(this.m_desiredHeading, mover.m_desiredHeading) <= 0f || (this.m_desiredSpeed >= this.GetRunSpeed() && mover.m_desiredSpeed < mover.GetRunSpeed()))
						{
							float sqrMagnitude = GameUtilities.V3Subtract2D(mover.transform.position, base.transform.position).sqrMagnitude;
							if (sqrMagnitude <= num)
							{
								mover.m_obstacleCluster = null;
								Mover.s_potentialObstacles.Add(mover);
							}
						}
					}
				}
			}
			return Mover.s_potentialObstacles;
		}
		[ModifiesMember("PushPathBlockedState")]
		private void PushPathBlockedStateNew(List<Mover> obstacles, bool forceBlock)
		{
			if (this.AIController != null && !Cutscene.CutsceneActive)
			{
				if (!forceBlock && !this.AIController.StateManager.CurrentState.AllowBlockedMovement())
				{
					return;
				}
				this.SaveBlockedRoute();
				this.m_desiredHeading = GameUtilities.V3Subtract2D(this.m_nextCornerPos, base.transform.position);
				this.m_desiredHeading.Normalize();
				this.m_heading = this.m_desiredHeading;
				if (this.AIController is PartyMemberAI)
				{
					if (this.AIController.StateManager.CurrentState is AI.Player.WaitForClearPath)
					{
						return;
					}
					AI.Player.WaitForClearPath waitForClearPath = AIStateManager.StatePool.Allocate<AI.Player.WaitForClearPath>();
					waitForClearPath.Obstacles.AddRange(obstacles);
					waitForClearPath.BlockerDistance = 0.5f;
					this.AIController.StateManager.PushState(waitForClearPath, false);
				}
				else
				{
					if (this.AIController.StateManager.CurrentState is AI.Plan.WaitForClearPath)
					{
						return;
					}
					if (IEModOptions.ImprovedAI) // added this if statement
					{
						// race condition if both Mover and PathFindingManager decide they are blocked in the same tick
						if (this.AIController.StateManager.CurrentState is AI.Plan.ApproachTarget)
						{
							return;
						}
						var pathToPosition = this.AIController.StateManager.CurrentState as AI.Achievement.PathToPosition;
						if (pathToPosition != null)
						{
							if (((Mod_AI_PathToPosition)pathToPosition).TryToPickNewTarget(true))
							{
								return;
							}
						}
					}
					AI.Plan.WaitForClearPath waitForClearPath2 = AIStateManager.StatePool.Allocate<AI.Plan.WaitForClearPath>();
					waitForClearPath2.Obstacles.AddRange(obstacles);
					waitForClearPath2.BlockerDistance = 0.5f;
					this.AIController.StateManager.PushState(waitForClearPath2, false);
				}
			}
		}
	}

	[ModifiesType]
	class Mod_AIPathFindingManager : PathFindingManager
	{
		[ModifiesMember("PushPathBlockedState")]
		private void PushPathBlockedStateNew(Mover mover, Mover blocker, float blockerDistance, List<Mover> obstacles)
		{
			if (mover.AIController != null && !Cutscene.CutsceneActive)
			{
				if (!mover.AIController.StateManager.CurrentState.AllowBlockedMovement())
				{
					return;
				}
				mover.SaveBlockedRoute();
				if (mover.AIController is PartyMemberAI)
				{
					if (mover.AIController.StateManager.CurrentState is AI.Player.WaitForClearPath)
					{
						return;
					}
					AI.Player.WaitForClearPath waitForClearPath = AIStateManager.StatePool.Allocate<AI .Player.WaitForClearPath>();
					waitForClearPath.Blocker = blocker;
					waitForClearPath.BlockerDistance = blockerDistance;
					mover.AIController.StateManager.PushState(waitForClearPath, false);
					if (obstacles != null && obstacles.Count > 0)
					{
						waitForClearPath.Obstacles.AddRange(obstacles);
					}
				}
				else
				{
					if (mover.AIController.StateManager.CurrentState is AI.Plan.WaitForClearPath)
					{
						return;
					}
					if (IEModOptions.ImprovedAI) // added this if statement
					{
						// race condition if both Mover and PathFindingManager decide they are blocked in the same tick
						if (mover.AIController.StateManager.CurrentState is AI.Plan.ApproachTarget)
						{
							return;
						}
						var pathToPosition = mover.AIController.StateManager.CurrentState as AI.Achievement.PathToPosition;
						if (pathToPosition != null)
						{
							if (((Mod_AI_PathToPosition)pathToPosition).TryToPickNewTarget(true))
							{
								return;
							}
						}
					}
					AI.Plan.WaitForClearPath waitForClearPath2 = AIStateManager.StatePool.Allocate<AI.Plan.WaitForClearPath>();
					waitForClearPath2.Blocker = blocker;
					waitForClearPath2.BlockerDistance = blockerDistance;
					mover.AIController.StateManager.PushState(waitForClearPath2, false);
					if (obstacles != null && obstacles.Count > 0)
					{
						waitForClearPath2.Obstacles.AddRange(obstacles);
					}
				}
			}
		}
	}


// Used to generate IL to call GameAIState.Update() without virtual dispatch
// (i.e. the equivalent of base.Update() from a subclass's overridden Update() method)

}