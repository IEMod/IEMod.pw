using System.IO;
using System.Text;
using IEMod.Helpers;
using Patchwork;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.VersionNumber {
	[ModifiesType]
	public class mod_UIVersionNumber : UIVersionNumber
	{
		
		[MemberAlias(".ctor", typeof(MonoBehaviour))]
		public void MonoBehavior_ctor() {
			
		}

		[ModifiesMember(".ctor")]
		public void CtorNew() {
			//TODO: GR 29/8 - set the FormatString correctly. Is there a const for the current version? I suspect there is.
			//Also, get rid of the PW version. It's really unnecessary. 
			MonoBehavior_ctor();
			var ieModVersion = IEMod.IEModVersion.Version;
			var pwVersion = PatchworkVersion.Version;
			this.FormatString = string.Format("[Game Version].{{0}} {{1}} - IEMod.pw {0} - pw {1}", ieModVersion, pwVersion);
			this.m_stringBuilder = new StringBuilder();
		}
	}
}
