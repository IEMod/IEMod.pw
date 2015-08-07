using System.Text;
using Patchwork.Attributes;

namespace IEMod.Mods.VersionNumber {
	[ModifiesType]
	public class mod_UIVersionNumber : UIVersionNumber
	{
		[ModifiesMember(".ctor")]
		public void CtorNew()
		{
			this.FormatString = "v1.0.6.{0} {1} - pw0.3 - IEMod 4.3.1";
			this.m_stringBuilder = new StringBuilder();
		}
	}
}
