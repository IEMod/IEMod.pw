using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.NoEngagement {
	[ModifiesType]
	public class Mod_NoEngagement_HudEngagementIndicator : HudEngagementIndicator
	{
		[ModifiesMember("Update")]
		protected void UpdateNew()
		{
			if (!this.Target || !this.Source)
			{
				return;
			}
			this.LineRenderer.renderer.enabled = (GameState.Paused || GameCursor.CharacterUnderCursor == this.m_Source || GameCursor.CharacterUnderCursor == this.m_Target);
			this.LineRenderer.renderer.enabled &= InGameHUD.Instance.ShowHUD;
			this.LineRenderer.renderer.enabled &= !IEModOptions.DisableEngagement; // added this line
			if (!this.LineRenderer.renderer.enabled)
			{
				return;
			}
			Vector3 vector = this.Target.transform.position - this.Source.transform.position;
			vector.y = 0f;
			base.transform.rotation = Quaternion.FromToRotation(Vector3.forward, vector.normalized);
			float vertScale = HudEngagementManager.Instance.ArrowScaleY;
			if (vector.sqrMagnitude > HudEngagementManager.Instance.ArrowMaxRange)
			{
				vertScale = HudEngagementManager.Instance.ArrowScaleY + (1f - HudEngagementManager.Instance.ArrowScaleY) * (vector.magnitude / HudEngagementManager.Instance.ArrowMaxRange);
			}
			float num = vector.magnitude - (this.m_SourceFaction.Mover.Radius + this.m_TargetFaction.Mover.Radius) / 3f;
			this.m_xrot += HudEngagementManager.Instance.ArrowRotSpeed * TimeController.sUnscaledDelta / num;
			this.GenerateVertexData(this.m_Mesh, num, vertScale, 0.0174532924f * this.m_xrot);
			base.transform.position = (this.Source.transform.position + this.Target.transform.position) / 2f;
			base.transform.position += new Vector3(0f, HudEngagementManager.Instance.ArrowElevation, 0f);
			if (this.TwoWay)
			{
				Quaternion rotation = base.transform.rotation * Quaternion.AngleAxis(90f, Vector3.up);
				base.transform.position += rotation * (Vector3.forward * HudEngagementManager.Instance.TwoWayOffset);
			}
			this.UpdateMaterial();
		}
	}
}