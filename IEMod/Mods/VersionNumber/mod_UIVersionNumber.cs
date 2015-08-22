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
			MonoBehavior_ctor();
			var ieModVersion = IEMod.IEModVersion.Version;
			var pwVersion = PatchworkVersion.Version;
			this.FormatString = string.Format("v1.0.6.{{0}} {{1}} - IEMod.pw {0} - pw {1}", ieModVersion, pwVersion);
			this.m_stringBuilder = new StringBuilder();
		}
	}
}
