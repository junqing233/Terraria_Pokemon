using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Pokemon.Projectiles.ForcedExerciserProj;
using Pokemon.Content.Items;
using System;
using System.Linq;
using Terraria.Localization;

namespace Pokemon.Content.Equipment
{
    public class ForcedExerciser : ModItem
    {
        //强制锻炼器
        //加新徽章需要将其加上戴上才可训练的限制
        public static int Exercisetime = 0; // 练习时间
        private HashSet<int> trackedEnemies = new HashSet<int>(); // 用于记录跟踪的敌人，并在声明时初始化
        public static int ExerciseTime = 0; // 练习时间
        private bool isEquipped = false;
        private bool hasProjectile = false;
        private bool heldByPlayer = false;

        public override void SetDefaults()
        {
            Item.width = 42; // 宽度
            Item.height = 42; // 高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Green; // 稀有度
            Item.shoot = ModContent.ProjectileType<ForcedExerciserProj1>(); // 射击类型
            Item.shootSpeed = 1f; // 射击速度
        }

        public override void HoldItem(Player player)
        {
            //SunflowerBall sunflowerBall = null;
            //GastlyBadge gastlyBadge = null;
            //for (int i = 0; i < player.armor.Length; i++)
            //{
            //    if (player.armor[i].ModItem is SunflowerBall sunflower)
            //    {
            //        sunflowerBall = sunflower;
            //        break;
            //    }
            //    if (player.armor[i].ModItem is GastlyBadge gastly)
            //    {
            //        gastlyBadge = gastly;
            //        break;
            //    }
            //}
            //if (Main.mouseLeft && ExerciseTime < 100 && !player.mouseInterface && gastlyBadge!= null && sunflowerBall!= null)// 右键按下且动画未开始
            //{
            //    sunflowerBall.playerSunflowerRank = 0;
            //    gastlyBadge.playerGastlyRank = 0;
            //    //ExerciseTime++;
            //}
            //if (Main.mouseRight && ExerciseTime > 0 && !player.mouseInterface)//
            //{
            //    ExerciseTime--;
            //}

            //if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.V) && !player.mouseInterface)//按下鼠标中键
            //{
            //    //输出练习时间
            //    Main.NewText("显示进度：" + Exercisetime, 175, 75, 175);
            //    Main.NewText("已击败敌人：" + ExerciseTime, 175, 95, 175);
            //}
        }

        public override void UpdateInventory(Player player)
        {
            // 检查是否已经存在弹幕，防止同时发射多个同类型的弹幕
            // 遍历当前的投射物
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                // 检查当前投射物是否为 SacredSwordProj8 且是否仍然活跃
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<ForcedExerciserProj1>())
                {
                    hasProjectile = true;
                    break; // 找到后可以退出循环
                }else
                {
                    hasProjectile = false;
                }
            }
            if(player.HeldItem.type == ModContent.ItemType<ForcedExerciser>())
            {
                heldByPlayer = true;
            }
            // 根据条件创建弹幕
            if (!hasProjectile && heldByPlayer)
            {
                //Main.NewText("练习时间：" + Exercisetime, 175, 75, 175);
                // 获取玩家中心
                Vector2 playerCenter = player.position;

                // 创建新的位置
                Vector2 position = playerCenter; // 以玩家中心为发射点

                // 创建新的弹幕
                Projectile.NewProjectile(player.GetSource_FromThis(), position, Vector2.Zero, ModContent.ProjectileType<ForcedExerciserProj1>(), 0, 0f, player.whoAmI);
            }

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
            if(pokeRadar != null)
            {
                for (int i = 0; i < PokeRadar.MaxItems; i++)
                {
                    if (pokeRadar.items[i] != null && !pokeRadar.items[i].IsAir)
                        if (pokeRadar.items[i].ModItem is SunflowerBall ||
                        pokeRadar.items[i].ModItem is GastlyBadge ||
                        pokeRadar.items[i].ModItem is CharmanderBadge ||
                        pokeRadar.items[i].ModItem is BulbasaurBadge ||
                        pokeRadar.items[i].ModItem is SquirtleBadge ||
                        pokeRadar.items[i].ModItem is TaillowBadge ||
                        pokeRadar.items[i].ModItem is SpoinkBadge ||
                        pokeRadar.items[i].ModItem is BeldumBadge ||
                        pokeRadar.items[i].ModItem is WingullBadge ||
                        pokeRadar.items[i].ModItem is VoltorbBadge ||
                        pokeRadar.items[i].ModItem is MunchlaxBadge ||
                        pokeRadar.items[i].ModItem is FomantisBadge ||
                        pokeRadar.items[i].ModItem is TrapinchBadge ||
                        pokeRadar.items[i].ModItem is PikachuBadge) //加新徽章需要修改这里//14
                        {
                            isEquipped = true;
                            break;
                        }
                        else
                        {
                            isEquipped = false;
                        }
                }
            }
            if (isEquipped)
            {
                // 遍历所有活跃的敌人
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && Vector2.Distance(player.Center, npc.Center) <= 2000 && npc.lifeMax > 5
                        && !npc.friendly && ExerciseTime < 100)
                    {
                        // 如果敌人不在被跟踪的集合中，则添加到集合中
                        trackedEnemies.Add(npc.whoAmI);
                    }
                }
            }
            // 创建一个临时集合来存储需要移除的敌人ID
            HashSet<int> enemiesToRemove = new HashSet<int>();
            foreach (int enemyId in trackedEnemies)
            {
                if (enemyId >= 0 && enemyId < Main.npc.Length)
                {
                    NPC npc = Main.npc[enemyId];
                    // 如果敌人不再活跃（即被击败），则增加练习时间并计划将其从集合中移除
                    if (!npc.active)
                    {
                        ExerciseTime++;
                        // 检查玩家背包中是否有学习装置
                        if (player.inventory.Any(item => item.type == ModContent.ItemType<Expshare>()) && Expshare.ExtraExerciseEnabled)
                        {
                            // 遍历玩家的饰品栏，检查是否有指定的徽章
                            for (int i = 0; i < PokeRadar.MaxItems; i++)
                            {
                                if ((pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is SunflowerBall) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is GastlyBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is CharmanderBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is BulbasaurBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is SquirtleBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is TaillowBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is SpoinkBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is BeldumBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is WingullBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is VoltorbBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is MunchlaxBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is FomantisBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is TrapinchBadge) ||
                                    (pokeRadar.items[i] != null && pokeRadar.items[i].ModItem is PikachuBadge)) //加新徽章需要修改这里//14
                                {
                                    ExerciseTime++; // 每找到一个徽章，额外增加1
                                }
                            }
                        }
                        enemiesToRemove.Add(enemyId);
                    }
                }
                else
                {
                    // 如果 enemyId 不是一个有效的索引，则直接计划将其从集合中移除
                    enemiesToRemove.Add(enemyId);
                }
            }

            // 从被跟踪的敌人集合中移除已经被击败的敌人或无效的ID
            foreach (int enemyId in enemiesToRemove)
            {
                trackedEnemies.Remove(enemyId);
            }

            // 确保 ExerciseTime 不超过 100
            ExerciseTime = Math.Min(ExerciseTime, 100);

            // 更新 Exercisetime 基于 ExerciseTime 的值
            Exercisetime = ExerciseTime / 10;
        }
        
        public override bool CanRightClick()
        {
            Player player = Main.player[Main.myPlayer];
            // 检查练习时间是否达到10
            if (Exercisetime >= 10 && Main.mouseRight)
            {
                // 给玩家添加 GastlyBadge 项目
                //player.QuickSpawnItem(player.GetSource_FromThis(), ModContent.ItemType<MagicCandy>());
                // 创建新物品并放到鼠标上
                Main.mouseItem = new Item();
                Main.mouseItem.SetDefaults(ModContent.ItemType<MagicCandy>());

                // 重置练习时间
                Exercisetime = 0;
                ExerciseTime = 0;

                return false;
            }else
            {
                return false;
            }

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Language.ActiveCulture.Name == "zh-Hans")
            {
                if (Exercisetime == 0)
                {
                    tooltips.Add(new TooltipLine(Mod, "Exercisetime", $"训练进度: 0%"));
                }
                else
                {
                    // 显示 Exercisetime 的值
                    tooltips.Add(new TooltipLine(Mod, "Exercisetime", $"训练进度: {Exercisetime}0%"));
                }
                tooltips.Add(new TooltipLine(Mod, "ExerciseTime", $"已击败敌人: {ExerciseTime}"));

                if (ExerciseTime >= 100)
                {
                    tooltips.Add(new TooltipLine(Mod, "ForcedExerciser", $"右键点击获得神奇糖果"));
                }
                tooltips.Add(new TooltipLine(Mod, "", "【背包生效】"));
                if (!Main.mouseRight)//按下鼠标右键
                {
                    tooltips.Add(new TooltipLine(Mod, "", $"右键长按查看详细信息"));
                    return; // 如果鼠标右键未按下，则不绘制任何内容
                }
                tooltips.Add(new TooltipLine(Mod, "",
                    $"手持此物品打开进度条绘制开关，退出地图后重置开关\n" +
                    $"进度条不会主动消失，手持此物品按下B键即可隐藏进度条\n" +
                    $"手持此物品同时按下鼠标左键和右键可以调整进度条位置\n" +
                    $"当击败100个敌人后，可以右键点击此物品获得神奇糖果\n" +
                    $"提示:玩家至少装备了一个宝可梦徽章才可以进行训练计数"));
            }
            else
            {
                if (Exercisetime == 0)
                {
                    tooltips.Add(new TooltipLine(Mod, "Exercisetime", $"Training progress: 0%"));
                }
                else
                {
                    // 显示 Exercisetime 的值
                    tooltips.Add(new TooltipLine(Mod, "Exercisetime", $"Training progress: {Exercisetime}0%"));
                }
                tooltips.Add(new TooltipLine(Mod, "ExerciseTime", $"Enemies defeated: {ExerciseTime}"));

                if (ExerciseTime >= 100)
                {
                    tooltips.Add(new TooltipLine(Mod, "ForcedExerciser", $"Right-click to receive a magical candy"));
                }
                tooltips.Add(new TooltipLine(Mod, "", "【Inventory Effects】"));
                if (!Main.mouseRight)//按下鼠标右键
                {
                    tooltips.Add(new TooltipLine(Mod, "", $"Right-click to view detailed information"));
                    return; // 如果鼠标右键未按下，则不绘制任何内容
                }
                tooltips.Add(new TooltipLine(Mod, "",
                    $"Holding this item opens the progress bar and resets the switch when you exit the map\n" +
                    $"The progress bar will not disappear automatically, and you can hide it by pressing B while holding this item\n" +
                    $"Holding this item and pressing the left and right mouse buttons can adjust the position of the progress bar\n" +
                    $"When defeating 100 enemies, you can right-click this item to receive a magical candy\n" +
                    $"Note: The player must have equipped at least one Pokémon badge to count for training"));
            }
                
        }

    }
}
