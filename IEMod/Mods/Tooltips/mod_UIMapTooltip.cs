using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Tooltips {
	[ModifiesType()]
	public class mod_UIMapTooltip : UIMapTooltip
	{
		[ModifiesMember("Set")]
		public void SetNew(GameObject target)
		{
			//Debug.Log("P! begin UIMapTooltip.Set\nVerticalPartyBars is: " + PlayerPrefs.GetInt("PartyBarToggled", 0).ToString());
			if (!this.m_Init)
			{
				this.Initialize();
			}
			if (this.m_Target != target)
			{
				this.Reset();
			}
			if (target != this.m_Target)
			{
				this.m_Target = target;
				this.m_Stats = this.m_Target.GetComponent<CharacterStats>();
				if (this.OnSelectedCharacterChanged != null)
				{
					this.OnSelectedCharacterChanged(this.SelectedCharacter);
				}
			}
			this.m_PartyAI = this.m_Target.GetComponent<PartyMemberAI>();
			this.m_Health = this.m_Target.GetComponent<Health>();
			this.m_Faction = this.m_Target.GetComponent<Faction>();
			this.m_BackerContent = this.m_Target.GetComponent<BackerContent>();
			if (this.PointerAnchor != null)
			{
				this.PointerAnchor.relativeOffset = new Vector2(0f, this.PointerAnchor.relativeOffset.y);
			}
			if (this.PortraitAnchor != null)
			{
				this.PortraitAnchor.relativeOffset = new Vector2(0f, this.PortraitAnchor.relativeOffset.y);
				this.PortraitAnchor.side = UIAnchor.Side.Center;
			}

			// displays tooltip over your party members models if you hover over their ingame models, instead of always displaying it above portraits.
			// and displays it above portraits if you hover over their portraits
			bool npcUnderCursor = GameCursor.OverrideCharacterUnderCursor == target; // added this line

			if (this.TargetIsParty && (this.PortraitAnchor != null) && npcUnderCursor) // added && npcUnderCursor
			{
				UIPartyPortrait portraitFor = UIPartyPortraitBar.Instance.GetPortraitFor(this.m_PartyAI);
				this.PortraitAnchor.widgetContainer = (portraitFor == null) ? null : portraitFor.Border;
				this.PortraitAnchor.enabled = true;
				this.WorldAnchor.enabled = false;

				// the following part was completely rewritten. If you have a horizontal alignment, the tooltip will be displayed
				// above or under the portrait depending on the portrait bar position on the screen.
				// Same principle is applied to the vertical alignment - the tooltip on the left if the partybar is in the right part
				// of the screen, and vice versa.
				if (PlayerPrefs.GetInt ("PartyBarToggled", 0) == 1) // if we're using vertical party bar alignment
				{
					if (UIPartyPortraitBar.Instance.transform.localPosition.x < Screen.width / 2) // if left side of the screen
					{
						this.PointerAnchor.relativeOffset = new Vector2 (-0.495f, 0.525f);
						this.PointerAnchor.transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, -90f));

						this.PortraitAnchor.side = UIAnchor.Side.Right;
						this.PortraitAnchor.relativeOffset = new Vector2 (1f, -1f);

						if (Screen.width < 1600)
							this.PortraitAnchor.relativeOffset = new Vector2 (this.PortraitAnchor.relativeOffset.x * 1.5f, this.PortraitAnchor.relativeOffset.y);
						else if (Screen.width < 1920)
							this.PortraitAnchor.relativeOffset = new Vector2 (this.PortraitAnchor.relativeOffset.x * 1.3f, this.PortraitAnchor.relativeOffset.y);
					} 
					else // if right side of the screen
					{
						this.PointerAnchor.relativeOffset = new Vector2 (0.495f, 0.525f);
						this.PointerAnchor.transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, 90f));

						this.PortraitAnchor.side = UIAnchor.Side.Left;
						this.PortraitAnchor.relativeOffset = new Vector2 (-1f, -1f);

						if (Screen.width < 1600)
							this.PortraitAnchor.relativeOffset = new Vector2 (this.PortraitAnchor.relativeOffset.x * 1.5f, this.PortraitAnchor.relativeOffset.y);
						else if (Screen.width < 1920)
							this.PortraitAnchor.relativeOffset = new Vector2 (this.PortraitAnchor.relativeOffset.x * 1.3f, this.PortraitAnchor.relativeOffset.y);
					}
				} else 															// horizontal party bar alignment
				{
					if (UIPartyPortraitBar.Instance.transform.localPosition.y > Screen.height / 2) // if we're in the top part of the screen
					{
						this.PointerAnchor.relativeOffset = new Vector2 (0f, 1f);
						this.PointerAnchor.transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, 180f));

						if (portraitFor.CurrentSlot == 0)
						{
							if (this.PointerAnchor != null)
							{
								this.PointerAnchor.relativeOffset = new Vector2 (-0.44f, this.PointerAnchor.relativeOffset.y);
							}
							//this.PortraitAnchor.pixelOffset = new Vector2 (100f, -200f);
							this.PortraitAnchor.relativeOffset = new Vector2 (0.4f, -2f);
							this.PortraitAnchor.side = UIAnchor.Side.Right;
						} else
						{
							this.PointerAnchor.relativeOffset = new Vector2 (0f, 1f); //
							this.PortraitAnchor.relativeOffset = new Vector2 (0.5f, -2f);
							this.PortraitAnchor.side = UIAnchor.Side.Left;
						}

						if (Screen.width < 1600)
							this.PortraitAnchor.relativeOffset = new Vector2 (this.PortraitAnchor.relativeOffset.x, this.PortraitAnchor.relativeOffset.y * 1.1f);
					} 
					else // if we're in the bottom half of the screen
					{
						this.PointerAnchor.relativeOffset = new Vector2 (0f, 0f);
						this.PointerAnchor.transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, 0f));

						if (portraitFor.CurrentSlot == 0)
						{
							if (this.PointerAnchor != null)
							{
								this.PointerAnchor.relativeOffset = new Vector2 (-0.44f, 0f);
							}
							this.PortraitAnchor.relativeOffset = new Vector2 (0.4f, 0f);
							this.PortraitAnchor.side = UIAnchor.Side.Right;
						} else
						{
							this.PointerAnchor.relativeOffset = new Vector2 (0f, 0f);
							this.PortraitAnchor.relativeOffset = new Vector2 (0.5f, 0f);
							this.PortraitAnchor.side = UIAnchor.Side.Left;
						}
					}
				}

				base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 0f); // ommiting this line will make your tooltip for party portrait be behind his abilities

			} else
			{ // for normal tooltip
				// added this code
				if (this.PointerAnchor != null) 
				{
					this.PointerAnchor.relativeOffset = new Vector2 (0f, 0f);
					this.PointerAnchor.transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, 0f));
				}
				// end of added code

				if (this.PortraitAnchor != null)
				{
					this.PortraitAnchor.enabled = false;
				}
				if (this.WorldAnchor != null)
				{
					this.WorldAnchor.enabled = true;
					this.WorldAnchor.SetAnchor (target);
				}
				base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 1f); //FIXME: this line, man... is this needed? and do we need to fix base?
			}
			this.RefreshDynamicContent();
		}
	}
}