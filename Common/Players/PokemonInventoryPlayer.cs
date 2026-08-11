using Pokemon.Content.Equipment;
using Pokemon.Content.Items;
using Pokemon.Content.Props;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Common.Players
{
	public class PokemonInventoryPlayer : ModPlayer
	{
		// AddStartingItems 是一个你可以用来向玩家起始背包中添加物品的方法。
		// 它在玩家死亡中度死亡时也会被调用。
		// 返回一个可枚举的物品集合，这些物品将被添加到玩家的背包中。
		// 这个方法会在玩家的背包中添加一个 ExampleItem 和 256 颗金矿石。
		//
		// 如果你知道 'yield return' 的用法，你也可以在这里使用它。
		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath) 
		{
			//if (mediumCoreDeath) // 如果处于旅途模式，我们会给玩家一个治疗药水。
			//{
			//	return new[]
			//	{
			//		new Item(ItemID.HealingPotion)
			//	};
			//}

			return new[]
			{
				new Item(ModContent.ItemType<PokemonPackage>()),
				//new Item(ItemID.GoldOre, 256),
				//new Item(ModContent.ItemType<ExampleBlock>(), 256),
				//new Item(ModContent.ItemType<ExampleWall>(), 256),
				//new Item(ModContent.ItemType<ExampleOre>(), 256),
				//new Item(ModContent.ItemType<ExampleChair>(), 99),
				//new Item(ModContent.ItemType<ExampleTable>(), 99),
				//new Item(ModContent.ItemType<ExampleChest>(), 99),
				//new Item(ModContent.ItemType<ExamplePlatform>(), 256)
			};
		}

		// ModifyStartingInventory 是一个更复杂的版本的 AddStartingItems，允许你移除由原版或其他模组添加的物品。
		// 你也可以在这里添加物品，但建议只在 AddStartingItems 中进行添加。
		// 在这个示例中，如果处于旅途模式，我们会阻止 Terraria 向玩家的背包中添加铁斧。
		// （如果你想阻止另一个模组添加的物品，其条目是该模组的内部名称，例如 itemsByMod["SomeMod"]）
		// Terraria 的条目总是命名为 "Terraria"
		public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath) 
		{
			//itemsByMod["Terraria"].RemoveAll(item => item.type == ItemID.IronAxe);
		}

		public override void OnEnterWorld()
		{
			// 如果玩家背包中有PokeRadar
			PokeRadar pokeRadar = null;
			for (int i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++)
			{
				if (Main.player[Main.myPlayer].inventory[i].ModItem is PokeRadar radar)
				{
					pokeRadar = radar;
					break;
				}
			}
			if(pokeRadar == null)
			{
                if (ModContent.GetInstance<PokeRadarSystem>().IsUIVisible())
                {
                    //如果UI为关闭，就默认为关闭
                    //打开，关闭
                    //for (int j = 0; j < 2; j++)
                        ModContent.GetInstance<PokeRadarSystem>().ToggleUI(pokeRadar); // 打开宝可梦雷达UI
                }
                //else
                //{
                //    //如果UI为打开，就默认为打开
                //    //关闭，打开
                //    for (int j = 0; j < 2; j++)
                //        ModContent.GetInstance<PokeRadarSystem>().ToggleUI(pokeRadar); // 打开宝可梦雷达UI
                //}
            }
			if (pokeRadar != null)
			{
				if (pokeRadar.items.Any(item => item != null && !item.IsAir))
				{
                    //如果有物品，就默认为打开
                    var pokeRadarSystem = ModContent.GetInstance<PokeRadarSystem>();
					if (!ModContent.GetInstance<PokeRadarSystem>().IsUIVisible())
					{
                        // 如果UI没有打开，打开UI
                        //打开，关闭，打开
                        for (int j = 0; j < 3; j++)
                        ModContent.GetInstance<PokeRadarSystem>().ToggleUI(pokeRadar); // 打开宝可梦雷达UI
                    }else
					{
                        // 如果UI打开，刷新打开UI
                        //关闭，打开
                        for (int j = 0; j < 2; j++)
                        ModContent.GetInstance<PokeRadarSystem>().ToggleUI(pokeRadar); // 打开宝可梦雷达UI
                    }
				}
				else
				{
                    //如果没有物品，就默认为退出时的状态
					var pokeRadarSystem = ModContent.GetInstance<PokeRadarSystem>();
                    if (!ModContent.GetInstance<PokeRadarSystem>().IsUIVisible())
                    {
						//如果UI为关闭，就默认为关闭
                        //打开，关闭
                        for (int j = 0; j < 2; j++)
                            ModContent.GetInstance<PokeRadarSystem>().ToggleUI(pokeRadar); // 打开宝可梦雷达UI
                    }
                    else
                    {
                        //如果UI为打开，就默认为打开
                        //关闭，打开
                        for (int j = 0; j < 2; j++)
                            ModContent.GetInstance<PokeRadarSystem>().ToggleUI(pokeRadar); // 打开宝可梦雷达UI
                    }
                }
			}
		}


	}
}
