using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.SelectionCircles {
	[ModifiesType]
	public class mod_InGameHUD : InGameHUD
	{
		[ModifiesMember("FetchColors")]
		public void FetchColorsNew()
		{
			if (IEModOptions.BlueCirclesBG)
				this.FriendlySelectedColorBlind.color = new Color (0.25f, 1f, 1f, 1f); // BG's cyan (64, 255, 255) selection circles for neutral mobs... for hardcore nostalgia fags...

			this.FriendlyColor = this.FriendlySelected.color;
			this.FoeColor = this.FoeMaterial.color;
			this.FriendlyColorColorBlind = this.FriendlySelectedColorBlind.color;
		}
	}

}