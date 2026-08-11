using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.GastlyBadgeProj
{
    public class GastlyBadgeProj2 : ModProjectile
    {
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
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
            Projectile.alpha = 10; // 透明度
            Projectile.light = 0.5f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 8; //独立无敌帧时间
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            GastlyBadgeProj3.Gasji = 0;
            //遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                //找到GastlyBadgeProj3
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<GastlyBadgeProj1>())
                {
                    Projectile.Center = Main.projectile[i].Center + new Vector2(0, -120);
                }
            }
            Projectile.scale = 0.1f;
            Projectile.velocity = Vector2.Zero;
        }

        public override void AI()
        {
            Projectile.rotation -= 0.1f; // 随机旋转
            
            if (GastlyBadgeProj3.Gasji >= 6)
            {
                timer++;
                if (timer > 5 && Projectile.scale < 1.2f)
                {
                    Projectile.scale += 0.1f;
                    timer = 0;
                }
                NPC target = null;
                if (target == null || !target.active) // 如果目标是空的或者失活的，那么重新寻找敌人
                {
                    int t = Projectile.FindTargetWithLineOfSight(1500); // 寻找1500像素范围内最近敌人号码（不隔墙）
                    // 这个方法如果在没有敌怪时会返回-1，用来检测是否能找到敌人
                    if (t >= 0)
                    {
                        target = Main.npc[t]; // 定义这个NPC为目标
                    }
                }
                if (target != null && target.active && target.friendly == false && Projectile.scale > 1f)
                {
                    Vector2 directionToTarget = target.Center - Projectile.Center;
                    directionToTarget.Normalize(); // 归一化方向
                                                   // 计算目标NPC的头部位置
                    Vector2 targetHeadPosition = target.Center;
                    // 计算弹幕到目标头部的向量
                    Vector2 direction = targetHeadPosition - Projectile.Center;
                    //计算距离
                    float distance = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
                    //Projectile.velocity = Vector2.Normalize(target.Center - Projectile.Center) * 26f; // 跟踪目标
                    if (distance >= 100)
                        // 设置弹幕新的速度和穿透能力
                        Projectile.velocity = directionToTarget * 26f; // 设置向目标的速度
                    else if(distance <= 20)
                    {
                        // 生成一个随机角度
                        float randomAngle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                        // 计算新的位置，以目标NPC中心为圆心，150像素为半径
                        Vector2 newPosition = target.Center + new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle)) * 400f;
                        // 计算新的方向向量
                        Vector2 newDirection = newPosition - Projectile.Center;
                        // 归一化方向向量
                        newDirection.Normalize();
                        // 设置新的速度
                        Projectile.velocity = newDirection * 26f;
                    }
                    Projectile.rotation -= Projectile.velocity.X * 0.001f; // 随机旋转
                }
            }
            else 
            {
                Projectile.velocity *= 0.9f; // 减速
                Projectile.rotation -= 0.05f; // 随机旋转
            }
            if(Projectile.velocity.Length() > 10f)
            Projectile.velocity -= Projectile.velocity * 0.12f; // 减速
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            //粒子效果
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.PurpleMoss, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default(Color), 1.5f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].scale = 2f;
                Main.dust[dustIndex].color = Color.DarkGreen;
            }
           
            //获得玩家位置
            Player player = Main.player[Projectile.owner];
            for (int i = 0; i < Main.rand.Next(4, 6); i++)
            {
                //生成新的弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                    Projectile.Center, (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f,
                    ModContent.ProjectileType<GastlyBadgeProj3>(), // 生成我们自己写的弹幕
                    Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.55f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                    player.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
            }
            // 弹幕消失时的行为
        }

        public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            ProjectileID.Sets.TrailingMode[Type] = 2; // 设置尾迹模式为2，即尾迹为圆形
            ProjectileID.Sets.TrailCacheLength[Type] = 6; // 设置尾迹缓存长度为6，即最多保留6个尾迹

            Rectangle rectangle = new Rectangle(
                0,
                texture.Height / Main.projFrames[Type] * Projectile.frame,
                texture.Width,
                texture.Height / Main.projFrames[Type]
            );

            Color MyColor = Color.White * 0.8f;
            MyColor.A = 0;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                Main.EntitySpriteDraw(texture, oldcenter, rectangle, MyColor * factor,
                    Projectile.oldRot[i],
                    new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                    Projectile.scale * 1.01f,
                    SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                rectangle,
                lightColor,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}
