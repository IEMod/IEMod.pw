using Patchwork.Attributes;
using UnityEngine;
using IEMod.Helpers;
using System;
using Polenter.Serialization;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace IEMod.Mods.AlterResourcesMod
{
    [ModifiesType]
    public class mod_GameResources : GameResources
    {
        [NewMember]
        [DuplicatesBody(nameof(LoadPrefab))]
        public static T orig_LoadPrefab<T>(string filename, string assetName, bool instantiate)
        where T : UnityEngine.Object
        {

            return default(T);
        }

        [NewMember]
        private static AbilityActionData m_AbilityActionData;

        [NewMember]
        private static AbilityActionData AbilityActionData
        {
            get
            {
                if(m_AbilityActionData == null)
                {
                    m_AbilityActionData = GetAbilityActionData();
                }

                return m_AbilityActionData;
            }
        }


        [NewMember]
        public static AbilityActionData GetAbilityActionData()
        {
            AbilityActionData result = new AbilityActionData();

            string abilityDataPath = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/data"), "abilitydata.xml");
            IEDebug.Log($"Looking for file {abilityDataPath}");
            if (File.Exists(abilityDataPath))
            {
                IEDebug.Log("Found abilitydata.xml");
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Serialization.AbilityActionData));
                    Serialization.AbilityActionData serializedData;
                    using (var openRead = File.OpenRead(abilityDataPath))
                    {
                        serializedData = (Serialization.AbilityActionData)serializer.Deserialize(openRead);
                    }

                    result = new AbilityActionData(serializedData);

                    //List<AbilityChange> temp = new List<AbilityChange>(result.AbilityChanges["Blizzard"]);
                    //temp.Add(new AbilityChange()
                    //{
                    //    Type = AbilityChange.ChangeType.StatusEffect,
                    //    Value = new AbilityChange.StatusEffectChange()
                    //    {
                    //        Index = 0,
                    //        Magnitude = .7f,
                    //        Duration = 15f
                    //    }
                    //});

                    //result.AbilityChanges["Blizzard"] = temp;


                    //value = new Serialization.AbilityActionData();
                    //value.AbilityExports = new List<Serialization.AbilityExport>();
                    //value.AbilityExports.Add(new Serialization.AbilityExport { Name = "Test1" });
                    //value.AbilityExports.Add(new Serialization.AbilityExport { Name = "Test2" });
                    //value.AbilityChanges = new List<Serialization.AbilityChange>();
                    //value.AbilityChanges.Add(new Serialization.AbilityChange()
                    //{
                    //    Name = "Test3",
                    //    MinDamage = "3",
                    //    MaxDamage = "5.5",
                    //    Accuracy = "12",
                    //    BlastRadius = "3.2",
                    //    BlastAngle = "120",
                    //    DTBypass = "4.4",
                    //    Range = "10",
                    //    ExtraAOE = new Serialization.AbilityChange()
                    //    {
                    //        Name="Doesn't Matter",
                    //        MinDamage = "6",
                    //        MaxDamage = "9.2",
                    //        Accuracy = "12"
                    //    }

                    //});

                    //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    //using (var writer = new StringWriter(sb)) {
                    //    serializer.Serialize(writer, value);

                    //    IEDebug.Log($"Serialization example: {sb.ToString()}");
                    //}

                }
                catch (Exception ex)
                {
                    IEDebug.Log($"Couldn't process ability data: {ex.ToString()}");
                }
            }
            
            return result;
        }

        [NewMember]
        public static void ApplyChangesToAttack(AttackBase attack, IEnumerable<AbilityChange> changes)
        {
            IEDebug.Log($"Applying changes to {attack.name}");
            if(changes != null)
            {
                foreach(AbilityChange change in changes)
                {
                    ApplySingleChangeToAttack(attack, change);
                }
            }
        }

        [NewMember]
        public static void ApplySingleChangeToAttack(AttackBase attack, AbilityChange singleChange)
        {
            IEDebug.Log($"Applying change {singleChange.Type.ToString()} to {attack.name.ToString()}");

            try {
                AttackAOE aoe;

                switch (singleChange.Type)
                {
                    case AbilityChange.ChangeType.Accuracy:
                        attack.AccuracyBonus = singleChange.GetValue<int>();
                        break;

                    case AbilityChange.ChangeType.Range:
                        attack.AttackDistance = singleChange.GetValue<float>();
                        break;

                    case AbilityChange.ChangeType.MinDamage:
                        attack.DamageData.Minimum = singleChange.GetValue<float>();
                        break;

                    case AbilityChange.ChangeType.MaxDamage:
                        attack.DamageData.Maximum = singleChange.GetValue<float>();
                        break;

                    case AbilityChange.ChangeType.DamageType:
                        attack.DamageData.Type = singleChange.GetValue<DamagePacket.DamageType>();
                        break;

                    case AbilityChange.ChangeType.DefendedBy:
                        attack.DefendedBy = singleChange.GetValue<CharacterStats.DefenseType>();
                        break;

                    case AbilityChange.ChangeType.DTBypass:
                        attack.DTBypass = singleChange.GetValue<float>();
                        break;

                    case AbilityChange.ChangeType.Speed:
                        attack.AttackSpeed = singleChange.GetValue<AttackBase.AttackSpeedType>();
                        break;

                    case AbilityChange.ChangeType.BlastRadius:
                        if(!(attack is AttackAOE))
                        {
                            throw new InvalidOperationException("Cannot change BlastRadius on a non-AOE attack");
                        }

                        aoe = attack as AttackAOE;
                        aoe.BlastRadius = singleChange.GetValue<float>();

                        break;

                    case AbilityChange.ChangeType.BlastAngle:
                        if (!(attack is AttackAOE))
                        {
                            throw new InvalidOperationException("Cannot change BlastAngle on a non-AOE attack");
                        }

                        aoe = attack as AttackAOE;
                        aoe.DamageAngleDegrees = singleChange.GetValue<float>();

                        break;

                    case AbilityChange.ChangeType.StatusEffect:
                        AbilityChange.StatusEffectChange statusEffectData = singleChange.GetValue<AbilityChange.StatusEffectChange>();
                        if (attack.StatusEffects.Count <= statusEffectData.Index)
                        {
                            throw new InvalidOperationException($"Attempt to change Status Effect at Index {statusEffectData.Index}, but attack only contains {attack.StatusEffects.Count} status effects.");
                        }

                        if (statusEffectData.Magnitude.HasValue)
                        {
                            attack.StatusEffects[statusEffectData.Index].Value = statusEffectData.Magnitude.Value;
                        }
                        if (statusEffectData.Duration.HasValue)
                        {
                            attack.StatusEffects[statusEffectData.Index].Duration = statusEffectData.Duration.Value;
                        }

                        break;

                    case AbilityChange.ChangeType.Affliction:
                        AbilityChange.AfflictionChange afflictionData = singleChange.GetValue<AbilityChange.AfflictionChange>();
                        if (attack.Afflictions.Count <= afflictionData.Index)
                        {
                            throw new InvalidOperationException($"Attempt to change Affliction at Index {afflictionData.Index}, but attack only contains {attack.Afflictions.Count} afflictions.");
                        }

                        attack.Afflictions[afflictionData.Index].Duration = afflictionData.Duration;

                        break;

                    case AbilityChange.ChangeType.ExtraAOE:
                        ApplyChangesToAttack(attack.ExtraAOE, singleChange.GetValue<IEnumerable<AbilityChange>>());
                        break;

                    default:
                        throw new InvalidOperationException("Unsupported type of change");
                }
            } catch (Exception ex)
            {
                IEDebug.Log($"Failed to apply change {singleChange.Type.ToString()} to {attack.name.ToString()}. Reason: {ex.ToString()}");
            }
        }



        [NewMember]
        public static void AddIndentedString(System.Text.StringBuilder stringBuilder, String text, int indentLevel)
        {
            for(int i = 0; i<indentLevel; i++)
            {
                stringBuilder.Append("\t");
            }
            stringBuilder.AppendLine(text);
        }

        [NewMember]
        public static string SerializeAttackData(AttackBase ab, int indentLevel)
        {
            System.Text.StringBuilder attackString = new System.Text.StringBuilder();

            AddIndentedString(attackString, String.Format("Data for {0}", ab.name), indentLevel);
            AddIndentedString(attackString, $"Damage Data: {ab.DamageData.Minimum} - {ab.DamageData.Maximum} of type {ab.DamageData.Type.ToString()}", indentLevel);
            AddIndentedString(attackString, $"Accuracy Bonus: {ab.AccuracyBonus}", indentLevel);
            AddIndentedString(attackString, $"Range: {ab.AttackDistance}", indentLevel);
            AddIndentedString(attackString, $"DT Bypass: {ab.DTBypass}", indentLevel);
            AddIndentedString(attackString, $"Speed: {ab.AttackSpeedTime}", indentLevel);
            AddIndentedString(attackString, $"Push: {ab.PushDistance}", indentLevel);
            AddIndentedString(attackString, $"Defended By: {ab.DefendedBy.ToString()}", indentLevel);

            AddIndentedString(attackString, $"Status Effects Count: {ab.StatusEffects.Count}", indentLevel);
            foreach (StatusEffectParams effect in ab.StatusEffects)
            {
                AddIndentedString(attackString, String.Format("Status Effect: {0} for {1}, {2}", effect.AffectsStat, effect.Duration, effect.Value), indentLevel);
            }
            AddIndentedString(attackString, $"Afflictions Count: {ab.Afflictions.Count}", indentLevel);
            foreach (AfflictionParams affliction in ab.Afflictions)
            {                
                AddIndentedString(attackString, $"Affliction: {affliction.AfflictionPrefab.name} for {affliction.Duration}", indentLevel);
            }
            if (ab is AttackRanged)
            {
                AttackRanged abRanged = ab as AttackRanged;

                //Anything to do here?
                
            }

            if(ab is AttackAOE)
            {
                AttackAOE abAOE = ab as AttackAOE;
                AddIndentedString(attackString, $"Blast Radius: {abAOE.BlastRadius}", indentLevel);
                AddIndentedString(attackString, $"Blast Angle: {abAOE.DamageAngleDegrees}", indentLevel);
            }


            if (ab.ExtraAOE != null)
            {
                AddIndentedString(attackString, "Extra AOE Data", indentLevel);
                attackString.Append(SerializeAttackData(ab.ExtraAOE, indentLevel + 1));                
            }

            return attackString.ToString();
        }


        [ModifiesMember(nameof(LoadPrefab))]
        public static T mod_LoadPrefab<T>(string filename, string assetName, bool instantiate)
            where T : UnityEngine.Object
        {
            var prefab = orig_LoadPrefab<T>(filename, assetName, instantiate);
            var asGo = prefab as GameObject;
            var ab = asGo?.GetComponent<AttackBase>();
            if (ab != null)
            {
                string abilityName = ab.name;
                if (abilityName.EndsWith("(Clone)"))
                {
                    abilityName = abilityName.Substring(0, abilityName.Length - 7);
                }

                if (AbilityActionData == null)
                {
                    IEDebug.Log("AbilityActionData is null!!!");
                    return prefab;
                }

                bool dumpAbilityData = AbilityActionData.ExportAll;

                if (!dumpAbilityData)
                {
                    if (AbilityActionData.AbilityExports.Contains(abilityName))
                    {
                        dumpAbilityData = true;
                    }
                }

                if (dumpAbilityData)
                {
                    IEDebug.Log("-------------------------------------------");
                    IEDebug.Log(SerializeAttackData(ab, 0));
                    IEDebug.Log("-------------------------------------------");
                }

                if (AbilityActionData.AbilityChanges.ContainsKey(abilityName))
                {
                    ApplyChangesToAttack(ab, AbilityActionData.AbilityChanges[abilityName]);
                    if(dumpAbilityData)
                    {
                        IEDebug.Log("-------------= CHANGED DATA =---------------");
                        IEDebug.Log(SerializeAttackData(ab, 0));
                        IEDebug.Log("--------------------------------------------");
                    }
                }
                
            }

            //if (prefab != null && prefab is AbilityProgressionTable)
            //{
            //    AbilityProgressionTable table = prefab as AbilityProgressionTable;

            //    foreach (AbilityProgressionTable.AbilityPointUnlock unlock in table.AbilityPointUnlocks)
            //    {
            //        if (unlock.Level % 2 == 0)
            //        {
            //            foreach (AbilityProgressionTable.AbilityPointUnlock.CategoryPointPair pair in unlock.CategoryPointPairs)
            //            {
            //                if (pair.Category == AbilityProgressionTable.CategoryFlag.Talent)
            //                {
            //                    pair.PointsGranted = 2;
            //                }
            //            }
            //        }
            //    }
            //}

            return prefab;
        }

        [NewMember]
        [DuplicatesBody(nameof(LoadPrefab))]
        public static UnityEngine.Object dup_LoadPrefab(string assetName, string bundlePath, Type bundleType, bool instantiate)
        {
            return null;
        }

        [ModifiesMember(nameof(LoadPrefab))]
        public static UnityEngine.Object mod_LoadPrefab(string assetName, string bundlePath, Type bundleType, bool instantiate)
        {
            UnityEngine.Object prefab = dup_LoadPrefab(assetName, bundlePath, bundleType, instantiate);
            HashSet<string> interestedInList = new HashSet<string>()
            {
                "FighterAbilityProgressionTable",
                "CipherAbilityProgressionTable",
                "ChanterAbilityProgressionTable",
                "PaladinAbilityProgressionTable",
                "WizardAbilityProgressionTable",
                "MonkAbilityProgressionTable",
                "RogueAbilityProgressionTable",
                "BarbarianAbilityProgressionTable",
                "PriestAbilityProgressionTable",
                "DruidAbilityProgressionTable",
            };
            bool interestedIn = interestedInList.Contains(assetName);


            if (prefab != null && prefab is AbilityProgressionTable)
            {
                AbilityProgressionTable table = prefab as AbilityProgressionTable;

                if (interestedIn)
                {
                    IEDebug.Log(string.Format("Found AbilityProgressionTable, name = {1}, ability unlocks count = {0}", table.AbilityPointUnlocks.Length.ToString(), assetName));
                }


                foreach (AbilityProgressionTable.AbilityPointUnlock unlock in table.AbilityPointUnlocks)
                {
                    if (interestedIn)
                    {
                        IEDebug.Log(string.Format("Found unlock for level {0}", unlock.Level));
                    }
                    foreach (AbilityProgressionTable.AbilityPointUnlock.CategoryPointPair pair in unlock.CategoryPointPairs)
                    {
                        if (interestedIn)
                        {
                            IEDebug.Log(string.Format("Unlock: Type = {0}, Points = {1}", pair.Category.ToString(), pair.PointsGranted.ToString()));
                        }

                        if (pair.Category == AbilityProgressionTable.CategoryFlag.Talent)
                        {
                            pair.PointsGranted = 2;
                        }
                    }

                }
                if (interestedIn)
                {
                    foreach (AbilityProgressionTable.UnlockableAbility abilityUnlock in table.AbilityUnlocks)
                    {
                        IEDebug.Log($"Ability Unlock, Category: {abilityUnlock.Category}, Style: {abilityUnlock.UnlockStyle}, Name: {abilityUnlock.Ability.name}");
                        if (abilityUnlock.RequirementSets != null)
                        {
                            IEDebug.Log($"Ability Requirement Count = {abilityUnlock.RequirementSets.Length}");
                            foreach (AbilityProgressionTable.AbilityRequirements requirement in abilityUnlock.RequirementSets)
                            {
                                IEDebug.Log($"Level {requirement.MinimumLevel} - {requirement.MaximumLevel}");
                                IEDebug.Log($"MustBePC = {requirement.MustBePlayerCharacter}");
                                IEDebug.Log($"Class = {requirement.Class}");
                                IEDebug.Log($"Abilities = {requirement.Abilities.Length}");
                                IEDebug.Log($"Attributes = {requirement.Attributes.Length}");
                            }
                        }
                    }
                }

            }

            return prefab;
        }

    }

 


}
