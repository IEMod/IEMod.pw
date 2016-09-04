
/// Requires DLC, Take it out if compiling for non-DLC DLL.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.SoulBoundCrafting
{
    [ModifiesType]
    public class mod_UIItemInspectManager : UIItemInspectManager
    {
        [ModifiesMember("Reload")]
        public new void Reload()
        {
            CharacterStats characterStat;
            UIWidget uIWidget;
            bool flag;
            AttackBase attackBase;
            AttackBase attackBase1;
            this.m_NeedsReload = false;
            this.DragPanel.ResetPosition();
            StringBuilder stringBuilder = new StringBuilder();
            GenericAbility component = null;
            AttackBase component1 = null;
            if (this.InspectionObject)
            {
                component1 = this.InspectionObject.GetComponent<AttackBase>();
                component = this.InspectionObject.GetComponent<GenericAbility>();
                if (component1)
                {
                    component1.UICleanStatusEffects();
                }
                if (component)
                {
                    component.UICleanStatusEffects();
                }
            }
            this.EnchantButton.gameObject.SetActive(false);
            this.CompareButton.gameObject.SetActive(false);
            this.LearnSpellButton.gameObject.SetActive(false);
            this.ExamineButton.gameObject.SetActive(false);
            this.SoulbindButton.gameObject.SetActive(false);
            this.EnchantParent.gameObject.SetActive(false);
            this.ItemTypeLabel.text = string.Empty;
            this.ImageTexture.mainTexture = null;
            this.LargeImageTexture.mainTexture = null;
            string empty = string.Empty;
            this.TitleSepAnchor.widgetContainer = this.IconBackground;
            this.TitleSepAnchor.side = UIAnchor.Side.Right;
            if (this.InspectionObject == null)
            {
                this.TitleLabel.text = string.Empty;
                this.EffectTextLabel.text = string.Empty;
                this.ImageTexture.alpha = 0f;
                this.DragPanel.ResetPosition();
                this.ButtonsGrid.Reposition();
                return;
            }
            CharacterStats characterStat1 = this.InspectionObject.GetComponent<CharacterStats>();
            if (this.m_InspectStat == StatusEffect.ModifiedStat.NoEffect)
            {
                Item item = this.InspectionObject.GetComponent<Item>();
                Phrase phrase = this.InspectionObject.GetComponent<Phrase>();
                GenericTalent genericTalent = this.InspectionObject.GetComponent<GenericTalent>();
                EquipmentSoulbind equipmentSoulbind = this.InspectionObject.GetComponent<EquipmentSoulbind>();
                ItemMod itemMod = this.InspectionObject.GetComponent<ItemMod>();
                if (item)
                {
                    this.LargeImageTexture.alpha = 1f;
                    this.LargeImageTexture.mainTexture = item.GetIconLargeTexture();
                    this.LargeImageTexture.MakePixelPerfect();
                    this.TitleLabel.text = item.Name;
                    if (item.DescriptionText.IsValidString)
                    {
                        stringBuilder.AppendLine(item.DescriptionText.GetText());
                        stringBuilder.AppendLine();
                    }
                }
                if (!equipmentSoulbind || !this.ObjectOwner)
                {
                    this.SoulbindButton.gameObject.SetActive(false);
                }
                else
                {
                    this.SoulbindButton.gameObject.SetActive((!equipmentSoulbind.IsBound ? true : !equipmentSoulbind.CannotUnbind));
                }
                if (equipmentSoulbind)
                {
                    this.SoulbindButton.Label.GetComponent<GUIStringLabel>().SetString((!equipmentSoulbind.IsBound ? 2030 : 2031));
                    string extraDescription = equipmentSoulbind.GetExtraDescription();
                    if (!string.IsNullOrEmpty(extraDescription))
                    {
                        stringBuilder.AppendLine(extraDescription);
                        stringBuilder.AppendLine();
                    }
                    empty = equipmentSoulbind.GetPencilSketch();
                }
                if (this.InspectionObject.GetComponent<QuestAsset>())
                {
                    this.ExamineButton.gameObject.SetActive(true);
                }
                Equippable equippable = item as Equippable;
                if (equippable)
                {
                    BackerContent backerContent = this.InspectionObject.GetComponent<BackerContent>();
                    if (backerContent)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine();
                        stringBuilder.Append(GUIUtils.GetText(994));
                        stringBuilder.Append(" ");
                        stringBuilder.Append(backerContent.BackerName);
                    }
                    bool flag1 = (this.InspectionObject.GetComponent<Shield>() || this.InspectionObject.GetComponent<Armor>() || equippable is Weapon ? !equipmentSoulbind : false);
                    bool flag2 = (!flag1 ? false : !equippable.IsPrefab);
                    if (equippable.EquippedOwner)
                    {
                        CharacterStats component2 = equippable.EquippedOwner.GetComponent<CharacterStats>();
                        if (component2)
                        {
                            if (!component2.IsEquipmentLocked)
                            {
                                Equipment equipment = component2.GetComponent<Equipment>();
                                if (equipment && equipment.IsSlotLocked(equippable.EquippedSlot))
                                {
                                    flag2 = false;
                                }
                            }
                            else
                            {
                                flag2 = false;
                            }
                        }
                    }
                    characterStat = (!UILootManager.Instance || !UILootManager.Instance.IsVisible ? UIInventoryManager.Instance.SelectedCharacter : UILootManager.Instance.SelectedCharacter);
                    if (!characterStat || this.NoCompare)
                    {
                        this.CompareButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        Equipment equipment1 = characterStat.GetComponent<Equipment>();
                        IEnumerable<Item.UIEquippedItem> comparisonTargets = UIInventoryEquipment.GetComparisonTargets(this.InspectionObject.GetComponent<Equippable>(), equipment1);
                        this.CompareButton.gameObject.SetActive((!equipment1 || !comparisonTargets.Any<Item.UIEquippedItem>() ? false : !equipment1.CurrentItems.Contains<Equippable>(equippable)));
                    }
                    if (flag1 && this.LblEnchantValue && this.LblEnchantValue)
                    {
                        this.LblEnchantLabel.text = string.Concat(GUIUtils.GetText(1987), ": ");
                        this.LblEnchantValue.text = GUIUtils.Format(451, new object[] { equippable.TotalItemModValue(), ItemMod.MaximumModValue });
                    }
                    this.EnchantParent.gameObject.SetActive(flag1);
                    flag = (!flag2 || this.m_IsStore || this.m_NoEnchant ? false : !GameState.InCombat);
                    this.EnchantButton.gameObject.SetActive(flag);

                    //Start of mod
                    if (equipmentSoulbind) {
                        this.EnchantButton.gameObject.SetActive(true);
                    }
                    //End of mod

                    string equippableItemType = UIItemInspectManager.GetEquippableItemType(this.InspectionObject, null, equippable);
                    if (equippableItemType.Length > 0)
                    {
                        this.ItemTypeLabel.text = equippableItemType;
                    }
                }
                else if (phrase)
                {
                    this.ImageTexture.alpha = 1f;
                    if (phrase.Icon)
                    {
                        this.ImageTexture.mainTexture = phrase.Icon;
                    }
                    this.ImageTexture.MakePixelPerfect();
                    this.TitleLabel.text = phrase.DisplayName.GetText();
                    if (phrase.Description.IsValidString)
                    {
                        stringBuilder.AppendLine(phrase.Description.GetText());
                    }
                }
                else if (component)
                {
                    this.ImageTexture.alpha = 1f;
                    if (component.Icon)
                    {
                        this.ImageTexture.mainTexture = component.Icon;
                    }
                    this.ImageTexture.MakePixelPerfect();
                    if (!item)
                    {
                        this.TitleLabel.text = GenericAbility.Name(component);
                    }
                    if (component.Description.IsValidString && !item)
                    {
                        stringBuilder.AppendLine(component.Description.GetText());
                    }
                    if (component1)
                    {
                        this.ItemTypeLabel.text = component1.GetKeywordsString();
                    }
                    this.LearnSpellButton.gameObject.SetActive(this.LearnSpellAllowed);
                }
                else if (genericTalent)
                {
                    this.ImageTexture.alpha = 1f;
                    if (genericTalent.Icon)
                    {
                        this.ImageTexture.mainTexture = genericTalent.Icon;
                    }
                    this.ImageTexture.MakePixelPerfect();
                    if (genericTalent.Description.IsValidString)
                    {
                        stringBuilder.AppendLine(genericTalent.Description.GetText());
                    }
                    this.TitleLabel.text = genericTalent.Name(this.ObjectOwner);
                }
                else if (!itemMod)
                {
                    BackerContent backerContent1 = this.InspectionObject.GetComponent<BackerContent>();
                    if (backerContent1)
                    {
                        this.ImageTexture.alpha = 0f;
                        this.TitleLabel.text = backerContent1.BackerName;
                        stringBuilder.AppendLine(backerContent1.BackerDescription.GetText());
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine();
                        stringBuilder.Append(GUIUtils.GetText(994, CharacterStats.GetGender(backerContent1)));
                        stringBuilder.Append(' ');
                        stringBuilder.Append(backerContent1.BackerName);
                    }
                    if (characterStat1)
                    {
                        this.TitleLabel.text = characterStat1.Name();
                    }
                }
                else
                {
                    this.TitleLabel.text = itemMod.DisplayName.GetText();
                }
                StringEffects stringEffect = new StringEffects();
                string str = UIItemInspectManager.GetEffectText(this.InspectionObject, this.ObjectOwner, stringEffect, false).TrimEnd(new char[0]);
                this.StringEffectDisplay.Load(stringEffect);
                if (!this.StringEffectDisplay.Empty)
                {
                    str = string.Concat(str, "\n", GUIUtils.GetText(1604));
                }
                if (item && !item.IsQuestItem && !(item is CampingSupplies) && !(item is Currency))
                {
                    string empty1 = string.Empty;
                    if (this.ItemTypeLabel.text.Length > 0)
                    {
                        empty1 = string.Concat(empty1, "\n");
                    }
                    empty1 = string.Concat(empty1, GUIUtils.GetText(1499), ": ", GUIUtils.Format(466, new object[] { item.GetDefaultSellValue() }));
                    this.ItemTypeLabel.text = string.Concat(this.ItemTypeLabel.text, empty1);
                }
                if (Glossary.Instance)
                {
                    str = Glossary.Instance.AddUrlTags(str);
                }
                this.EffectTextLabel.text = str.Trim();
                if (this.Goals)
                {
                    this.Goals.Set(equipmentSoulbind, this.SoulbindUnlockMode);
                }
                this.FlavorTextLabel.text = stringBuilder.ToString().TrimEnd(new char[0]);
            }
            else
            {
                CharacterStats.SkillType skillType = StatusEffect.ModifiedStatToSkillType(this.m_InspectStat);
                CharacterStats.AttributeScoreType attributeScoreType = StatusEffect.ModifiedStatToAttributeScoreType(this.m_InspectStat);
                CharacterStats.DefenseType defenseType = StatusEffect.ModifiedStatToDefenseType(this.m_InspectStat);
                string str1 = string.Empty;
                if (characterStat1)
                {
                    if (skillType != CharacterStats.SkillType.Count)
                    {
                        this.TitleLabel.text = GUIUtils.GetSkillTypeString(skillType);
                        str1 = string.Concat(characterStat1.CalculateSkill(skillType).ToString(), GUIUtils.Format(1731, new object[] { UICharacterSheetContentManager.GetSkillEffectsInverted(characterStat1, skillType, GUIUtils.Comma(), UIGlobalColor.LinkStyle.NONE) }));
                        this.FlavorTextLabel.text = GUIUtils.GetSkillTypeDescriptionString(skillType);
                    }
                    else if (attributeScoreType != CharacterStats.AttributeScoreType.Count)
                    {
                        this.TitleLabel.text = GUIUtils.GetAttributeScoreTypeString(attributeScoreType);
                        str1 = string.Concat(characterStat1.GetAttributeScore(attributeScoreType).ToString(), GUIUtils.Format(1731, new object[] { UICharacterSheetContentManager.GetAttributeEffectsInverted(characterStat1, attributeScoreType, GUIUtils.Comma(), UIGlobalColor.LinkStyle.NONE) }));
                        this.FlavorTextLabel.text = GUIUtils.GetAttributeScoreDescriptionString(attributeScoreType);
                    }
                    else if (defenseType != CharacterStats.DefenseType.None)
                    {
                        this.TitleLabel.text = GUIUtils.GetDefenseTypeString(defenseType);
                        str1 = string.Concat(characterStat1.CalculateDefense(defenseType).ToString(), GUIUtils.Format(1731, new object[] { UICharacterSheetContentManager.GetDefenseEffectsInverted(characterStat1, defenseType, GUIUtils.Comma(), UIGlobalColor.LinkStyle.NONE) }));
                        this.FlavorTextLabel.text = GUIUtils.GetDefenseTypeDescription(defenseType);
                    }
                    else if (this.m_InspectStat == StatusEffect.ModifiedStat.InterruptBonus)
                    {
                        this.TitleLabel.text = StringTableManager.GetText(DatabaseString.StringTableType.Cyclopedia, 173);
                        str1 = string.Concat(characterStat1.ComputeInterruptHelper().ToString("#0"), GUIUtils.Format(1731, new object[] { UICharacterSheetContentManager.GetInterruptEffectsInverted(characterStat1, GUIUtils.Comma(), UIGlobalColor.LinkStyle.NONE) }));
                        this.FlavorTextLabel.text = StringTableManager.GetText(DatabaseString.StringTableType.Cyclopedia, 174);
                    }
                    else if (this.m_InspectStat == StatusEffect.ModifiedStat.ConcentrationBonus)
                    {
                        this.TitleLabel.text = StringTableManager.GetText(DatabaseString.StringTableType.Cyclopedia, 159);
                        str1 = string.Concat(characterStat1.ComputeConcentrationHelper().ToString("#0"), GUIUtils.Format(1731, new object[] { UICharacterSheetContentManager.GetConcentrationEffectsInverted(characterStat1, GUIUtils.Comma(), UIGlobalColor.LinkStyle.NONE) }));
                        this.FlavorTextLabel.text = StringTableManager.GetText(DatabaseString.StringTableType.Cyclopedia, 160);
                    }
                    else if (this.m_InspectStat == StatusEffect.ModifiedStat.DamageThreshhold)
                    {
                        this.TitleLabel.text = StringTableManager.GetText(DatabaseString.StringTableType.Cyclopedia, 157);
                        if (this.m_InspectDamageType != DamagePacket.DamageType.All && this.m_InspectDamageType != DamagePacket.DamageType.None)
                        {
                            UILabel titleLabel = this.TitleLabel;
                            titleLabel.text = string.Concat(titleLabel.text, GUIUtils.Format(1731, new object[] { GUIUtils.GetDamageTypeString(this.m_InspectDamageType) }));
                        }
                        float single = characterStat1.CalcDT(this.m_InspectDamageType, false);
                        str1 = string.Concat(single.ToString("#0"), GUIUtils.Format(1731, new object[] { UICharacterSheetContentManager.GetDamageThresholdEffectsInverted(characterStat1, this.m_InspectDamageType, GUIUtils.Comma(), UIGlobalColor.LinkStyle.NONE) }));
                        this.FlavorTextLabel.text = StringTableManager.GetText(DatabaseString.StringTableType.Cyclopedia, 158);
                    }
                    else if (this.m_InspectStat == StatusEffect.ModifiedStat.Damage)
                    {
                        this.TitleLabel.text = GUIUtils.GetText(428);
                        Equipment equipment2 = this.InspectionObject.GetComponent<Equipment>();
                        if (!equipment2)
                        {
                            attackBase1 = null;
                        }
                        else
                        {
                            attackBase1 = (!this.m_InspectOffhand ? equipment2.PrimaryAttack : equipment2.SecondaryAttack);
                        }
                        AttackBase attackBase2 = attackBase1;
                        DamageInfo damageInfo = new DamageInfo(null, 0f, attackBase2);
                        characterStat1.AdjustDamageForUi(damageInfo);
                        str1 = string.Concat(damageInfo.GetAdjustedDamageRangeString(), GUIUtils.Format(1731, new object[] { UICharacterSheetContentManager.GetDamageEffectsInverted(characterStat1, attackBase2, GUIUtils.Comma(), UIGlobalColor.LinkStyle.NONE) }));
                        this.FlavorTextLabel.text = StringTableManager.GetText(DatabaseString.StringTableType.Cyclopedia, 194);
                    }
                    else if (this.m_InspectStat == StatusEffect.ModifiedStat.Accuracy)
                    {
                        this.TitleLabel.text = GUIUtils.GetText(369);
                        Equipment component3 = this.InspectionObject.GetComponent<Equipment>();
                        if (!component3)
                        {
                            attackBase = null;
                        }
                        else
                        {
                            attackBase = (!this.m_InspectOffhand ? component3.PrimaryAttack : component3.SecondaryAttack);
                        }
                        AttackBase attackBase3 = attackBase;
                        int num = characterStat1.CalculateAccuracyForUi(attackBase3, null, null);
                        str1 = string.Concat(num.ToString(), GUIUtils.Format(1731, new object[] { UICharacterSheetContentManager.GetAccuracyEffectsInverted(characterStat1, attackBase3, GUIUtils.Comma(), UIGlobalColor.LinkStyle.NONE) }));
                        this.FlavorTextLabel.text = StringTableManager.GetText(DatabaseString.StringTableType.Cyclopedia, 84);
                    }
                }
                string str2 = string.Concat(CharacterStats.Name(characterStat1), ": ", str1);
                if (Glossary.Instance)
                {
                    str2 = Glossary.Instance.AddUrlTags(str2);
                }
                this.EffectTextLabel.text = str2;
            }
            if (this.NoDescription)
            {
                this.FlavorTextLabel.text = string.Empty;
            }
            if (this.PencilSketch)
            {
                this.PencilSketch.SetPath(empty);
            }
            if (this.LargeImageTexture.mainTexture)
            {
                this.ImageTexture.mainTexture = null;
            }
            this.TitleSepAnchor.pixelOffset.y = 0f;
            if (this.ImageTexture.mainTexture)
            {
                Transform iconBackground = this.IconBackground.transform;
                Vector3 imageTexture = this.ImageTexture.transform.localScale;
                Vector3 vector3 = this.ImageTexture.transform.localScale;
                iconBackground.localScale = new Vector3(imageTexture.x + 12f, vector3.y + 12f, 1f);
                if (this.TitleLabel.processedText.Contains("\n"))
                {
                    this.TitleSepAnchor.pixelOffset.y = -(float)this.TitleLabel.font.size;
                }
                this.TitleSepAnchor.widgetContainer = this.IconBackground;
            }
            else if (!this.LargeImageTexture.mainTexture)
            {
                this.TitleSepAnchor.widgetContainer = this.IconBackground;
                this.TitleSepAnchor.side = UIAnchor.Side.Left;
            }
            else
            {
                Vector3 largeImageTexture = this.LargeImageTexture.transform.localScale;
                if (largeImageTexture.x > 78f)
                {
                    float single1 = largeImageTexture.y / largeImageTexture.x;
                    largeImageTexture.x = 78f;
                    largeImageTexture.y = largeImageTexture.x * single1;
                }
                else if (largeImageTexture.y > 78f)
                {
                    float single2 = largeImageTexture.x / largeImageTexture.y;
                    largeImageTexture.y = 78f;
                    largeImageTexture.x = largeImageTexture.y * single2;
                }
                this.LargeImageTexture.transform.localScale = largeImageTexture;
                this.LargeImageBackground.transform.localScale = new Vector3(largeImageTexture.x + 12f, largeImageTexture.y + 12f, 1f);
                if (this.TitleLabel.processedText.Contains("\n"))
                {
                    this.TitleSepAnchor.pixelOffset.y = -(float)this.TitleLabel.font.size * 0.5f;
                }
                this.TitleSepAnchor.widgetContainer = this.LargeImageBackground;
            }
            this.ImageTexture.alpha = (!this.ImageTexture.mainTexture ? 0f : 1f);
            this.LargeImageTexture.alpha = (!this.LargeImageTexture.mainTexture ? 0f : 1f);
            this.IconBackground.alpha = (!this.ImageTexture.mainTexture ? 0f : 0.6666667f);
            this.LargeImageBackground.alpha = (!this.LargeImageTexture.mainTexture ? 0f : 0.6666667f);
            uIWidget = (this.ImageTexture.alpha <= 0f ? this.LargeImageTexture : this.ImageTexture);
            this.TitleLabel.GetComponent<UIShrinkOpposingWidget>().Widget = uIWidget;
            this.EffectTextLabel.gameObject.SetActive(!string.IsNullOrEmpty(this.EffectTextLabel.text));
            UIWidgetUtils.UpdateDependents(base.gameObject, 2);
            this.ButtonsGrid.Reposition();
            this.LayoutScrollArea.Reposition();
            this.DragPanel.ResetPosition();
        }
    }
}
