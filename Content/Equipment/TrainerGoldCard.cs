using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using Terraria.ModLoader.IO;
using Pokemon.Content.Items;
using Pokemon.Buffs;
using Terraria.Localization;


namespace Pokemon.Content.Equipment
{
    public class TrainerGoldCard : ModItem
    {
        //训练师金卡
        //加新徽章要在这里添加
        private Texture2D itemIcon; // 用于存储图标纹理
       
        public bool hasBoulderBadge = false;// 是否拥有灰色徽章
        public bool hasCascadeBadge = false;// 是否拥有蓝色徽章
        public bool hasThunderBadge = false;// 是否拥有橘色徽章
        public bool hasRainbowBadge = false;// 是否拥有彩虹徽章
        public bool hasMarshBadge = false;// 是否拥有粉红徽章
        public bool hasSoulBadge = false;// 是否拥有黄金徽章
        public bool hasVolcanoBadge = false;// 是否拥有深红徽章
        public bool hasEarthBadge = false;// 是否拥有绿色徽章

        public override void SetDefaults()
        {
            Item.width = 38; // 宽度
            Item.height = 28; // 高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Quest; // 稀有度
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.npcShop != 0)
            {
                return; // 如果在商店界面，则不绘制任何内容
            }
            if (!Main.mouseRight)//按下鼠标右键
            {
                if(Language.ActiveCulture.Name == "zh-Hans")
                    tooltips.Add(new TooltipLine(Mod, "", $"“训练师【{Main.LocalPlayer.name}】，你可以将收集到的道馆徽章\n放进卡片中”\n道馆徽章可以给予你不同效果的增益\n右键长按查看详细信息"));
                else
                    tooltips.Add(new TooltipLine(Mod, "", $"\"Trainer Gold Card\nYou can put the badges you collected in the card\nto give you various benefits\nRight-click to view detailed information\""));
            
                return; // 如果鼠标右键未按下，则不绘制任何内容
            }
            
            tooltips.Add(new TooltipLine(Mod, "", $"\n\n\n"));
            // 检查是否需要绘制 灰色徽章
            if (hasBoulderBadge)
            {
                tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? $"灰色徽章: 防御力提升至当前防御力的120%" : "Gray Badge: Defense increased to 120% of current defense"));
            }
            // 检查是否需要绘制 蓝色徽章
            if (hasCascadeBadge)
            {
                tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? $"蓝色徽章: 移动速度提升至当前移动速度的120%并提供额外加速度" : "Blue Badge: Movement speed increased to 120% of current movement speed and provide extra acceleration"));
            }
            // 检查是否需要绘制 橘色徽章
            if (hasThunderBadge)
            {
                tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? $"橘色徽章: 最大魔力值提升至120%,，所装备的宝可梦胸章越多，提升越多" : "Orange Badge: Max mana increased to 120%, the number of pokemon's chest badges increases the increase"));
            }
            // 检查是否需要绘制 彩虹徽章
            if (hasRainbowBadge)
            {
                tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? $"彩虹徽章: 生命恢复速度提升，所装备的宝可梦胸章越多，提升越多" : "Rainbow Badge: Life regeneration increased, the number of pokemon's chest badges increases the increase"));
            }
            // 检查是否需要绘制 粉红徽章
            if (hasMarshBadge)
            {
                tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? $"粉红徽章: 受到的伤害减少20%" : "Pink Badge: Damage reduced by 20%"));
            }
            // 检查是否需要绘制 黄金徽章
            if (hasSoulBadge)
            {
                tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? $"黄金徽章: 增加一定幸运值" : "Gold Badge: Increase luck value"));
            }
            // 检查是否需要绘制 深红徽章
            if (hasVolcanoBadge)
            {
                tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? $"深红徽章: 伤害提升20%，暴击率提升20%" : "Red Badge: Damage increased by 20%, critical strike chance increased by 20%"));
            }
            // 检查是否需要绘制 绿色徽章
            if (hasEarthBadge)
            {
                tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? $"绿色徽章: 最大基础生命值提升至当前最大基础生命值的120%" : "Green Badge: Increases max base health to 120% of current max base health"));
            }
        }
       
        //如果不在与其他npc对话中，则绘制图标
        public override void PostDrawTooltip(ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            if (Main.npcShop != 0)
            {
                return; // 如果在商店界面，则不绘制任何内容
            }
            if(!Main.mouseRight)//按下鼠标右键
            {
                return; // 如果鼠标右键未按下，则不绘制任何内容
            }

            // 绘制图标
            itemIcon ??= ModContent.Request<Texture2D>("Pokemon/Textures/TrainerGoldCardPanel").Value;
            int x = 0;
            int y = 0;
            int width = 441;
            int height = 93;
            Rectangle sourceRectangle = new Rectangle(x, y, width, height);
            Vector2 drawPosition = new Vector2(Main.mouseX + 25, Main.mouseY + 60);
            Main.spriteBatch.Draw(itemIcon, drawPosition, sourceRectangle, Color.White * 0.8f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);


            // 检查是否需要绘制 灰色徽章 图标
            if (hasBoulderBadge)
            {
                Texture2D boulderBadgeIcon = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/BoulderBadge").Value;
                int x2 = 0;
                int y2 = 0;
                int width2 = 28;
                int height2 = 28;
                Rectangle sourceRectangle2 = new Rectangle(x2, y2, width2, height2);
                Vector2 drawPosition2 = new Vector2(Main.mouseX + 42.5f, Main.mouseY + 110.5f);
                Main.spriteBatch.Draw(boulderBadgeIcon, drawPosition2, sourceRectangle2, Color.White * 0.8f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            // 检查是否需要绘制 蓝色徽章 图标
            if (hasCascadeBadge)
            {
                Texture2D cascadeBadgeIcon = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/CascadeBadge").Value;
                int x3 = 0;
                int y3 = 0;
                int width3 = 22;
                int height3 = 28;
                Rectangle sourceRectangle3 = new Rectangle(x3, y3, width3, height3);
                Vector2 drawPosition3 = new Vector2(Main.mouseX + 97, Main.mouseY + 112);
                Main.spriteBatch.Draw(cascadeBadgeIcon, drawPosition3, sourceRectangle3, Color.White * 0.8f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            // 检查是否需要绘制 橘色徽章 图标
            if (hasThunderBadge)
            {
                Texture2D thunderBadgeIcon = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/ThunderBadge").Value;
                int x4 = 0;
                int y4 = 0;
                int width4 = 32;
                int height4 = 32;
                Rectangle sourceRectangle4 = new Rectangle(x4, y4, width4, height4);
                Vector2 drawPosition4 = new Vector2(Main.mouseX + 144.2f, Main.mouseY + 109.5f);
                Main.spriteBatch.Draw(thunderBadgeIcon, drawPosition4, sourceRectangle4, Color.White * 0.8f, 0f, Vector2.Zero, 0.95f, SpriteEffects.None, 0f);
            }

            // 检查是否需要绘制 彩虹徽章 图标
            if (hasRainbowBadge)
            {
                Texture2D rainbowBadgeIcon = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/RainbowBadge").Value;
                int x5 = 0;
                int y5 = 0;
                int width5 = 32;
                int height5 = 32;
                Rectangle sourceRectangle5 = new Rectangle(x5, y5, width5, height5);
                Vector2 drawPosition5 = new Vector2(Main.mouseX + 198.8f, Main.mouseY + 108.8f);
                Main.spriteBatch.Draw(rainbowBadgeIcon, drawPosition5, sourceRectangle5, Color.White * 0.8f, 0f, Vector2.Zero, 0.98f, SpriteEffects.None, 0f);
            }

            // 检查是否需要绘制 粉红徽章 图标
            if (hasMarshBadge)
            {
                Texture2D marshBadgeIcon = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/MarshBadge").Value;
                int x6 = 0;
                int y6 = 0;
                int width6 = 28;
                int height6 = 28;
                Rectangle sourceRectangle6 = new Rectangle(x6, y6, width6, height6);
                Vector2 drawPosition6 = new Vector2(Main.mouseX + 254.5f, Main.mouseY + 109.5f);
                Main.spriteBatch.Draw(marshBadgeIcon, drawPosition6, sourceRectangle6, Color.White * 0.8f, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
            }

            // 检查是否需要绘制 黄金徽章 图标
            if (hasSoulBadge)
            {
                Texture2D soulBadgeIcon = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/SoulBadge").Value;
                int x7 = 0;
                int y7 = 0;
                int width7 = 28;
                int height7 = 28;
                Rectangle sourceRectangle7 = new Rectangle(x7, y7, width7, height7);
                Vector2 drawPosition7 = new Vector2(Main.mouseX + 308.5f, Main.mouseY + 109.5f);
                Main.spriteBatch.Draw(soulBadgeIcon, drawPosition7, sourceRectangle7, Color.White * 0.8f, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
            }

            // 检查是否需要绘制 深红徽章 图标
            if (hasVolcanoBadge)
            {
                Texture2D volcanoBadgeIcon = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/VolcanoBadge").Value;
                int x8 = 0;
                int y8 = 0;
                int width8 = 26;
                int height8 = 30;
                Rectangle sourceRectangle8 = new Rectangle(x8, y8, width8, height8);
                Vector2 drawPosition8 = new Vector2(Main.mouseX + 366f, Main.mouseY + 110.2f);
                Main.spriteBatch.Draw(volcanoBadgeIcon, drawPosition8, sourceRectangle8, Color.White * 0.8f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            // 检查是否需要绘制 绿色徽章 图标
            if (hasEarthBadge)
            {
                Texture2D earthBadgeIcon = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/EarthBadge").Value;
                int x9 = 0;
                int y9 = 0;
                int width9 = 32;
                int height9 = 32;
                Rectangle sourceRectangle9 = new Rectangle(x9, y9, width9, height9);
                Vector2 drawPosition9 = new Vector2(Main.mouseX + 419, Main.mouseY + 109);
                Main.spriteBatch.Draw(earthBadgeIcon, drawPosition9, sourceRectangle9, Color.White * 0.8f, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["BoulderBadgeUsed"] = hasBoulderBadge;
            tag["CascadeBadgeUsed"] = hasCascadeBadge;
            tag["ThunderBadgeUsed"] = hasThunderBadge;
            tag["RainbowBadgeUsed"] = hasRainbowBadge;
            tag["MarshBadgeUsed"] = hasMarshBadge;
            tag["SoulBadgeUsed"] = hasSoulBadge;
            tag["VolcanoBadgeUsed"] = hasVolcanoBadge;
            tag["EarthBadgeUsed"] = hasEarthBadge;
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            hasBoulderBadge = tag.Get<bool>("BoulderBadgeUsed");
            hasCascadeBadge = tag.Get<bool>("CascadeBadgeUsed");
            hasThunderBadge = tag.Get<bool>("ThunderBadgeUsed");
            hasRainbowBadge = tag.Get<bool>("RainbowBadgeUsed");
            hasMarshBadge = tag.Get<bool>("MarshBadgeUsed");
            hasSoulBadge = tag.Get<bool>("SoulBadgeUsed");
            hasVolcanoBadge = tag.Get<bool>("VolcanoBadgeUsed");
            hasEarthBadge = tag.Get<bool>("EarthBadgeUsed");
            base.LoadData(tag);
        }

        public override void UpdateInventory(Player player)
        {
            if (hasBoulderBadge)
            {
                player.statDefense *= 1.2f;// 获得灰色徽章，防御力提升20%
            }

            if(hasCascadeBadge)
            {
                player.moveSpeed *= 1.2f;// 获得蓝色徽章，移动力提升20%
                //加速度
                if(Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D) 
                        && player.velocity.X < 0f)//按下D键
                    player.velocity += 2f * Vector2.UnitX;

                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A) 
                        &&player.velocity.X > 0f)//按下D键
                    player.velocity -= 2f * Vector2.UnitX;
            }

            if(hasThunderBadge)
            {
                // 获得橘色徽章
                player.statManaMax2 += (int)(player.statManaMax2 * 0.2f);
                //// 神奇宝贝胸章
                ////遍历玩家装备栏
                for (int i = 0; i < player.armor.Length; i++)
                {
                    if (
                        player.armor[i].type == ModContent.ItemType<SunflowerBall>() &&
                        player.HasBuff(ModContent.BuffType<BuffsSunflowerBall>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if (player.armor[i].type == ModContent.ItemType<GastlyBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsGastlyBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if (player.armor[i].type == ModContent.ItemType<CharmanderBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsCharmanderBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if (player.armor[i].type == ModContent.ItemType<BulbasaurBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsBulbasaurBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if (player.armor[i].type == ModContent.ItemType<SquirtleBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsSquirtleBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if (player.armor[i].type == ModContent.ItemType<TaillowBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsTaillowBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if (player.armor[i].type == ModContent.ItemType<SpoinkBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsSpoinkBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if (player.armor[i].type == ModContent.ItemType<BeldumBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsBeldumBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if(player.armor[i].type == ModContent.ItemType<WingullBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsWingullBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if(player.armor[i].type == ModContent.ItemType<VoltorbBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsVoltorbBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if (player.armor[i].type == ModContent.ItemType<MunchlaxBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsMunchlaxBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if (player.armor[i].type == ModContent.ItemType<FomantisBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsFomantisBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if(player.armor[i].type == ModContent.ItemType<TrapinchBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsTaillowBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                    if(player.armor[i].type == ModContent.ItemType<PikachuBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsPikachuBadge>()))
                    {
                        player.statManaMax2 += (int)(player.statManaMax2 * 0.1f);
                    }
                }
            }

            if(hasRainbowBadge)
            {
                // 获得彩虹徽章，生命恢复速度提升20%
                //遍历玩家装备栏
                for (int i = 0; i < player.armor.Length; i++)
                {
                    if (player.armor[i].type == ModContent.ItemType<SunflowerBall>() &&
                        player.HasBuff(ModContent.BuffType<BuffsSunflowerBall>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if (player.armor[i].type == ModContent.ItemType<GastlyBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsGastlyBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if (player.armor[i].type == ModContent.ItemType<CharmanderBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsCharmanderBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if (player.armor[i].type == ModContent.ItemType<BulbasaurBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsBulbasaurBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if (player.armor[i].type == ModContent.ItemType<SquirtleBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsSquirtleBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if (player.armor[i].type == ModContent.ItemType<TaillowBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsTaillowBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if (player.armor[i].type == ModContent.ItemType<SpoinkBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsSpoinkBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if(player.armor[i].type == ModContent.ItemType<BeldumBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsBeldumBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if(player.armor[i].type == ModContent.ItemType<WingullBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsWingullBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if(player.armor[i].type == ModContent.ItemType<VoltorbBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsVoltorbBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if (player.armor[i].type == ModContent.ItemType<MunchlaxBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsMunchlaxBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if (player.armor[i].type == ModContent.ItemType<FomantisBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsFomantisBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if (player.armor[i].type == ModContent.ItemType<TrapinchBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsTaillowBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                    if (player.armor[i].type == ModContent.ItemType<PikachuBadge>() &&
                        player.HasBuff(ModContent.BuffType<BuffsPikachuBadge>()))
                    {
                        player.lifeRegen += 5;
                    }
                }
            }

            if(hasMarshBadge)
            {
                // 获得粉红徽章
                player.endurance += 0.2f;
            }

            if(hasSoulBadge)
            {
                // 获得黄金徽章，幸运值提升20%
                player.luckPotion = 20;
            }

            if(hasVolcanoBadge)
            {
                // 获得深红徽章，伤害提升20%
                player.GetDamage(DamageClass.Generic) += 0.2f;
                //暴击率提升20%
                player.GetCritChance(DamageClass.Generic) += 20f;
            }

            if(hasEarthBadge)
            {
                // 获得绿色徽章，最大基础生命值提升20%
                player.statLifeMax2 += (int)(player.statLifeMax2 * 0.2f);
            }
        }

        public override bool CanRightClick()
        {
            return false;
        }
    }
}