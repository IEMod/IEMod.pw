using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEMod.Helpers;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.QuickControls {
	[NewType]
	public static class Prefabs {

		public static UIOptionsTag QuickCheckbox {
			get {
				if (_quickCheckbox == null) {
					throw NotInitialized("QuickCheckbox");
				}
				return _quickCheckbox;
			}
			set {
				_quickCheckbox = value;
			}
		}

		private static IEModException NotInitialized(string what) {

			return IEDebug.Exception(null, "The {0} prefab hasn't been initialized! You have to initialize it before creating any {0}s.", what);
		}

		public static GameObject QuickDropdown {
			get {
				if (_quickCheckbox == null) {
					throw NotInitialized("QuickDropdown");
				}
				return _quickDropdown;
			}
			set {
				_quickDropdown = value;
			}
		}

		public static GameObject QuickButton {
			get {
				if (_quickButton == null) {
					throw NotInitialized("QuickButton");
				}
				return _quickButton;
			}
			set {

				_quickButton = value;
			}
		}

		public static GameObject QuickPage {
			get {
				if (_quickButton == null) {
					throw NotInitialized("Page");
				}
				return _page;
			}
			set {
				_page = value;
			}
		}

		private static UIOptionsTag _quickCheckbox;
		private static GameObject _quickDropdown;
		private static GameObject _quickButton;
		private static GameObject _page;
	}
}
