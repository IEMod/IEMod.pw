using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patchwork.Attributes;
using IEMod.Mods.Options;
using UnityEngine;

namespace IEMod.Mods.CheatKeys
{
    [ModifiesType("GameCursor")]
    public class mod_GameCursor : GameCursor
    {
        [ModifiesMember("Update")]
        public void NewUpdate()
        {
            //Original code
            if (this.PrefabCursors == null)
            {
                return;
            }

            if (GameCursor.UnusableUnderCursor && GameCursor.UnusableUnderCursor.IsUsable)
            {
                GameCursor.GenericUnderCursor = GameCursor.UnusableUnderCursor.gameObject;
                GameCursor.UnusableUnderCursor.NotifyMouseOver(true);
                GameCursor.UnusableUnderCursor = null;
            }
            this.ValidateObjectsUnderCursors();
            if (GameCursor.ShowDebug)
            {
                this.DrawDebugCursorInfo();
            }
            for (int i = this.m_ObjectsHidingCursor.Count - 1; i >= 0; i--)
            {
                if (this.m_ObjectsHidingCursor[i].Target == null || !this.m_ObjectsHidingCursor[i].IsAlive)
                {
                    this.m_ObjectsHidingCursor.RemoveAt(i);
                }
            }
            this.ShowCursor = this.m_ObjectsHidingCursor.Count == 0;
            if (UIGlobalInventory.Instance && UIGlobalInventory.Instance.DraggingItem)
            {
                this.ShowCursor = false;
                this.DisableCursor = false;
            }
            else if (UIWindowManager.Instance && UIWindowManager.Instance.AnyWindowShowing())
            {
                this.ShowCursor = true;
            }
            if (GameCursor.m_frameCount < 2)
            {
                GameCursor.m_frameCount = GameCursor.m_frameCount + 1;
                return;
            }
            GameCursor.CursorType desiredCursor = GameCursor.DesiredCursor;
            desiredCursor = this.HandleDownState(desiredCursor, GameCursor.CursorType.Normal, GameCursor.CursorType.NormalHeld);
            desiredCursor = this.HandleDownState(desiredCursor, GameCursor.CursorType.Walk, GameCursor.CursorType.WalkHeld);
            desiredCursor = this.HandleDownState(desiredCursor, GameCursor.CursorType.Disengage, GameCursor.CursorType.DisengageHeld);
            desiredCursor = this.HandleDownState(desiredCursor, GameCursor.CursorType.SelectionSubtract, GameCursor.CursorType.SelectionSubtractHeld);
            desiredCursor = this.HandleDownState(desiredCursor, GameCursor.CursorType.SelectionAdd, GameCursor.CursorType.SelectionAddHeld);
            if (!this.ShowCursor)
            {
                return;
            }
            if (GameCursor.m_activeCursor != desiredCursor)
            {
                switch (desiredCursor)
                {
                    case GameCursor.CursorType.Normal:
                    case GameCursor.CursorType.Deprecated1:
                        {
                            Cursor.SetCursor(this.PrefabCursors.NormalCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.Walk:
                        {
                            Cursor.SetCursor(this.PrefabCursors.WalkCursor, GameCursor.m_centerHotSpot, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.NoWalk:
                        {
                            Cursor.SetCursor(this.PrefabCursors.NoWalkCursor, GameCursor.m_centerHotSpot, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.RotateFormation:
                        {
                            Cursor.SetCursor(this.PrefabCursors.RotateFormationCursor, GameCursor.m_centerHotSpot, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.Attack:
                        {
                            Cursor.SetCursor(this.PrefabCursors.AttackCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.OpenDoor:
                        {
                            Cursor.SetCursor(this.PrefabCursors.OpenDoorCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.CloseDoor:
                        {
                            Cursor.SetCursor(this.PrefabCursors.CloseDoorCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.LockedDoor:
                        {
                            Cursor.SetCursor(this.PrefabCursors.LockedDoorCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.AreaTransition:
                        {
                            Cursor.SetCursor(this.PrefabCursors.AreaTransition, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.Examine:
                        {
                            Cursor.SetCursor(this.PrefabCursors.ExamineCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.Talk:
                        {
                            Cursor.SetCursor(this.PrefabCursors.TalkCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.Interact:
                        {
                            Cursor.SetCursor(this.PrefabCursors.InteractCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.CastAbility:
                        {
                            if (!GameCursor.m_castCursorValid || !GameCursor.m_castCursor)
                            {
                                Cursor.SetCursor(this.PrefabCursors.CastAbility, Vector2.zero, CursorMode.Auto);
                            }
                            else
                            {
                                Cursor.SetCursor(GameCursor.m_castCursor, Vector2.zero, CursorMode.Auto);
                            }
                            break;
                        }
                    case GameCursor.CursorType.CastAbilityInvalid:
                        {
                            if (!GameCursor.m_castCursorValid || !GameCursor.m_castCursorInvalid)
                            {
                                Cursor.SetCursor(this.PrefabCursors.CastAbilityInvalid, Vector2.zero, CursorMode.Auto);
                            }
                            else
                            {
                                Cursor.SetCursor(GameCursor.m_castCursorInvalid, Vector2.zero, CursorMode.Auto);
                            }
                            break;
                        }
                    case GameCursor.CursorType.ArrowUp:
                    case GameCursor.CursorType.ArrowRight:
                    case GameCursor.CursorType.ArrowDown:
                    case GameCursor.CursorType.ArrowLeft:
                    case GameCursor.CursorType.ArrowUpRight:
                    case GameCursor.CursorType.ArrowDownRight:
                    case GameCursor.CursorType.ArrowUpLeft:
                    case GameCursor.CursorType.ArrowDownLeft:
                        {
                            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.DoubleArrow_L_R:
                        {
                            Cursor.SetCursor(this.PrefabCursors.DoubleArrow_L_R, GameCursor.m_centerHotSpot, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.DoubleArrow_U_D:
                        {
                            Cursor.SetCursor(this.PrefabCursors.DoubleArrow_U_D, GameCursor.m_centerHotSpot, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.DoubleArrow_DL_UR:
                        {
                            Cursor.SetCursor(this.PrefabCursors.DoubleArrow_DL_UR, GameCursor.m_centerHotSpot, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.DoubleArrow_UL_DR:
                        {
                            Cursor.SetCursor(this.PrefabCursors.DoubleArrow_UL_DR, GameCursor.m_centerHotSpot, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.Disarm:
                        {
                            Cursor.SetCursor(this.PrefabCursors.DisarmCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.CastAbilityNoLOS:
                        {
                            if (!GameCursor.m_castCursorValid || !GameCursor.m_castCursorLoS)
                            {
                                Cursor.SetCursor(this.PrefabCursors.CastAbilityNoLOS, Vector2.zero, CursorMode.Auto);
                            }
                            else
                            {
                                Cursor.SetCursor(GameCursor.m_castCursorLoS, Vector2.zero, CursorMode.Auto);
                            }
                            break;
                        }
                    case GameCursor.CursorType.CastAbilityFar:
                        {
                            if (!GameCursor.m_castCursorValid || !GameCursor.m_castCursorMove)
                            {
                                Cursor.SetCursor(this.PrefabCursors.CastAbilityFar, Vector2.zero, CursorMode.Auto);
                            }
                            else
                            {
                                Cursor.SetCursor(GameCursor.m_castCursorMove, Vector2.zero, CursorMode.Auto);
                            }
                            break;
                        }
                    case GameCursor.CursorType.Stealing:
                        {
                            Cursor.SetCursor(this.PrefabCursors.Stealing, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.Loot:
                        {
                            Cursor.SetCursor(this.PrefabCursors.LootCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.NormalHeld:
                        {
                            Cursor.SetCursor(this.PrefabCursors.NormalHeldCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.WalkHeld:
                        {
                            Cursor.SetCursor(this.PrefabCursors.WalkHeldCursor, GameCursor.m_centerHotSpot, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.SpecialAttack:
                        {
                            if (!GameCursor.m_castCursorValid || !GameCursor.m_castCursor)
                            {
                                Cursor.SetCursor(this.PrefabCursors.SpecialAttackCursor, Vector2.zero, CursorMode.Auto);
                            }
                            else
                            {
                                Cursor.SetCursor(GameCursor.m_castCursor, Vector2.zero, CursorMode.Auto);
                            }
                            break;
                        }
                    case GameCursor.CursorType.StealingLocked:
                        {
                            Cursor.SetCursor(this.PrefabCursors.StealingLocked, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.Disengage:
                        {
                            Cursor.SetCursor(this.PrefabCursors.DisengageCursor, GameCursor.m_centerHotSpot, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.DisengageHeld:
                        {
                            Cursor.SetCursor(this.PrefabCursors.DisengageHeldCursor, GameCursor.m_centerHotSpot, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.AttackAdvantage:
                        {
                            Cursor.SetCursor(this.PrefabCursors.AttackAdvantageCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.SelectionAdd:
                        {
                            Cursor.SetCursor(this.PrefabCursors.SelectionAddCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.SelectionSubtract:
                        {
                            Cursor.SetCursor(this.PrefabCursors.SelectionSubtractCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.SelectionAddHeld:
                        {
                            Cursor.SetCursor(this.PrefabCursors.SelectionAddHeldCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.SelectionSubtractHeld:
                        {
                            Cursor.SetCursor(this.PrefabCursors.SelectionSubtractHeldCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    case GameCursor.CursorType.DuplicateItem:
                        {
                            Cursor.SetCursor(this.PrefabCursors.DuplicateItemCursor, Vector2.zero, CursorMode.Auto);
                            break;
                        }
                    default:
                        {
                            goto case GameCursor.CursorType.ArrowDownLeft;
                        }
                }
                GameCursor.m_activeCursor = desiredCursor;

                //End of Original Code, Start of mod
                if (UIWindowManager.KeyInputAvailable && IEModOptions.EnableCheatKeys)
                {
                    if (GameInput.GetControlkey())
                    {
                        if (GameInput.GetKeyDown(KeyCode.J)) {
                            CheatKeyFunctions.TeleportToCursorLocation();
                            //Console.AddMessage(" CTRL + J Teleport", Color.green);
                        }
                        else if (GameInput.GetKeyDown(KeyCode.Y))
                        {
                            CheatKeyFunctions.DamageUnderCursor();
                            //Console.AddMessage(" CTRL + Y Damage", Color.green);
                        }
                        else if (GameInput.GetKeyDown(KeyCode.R))
                        {
                            CheatKeyFunctions.RestoreUnderCursor();
                            //Console.AddMessage(" CTRL + R Restore health and endurance", Color.green);
                        }
                        else if (GameInput.GetKeyDown(KeyCode.T))
                        {
                            CheatKeyFunctions.AdvanceTime();
                            //Console.AddMessage(" CTRL + T Advance time ", Color.green);
                        }
                        else if (GameInput.GetKeyDown(KeyCode.L))
                        {
                            CheatKeyFunctions.Unlock();
                            //Console.AddMessage(" CTRL + L Unlocks container", Color.green);
                        }
                        else if (GameInput.GetKeyDown(KeyCode.S))
                        {
                            CheatKeyFunctions.RestoreSpellsAndAbility();
                            //Console.AddMessage(" CTRL + S restore spell and ability usage", Color.green);
                        }
                    }
                }
                //End of mod
            }
        }
    }

}
