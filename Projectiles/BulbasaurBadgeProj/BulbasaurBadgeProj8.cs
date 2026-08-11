using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Pokemon.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.BulbasaurBadgeProj
{
    public class BulbasaurBadgeProj8 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 10; // 弹幕宽度
            Projectile.height = 10; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 360; // 存在时间，单位为帧
            Projectile.alpha = 10; // 透明度
            Projectile.light = 0.5f; // 发光亮度
            base.SetDefaults();
        }
        public override void OnSpawn(IEntitySource source)
        {
            // 将伤害设置为0，防止伤害
            Projectile.damage = 0;
        }

        public override void AI()
        {
            //弹幕向玩家处移动
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.Center - Projectile.Center;
            vector.Normalize();
            //弹幕速度
            Projectile.velocity = vector * 4f + Projectile.velocity * 0.7f;
            //加速度
            Projectile.velocity.X += Main.rand.Next(-10, 11) * 0.05f;
            Projectile.velocity.Y += Main.rand.Next(-10, 11) * 0.05f;

            float distance = Vector2.Distance(Projectile.Center, player.Center);

            //当距离玩家较近
            if (distance < 30f)
            {
                if (Projectile.originalDamage / 4 < 1)
                {
                    //恢复玩家血量
                    player.statLife += 1; // 恢复生命值
                    player.HealEffect(1); // 显示恢复生命的效果
                }
                else
                {
                    //恢复玩家血量
                    player.statLife += Projectile.originalDamage / 4; // 恢复生命值
                    player.HealEffect(Projectile.originalDamage / 4); // 显示恢复生命的效果
                }
                int dust = Dust.NewDust(player.Center + new Vector2(-player.width / 2, 0), player.width, player.height,
                    ModContent.DustType<SunflowerDust>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].velocity = new Vector2(0, 0);
                Main.dust[dust].scale = 0.1f; // 设置大小
                Main.dust[dust].velocity *= 0.1f; // 设置速度
                Projectile.Kill(); // 消失
            }
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, Projectile.width + Main.rand.Next(-40, 40),
                    Projectile.height + Main.rand.Next(-40, 40), DustID.GreenTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
                Main.dust[dust].color = Color.White; // 设置颜色
                Main.dust[dust].noGravity = true; // 让灰尘不受重力影响
                Main.dust[dust].scale = 1f; // 设置大小
                Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.05f; // 设置渐入时间
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
                    Projectile.scale,
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