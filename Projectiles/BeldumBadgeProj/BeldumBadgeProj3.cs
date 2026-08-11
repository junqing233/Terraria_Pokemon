using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Buffs;
using Pokemon.Content.DamageClasses;
using Pokemon.Content.Equipment;
using Pokemon.Content.Items;
using Pokemon.Projectiles.GastlyBadgeProj;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.BeldumBadgeProj
{
    public class BeldumBadgeProj3 : ModProjectile
    {
        private int AttackTimer = 0;
        private bool isshoot = true;
        private int timer = 0;
        private bool isfindtarget = false;//是否找到目标
        private bool isEquipped = false;
        private bool iszuizhong = false;
        NPC targetNPC = null;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 30; // 弹幕宽度
            Projectile.height = 30; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 360; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 0.5f; // 发光亮度
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                //找到BeldumBadgeProj1
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BeldumBadgeProj1>())
                {
                    Projectile.Center = Main.projectile[i].Center;
                    break; // 找到目标后立即退出循环
                }
            }
            Projectile.velocity = Vector2.Zero;
            Projectile.damage = 0; // 伤害为0
        }

        void MoveToTarget(Vector2 targetPos, float MaxSpeed = 20f, float accSpeed = 0.5f)//运用之前学到的惯性追击
        {
            //原理：比较目标和自己的横向或者纵向坐标差，然后给自己的速度加上向着差值变小前进的加速度
            //如果自己的速度坐标差一样，说明自己正在原理目标，需要更大的加速度，这里我设定的是2倍
            if (Projectile.Center.X - targetPos.X < 0f)
                Projectile.velocity.X += Projectile.velocity.X < 0 ? 2 * accSpeed : accSpeed;
            else
                Projectile.velocity.X -= Projectile.velocity.X > 0 ? 2 * accSpeed : accSpeed;

            if (Projectile.Center.Y - targetPos.Y < 0f)
                Projectile.velocity.Y += Projectile.velocity.Y < 0 ? 2 * accSpeed : accSpeed;
            else
                Projectile.velocity.Y -= Projectile.velocity.Y > 0 ? 2 * accSpeed : accSpeed;
            if (Math.Abs(Projectile.velocity.X) > MaxSpeed)//如果横向速度超越最大值，则回到最大值
                Projectile.velocity.X = MaxSpeed * Math.Sign(Projectile.velocity.X);
            if (Math.Abs(Projectile.velocity.Y) > MaxSpeed)//如果纵向速度超越最大值，则回到最大值
                Projectile.velocity.Y = MaxSpeed * Math.Sign(Projectile.velocity.Y);

        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // 遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Vector2 myposOffset = new Vector2(0, -50);
                Rectangle myposRectangle;
                // 定义矩形的宽度和高度
                float width = 40;
                float height = 50;
                //找到BeldumBadgeProj1
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BeldumBadgeProj1>())
                {
                    // 计算矩形的左上角和右下角
                    Vector2 topLeft = player.Center + myposOffset - new Vector2(width / 2, height / 2);

                    // 创建矩形区域
                    myposRectangle = new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)width, (int)height);

                    // 将 Vector2 转换为 Point
                    Point projectilePoint = new Point((int)Projectile.Center.X, (int)Projectile.Center.Y);
                    
                    Vector2 mypos = Main.projectile[i].Center + new Vector2(0, -50);
                   
                    float dis = Projectile.Distance(mypos); // 到玩家中心的距离
                    if(!myposRectangle.Contains(projectilePoint))
                    {
                        if (dis > 1200) // 距离玩家过远时加速回归
                        {
                            Projectile.Kill();
                        }
                        else if (dis > 620) // 中程时，作惯性追击运动
                        {
                            MoveToTarget(mypos, 10, 0.3f); // 对着目标做追击运动
                        }
                        else if (dis > 20)
                        {
                            MoveToTarget(mypos, 8, 0.32f); // 对着目标做追击运动
                        }
                    }
                    
                }
            }

            if (!player.HasBuff(ModContent.BuffType<BuffsBeldumBadge>()))
            {
                Projectile.Kill();
            }

            NPC target = BeldumBadgeProj1.FindTargetWithinRange(player, 1200f); // 寻找1500像素范围内最近敌人号码（不隔墙）
            targetNPC = target;
            if (target != null)
            {
                //朝向敌人
                if (Projectile.Center.X - target.Center.X > 0f)
                    Projectile.rotation = (Projectile.Center - target.Center).ToRotation();
                else
                    Projectile.rotation = (Projectile.Center - target.Center).ToRotation() + MathHelper.Pi;
                AttackTimer++;
                Projectile.timeLeft = 180; // 弹幕存在时间
                if (AttackTimer >= 60)
                {
                    AttackTimer = 0;
                    //生成新的弹幕
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                        Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16f,
                        ModContent.ProjectileType<BeldumBadgeProj4>(), // 生成我们自己写的弹幕
                        Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.2f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                        target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
                }
                isfindtarget = true;
            }
            else
            {
                Projectile.rotation = MathHelper.Pi*2;
                timer++;
                if (timer > 240)
                {
                    Projectile.Kill();
                }
                isfindtarget = false;
            }

            if (isfindtarget)
            {
                timer = 0;
                Projectile.timeLeft = 180; // 弹幕存在时间
            }
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            //粒子效果
            for (int i = 0; i < 2; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.BlueTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default(Color), 1.5f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].scale = 1.5f;
            }
        }

        public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            // 默认使用第一帧
            int frameY = 0;

            // 如果有目标敌人
            if (targetNPC != null && targetNPC.active)
            {
                // 根据弹幕相对于敌人的位置选择帧数
                if (Projectile.Center.X < targetNPC.Center.X)
                {
                    frameY = 0; // 第一帧
                }
                else
                {
                    frameY = 1; // 第二帧
                }
            }
            else
            {
                // 根据弹幕的速度选择帧数
                if (Projectile.velocity.X > 0)
                {
                    frameY = 0; // 第一帧
                }
                else
                {
                    frameY = 1; // 第二帧
                }
            }

            Rectangle rectangle = new Rectangle(
                0,
                texture.Height / Main.projFrames[Type] * frameY,
                texture.Width,
                texture.Height / Main.projFrames[Type]
            );

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                rectangle,
                lightColor,
                Projectile.rotation * Projectile.spriteDirection,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                Projectile.scale * 1f,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}
