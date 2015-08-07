using System.Collections.Generic;
using IEMod.Mods.Options;
using Patchwork.Attributes;

namespace IEMod.Mods.AI {
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
					if (mover.AIController.StateManager.CurrentState is global::AI.Player.WaitForClearPath)
					{
						return;
					}
					global::AI.Player.WaitForClearPath waitForClearPath = AIStateManager.StatePool.Allocate<global::AI.Player.WaitForClearPath>();
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
					if (mover.AIController.StateManager.CurrentState is global::AI.Plan.WaitForClearPath)
					{
						return;
					}
					if (IEModOptions.ImprovedAI) // added this if statement
					{
						// race condition if both Mover and PathFindingManager decide they are blocked in the same tick
						if (mover.AIController.StateManager.CurrentState is global::AI.Plan.ApproachTarget)
						{
							return;
						}
						var pathToPosition = mover.AIController.StateManager.CurrentState as global::AI.Achievement.PathToPosition;
						if (pathToPosition != null)
						{
							if (((Mod_AI_PathToPosition)pathToPosition).TryToPickNewTarget(true))
							{
								return;
							}
						}
					}
					global::AI.Plan.WaitForClearPath waitForClearPath2 = AIStateManager.StatePool.Allocate<global::AI.Plan.WaitForClearPath>();
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
}