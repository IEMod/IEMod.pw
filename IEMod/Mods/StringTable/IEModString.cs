using System.Collections.Generic;
using Patchwork.Attributes;

namespace IEMod.Mods.StringTable {
	/// <summary>
	///     Use this DatabaseString entry for IEMod strings.
	/// </summary>
	[NewType]
	public class IEModString : GUIDatabaseString {
		public static readonly Dictionary<int, string> IEModStringTable = new Dictionary<int, string>();
		public static int LastId = 1;

		private IEModString(int id)
			: base(id) {
			base.StringTable = (StringTableType) mod_StringTableType.IEModGUI;
		}

		public static string GetString(int id) {
			if (!IEModStringTable.ContainsKey(id)) {
				return string.Format("?? IEMod {0} ??", id);
			}
			return IEModStringTable[id];
		}

		public static IEModString Register(string str) {
			var lastId = LastId;
			LastId++;
			IEModStringTable[lastId] = str;
			return new IEModString(lastId);
		}

		public override StringTableType GetStringTable() {
			return (StringTableType) mod_StringTableType.IEModGUI;
		}

		[ModifiesType]
		public class mod_DatabaseString : DatabaseString {
			//don't remember if you need these or not
			[ModifiesAccessibility]
			public new bool StringTable;

			[ModifiesAccessibility]
			public new int StringID;
		}
	}
}