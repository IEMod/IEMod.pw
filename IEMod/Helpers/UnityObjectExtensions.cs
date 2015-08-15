using System;
using System.Collections.Generic;
using System.Linq;
using IEMod.Mods.StringTable;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IEMod.Helpers {

	[NewType]
	public static class ControlHelper {

		/// <summary>
		/// Sets the selected value for dropdowns that use IEDropdownChoice.
		/// </summary>
		/// <param name="dropdown"></param>
		/// <param name="value"></param>
		public static void SetSelectedValue(this UIDropdownMenu dropdown, object value) {
			dropdown.SelectedItem = dropdown.Options.Cast<IEDropdownChoice>().SingleOrDefault(x => Equals(x.Value, value))
				?? dropdown.Options[0];
		}
	}

	/// <summary>
	/// Various helper extension methods for common tasks involving navigating the unity GameObject and Component model. 
	/// </summary>
	[NewType]
	public static class UnityObjectExtensions {

		public static void MaybeUnregister(this GUIDatabaseString str) {
			var asIEString = str as IEModString;
			if (asIEString != null) {
				asIEString.Unregister();
			}

		}
		public static Vector3 ScaleBy(this Vector3 self, float scalar) {
			self.Set(self.x * scalar, self.y * scalar, self.z * scalar);
			return self;
		}

		/// <summary>
		/// Returns the child at the specified path, where 'path' is a string like name1/name2/name3. You can use #N/name1/#M, where #N is child number N.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static GameObject ChildPath(this GameObject start, string path) {
			var names = path.Split('/');
			GameObject current = start;
			foreach (var curName in names) {
				var name = curName;
				if (name == "") continue;
				int asInt;
				var hasHashFirst = name.StartsWith("#");
				if (hasHashFirst) {
					name = name.Substring(1);
				}
				current = hasHashFirst && int.TryParse(name, out asInt) ? current.Child(asInt) : current.Child(name);
			}
			return current;
		}

		/// <summary>
		/// Does it have a child with this name?
		/// </summary>
		/// <param name="o"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool HasChild(this GameObject o, string name) {
			return o.Children(name).Any();
		}

		/// <summary>
		/// Does it have a component of this type?
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o"></param>
		/// <returns></returns>
		public static bool HasComponent<T>(this GameObject o) where T : Component {
			return o.GetComponent<T>() != null;
		}

		public static Component[] Components(this GameObject o, Type t = null) {
			if (o == null) {
				throw IEDebug.Exception(null, "GameObject cannot be null.");
			}
			return o.GetComponents(t ?? typeof(Component));
		}

		public static T[] Components<T>(this GameObject o)
			where T : Component {
			return o.Components(typeof (T)).Cast<T>().ToArray();
		}

		 
		public static T Component<T>(this GameObject o) where T : Component {
			if (o == null) {
				throw IEDebug.Exception(null, "GameObject cannot be null.");
			}
			var components = o.Components<T>();
			if (components.Length > 1 || components.Length == 0) {
				IEDebug.Log(UnityPrinter.ShallowPrinter.Print(o));
				throw IEDebug.Exception(null, "GameObject '{0}' has {1} components of type {2}, but told to pick exactly one.",
					o.name, components.Length, typeof (T));
			}
			return components[0];
		}

		public static T[] ComponentsInDescendants<T>(this Component c, bool inactive = true) where T : Component {
			if (c == null) {
				throw IEDebug.Exception(null, "Component cannot be null.");
			}
			return c.gameObject.ComponentsInDescendants<T>(inactive);
		}

		public static T ComponentInDescendants<T>(this Component c, bool inactive = true) where T : Component {
			if (c == null) {
				throw IEDebug.Exception(null, "Component cannot be null.");
			}
			return c.gameObject.ComponentInDescendants<T>(inactive);
		}

		public static T[] ComponentsInDescendants<T>(this GameObject o, bool inactive = true) where T : Component {
			if (o == null) {
				throw IEDebug.Exception(null, "GameObject cannot be null.");
			}
			return o.GetComponentsInChildren<T>(inactive);
		}

		public static T ComponentInDescendants<T>(this GameObject o, bool inactive = true) where T : Component {
			if (o == null) {
				throw IEDebug.Exception(null, "GameObject cannot be null.");
			}
			var components = o.ComponentsInDescendants<T>(inactive);
			if (components.Length == 0 || components.Length > 1) {
				throw IEDebug.Exception(null,
					"GameObject '{0}' has {1} components of type {2} in its children, but told to pick exactly one.", o.name,
					components.Length, typeof (T));
			}
			return components[0];
		}


		public static IList<T> Components<T>(this Component o)
			where T : Component {
			if (o == null) {
				throw IEDebug.Exception(null, "Component cannot be null.");
			}
			return o.gameObject.Components<T>();
		}

		public static T Component<T>(this Component o) where T : Component {
			if (o == null) {
				throw IEDebug.Exception(null, "Component cannot be null.");
			}
			return o.gameObject.Component<T>();
		}

		/// <summary>
		/// Returns a child with this name.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static GameObject Child(this Component c, string name) {
			return c.gameObject.Child(name);
		}


		/// <summary>
		/// Gets the child with this name, or throws an exception.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static GameObject Child(this GameObject o, string name) {
			if (o == null) {
				throw IEDebug.Exception(null, "When trying to get Child with name '{0}': gameObject cannot be null.", name);
			}
			var seq = o.Children().Where(x => x.gameObject.name == name).ToList();
			if (seq.Count == 0 || seq.Count > 1) {
				IEDebug.Log(UnityPrinter.ShallowPrinter.Print(o));
				throw IEDebug.Exception(null, 
					"GameObject '{0}' has {1} children with the name '{2}', but told to pick exactly one.", o.name, seq.Count, name);
			}
			return seq[0];
		}

		/// <summary>
		/// Adds the specified values to the vector's components. Use named arguments to add to specific components.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public static Vector3 Plus(this Vector3 self, float x = 0f, float y = 0f, float z = 0f) {
			return new Vector3(self.x + x, self.y + y, self.z + z);
		}
		/// <summary>
		/// Adds the specified values to the vector's components. Use named arguments to add to specific components.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Vector2 Plus(this Vector2 self, float x = 0f, float y = 0f) {
			return new Vector2(self.x + x, self.y + y);
		}

		/// <summary>
		/// Adds the specified values to the vector's components. Use named arguments to add to specific components.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="w"></param>
		/// <returns></returns>
		public static Vector4 Plus(this Vector4 self, float x = 0f, float y = 0f, float z = 0f, float w = 0f) {
			return new Vector4(self.x + x, self.y + y, self.z + z, self.w + w);
		}

		/// <summary>
		/// Returns the ith child of this game object, or throws an exception.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public static GameObject Child(this GameObject o, int i) {
			if (o == null) {
				throw IEDebug.Exception(null, "The GameObject cannot be null.");
			}
			var child = o.transform.GetChild(i);
			if (child == null) {
				throw IEDebug.Exception(null, "The GameObject '{0}' has no child at index {1}", o.name, i);
			}
			return child.gameObject;
		}

		public static IEnumerable<GameObject> Children(this GameObject o, string name = null) {
			
			return
				from child in o.transform.Cast<Transform>()
				where name == null || name == child.name
				select child.gameObject;
		} 

		public static IEnumerable<GameObject> Descendants(this GameObject o, string name = null) {
			return o.ComponentsInDescendants<Transform>().Select(x => x.gameObject).Where(x => x.name == name);
		}

		public static GameObject Descendant(this GameObject o, string name) {
			
			if (o == null) {
				throw IEDebug.Exception(null, "GameObject cannot be null.");
			}
			var descendants = o.Descendants(name);
			var gameObjects = descendants.ToArray();
			if (gameObjects.Length == 0 || gameObjects.Length > 1) {
				throw IEDebug.Exception(null,
					"Game object '{0}' has {1} descendants with the name '{2}', but told to pick exactly one.", o.name,
					gameObjects.Count(), name);
			}
			return gameObjects[0];
		}

		public static IEnumerable<GameObject> Descendants(this Component o, string name = null) {
			if (o == null) {
				throw IEDebug.Exception(null, "Component cannot be null.");
			}
			return o.gameObject.Descendants(name);
		}

		public static GameObject Descendant(this Component o, string name) {
			
			if (o == null) {
				throw IEDebug.Exception(null, "GameObject cannot be null.");
			}
			return o.gameObject.Descendant(name);

		}
	}
}