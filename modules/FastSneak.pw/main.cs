using System;
using AI.Achievement;
using AI.Player;
using Patchwork.Attributes;
using UnityEngine;
using Attack = AI.Achievement.Attack;

namespace FastSneak.pw
{
    [ModifiesType]
    public class mod_PartyMemberAI : PartyMemberAI
    {
        [MemberAlias("Update", typeof(AIController), AliasCallMode.NonVirtual)]
        private void BaseUpdate()
        {
            //A call to this method will be translated to a "base.Update()" call in the target assembly.
        }

        [ModifiesMember("Update")]
        // PartyMemberAI
        public void UpdateNew()
        {
            if (this.m_mover != null && this.m_mover.AIController == null)
            {
                this.m_mover.AIController = this;
            }
            if (this.m_instructionTimer > 0f)
            {
                PartyMemberAI mInstructionTimer = this;
                mInstructionTimer.m_instructionTimer = mInstructionTimer.m_instructionTimer - Time.deltaTime;
            }
            if (this.m_instructions != null)
            {
                for (int i = 0; i < this.m_instructions.Count; i++)
                {
                    this.m_instructions[i].Update();
                }
            }
            if (GameState.s_playerCharacter != null && base.gameObject == GameState.s_playerCharacter.gameObject && PartyMemberAI.DebugParty)
            {
                UIDebug.Instance.SetText("Party Debug", PartyMemberAI.GetPartyDebugOutput(), Color.cyan);
                UIDebug.Instance.SetTextPosition("Party Debug", 0.95f, 0.95f, UIWidget.Pivot.TopRight);
            }
            if (this.m_destinationCircleState != null)
            {
                if (!base.StateManager.IsStateInStack(this.m_destinationCircleState))
                {
                    this.m_destinationCircleState = null;
                    this.HideDestination();
                }
                else
                {
                    this.ShowDestination(this.m_destinationCirclePosition);
                }
            }
            if (!(GameState.s_playerCharacter != null) || !GameState.s_playerCharacter.RotatingFormation || !this.Selected)
            {
                this.HideDestinationTarget();
            }
            else
            {
                this.ShowDestinationTarget(this.m_desiredFormationPosition);
            }
            if (this.m_revealer == null)
            {
                this.CreateFogRevealer();
            }
            else
            {
                this.m_revealer.WorldPos = base.gameObject.transform.position;
                this.m_revealer.RequiresRefresh = false;
            }
            if (GameState.Paused)
            {
                base.CheckForNullEngagements();
                if (this.m_ai != null)
                {
                    this.m_ai.Update();
                }
                return;
            }
            if (this.m_ai == null)
            {
                return;
            }
            if (GameState.Option.AutoPause.IsEventSet(AutoPauseOptions.PauseEvent.EnemySpotted))
            {
                this.UpdateEnemySpotted();
            }
            if (this.QueuedAbility != null && this.QueuedAbility.Ready)
            {
                AIState currentState = this.m_ai.CurrentState;
                Consumable component = this.QueuedAbility.GetComponent<Consumable>();
                if (!(component != null) || !component.IsFoodDrugOrPotion)
                {
                    Attack attack = currentState as Attack;
                    TargetedAttack targetedAttack = currentState as TargetedAttack;
                    if (this.QueuedAbility.Passive || attack == null && targetedAttack == null)
                    {
                        this.QueuedAbility.Activate(currentState.Owner);
                    }
                    else if (targetedAttack == null || !this.QueuedAbility.UsePrimaryAttack && !this.QueuedAbility.UseFullAttack)
                    {
                        Ability queuedAbility = AIStateManager.StatePool.Allocate<Ability>();
                        queuedAbility.QueuedAbility = this.QueuedAbility;
                        if (attack == null)
                        {
                            base.StateManager.PushState(queuedAbility);
                        }
                        else if (!attack.CanCancel)
                        {
                            base.StateManager.QueueStateAtTop(queuedAbility);
                        }
                        else
                        {
                            attack.OnCancel();
                            base.StateManager.PopCurrentState();
                            base.StateManager.PushState(queuedAbility);
                        }
                    }
                }
                else
                {
                    ConsumePotion queuedState = this.m_ai.QueuedState as ConsumePotion;
                    if (!(currentState is ConsumePotion) && (queuedState == null || currentState.Priority < 1))
                    {
                        ConsumePotion animationVariation = AIStateManager.StatePool.Allocate<ConsumePotion>();
                        base.StateManager.PushState(animationVariation);
                        animationVariation.Ability = this.QueuedAbility;
                        animationVariation.ConsumeAnimation = component.AnimationVariation;
                        AttackBase primaryAttack = this.GetPrimaryAttack();
                        if (!(primaryAttack is AttackMelee) || !(primaryAttack as AttackMelee).Unarmed)
                        {
                            animationVariation.HiddenObjects = primaryAttack.GetComponentsInChildren<Renderer>();
                        }
                    }
                }
                this.QueuedAbility = null;
            }
            BaseUpdate();
            if (GameState.IsLoading || GameState.s_playerCharacter == null)
            {
                return;
            }
            if (this.m_alphaControl != null && this.m_alphaControl.Alpha < 1.401298E-45f)
            {
                this.m_alphaControl.Alpha = 1f;
            }
            if (this.m_mover != null && !GameState.InCombat)
            {
                bool fastSneakActive = false;
                bool canSeeEnemy = false;
                bool flag = true;
                for (int i = 0; i < PartyMemberAI.PartyMembers.Length; ++i)
                {
                    var p = PartyMemberAI.PartyMembers[i];
                    if (p != null && p.m_enemySpotted)
                    {
                        canSeeEnemy = true;
                        break;
                    }
                }
                
                if (!canSeeEnemy)
                        fastSneakActive = true;

                if (Stealth.IsInStealthMode(base.gameObject) && !fastSneakActive)
                        flag = false;

                this.m_mover.UseCustomSpeed((flag ? 4f : 2f));
            }
        }
    }
}