using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.BulbasaurBadgeProj
{
    public class BulbasaurBadgeProj10 : ModProjectile
    {
        private int timer = 0; // 计时器

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 6; // 弹幕宽度
            Projectile.height = 6; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 360; // 存在时间，单位为帧
            Projectile.alpha = 10; // 透明度
            Projectile.light = 0.5f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 10; //独立无敌帧时间
            base.SetDefaults();
        }
        public override void OnSpawn(IEntitySource source)
        {
            if(Projectile.damage > 1)
            // 将伤害设置为原有伤害的1.2倍
                Projectile.damage = (int)(Projectile.damage * 0.5f);
            else
                Projectile.damage = 1; // 伤害最小为1
            Projectile.frame = Main.rand.Next(3); // 随机选择帧

            Vector2 direction = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
            direction.Normalize(); // 归一化方向

            Projectile.velocity = direction * Main.rand.Next(2,8) * 0.5f; // 设置速度
        }

        public override void AI()
        {
            Projectile.velocity *= 0.96f; // 减速
            timer++;
            if(timer > 60)
            {
                timer = 0;
                for (int i = 0; i < 1; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, Projectile.width / 4,
                   Projectile.height / 4, DustID.PurpleTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
                    Main.dust[dust].color = Color.White; // 设置颜色
                    Main.dust[dust].noGravity = true; // 让灰尘不受重力影响
                    Main.dust[dust].scale = 1f; // 设置大小
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.05f; // 设置渐入时间
                }
            }
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 1; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, Projectile.width,
                    Projectile.height, DustID.PurpleTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
                Main.dust[dust].color = Color.White; // 设置颜色
                Main.dust[dust].noGravity = true; // 让灰尘不受重力影响
                Main.dust[dust].scale = 1f; // 设置大小
                Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.05f; // 设置渐入时间
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCHitCooldown += 5; // 冷却时间延长
            if (Projectile.damage > 1)
                Projectile.damage = (int)(Projectile.damage * 0.5f); // 伤害变为原有伤害的1.2倍
            else
                Projectile.damage = 1; // 伤害最小为1

            if(!target.HasBuff(BuffID.Poisoned))
            {
                target.AddBuff(BuffID.Poisoned, 120); // 给目标添加中毒buff
            }
        }

        public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            ProjectileID.Sets.TrailingMode[Type] = 2; // 设置尾迹模式为2，即尾迹为圆形
            ProjectileID.Sets.TrailCacheLength[Type] = 4; // 设置尾迹缓存长度为6，即最多保留6个尾迹

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
                    Projectile.scale * 0.3f,
                    SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(
               texture,
               Projectile.Center - Main.screenPosition,
               rectangle,
               lightColor,
               Projectile.rotation,
               new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
               Projectile.scale * 0.3f,
               SpriteEffects.None,
               0);
            return false;
        }

    }
}