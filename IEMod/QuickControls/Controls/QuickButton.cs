using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEMod.Helpers;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IEMod.QuickControls {

	[NewType]
	public class QuickButton : QuickControl {
		private GameObject _collider;

		public event Action<QuickButton> Click;

		public event Action<QuickButton, bool> Press;
		public QuickButton(Transform parent = null, string name = "QuickButton", GameObject altPrototype = null, bool addCollider = true) {
			//What happens with controls linked to the UI is pretty strange.
			//At first, when you add them, you can see they have no colliders. You can see this by printing out their children, etc.
			//However, later when you check them using the object browser, you see they suddenly have colliders as though by magic!
			//Apparently, at some point a pixie comes along and gives every control its collider(s),
			//This is annoying, as we want to manipulat the collider, such as attach UIDRagObjects to it and other things.
			//so we just add it from the start. 
			var exampleButton = altPrototype ?? Prefabs.QuickButton;
			var newButton = (GameObject) Object.Instantiate(exampleButton);
			newButton.name = name;
			newButton.transform.parent = parent;
			newButton.transform.localScale = Vector3.one;
			newButton.transform.localPosition = Vector3.zero;
			GameObject = newButton;
			ButtonComponent.onClick += go => RaiseClick(this);
			ButtonComponent.onPress += (go, state) => RaisePress(this, state);

			//sometimes the button already comes with a collider (because the pixie already visited it)
			newButton.Children().Where(x => x.HasComponent<Collider>()).ToList().ForEach(Object.DestroyImmediate);

			//this is important. The pixie is kind of lazy and just checks for GameObjects named "Collider".
			//if we name it something else, the thing will end up with 2 colliders, which breaks it.
			if (addCollider) {
				_collider = new GameObject("Collider");
				_collider.transform.parent = newButton.transform;
				_collider.transform.localScale = new Vector3(269f, 56f, 1f);
				_collider.transform.localPosition = new Vector3(0f, 0, -2f);
				_collider.layer = 14;
				//without a BoxCollider + UINoClick, clicks will go through the control.
				_collider.AddComponent<BoxCollider>().size = new Vector3(1, 1, 1);
				_collider.AddComponent<UINoClick>().BlockClicking = true;
				_collider.AddComponent<UIEventListener>();
			}

			IEDebug.Log("Created: " + name);
		}

		public string Caption {
			get {
				return GameObject.ComponentInDescendants<GUIStringLabel>().FormatString;
			}
			set {
				var uiLabel = GameObject.ComponentInDescendants<GUIStringLabel>();
				uiLabel.FormatString = value;
				uiLabel.RefreshText();
			}
		}

		public GameObject Collider {
			get {
				AssertAlive();
				return _collider ?? (_collider = GameObject.Component<UIMultiSpriteImageButton>().Collider);
			}
		}

		public UIMultiSpriteImageButton ButtonComponent {
			get {
				
				return GameObject.ComponentInDescendants<UIMultiSpriteImageButton>();
			}
		}


		protected virtual void RaiseClick(QuickButton obj) {
			
			var handler = Click;
			handler?.Invoke(obj);
		}

		protected virtual void RaisePress(QuickButton arg1, bool arg2) {
			var handler = Press;
			handler?.Invoke(arg1, arg2);
		}
	}
}
