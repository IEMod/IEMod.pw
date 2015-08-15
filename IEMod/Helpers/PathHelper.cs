using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
