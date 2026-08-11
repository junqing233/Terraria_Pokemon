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
    public class BulbasaurBadgeProj4 : ModProjectile
    {
        // 物品的纹理文件名与物品名不同，因此此属性指向纹理文件。
        public override string Texture => "Pokemon/Projectiles/BulbasaurBadgeProj/BulbasaurBadgeProj4";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 20; // 弹幕宽度
            Projectile.height = 20; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = true; // 不与瓷砖碰撞
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
            // 将伤害设置为原有伤害的1.2倍
            Projectile.damage = (int)(Projectile.damage * 1f);
        }

        public override void AI()
        {
            Projectile.damage = 0; // 伤害为0
            // 更新帧动画
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10) // 每5帧切换下一帧
            {
                Projectile.frame++;
                Projectile.frame %= Main.projFrames[Projectile.type]; // 循环动画
                Projectile.frameCounter = 0;
            }

            if(Projectile.velocity.Y > 10)
            {
                Projectile.tileCollide = true; // 不与瓷砖碰撞
            }else
            {
                Projectile.tileCollide = false; // 与瓷砖碰撞
            }
            
            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height,
                DustID.UnusedBrown, Projectile.velocity.X * 0.01f, Projectile.velocity.Y * 0.01f);
            Main.dust[dust].color = Microsoft.Xna.Framework.Color.White; // 设置颜色
            Main.dust[dust].noGravity = true; // 取消重力
            Main.dust[dust].velocity *= 0.2f; // 减少速度

            // 更新垂直速度以模拟重力
            Projectile.velocity.Y += 0.3f;
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            //创建新弹幕
            Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                Projectile.Center, Projectile.velocity * 1f,
                ModContent.ProjectileType<BulbasaurBadgeProj6>(),
                Projectile.originalDamage, Projectile.knockBack, Projectile.owner);

            for (int i = 0; i < 4; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, Projectile.width + Main.rand.Next(-40, 40),
                    Projectile.height + Main.rand.Next(-40, 40), DustID.UnusedBrown, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
                Main.dust[dust].color = Color.White; // 设置颜色
                Main.dust[dust].noGravity = true; // 让灰尘不受重力影响
                Main.dust[dust].scale = 1f; // 设置大小
                Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.05f; // 设置渐入时间
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //创建新弹幕
            Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                Projectile.Center, Projectile.velocity * 1f,
                ModContent.ProjectileType<BulbasaurBadgeProj6>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner);
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
               Projectile.scale,
               SpriteEffects.None,
               0);
            return false;
        }

    }
}