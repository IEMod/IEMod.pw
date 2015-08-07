using AI.Achievement;
using AI.Plan;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEMod.Mods.AI {

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
		public Mod_AITargetScanner TargetScanner {
			[NewMember]
			get {
				return (Mod_AITargetScanner) base.m_params.TargetScanner;
			}
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

			TargetScanner.DeprioritizeCurrentTarget = targetIsHardToReach;
			var foundNewTarget = TargetScanner.ScanForTarget(this.m_owner, this.m_ai, -1f, true);
			TargetScanner.DeprioritizeCurrentTarget = false;

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
						if (this.TryToPickNewTarget(improvedAI) || base.Manager.CurrentState != this)
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

}