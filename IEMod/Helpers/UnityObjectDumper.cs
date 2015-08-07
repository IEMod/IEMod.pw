using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IEMod.Helpers {
	/// <summary>
	/// Prints Unity.GameObjects, including children and components. 
	/// </summary>
	[NewType]
	public class UnityObjectDumper {
		private UnityObjectDumper() { }
		private readonly IDictionary<object, bool> _visited = new Dictionary<object, bool>();

		/// <summary>
		/// Prints a UnityEngine.GameObject, along with its components and children. Prints properties of components, but not of GameObjects, and doesn't print GameObjects in properties.
		/// </summary>
		/// <param name="o">The game object to print</param>
		/// <param name="componentFilter">A filter that says which components to print fully (printing their properties). By default, all will be expanded.</param>
		/// <param name="gameObjectFilter">A filter that says which GameObject children to print recursively (e.g. print their components and their children). Only the names of filtered children will appear.</param>
		/// <returns></returns>
		public static string PrintUnityGameObject(GameObject o, Func<GameObject, bool> gameObjectFilter = null, Func<Component, bool> componentFilter = null) {
			componentFilter = componentFilter ?? (x => true);
			gameObjectFilter = gameObjectFilter ?? (x => true);
			var dumper = new UnityObjectDumper();
			var strWriter = new StringWriter();
			var identedWriter = new IndentedTextWriter(strWriter, "\t");
			dumper.PrintUnityGameObject(o, identedWriter,gameObjectFilter, componentFilter);
			identedWriter.Flush();
			strWriter.Flush();
			return strWriter.ToString();
		}

		private void DumpValue(IndentedTextWriter writer, string key, Func<object> getter) {
			object o;
			try {
				o = getter();
			}
			catch (Exception ex) {
				writer.WriteLine("{0} = !! {1} !! //an exception occured while evaluating this", key, ex.GetType());
				return;
			}
			if (o == null) {
				writer.WriteLine("{0} = {1}", key, "(null)");
				return;
			}
			var valueString = o.ToString();
			var lines = valueString.Split(new[] {
				'\r',
				'\n'
			},
				StringSplitOptions.RemoveEmptyEntries).ToList();

			if (lines.Count <= 1) {
				writer.WriteLine("{0} = {1}", key, valueString);
			} else {
				writer.WriteLine("{0} = ", key);
				writer.Indent++;
				lines.ForEach(writer.WriteLine);
				writer.Indent--;
			}
		}

		private void PrintObjectMembers(IndentedTextWriter writer, object o) {
			if (o == null) {
				writer.WriteLine("(null)");
				return;
			}
			var type = o.GetType();
			var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var prop in props) {
				var propType = prop.PropertyType;
				DumpValue(writer, string.Format("+p {0} {1}", propType.Name, prop.Name), () => prop.GetValue(o, null));
			}
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in fields) {
				var propType = field.FieldType;
				DumpValue(writer, string.Format("+p {0} {1}", propType.Name, field.Name), () => field.GetValue(o));
			}
		}

		private void PrintUnityGameObject(GameObject o, IndentedTextWriter writer, Func<GameObject, bool> gameObjectFilter, Func<Component, bool> componentFilter) {
			if (o == null) {
				writer.WriteLine("(null)");
				return;
			}
			
			writer.WriteLine("{0} : {1}", o.GetType(), o.name);
			if (_visited.ContainsKey(o)) {
				writer.Write(" (already dumped)");
				return;
			}
			writer.Indent++;
			writer.WriteLine("Components:");
			writer.Indent++;
			
			_visited[o] = true;
			foreach (var component in o.GetComponents(typeof (Component))) {
				writer.WriteLine("[Component] {0}", component.GetType().Name);
				if (!componentFilter(component)) continue;
				writer.Indent++;
				PrintObjectMembers(writer, component);
				writer.Indent--;
			}
			writer.Indent--;
			writer.WriteLine("Children:");

			var numChildren = o.transform.childCount;
			for (int i = 0; i < numChildren; i++) {
				var child = o.transform.GetChild(i);
				writer.Indent++;
				writer.Write("{0}. [Child] ", i);
				if (gameObjectFilter(child.gameObject)) {
					PrintUnityGameObject(child.gameObject, writer, gameObjectFilter, componentFilter);	
				}
				
				writer.Indent--;
			}
			writer.Indent--;
		}
	}
}