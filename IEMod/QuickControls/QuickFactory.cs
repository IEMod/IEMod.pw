using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using IEMod.Helpers;
using IEMod.Mods.StringTable;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IEMod.QuickControls {

	[NewType]
	public class QuickFactory {
		public QuickFactory(Transform currentParent) {
			CurrentParent = currentParent;
		}

		public QuickFactory() {
		}

		public Transform CurrentParent {
			get;
			set;
		}

		public QuickCheckbox Checkbox(Expression<Func<bool>> memberAccessExpr, string name = null) {
			var memberInfo = ReflectHelper.AnalyzeMember(memberAccessExpr);
			var label =  ReflectHelper.GetLabelInfo(memberInfo.TopmostMember);
			var desc = ReflectHelper.GetDescriptionInfo(memberInfo.TopmostMember);
			var checkbox = new QuickCheckbox(parent:CurrentParent, name:name ?? memberInfo.TopmostMember.Name) {
				Label = label,
				Tooltip = desc,
			};
			checkbox.IsChecked.Bind(memberAccessExpr);
			return checkbox;
		}

		public QuickDropdown<T> Dropdown<T>(Expression<Func<T>> memberAccessExpr, IEnumerable<DropdownChoice<T>> choices, string name = null) {
			var memberInfo = ReflectHelper.AnalyzeMember(memberAccessExpr);
			var label = ReflectHelper.GetLabelInfo(memberInfo.TopmostMember);
			var desc = ReflectHelper.GetDescriptionInfo(memberInfo.TopmostMember);
			var dropdown = new QuickDropdown<T>(CurrentParent, name:name ?? memberInfo.TopmostMember.Name) {
				Options = choices.ToArray(),
				LabelText = label,
				TooltipText = desc,
			};
			dropdown.SelectedValue.Bind(memberAccessExpr);
			return dropdown;
		}

		public QuickButton Button(string caption = "", string name = null, Vector3? localPos = null) {
			var illegalCharsStr = " :\t\n\r/1#$%^&*().".ToCharArray().Select(char.ToString).ToArray();
			name = name ?? caption.ReplaceAll("_", illegalCharsStr);
			return new QuickButton(CurrentParent, name) {
				LocalPosition = localPos ?? Vector3.zero,
				Caption = caption
			};
		}

		public QuickDropdown<T> EnumDropdown<T>(Expression<Func<T>> memberAccessExpr, string name = null) 
			where T : struct, IConvertible, IComparable, IFormattable {
			return Dropdown(memberAccessExpr, DropdownChoice.FromEnum<T>(), name);
		}		
	}
}
