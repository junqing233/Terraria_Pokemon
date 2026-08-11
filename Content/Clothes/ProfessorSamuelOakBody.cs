using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Content.Clothes
{
    [AutoloadEquip(EquipType.Body)]
    public class ProfessorSamuelOakBody : ModItem
    {
        public override void SetDefaults()
        {
            // 物品基础属性
            Item.width = 22;      // 贴图宽度 
            Item.height = 24;     // 贴图高度
            Item.value = Item.sellPrice(0, 1, 0, 0); // 价值5金币
            Item.rare = ItemRarityID.Pink; // 稀有度
            //Item.defense = 100;    // 防御力

            // 重要：标记为头盔装备
            Item.vanity = true;  //时装装备
        }
        // 是否允许触发套装效果
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return false; // 无套装
        }

        // 制作配方
        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient(ItemID.Wood, 15)
            //    .AddIngredient(ItemID.RedAcidDye, 1)
            //    .AddTile(TileID.MythrilAnvil)
            //    .Register();
        }
    }
}