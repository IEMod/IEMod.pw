using System;
using System.Collections.Generic;
using System.Linq;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Helpers {
	/// <summary>
	/// Various helper extension methods for common tasks.
	/// </summary>
	[NewType]
	public static class UnityObjectExtensions {

		public static Vector3 TransformOnly(this Vector3 vector, int index, Func<float, float> f) {
			switch (index) {
				case 0:
					vector.x = f(vector.x);
					break;
				case 1:
					vector.y = f(vector.y);
					break;
				case 2:
					vector.z = f(vector.z);
					break;
			}
			return vector;
		}

		public static IEnumerable<GameObject> GetChildren(this GameObject o) {
			return
				from child in o.transform.Cast<Transform>()
				select child.gameObject;
		} 

		public static IEnumerable<GameObject> GetDescendants(this GameObject o) {
			return
				from child in o.transform.Cast<Transform>()
				let childObject = child.gameObject
				from child2 in new[] {childObject}.Concat(child.gameObject.GetDescendants())
				select child2;
		}

		public static IEnumerable<GameObject> GetDescendants(this GameObject o, string name) {
			return
				from desc in o.GetDescendants()
				where string.Equals(desc.name, name, StringComparison.OrdinalIgnoreCase)
				select desc;
		}

		public static GameObject GetDescendant(this GameObject o, string name) {
			return o.GetDescendants(name).Single();
		}
	}
}