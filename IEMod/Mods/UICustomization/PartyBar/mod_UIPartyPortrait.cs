using System.Collections.Generic;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.PartyBar {
	[ModifiesType]
	public class mod_UIPartyPortrait : UIPartyPortrait
	{
		[NewMember]
		public bool Dragging;

		[MemberAlias(".ctor", typeof(MonoBehaviour))]
		private void MonoBehavior_ctor() {
			
		}

		[ModifiesMember(".ctor")]
		public void CtorNew() {
			MonoBehavior_ctor();
			this.PulsePeriodSeconds = 0.5f;
			this.PulseMinAlpha = 0.5f;
			this.PulseMaxAlpha = 0.7f;
			this.m_ClassCount = -1;

			this.Dragging = false; // added
		}

		[NewMember]
		public static bool IsVertical;

		[ModifiesMember("Update")]
		private void UpdateNew()
		{
			if (this.m_partyMemberAI == null)
			{
				return;
			}
			if (this.m_partyMemberAI.Selected)
			{
				//GT 27/8/15 - Glow member has been removed :(
				//this.Glow.alpha = 1f;
				this.Border.spriteName = "portSelected";
			}
			else
			{
				//GT 27/8/15 - Glow member has been removed :(
				//this.Glow.alpha = 0f;
				this.Border.spriteName = "portSelectedNot";
			}
			this.m_EnduranceValues.LineBreak = this.Minion.gameObject.activeSelf;
			this.UpdateLevelTalkGrid();
			if (this.m_ClassCounterType == UIPartyPortrait.ClassCounterType.Focus)
			{
				this.ClassCount = Mathf.FloorToInt(this.m_characterStats.Focus);
			}
			else
			{
				if (this.m_ClassCounterType == UIPartyPortrait.ClassCounterType.Phrases)
				{
					if (!this.m_characterChanter)
					{
						this.m_characterChanter = this.m_characterStats.GetChanterTrait();
					}
					if (this.m_characterChanter)
					{
						this.ClassCount = this.m_characterChanter.PhraseCount;
					}
					else
					{
						Debug.LogError("PartyBar thinks '" + this.m_characterStats.name + "' is a Chanter, but no chanter trait found.");
					}
				}
			}
			if (this.m_CipherFocusVfx)
			{
				this.m_CipherFocusVfx.transform.localPosition = new Vector3(this.m_CipherFocusVfx.transform.localPosition.x, this.m_CipherFocusVfx.transform.localPosition.y);
			}
			float num = this.PulseMinAlpha + (this.PulseMaxAlpha - this.PulseMinAlpha) * Mathf.Sin(3.14159274f * TimeController.sUnscaledDelta / this.PulsePeriodSeconds);
			if (this.m_StaminaPulseSprite && this.m_HealthPulseSprite)
			{
				UIWidget arg_1DD_0 = this.m_StaminaPulseSprite;
				float alpha = num;
				this.m_HealthPulseSprite.alpha = alpha;
				arg_1DD_0.alpha = alpha;
			}
			int num2 = 0;
			IList<StatusEffect> activeStatusEffects = this.m_characterStats.ActiveStatusEffects;
			for (int i = 0; i < activeStatusEffects.Count; i++)
			{
				StatusEffect statusEffect = activeStatusEffects[i];
				num2 += -Mathf.RoundToInt(statusEffect.DotExpectedDamage(this.m_partyMemberAI.gameObject));
			}

			//BEGINNING OF MOD CODE:
			//num2 = 0;
			//foreach (StatusEffect effect in this.m_characterStats.ActiveStatusEffects)
			//{
			//    num2 += -Mathf.RoundToInt(effect.DotExpectedDamage(this.m_partyMemberAI.gameObject));
			//}
			if (TimeController.Instance != null)
			{
				//this is pretty impressive!
				if (IsVertical) // ADDED THIS CONDITIONAL
				{
					//float desiredXPosition = this.m_Owner.GetDesiredXPosition(this.CurrentSlot); // old line
					float desiredYPosition = (UIPartyPortraitBar.Instance.PortraitWidth + 20) * (this.CurrentSlot * -1);
					//if (base.transform.localPosition.x != desiredXPosition) // old line
					if (base.transform.localPosition.y != desiredYPosition) // old line
					{
						// float num5 = Mathf.Sign(desiredXPosition - base.transform.localPosition.x); // old line
						float num5 = Mathf.Sign(desiredYPosition - base.transform.localPosition.y);
						//float num6 = Mathf.Max(1f, Mathf.Floor(Mathf.Abs((float) ((desiredXPosition - base.transform.localPosition.x) / this.m_Owner.PortraitWidth)))); // old line
						float num6 = Mathf.Max(1f, Mathf.Floor(Mathf.Abs((float)((desiredYPosition - base.transform.localPosition.y) / (UIPartyPortraitBar.Instance.PortraitWidth + 20)))));
						Transform transform = base.transform;

						float targetXpls = 0f;
						if (Dragging)
							targetXpls = UIPartyPortraitBar.Instance.OffsetOnDrag * -1;

						float num7 = Mathf.Sign(targetXpls - base.transform.localPosition.x);
						float num8 = Mathf.Max(1f, Mathf.Floor(Mathf.Abs((float)((targetXpls - base.transform.localPosition.x) / UIPartyPortraitBar.Instance.PortraitWidth))));

						float num9;
						if (PlayerPrefs.GetInt("dragging", 0) == 1)
							num9 = (targetXpls - base.transform.localPosition.x) / (UIPartyPortraitBar.Instance.PortraitSlideSpeed * TimeController.sUnscaledDelta);// works
						else
							num9 = ((num8 * UIPartyPortraitBar.Instance.PortraitSlideSpeed) * TimeController.sUnscaledDelta) * num7;

						transform.localPosition += new Vector3(num9, ((num6 * UIPartyPortraitBar.Instance.PortraitSlideSpeed) * TimeController.sUnscaledDelta) * num5, 0f);

						if (num5 != Mathf.Sign(desiredYPosition - base.transform.localPosition.y))
						{
							float tryX = 0f;
							if (this.Dragging == true)
								tryX = UIPartyPortraitBar.Instance.OffsetOnDrag * -1;

							base.transform.localPosition = new Vector3(tryX, desiredYPosition, base.transform.localPosition.z);
							if (this.m_NeedsUndisplace)
							{
								this.m_NeedsUndisplace = false;
								this.Undisplace();
							}
						}
					}

				}
				else
				{
					float desiredXPosition = UIPartyPortraitBar.Instance.GetDesiredXPosition(this.CurrentSlot);
					if (base.transform.localPosition.x != desiredXPosition)
					{
						float num5 = Mathf.Sign(desiredXPosition - base.transform.localPosition.x);
						float num6 = Mathf.Max(1f, Mathf.Floor(Mathf.Abs((float)((desiredXPosition - base.transform.localPosition.x) / UIPartyPortraitBar.Instance.PortraitWidth))));
						Transform transform = base.transform;

						float targetYpls = 0f;
						if (Dragging)
							targetYpls = UIPartyPortraitBar.Instance.OffsetOnDrag;

						float num7 = Mathf.Sign(targetYpls - base.transform.localPosition.y);
						float num8 = Mathf.Max(1f, Mathf.Floor(Mathf.Abs((float)((targetYpls - base.transform.localPosition.y) / (UIPartyPortraitBar.Instance.PortraitWidth + 20)))));

						float num9;
						if (PlayerPrefs.GetInt("dragging", 0) == 1)
							num9 = (targetYpls - base.transform.localPosition.y) / (UIPartyPortraitBar.Instance.PortraitSlideSpeed * TimeController.sUnscaledDelta);
						else
							num9 = ((num8 * UIPartyPortraitBar.Instance.PortraitSlideSpeed) * TimeController.sUnscaledDelta) * num7;

						transform.localPosition += new Vector3(((num6 * UIPartyPortraitBar.Instance.PortraitSlideSpeed) * TimeController.sUnscaledDelta) * num5, num9, 0f); // replaced 0f with num9
						if (num5 != Mathf.Sign(desiredXPosition - base.transform.localPosition.x))
						{
							float tryY = 0f;
							if (this.Dragging == true)
								tryY = UIPartyPortraitBar.Instance.OffsetOnDrag;

							base.transform.localPosition = new Vector3(desiredXPosition, tryY, base.transform.localPosition.z);
							if (this.m_NeedsUndisplace)
							{
								this.m_NeedsUndisplace = false;
								this.Undisplace();
							}
						}
					}
				}
			}
			//END OF MOD CODE
			this.UpdateHealthBar();
			this.Stamina.gameObject.SetActive(this.m_health.HealthVisible);
			this.StaminaPulse.gameObject.SetActive(this.m_health.HealthVisible);
			this.HealthObfuscator.gameObject.SetActive(!this.m_health.HealthVisible);
			if (this.m_health.HealthVisible)
			{
				float b = Mathf.Max(0f, this.m_health.CurrentHealth);
				float num5 = Mathf.Max(0f, this.m_health.CurrentStamina);
				float num6 = this.m_health.MaxStamina;
				num6 = Mathf.Min(num6, b);
				float num7 = this.m_characterStats.BaseMaxStamina - num6;
				this.Stamina.sliderValue = 1f - Mathf.Clamp01(num5 / num6);
				this.StaminaCap.sliderValue = Mathf.Clamp01(num7 / this.m_characterStats.BaseMaxStamina);
				this.PortraitTexture.material.SetFloat(this.m_SaturationMinV, 1f - this.StaminaCap.sliderValue);
			}
			else
			{
				this.PortraitTexture.material.SetFloat(this.m_SaturationMinV, 0f);
			}
		}
		[ModifiesMember("Displace")]
		private void DisplaceNew() 
		{
			base.transform.localPosition += new Vector3(0f, (float)UIPartyPortraitBar.Instance.OffsetOnDrag, 0f);
			Dragging = true; // added this line
			PlayerPrefs.SetInt ("dragging", 1);
			Transform transform = this.transform;
			if (IsVertical) // added this line
			{
				transform.localPosition -= new Vector3( (float) UIPartyPortraitBar.Instance.OffsetOnDrag, 0f, 0f); // MOD
			}
			else // added this line
			{
				transform.localPosition += new Vector3(0f, (float)UIPartyPortraitBar.Instance.OffsetOnDrag, 0f); //ORIG
			}
		}

		[ModifiesMember("Undisplace")]
		private void UndisplaceNew()
		{
			base.transform.localPosition -= new Vector3(0f, (float)UIPartyPortraitBar.Instance.OffsetOnDrag, 0f);
//
//		Transform transform = this.transform;
//		if (PlayerPrefs.GetInt ("PartyBarToggled", 0) == 1) // ATL
//			transform.localPosition += new Vector3((float) this.m_Owner.OffsetOnDrag, 0f, 0f); // ATL
//		else // ATL
//			transform.localPosition -= new Vector3(0f, (float) this.m_Owner.OffsetOnDrag, 0f); // old line
			UIPartyPortraitBar.Instance.RepositionPortraits ();
		}
		[ModifiesMember("LetGo")]
		public void LetGoNew()
		{
			float desiredXPosition = UIPartyPortraitBar.Instance.GetDesiredXPosition(this.CurrentSlot);
			Dragging = false;
			PlayerPrefs.SetInt ("dragging", 0);

			if (mod_UIPartyPortrait.IsVertical) // ADDED THIS CONDITIONAL
			{
				float desiredYPosition = (UIPartyPortraitBar.Instance.PortraitWidth + 20) * (this.CurrentSlot * -1); // added this line
				//if (base.transform.localPosition.x != desiredXPosition) // old line
				if (base.transform.localPosition.y != desiredYPosition)
				{
					this.m_NeedsUndisplace = true;
				} else
				{
					this.Undisplace ();
				}
			} else
			{
				if (base.transform.localPosition.x != desiredXPosition)
				{
					this.m_NeedsUndisplace = true;
				} else
				{
					this.Undisplace ();
				}
			}
		}
	}
}