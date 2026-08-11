using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.Localization;
using Pokemon.Content.Equipment;
using Pokemon.Content.Items;

namespace Pokemon.Content.Accessories
{
    //吃剩的东西
    public class Leftovers : ModItem
    {
        private bool isEquippedMunchlax = false; // 是否装备了小卡比兽
        public override void SetDefaults()
        {
            Item.width = 26; // 饰品宽度
            Item.height = 40; // 饰品高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Green; // 稀有度
            Item.accessory = true; // 设为装备
            Item.defense = 2; // 防御力加成
        }
        // 合成材料
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient(ItemID.Apple, 1) // 苹果
               .AddIngredient(ItemID.Worm, 1) // 蠕虫
               .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Language.ActiveCulture.Name == "zh-Hans")
                tooltips.Add(new TooltipLine(Mod, "", isEquippedMunchlax? "“装备该道具后，每10秒恢复玩家最大生命值的1/8”" : "“装备该道具后，每10秒恢复玩家最大生命值的1/16”"));
            else
                tooltips.Add(new TooltipLine(Mod, "", isEquippedMunchlax? "“Equip this item to restore your maximum life by 1/8 every 10 seconds”" : "“Equip this item to restore your maximum life by 1/16 every 10 seconds”"));
        }
        public override void UpdateInventory(Player player)
        {
            // 获取 PokeRadar 实例
            PokeRadar pokeRadar = null;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is PokeRadar radar)
                {
                    pokeRadar = radar;
                    break;
                }
            }
            isEquippedMunchlax = false;
            if (pokeRadar != null)
            {
                for (int i = 0; i < PokeRadar.MaxItems; i++)
                {
                    if (pokeRadar.items[i] != null && !pokeRadar.items[i].IsAir)
                        if (pokeRadar.items[i].ModItem is MunchlaxBadge)
                        {
                            isEquippedMunchlax = true;
                            break;
                        }
                }
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // 获取 PokeRadar 实例
            PokeRadar pokeRadar = null;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is PokeRadar radar)
                {
                    pokeRadar = radar;
                    break;
                }
            }
            isEquippedMunchlax = false;
            if (pokeRadar != null)
            {
                for (int i = 0; i < PokeRadar.MaxItems; i++)
                {
                    if (pokeRadar.items[i] != null && !pokeRadar.items[i].IsAir)
                        if (pokeRadar.items[i].ModItem is MunchlaxBadge)
                        {
                            isEquippedMunchlax = true;
                            break;
                        }
                }
            }
            LeftoversPlayer modPlayer = player.GetModPlayer<LeftoversPlayer>();
            modPlayer.hasLeftoversEquipped = true; // 标记为已装备
            modPlayer.healTimer++; // 增加计时器

            if (modPlayer.healTimer >= 600) // 每10秒（600帧）
            {
                int healAmount = isEquippedMunchlax ? player.statLifeMax2 / 8 : player.statLifeMax2 / 16; // 恢复最大生命值的16%
                player.statLife += healAmount; // 恢复生命值
                player.HealEffect(healAmount); // 显示恢复生命的效果
                modPlayer.healTimer = 0; // 重置计时器
            }
        }
    }
}

namespace Pokemon.Content.Accessories
{
    public class LeftoversPlayer : ModPlayer
    {
        public int healTimer = 0; // 计时器
        public bool hasLeftoversEquipped = false; // 是否装备了吃剩的东西

        public override void ResetEffects()
        {
            hasLeftoversEquipped = false; // 重置装备状态
        }

        public override void UpdateEquips()
        {
            if (!hasLeftoversEquipped)
            {
                healTimer = 0; // 重置计时器
            }
        }
    }
}