using Patchwork.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace IEMod.Mods.CheatKeys
{
    /// <summary>
    /// Utility class that stores the cheat key functions
    /// These could be called in any update method, but GameCursor was chosen for some reason...
    /// </summary>
    [NewType]
    public static class CheatKeyFunctions
    {

        /// <summary>
        /// Teleport to cursor location, like old IE games with Ctrl + J
        /// Stolen from Mod_Commandline
        /// </summary>
        public static void TeleportToCursorLocation() {
            
            if (GameState.s_playerCharacter.IsMouseOnWalkMesh())
            {
                foreach (var partymember in PartyMemberAI.GetSelectedPartyMembers())
                {
                    partymember.transform.position = GameInput.WorldMousePosition;
                }
            }
            else
            {
                global::Console.AddMessage("Mouse is not on navmesh !", Color.red);
            }
        }

        /// <summary>
        /// Kills the unlucky character under the cursor
        /// Stolen from Script DealDamage
        /// </summary>
        public static void DamageUnderCursor()
        {
            GameObject underCursor = GameCursor.CharacterUnderCursor;

            Health healthComponent = underCursor.GetComponent<Health>();

            if (healthComponent){
                bool canBeTargeted = !healthComponent.CanBeTargeted;
                healthComponent.CanBeTargeted = true;
                healthComponent.ApplyDamageDirectly(50000f);
                if (canBeTargeted)
                {
                    healthComponent.CanBeTargeted = false;
                }
            }

        }

        /// <summary>
        /// Restore health of lucky character under the cursor
        /// Inspired from Script HealParty
        /// </summary>
        public static void RestoreUnderCursor()
        {

            GameObject underCursor = GameCursor.CharacterUnderCursor;
            CharacterStats stats = underCursor.GetComponent<CharacterStats>();


            Health healthComponent = underCursor.GetComponent<Health>();

            if (stats) {
                stats.HandleGameOnResting();
            }

            IEnumerator<GenericAbility> enumerator = stats.ActiveAbilities.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    GenericAbility current = enumerator.Current;
                    if (current == null)
                    {
                        continue;
                    }
                    current.RestoreCooldown();
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        /// <summary>
        /// Advances time by 8 hours
        /// Direct call to Scripts AdvanceTimeByHoursNoRest
        /// </summary>
        public static void AdvanceTime()
        {
            Scripts.AdvanceTimeByHoursNoRest(8);
        }

        /// <summary>
        /// Unlocks Container under cursor
        /// </summary>
        public static void Unlock()
        {
            GameObject underCursor = GameCursor.GenericUnderCursor;
            OCL container = underCursor.GetComponent<OCL>();

            List<GameObject> partyMembers = PartyMemberAI.GetSelectedPartyMembers();
            IEnumerator<GameObject> enumerator = partyMembers.GetEnumerator();

            try
            {
                enumerator.MoveNext();
                if (container)
                {
                    container.Unlock(enumerator.Current);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }

        }       
    }
}
