using System;
using IEMod.Helpers;
using Patchwork.Attributes;

namespace IEMod.QuickControls {
	[NewType]
	public interface IBindingValue<T> : IDisposable {
		event Action<IBindingValue<T>> HasChanged;

		T Value {
			get;
			set;
		}

		/// <summary>
		/// The Name of a BindingValue is used for debugging and informational purposes.
		/// </summary>
		string Name {
			get;
		}

		void NotifyChange();

		bool IsDisposed { get;}
	}

	[NewType]
	public class BindingValue<T> : IBindingValue<T> {
		private readonly Action<T> _setter;
		private readonly Func<T> _getter;

		public BindingValue(Action<T> setter, Func<T> getter, string name = null) {
			_setter = setter;
			_getter = getter;
			Name = name;
		}

		public static implicit operator T(BindingValue<T> bindingValue) {
			return bindingValue == null ? (T)(object)null : bindingValue.Value;
		}

		private Action<IBindingValue<T>> _hasChanged;

		public event Action<IBindingValue<T>> HasChanged {
			add {
				if (IsDisposed) return;
				_hasChanged += value;
			}
			remove {
				if (IsDisposed) return;
				_hasChanged -= value;
			}
		}

		public virtual void NotifyChange() {
			if (IsDisposed) return;
			var handler = _hasChanged;
			handler?.Invoke(this);
		}

		public bool IsDisposed {
			get;
			private set;
		}

		public T Value {
			get {
				if (_getter == null) {
					throw IEDebug.Exception(null, "No getter!");
				}
				return _getter == null ? default(T) : _getter();
			}
			set {
				if (_setter != null) {
					_setter(value);
					NotifyChange();
				}
			}
		}

		/// <summary>
		/// The Name of a BindingValue is used for debugging and informational purposes.
		/// </summary>
		public string Name {
			get;
			set;
		}

		public void Dispose() {
			_hasChanged = null;
			IsDisposed = true;
		}
	}
}