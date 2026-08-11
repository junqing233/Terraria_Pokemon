using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Buffs;
using Pokemon.Content.DamageClasses;
using Pokemon.Content.Equipment;
using Pokemon.Content.Items;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.GastlyBadgeProj
{
    public class GastlyBadgeProj4 : ModProjectile
    {
        private bool isshoot = true;
        private int timer = 0;
        private bool isfindtarget = false;//是否找到目标
        //private bool isEquipped = false;
        //private bool iszuizhong = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
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
                //找到GastlyBadgeProj3
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<GastlyBadgeProj1>())
                {
                    Projectile.Center = Main.projectile[i].Center + new Vector2(0, 0); // ; // 跟踪GastlyBadgeProj3
                }
                Projectile.velocity = Vector2.Zero;
            }
            Projectile.damage = 0; // 伤害为0
            Projectile.scale = 0.5f;
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
            if(Projectile.scale < 1f)
            {
                timer++;
                if (timer > 2)
                {
                    Projectile.scale += 0.05f;
                    timer = 0;
                }
            }
            // 更新帧动画
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10) // 每5帧切换下一帧
            {
                Projectile.frame++;
                Projectile.frame %= Main.projFrames[Projectile.type]; // 循环动画
                Projectile.frameCounter = 0;
            }

            Player player = Main.player[Projectile.owner];
            
            //遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                //找到GastlyBadgeProj3
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<GastlyBadgeProj1>())
                {
                    Vector2 mypos = Main.projectile[i].Center + new Vector2(0, -100);
                    float dis = Projectile.Distance(mypos); // 到玩家中心的距离
                  
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
                if(Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<GastlyBadgeProj5>() &&
                    (Main.projectile[i].position - Projectile.position).Length() < 1200f // 1200范围内找到GastlyBadgeProj5
                    )//1200范围内找到GastlyBadgeProj5
                {
                    isshoot = false;
                   // break;
                }
            }
            //PokeRadar pokeRadar = null;
            //for (int i = 0; i < player.inventory.Length; i++)
            //{
            //    if (player.inventory[i].ModItem is PokeRadar radar)
            //    {
            //        pokeRadar = radar;
            //        break;
            //    }
            //}

            //if (pokeRadar != null)
            //{
            //    for (int j = 0; j < PokeRadar.MaxItems; j++)
            //    {
            //        if (pokeRadar.items[j].ModItem is GastlyBadge)
            //        {
            //            isEquipped = true;
            //            break;
            //        }
            //    }
            //}
            
            if(!player.HasBuff(ModContent.BuffType<BuffsGastlyBadge>()))
            {
                Projectile.Kill();
            }
            if (isshoot)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                   Projectile.Center, (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f,
                   ModContent.ProjectileType<GastlyBadgeProj5>(), // 生成我们自己写的弹幕
                   Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.55f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                   player.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
            }
            NPC target = Main.npc[Projectile.owner];
            {
                int t = Projectile.FindTargetWithLineOfSight(1200); // 寻找1500像素范围内最近敌人号码（不隔墙）
                // 这个方法如果在没有敌怪时会返回-1，用来检测是否能找到敌人
                if (t >= 0)
                {
                    
                    isfindtarget = true;
                   
                }else
                {
                    Projectile.scale -= 0.02f;
                    if (Projectile.scale < 0.1f)
                    {
                        Projectile.Kill();
                    }
                    
                    isfindtarget = false;
                }
            }
           
            if(isfindtarget)
            {
                Projectile.timeLeft = 180; // 弹幕存在时间
            }
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            //粒子效果
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Clentaminator_Purple, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default(Color), 1.5f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].scale = 1.5f;
                Main.dust[dustIndex].color = Color.Purple;
            }
        }

        public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
           

            Rectangle rectangle = new Rectangle(
                0,
                texture.Height / Main.projFrames[Type] * Projectile.frame,
                texture.Width,
                texture.Height / Main.projFrames[Type]
            );

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                rectangle,
                lightColor,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                Projectile.scale * 1.8f,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}
