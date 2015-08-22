using Patchwork.Attributes;

namespace IEMod.QuickControls
{

	/// <summary>
	/// Represents a Binding for some Bindable, consisting of a binding source and a binding mode.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[NewType]
	public class Binding<T>{

		public Binding(IBindingValue<T> source, BindingMode mode = BindingMode.TwoWay) {
			Source = source;
			Mode = mode;
		}

		public IBindingValue<T> Source {
			get;
		}

		public BindingMode Mode {
			get;
		}
	}


}
