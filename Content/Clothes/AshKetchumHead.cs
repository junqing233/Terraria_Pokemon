using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Content.Clothes
{
    [AutoloadEquip(EquipType.Head)]
    public class AshKetchumHead : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;  
            Item.height = 14; 
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