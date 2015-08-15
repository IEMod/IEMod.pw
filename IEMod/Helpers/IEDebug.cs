using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using Patchwork.Attributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

		public static void Log(object format) {
			if (format == null || (format is string && string.IsNullOrEmpty((string)format))) return;
			_logger.WriteLine(format);
			_logger.Flush();
			_innerStream.Flush();
		}

		public static void Log(string format, params object[] args) {
			Log((object)string.Format(format, args));
		}

		public static IEModException Exception(Exception innerEx, string message, params object[] args) {
			Log("!! EXCEPTION !!: " + message, args);
			args = args ?? new object[] {};
			var writer = new IndentedTextWriter(new StringWriter());
			PrintStackTrace(writer, new StackTrace(1));
			return new IEModException(String.Format(message, args), innerEx);
		}

		private static void PrintStackTrace(IndentedTextWriter writer, StackTrace trace) {
			var frames = trace.GetFrames();
			if (frames == null) {
				writer.WriteLine("(none)");
				return;
			}
			for (int i = 0; i < frames.Length; i++) {
				var frame = frames[i];
				writer.WriteLine("{0}. At {1}", i, frame.GetMethod());
				writer.Indent++;
				writer.WriteLine("Source Location: {0}, ln# {1}, col# {2}", frame.GetFileName(), frame.GetFileLineNumber(), frame.GetFileColumnNumber());
				writer.WriteLine("IL Offset: {0}, Native Offset: {1}", frame.GetILOffset(), frame.GetNativeOffset());
				writer.Indent--;
			}
		}

		private static void PrintException(IndentedTextWriter iWriter, Exception ex) {
			if (ex == null) {
				iWriter.WriteLine("(null)");
				return;
			}
			iWriter.WriteLine("[{0}]", ex.GetType());
			iWriter.Indent++;
			iWriter.WriteLine("Message: {0}", ex.Message);
			iWriter.WriteLine("Source: {0}", ex.Source);
			iWriter.WriteLine("TargetSite: {0}", ex.TargetSite);
			iWriter.WriteLine("HelpLink: {0}", ex.HelpLink);
			if (ex.Data.Count > 0) {
				iWriter.WriteLine("Data:");
				iWriter.Indent++;
				foreach (var key in ex.Data.Keys) {
					iWriter.WriteLine("• {0} = {1}", key, ex.Data[key]);
				}
				iWriter.Indent--;
			}
			iWriter.WriteLine("Full Stack Trace:");
			iWriter.Indent++;
			PrintStackTrace(iWriter, new StackTrace(ex, true));
			iWriter.Indent--;
			iWriter.WriteLine("Inner Exception: ");
			iWriter.Indent++;
			PrintException(iWriter, ex.InnerException);
			iWriter.Indent--;
		}

		public static string PrintException(Exception ex) {
			var strWriter = new StringWriter();
			var indentedWriter = new IndentedTextWriter(strWriter);
			PrintException(indentedWriter, ex);
			indentedWriter.WriteLine(StackTraceUtility.ExtractStringFromException(ex));
			indentedWriter.Flush();
			strWriter.Flush();

			return strWriter.ToString();
		}


	}
}
