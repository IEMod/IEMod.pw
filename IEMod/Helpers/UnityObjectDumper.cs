using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IEMod.Helpers {


	/// <summary>
	/// Prints various unity game objects using configurable settings.
	/// </summary>
	[NewType]
	public class UnityPrinter {
		[NewType]
		private class RecursiveObjectPrinter {
			private readonly IndentedTextWriter _writer;
			private readonly UnityPrinter _parent;
			private readonly IDictionary<object, bool> _visited = new Dictionary<object, bool>();

			public RecursiveObjectPrinter(TextWriter writer, UnityPrinter parent) {
				_parent = parent;
				_writer = new IndentedTextWriter(writer);
			}

			private void PrintValue(string key, Func<object> getter) {
				object o;
				try {
					o = getter();
				}
				catch (Exception ex) {
					_writer.WriteLine("{0} = !! {1} !! //an exception occured while evaluating this", key, ex.GetType());
					return;
				}
				if (o == null) {
					_writer.WriteLine("{0} = {1}", key, "(null)");
					return;
				}

				var valueString = o.ToString();
				var lines = valueString.Split(new[] {
					'\r',
					'\n'
				},
					StringSplitOptions.RemoveEmptyEntries).ToList();

				if (lines.Count <= 1) {
					_writer.WriteLine("{0} = {1}", key, valueString);
				} else {
					_writer.WriteLine("{0} = ", key);
					_writer.Indent++;
					lines.ForEach(_writer.WriteLine);
					_writer.Indent--;
				}
			}

			private void PrintObjectMembers(object o) {
				if (o == null) {
					_writer.WriteLine("(null)");
					return;
				}
				var type = o.GetType();
				var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				foreach (var prop in props) {
					var propType = prop.PropertyType;
					PrintValue(string.Format("{0} {1}", propType.Name, prop.Name), () => prop.GetValue(o, null));
				}
				var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (var field in fields) {
					var propType = field.FieldType;
					PrintValue(string.Format("{0} {1}", propType.Name, field.Name), () => field.GetValue(o));
				}
			}

			public void PrintObject(Object o, int recursionDepth = 0) {
				var asComponent = o as Component;
				var asGameObject = o as GameObject;
				if (asComponent == null && asGameObject == null) {
					_writer.WriteLine("{0} : {1}", o.GetType(), o.name);
					_writer.Indent++;
					PrintObjectMembers(o);
					_writer.Indent--;
					return;
				}
				if (asComponent != null) {
					asGameObject = asComponent.gameObject;
				}
				PrintUnityGameObject(asGameObject, recursionDepth);
			}

			public void PrintUnityGameObject(GameObject o, int recursionDepth = 0) {
				if (o == null) {
					_writer.WriteLine("(null)");
					return;
				}
				_writer.Write("{0} : {1}", o.GetType(), o.name);
				if (_visited.ContainsKey(o)) {
					_writer.WriteLine(" (already dumped)");
					return;
				}
				if (recursionDepth >= _parent.MaxRecursionDepth) {
					_writer.WriteLine(" (recursion depth exceeded)");
					return;
				}
				_writer.WriteLine();
				_writer.Indent++;
				_writer.WriteLine($"Parent: {o.transform.parent?.name}");
				_writer.WriteLine("Components:");
				_writer.Indent++;

				_visited[o] = true;
				foreach (var component in o.Components(typeof (Component))) {
					_writer.WriteLine("[Component] {0}", component.GetType().Name);
					if (!_parent.ComponentFilter(component)) continue;
					_writer.Indent++;
					PrintObjectMembers(component);
					_writer.Indent--;
				}
				_writer.Indent--;
				_writer.WriteLine("Children:");

				var numChildren = o.transform.childCount;
				for (int i = 0; i < numChildren; i++) {
					var child = o.transform.GetChild(i);
					_writer.Write("{0}.\t [Child] ", i);
					if (_parent.GameObjectFilter(child.gameObject)) {
						PrintUnityGameObject(child.gameObject, recursionDepth + 1);
					}
				}
				_writer.Indent--;
			}
		}

		/// <summary>
		/// A filter that determines which GameObject descendants will be expanded.
		/// </summary>
		public Func<GameObject, bool> GameObjectFilter {
			get;
			set;
		}

		/// <summary>
		/// A filter that determines which Components will be expanded (printed with properties).
		/// </summary>
		public Func<Component, bool> ComponentFilter {
		 get;
			set;
		}

		/// <summary>
		/// Max recursion depth when printing descendants. 0 means only the current object is expanded.
		/// </summary>
		public int MaxRecursionDepth {
		 get;
			set;
		}

		/// <summary>
		/// The millisecond interval between print calls to print the same object. This stops the same object being printed too frequently.
		/// </summary>
		public double MillisecondInterval {
		 get;
			set;
		}

		private readonly IDictionary<object, double> _timestamps = new Dictionary<object, double>();

		/// <summary>
		/// Preconfigured printed that prints all descendants, their respective components, and the properties and fields of those components. Prints the same object once per second.
		/// </summary>
		/// <remarks>
		/// Note that for large objects this printer can take some time, and produces files that are difficult to read.
		/// </remarks>
		public static UnityPrinter FullPrinter = new UnityPrinter() {
			MillisecondInterval = 1000
		};

		/// <summary>
		/// Preconfigured printer that prints all descendants, but doesn't expand any components. Good for viewing the hierarchy. Prints the same object up to once per second.
		/// </summary>
		public static UnityPrinter HierarchyPrinter = new UnityPrinter() {
			ComponentFilter = x => false,
			MillisecondInterval = 1000
		};

		/// <summary>
		/// Preconfigured printer that prints the object only (it doesn't expand components or children). Prints the same object up to once per second.
		/// </summary>
		public static readonly UnityPrinter ShallowPrinter = new UnityPrinter() {
			ComponentFilter = x => false,
			MaxRecursionDepth = 1,
			MillisecondInterval = 1000
		};

		/// <summary>
		/// Preconfigured printer that fully expands the object's components, but does not expand its children.
		/// </summary>
		public static readonly UnityPrinter ComponentPrinter = new UnityPrinter() {
			ComponentFilter = x => true,
			MaxRecursionDepth = 1,
			MillisecondInterval = 1000
		};

		/// <summary>
		/// Use the initializer syntax to set properties.
		/// </summary>
		public UnityPrinter() {
			GameObjectFilter = x => true;
			ComponentFilter = x => true;
			MaxRecursionDepth = 1024;
			MillisecondInterval = 0;
		}

		public void Print(Object o) {
			IEDebug.Log(PrintString(o));
		}

		public string PrintString(Object o) {
			var newTime = TimeSpan.FromTicks(Environment.TickCount).TotalMilliseconds;
			if (_timestamps.ContainsKey(o)) {
				var lastTime = _timestamps[o];
				if (newTime - lastTime < MillisecondInterval) {
					return "";
				}
			}
			_timestamps[o] = newTime;
			var strWriter = new StringWriter();
			var indentedWriter = new IndentedTextWriter(strWriter);
			var printer = new RecursiveObjectPrinter(indentedWriter, this);
			printer.PrintObject(o);
			indentedWriter.Flush();
			strWriter.Flush();
			_timestamps[o] = TimeSpan.FromTicks(Environment.TickCount).TotalMilliseconds;
			return strWriter.ToString();
		}


	}

}