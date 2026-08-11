using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Content.Clothes
{
    [AutoloadEquip(EquipType.Legs)]
    public class AshKetchumLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;  
            Item.height = 12; 
            Item.value = Item.sellPrice(0, 1, 0, 0); 
            Item.rare = ItemRarityID.Pink;
            Item.vanity = true;  
        }
        // 是否允许触发套装效果
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return false; // 无套装
        }

        //public override void AddRecipes()
        //{
        //    CreateRecipe()
        //        .AddIngredient(ItemID.Wood, 15)
        //        .AddIngredient(ItemID.RedAcidDye, 1)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();
        //}
    }
}