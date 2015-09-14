using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patchwork.Attributes;
using IEMod.Helpers;

namespace IEMod.Mods.AlterResourcesMod
{
    [NewType]
    public class AbilityActionData
    {
        public AbilityActionData()
        {
            AbilityChanges = new Dictionary<string, IEnumerable<AbilityChange>>();
            AbilityExports = new HashSet<string>();
            ExportAll = false;
        }

        public AbilityActionData(Serialization.AbilityActionData serializedData)
        {
            AbilityChanges = new Dictionary<string, IEnumerable<AbilityChange>>();
            AbilityExports = new HashSet<string>();

            foreach(Serialization.AbilityExport ability in serializedData.AbilityExports)
            {
                AbilityExports.Add(ability.Name);
            }

            foreach (Serialization.AbilityChange ability in serializedData.AbilityChanges)
            {
                AbilityChanges.Add(ability.Name, TranslateAbilityChanges(ability));
            }

        }

        public IEnumerable<AbilityChange> TranslateAbilityChanges(Serialization.AbilityChange serializedChangeData)
        {
            List<AbilityChange> result = new List<AbilityChange>();

            if(!String.IsNullOrEmpty(serializedChangeData.MinDamage))
            {
                AddFloatValue(AbilityChange.ChangeType.MinDamage, serializedChangeData.MinDamage, result);
            }
            if (!String.IsNullOrEmpty(serializedChangeData.MaxDamage))
            {
                AddFloatValue(AbilityChange.ChangeType.MaxDamage, serializedChangeData.MaxDamage, result);
            }
            if (!String.IsNullOrEmpty(serializedChangeData.BlastAngle))
            {
                AddFloatValue(AbilityChange.ChangeType.BlastAngle, serializedChangeData.BlastAngle, result);
            }
            if (!String.IsNullOrEmpty(serializedChangeData.BlastRadius))
            {
                AddFloatValue(AbilityChange.ChangeType.BlastRadius, serializedChangeData.BlastRadius, result);
            }
            if (!String.IsNullOrEmpty(serializedChangeData.DTBypass))
            {
                AddFloatValue(AbilityChange.ChangeType.DTBypass, serializedChangeData.DTBypass, result);
            }
            if (!String.IsNullOrEmpty(serializedChangeData.Range))
            {
                AddFloatValue(AbilityChange.ChangeType.Range, serializedChangeData.Range, result);
            }
            if (!String.IsNullOrEmpty(serializedChangeData.Accuracy))
            {
                AddIntValue(AbilityChange.ChangeType.Accuracy, serializedChangeData.Accuracy, result);
            }

            if (!String.IsNullOrEmpty(serializedChangeData.DamageType))
            {
                AddEnumValue<DamagePacket.DamageType>(AbilityChange.ChangeType.DamageType, serializedChangeData.DamageType, result);
            }
            if (!String.IsNullOrEmpty(serializedChangeData.DefendedBy))
            {
                AddEnumValue<CharacterStats.DefenseType>(AbilityChange.ChangeType.DefendedBy, serializedChangeData.DefendedBy, result);
            }

            if (serializedChangeData.AfflictionChanges != null)
            {
                foreach(Serialization.AfflictionChange change in serializedChangeData.AfflictionChanges)
                {
                    int index;
                    float duration;
                    bool flag;

                    flag = int.TryParse(change.Index, out index);
                    if (flag)
                    {
                        flag = float.TryParse(change.Duration, out duration);

                        if(flag)
                        {
                            result.Add(new AbilityChange() {
                                Type = AbilityChange.ChangeType.Affliction,
                                Value = new AbilityChange.AfflictionChange() { Index = index, Duration = duration},
                            });
                        } else
                        {
                            IEDebug.Log($"Failed to parse Affliction Duration. Expected float, got {change.Duration}");
                        }
                    } else
                    {
                        IEDebug.Log($"Failed to parse Affliction Index. Expected int, got {change.Index}");
                    }
                }
            }

            if(serializedChangeData.ExtraAOE != null)
            {
                IEnumerable<AbilityChange> aoeAbilityChanges = TranslateAbilityChanges(serializedChangeData.ExtraAOE);
                if(aoeAbilityChanges != null)
                {
                    result.Add(new AbilityChange()
                    {
                        Type = AbilityChange.ChangeType.ExtraAOE,
                        Value = aoeAbilityChanges,
                    });
                }

            }

            return result;
        }

        public void AddFloatValue(AbilityChange.ChangeType changeType, string source, List<AbilityChange> accumulator)
        {
            float value;

            if (float.TryParse(source, out value))
            {
                accumulator.Add(new AbilityChange() { Type = changeType, Value = value });
            }
            else
            {
                IEDebug.Log($"Tried to assign non-float value {source} to field {changeType.ToString()}");
            }

        }

        public void AddIntValue(AbilityChange.ChangeType changeType, string source, List<AbilityChange> accumulator)
        {
            int value;

            if (int.TryParse(source, out value))
            {
                accumulator.Add(new AbilityChange() { Type = changeType, Value = value });
            }
            else
            {
                IEDebug.Log($"Tried to assign non-int value {source} to field {changeType.ToString()}");
            }

        }

        public void AddEnumValue<TEnum>(AbilityChange.ChangeType changeType, string source, List<AbilityChange> accumulator)
        {
            TEnum value;

            try { 
                value = (TEnum) Enum.Parse(typeof(TEnum), source);
                accumulator.Add(new AbilityChange() { Type = changeType, Value = value });
            } catch(Exception ex)
            {
                IEDebug.Log($"Could not parse enum value {source} for field {changeType.ToString()}. Expected value of type {typeof(TEnum).ToString()}. Exception: {ex.ToString()}");
            }

        }


        public Dictionary<string, IEnumerable<AbilityChange>> AbilityChanges { get; set; }
        public bool ExportAll { get; set; }
        public HashSet<string> AbilityExports { get; set; }
    }
}
