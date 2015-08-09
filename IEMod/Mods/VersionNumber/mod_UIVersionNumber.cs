using System.IO;
using System.Text;
using IEMod.Helpers;
using Patchwork.Attributes;
using Patchwork.Shared;

namespace IEMod.Mods.VersionNumber {
	[ModifiesType]
	public class mod_UIVersionNumber : UIVersionNumber
	{
		[ModifiesMember(".ctor")]
		public void CtorNew()
		{
			var ieModVersion = IEMod.IEModVersion.Version;
			var pwVersion = PwVersion.Version;
			this.FormatString = string.Format("v1.0.6.{{0}} {{1}} - IEMod.pw {0} - pw {1}", ieModVersion, pwVersion);
			this.m_stringBuilder = new StringBuilder();
		}
	}
}
