using System;
using Patchwork.Attributes;

namespace IEMod.QuickControls {
	/// <summary>
	/// Represents the direction in which changes propogate in a binding.
	/// </summary>
	[Flags]
	[NewType]
	public enum BindingMode {
		/// <summary>
		/// The binding is inactive. Changes do not propogate.
		/// </summary>
		Disabled = 0x0,
		/// <summary>
		/// Changes propogate to the binding target (usually the owner of the Bindable) from the source.
		/// </summary>
		FromSource = 0x1,
		/// <summary>
		/// Changes propogate to the binding source from the target (usually the owner of the Bindable).
		/// </summary>
		ToSource = 0x2,
		/// <summary>
		/// Changes propogate in both directions.
		/// </summary>
		TwoWay = FromSource | ToSource
	}
}