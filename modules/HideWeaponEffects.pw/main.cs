using Patchwork.Attributes;
using UnityEngine;
using System.Collections.Generic;

namespace HideWeaponEffects.pw
{
 [ModifiesType]
    public class mod_ItemModComponent : ItemModComponent
    {
        [ModifiesMember("ApplyEquipFX")]
        /// <summary>
        /// Only difference here is the addition of a condition that checks if the checkbox in the IEmod options is disabled
        /// before adding the effect to the list
        /// 
        /// Update since module form... Now only removes the code to prevent effect from applying
        /// 
        /// </summary>
        public void ApplyEquipFXNew(Transform t, List<GameObject> fx_list)
        {
            if (this.m_mod.OnEquipVisualEffect != null )  //Added comparison here
            {
            }
        }
    }
}
