using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Localization;


namespace Pokemon.Content.Props
{
    public class Nugget : ModItem
    {
        //金珠
        public override void SetDefaults()
        {
            Item.width = 28; // 宽度
            Item.height = 28; // 高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Green; // 稀有度
            //设置为消耗品
            Item.consumable = true;// 消耗品
            Item.maxStack = 1; // 最大堆叠数量
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? "右键点击转换为铂金币" : "Right-click to convert to Platinum Coin"));
        }
        
        public override bool CanRightClick()
        {
            Player player = Main.LocalPlayer;
            if(Main.mouseRight)
            {
                // 获取玩家的背包中的物品
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    if (player.inventory[i].type == ModContent.ItemType<Nugget>())
                    {
                        // 将物品更改为 MyBoss2_3Potion
                        player.inventory[i].SetDefaults(ItemID.PlatinumCoin);
                        player.inventory[i].stack = 6; // 设置物品数量
                        SoundEngine.PlaySound(SoundID.Coins, player.Center);
                        break; // 找到并处理后退出循环
                    }
                }
            }
            return true;
        }
    }
}
