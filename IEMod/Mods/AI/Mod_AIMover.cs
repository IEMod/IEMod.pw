using System;
using System.Collections.Generic;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.AI {
	[ModifiesType]
	public class Mod_AIMover : Mover
	{
		[ModifiesAccessibility]
		public new Mover m_pivotObstacle;

		[ModifiesAccessibility]
		public new bool m_hasSteeredAroundPivotObstacle;

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
					if (this.AIController.StateManager.CurrentState is global::AI.Player.WaitForClearPath)
					{
						return;
					}
					global::AI.Player.WaitForClearPath waitForClearPath = AIStateManager.StatePool.Allocate<global::AI.Player.WaitForClearPath>();
					waitForClearPath.Obstacles.AddRange(obstacles);
					waitForClearPath.BlockerDistance = 0.5f;
					this.AIController.StateManager.PushState(waitForClearPath, false);
				}
				else
				{
					if (this.AIController.StateManager.CurrentState is global::AI.Plan.WaitForClearPath)
					{
						return;
					}
					if (IEModOptions.ImprovedAI) // added this if statement
					{
						// race condition if both Mover and PathFindingManager decide they are blocked in the same tick
						if (this.AIController.StateManager.CurrentState is global::AI.Plan.ApproachTarget)
						{
							return;
						}
						var pathToPosition = this.AIController.StateManager.CurrentState as global::AI.Achievement.PathToPosition;
						if (pathToPosition != null)
						{
							if (((Mod_AI_PathToPosition)pathToPosition).TryToPickNewTarget(true))
							{
								return;
							}
						}
					}
					global::AI.Plan.WaitForClearPath waitForClearPath2 = AIStateManager.StatePool.Allocate<global::AI.Plan.WaitForClearPath>();
					waitForClearPath2.Obstacles.AddRange(obstacles);
					waitForClearPath2.BlockerDistance = 0.5f;
					this.AIController.StateManager.PushState(waitForClearPath2, false);
				}
			}
		}
	}
}