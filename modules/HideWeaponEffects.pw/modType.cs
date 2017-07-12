using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Patchwork.Attributes;

namespace HideWeaponEffects
{
    [PatchInfo]
    public class HideWeaponEffectsModType : IPatchInfo
    {
        public static string Combine(params string[] paths)
        {
            var current = paths.Aggregate(@"", Path.Combine);
            return current;
        }

        public FileInfo GetTargetFile(AppInfo app)
        {
            var file = Combine(app.BaseDirectory.FullName, "PillarsOfEternity_Data", "Managed", "Assembly-CSharp.dll");
            return new FileInfo(file);
        }

        public string CanPatch(AppInfo app)
        {
            return null;
        }

        public string PatchVersion => "1.0";

        public string Requirements => "None";

        public string PatchName => "Hide Weapon Effects";
    }
}