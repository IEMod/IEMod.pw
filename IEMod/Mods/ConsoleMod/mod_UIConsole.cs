using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.ConsoleMod {
	[ModifiesType()]
	public  class mod_UIConsole : UIConsole
	{
		

		[ModifiesMember("AdjustBackground")]
		private void AdjustBackgroundNew(Vector2 delta)
		{
			this.m_SizeChanged = true;
			float num = this.MatchScale.GetScaleX() / InGameUILayout.Root.pixelSizeAdjustment;
			float num2 = this.MatchScale.GetScaleY() / InGameUILayout.Root.pixelSizeAdjustment;
			if (num == 0f)
			{
				delta.x = 0f;
			}
			else
			{
				delta.x /= num;
			}
			if (num2 == 0f)
			{
				delta.y = 0f;
			}
			else
			{
				delta.y /= num2;
			}
			//TODO: GR 29/9 - manually check if this block is compatible with 2.0 block
			Transform transform = this.CornerHandle.gameObject.transform;
			transform.localPosition += new Vector3(delta.x, delta.y, 0f);  //GameUtilities.V2ToV3(delta); <-- using V2ToV3 doesn't work, for some reason.. caused a bug
			//float vecX = (this.ScreenBR.transform.localPosition.x - (this.LeftMarkerPosition.x + 28f)) - this.MaxMargins.x; // original code but it wasn't exactly written like that
			float vecX = (((float)Screen.width) / num2) - (this.MaxMargins.x * 2f);
			float vecY = (((float)Screen.height) / num2) - (this.MaxMargins.y * 2f);
			Vector2 vector = new Vector2(vecX, vecY);
			float num3 = this.Background.transform.localScale.x - delta.x;
			float num4 = this.Background.transform.localScale.y + delta.y;
			num3 = Mathf.Clamp(num3, this.MinSize.x, vector.x);
			num4 = Mathf.Clamp(num4, this.MinSize.y, vector.y);
			this.Background.transform.localScale = new Vector3(num3, num4, this.Background.transform.localScale.z);
			if (this.Background.transform.localScale.y > ((vector.y - this.MinSize.y) / 2f))
			{
				this.TransitionAnchorPoint.pixelOffset = new Vector2(-42f, -42f);
			}
			else
			{
				this.TransitionAnchorPoint.pixelOffset = new Vector2(42f, 42f);
			}
			if (this.m_PanelStretch != null)
			{
				this.m_PanelStretch.Update();
			}
			if (this.m_PanelOrigin != null)
			{
				this.m_PanelOrigin.DoUpdate();
			}
			InGameHUD.Instance.m_CombatLogCurrentSize = this.Background.transform.localScale;
		}
	}
}