using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IEMod.QuickControls;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Helpers {
	[NewType]
	public class MemberAccess<T> {
		private readonly MemberExpression _expression;

		public Action<T> Setter {
			get;
			private set;
		}

		public Func<T> Getter {
			get;
			private set;
		}

		public MemberInfo TopmostMember {
			get;
			private set;
		}

		public Func<object> InstanceGetter {
			get;
			private set;
		}

		public MemberAccess(Expression<Func<T>> expression) {
			var asMemberExpr = (MemberExpression)expression.Body;
			_expression = asMemberExpr;
			Getter = ReflectHelper.CreateGetter(expression);
			Getter = expression.Compile();
			var setter = ReflectHelper.CreateSetter(expression);
			Setter = v => setter(v);
			TopmostMember = asMemberExpr.Member;
			InstanceGetter = ReflectHelper.CreateGetter(asMemberExpr.Expression);
		}
	}

	[NewType]
	public static class ReflectHelper {
		public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider provider) {
			return provider.GetCustomAttributes(typeof (T), true).OfType<T>();
		}

		public static T GetCustomAttribute<T>(this ICustomAttributeProvider provider) {
			return provider.GetCustomAttributes<T>().SingleOrDefault();
		}

		public static Action<object> CreateSetter(MemberExpression expr) {
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
			return () => (T)CreateGetter(memberAccess.Body)();
		}

		public static Func<object> CreateBaseGetter<T>(Expression<Func<T>> memberAccess) {
			if (!(memberAccess.Body is MemberExpression)) {
				throw IEDebug.Exception(null, "The topmost expression must be a simple member access expression.");
			}
			var asMemberExpr = (MemberExpression)memberAccess.Body;
			return CreateGetter(asMemberExpr.Expression);
		}

		public static string TryGetName(object o) {
			var wrapper = o as IGameObjectWrapper;
			var go = o as GameObject;
			return wrapper != null ? wrapper.Name : go?.name;
		}

		public static MemberAccess<T> AnalyzeMember<T>(Expression<Func<T>> memberAccessExpr) {
			return new MemberAccess<T>(memberAccessExpr);
		}

		public static Func<object> CreateGetter(Expression expr) {
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

		public static string GetLabelInfo(MemberInfo provider) {
			var labelAttr = provider.GetCustomAttribute<LabelAttribute>();
			var label = labelAttr == null ? null : labelAttr.Label;
			label = label ?? provider.Name;
			return label;
		}

		public static string GetDescriptionInfo(MemberInfo provider) {
			var descAttr = provider.GetCustomAttribute<DescriptionAttribute>();
			var desc = descAttr == null ? null : descAttr.Description;
			desc = desc ?? provider.Name;
			return desc;
		}
	}
}