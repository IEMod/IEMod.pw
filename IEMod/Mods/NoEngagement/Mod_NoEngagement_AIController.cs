using System;
using System.Linq;
using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.NoEngagement {
	[ModifiesType]
	public abstract class Mod_NoEngagement_AIController : AIController
	{
		[ModifiesMember("EngageEnemy")]
		public void EngageEnemyNew(GameObject enemy)
        {
            AIController component = enemy.GetComponent<AIController>();
            if (!component || component == this)
            {
                return;
            }
            if (!this.EngagedEnemies.Contains(enemy))
            {
                this.EngagedEnemies.Add(enemy);
            }
            GameObject owner = this.StateManager.CurrentState.Owner;
            CharacterStats characterStat = owner.GetComponent<CharacterStats>();
            if (characterStat != null)
            {
                characterStat.NotifyEngagement(enemy);
            }
            GameEventArgs gameEventArg = new GameEventArgs()
            {
                Type = GameEventType.MeleeEngaged,
                GameObjectData = new GameObject[] { owner }
            };
            component.OnEvent(gameEventArg);
            if (FogOfWar.Instance.PointVisible(owner.transform.position) && (!IEModOptions.DisableEngagement)) // added && ((Mod_GameOptions_GameMode)GameState.Mode).DisableEngagement == 0
            {
                Console.AddMessage(Console.Format(GUIUtils.GetTextWithLinks(100), new object[] { CharacterStats.NameColored(owner), CharacterStats.NameColored(enemy) }));
            }
            component.AddEngagedBy(owner);

		}

		[ModifiesMember("DisengageEnemy")]
		public void DisengageEnemyNew(GameObject enemy, AttackBase attack)
		{
            CharacterStats component = enemy.GetComponent<CharacterStats>();
            if ((component != null) && !component.IsImmuneToEngagement && !IEModOptions.DisableEngagement) // added && (Mod_GameOptions_GameMode)GameState.Mode).DisableEngagement == 0
            {
                attack.IsDisengagementAttack = true;
                attack.Launch(enemy, -1);
                UIHealthstringManager.Instance.ShowNotice(GUIUtils.GetText(2150), enemy, 2.5f);
            }
            GameObject owner = this.StateManager.CurrentState.Owner;
            this.m_disengagementTrackers.Add(new AIController.DisengagementTracker(enemy, false));
            this.StopEngagement(enemy);
            if (this.m_stats != null)
            {
                this.m_stats.NotifyEngagementBreak(enemy);
            }
            this.EnemyBreaksEngagement(enemy);
            AIController aIController = enemy.GetComponent<AIController>();
            if (aIController)
            {
                GameEventArgs gameEventArg = new GameEventArgs()
                {
                    Type = GameEventType.MeleeEngageBroken,
                    GameObjectData = new GameObject[] { owner }
                };
                aIController.OnEvent(gameEventArg);
                aIController.EnemyBreaksEngagement(owner);
            }
		}
	}

}