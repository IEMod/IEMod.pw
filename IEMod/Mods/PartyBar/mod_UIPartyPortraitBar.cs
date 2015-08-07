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
			for (int i = 0; i < this.m_Portraits.Length; i++)
			{
				if (PlayerPrefs.GetInt ("PartyBarToggled", 0) == 1) // added line
				{
					/*sb.Append("Setting local position of portrait                     + "(0, " + ((PortraitWidth + 20) * (i * -1)).ToString()
                    + ", " + this.PartyPortrait.transform.localPosition.z.ToString() + ")\n");*/
					this.m_Portraits[i].transform.localPosition = new Vector3(0f , (PortraitWidth + 20) * (i * -1), this.PartyPortrait.transform.localPosition.z); // added line
				}
				else // added line
				{
					this.m_Portraits[i].transform.localPosition = new Vector3(this.GetDesiredXPosition(i), 0f, this.PartyPortrait.transform.localPosition.z); // original line, except changed vector3(y) from this.PartyPortrait.transform.localPosition.y to 0
				}
			}
			//Debug.Log(sb.ToString());
		}
	}

}