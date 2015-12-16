using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using IEMod;
using ModdingEnvironment;
using Mono.Cecil;
using Patchwork;
using Patchwork.Attributes;
using Patchwork.Utility;
using Serilog;
using Serilog.Events;

namespace Start {
	class Program {
		static Program() {
			var log = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.ColoredConsole().CreateLogger();
			Serilog.Log.Logger = log;
		}

		[STAThread]
		private static void Main(string[] args) {

			DoSetup();
			//BuildAllVersions();
			PatchIntoGame();
			LogFile.Flush();
			LogFile.Close();
		}

		private static ILogger Log {
			get;
			set;
		}

		private static StreamWriter LogFile {
			get;
			set;
		}

		private static void DoSetup() {
			//+ Logging-related
			//log messages can get pretty big, so it's nice to have a lot of space to view them:
			Console.WindowWidth = (int) (Console.LargestWindowWidth * 0.75);
			Console.BufferWidth = 300; //for extra long messages
			Console.WindowHeight = (int) (Console.LargestWindowHeight * 0.75);
			Console.BufferHeight = 2000; //so everything is visible

			LogFile = new StreamWriter(File.Open("log.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));

			var log =
				new LoggerConfiguration()
					.MinimumLevel.Debug()
					.WriteTo.ColoredConsole(LogEventLevel.Information)
					.WriteTo.TextWriter(LogFile).CreateLogger();
	
			//note: if you're going to be looking at this a lot, better set your console font to something snazzy
			//(right click on titlebar, Default)
			Log = log;
		}

		private static void BuildAllVersions() {
			Log.Information("PATCHING WINDOWS");
			PatchGame("win", Path.Combine(RelativePaths.LatestBuildFolder, "win"), false);
			Log.Information("PATCHING MAC");
			PatchGame("mac", Path.Combine(RelativePaths.LatestBuildFolder, "mac"), false);
			Log.Information("PATCHING LINUX");
			PatchGame("linux", Path.Combine(RelativePaths.LatestBuildFolder, "linux"), false);

		}

		private static void PatchIntoGame() {
			PatchGame("win", Paths.YourNormalManagedFolder, true);
		}

		private static readonly long[] _ignoreErrors = {
			//Expected an ObjRef on the stack.(Error: 0x8013185E). 
			//-you can ignore the following. They are present in the original DLL. I'm not sure if they are actually errors.
			0x8013185E,
			//The 'this' parameter to the call must be the calling method's 'this' parameter.(Error: 0x801318E1)
			//-this isn't really an issue. PEV is just confused.
			0x801318E1,
			//Call to .ctor only allowed to initialize this pointer from within a .ctor. Try newobj.(Error: 0x801318BF)
			//-this is a *verificaiton* issue is caused by copying the code from an existing constructor to a non-constructor method 
			//-it contains a call to .ctor(), which is illegal from a non-constructor method.
			//-There will be an option to fix this at some point, but it's not really an error.
			0x801318BF,
		};

		private static void PatchGame(string version, string copyToFolder, bool runPeVerify) {
			if (!Directory.Exists(copyToFolder)) {
				Log.Information("Creating copy to folder.");
				Directory.CreateDirectory(copyToFolder);
			}
			//+ Path-related
			var copyToPath = Path.Combine(copyToFolder, @"Assembly-CSharp.dll");
			var originalDllPath = Path.Combine(RelativePaths.DllSourcesPath, version, "Assembly-CSharp.dll");

			//+ Creating patcher
			var patcher = new AssemblyPatcher(originalDllPath, Log) {
				EmbedHistory = true
			};
			
			File.Copy(typeof(NewTypeAttribute).Assembly.Location, Path.Combine(copyToFolder, "Patchwork.Attributes.dll"), true);

			//+ Patching assemblies
			patcher.PatchAssembly(typeof (IEModType).Assembly.Location);
			//add more lines to patch more things

			//+ End

			if (runPeVerify) {
				Console.WriteLine(
					"Running PEVerify on the assembly to check the IL for errors. Please wait.");
				Log.Information(patcher.RunPeVerify(ignoreErrors: _ignoreErrors));
			}
			patcher.WriteTo(copyToPath);
			LogFile.Flush();
			Console.WriteLine("Press any key to close.");
			Console.Read();
		}

		private static void RunILSpy(string path) {
			Process.Start($"\"{RelativePaths.ILSpyPath}\"", $"\"{path}\"");
		}


	}
}