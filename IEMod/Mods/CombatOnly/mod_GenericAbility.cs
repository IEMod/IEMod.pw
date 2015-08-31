using AI.Player;
using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;


namespace IEMod.Mods.CombatOnly {
	[ModifiesType()]
	public class mod_GenericAbility : GenericAbility {
		[NewMember]
		private bool? originalCombatOnly;

		[NewMember]
		[DuplicatesBody("Update")]
		private void orig_Update() {
			
		}


		[ModifiesMember("Update")]
		protected virtual void mod_Update() {
			if (originalCombatOnly == null) {
				originalCombatOnly = CombatOnly;
			}
			CombatOnly = !IEModOptions.CombatOnlyMod && originalCombatOnly.Value;
			orig_Update();
		}

	}

}


