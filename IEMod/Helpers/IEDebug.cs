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

		public static void WriteLine(string format) {
			_logger.WriteLine(format);
			Debug.Log("IEMod: " + format);
			_logger.Flush();
		}

		public static void WriteLine(string format, params object[] args) {
			_logger.WriteLine(format, args);
			Debug.Log("IEMod: " + string.Format(format, args));
			_logger.Flush();
		}

		public static void Throw(string message, object[] args = null, string locationId = "", Exception innerEx = null) {
			WriteLine("!! EXCEPTION !!: " + message);
			args = args ?? new object[] {};
			throw new IEModException(String.Format(message, args) + " \r\n LocationId: " + locationId, innerEx);
		}
	}
}