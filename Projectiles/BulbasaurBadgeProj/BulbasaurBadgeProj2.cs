using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Pokemon.Projectiles.BulbasaurBadgeProj
{
    public class BulbasaurBadgeProj2 : ModProjectile
    {
        private bool VelocityChange = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 10; // 弹幕宽度
            Projectile.height = 10; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = 6; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 60; // 存在时间，单位为帧
            Projectile.alpha = 10; // 透明度
            Projectile.light = 0.75f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 10; //独立无敌帧时间
            base.SetDefaults();
        }
        public override void OnSpawn(IEntitySource source)
        {
            // 将伤害设置为原有伤害的1.2倍
            Projectile.damage = (int)(Projectile.damage * 1f);
            Projectile.frame = Main.rand.Next(4);// 随机选择帧
            Projectile.rotation = Projectile.velocity.X * 0.01f; // 随机旋转
        }
        
        public override void AI()
        {
            if (!VelocityChange)
            {
                Vector2 direction = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                direction.Normalize(); // 归一化方向
                Projectile.velocity = direction * 1f; // 设置速度
                VelocityChange = true; // 已经改变了速度
                Projectile.damage = 0; // 伤害为0，防止伤害
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 4; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, Projectile.width + Main.rand.Next(-40, 40),
                    Projectile.height + Main.rand.Next(-40, 40), DustID.GreenTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
                Main.dust[dust].color = Color.White; // 设置颜色
                Main.dust[dust].noGravity = true; // 让灰尘不受重力影响
                Main.dust[dust].scale = 1f; // 设置大小
                Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.05f; // 设置渐入时间
            }
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            NPC target = null; // 先设出目标NPC，默认为空
            if (target == null || !target.active) // 如果目标是空的或者失活的，那么重新寻找敌人
            {
                int t = Projectile.FindTargetWithLineOfSight(1500); // 寻找1500像素范围内最近敌人号码（不隔墙）
                // 这个方法如果在没有敌怪时会返回-1，用来检测是否能找到敌人
                if (t >= 0)
                {
                    target = Main.npc[t]; // 定义这个NPC为目标
                }
            }

            //攻击目标
            if (target != null && target.active)
            {
                //创建新弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                    Projectile.Center, Projectile.velocity * 1f,
                    ModContent.ProjectileType<BulbasaurBadgeProj3>(),
                    Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            }
            for (int i = 0; i < 2; i++)
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
               Projectile.scale / 2,
               SpriteEffects.None,
               0);
            return false;
        }
    }
}
