using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Audio;
using Pokemon.Content.Equipment;
using Terraria.Localization;
using Pokemon.Content.Ball;
using Pokemon.Content.Clothes;


namespace Pokemon.Content.Props
{
    public class PokemonPackage : ModItem
    {
        // 宝可梦包裹
        public override void SetDefaults()
        {
            Item.width = 40; // 宽度
            Item.height = 38; // 高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Green; // 稀有度
            Item.consumable = true; // 消耗品
            Item.maxStack = 1; // 最大堆叠数量
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // 添加提示信息
            //if (Language.ActiveCulture.Name == "zh-Hans")
                tooltips.Add(new TooltipLine(Mod, "", Language.ActiveCulture.Name == "zh-Hans" ? "右键点击打开包裹" : "Right-click to open the package"));
            //else
            //    tooltips.Add(new TooltipLine(Mod, "", "Right-click to open the package"));
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            // 在玩家背包中生成指定物品
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<PokeRadar>());//宝可梦战斗仪
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<PokeonEggMachineItem>());//宝可蛋
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<TrainerGoldCard>());//训练师金卡
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<Pokedex>());//宝可梦图鉴
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<BerryPouch>());//树果袋
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<PokeBall>(),10);//精灵球
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<AshKetchumHead>());//小智帽
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<AshKetchumBody>());//小智衣
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<AshKetchumLegs>());//小智鞋
        }
    }
}
