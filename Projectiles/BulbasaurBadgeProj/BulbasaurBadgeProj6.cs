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
    public class BulbasaurBadgeProj6 : ModProjectile
    {
        private NPC target; // 用于保存目标NPC
        private int attacktimer = 0; // 计时器，用于控制弹幕的持续时间


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 20; // 弹幕宽度
            Projectile.height = 20; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 480; // 存在时间，单位为帧
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
            Projectile.damage = 0; // 弹幕伤害为0
            Projectile.velocity = Vector2.Zero; // 弹幕的速度为0

            // 更新帧动画
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 30) // 每5帧切换下一帧
            {
                Projectile.frameCounter = 0;
                if(Projectile.timeLeft > 60)
                {
                    if (Projectile.frame == 2)
                    {
                        Projectile.frame = 2;// 防止死循环
                    }
                    else
                    {
                        Projectile.frame++;
                    }
                }else
                {
                    Projectile.timeLeft = 60;
                    if(Projectile.frame == 0)
                    {
                        Projectile.Kill();
                    }else
                    {
                        Projectile.frame--;
                    }
                }
                
            }
            // 如果目标是空的或者失活的，那么重新寻找敌人
            if (target == null || !target.active)
            {
                int t = Projectile.FindTargetWithLineOfSight(1500); // 寻找1500像素范围内最近敌人号码（不隔墙）
                if (t >= 0)
                {
                    target = Main.npc[t]; // 定义这个NPC为目标
                }
            }
            // 如果目标不为空且存活在此处执行攻击性AI
            if (target != null && target.active && target.damage > 0)
            {
                attacktimer++;
                if(attacktimer == 120)
                {
                    attacktimer = 0;
                    //创建新弹幕
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                        Projectile.Center, Projectile.velocity * 1f,
                        ModContent.ProjectileType<BulbasaurBadgeProj7>(),
                        Projectile.originalDamage, Projectile.knockBack, Projectile.owner);

                }
            }
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
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

