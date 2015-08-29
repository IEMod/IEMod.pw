using IEMod.Mods.Options;
using Patchwork.Attributes;

namespace IEMod.Mods.CombatLooting {
	[ModifiesType]
	public class Mod_CombatLooting_Container : Container
	{
		public bool IsUsableNew
		{
			[ModifiesMember("get_IsUsable")]
			get
			{
				//TODO: GR 29/8 - manually check if this code is valid
				if (IEModOptions.UnlockCombatInv)
				{
					if (GameState.InCombat && (this.gameObject.name.Contains ("DefaultDropItem") || this.gameObject.GetComponent<CharacterStats> () != null)) // this is a check for ground loot or body loot
					{
						return false;
					} else
					{
						return (((base.m_currentState != State.Sealed) && (base.m_currentState != State.SealedOpen)) && base.IsVisible);
					}
				} else
				{
					if (this.IsEmptyDeadBody())
					{
						return false;
					}
					return (!GameState.InCombat && ((base.m_currentState != State.Sealed) && (base.m_currentState != State.SealedOpen)) && base.IsVisible); // this code is a combination of OCL.IsUsable and Container.IsUsable to avoid the .base problem
				}
			}
		}
	}

}