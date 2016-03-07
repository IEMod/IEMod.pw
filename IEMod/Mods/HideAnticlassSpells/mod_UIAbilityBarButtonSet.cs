using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.HideAntiClassSpells
{
    [ModifiesType]
    public class mod_UIAbilityBarButtonSet : UIAbilityBarButtonSet
    {
        [ModifiesMember(nameof(SetButtonsAnticlassSpells))]
        public void mod_SetButtonsAnticlassSpells(CharacterStats stats)
        {
            this.Label = null;
            int num = 0;
            foreach (GenericAbility current in stats.Abilities)
            {
                if (current is GenericSpell)
                {
                    CharacterStats.Class spellClass = ((GenericSpell)current).SpellClass;
                    if (spellClass != stats.CharacterClass || !CharacterStats.IsPlayableClass(spellClass))
                    {

                        if ((stats.CharacterClass == CharacterStats.Class.Wizard ||
                            stats.CharacterClass == CharacterStats.Class.Priest ||
                            stats.CharacterClass == CharacterStats.Class.Druid ||
                            stats.CharacterClass == CharacterStats.Class.Chanter ||
                            stats.CharacterClass == CharacterStats.Class.Cipher) && IEModOptions.HideAnticlassSpells)
                        {

                        }

                        else
                        {
                            this.SetButton(num, current.gameObject, UIAbilityBarButtonSet.AbilityButtonAction.CAST_SPELL_ABILITY, current.Icon);
                            num++;
                        }
                    }
                }
            }
            this.HideButtons(num);
            this.m_DoRefresh = true;
        }

        [ModifiesMember(nameof(ShowOnSpellBar))]
        public static bool mod_ShowOnSpellBar(GenericAbility ability, CharacterStats stats, int spellLevel)
        {
            if (ability.Passive)
            {
                return false;
            }
            GenericSpell genericSpell = ability as GenericSpell;
            GenericCipherAbility genericCipherAbility = ability as GenericCipherAbility;
            if (!genericSpell)
            {
                return genericCipherAbility && (spellLevel <= 0 || genericCipherAbility.SpellLevel == spellLevel);
            }

            if (genericSpell.SpellClass != stats.CharacterClass && spellLevel > 0 && genericSpell.SpellLevel == spellLevel && IEModOptions.HideAnticlassSpells)
            {
                return true;
            }

            if (genericSpell.SpellClass != stats.CharacterClass || (spellLevel > 0 && genericSpell.SpellLevel != spellLevel))
            {
                return false;
            }
            if (stats.CharacterClass != CharacterStats.Class.Wizard || !genericSpell.NeedsGrimoire)
            {
                return true;
            }
            Equipment component = stats.GetComponent<Equipment>();
            if (component == null || component.CurrentItems == null || component.CurrentItems.Grimoire == null)
            {
                return false;
            }
            Grimoire component2 = component.CurrentItems.Grimoire.GetComponent<Grimoire>();
            return !(component2 == null) && component2.HasSpell(genericSpell);
        }
    }
}
