using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.InventorySlots
{
    [ModifiesType]
    public class mod_CharacterStats : CharacterStats
    {

        public int MaxQuickSlotsNew
        {
            [ModifiesMember("get_MaxQuickSlots")]
            get
            {
                if (IEModOptions.AllInventorySlots)
                {
                    return 6;
                }
                else {
                    return 4 + this.BonusQuickSlots;
                }
            }
        }

        public int MaxWeaponSetsNew
        {
            [ModifiesMember("get_MaxWeaponSets")]
            get
            {
                if (IEModOptions.AllInventorySlots)
                {
                    return 4;
                }
                else {
                    return 2 + this.BonusWeaponSets;
                }
            }
        }
    }
}


