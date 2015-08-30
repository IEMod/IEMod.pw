using IEMod.Helpers;
using Patchwork.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEMod.Mods.ToggleWalking
{

    [ModifiesType("MappedInput")]
    public class mod_MappedInput
    {

        [NewMember]
        [DuplicatesBody(".cctor")]
        public static void StaticConstructorOriginal()
        {

        }

        [ModifiesMember(".cctor")]
        public static void StaticConstructor()
        {
            StaticConstructorOriginal();

            //Add the control we are interested in to the first category
            MappedInput.CategorizedControls[0] = new MappedControl[]
            { MappedControl.SELECT, MappedControl.MOVE, MappedControl.INTERACT, MappedControl.ATTACK, MappedControl.ATTACK_CURSOR, MappedControl.CANCEL_ACTION, MappedControl.SELECT_ALL, MappedControl.ROTATE_FORMATION, MappedControl.STEALTH_TOGGLE, MappedControl.STEALTH_ON, MappedControl.STEALTH_OFF, MappedControl.Deprecated_CHANT_EDITOR };

        }

        [NewMember]
        [DuplicatesBody("GetControlName")]
        public static string GetControlNameOrig(MappedControl control)
        {
            throw new DeadEndException("Duplicate Body, shouldn't be able to get here");
        }

        [ModifiesMember("GetControlName")]
        public static string GetControlNameNew(MappedControl control)
        {
            //WARNING - Not localization safe
            if(control == MappedControl.Deprecated_CHANT_EDITOR)
            {
                return "Toggle Walk Mode";
            }

            return GetControlNameOrig(control);
        }


    }
}
