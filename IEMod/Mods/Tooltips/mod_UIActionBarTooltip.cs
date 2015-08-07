using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Tooltips {
	[ModifiesType()]
	public class mod_UIActionBarTooltip : UIActionBarTooltip
	{
		// this mod adjusts the tooltip for actionbar (central hud) buttons depending on their position, so as to avoid the tooltip from going off the screen when the actionbar is placed near the edge of the screen
		[ModifiesMember("GlobalShow")]
		public static void GlobalShowNew(UIWidget button, string text)
		{
			if (s_Instance != null)
			{
				s_Instance.Show(button, text);

				// ADDED all the code below

				//icon's position for which we'll be displaying the tooltip
				float posX = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild (9).GetComponent<UIAnchor> ().widgetContainer.transform.position.x;

				if (posX < -1.5f)
				{
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild (9).GetComponent<UIAnchor> ().relativeOffset = new Vector2 (0f, 0f);

				} else if (posX < 1.4f)
				{
					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild (9).GetComponent<UIAnchor> ().relativeOffset = new Vector2 (-1.3f, 0f);
					//UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild (9).GetComponent<UIAnchor> ().side = UIAnchor.Side.Center;
				} else
				{
					// the reason we check for text's length is because... well we'd rather check for the frame's localscale.x, but its localscale will be set on the next frame, so we can't.
					int textLength = UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild (9).GetChild (0).GetComponent<UILabel> ().text.Length;

					float firstOffset = -2.6f;

					// the tooltip is always the same size, it doesn't have a resolution-scaler, so we adjust it like this, clumsily
					if (Screen.width < 1920)
						firstOffset *= 1.5f;
					if (Screen.width < 1441)
						firstOffset *= 1.2f;

					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild (9).GetComponent<UIAnchor> ().relativeOffset = new Vector2 (firstOffset, 0f);

					float moreOffset = 0f;
					if (textLength > 11)
					{
						textLength -= 11;
						moreOffset = (((float)textLength) / 10f) * 2;
					}

					UIOptionsManager.Instance.transform.parent.GetChild(6).GetChild (9).GetComponent<UIAnchor> ().relativeOffset += new Vector2 ((moreOffset * -1), 0f);
				}
			}
		}
	}
}