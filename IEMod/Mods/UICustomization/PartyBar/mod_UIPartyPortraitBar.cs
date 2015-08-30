using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.PartyBar {
	[ModifiesType]
	public class mod_UIPartyPortraitBar : UIPartyPortraitBar
	{
		[ModifiesMember("RepositionPortraits")]
		public void mod_RepositionPortraits()
		{
            ////ORIGINAL
            //for (int i = 0; i < (int)this.m_Portraits.Length; i++)
            //{
            //    Transform mPortraits = this.m_Portraits[i].transform;
            //    float desiredXPosition = this.GetDesiredXPosition(i);
            //    float single = this.m_Portraits[i].transform.localPosition.y;
            //    Vector3 partyPortrait = this.PartyPortrait.transform.localPosition;
            //    mPortraits.localPosition = new Vector3(desiredXPosition, single, partyPortrait.z);
            //}

            //REWRITE
            for (int i = 0; i < this.m_Portraits.Length; i++) {
				this.m_Portraits[i].transform.localPosition = mod_UIPartyPortrait.IsVertical
					? new Vector3(0f, (PortraitWidth + 20) * (i * -1), this.PartyPortrait.transform.localPosition.z)
					: new Vector3(this.GetDesiredXPosition(i), 0f, this.PartyPortrait.transform.localPosition.z);
			}			
		}
	}

}