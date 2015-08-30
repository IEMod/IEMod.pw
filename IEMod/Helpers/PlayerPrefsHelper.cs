using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
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

		private static object GetXmlObject(string name, Type type) {
			if (!PlayerPrefs.HasKey(name)) {
				return null;
			}
			var xmlSerializer = new XmlSerializer(type);
			var content = PlayerPrefs.GetString(name);
			if (String.IsNullOrEmpty(content)) {
				return Activator.CreateInstance(type);
			}
			object obj = null;
			try {
				obj = xmlSerializer.Deserialize(new StringReader(content));
			}
			catch (Exception ) {
				IEDebug.Log($"Error when deserializing: {name}");
			}
			return obj;
		}

		private static void SetXmlObject(string name, Type type, object o) {
			if (o == null) {
				PlayerPrefs.DeleteKey(name);
			}
			var xmlSerializer = new XmlSerializer(type);
			var strWriter = new StringWriter();
			xmlSerializer.Serialize(strWriter, o);
			var content = strWriter.ToString();
			PlayerPrefs.SetString(name, content);
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
				IEDebug.Log("Going to try to deserialize PlayerPref {0} as XML", name);
				return GetXmlObject(name, type);
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
				IEDebug.Log("Going to try to serialize PlayerPref '{0}' as XML", name);
				SetXmlObject(name, type, o);
			}
		}


	}

}