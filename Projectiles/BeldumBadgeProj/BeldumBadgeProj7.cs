using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Buffs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.BeldumBadgeProj
{
    public class BeldumBadgeProj7 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.light = 0.1f;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.light = 0.2f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.damage = 0;
            //Projectile.scale = 0.2f;
            player.AddBuff(ModContent.BuffType<BuffsBeldumBadgeProj7>(), 360);
        }

        private int UpdateCount = 0;
        private bool isIncreasing = true; // 新增方向标记
        public override void AI()
        {
            //if(Projectile.scale < 0.8f)
            //{
            //    Projectile.scale += 0.02f;
            //}
            Player player = Main.LocalPlayer;
           
            Projectile.Center = player.Center;

            if (isIncreasing)
            {
                UpdateCount += 10;
                if (UpdateCount >= 250)
                {
                    isIncreasing = false; // 到达上限切换方向
                }
            }
            else
            {
                UpdateCount -= 10;
                if (UpdateCount <= 0)
                {
                    isIncreasing = true; // 到达下限切换方向
                }
            }
            player.immune = true;// 玩家无敌
            player.immuneTime = 2; // 设置无敌时间为2帧，确保每帧都重新设置无敌状态

            Projectile.alpha = UpdateCount;
            Projectile.scale = 0.6f + (float)UpdateCount / 400f;
        }

        [Obsolete]
        public override void Kill(int timeLeft)
        {
            //粒子效果
            for (int i = 0; i < 4; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Smoke, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].scale = 1.2f;
                Main.dust[dustIndex].color = Color.White;
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
               lightColor*Projectile.Opacity,
               Projectile.rotation,
               new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
               Projectile.scale * 0.4f,
               SpriteEffects.None,
               0);
            return false;
        }
    }
}
