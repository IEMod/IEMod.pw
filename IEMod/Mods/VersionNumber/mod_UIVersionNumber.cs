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
		public void alias_MonoBehavior_ctor() {
			
		}

		[ModifiesMember(".ctor")]
		public void mod_ctor() {
			alias_MonoBehavior_ctor();
			var ieModVersion = IEMod.IEModVersion.Version;
			this.FormatString = $"v{{0}}.{{1}} {{2}} {{3}} - IEMod {ieModVersion}";
			this.m_stringBuilder = new StringBuilder();
		}
	}
}
