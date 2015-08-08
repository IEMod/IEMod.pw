using Patchwork.Attributes;

namespace IEMod.Helpers {
	[NewType]
	public class IEComboBoxChoice {
		public string Label;
		public object Value;

		public IEComboBoxChoice(object value, string label) {
			Label = label;
			Value = value;
		}

		public override string ToString() {
			return Label;
		}
	}
}