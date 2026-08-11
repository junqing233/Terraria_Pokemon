using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.GastlyBadgeProj
{
    public class GastlyBadgeProj7 : ModProjectile
    {
       
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 30; // 弹幕宽度
            Projectile.height = 30; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = 1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 360; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 0.5f; // 发光亮度
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
           
        }
      
        public override void AI()
        {
            if(Projectile.scale < 1.6f)
            Projectile.scale += 0.05f; // 弹幕扩散
            Projectile.rotation = Projectile.velocity.ToRotation();
            // 更新帧动画
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10) // 每5帧切换下一帧
            {
                Projectile.frame++;
                //Projectile.frame %= Main.projFrames[Projectile.type]; // 循环动画
                Projectile.frameCounter = 0;
                if(Projectile.frame >= 3)
                {
                    Projectile.frame = 3;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(!target.HasBuff(ModContent.BuffType<Buffs.BuffsGastlyBadgeProj7>()))
            {
                target.AddBuff(ModContent.BuffType<Buffs.BuffsGastlyBadgeProj7>(), 120); // 给目标添加buff
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
            ProjectileID.Sets.TrailingMode[Type] = 2; // 设置尾迹模式为2，即尾迹为圆形
            ProjectileID.Sets.TrailCacheLength[Type] = 6; // 设置尾迹缓存长度为6，即最多保留6个尾迹

            Rectangle rectangle = new Rectangle(
                0,
                texture.Height / Main.projFrames[Type] * Projectile.frame,
                texture.Width,
                texture.Height / Main.projFrames[Type]
            );

            Color MyColor = Color.White * 1f;
            MyColor.A = 0;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                Main.EntitySpriteDraw(texture, oldcenter, rectangle, MyColor * factor,
                    Projectile.oldRot[i],
                    new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                    Projectile.scale * 1.8f,
                    SpriteEffects.None, 0);
            }
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
            Main.EntitySpriteDraw(
                texture,
                /*Projectile.Center*/ - Main.screenPosition + Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2),
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
