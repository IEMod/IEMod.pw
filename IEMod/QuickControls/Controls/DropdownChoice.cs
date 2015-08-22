using System;
using System.Collections.Generic;
using System.ComponentModel;
using IEMod.Helpers;
using Patchwork.Attributes;

namespace IEMod.QuickControls {
	[NewType]
	public class DropdownChoice<T> {
		public readonly T Value;
		public readonly string Label;

		public DropdownChoice(string label, T value) {
			Label = label;
			Value = value;
		}

		public override string ToString() {
			return Label;
		}
	}

	[NewType]
	public static class DropdownChoice {
		public static DropdownChoice<T>[] FromEnum<T>() {
			var list = new List<DropdownChoice<T>>();
			Type enumType = typeof(T);
			
			foreach (var value in Enum.GetValues(enumType)) {
				var field = enumType.GetField(value.ToString());
				var attr = field.GetCustomAttribute<DescriptionAttribute>();
				var label = attr == null ? null : attr.Description;
				label = label ?? value.ToString();
				list.Add(new DropdownChoice<T>(label, (T)value));
			}
			return list.ToArray();
		}
	}
}