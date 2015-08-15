using System.Collections.Generic;
using Patchwork.Attributes;

namespace IEMod.Mods.StringTable {

	/// <summary>
	///     Use this DatabaseString entry for IEMod strings.
	/// </summary>
	[NewType]
	public class IEModString : GUIDatabaseString {
		private static readonly Dictionary<int, string> IeModStringTable = new Dictionary<int, string>();
		private static int _lastId = 1;

		private IEModString(int id)
			: base(id) {
			base.StringTable = (StringTableType) mod_StringTableType.IEModGUI;
		}

		public static string GetString(int id) {
			if (!IeModStringTable.ContainsKey(id)) {
				return string.Format("?? IEMod {0} ??", id);
			}
			return IeModStringTable[id];
		}

		public void Unregister() {
			IeModStringTable.Remove(this.StringID);
		}

		public static IEModString Register(string str) {
			var lastId = _lastId;
			_lastId++;
			IeModStringTable[lastId] = str;
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