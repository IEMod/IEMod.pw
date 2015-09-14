using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Patchwork.Attributes;

namespace IEMod.Mods.AlterResourcesMod.Serialization
{
    [NewType]
    public class AbilityExport
    {
        [XmlAttribute(AttributeName ="Name")]
        public string Name { get; set; }
    }
}
