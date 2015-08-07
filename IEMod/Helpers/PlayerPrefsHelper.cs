using System;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Helpers {
	
	/// <summary>
	/// Helper methods for PlayerPrefs.
	/// </summary>
	[NewType]
	public class PlayerPrefsHelper {
		public static void SetBool(string name, bool value) {
			PlayerPrefs.SetInt(name, value ? 1 : 0);
		}

		public static bool GetBool(string name, bool defaultValue = false) {
			return PlayerPrefs.GetInt(name, defaultValue ? 1 : 0) == 1;
		}

		public static object GetObject(string name, Type type) {
			object value;
			var realType = type.IsEnum ? Enum.GetUnderlyingType(type) : type;
			if (realType == typeof (bool)) {
				value = PlayerPrefsHelper.GetBool(name, false);
			} else if (realType == typeof (int)) {
				value = PlayerPrefs.GetInt(name, 0);
			} else if (realType == typeof (string)) {
				value = PlayerPrefs.GetString(name, "");
			} else if (realType == typeof (float)) {
				value = PlayerPrefs.GetFloat(name, 0.0f);
			} else {
				throw new IEModException("Invalid field type: " + realType);
			}
			var typedValue = type.IsEnum ? Enum.ToObject(type, value) : value;
			return typedValue;
		}

		public static void SetObject(string name, Type type, object o) {
			var realType = type.IsEnum ? Enum.GetUnderlyingType(type) : type;
			if (realType == typeof (bool)) {
				PlayerPrefsHelper.SetBool(name, (bool)o);
			} else if (realType == typeof (int)) {
				PlayerPrefs.SetInt(name, (int)o);
			} else if (realType == typeof (string)) {
				PlayerPrefs.SetString(name, (string)o);
			} else if (realType == typeof (float)) {
				PlayerPrefs.SetFloat(name, (float) o);
			} else {
				throw new IEModException("Invalid field type: " + realType);
			}
		}


	}

}