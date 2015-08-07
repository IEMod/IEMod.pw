using IEMod.Helpers;
using Patchwork.Attributes;

namespace IEMod.Mods.StringTable {
	[ModifiesType("StringTableManager")]
	public class mod_StringTableManager
	{
		[ModifiesAccessibility]
		public static bool StringTables;
		[ModifiesAccessibility]
		public static bool StringTableLookup;

		[NewMember]
		[DuplicatesBody("GetText")]
		public static string GetTextOrig(DatabaseString.StringTableType type, int stringID, Gender gender) {
			throw new DeadEndException("GetTextOrig");
		}

		[ModifiesMember]
		public static string GetText(DatabaseString.StringTableType type, int stringID, Gender gender) {
			if (((int)type) == ((int) mod_StringTableType.IEModGUI)) {
				return IEModString.GetString(stringID);
			}
			return GetTextOrig(type, stringID, gender);
		}

	}
}