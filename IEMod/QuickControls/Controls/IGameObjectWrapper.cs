
using System.Collections.Generic;
using IEMod.Helpers;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.QuickControls {
	[NewType]
	public interface IGameObjectWrapper {
		/// <summary>
		/// This property will throw an exception if the GameObject has been destroyed. Use IsAlive to check if the wrapped object is destroyed or not.
		/// </summary>
		GameObject GameObject { get;}
		/// <summary>
		/// If the GameObject was destroyed, this property will instead return the last known name of the object.
		/// </summary>
		string Name {get;set;}
		Transform Transform {get;}
		/// <summary>
		/// Returns true if the underlying GameObject is still alive and hasn't been destroyed.
		/// </summary>
		bool IsAlive { get;}
	}

	[NewType]
	public static class QuickControlExtensions {

		public static bool IsAlive(this IGameObjectWrapper gow) {
			return gow != null && gow.IsAlive;
		}

		public static IEnumerable<T> Components<T>(this IGameObjectWrapper gow) where T : Component {
			return (gow?.GameObject).Components<T>();
		}

		public static IEnumerable<T> ComponentsInDescendants<T>(this IGameObjectWrapper gow) where T : Component {
			return (gow?.GameObject).ComponentsInDescendants<T>();
		}

		public static T ComponentInDescendants<T>(this IGameObjectWrapper gow) where T : Component {
			return (gow?.GameObject).ComponentInDescendants<T>();
		}

		public static T Component<T>(this IGameObjectWrapper gow) where T : Component {
			return (gow?.GameObject).Component<T>();
		}

		public static IEnumerable<GameObject> Children(this IGameObjectWrapper gow, string name = null) {
			return (gow?.GameObject).Children(name);
		}

		public static GameObject Child(this IGameObjectWrapper gow, string name) {
			return (gow?.GameObject).Child(name);
		}

		public static GameObject Descendant(this IGameObjectWrapper gow, string name) {
			return (gow?.GameObject).Descendant(name);
		}

		public static void AddComponent<T>(this IGameObjectWrapper gow) where T : Component {
			if (gow == null) {
				throw IEDebug.Exception(null, "GameObject cannot be null");
			}
			gow.GameObject.AddComponent<T>();
		}

		public static IEnumerable<GameObject> Descendants(this IGameObjectWrapper gow, string name = null) {
			return (gow?.GameObject).Descendants(name);
		}

		public static bool HasComponent<T>(this IGameObjectWrapper gow) where T : Component {
			return (gow?.GameObject).HasComponent<T>();
		}
	}
}