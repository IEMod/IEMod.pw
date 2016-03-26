using Patchwork.Attributes;

namespace XPNerf25.pw
{
    [ModifiesType]
    public class mod_CharacterStats : CharacterStats
    {
        //using dropdown box
        [ModifiesMember("ExperienceNeededForLevel")]
        public static int ExperienceNeededForLevelNew(int level)
        {
            return (level - 1) * level * 625;
        }

        [ModifiesMember("ExperienceNeededForNextLevel")]
        public static int ExperienceNeededForNextLevelNew(int currentLevel)
        {
            return currentLevel * (currentLevel + 1) * 625;
        }

    }
}
