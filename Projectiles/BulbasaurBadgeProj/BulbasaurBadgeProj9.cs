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
    public class BulbasaurBadgeProj9 : ModProjectile
    {
        private NPC target; // 目标NPC

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 20; // 弹幕宽度
            Projectile.height = 20; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = 1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 360; // 存在时间，单位为帧
            Projectile.alpha = 10; // 透明度
            Projectile.light = 0.5f; // 发光亮度
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            // 将伤害设置为原有伤害的1倍
            Projectile.damage = (int)(Projectile.damage * 1f);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            // 如果目标是空的或者失活的，那么重新寻找敌人
            if (target == null || !target.active)
            {
                target = BulbasaurBadgeProj1.FindTargetWithinRange(player, 1200f);
            }
            // 如果目标不为空且存活在此处执行攻击性AI
            if (target != null && target.active && target.CanBeChasedBy() && target.lifeMax > 5)
            {
                //追踪目标并攻击
                Vector2 targetCenter = target.Center;
                Vector2 vectorToTarget = targetCenter - Projectile.Center;
                vectorToTarget.Normalize();

                // 向目标方向移动
                Projectile.velocity = vectorToTarget * 12f + Projectile.velocity * 0.2f; // 速度为10

            }
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {

            for (int i = 0; i < 4; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, Projectile.width + Main.rand.Next(-40, 40),
                    Projectile.height + Main.rand.Next(-40, 40), DustID.PurpleTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
                Main.dust[dust].color = Color.White; // 设置颜色
                Main.dust[dust].noGravity = true; // 让灰尘不受重力影响
                Main.dust[dust].scale = 1f; // 设置大小
                Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.05f; // 设置渐入时间
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 32; i++)
            {
                //创建新弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                    Projectile.Center, Projectile.velocity * 1f,
                    ModContent.ProjectileType<BulbasaurBadgeProj10>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner);
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