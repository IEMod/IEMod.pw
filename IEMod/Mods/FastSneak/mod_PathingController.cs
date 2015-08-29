using IEMod.Mods.NoEngagement;
using IEMod.Mods.Options;
using Patchwork.Attributes;

namespace IEMod.Mods.FastSneak
{
	//TODO: GR 29/8 - if it's still correct, it will have to be rewritten.
	/*
	
	[ModifiesType]
	public class mod_PathingController : PathingController
	{
		[ModifiesMember("UpdateStealth")]
		public void UpdateStealthNew()
		{
			if (IEModOptions.FastSneak || (!GameState.InCombat && Mod_NoEngagement_Player.WalkMode)) // modified
			{
				if (this.m_currentMovementType != AnimationController.MovementType.Walk)
				{
					this.m_currentMovementType = AnimationController.MovementType.Walk;
					this.m_mover.UseWalkSpeed();
				}
			}
			else if (this.m_params.MovementType != AnimationController.MovementType.Walk
					&& (this.m_currentMovementType != AnimationController.MovementType.Run || IEModOptions.FastSneak)) //this.m_currentMovementType may be wrong with mod enabled, so ignore it here.
			{
				this.m_currentMovementType = AnimationController.MovementType.Run;
				this.m_mover.UseRunSpeed();
			}
		}
	}
	*/

}