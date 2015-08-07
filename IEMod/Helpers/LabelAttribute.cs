using System;
using Patchwork.Attributes;

namespace IEMod.Helpers {
	/// <summary>
	/// Used in the IEMod UI to attach a string label to an option.
	/// </summary>
	[NewType]
	public class LabelAttribute : Attribute {
		public LabelAttribute(string label) {
			Label = label;
		}

		public string Label;
	}
}