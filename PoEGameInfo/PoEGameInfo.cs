using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Patchwork.Attributes;

namespace PoEGameInfo {

    [AppInfoFactory]
    internal class PoEAppInfoFactory : AppInfoFactory {

		

        public override AppInfo CreateInfo(DirectoryInfo folderInfo) {

	        FileInfo exeFile;
	        string exeFileName;

	        exeFileName = Environment.OSVersion.Platform == PlatformID.Unix ? "PillarsOfEternity" : "PillarsOfEternity.exe";
	        var fileInfos = folderInfo.GetFiles(exeFileName).ToList();

	        if (fileInfos.Count == 0) {
		        throw new FileNotFoundException($"The Pillars of Eternity executable file '{exeFileName}' was not found in this directory.", exeFileName);
	        }
	        exeFile = fileInfos[0];

	        return new AppInfo() {
		        BaseDirectory = folderInfo,
		        Executable = exeFile,
		        AppVersion = FileVersionInfo.GetVersionInfo(exeFile.FullName).FileVersion,
		        AppName = "Pillars of Eternity",
		        IgnorePEVerifyErrors = new[] {
			        //Expected an ObjRef on the stack.(Error: 0x8013185E). 
			        //-you can ignore the following. They are present in the original DLL. I'm not sure if they are actually errors.
			        0x8013185EL,
			        //The 'this' parameter to the call must be the calling method's 'this' parameter.(Error: 0x801318E1)
			        //-this isn't really an issue. PEV is just confused.
			        0x801318E1,
			        //Call to .ctor only allowed to initialize this pointer from within a .ctor. Try newobj.(Error: 0x801318BF)
			        //-this is a *verificaiton* issue is caused by copying the code from an existing constructor to a non-constructor method 
			        //-it contains a call to .ctor(), which is illegal from a non-constructor method.
			        //-There will be an option to fix this at some point, but it's not really an error.
			        0x801318BF,
		        }
	        };
        }
    }

}
