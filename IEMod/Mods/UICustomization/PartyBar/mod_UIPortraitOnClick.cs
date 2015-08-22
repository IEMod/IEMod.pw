using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.PartyBar {
	[ModifiesType]
	public class mod_UIPortraitOnClick : UIPortraitOnClick
	{
		[ModifiesMember("OnDrag")]
		void OnDragNew(Vector2 delta)
		{
			if (this.AllowDrag)
			{
				if (s_Dragging == null)
				{
					s_Dragging = this;
					this.m_Portrait.Grab();
				}
				if (mod_UIPartyPortrait.IsVertical) // added line
					this.m_DraggedX += delta.y * -1; // added line
				else // added line
					this.m_DraggedX += delta.x; // old line

				if (!(Mathf.Abs(this.m_DraggedX) > (this.m_Owner.PortraitWidth / 2f))) {
					return;
				}

				this.m_Owner.ShiftPortrait(base.transform.parent.gameObject, (int) Mathf.Sign(this.m_DraggedX));

				if (mod_UIPartyPortrait.IsVertical) // added line
					this.m_DraggedX -= Mathf.Sign(this.m_DraggedX) * (this.m_Owner.PortraitWidth); // added line
				else // added line
					this.m_DraggedX -= Mathf.Sign(this.m_DraggedX) * this.m_Owner.PortraitWidth; // old line
			}
		}
		[ModifiesMember("OnPress")]
		void OnPressNew(bool down)
		{
			if (this.AllowDrag)
			{
				if (down)
				{
					Vector3 v = InGameUILayout.NGUICamera.ScreenToWorldPoint(GameInput.MousePosition);
					Vector3 vector2 = base.transform.worldToLocalMatrix.MultiplyPoint3x4(v);
					if (mod_UIPartyPortrait.IsVertical) // added line
						this.m_DraggedX = vector2.y * -1; // added line
					else // added line
						this.m_DraggedX = vector2.x; // old line
					this.m_Owner.StartDrag();
				}
				else if (s_Dragging == this)
				{
					s_Dragging = null;
					this.m_Portrait.LetGo();
					this.m_Owner.EndDrag();
				}
			}
		}
	}
}