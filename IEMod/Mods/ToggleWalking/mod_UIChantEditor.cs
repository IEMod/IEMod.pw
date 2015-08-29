using Patchwork.Attributes;

namespace IEMod.Mods.ToggleWalking {
	[ModifiesType]
	public class mod_UIChantEditor : UIChantEditor
	{
		[ModifiesMember("Awake")]
		private void AwakeNew()
		{
			UIChantEditor.Instance = this;
			this.ToggleKey = MappedControl.NONE; // prevent it from opening when Deprecated_CHANT_EDITOR is pressed.
		}
	}
}