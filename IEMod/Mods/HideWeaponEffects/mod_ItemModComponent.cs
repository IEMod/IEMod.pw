using IEMod.Helpers;
using IEMod.Mods.Options;
using System.Collections.Generic;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.HideWeaponEffects
{
    [ModifiesType]
    public class mod_ItemModComponent : ItemModComponent
    {
        [ModifiesMember("ApplyEquipFX")]
        /// <summary>
        /// Only difference here is the addition of a condition that checks if the checkbox in the IEmod options is disabled
        /// before adding the effect to the list
        /// </summary>
        public void ApplyEquipFXNew(Transform t, List<GameObject> fx_list)
        {
            if (this.m_mod.OnEquipVisualEffect != null && !IEModOptions.HideWeaponEffects)  //Added comparison here
            {
                GameObject gameObject = GameUtilities.LaunchLoopingEffect(this.m_mod.OnEquipVisualEffect, 1f, t, null);
                if (gameObject != null && fx_list != null)
                {
                    fx_list.Add(gameObject);
                }
            }
        }
    }
}
