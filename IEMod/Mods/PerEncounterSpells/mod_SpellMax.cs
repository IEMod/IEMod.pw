using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;


namespace IEMod.Mods.PerEncounterSpells {
	[ModifiesType]
	public class mod_SpellMax : SpellMax
	{
		[ModifiesMember("GetPerEncounterCharacterLevel")]
		public int GetPerEncounterCharacterLevelNew(GameObject caster, int spellLevel)
		{
			int result = 2147483647;
			if (caster != null) {
				CharacterStats component = caster.GetComponent<CharacterStats>();
				if (component != null) {
					switch (IEModOptions.PerEncounterSpellsSetting) {
						case IEModOptions.PerEncounterSpells.NoChange:
							return this.SpellPerEncounterLevelLookup(component.CharacterClass, spellLevel);
								//default behaviour
						case IEModOptions.PerEncounterSpells.Levels_9_12:
							switch (spellLevel) {
								case 1:
									return 9;
								case 2:
									return 12;
								default:
									return 256;
							}
						case IEModOptions.PerEncounterSpells.Levels_6_9_12:
							switch (spellLevel) {
								case 1:
									return 6;
								case 2:
									return 9;
								case 3:
									return 12;
								default:
									return 256;
							}
						case IEModOptions.PerEncounterSpells.Levels_6_8_10_12:
							switch (spellLevel) {
								case 1:
									return 6;
								case 2:
									return 8;
								case 3:
									return 10;
								case 4:
									return 12;
								default:
									return 256;
							}
						case IEModOptions.PerEncounterSpells.Levels_4_6_8_10_12:
							switch (spellLevel) {
								case 1:
									return 4;
								case 2:
									return 6;
								case 3:
									return 8;
								case 4:
									return 10;
								case 5:
									return 12;
								default:
									return 256;
							}
						case IEModOptions.PerEncounterSpells.AllPerEncounter:
							return 1;
						case IEModOptions.PerEncounterSpells.AllPerRest:
							return 256;
						default:
							goto case IEModOptions.PerEncounterSpells.NoChange;
					}
				}
			}
			return result;
		}
	}
}
