using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IEMod.Helpers;
using Patchwork.Attributes;

namespace IEMod
{
	[PatchInfo]
	public class IEModType : IPatchInfo
	{
		public FileInfo GetTargetFile(AppInfo app) {
			var file = PathHelper.Combine(app.BaseDirectory.FullName, "PillarsOfEternity_Data", "Managed", "Assembly-CSharp.dll");
			return new FileInfo(file);
		}

		public string PatchVersion => IEModVersion.Version;

		public string Requirements => "None";

		public string PatchName => "IEMod";
	}
}
