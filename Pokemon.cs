using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Common.Systems;
using Pokemon.Content.Accessories;
using Pokemon.Content.Equipment;
using Pokemon.Content.Furnitures.Dolls;
using Pokemon.Content.NPCs.Bosses.Beedrill_Mega;
using Pokemon.Content.Props;
using Pokemon.Content.Tools;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Pokemon
{
    //public class Pokemon : Mod
    //{
    //    //*****************跳跳猪精神冲击/\
    //    //public static Effect DefaultEffect;
    //    //public static Texture2D MainColor;
    //    //public static Texture2D MainShape;
    //    //public static Texture2D MaskColor;

    //    //public override void PostSetupContent()
    //    //{
    //    //    DefaultEffect = ModContent.Request<Effect>("Pokemon/Effects/Trail").Value;
    //    //    MainColor = ModContent.Request<Texture2D>("Pokemon/Textures/heatmap").Value;
    //    //    MainShape = ModContent.Request<Texture2D>("Pokemon/Textures/Extra_197").Value;
    //    //    MaskColor = ModContent.Request<Texture2D>("Pokemon/Textures/Extra_189").Value;
    //    //    base.PostSetupContent();
    //    //}
    //    //******************跳跳猪精神冲击\/
    //}
    public class ChestLootSystem : ModSystem
    {
        public override void PostWorldGen()
        {
            // 遍历所有宝箱
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null) continue; // 如果宝箱为空，跳过

                // 遍历宝箱中的物品槽
                for (int itemIndex = 0; itemIndex < chest.item.Length; itemIndex++)
                {
                    // 如果当前物品槽为空
                    if (chest.item[itemIndex].type == ItemID.None)
                    {
                        // 2% 概率将物品添加到宝箱中
                        if (Main.rand.NextFloat() < 0.04f)
                            chest.item[itemIndex].SetDefaults(ModContent.ItemType<ModTicket>());
                        if (Main.rand.NextFloat() < 0.08f)
                            chest.item[itemIndex].SetDefaults(ModContent.ItemType<PekemonTicket>());
                        if (Main.rand.NextFloat() < 0.1f)
                            chest.item[itemIndex].SetDefaults(ModContent.ItemType<PropTicket>());
                        if (Main.rand.NextFloat() < 0.1f)
                            chest.item[itemIndex].SetDefaults(ModContent.ItemType<Nugget>());
                        if (Main.rand.NextFloat() < 0.1f)
                            chest.item[itemIndex].SetDefaults(ModContent.ItemType<WrapTightlyHookItem>());
                        break; // 跳出物品槽循环
                    }
                }
            }

            // 定义需要放入箱子的物品列表
            List<int> dollItems = new List<int>
            {
                ModContent.ItemType<AzurillDollItem>(),
                ModContent.ItemType<BaltoyDollItem>(),
                ModContent.ItemType<ChikoritaDollItem>(),
                ModContent.ItemType<ClefairyDollItem>(),
                ModContent.ItemType<CyndaquilDollItem>(),
                ModContent.ItemType<DittoDollItem>(),
                ModContent.ItemType<DuskullDollItem>(),
                ModContent.ItemType<GulpinDollItem>(),
                ModContent.ItemType<JigglypuffDollItem>(),
                ModContent.ItemType<KecleonDollItem>(),
                ModContent.ItemType<LittleSawCrocodileDollItem>(),
                ModContent.ItemType<LotadDollItem>(),
                ModContent.ItemType<MeowthDollItem>(),
                ModContent.ItemType<MudkipDollItem>(),
                ModContent.ItemType<PichuDollItem>(),
                ModContent.ItemType<PikachuDollItem>(),
                ModContent.ItemType<SeedotDollItem>(),
                ModContent.ItemType<SkittyDollItem>(),
                ModContent.ItemType<SmoochumDollItem>(),
                ModContent.ItemType<SwabluDollItem>(),
                ModContent.ItemType<TogepiDollItem>(),
                ModContent.ItemType<TorchicDollItem>(),
                ModContent.ItemType<TreeckoDollItem>(),
                ModContent.ItemType<WobbuffetDollItem>()
            };

            // 记录已经使用的箱子索引
            HashSet<int> usedChests = new HashSet<int>();

            // 遍历物品列表
            foreach (int itemType in dollItems)
            {
                // 随机选择一个未被使用的箱子
                int attempts = 0;
                while (attempts < 1000) // 防止死循环
                {
                    int chestIndex = Main.rand.Next(Main.maxChests);
                    Chest chest = Main.chest[chestIndex];

                    if (chest != null && !usedChests.Contains(chestIndex)) // 确保箱子未被使用
                    {
                        // 找到第一个空的物品槽
                        for (int itemIndex = 0; itemIndex < chest.item.Length; itemIndex++)
                        {
                            if (chest.item[itemIndex].type == ItemID.None) // 如果物品槽为空
                            {
                                chest.item[itemIndex].SetDefaults(itemType); // 放入物品
                                usedChests.Add(chestIndex); // 标记箱子为已使用
                                break;
                            }
                        }
                        break; // 成功放入物品后跳出循环
                    }
                    attempts++;
                }
            }
        }
        
    }

}
