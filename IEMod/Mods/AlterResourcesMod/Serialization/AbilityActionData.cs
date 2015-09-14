using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patchwork.Attributes;
using System.Xml.Serialization;

namespace IEMod.Mods.AlterResourcesMod.Serialization
{
    [NewType]
    public class AbilityActionData
    {

        public List<Serialization.AbilityChange> AbilityChanges { get; set; }

        [XmlArrayItem(ElementName = "AbilityName", Type = typeof(string))]
        [XmlArray(ElementName ="AbilityExport")]
        public List<AbilityExport> AbilityExports { get; set; }

        public bool? ExportAll { get; set; }
    }
}
