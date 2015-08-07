using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.LootShuffler {
	[ModifiesType]
	public class mod_Loot : Loot
	{
		[ModifiesMember("SetSeed", ModificationScope.Body)]
		private void SetSeedNew()
		{
		
			if (IEModOptions.LootShuffler) 
				ResetSeed();
			else //default behavior... but I had to change explicit accessor calls because I was getting errors. what's up with that?
			{
				//Random.set_seed((int)(base.get_transform().get_position().x + base.get_transform().get_position().z) * GameState.s_playerCharacter.get_name().GetHashCode() + WorldTime.Instance.CurrentDay);
				Random.seed = ((int)(base.transform.position.x + base.transform.position.z) * GameState.s_playerCharacter.name.GetHashCode() + WorldTime.Instance.CurrentDay);
			}
		}
	}
}
