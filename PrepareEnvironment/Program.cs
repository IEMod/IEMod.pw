using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Patchwork.Utility;
using Serilog;
using Serilog.Events;

namespace ModdingEnvironment
{
	class Program
	{


		private static void DoSetup()
        {
            //+ Logging-related
            //log messages can get pretty big, so it's nice to have a lot of space to view them:
            Console.WindowWidth = (int)(Console.LargestWindowWidth * 0.75);
            Console.BufferWidth = 300; //for extra long messages
            Console.WindowHeight = (int)(Console.LargestWindowHeight * 0.75);
            Console.BufferHeight = 2000; //so everything is visible

            LogFile = new StreamWriter(File.Open("log.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));

            var log =
                new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.ColoredConsole(LogEventLevel.Debug)
                .WriteTo.TextWriter(LogFile).CreateLogger();

            //note: if you're going to be looking at this a lot, better set your console font to something snazzy
            //(right click on titlebar, Default)
            Log = log;
        }

		public static ILogger Log {
			get;
			set;
		}

		public static StreamWriter LogFile {
			get;
			set;
		}

		static Program()
        {
            var log = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.ColoredConsole().CreateLogger();
            Serilog.Log.Logger = log;
        }
        [STAThread]
        private static void Main(string[] args)
        {
            DoSetup();
	        PrepareEnvironment();
            LogFile.Flush();
            LogFile.Close();
        }

        private static void PrepareEnvironment()
        {
            Console.WindowWidth = Console.LargestWindowWidth / 2;
            Console.WindowHeight = Console.LargestWindowHeight / 2;
            Console.BufferWidth = 250;
            Console.BufferHeight = 1000;
            var log = Serilog.Log.Logger;
            log.Information("Hey there. Just going to set up your environment.");

            var assemblyCSharpPath = Paths.YourOriginalManagedFolder + @"\Assembly-CSharp.dll";
            var filesToCopy = new[] {
                "Assembly-CSharp-firstpass"
                , "Assembly-CSharp-firstpass"
                , "Assembly-UnityScript-firstpass"
                , "OEICommon"
                , "OEIFormats",
                "UnityEngine"
            };

            var referencesPath = PathHelper.GetAbsolutePath(Paths.YourDllReferencesPath);
            var workingPath = PathHelper.GetAbsolutePath(Paths.YourDllSourcesPath);
            var sourcePath = PathHelper.GetAbsolutePath(Paths.YourOriginalManagedFolder);
            Serilog.Log.Information("Copying {0} files from {1} to {2}", filesToCopy.Length + 1, sourcePath, referencesPath);
            Directory.CreateDirectory(referencesPath);
            Directory.CreateDirectory(workingPath);
            foreach (var file in filesToCopy)
            {
                var fileName = file + ".dll";
                var filePath = Path.Combine(sourcePath, fileName);
                Serilog.Log.Information("Copying {0}", filePath);
                var targetPath = Path.Combine(referencesPath, fileName);
                File.Copy(filePath, targetPath, true);
            }

            var workTarget = Path.Combine(workingPath, "Assembly-CSharp.dll");
            Serilog.Log.Information("Copying {0}", assemblyCSharpPath);
            File.Copy(assemblyCSharpPath, workTarget, true);

            AssemblyDefinition publicAssembly;
            using (var openRead = File.OpenRead(assemblyCSharpPath))
            {
                publicAssembly = AssemblyDefinition.ReadAssembly(openRead);
            }
            CecilHelper.MakeOpenAssembly(publicAssembly, true);
            var publicAssemblyPath = Path.Combine(referencesPath, "Assembly-CSharp.dll");
            Serilog.Log.Information("Deleting old assembly at {0}", publicAssemblyPath);
            File.Delete(publicAssemblyPath);
            Serilog.Log.Information("Writing assembly");

            using (var fileStream = File.OpenWrite(publicAssemblyPath))
            {
                publicAssembly.Write(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            Serilog.Log.Information("Success");
            Console.Read();
        }
	}
}
