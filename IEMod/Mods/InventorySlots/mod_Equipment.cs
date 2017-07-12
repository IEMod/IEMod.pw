using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.InventorySlots
{
    [ModifiesType]
    public class mod_Equipement : Equipment
    {
        [ModifiesMember("HasEquipmentSlot")]
        public bool HasEquipmentSlotNew(Equippable.EquipmentSlot slot)
        {
            // Start of mod
            if(IEModOptions.AllInventorySlots)
            {
                return true;
            }
            //End of mod, rest is normal code
            CharacterStats component = base.GetComponent<CharacterStats>();
            if (slot == Equippable.EquipmentSlot.Grimoire)
            {
                return (!component ? true : component.CharacterClass == CharacterStats.Class.Wizard);
            }
            if (slot != Equippable.EquipmentSlot.Head)
            {
                if (slot != Equippable.EquipmentSlot.Pet)
                {
                    return true;
                }
                return base.GetComponent<Player>();
            }
            return (!component ? true : component.CharacterRace != CharacterStats.Race.Godlike);
        }

    }
}
