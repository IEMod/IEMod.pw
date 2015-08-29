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
			PatchGame();
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

		private static void PatchGame() {
			//+ Path-related
			var copyToPath = Paths.YourNormalManagedFolder + @"\Assembly-CSharp.dll";
			var originalDllPath = Path.Combine(Paths.YourDllSourcesPath, "Assembly-CSharp.dll");

			//+ Creating patcher
			var patcher = new AssemblyPatcher(originalDllPath, ImplicitImportSetting.OnlyCompilerGenerated, Log);

			//+ Patching assemblies
			patcher.PatchAssembly(typeof (IEModType).Assembly.Location);

			//add more lines to patch more things

			
			//+ End

			patcher.WriteTo(copyToPath);
			Console.WriteLine(
				"Going to run PEVerify to check the IL for errors. Press ESC to cancel, or any other key to continue.");
			if (Console.ReadKey().Key != ConsoleKey.Escape) {
				RunPEVerify(copyToPath);
				//RunILSpy(copyToPath);
				//RunILSpy(typeof (IEModType).Assembly.Location);
			}
		}

		private static void RunILSpy(string path) {
			Process.Start(string.Format("\"{0}\"", Paths.ILSpyPath), string.Format("\"{0}\"", path));
		}

		private static void RunPEVerify(string path) {

			//PEVerify is a tool that verifies IL. It goes over it and looks for various issues.
			//Unfortunately, it just says every issue is an ERROR, but in reality some of them are fine as long as you run with full trust
			//others are just invalid and will throw InvalidProgramExceptions or similar.

			//PEVerify checks both validity and verifiability. Verifiability is primarily a security matter
			//it doesn't tell you which error is related to verifiability and which will just make the whole business fail to run.
			//it is best practice to have as few verifiability errors as possible, even though the game is running at full trust.

			//
			var ignoreErrors = new string[] {
				//Expected an ObjRef on the stack.(Error: 0x8013185E). 
				//-you can ignore the following. They are present in the original DLL. I'm not sure if they are actually errors.
				"0x8013185E",
				"0x801312BB",
				"0x801312C2",
				"0x80131274",
				//The 'this' parameter to the call must be the calling method's 'this' parameter.(Error: 0x801318E1)
				//-this isn't really an issue. PEV is just confused.
				"0x801318E1",
				//Call to .ctor only allowed to initialize this pointer from within a .ctor. Try newobj.(Error: 0x801318BF)
				//-this is a *verificaiton* issue is caused by copying the code from an existing constructor to a non-constructor method 
				//-it contains a call to .ctor(), which is illegal from a non-constructor method.
				//-There will be an option to fix this at some point, but it's not really an error.
				"0x801318BF",
			};
			var info = new ProcessStartInfo() {
				UseShellExecute = false,
				FileName = "cmd",
				RedirectStandardOutput = true,
				Arguments =
					string.Format("/c \"\"{1}\" /il /md /verbose /hresult /ignore={2} \"{0}\"\"", path, Paths.PeVerifyPath,
						ignoreErrors.Join(","))
			};
			using (var process = Process.Start(info)) {
				Log.Information("Running PEVerify.exe...");
				Log.Information(process.StandardOutput.ReadToEnd());
			}

			Console.WriteLine("Press any key to close.");
			Console.ReadKey();
		}



	}
}