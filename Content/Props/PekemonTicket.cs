using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Audio;
using Pokemon.Buffs;
using Terraria.Localization;


namespace Pokemon.Content.Props
{
    public class PekemonTicket : ModItem
    {
        //宝可梦奖券
        public override void SetDefaults()
        {
            Item.width = 42; // 宽度
            Item.height = 34; // 高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Green; // 稀有度
            Item.consumable = true; // 消耗品
            Item.maxStack = 999; // 最大堆叠数量

            // 设置为货币，可以放在钱币栏
            ItemID.Sets.CommonCoin[Item.type] = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // 添加提示信息
            //if (Language.ActiveCulture.Name == "zh-Hans")
                tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? "用于抽取宝可蛋中的宝可梦" : "Used to extract Pokémon from the PokeEgg"));
            //else
            //    tooltips.Add(new TooltipLine(Mod, "", "Used to extract Pokémon from the PokeEgg"));
        }

        public override bool CanRightClick()
        {
            return false;
        }

        public override void RightClick(Player player)
        {
            // 右键点击时的行为
        }
    }
}
