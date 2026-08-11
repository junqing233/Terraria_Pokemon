using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Localization;


namespace Pokemon.Content.Props
{
    public class ModTicket : ModItem
    {
        //模组奖券
        public override void SetDefaults()
        {
            Item.width = 34; // 宽度
            Item.height = 42; // 高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Green; // 稀有度
            //设置为消耗品
            Item.consumable = true; // 消耗品
            Item.maxStack = 999; // 最大堆叠数量

            // 设置为货币，可以放在钱币栏
            ItemID.Sets.CommonCoin[Item.type] = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // 添加提示信息
            //if (Language.ActiveCulture.Name == "zh-Hans")
                tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? "用于抽取宝可蛋中的模组物品" : "Used to extract mod items from the PokeEgg"));
            //else
            //    tooltips.Add(new TooltipLine(Mod, "", "Used to extract mod items from the PokeEgg"));
        }
        
        public override bool CanRightClick()
        {
            return false;
        }
    }
}
