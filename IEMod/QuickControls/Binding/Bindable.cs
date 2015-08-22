using System;
using System.Reflection;
using IEMod.Helpers;
using Patchwork.Attributes;

namespace IEMod.QuickControls {
	/// <summary>
	/// A type that provides binding functionality, wrapping a legal binding target.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[NewType]
	public class Bindable<T> : IBindingValue<T>{
		private readonly IBindingValue<T> _target;
		private bool _isUpdating;
		private readonly object _lock = new object();
		private Binding<T> _binding;

		public Bindable(IBindingValue<T> target)  {
			_target = target;
			_target.HasChanged += OnChanged;
		}

		private void OnChanged(IBindingValue<T> bindingValue) {
			if (_isUpdating || Binding == null) return;
			var wasSourceDisposed = (bool?) null;
			_isUpdating = true;
			if (bindingValue == _target && (Binding.Mode & BindingMode.ToSource) == 0) {
				return;
			}
			if (bindingValue == Binding.Source && (Binding.Mode & BindingMode.FromSource) == 0) {
				return;
			}
			var gettingFrom = bindingValue == _target ? "target" : "source";
			var puttingIn = bindingValue != _target ? "target" : "source";
			var valueTarget = bindingValue == _target ? Binding.Source : _target;
			T value;
			try {
				value = bindingValue.Value;
			}
			catch (Exception ex) {
				//Unity's Mono does not support C#6's exception guards (even though they've been part of the CLI for a while...)
				if (ex is ObjectDisposedException || ex.InnerException is ObjectDisposedException) {
					IEDebug.Log(
						$"In binding ({BindingString}), when the {gettingFrom} changed, tried to get the value but the object was disposed.");
					//IEDebug.PrintException(ex);
					wasSourceDisposed = bindingValue != _target;
					//!!!! WARNING WARNING WARNING !!!! CODER DISCRETION IS ADVISED: 
					goto skipUpdate;
				} else throw;
			}
			try {
				valueTarget.Value = value;
			}
			catch (Exception ex) {
				if (ex is ObjectDisposedException || ex.InnerException is ObjectDisposedException) {
					IEDebug.Log(
						$"In binding ({BindingString}), when the {gettingFrom} changed, tried to update the {puttingIn}'s value, but the object was disposed.");
					//IEDebug.PrintException(ex);
					wasSourceDisposed = bindingValue != _target;
				} else throw;
			}
			skipUpdate:
			_isUpdating = false;

			switch (wasSourceDisposed) {
				case true:
					//means the binding is broken. Better clear it. However, the _target is fine as far as we know.
					IEDebug.Log(
						$"In binding ({BindingString}), the source (thing bound to this) was disposed, so the binding will be scrapped.");
					SetBindingDirect(null);
					break;
				case false:
					//the _target has been disposed, so this Bindable is useless.
					IEDebug.Log(
						$"In binding ({BindingString}), the target (the thing backing this) was disposed, so the binding will be scrapped.");
					Dispose();
					break;
			}

		}

		public static implicit operator Bindable<T>(BindingValue<T> bv) {
			return bv.ToBindable();
		}

		public T Value {
			get {
				return _target.Value;
			}
			set {
				_target.Value = value;
			}
		}

		private void SetBindingDirect(Binding<T> newBinding) {
			if (_binding != null) {
				_binding.Source.HasChanged -= OnChanged;	
			}
			_binding = newBinding;
			if (_binding != null) {
				_binding.Source.HasChanged += OnChanged;	
			}
		}

		public Binding<T> Binding {
			get {
				return _binding;
			}
			set {
				SetBindingDirect(value);
				if (_binding != null) {
					OnChanged(_binding.Source);	
				}
			}
		}

		public event Action<IBindingValue<T>> HasChanged {
			add {
				if (IsDisposed) return;
				_target.HasChanged += value;
			}	
			remove {
				if (IsDisposed) return;
				_target.HasChanged -= value;
			}
		}

		public string BindingString {
			get {
				string symbol = "";
				switch (Binding?.Mode) {
					case BindingMode.FromSource:
						symbol = "<=";
						break;
					case BindingMode.ToSource:
						symbol = "=>";
						break;
					case BindingMode.TwoWay:
						symbol = "<=>";
						break;
					case BindingMode.Disabled:
						symbol = "<=/=>";
						break;
					case null:
						return _target.Name;
				}

				var type = typeof(T).Name;
				return $"[{type}] '{_target.Name}' {symbol} '{Binding.Source.Name}'";
			}
		}

		public string Name => _target.Name;

		public void NotifyChange() {
			if (IsDisposed) return;
			_target.NotifyChange();
		}

		public bool IsDisposed => _target.IsDisposed;

		public void Dispose() {
			Binding = null;
			_target.Dispose();
		}
	}
}