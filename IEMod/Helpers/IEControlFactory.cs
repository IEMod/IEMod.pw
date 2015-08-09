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

namespace IEMod.Helpers {

	/// <summary>
	/// This class allows you to construct standard controls quickly and easily, obscuring any ugliness the task involves.
	/// </summary>
	[NewType]
	public class IEControlFactory {


		public GameObject ExampleCheckbox;
		public GameObject ExampleComboBox;
		public Transform CurrentParent;
		public GameObject ExamplePage;
		

		private IEComboBoxChoice[] EnumToChoices(Type enumType) {
			var list = new List<IEComboBoxChoice>();

			foreach (var value in Enum.GetValues(enumType)) {
				var field = enumType.GetField(value.ToString());
				var attr = field.GetCustomAttribute<DescriptionAttribute>();
				var label = attr == null ? null : attr.Description;
				label = label ??  value.ToString();
				list.Add(new IEComboBoxChoice(value, label));
			}
			return list.ToArray();
		}

		private string GetLabel(MemberInfo provider) {
			var labelAttr = provider.GetCustomAttribute<LabelAttribute>();
			var label = labelAttr == null ? null : labelAttr.Label;
			label = label ?? provider.Name;
			return label;
		}

		private string GetDesc(MemberInfo provider) {
			var descAttr = provider.GetCustomAttribute<DescriptionAttribute>();
			var desc = descAttr == null ? null : descAttr.Description;
			desc = desc ?? provider.Name;
			return desc;
		}

		/// <summary>
		/// Creates a new Page object, for the properties screen. ExamplePage must be set before calling this.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public GameObject Page(string name) {
			if (ExamplePage == null) {
				IEDebug.Exception(null, "You must initialize the ExamplePage to create a Page", null);
			}
			var newPage = new GameObject ();
			
			newPage.transform.localScale =  ExamplePage.transform.localScale;
			newPage.transform.localPosition =  ExamplePage.transform.localPosition;
			newPage.name = name;
			newPage.transform.parent = ExamplePage.transform.parent;
			return newPage;
		}

		/// <summary>
		/// Creates a new dropdown/combo box bound to an enum-valued field or property. The enum part is important, as this is how the dropdown's options are generated. Labels are taken from attributes.
		/// </summary>
		/// <typeparam name="T">The type of the field/property. MUST be an enum type.</typeparam>
		/// <param name="enumMemberAccessExpr">A simple member access expression for accessing the property/field you want to bind this control to. For example, <c>() => IEModOptions.YourProperty</c></param>
		/// <param name="width">The width of the dropdown. You have to specify it now because setting it is complicated.</param>
		/// <param name="labelWidth">The width of the GUI label attached to the dropdown. If you set it to 0, there will be no label.</param>
		/// <param name="localPos">The transform.localPosition of the dropdown. You can set this later.</param>
		/// <returns></returns>
		public GameObject EnumBoundDropdown<T>(Expression<Func<T>> enumMemberAccessExpr, int width, int labelWidth)
		where T : struct, IConvertible, IFormattable, IComparable{
			/*
			 *
			//+ FOR REFERENCE:
			const int nerfedXPTableWidth = 515;
			UIDropdownMenu drop;
			nerfedXPTableDropdown.transform.GetChild(1).GetChild(0).localScale = new Vector3(nerfedXPTableWidth, 32, 1); // width of combobox
			nerfedXPTableDropdown.transform.GetChild(1).GetChild(3).localPosition = new Vector3(nerfedXPTableWidth - 27, 10, 0); // position of down arrow
			nerfedXPTableDropdown.transform.GetChild(1).GetChild(2).GetChild(0).localScale = new Vector3(nerfedXPTableWidth, 37, 0); // width of dropdown
			nerfedXPTableDropdown.transform.GetChild(1).GetComponent<UIDropdownMenu>().OptionRootText.lineWidth = nerfedXPTableWidth;
			nerfedXPTableDropdown.transform.GetChild(1).GetComponent<UIDropdownMenu>().SelectedText.lineWidth = nerfedXPTableWidth;
			nerfedXPTableDropdown.transform.GetChild(1).GetComponent<UIDropdownMenu>().OptionGrid.cellWidth = nerfedXPTableWidth;
			nerfedXPTableDropdown.transform.GetChild(1).GetComponent<UIDropdownMenu>().Options = nerfedXPTableChoices;
			//nerfedXPTableDropdown.transform.GetChild(1).GetComponent<UIDropdownMenu>().SelectedItem = nerfedXPTableChoices[PlayerPrefs.GetInt("NerfedXPTableSetting")];
			//nerfedXPTableDropdown.transform.GetChild(1).GetComponent<UIDropdownMenu>().OnDropdownOptionChangedEvent += new UIDropdownMenu.DropdownOptionChanged(OnNerfedXPTableSettingChanged);
			nerfedXPTableDropdown.transform.GetChild(0).GetComponent<UILabel>().lineWidth = 300;
			nerfedXPTableDropdown.transform.GetChild(0).GetComponent<UILabel>().shrinkToFit = false;
			nerfedXPTableDropdown.transform.GetChild(0).GetComponent<GUIStringLabel>().DatabaseString = new GUIDatabaseString(++StringId);
			nerfedXPTableDropdown.GetComponent<UIOptionsTag>().TooltipString = new GUIDatabaseString(++StringId);
			// end of adding dropdown
			 */
			if (!typeof (T).IsEnum) {
				IEDebug.Exception(null, "Expected an enum type, but got {0}", typeof(T));
			}
			if (ExampleComboBox == null) {
				IEDebug.Exception(null, "You must initialize the ExampleComboBox to create a combo box.", null);
			}
			var pos = new Vector3(0, 0, 0);
			var getter = ReflectHelper.CreateGetter(enumMemberAccessExpr);
			var setter = ReflectHelper.CreateSetter(enumMemberAccessExpr);
			var asMemberExpr = (MemberExpression) enumMemberAccessExpr.Body;
			var comboBox = (GameObject) GameObject.Instantiate(ExampleComboBox);

			//+ Basic setup
					
			comboBox.name = asMemberExpr.Member.Name;
			//! You have to explicitly set localPosition and localScale to something after Instantiate!!!
			//! Otherwise, the UI will broken, but no exception will be reported.
			comboBox.transform.localPosition = pos;
			comboBox.transform.localScale = new Vector3(1, 1, 1);

			var dropdown = comboBox.transform.GetComponentsInChildren<UIDropdownMenu>(true).Single();
			var label = GetLabel(asMemberExpr.Member);
			var desc = GetDesc(asMemberExpr.Member);
			
			//+ Set the labels
			//There are multiple things called Label in this visual tree, but only one with a GUIStringLabel component.
			var guiLabel = comboBox.transform.GetComponentsInChildren<GUIStringLabel>(true).Single();
			if (labelWidth > 0) {
				guiLabel.DatabaseString = IEModString.Register(label);
				var uiLabel = guiLabel.gameObject.GetComponent<UILabel>();
				uiLabel.lineWidth = labelWidth;
			} else {
				guiLabel.DatabaseString = IEModString.Register("");
				var uiLabel = guiLabel.gameObject.GetComponent<UILabel>();
				uiLabel.lineWidth = labelWidth;
			}
			
			var uiTag = comboBox.GetComponent<UIOptionsTag>();
			uiTag.TooltipString = IEModString.Register(desc);

			//+ set the choices and SelectedItem
			var choices = EnumToChoices(typeof (T));
			dropdown.Options = choices.Cast<object>().ToArray();
			dropdown.SelectedItem = choices.SingleOrDefault(x => x.Value.Equals(getter())) ?? dropdown.Options[0];

			//+ Set position and scale
			//To get the names, I used the GetChild calls as reference, and dumped the comboBox to the log using UnityObjectDumper.
			var comboBoxBackground = comboBox.GetDescendant("Background");
			var oldWidth = comboBoxBackground.transform.localScale.x;
			comboBoxBackground.transform.localScale = new Vector3(width, 32, 1); //this is the width of the combobox
			
			var arrowThing = comboBox.GetDescendant("ArrowPivot");
			arrowThing.transform.localPosition = new Vector3(width - 27, 10, 0);

			dropdown.OptionRootText.lineWidth = width;
			dropdown.SelectedText.lineWidth = width;
			dropdown.OptionGrid.cellWidth = width;
			var optGrid = dropdown.OptionGrid;
			//+ Set the default handler that binds the control to the member
			dropdown.OnDropdownOptionChangedEvent += option => {
				var asChoice = (IEComboBoxChoice) option;
				setter((T) asChoice.Value);
			};
			comboBox.transform.parent = CurrentParent;	
			return comboBox;
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
			FixBackerNamesChbox.GetComponent<UIOptionsTag> ().CheckboxLabel = new GUIDatabaseString (StringId);
			StringId++;
			FixBackerNamesChbox.GetComponent<UIOptionsTag> ().TooltipString = new GUIDatabaseString (StringId);
			FixBackerNamesChbox.GetComponent<UIOptionsTag> ().Checkbox.startsChecked = PlayerPrefs.GetInt ("FixBackerNames", 0) > 0 ? true : false;
			FixBackerNamesChbox.GetComponent<UIOptionsTag> ().Checkbox.onStateChange = (UICheckbox.OnStateChange) new UICheckbox.OnStateChange((test, state) => {
		 */
			if (ExampleCheckbox == null) {
				IEDebug.Exception(null, "You must initialize the ExampleCheckbox to create a check box.", null);
			}
			var asMemberExpr = (MemberExpression) memberAccessExpr.Body;
			var member = asMemberExpr.Member;
			IEDebug.Log("Creating Checkbox : {0}", member.Name);
			var setter = ReflectHelper.CreateSetter(memberAccessExpr);
			var chBox = (GameObject) GameObject.Instantiate(ExampleCheckbox);
			

			var getter = ReflectHelper.CreateGetter(memberAccessExpr);
			chBox.name = asMemberExpr.Member.Name;
			
			var uiTag = chBox.GetComponent<UIOptionsTag>();
			chBox.transform.localScale = new Vector3(1, 1, 1);
			chBox.transform.localPosition = new Vector3(0, 0, 0);
			var label = GetLabel(member);
			var desc = GetDesc(member);

			uiTag.CheckboxLabel = IEModString.Register(label);
			uiTag.TooltipString = IEModString.Register(desc);

			uiTag.Checkbox.startsChecked = getter();
			uiTag.Checkbox.onStateChange += (sender, state) => setter(state);
			chBox.transform.parent = CurrentParent;
			IEDebug.Log("IEMod created: " + chBox.name);
			return chBox;
		}
	}

}