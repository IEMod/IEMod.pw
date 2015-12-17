using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Patchwork.Attributes;

namespace IEMod.Helpers {
	[NewType]
	public static class PathHelper {
		public static string Combine(params string[] paths) {
			var current = paths.Aggregate(@"", Path.Combine);
			return current;
		}

		private static readonly string _curDir = Environment.CurrentDirectory;
		private static readonly bool _isWindows = Environment.OSVersion.VersionString.Contains("Windows");
		public static string GetAbsolutePath(string relativePath) {
			if (Path.IsPathRooted(relativePath)) {
				return relativePath;
			}
			var result = PathHelper.Combine(_curDir, relativePath);
			var full = Path.GetFullPath(result);
			return full;
		}

		public static bool IsCaseSensitive {
			get {
				return _isWindows;
			}
		}

		/// <summary>
		/// Creates a relative path from one file or folder to another.
		/// </summary>
		/// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
		/// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
		/// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="UriFormatException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public static String GetRelativePath(String fromPath, String toPath) {
			if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
			if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");
			if (!Path.IsPathRooted(toPath)) {
				return toPath;
			}
			Uri fromUri = new Uri(fromPath);
			Uri toUri = new Uri(toPath, UriKind.RelativeOrAbsolute);

			if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (toUri.Scheme.ToUpperInvariant() == "FILE") {
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			return relativePath;
		}

		public static string GetRelativePath(string path) {
			return GetRelativePath(_curDir, path);
		}

		public static bool Equal(string path1, string path2) {
			return String.Equals(path1, path2, IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
		}

		public static string ChangeExtension(string path, Func<string, string> selector) {
			var ext = Path.GetExtension(path);
			return Path.ChangeExtension(path, selector(ext));
		}

		public static IEnumerable<string> Components(string path) {
			return path.Split(Path.DirectorySeparatorChar);
		} 

		public static string GetUserFriendlyPath(string path) {
			var components = Components(path).ToList();
			var total = components.Count;
			if (total <= 5) {
				return path;
			}
			components.RemoveRange(2, components.Count - 4);
			components.Insert(2, "...");
			return PathHelper.Combine(components.ToArray());
		}
	}
	[NewType]
	public static class StringHelper {
		public static string ReplaceAll(this string str, string replaceWith, params string[] replaceWhat) {
			return replaceWhat.Aggregate(str, (current, what) => current.Replace(what, replaceWith));
		}

		public static string SentenceCase(this string str) {
			if (string.IsNullOrEmpty(str)) return str;
			return char.ToUpper(str[0]) + str.Substring(1);
		}

		public static string Join(this IEnumerable<string> str, string separator) {
			return string.Join(separator, str.ToArray());
		}
	}
}
