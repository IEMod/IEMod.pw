using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using IEMod.Helpers;
using Patchwork.Attributes;

namespace IEMod.QuickControls {

	[NewType]
	public static class BindingValue {
		/// <summary>
		/// Creates a BindingValue from an expression that accesses a property or field. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expr"></param>
		/// <param name="notifier"></param>
		/// <returns></returns>
		public static BindingValue<T> Member<T>(Expression<Func<T>> expr, INotifyPropertyChanged notifier = null) {
			var accessor = ReflectHelper.AnalyzeMember(expr);
			var instance = accessor.InstanceGetter();


			var instanceName = ReflectHelper.TryGetName(instance);
			notifier = notifier ?? accessor.InstanceGetter() as INotifyPropertyChanged;
			var bindingValue = new BindingValue<T>(accessor.Setter, accessor.Getter, $"{instanceName ?? "?"}.{accessor.TopmostMember.Name}");
			if (notifier != null) {
				notifier.PropertyChanged += (sender, e) => {
					if (e.PropertyName == accessor.TopmostMember.Name) {
						bindingValue.NotifyChange();
					}
				};
			}
			return bindingValue;
		}

		public static IBindingValue<T> OnChange<T>(this IBindingValue<T> bv, Action<IBindingValue<T>> action) {
			bv.HasChanged += action;
			return bv;
		}

		/// <summary>
		/// Creates a special storage locaiton wrapped in a BindingValue, which supports change notification.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="initialValue">The initial value to which the storage location is initialized.</param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static BindingValue<T> Variable<T>(T initialValue, string name = "?") {
			var storage = initialValue;
			BindingValue<T> bindingValue = null;
			Action<T> setter = v => {
				storage = v;
				bindingValue.NotifyChange();
			};
			Func<T> getter = () => storage;
			bindingValue = new BindingValue<T>(setter, getter, name);
			return bindingValue;
		}

		public static BindingValue<T> Const<T>(T constant) {
			return new BindingValue<T>(null, () => constant);
		}

		public static Bindable<T> ToBindable<T>(this IBindingValue<T> bv) {
			return new Bindable<T>(bv);
		}

		public static Binding<T> ToBinding<T>(this IBindingValue<T> bv, BindingMode mode = BindingMode.TwoWay) {
			return new Binding<T>(bv, mode);
		}
	}
	[NewType]
	public static class Binding {
		public static IBindingValue<T> Bind<T>(this Bindable<T> bindable, IBindingValue<T> source, BindingMode mode = BindingMode.TwoWay) {
			bindable.Binding = source.ToBinding(mode);
			return source;
		}

		public static IBindingValue<T> Bind<T>(this Bindable<T> bindable, Expression<Func<T>> memberExpr, BindingMode mode = BindingMode.TwoWay) {
			return bindable.Bind(BindingValue.Member(memberExpr));
		}

	}
}
