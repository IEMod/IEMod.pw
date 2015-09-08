using UnityEngine;
using AI.Player;
using Patchwork.Attributes;

namespace IEMod.Mods.Bugfix2_01_Caster_Centered_Spells
{
    [ModifiesType]
    public class mod_AI_Player_Attack : Attack
    {
        [NewMember]
        [MemberAlias("Update", typeof(AI.Player.PlayerState), AliasCallMode.NonVirtual)]
        private void add_BaseUpdate()
        {
            //A call to this method will be translated to a "base.Update()" call in the target assembly.
        }



        [ModifiesMember("Update")]
        public void mod_Update()
        {
            //METHOD SHOULD BE ASSUMED TO BE THE SAME AS THE ORIGINAL EXCEPT WHERE DOCUMENTED OTHERWISE

            string abilityString = "no ability";
            if (this.GetAbility() != null)
            {
                abilityString = this.GetAbility().Name();
            }         

            bool flag;
            if (this.m_ai == null)
            {
                Debug.LogError("AI update run without OnEnter being run first!");
                return;
            }

            //CHANGE TO AVOID RECURSION
            add_BaseUpdate();
            //END CHANGE

            if (this.m_queueWeaponSetChange)
            {
                this.m_attackToUse = null;
                this.m_weaponAttack = null;
                this.m_targetTeam = null;
                this.m_targetMover = null;
                this.m_inCombat = false;
                this.m_switchHands = false;
                this.m_isStealthAttack = false;
                this.m_queueWeaponSetChange = false;
                this.OnEnter();
                return;
            }
            AIController.AggressionType autoAttackAggression = this.m_ai.GetAutoAttackAggression();
            //CHANGE TO EXEMPT SELF-RADIUS HOSTILE ATTACKS FROM FIZZLING BECAUSE WE CAN'T ATTACK OURSELVES
            if (!this.CanAttackTarget() && !this.m_attackToUse.ApplyToSelfOnly && (autoAttackAggression == AIController.AggressionType.Passive || !this.m_partyMemberAI.AutoPickNearbyEnemy(this)))
            //END CHANGE
            {
                base.Manager.PopCurrentState();
                return;
            }
            if (this.m_attackToUse == null || this.m_switchHands)
            {
                this.SetupAttack();
                this.m_switchHands = false;
                if (this.m_issuedSelfCast)
                {
                    this.m_issuedSelfCast = false;
                    return;
                }
                if (this.m_ai.StateManager.CurrentState is AI.Player.Ability)
                {
                    return;
                }
            }
            this.m_ai.UpdateEngagement(base.Owner, AIController.GetPrimaryAttack(base.Owner));
            AttackFirearm mAttackToUse = this.m_attackToUse as AttackFirearm;
            if (mAttackToUse != null && mAttackToUse.BaseIsReady() && mAttackToUse.RequiresReload)
            {
                base.PushState<ReloadWeapon>().AttackToUse = mAttackToUse;
                return;
            }
            if (base.OutOfCharges(this.m_attackToUse))
            {
                base.Manager.PopCurrentState();
                return;
            }
            if (this.m_isAutoAttack && !GameState.InCombat)
            {
                base.Manager.PopCurrentState();
                return;
            }
            Vector3 mTarget = this.m_target.transform.position - this.m_owner.transform.position;
            float single = mTarget.magnitude;
            float radius = this.m_ai.Mover.Radius + this.m_targetMover.Radius;
            single = single - radius;
            if (this.m_attackToUse == null)
            {
                return;
            }
            if (single <= this.m_attackToUse.TotalAttackDistance && (!(this.m_attackToUse is AttackRanged) || base.LineOfSightToTarget(this.m_target)))
            {
                if (this.m_target != null && !this.m_attackToUse.ApplyToSelfOnly && !this.m_attackToUse.IsValidTarget(this.m_target))
                {
                    base.Manager.PopCurrentState();
                    return;
                }
                if (this.m_target == null)
                {
                    this.m_inCombat = GameState.InCombat;
                }
                else
                {
                    Faction component = this.m_target.GetComponent<Faction>();
                    if (component != null && (this.m_faction.IsHostile(component) || component.IsHostile(this.m_faction)))
                    {
                        this.m_inCombat = GameState.InCombat;
                    }
                }
                if (!this.m_attackToUse.IsReady())
                {
                    this.m_ai.FaceTarget(this.m_attackToUse);
                    return;
                }
                if (this.m_attackToUse.ApplyToSelfOnly && !this.m_isAutoAttack)
                {
                    base.Manager.PopCurrentState();
                }
                if (this.m_isStealthAttack)
                {
                    this.m_attackToUse.IsStealthAttack = this.m_isStealthAttack;
                }
                this.m_isStealthAttack = Stealth.IsInStealthMode(this.m_owner);
                AI.Achievement.Attack mEffectsOnLaunch = base.PushState<AI.Achievement.Attack>();
                mEffectsOnLaunch.Parameters.Attack = this.m_attackToUse;
                mEffectsOnLaunch.Parameters.TargetObject = this.m_target;
                mEffectsOnLaunch.Parameters.EffectsOnLaunch = this.m_effectsOnLaunch;
                mEffectsOnLaunch.Parameters.WeaponAttack = this.m_weaponAttack;
                mEffectsOnLaunch.Parameters.Ability = this.GetAbility();
                mEffectsOnLaunch.Parameters.ShouldAttackObject = mEffectsOnLaunch.Parameters.TargetObject != null;
                this.m_switchHands = true;
                return;
            }
            if (this.m_isAutoAttack && this.m_ai.EngagedBy.Count > 0)
            {
                if (autoAttackAggression != AIController.AggressionType.Passive && this.m_partyMemberAI.AutoPickNearbyEnemy(this))
                {
                    return;
                }
                base.Manager.PopCurrentState();
                return;
            }
            if (this.m_target == null)
            {
                base.Manager.PopCurrentState();
                return;
            }
            float totalAttackDistance = this.m_attackToUse.TotalAttackDistance;
            if (totalAttackDistance > 4f)
            {
                totalAttackDistance = totalAttackDistance - 1f;
            }
            else if (totalAttackDistance > 1f)
            {
                totalAttackDistance = totalAttackDistance - 0.5f;
            }
            else if (totalAttackDistance > 0.4f)
            {
                totalAttackDistance = totalAttackDistance - 0.1f;
            }
            AI.Achievement.PathToPosition pathToPosition = base.PushState<AI.Achievement.PathToPosition>();
            pathToPosition.Parameters.Target = this.m_target;
            pathToPosition.Parameters.StopOnLOS = true;
            pathToPosition.Parameters.Range = totalAttackDistance;
            pathToPosition.ParentState = this;
            if (this.m_attackToUse is AttackMelee && totalAttackDistance > 1f && totalAttackDistance < 2f)
            {
                pathToPosition.Parameters.IgnoreObstaclesWithinRange = true;
            }
            pathToPosition.Parameters.PopOnEnterIfTargetInvalid = true;
            bool mAttackToUse1 = this.m_attackToUse is AttackRanged;
            AttackMelee attackMelee = this.m_attackToUse as AttackMelee;
            AI.Achievement.PathToPosition.Params parameters = pathToPosition.Parameters;
            if (mAttackToUse1)
            {
                flag = false;
            }
            else
            {
                flag = (attackMelee == null ? true : attackMelee.TotalAttackDistance < 0.01f);
            }
            parameters.GetAsCloseAsPossible = flag;
            pathToPosition.Parameters.DesiresMaxRange = mAttackToUse1;
        }

    }
}
