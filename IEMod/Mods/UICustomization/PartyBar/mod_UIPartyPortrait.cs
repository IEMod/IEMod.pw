using System.Collections.Generic;
using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.PartyBar {
	[ModifiesType]
	public class mod_UIPartyPortrait : UIPartyPortrait
	{
		[NewMember]
		public bool Dragging;
		[NewMember]
		private static bool _isVertical;

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
		public static bool IsVertical {
			get {
				return IEModOptions.EnableCustomUi && _isVertical;
			}
			set {
				_isVertical = value;
			}
		}

		[ModifiesMember("Update")]
		private void UpdateNew() {
            if (this.m_partyMemberAI == null)
            {
                return;
            }
            this.m_EnduranceValues.LineBreak = this.Minion.gameObject.activeSelf;
            this.ActionIcon.Visible = (!GameState.InCombat ? false : GameState.Option.GetOption(GameOption.BoolOption.SHOW_PORTRAIT_ACTION_ICONS));
            this.UpdateLevelTalkGrid();
            if (this.m_ClassCounterType == UIPartyPortrait.ClassCounterType.Focus)
            {
                this.ClassCount = Mathf.FloorToInt(this.m_characterStats.Focus);
            }
            else if (this.m_ClassCounterType == UIPartyPortrait.ClassCounterType.Phrases)
            {
                if (!this.m_characterChanter)
                {
                    this.m_characterChanter = this.m_characterStats.GetChanterTrait();
                }
                if (this.m_characterChanter)
                {
                    this.ClassCount = this.m_characterChanter.PhraseCount;
                }
            }
            if (this.m_CipherFocusVfx)
            {
                Transform mCipherFocusVfx = this.m_CipherFocusVfx.transform;
                float single = this.m_CipherFocusVfx.transform.localPosition.x;
                Vector3 vector3 = this.m_CipherFocusVfx.transform.localPosition;
                mCipherFocusVfx.localPosition = new Vector3(single, vector3.y);
            }
            float pulseMinAlpha = this.PulseMinAlpha + (this.PulseMaxAlpha - this.PulseMinAlpha) * Mathf.Sin(3.14159274f * TimeController.sUnscaledDelta / this.PulsePeriodSeconds);
            if (this.m_StaminaPulseSprite && this.m_HealthPulseSprite)
            {
                UISprite mStaminaPulseSprite = this.m_StaminaPulseSprite;
                float single1 = pulseMinAlpha;
                this.m_HealthPulseSprite.alpha = single1;
                mStaminaPulseSprite.alpha = single1;
            }
            int num = 0;
            for (int i = 0; i < this.m_characterStats.ActiveStatusEffects.Count; i++)
            {
                num = num + -Mathf.RoundToInt(this.m_characterStats.ActiveStatusEffects[i].DotExpectedDamage(this.m_partyMemberAI.gameObject));
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
            bool healthVisible = this.m_health.HealthVisible;
            this.UpdateHealthBar();
            this.Stamina.gameObject.SetActive(healthVisible);
            this.StaminaPulse.gameObject.SetActive(healthVisible);
            this.StaminaEdge.gameObject.SetActive(healthVisible);
            this.HealthObfuscator.gameObject.SetActive(!healthVisible);
            if (!healthVisible)
            {
                this.PortraitTexture.material.SetFloat(UIPartyPortrait.m_Saturation, 0f);
                this.PortraitTexture.material.SetFloat(UIPartyPortrait.m_SaturationMinV, 0f);
            }
            else
            {
                float single5 = Mathf.Max(0f, this.m_health.CurrentHealth);
                float single6 = Mathf.Max(0f, this.m_health.CurrentStamina);
                float maxStamina = this.m_health.MaxStamina;
                maxStamina = Mathf.Min(maxStamina, single5);
                float baseMaxStamina = this.m_characterStats.BaseMaxStamina - maxStamina;
                float single7 = Mathf.Clamp01(baseMaxStamina / this.m_characterStats.BaseMaxStamina);
                float single8 = 1f - Mathf.Clamp01(single6 / maxStamina);
                single8 = Mathf.Min(single8, 1f - single7);
                this.Stamina.sliderValue = single8;
                this.StaminaCap.sliderValue = single7;
                this.PortraitTexture.material.SetFloat(UIPartyPortrait.m_Saturation, 0f);
                this.PortraitTexture.material.SetFloat(UIPartyPortrait.m_SaturationMinV, 1f - this.StaminaCap.sliderValue);
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