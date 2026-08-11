using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Content.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class ButterfreeWings : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.wingTimeMax = 25; //飞行时间
            player.rocketBoots = 2;//火箭靴效果
            player.jumpSpeedBoost += 2f;//增强跳跃
            player.GetModPlayer<ButterfreeWingsPlayer>().ButterfreeWingsEquipped = true;//启用
            // 移除粒子特效相关代码
        }
        public override void UpdateVanity(Player player)
        {
            // 移除时装栏粒子相关代码
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            // 飞行参数
            ascentWhenFalling = 0.5f;                   //下落时的上升速度
            ascentWhenRising = 0.1f;                   //上升时的额外推力
            maxCanAscendMultiplier = 0.5f;            //最大上升乘数
            maxAscentMultiplier = 1.5f;              //最大上升速度
            constantAscend = 0.1f;                  //恒定上升速度
        }
        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 3f;//最大水平速度
            acceleration = 1f;//水平加速度
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CreativeWings, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
    public class ButterfreeWingsPlayer : ModPlayer
    {
        //翅膀动画参数
        public int WingFrame { get; set; }
        public int WingFrameCounter { get; set; }
        private const int AnimationSpeed = 8; //动画速度（值越大越慢）
        private const int FrameCount = 4; //总帧数
        // 移除 hasEffect 相关内容
        public bool ButterfreeWingsEquipped { get; set; }//翅膀是否装备

        public override void ResetEffects()
        {
            //重置翅膀装备状态
            ButterfreeWingsEquipped = false;
        }

        public override void UpdateDead()
        {
            //玩家死亡时重置动画
            WingFrame = 0;
            WingFrameCounter = 0;
        }

        public override void PostUpdate()
        {
            //更新翅膀动画
            UpdateWingAnimation();
        }
        // 移除 PostUpdateEquips 粒子相关方法

        private void UpdateWingAnimation()
        {
            if (!ButterfreeWingsEquipped)
            {
                WingFrame = 0;
                WingFrameCounter = 0;
                return;
            }

            //在地面时保持第一帧
            if (Player.velocity.Y == 0)
            {
                WingFrame = 0;//保持第一帧但不重置计数器
                return;
            }

            //只在空中更新动画
            if (++WingFrameCounter >= AnimationSpeed)
            {
                WingFrameCounter = 0;
                WingFrame = (WingFrame + 1) % FrameCount;
            }
        }
    }
}