using Patchwork.Attributes;

namespace IEMod.Helpers {
	[NewType]
	public class IEDropdownChoice {
		public string Label;
		public object Value;

		public IEDropdownChoice(object value, string label) {
			Label = label;
			Value = value;
		}

		public override string ToString() {
			return Label;
		}
	}
}