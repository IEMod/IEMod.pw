using Patchwork.Attributes;
using UnityEngine;
using IEMod.Helpers;

namespace IEMod.Mods.AlterAbilityMod
{
    [ModifiesType]
    public class mod_GameResources : GameResources
    {
        [NewMember]
        [DuplicatesBody(nameof(LoadPrefab))]
        public static T orig_LoadPrefab<T>(string filename, string assetName, bool instantiate)
        where T : Object
        {

            return default(T);
        }

        [ModifiesMember(nameof(LoadPrefab))]
        public static T mod_LoadPrefab<T>(string filename, string assetName, bool instantiate)
            where T : Object
        {
            var prefab = orig_LoadPrefab<T>(filename, assetName, instantiate);
            var asGo = prefab as GameObject;
            var ab = asGo?.GetComponent<AttackBase>();
            if (ab != null)
            {
                //IEDebug.Log($"Loaded prefab: {asGo.name}");
                if (ab.name.Contains("Fireball"))
                {
                    ab.DamageData.Maximum = ab.DamageData.Maximum * 1.25f;
                    ab.DamageData.Minimum = ab.DamageData.Minimum * 1.25f;
                }
                else if (ab.name.Contains("Soul_Shock"))
                {
                    ab.ExtraAOE.BlastRadius = 1.75f;
                }
                else if (ab.name.Contains("Mind_Blades"))
                {
                    ab.DamageData.Maximum = ab.DamageData.Maximum * 1.5f;
                    ab.DamageData.Minimum = ab.DamageData.Minimum * 1.5f;
                }
            }
            return prefab;
        }

    }

    //[ModifiesType]
    //public class mod_GenericAbility : GenericAbility
    //{

    //    AttackBase GetAttackBase()
    //    {
    //        //return m_AttackBase;
    //    }

    //}
}
