using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patchwork.Attributes;

namespace IEMod.Mods.AlterResourcesMod.Serialization
{
    [NewType]
    public class AbilityChange
    {
        public string Name { get; set; }
        public string MinDamage { get; set; }
        public string MaxDamage { get; set; }
        public string Accuracy { get; set; }
        public string Range { get; set; }
        public string DTBypass { get; set; }
        public string BlastRadius { get; set; }
        public string BlastAngle { get; set; }
        public string DamageType { get; set; }
        public string DefendedBy { get; set; }
        public List<AfflictionChange> AfflictionChanges { get; set;}
        public AbilityChange ExtraAOE { get; set; }
    }
}
