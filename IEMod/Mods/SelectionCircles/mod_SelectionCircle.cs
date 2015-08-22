using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.SelectionCircles {
	[ModifiesType]
	public class mod_SelectionCircle : SelectionCircle
	{
		[ModifiesMember("UpdateVisibility")]
		// disables spikes in the selection circles during combat if the "no engagement" mod is on
		private void UpdateVisibilityNew()
		{
			float alpha = 1f;
			bool flag = this.m_Mode == Mode.TARGETED;
			bool flag2 = this.m_Mode != Mode.TARGETED;
			if (this.m_Owner != null)
			{
				AlphaControl component = this.m_Owner.GetComponent<AlphaControl>();
				if (component != null)
				{
					alpha = component.Alpha;
				}
			}
			if (flag && (alpha < float.Epsilon))
			{
				flag = false;
			}
			if (flag2 && (alpha < float.Epsilon))
			{
				flag2 = false;
			}
			if (this.m_Pies != null)
			{
				foreach (MeshRenderer renderer in this.m_Pies)
				{
					renderer.renderer.enabled = flag;
				}
			}
			if (this.m_TargetedEngageRenderer != null)
			{
				this.m_TargetedEngageRenderer.enabled = this.OwnerEngaged && (flag || flag2) && !IEModOptions.DisableEngagement; // added && (PlayerPrefs.GetInt ("DisableEngagement", 0) == 0)
			}
			if ((this.m_Circle != null) && (this.m_Circle.renderer != null))
			{
				this.m_Circle.renderer.enabled = flag2;
				if (flag2)
				{
					foreach (Material material in this.m_Circle.renderer.sharedMaterials)
					{
						if ((material != null) && material.HasProperty("_Alpha"))
						{
							material.SetFloat("_Alpha", alpha);
						}
					}
				}
			}
			if (this.m_EngageRenderer != null)
			{
				this.m_EngageRenderer.enabled = this.OwnerEngaged && flag2 && !IEModOptions.DisableEngagement; // added && (PlayerPrefs.GetInt ("DisableEngagement", 0) == 0)
			}
		}

		[ModifiesMember("SetColor")]
		public void SetColorNew(bool isFoe, bool isSelected)
		{
			if (base.renderer != null)
			{
				if (isFoe)
				{
					this.m_selectedMaterial = this.m_FoeMaterial;
					base.renderer.sharedMaterial = this.m_FoeMaterial;
					this.m_ReferenceMaterial = InGameHUD.Instance.FoeMaterial;
				} else if (InGameHUD.Instance.UseColorBlindSettings)
				{
					this.m_selectedMaterial = this.m_FriendlySelectedColorBlindMaterial;
					if (isSelected)
					{
						base.renderer.sharedMaterial = this.m_FriendlySelectedColorBlindMaterial;
						this.m_ReferenceMaterial = InGameHUD.Instance.FriendlySelectedColorBlind;
					} else
					{
						base.renderer.sharedMaterial = this.m_FriendlyColorBlindMaterial;
						this.m_ReferenceMaterial = InGameHUD.Instance.FriendlyColorBlind;
					}
				} else
				{
					/////////////// changed this code
					if (IEModOptions.BlueCircles)
					{
						if ((m_Owner.GetComponent<PartyMemberAI> () != null) && (m_Owner.GetComponent<Faction> ().CurrentTeam == Team.GetTeamByTag ("player"))) // if party member (additional check for pets...)
						{

							this.m_selectedMaterial = this.m_FriendlySelectedMaterial; // green circle
							if (isSelected)
							{
								base.renderer.sharedMaterial = this.m_FriendlySelectedMaterial;
								this.m_ReferenceMaterial = InGameHUD.Instance.FriendlySelected;
							} else
							{
								base.renderer.sharedMaterial = this.m_FriendlyMaterial;
								this.m_ReferenceMaterial = InGameHUD.Instance.Friendly;
							}
						} else
						{
							m_selectedMaterial = m_FriendlySelectedColorBlindMaterial; // if neutral npc, blue circle
							if (isSelected)
							{
								base.renderer.sharedMaterial = m_FriendlySelectedColorBlindMaterial;
								m_ReferenceMaterial = InGameHUD.Instance.FriendlySelectedColorBlind;
							} else
							{
								base.renderer.sharedMaterial = m_FriendlySelectedColorBlindMaterial; // there's no such thing as a selected neutral npc, so we use the bright blue in both cases
								m_ReferenceMaterial = InGameHUD.Instance.FriendlySelectedColorBlind;
							}
						}
					} else
					{
						this.m_selectedMaterial = this.m_FriendlySelectedMaterial;
						if (isSelected)
						{
							base.renderer.sharedMaterial = this.m_FriendlySelectedMaterial;
							this.m_ReferenceMaterial = InGameHUD.Instance.FriendlySelected;
						} else
						{
							base.renderer.sharedMaterial = this.m_FriendlyMaterial;
							this.m_ReferenceMaterial = InGameHUD.Instance.Friendly;
						}
					}
					/////////// end of changes
				}
				if (this.m_ReferenceMaterial != null)
				{
					if (base.renderer.sharedMaterial != null)
					{
						base.renderer.sharedMaterial.color = this.m_ReferenceMaterial.color;
					}
					this.ColorTween.from = this.m_ReferenceMaterial.color;
				}
				if (this.m_EngageRenderer != null)
				{
					this.m_EngageRenderer.sharedMaterial = base.renderer.sharedMaterial;
				}
				if (this.m_TargetedEngageRenderer != null)
				{
					this.m_TargetedEngageRenderer.sharedMaterial = base.renderer.sharedMaterial;
				}
				if (this.OnSharedMaterialChanged != null)
				{
					this.OnSharedMaterialChanged(base.renderer.sharedMaterial);
				}
			}
		}
	}
}