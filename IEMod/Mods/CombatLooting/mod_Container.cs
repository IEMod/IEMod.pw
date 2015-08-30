using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;

namespace IEMod.Mods.CombatLooting {
	[ModifiesType]
	public class Mod_CombatLooting_Container : Container
	{
		/// <summary>
		/// base.IsUsable
		/// </summary>
		/// <returns></returns>
		[MemberAlias("get_IsUsable", typeof(OCL), AliasCallMode.NonVirtual)]
		private bool base_get_IsUsable() {
			throw new DeadEndException(nameof(Mod_CombatLooting_Container.base_get_IsUsable));
			//this will be translated to base.get_IsUsable
		}

		public bool IsUsableNew
		{
			[ModifiesMember("get_IsUsable")]
			get {
				//TODO: GR 29/8 - manually check if this code is valid
				//!+ ADDED CODE
				if (IEModOptions.UnlockCombatInv) {
					if (GameState.InCombat
						&& (this.gameObject.name.Contains("DefaultDropItem") || this.gameObject.GetComponent<CharacterStats>() != null))
						// this is a check for ground loot or body loot
					{
						return false;
					}
					return !this.IsEmptyDeadBody() && base_get_IsUsable();
				}
				//!+ END ADD
				return !this.IsEmptyDeadBody() && !GameState.InCombat && base_get_IsUsable();
				
			}
		}
	}

}