using System;
using IEMod.Helpers;
using IEMod.Mods.UICustomization;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Tooltips {
	[ModifiesType()]
	public class mod_UIActionBarTooltip : UIActionBarTooltip {
		[NewMember]
		public static string DebugRegister;

		// this mod adjusts the tooltip for actionbar (central hud) buttons depending on their position, so as to avoid the tooltip from going off the screen when the actionbar is placed near the edge of the screen
		[ModifiesMember("GlobalShow")]
		public static void GlobalShowNew(UIWidget button, string text) {
			if (s_Instance != null) {
				s_Instance.Show(button, text);

				// ADDED all the code below

				//icon's position for which we'll be displaying the tooltip
				var hud = UICustomizer.Hud;

				var logTooltipParent = hud.Child("LogTooltipParent");
				var uiAnchor = logTooltipParent.Component<UIAnchor>();
				float posX =
					uiAnchor.widgetContainer
						.transform.position.x;

				if (posX < -1.5f) {
					uiAnchor.relativeOffset =
						new Vector2(0f, 0f);

				} else if (posX < 1.4f) {
					uiAnchor.relativeOffset =
						new Vector2(-1.3f, 0f);
					//UIOptionsManager.Instance.transform.parent.Child(6).Child (9).Component<UIAnchor> ().side = UIAnchor.Side.Center;
				} else {
					// the reason we check for text's length is because... well we'd rather check for the frame's localscale.x, but its localscale will be set on the next frame, so we can't.
					var logTooltip = logTooltipParent.Child("LogTooltip");

					//GR: logTooltip may not have a UILabel at all sometimes, in which case we just use 0.
					//This was causing various exceptions.
					var textLength =
						logTooltip.GetComponent<UILabel>()?.text.Length ?? 0;

					float firstOffset = -2.6f;

					// the tooltip is always the same size, it doesn't have a resolution-scaler, so we adjust it like this, clumsily
					if (Screen.width < 1920)
						firstOffset *= 1.5f;
					if (Screen.width < 1441)
						firstOffset *= 1.2f;

					uiAnchor.relativeOffset =
						new Vector2(firstOffset, 0f);

					float moreOffset = 0f;
					if (textLength > 11) {
						textLength -= 11;
						moreOffset = (((float) textLength) / 10f) * 2;
					}

					uiAnchor.relativeOffset +=
						new Vector2((moreOffset * -1), 0f);
				}
			}
		}
	}
}