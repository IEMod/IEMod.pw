using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

		private static Action<object> CreateSetter(MemberExpression expr) {
			if (expr == null) {
				throw IEDebug.Exception(null, "The expression is not allowed to be null.");
			}
			var targetGetter = CreateGetter(expr.Expression);
			Action<object> setter;
			if (expr.Member is FieldInfo) {
				var asFieldInfo = (FieldInfo) expr.Member;
				setter = value => asFieldInfo.SetValue(targetGetter(), value);
			} else if (expr.Member is PropertyInfo) {
				var asPropertyInfo = (PropertyInfo) expr.Member;
				setter = value => asPropertyInfo.SetValue(targetGetter(), value, null);
			} else {
				throw new IEModException(
					"Expected PropertyInfo or FieldInfo member, but got: "
						+ expr.Member);
			}
			return setter;
		}

		/// <summary>
		/// Takes an expression which is expected to be a member access expression (e.g. x.Prop1.Field2.Prop3), and returns a setter for setting that member on that target.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="memberAccess"></param>
		/// <returns></returns>
		public static Action<T> CreateSetter<T>(Expression<Func<T>> memberAccess) {
			if (!(memberAccess.Body is MemberExpression)) {
				throw IEDebug.Exception(null, "The topmost expression must be a simple member access expression.");
			}
			return v => CreateSetter((MemberExpression) memberAccess.Body)(v);
		}

		public static Func<T> CreateGetter<T>(Expression<Func<T>> memberAccess) {
			return memberAccess.Compile();
		}

		private static Func<object> CreateGetter(Expression expr) {
			if (expr == null) {
				return () => null;
			}
			switch (expr.NodeType) {
				case ExpressionType.Constant:
					var asConst = (ConstantExpression)expr;
					return () => asConst.Value;
				case ExpressionType.MemberAccess:
					var asMember = (MemberExpression) expr;
					var targetGetter = CreateGetter(asMember.Expression);
					if (asMember.Member is FieldInfo) {
						var field = (FieldInfo)asMember.Member;
						return () => field.GetValue(targetGetter());
					}
					if (asMember.Member is PropertyInfo) {
						var prop = (PropertyInfo) asMember.Member;
						return () => prop.GetValue(targetGetter(), null);
					}
					throw IEDebug.Exception(null, "Unexpected member type ", asMember.Member.GetType());
				default:
					throw IEDebug.Exception(null, "Unexpected node type {0}", expr.NodeType);
					
			}
		}
	}
}