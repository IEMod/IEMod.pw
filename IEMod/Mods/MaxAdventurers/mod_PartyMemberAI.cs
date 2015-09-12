using IEMod.Mods.Options;
using Patchwork.Attributes;
using System.Reflection;

namespace IEMod.Mods.MaxAdventurers
{
    [ModifiesType]
    public class mod_PartyMemberAI : PartyMemberAI
    {
        [ToggleFieldAttributes(FieldAttributes.InitOnly)]
        [ModifiesMember(nameof(MaxAdventurers))]        
        public static int mod_MaxAdventurers;

        [NewMember()]
        public static void newUpdateMaxAdventurers()
        {
            switch (IEModOptions.MaxAdventurersCount)
            {
                case IEModOptions.MaxAdventurersOptions.Double_16:
                    MaxAdventurers = 16;
                    break;
                case IEModOptions.MaxAdventurersOptions.Triple_24:
                    MaxAdventurers = 24;
                    break;
                case IEModOptions.MaxAdventurersOptions.Quadra_32:
                    MaxAdventurers = 32;
                    break;
                case IEModOptions.MaxAdventurersOptions.OneHundredTwentyEight:
                    MaxAdventurers = 128;
                    break;
                case IEModOptions.MaxAdventurersOptions.Normal_8:
                default:
                    MaxAdventurers = 8;
                    break;
            }
        }
    }
}