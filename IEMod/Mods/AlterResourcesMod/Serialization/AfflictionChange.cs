using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patchwork.Attributes;

namespace IEMod.Mods.AlterResourcesMod.Serialization
{
    [NewType]
    public class AfflictionChange
    {
        public string Index { get; set; }
        public string Duration { get; set; }
    }
}
