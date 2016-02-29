using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Targeting {
	
	[ModifiesType]
	public class mod_Trap : Trap
	{
		[NewMember]
		[DuplicatesBody(nameof(CanActivate))]
		private bool orig_CanActivate(GameObject victim) {
            
            return false;
        }

		[ModifiesMember(nameof(CanActivate))]
		public bool mod_CanActivate(GameObject victim) {
			if (!this.m_trap_initialized)
			{
				Debug.LogError("Cannot activate uninitialized trap!");
				return false;
			}
			if (IEModOptions.DisableFriendlyFire) {
				Faction victimFaction = victim?.GetComponent<Faction>();
				if (victimFaction?.isPartyMember == true && this.IsPlayerOwnedTrap)
				{
					return false;
				}
			}
			return orig_CanActivate(victim);
		}
	}
}