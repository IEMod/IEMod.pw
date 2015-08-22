using System;
using IEMod.Helpers;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.QuickControls {
	[NewType]
	public abstract class QuickControl : IGameObjectWrapper {
		private string _name;
		private GameObject _gameObject;

		bool IGameObjectWrapper.IsAlive {
			get {
				return _gameObject;
			}
		}

		protected void AssertAlive() {
			if (!this.IsAlive()) {
				throw new ObjectDisposedException(_name,
					$"Tried to access the GameObject probably named '{_name}' backing a QuickControl, but it has been destroyed. GameObject's InstanceId: {_gameObject.GetInstanceID()}");
			}
		}

		public static bool operator true(QuickControl qc) {
			return qc.IsAlive();
		}

		public static bool operator false(QuickControl qc) {
			return !qc.IsAlive();
		}

		public static implicit operator GameObject(QuickControl qc) {
			return qc.GameObject;
		}

		public static implicit operator Transform(QuickControl qc) {
			return qc.Transform;
		}

		public GameObject GameObject {
			get {
				AssertAlive();
				return _gameObject;
			}
			protected set {
				_gameObject = value;
			}
		}

		public int Layer {
			get {
				
				
				return GameObject.layer;
			}
			set {
				
				GameObject.layer = value;
				NGUITools.SetLayer(GameObject, value);
			}
		}

		public string Name {
			get {
				if (this.IsAlive()) {
					return _name = GameObject.name;
				}
				return _name;
			}
			set {
				
				GameObject.name = _name = value;
			}
		}

		public Transform Transform {
			get {
				
				return GameObject.transform;
			}
		}

		public Transform Parent {
			get {
				
				return Transform.parent;
			}
			set {
				
				Transform.parent = value;
			}
		}

		public Vector3 LocalPosition {
			get {
				
				return GameObject.transform.localPosition;
			}
			set {
				
				GameObject.transform.localPosition = value;
			}
		}

		public Vector3 LocalScale {
			get {
				
				return GameObject.transform.localScale;
			}
			set {
				
				GameObject.transform.localScale = value;
			}
		}

		public void SetActive(bool state) {
			
			GameObject.SetActive(state);
		}

		public bool ActiveSelf {
			get {
				
				return GameObject.activeSelf;
			}
		}

	}
}