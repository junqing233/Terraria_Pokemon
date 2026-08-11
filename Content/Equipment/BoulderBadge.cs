using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Pokemon.Projectiles.TrainerGoldCardProj;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Pokemon.Content.Equipment
{
    public class BoulderBadge : ModItem
    {
        //灰色徽章
        public override void SetDefaults()
        {
            Item.width = 28; // 宽度
            Item.height = 28; // 高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Green; // 稀有度
            //设置为消耗品
            Item.consumable = true;// 消耗品
            Item.maxStack = 1; // 最大堆叠数量
            Item.useTime = 25; // 使用时间
            Item.useAnimation = 25; // 使用动画
            Item.useStyle = ItemUseStyleID.HoldUp; // 使用方式
            Item.UseSound = SoundID.Item4;// 使用音效
            Item.shoot = ModContent.ProjectileType<BoulderBadgeProj1>(); // 射击类型
        }

        public override bool CanUseItem(Player player)
        {
            bool isuse = false;

            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is TrainerGoldCard trainerGold)
                {
                    isuse = true;
                    break;
                }
            }
            if(isuse)
            {
                return true;

            }else
            {
                if (Language.ActiveCulture.Name == "zh-Hans")
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                        new Color(250, 250, 250), "背包中没有训练师金卡，使用失败！"); // 显示文本提示
                }
                else
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                        new Color(250, 250, 250), "There is no Trainer Gold Card in your bag, use failed!"); // 显示文本提示
                }
                return false;
            }
            
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //tooltips.Add(new TooltipLine(Mod, "", $"\n\n"));
        }

        
        public override bool CanRightClick()
        {
           
            return false;
        }
       
    }
}
