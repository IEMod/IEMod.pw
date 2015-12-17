using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IEMod.Mods.Options;
using IEMod.Mods.StringTable;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IEMod.Helpers {

	/// <summary>
	/// This class allows you to construct standard controls quickly and easily, obscuring any ugliness the task involves.
	/// </summary>
	[NewType]
	[Obsolete("Use QuickFactory instead.")]
	public class IEControlFactory {


		public UIOptionsTag ExampleCheckbox;
		public UIOptionsTag ExampleDropdown;
		public Transform CurrentParent;
		public GameObject ExamplePage;
		public GameObject ExampleButton;

		private static IEDropdownChoice[] EnumToChoices(Type enumType) {
			var list = new List<IEDropdownChoice>();

			foreach (var value in Enum.GetValues(enumType)) {
				var field = enumType.GetField(value.ToString());
				var attr = field.GetCustomAttribute<DescriptionAttribute>();
				var label = attr == null ? null : attr.Description;
				label = label ??  value.ToString();
				list.Add(new IEDropdownChoice(value, label));
			}
			return list.ToArray();
		}

		/// <summary>
		/// Creates a new Page object, for the properties screen. ExamplePage must be set before calling this.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public GameObject Page(string name) {
			if (ExamplePage == null) {
				throw IEDebug.Exception(null, "You must initialize the ExamplePage to create a Page", null);
			}
			var newPage = new GameObject ();
			newPage.transform.parent = ExamplePage.transform.parent;
			newPage.transform.localScale =  ExamplePage.transform.localScale;
			newPage.transform.localPosition =  ExamplePage.transform.localPosition;
			newPage.name = name;
			
			return newPage;
		}

		/// <summary>
		/// Creates a dropdown bound to a field or property. You need to supply a collection of dropdown choices.
		/// </summary>
		public GameObject Dropdown<T>(Expression<Func<T>> memberAccessExpr,
			IEnumerable<IEDropdownChoice> choices,
			int width, int labelWidth,
			string label = null,
			string tooltip = null
			) {
			#region Old code

			/*
			//+ FOR REFERENCE:
			const int nerfedXPTableWidth = 515;
			UIDropdownMenu drop;
			nerfedXPTableDropdown.transform.GetChild(1).GetChild(0).localScale = new Vector3(nerfedXPTableWidth, 32, 1); // width of combobox
			nerfedXPTableDropdown.transform.GetChild(1).GetChild(3).localPosition = new Vector3(nerfedXPTableWidth - 27, 10, 0); // position of down arrow
			nerfedXPTableDropdown.transform.GetChild(1).GetChild(2).GetChild(0).localScale = new Vector3(nerfedXPTableWidth, 37, 0); // width of dropdown
			nerfedXPTableDropdown.transform.GetChild(1).Component<UIDropdownMenu>().OptionRootText.lineWidth = nerfedXPTableWidth;
			nerfedXPTableDropdown.transform.GetChild(1).Component<UIDropdownMenu>().SelectedText.lineWidth = nerfedXPTableWidth;
			nerfedXPTableDropdown.transform.GetChild(1).Component<UIDropdownMenu>().OptionGrid.cellWidth = nerfedXPTableWidth;
			nerfedXPTableDropdown.transform.GetChild(1).Component<UIDropdownMenu>().Options = nerfedXPTableChoices;
			//nerfedXPTableDropdown.transform.GetChild(1).Component<UIDropdownMenu>().SelectedItem = nerfedXPTableChoices[PlayerPrefs.GetInt("NerfedXPTableSetting")];
			//nerfedXPTableDropdown.transform.GetChild(1).Component<UIDropdownMenu>().OnDropdownOptionChangedEvent += new UIDropdownMenu.DropdownOptionChanged(OnNerfedXPTableSettingChanged);
			nerfedXPTableDropdown.transform.GetChild(0).Component<UILabel>().lineWidth = 300;
			nerfedXPTableDropdown.transform.GetChild(0).Component<UILabel>().shrinkToFit = false;
			nerfedXPTableDropdown.transform.GetChild(0).Component<GUIStringLabel>().DatabaseString = new GUIDatabaseString(++StringId);
			nerfedXPTableDropdown.Component<UIOptionsTag>().TooltipString = new GUIDatabaseString(++StringId);
			// end of adding dropdown
			 */

			#endregion

			if (ExampleDropdown == null) {
				throw IEDebug.Exception(null, "You must initialize the ExampleDropdown to create a dropdown.", null);
			}
			var pos = new Vector3(0, 0, 0);
			var getter = ReflectHelper.CreateGetter(memberAccessExpr);
			var setter = ReflectHelper.CreateSetter(memberAccessExpr);
			var asMemberExpr = (MemberExpression) memberAccessExpr.Body;
			var comboBox = (UIOptionsTag) Object.Instantiate(ExampleDropdown);
			comboBox.transform.parent = CurrentParent;
			//+ Basic setup

			comboBox.name = asMemberExpr.Member.Name;
			//! You have to explicitly set localPosition and localScale to something after Instantiate!!!
			//! Otherwise, the UI will broken, but no exception will be reported.
			comboBox.transform.localPosition = pos;
			comboBox.transform.localScale = new Vector3(1, 1, 1);

			var dropdown = comboBox.transform.ComponentsInDescendants<UIDropdownMenu>(true).Single();
			label = label ?? ReflectHelper.GetLabelInfo(asMemberExpr.Member);
			tooltip = tooltip ?? ReflectHelper.GetDescriptionInfo(asMemberExpr.Member);

			//+ Set the labels
			//There are multiple things called Label in this visual tree, but only one with a GUIStringLabel component.
			var guiLabel = comboBox.transform.ComponentsInDescendants<GUIStringLabel>(true).Single();
			if (labelWidth > 0) {
				guiLabel.DatabaseString = IEModString.Register(label);
				var uiLabel = guiLabel.gameObject.Component<UILabel>();
				uiLabel.lineWidth = labelWidth;
			} else {
				guiLabel.DatabaseString = IEModString.Register("");
				var uiLabel = guiLabel.gameObject.Component<UILabel>();
				uiLabel.lineWidth = labelWidth;
			}

			var uiTag = comboBox.Component<UIOptionsTag>();
			uiTag.TooltipString = IEModString.Register(tooltip);

			//+ set the choices and SelectedItem
			var choicesArr = choices.ToArray();
			dropdown.Options = choicesArr.ToArray();
			dropdown.SelectedItem = choicesArr.SingleOrDefault(x => object.Equals(x.Value, getter())) ?? dropdown.Options[0];

			//+ Set position and scale
			//To get the names, I used the GetChild calls as reference, and dumped the comboBox to the log using UnityObjectDumper.
			var comboBoxBackground = comboBox.Descendant("Background");
			comboBoxBackground.transform.localScale = new Vector3(width, 32, 1); //this is the width of the combobox

			var arrowThing = comboBox.Descendant("ArrowPivot");
			arrowThing.transform.localPosition = new Vector3(width - 27, 10, 0);
			//dropdown.OptionRootText.lineWidth = width;
			dropdown.SelectedText.lineWidth = width;
			dropdown.OptionGrid.cellWidth = width;
			var optGrid = dropdown.OptionGrid;
			//+ Set the default handler that binds the control to the member
			dropdown.OnDropdownOptionChangedEvent += option => {
				var asChoice = (IEDropdownChoice) option;
				setter((T) asChoice.Value);
			};

			return comboBox.gameObject;
		}


		/// <summary>
		/// Creates a new dropdown/combo box bound to an enum-valued field or property. The enum part is important, as this is how the dropdown's options are generated. Labels are taken from attributes.
		/// </summary>
		/// <typeparam name="T">The type of the field/property. MUST be an enum type.</typeparam>
		/// <param name="enumMemberAccessExpr">A simple member access expression for accessing the property/field you want to bind this control to. For example, <c>() => IEModOptions.YourProperty</c></param>
		/// <param name="width">The width of the dropdown. You have to specify it now because setting it is complicated.</param>
		/// <param name="labelWidth">The width of the GUI label attached to the dropdown. If you set it to 0, there will be no label.</param>
		/// <returns></returns>
		public GameObject EnumBoundDropdown<T>(Expression<Func<T>> enumMemberAccessExpr, int width, int labelWidth)
		where T : struct, IConvertible, IFormattable, IComparable{

			if (!typeof (T).IsEnum) {
				throw IEDebug.Exception(null, "Expected an enum type, but got {0}", typeof(T));
			}
			return Dropdown<T>(enumMemberAccessExpr, EnumToChoices(typeof (T)), width, labelWidth);
		}

		/// <summary>
		/// Creates a new checkbox bound to a boolean-valued property or field. Labels are taken from attributes.
		/// </summary>
		/// <param name="memberAccessExpr">A simple member access expression for accessing the property/field you want to bind this control to. For example, <c>() => IEModOptions.YourProperty</c></param>
		/// <returns></returns>
		public GameObject Checkbox(Expression<Func<bool>> memberAccessExpr) {
		/*
		 * //+ Reference
		 * 	FixBackerNamesChbox.transform.parent = ModPage;
			FixBackerNamesChbox.name = "FixBackerNames";
			FixBackerNamesChbox.transform.localScale = new Vector3 (1, 1, 1);
			FixBackerNamesChbox.transform.localPosition = new Vector3 (-210, 150, 0);
			StringId++;
			FixBackerNamesChbox.Component<UIOptionsTag> ().CheckboxLabel = new GUIDatabaseString (StringId);
			StringId++;
			FixBackerNamesChbox.Component<UIOptionsTag> ().TooltipString = new GUIDatabaseString (StringId);
			FixBackerNamesChbox.Component<UIOptionsTag> ().Checkbox.startsChecked = PlayerPrefs.GetInt ("FixBackerNames", 0) > 0 ? true : false;
			FixBackerNamesChbox.Component<UIOptionsTag> ().Checkbox.onStateChange = (UICheckbox.OnStateChange) new UICheckbox.OnStateChange((test, state) => {
		 */
			if (ExampleCheckbox == null) {
				throw IEDebug.Exception(null, "You must initialize the ExampleCheckbox to create a check box.", null);
			}
			var asMemberExpr = (MemberExpression) memberAccessExpr.Body;
			var member = asMemberExpr.Member;
			IEDebug.Log("Creating Checkbox : {0}", member.Name);
			var setter = ReflectHelper.CreateSetter(memberAccessExpr);
			var chBox = (UIOptionsTag) Object.Instantiate(ExampleCheckbox);
			
			chBox.transform.parent = CurrentParent;
			var getter = ReflectHelper.CreateGetter(memberAccessExpr);
			chBox.name = asMemberExpr.Member.Name;

			chBox.transform.localScale = new Vector3(1, 1, 1);
			chBox.transform.localPosition = new Vector3(0, 0, 0);
			var label = ReflectHelper.GetLabelInfo(member);
			var desc = ReflectHelper.GetDescriptionInfo(member);

			chBox.CheckboxLabel = IEModString.Register(label);
			chBox.TooltipString = IEModString.Register(desc);

			chBox.Checkbox.startsChecked = getter();
			chBox.Checkbox.onStateChange += (sender, state) => setter(state);
			
			IEDebug.Log("IEMod created: " + chBox.name);
			return chBox.gameObject;
		}
		public GameObject Button(string caption, string name = null, Vector3? localPos = null) {
			/* //+ Reference
			 * 	DragBtn2 = GameObject.Instantiate(DragBtn1) as GameObject;
				DragBtn2.name = "DragActionbarButton";
				DragBtn2.transform.parent = DragBtn1.transform.parent;
				DragBtn2.transform.localScale = new Vector3(1f, 1f, 1f);
				DragBtn2.transform.localPosition = DragBtn1.transform.localPosition;
				DragBtn2.transform.localPosition += new Vector3(0f, 50f, 0f);
				DragBtn2.ComponentInChildren<UIDragObject>().target = _actionBarTrimB.transform;
				DragBtn2.ComponentInChildren<UIMultiSpriteImageButton>().Label.Component<GUIStringLabel>().FormatString = "Drag Hud Bgr";
			 */
			
			if (ExampleButton == null) {
				throw IEDebug.Exception(null, "You must initialize ExampleButton to create one.");
			}
			var illegalCharsStr = " :\t\n\r/1#$%^&*().".ToCharArray().Select(char.ToString).ToArray();
			name = name ?? caption.ReplaceAll("_", illegalCharsStr);
			var newButton = (GameObject) Object.Instantiate(ExampleButton);
			newButton.name = name;
			newButton.transform.parent = CurrentParent;
			newButton.transform.localScale = Vector3.one;
			newButton.transform.localPosition = localPos ?? Vector3.zero;
			newButton.ComponentInDescendants<UIMultiSpriteImageButton>().Label.Component<GUIStringLabel>().FormatString =
				caption;
			return newButton;
		}
	}

}