using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;


namespace IEMod.Mods.PerEncounterSpells
{
    [ModifiesType]
    public class mod_SpellMax : SpellMax
    {
        //Note - naively assumes all casters will always get same progression. Not worth fixing at this time.
        [NewMember]
        public int[] GetModifiedEncounterData()
        {
            int[] result = new int[Grimoire.MaxSpellLevel];
            for (int i = 0; i < Grimoire.MaxSpellLevel; i++)
            {
                result[i] = 256;
            }

            switch (IEModOptions.PerEncounterSpellsSetting)
            {
                case IEModOptions.PerEncounterSpells.NoChange:
                default:
                    //This isn't used, but we'll fill it in anyway
                    result[0] = 9;
                    result[1] = 11;
                    result[2] = 13;
                    break;

                case IEModOptions.PerEncounterSpells.Levels_9_12:
                    result[0] = 9;
                    result[1] = 12;
                    break;

                case IEModOptions.PerEncounterSpells.Levels_6_9_12:
                    result[0] = 6;
                    result[1] = 9;
                    result[2] = 12;
                    break;

                case IEModOptions.PerEncounterSpells.Levels_8_10_12_14:
                    result[0] = 8;
                    result[1] = 10;
                    result[2] = 12;
                    result[3] = 14;
                    break;

                case IEModOptions.PerEncounterSpells.Levels_6_9_12_14:
                    result[0] = 6;
                    result[1] = 9;
                    result[2] = 12;
                    result[3] = 14;
                    break;

                case IEModOptions.PerEncounterSpells.Levels_6_8_10_12_14:
                    result[0] = 6;
                    result[1] = 8;
                    result[2] = 10;
                    result[3] = 12;
                    result[4] = 14;
                    break;

                case IEModOptions.PerEncounterSpells.Levels_4_6_8_10_12_14:
                    result[0] = 4;
                    result[1] = 6;
                    result[2] = 8;
                    result[3] = 10;
                    result[4] = 12;
                    result[5] = 14;
                    break;

                case IEModOptions.PerEncounterSpells.AllPerEncounter:
                    for (int i = 0; i < Grimoire.MaxSpellLevel; i++)
                    {
                        result[i] = 1;
                    }
                    break;

                case IEModOptions.PerEncounterSpells.AllPerRest:
                    //Body intentionally left blank!
                    break;

            }

            return result;
        }

        //Note - naively assumes all casters will always get same progression. Not worth fixing at this time.
        [ModifiesMember("GetPerEncounterCharacterLevel")]
        public int GetPerEncounterCharacterLevelNew(GameObject caster, int spellLevel)
        {
            int result = 2147483647;
            if (caster != null)
            {
                CharacterStats component = caster.GetComponent<CharacterStats>();
                if (component != null)
                {
                    switch (IEModOptions.PerEncounterSpellsSetting)
                    {
                        case IEModOptions.PerEncounterSpells.NoChange:
                            return this.SpellPerEncounterLevelLookup(component.CharacterClass, spellLevel);
                        //default behaviour
                        case IEModOptions.PerEncounterSpells.Levels_9_12:
                        case IEModOptions.PerEncounterSpells.Levels_6_9_12:
                        case IEModOptions.PerEncounterSpells.Levels_8_10_12_14:
                        case IEModOptions.PerEncounterSpells.Levels_6_9_12_14:
                        case IEModOptions.PerEncounterSpells.Levels_6_8_10_12_14:
                        case IEModOptions.PerEncounterSpells.Levels_4_6_8_10_12_14:
                        case IEModOptions.PerEncounterSpells.AllPerEncounter:
                        case IEModOptions.PerEncounterSpells.AllPerRest:
                            int[] encounterLevel = GetModifiedEncounterData();

                            return encounterLevel[spellLevel - 1];

                        default:
                            goto case IEModOptions.PerEncounterSpells.NoChange;
                    }
                }
            }
            return result;
        }

        [NewMember]
        [DuplicatesBody("GetSpellLevelNowTriggeredPerEncounter")]
        public int GetSpellLevelNowTriggeredPerEncounterOriginal(CharacterStats.Class casterClass, int prevLevel, int newLevel)
        {
            throw new DeadEndException("Can't reach this body");
        }

        //Note - naively assumes all casters will always get same progression. Not worth fixing at this time.
        [ModifiesMember("GetSpellLevelNowTriggeredPerEncounter")]
        public int GetSpellLevelNowTriggeredPerEncounterNew(CharacterStats.Class casterClass, int prevLevel, int newLevel)
        {
            switch (IEModOptions.PerEncounterSpellsSetting)
            {
                case IEModOptions.PerEncounterSpells.NoChange:
                    return GetSpellLevelNowTriggeredPerEncounterOriginal(casterClass, prevLevel, newLevel);

                //default behaviour
                case IEModOptions.PerEncounterSpells.Levels_9_12:
                case IEModOptions.PerEncounterSpells.Levels_6_9_12:
                case IEModOptions.PerEncounterSpells.Levels_8_10_12_14:
                case IEModOptions.PerEncounterSpells.Levels_6_9_12_14:
                case IEModOptions.PerEncounterSpells.Levels_6_8_10_12_14:
                case IEModOptions.PerEncounterSpells.Levels_4_6_8_10_12_14:
                case IEModOptions.PerEncounterSpells.AllPerEncounter:
                case IEModOptions.PerEncounterSpells.AllPerRest:
                    if (casterClass == CharacterStats.Class.Priest || casterClass == CharacterStats.Class.Wizard || casterClass == CharacterStats.Class.Druid)
                    {

                        int[] encounterLevel = GetModifiedEncounterData();

                        for (int j = 0; j < Grimoire.MaxSpellLevel; j++)
                        {
                            if (this.SpellCastMaxLookup(casterClass, newLevel, j + 1) > 0 && encounterLevel[j] > prevLevel && encounterLevel[j] <= newLevel)
                            {
                                return j + 1;
                            }
                        }
                    }
                    return 0;


                default:
                    goto case IEModOptions.PerEncounterSpells.NoChange;
            }
        }
    }
}
