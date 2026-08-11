using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.Equipment;

namespace Pokemon.Projectiles.ForcedExerciserProj
{
    public class ForcedExerciserProj1 : ModProjectile
    {
        private bool isFirst = false; // 刚出现
        //private bool isFirst1 = false; // 刚出现
        //private bool isFirst2 = false; // 刚出现
        private int direction = 0; // 弹幕方向
        public override string Texture => "Pokemon/Projectiles/BlankProj/BlankProj1";
        public override void SetDefaults()
        {
            Projectile.knockBack = 0f; // 击退
            Projectile.width = 14; // 弹幕宽度
            Projectile.height = 92; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = DamageClass.Default; // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 60; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 0f; // 发光亮度
        }

        private Vector2 offset; // 用于记录弹幕与玩家的相对位置
        private bool isB = false; // 用于记录是否按下 B 键
        private bool ismove = false; // 用于记录是否移动了弹幕
        private bool isshow = false; // 用于记录是否显示了弹幕

        public override bool? CanCutTiles()
        {
            return false;//我们不想召唤兽会割草
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // 维持弹幕的存在时间
            if (player.HeldItem.type == ModContent.ItemType<ForcedExerciser>() 
                || player.HasItemInAnyInventory(ModContent.ItemType<ForcedExerciser>()))
            {
                Projectile.timeLeft = 30;
                if(!ismove)
                {
                    //if(player.direction == 1 && !isFirst)
                    //{
                    //    Projectile.Center = player.Center + new Vector2(-player.width * 2, player.height / 2); // 保持弹幕与玩家的相对位置
                    //    isFirst = true; // 记录刚出现
                    //}else
                    //{
                    //    isFirst2 = false; // 记录刚出现
                    //}
                    //if(player.direction == -1 && !isFirst2)
                    //{
                    //    Projectile.Center = player.Center + new Vector2(player.width * 2, player.height / 2); // 保持弹幕与玩家的相对位置
                    //    isFirst = true; // 记录刚出现
                    //}else
                    //{
                    //    isFirst = false; // 记录刚出现
                    //}

                    if (player.direction == 1 && !isFirst)
                    {
                        direction = 1; // 记录弹幕方向
                        isFirst = true; // 记录刚出现
                    }
                    else if(player.direction == -1 && !isFirst)
                    {
                        direction = -1; // 记录弹幕方向
                        isFirst = true; // 记录刚出现
                    }
                    if (direction == 1)
                    {
                        Projectile.Center = player.Center + new Vector2(-player.width * 2, player.height / 2); // 保持弹幕与玩家的相对位置
                    }
                    else if(direction == -1)
                    {
                        Projectile.Center = player.Center + new Vector2(player.width * 2, player.height / 2); // 保持弹幕与玩家的相对位置
                    }
                    else
                    {
                        Projectile.Center = player.Center + new Vector2(0, -player.height); // 保持弹幕与玩家的相对位置
                    }
                }
            }
            //获取鼠标位置
            Vector2 mousePosition = Main.MouseWorld; // 获取鼠标位置
            bool fastisB = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.B);
            if (player.HeldItem.type == ModContent.ItemType<ForcedExerciser>())
            {
                
                if (fastisB && !isB)
                {
                    if (!isshow)
                    {
                        isshow = true; // 记录 B 键按下
                    }
                    else
                    {
                        isshow = false; // 记录 B 键松开
                    }
                }
                isB = fastisB; // 记录 B 键松开

                // 当按下 B 键时，调整弹幕位置到鼠标位置
                if (Main.mouseLeft && Main.mouseRight
                    //Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.B)
                    )
                {
                    //Projectile.velocity = 
                    //Vector2 mousePosition = Main.MouseWorld; // 获取鼠标位置
                    Projectile.position = mousePosition;  // 将弹幕位置设置为鼠标位置
                    offset = Projectile.position - player.position; // 更新相对偏移
                    ismove = true; // 记录 B 键按下
                }
            }
           
            if(ismove)
            {
                // 将弹幕保持在与玩家之间的相对位置
                Projectile.position = player.position + offset;
            }

            // 设置伤害为 0，确保不造成伤害
            Projectile.damage = 0;
        }

        public override bool PreDraw(ref Color lightColor) // PreDraw 方法
        {
            Player player = Main.LocalPlayer; // 获取本地玩家信息

            // 检查玩家当前手持的物品是否是 SacredSword
            if ((player.HeldItem.type == ModContent.ItemType<ForcedExerciser>() || 
                player.HasItemInAnyInventory(ModContent.ItemType<ForcedExerciser>())) && !isshow
                )
            {
                // 获取根据当前能量绘制不同的纹理
                string texturePath = $"Pokemon/Projectiles/ForcedExerciserProj/ForcedExerciserProj1_{ForcedExerciser.Exercisetime}"; // 构建纹理路径
                
                // 计算颜色
                Color drawColor = Color.White; // 默认白色

                // 缩放比例
                float scale = 1f; // 默认缩放为1

                // 获取对应的纹理并绘制
                if (ForcedExerciser.Exercisetime >= 0 && ForcedExerciser.Exercisetime<= 10) // 确保能量值在合理范围
                {
                    Texture2D energyTexture = ModContent.Request<Texture2D>(texturePath).Value; // 请求对应的纹理

                    // 绘制纹理
                    Main.spriteBatch.Draw(energyTexture, Projectile.position - Main.screenPosition, null, drawColor, 0f, 
                        new Vector2(energyTexture.Width / 2, energyTexture.Height / 2), scale, SpriteEffects.None, 0f);
                }
            }

            return false; // 返回 false 禁用默认的绘制
        }

    }
}
