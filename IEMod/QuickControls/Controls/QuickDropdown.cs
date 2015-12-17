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
	public class QuickDropdown<T> : QuickControl {
		[ModifiesType]

		private class mod_UIDropdownMenu : UIDropdownMenu {
			[ModifiesAccessibility]
			public new void RefreshSelected() {

			}

			[ModifiesAccessibility]
			public new void RefreshDropdown() {
				
			}
		}

		private class mod_GUIStringLabel : GUIStringLabel {
			[ModifiesAccessibility]
			public new void RefreshText() {
				
			}
		}

		public QuickDropdown(Transform parent = null, string name = "QuickDropdown", GameObject altPrototype = null) {
			var exampleDropdown = altPrototype ?? Prefabs.QuickDropdown;
			if (exampleDropdown == null) {
				throw IEDebug.Exception(null, "You must initialize the ExampleDropdown to create a dropdown.", null);
			}
			GameObject = (GameObject)Object.Instantiate(exampleDropdown);
			GameObject.transform.parent = parent;
			GameObject.name = name;
			//! You have to explicitly set localPosition and localScale to something after Instantiate!!!
			//! Otherwise, the UI will broken, but no exception will be reported.
			GameObject.transform.localPosition = Vector3.zero;
			GameObject.transform.localScale = Vector3.one;

			SelectedValue = BindingValue.Member(() => this.selectedValue).ToBindable();
			Options = new List<DropdownChoice<T>>();
			DropdownComponent.OnDropdownOptionChangedEvent += x => SelectedValue.NotifyChange();
			IEDebug.Log("Created: " + name);
		}

		public GUIStringLabel LabelComponent {
			get {
				return GameObject.transform.ComponentInDescendants<GUIStringLabel>(true);
			}
		}

		public string LabelText {
			get {
				
				var label = LabelComponent.DatabaseString;
				return label.GetText();
			}
			set {
				
				LabelComponent.DatabaseString.MaybeUnregister();
				LabelComponent.DatabaseString = IEModString.Register(value);
				Refresh();
			}
		}

		public int LabelWidth {
			get {
				
				return LabelComponent.Component<UILabel>().lineWidth;
			}
			set {
				
				if (value == 0) {
					LabelText = "";
				}
				LabelComponent.Component<UILabel>().lineWidth = value;
				Refresh();
			}
		}

		public string TooltipText {
			get {
				
				var tts = GameObject.Component<UIOptionsTag>().TooltipString;
				return tts != null ? tts.GetText() : null;
			}
			set {
				
				var uiOptionsTag = GameObject.Component<UIOptionsTag>();
				uiOptionsTag.TooltipString.MaybeUnregister();
				uiOptionsTag.TooltipString = IEModString.Register(value);
				Refresh();
			}
		}

		public int Width {
			get {
                return (int) DropdownComponent.DropdownBackground.transform.localScale.x;
                
			}
			set {

                var comboBoxBackground = DropdownComponent.DropdownBackground;
                comboBoxBackground.transform.localScale = new Vector3(value, 32, 1); //this is the width of the combobox
                DropdownComponent.BaseCollider.transform.localScale = new Vector3(value, 32, 1); //this is the width of the combobox

                var arrowThing = DropdownComponent.ArrowPivot;
                arrowThing.transform.localPosition = new Vector3(value - 27, 10, 0); //the location of the arrow thing

                var dropdown = DropdownComponent;
				foreach (var label in dropdown.OptionGrid.ComponentsInDescendants<UILabel>()) {
					label.lineWidth = value; //the width of each line in the OpitonGrid.
				}

                dropdown.SelectedText.lineWidth = value;
                dropdown.OptionGrid.cellWidth = value;
                Refresh();
            }
		}

		public UIDropdownMenu DropdownComponent {
			get {
				
				return GameObject.ComponentInDescendants<UIDropdownMenu>();
			}
		}

		public IEnumerable<DropdownChoice<T>> Options {
			get {
				
				var dropdown = DropdownComponent;
				return dropdown.Options.Cast<DropdownChoice<T>>();
			}
			set {
				var dropdown = DropdownComponent;
				dropdown.Options = value.Cast<object>().ToArray();
				var first = value.FirstOrDefault();
				SelectedValue.Value = first == null ? default(T) : first.Value;
				Refresh();
			}
		}

		public void Refresh() {
			
			var dropdown = DropdownComponent;
			dropdown.RefreshDropdown();
			dropdown.RefreshSelected();
			LabelComponent.RefreshText();
		}

		private T selectedValue {
			get {
				
				var asChoice = (DropdownChoice<T>)DropdownComponent.SelectedItem;
				return asChoice == null ? default(T) : asChoice.Value;
			}
			set {
				var xfg = default(int);
				var dropdown = DropdownComponent;
				var tryFindChoice = dropdown.Options.Cast<DropdownChoice<T>>().SingleOrDefault(x => Equals(x.Value, value));
				dropdown.SelectedItem = tryFindChoice ?? (dropdown.Options.Length == 0 ? null : dropdown.Options[0]);
				Refresh();
			}
		}

		public Bindable<T> SelectedValue {
			get;
		}

	}
}
