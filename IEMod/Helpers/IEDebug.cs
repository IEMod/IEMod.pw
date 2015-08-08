using System;
using System.IO;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Helpers {
	/// <summary>
	/// Log IEMod-specific messages here. It will help you find them later. It writes to file, so don't use it in production too much.
	/// </summary>
	[NewType]
	public static class IEDebug {
		private static readonly TextWriter _logger;

		private static readonly Stream _innerStream;
		static IEDebug() {
			var fs = File.Open("IEMod.log", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
			_innerStream = fs;
			var writer = new StreamWriter(_innerStream);
			_logger = writer;
		}

		public static void Log(string format) {
			_logger.WriteLine(format);
			Debug.Log("IEMod: " + format);
			_logger.Flush();
		}

		public static void Log(string format, params object[] args) {
			_logger.WriteLine(format, args);
			Debug.Log("IEMod: " + string.Format(format, args));
			_logger.Flush();
		}

		public static IEModException Exception(Exception innerEx, string message, params object[] args) {
			Log("!! EXCEPTION !!: " + message);
			args = args ?? new object[] {};
			return new IEModException(String.Format(message, args), innerEx);
		}
	}
}