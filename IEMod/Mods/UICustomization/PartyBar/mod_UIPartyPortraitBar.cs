using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.PartyBar {
	[ModifiesType]
	public class mod_UIPartyPortraitBar : UIPartyPortraitBar
	{
		[ModifiesMember("RepositionPortraits")]
		public void RepositionPortraitsNew()
		{
			/*StringBuilder sb = new StringBuilder();
        sb.Append("P! RepositionPortraitsNew() start. PartyBarToggle = " + PlayerPrefs.GetInt("PartyBarToggled", 0).ToString() + "\n");*/
			for (int i = 0; i < this.m_Portraits.Length; i++) {
				this.m_Portraits[i].transform.localPosition = mod_UIPartyPortrait.IsVertical
					? new Vector3(0f, (PortraitWidth + 20) * (i * -1), this.PartyPortrait.transform.localPosition.z)
					: new Vector3(this.GetDesiredXPosition(i), 0f, this.PartyPortrait.transform.localPosition.z);
			}
			//Debug.Log(sb.ToString());
		}
	}

}