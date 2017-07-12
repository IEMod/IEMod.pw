using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.SelectionCircles {

	//GT 27/8/15 - a lot of these members have disappeared :(
	[ModifiesType]
	public class mod_SelectionCircle : SelectionCircle {

		//[ModifiesMember("UpdateVisibility")]
		//! Verify that this is correct
		// disables spikes in the selection circles during combat if the "no engagement" mod is on
		private void UpdateVisibilityNew() {
			float alpha = 1f;
			bool flag = this.CurrentMode == Mode.Targeted;
			bool flag2 = this.CurrentMode != Mode.Targeted;
			if (this.m_Owner != null) {
				AlphaControl component = this.m_Owner.GetComponent<AlphaControl>();
				if (component != null) {
					alpha = component.Alpha;
				}
			}
			if (flag && (alpha < float.Epsilon)) {
				flag = false;
			}
			if (flag2 && (alpha < float.Epsilon)) {
				flag2 = false;
			}
			if (this.m_Pies != null) {
				foreach (MeshRenderer renderer in this.m_Pies) {
					renderer.renderer.enabled = flag;
				}
			}
			if (this.m_TargetedEngageRenderer != null) {
				this.m_TargetedEngageRenderer.enabled = this.OwnerEngaged && (flag || flag2) && !IEModOptions.DisableEngagement;
					// added && (PlayerPrefs.GetInt ("DisableEngagement", 0) == 0)
			}
			if ((this.m_Circle != null) && (this.m_Circle.renderer != null)) {
				this.m_Circle.renderer.enabled = flag2;
				if (flag2) {
					foreach (Material material in this.m_Circle.renderer.sharedMaterials) {
						if ((material != null) && material.HasProperty("_Alpha")) {
							material.SetFloat("_Alpha", alpha);
						}
					}
				}
			}

			if (this.m_EngageRenderer != null) {
				this.m_EngageRenderer.enabled = this.OwnerEngaged && flag2 && !IEModOptions.DisableEngagement;
					// added && (PlayerPrefs.GetInt ("DisableEngagement", 0) == 0)
			}
		}

		[NewMember]
		[DuplicatesBody("SetMaterial")]
		public void SetMaterialOriginal(bool isFoe, bool isSelected, bool isStealthed, bool isDominated) {
			throw new DeadEndException("SetMaterialOriginal");
		}

		/// <summary>
		/// Insertion before original method.
		/// </summary>
		/// <param name="isFoe"></param>
		/// <param name="isSelected"></param>
		/// <param name="isStealthed"></param>
		[ModifiesMember("SetMaterial")]
		public void SetMaterialNew(bool isFoe, bool isSelected, bool isStealthed, bool isDominated)
		{
			if (base.renderer == null) {
				return;
			}
			if (IEModOptions.BlueCircles && !isFoe && !isStealthed && !isDominated && !InGameHUD.Instance.UseColorBlindSettings) {
				var isPartyMember = m_Owner.HasComponent<PartyMemberAI>()
					&& m_Owner.Component<Faction>().CurrentTeam == Team.GetTeamByTag("player");
				if (!isPartyMember) {
					//colorblind material for friendlies happens to be a nice azure. The non-selected material is an ugly navy.
					//selected colorblind material for non-stealthed friendlies.
					this.m_selectedMaterial = InGameHUD.Instance.CircleMaterials.Get(true, true, false, false, false);
					//non-selected colorblind material for non-stealthed friendlies
					this.m_Circle.sharedMaterial = InGameHUD.Instance.CircleMaterials.Get(true, true, true, false, false);
					this.OnColorChanged(this.m_Circle.sharedMaterial.color);
					this.OnSharedMaterialChanged?.Invoke(this.m_Circle.sharedMaterial);
					return;
				}
				//if party member, just get the regular circle.
			}
			SetMaterialOriginal(isFoe, isSelected, isStealthed, isDominated);
		}

	}
}