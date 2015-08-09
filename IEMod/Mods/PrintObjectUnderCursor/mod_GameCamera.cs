using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEMod.Helpers;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.PrintObjectUnderCursor
{
	[ModifiesType]
	class mod_GameCamera : UICamera {

		[NewMember]
		[DuplicatesBody("Update")]
		public void UpdateOrig() {
			
		}

		[ModifiesMember("Update")]
		public void UpdateNew() {
			IEDebug.Log(DateTime.Now + " Hi from mod_GameCamera");
			UpdateOrig();
		}
	}
}
