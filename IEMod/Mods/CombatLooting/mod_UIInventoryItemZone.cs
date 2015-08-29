using IEMod.Mods.Options;
using Patchwork.Attributes;

namespace IEMod.Mods.CombatLooting {
	[ModifiesType]
	public abstract class mod_UIInventoryItemZone : UIInventoryItemZone
	{
		[ModifiesMember("GetCurrentAccessLevel")]
		public static AccessLevel GetCurrentAccessLevelNew()
		{
			if (GameState.InCombat)
			{
				if (!IEModOptions.UnlockCombatInv) // if the "unlock inv" isn't activated
				{
					return AccessLevel.InCombat; 
				}
			}
			/*
			pre-2.0:
			if ((!RestZone.PartyInRestZone && !UIInventoryManager.Instance.StashAccess) && (((GameState.Instance.CurrentMap != null) && GameState.Instance.CurrentMap.CanCamp) && !GameState.Option.GetOption(GameOption.BoolOption.DONT_RESTRICT_STASH)))
			{
				return AccessLevel.InField;
			}
			return AccessLevel.Rest;
			*/
			if (RestZone.PartyInRestZone || UIInventoryManager.Instance.StashAccess || GameState.Instance.CurrentMap == null || GameState.Instance.CurrentMap.CanAccessStash || GameState.Option.GetOption(GameOption.BoolOption.DONT_RESTRICT_STASH))
			{
				return AccessLevel.Rest;
			}
			return AccessLevel.InField;
		}
	}
}
