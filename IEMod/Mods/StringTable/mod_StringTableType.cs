using Patchwork.Attributes;

namespace IEMod.Mods.StringTable {
	/// <summary>
	/// We add an extra member to the enum for IEMod strings.
	/// That way they won't conflict with other GUI strings, and also lets us circumvent the standard string table system
	/// which is a massive pain in the underside.
	/// </summary>
	[ModifiesType("DatabaseString/StringTableType")]
	public enum mod_StringTableType {
		[NewMember]
		IEModGUI = -2135132,
	}
}