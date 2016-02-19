using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.BonusSpellsPerDay {
	[ModifiesType]
	public class mod_SpellMax : SpellMax {
		[ModifiesMember("GetSpellCastMax")]
		public int GetSpellCastMaxNew(GameObject caster, int spellLevel) {
			int num = 2147483647;
			bool calcBonusSpells = IEModOptions.BonusSpellsPerDay;

			if (caster != null && spellLevel >= 1) {
				CharacterStats component = caster.GetComponent<CharacterStats>();
				if (component != null) {
					num = this.SpellCastMaxLookup(component.CharacterClass, component.ScaledLevel, spellLevel);
					if (num == 2147483647) {
						if (CharacterStats.IsPlayableClass(component.CharacterClass)) {
							PartyMemberAI component2 = caster.GetComponent<PartyMemberAI>();
							if (component2 != null) {
								num = 0;
							}
						}
					} else if (num == -1) {
						num = 2147483647;
					}
					if (num > 0 && num < 2147483647) {
						int bonusSpells = 0;

						if (calcBonusSpells) {
							for (int c = 14 + spellLevel; c <= component.Intellect; c += 4) {
								++bonusSpells;
							}
						}

						num += component.SpellCastBonus[spellLevel - 1] + bonusSpells;
					}
				}
			}
			return num;
		}
	}
}