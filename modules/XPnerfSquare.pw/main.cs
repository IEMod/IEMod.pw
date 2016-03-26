using Patchwork.Attributes;

namespace XPNerfSquare.pw
{
    [ModifiesType]
    public class mod_CharacterStats : CharacterStats
    {
        //using dropdown box
        [ModifiesMember("ExperienceNeededForLevel")]
        public static int ExperienceNeededForLevelNew(int level)
        {
            return (level - 1) * (level - 1) * 1000;
        }

        [ModifiesMember("ExperienceNeededForNextLevel")]
        public static int ExperienceNeededForNextLevelNew(int currentLevel)
        {
            return currentLevel * currentLevel * 1000;
        }

    }
}
