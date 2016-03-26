using Patchwork.Attributes;

namespace XPNerf50.pw
{
    [ModifiesType]
    public class mod_CharacterStats : CharacterStats
    {
        //using dropdown box
        [ModifiesMember("ExperienceNeededForLevel")]
        public static int ExperienceNeededForLevelNew(int level)
        {
            return (level - 1) * level * 750;
        }

        [ModifiesMember("ExperienceNeededForNextLevel")]
        public static int ExperienceNeededForNextLevelNew(int currentLevel)
        {
            return currentLevel * (currentLevel + 1) * 750;
        }

    }
}
