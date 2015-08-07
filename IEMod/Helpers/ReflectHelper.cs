using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Patchwork.Attributes;

namespace IEMod.Helpers {
	[NewType]
	public static class ReflectHelper {
		public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider provider) {
			return provider.GetCustomAttributes(typeof (T), true).OfType<T>();
		}

		public static T GetCustomAttribute<T>(this ICustomAttributeProvider provider) {
			return provider.GetCustomAttributes<T>().SingleOrDefault();
		}
	}
}