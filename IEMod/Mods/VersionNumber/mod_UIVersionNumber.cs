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
			//GR 29/8 - there has to be some const for the version, but I haven't found it :/
			this.FormatString = string.Format("v2.00.{{0}} {{1}} - IEMod.pw {0}", ieModVersion);
			this.m_stringBuilder = new StringBuilder();
		}
	}
}
