using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patchwork.Attributes;
using IEMod.Helpers;

namespace IEMod.Mods.AlterResourcesMod
{
    [NewType]
    public class AbilityChange
    {
        public ChangeType Type { get; set; }
        public Object Value { get; set; }

        public T GetValue<T>()
        {
            if(Value is T)
            {
                return (T)Value;
            }
                    
            //It'd be nice if this could check if desired type is float and we have an int and convert it, but it's
            //  way harder than you'd think because int and float are intracovertable only by language convention, not
            //  formally. Thus, I'm forcing the serialization code to deal with it instead of here.    

            throw new IEMod.Helpers.IEModException($"Wrong type of value in Ability Change {Type.ToString()}");
        }

        [NewType]
        public class AfflictionChange
        {
            public int Index { get; set; }
            public float Duration { get; set; }
        }


        [NewType]
        public enum ChangeType
        {
            MinDamage,
            MaxDamage,
            DamageType,
            Accuracy,
            Range,
            DTBypass,
            DefendedBy,
            BlastRadius,
            BlastAngle,
            //StatusEffect,
            Affliction,
            ExtraAOE,
            //NewStatusEffect,
            //NewAffliction,
            //NewExtraAOE
        }
    }
}
