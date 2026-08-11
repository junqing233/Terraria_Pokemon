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

namespace Pokemon.Projectiles.FomantisBadgeProj
{
    public class FomantisBadgeProj2 : ModProjectile
    {
        private bool VelocityChange = false;
        private bool Targeting = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
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
            Projectile.timeLeft = 120; // 存在时间，单位为帧
            Projectile.alpha = 10; // 透明度
            Projectile.light = 0.75f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 10; //独立无敌帧时间
            base.SetDefaults();
        }
        public override void OnSpawn(IEntitySource source)
        {
            //// 将伤害设置为原有伤害的1.2倍
            Projectile.damage = (int)(Projectile.damage * 1f);
            //Projectile.frame = Main.rand.Next(4);// 随机选择帧
            Projectile.rotation = Projectile.velocity.X * 0.01f; // 随机旋转
        }
        
        public override void AI()
        {
            // 更新帧动画
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10) // 每5帧切换下一帧
            {
                Projectile.frame++;
                Projectile.frame %= Main.projFrames[Projectile.type]; // 循环动画
                Projectile.frameCounter = 0;
            }
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
            
            if (!VelocityChange)
            {
                Vector2 direction = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                direction.Normalize(); // 归一化方向
                Projectile.velocity = direction * 1f; // 设置速度
                VelocityChange = true; // 已经改变了速度
                Projectile.damage = 0; // 伤害为0，防止伤害
            }
           
            if(Projectile.timeLeft <= 60)
            {
                //攻击目标
                if (target!= null && target.active && !Targeting)
                {
                    Projectile.damage = Projectile.originalDamage; // 回到原有伤害
                    Projectile.velocity = Vector2.Normalize(target.Center - Projectile.Center) * 26f; // 跟踪目标
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    //if (Projectile.frame == 0 || Projectile.frame == 2)
                    //    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 / 2; // 设置初始旋转角度
                    //else
                    //    Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2 / 2; // 设置初始旋转角度
                }
            }
            else
            {
                Projectile.rotation += 0.1f * Projectile.direction; // 随机旋转
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if(Main.rand.Next(5) < 3)
            modifiers.SetCrit(); // 设置爆击
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Targeting = true; // 停止跟踪
            for (int i = 0; i < 4; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, Projectile.width + Main.rand.Next(-40, 40),
                    Projectile.height + Main.rand.Next(-40, 40), DustID.GreenTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
                Main.dust[dust].color = Color.White; // 设置颜色
                Main.dust[dust].noGravity = true; // 让灰尘不受重力影响
                Main.dust[dust].scale = 1f; // 设置大小
                Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.05f; // 设置渐入时间
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
                Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.05f; // 设置渐入时间
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            ProjectileID.Sets.TrailingMode[Type] = 2;//设置尾迹模式为2，即尾迹为圆形
            ProjectileID.Sets.TrailCacheLength[Type] = 6;//设置尾迹缓存长度为5，即最多保留5个尾迹
            Rectangle rectangle = new Rectangle(
               0,
               texture.Height / Main.projFrames[Type] * Projectile.frame,
               texture.Width,
               texture.Height / Main.projFrames[Type]
           );
            Color MyColor = Color.White;
            MyColor.A = 0;
            if(Projectile.velocity.Length() > 2f)
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                Main.EntitySpriteDraw(texture, oldcenter, rectangle, MyColor * factor,
                    Projectile.oldRot[i],
                    new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                    new Vector2(0.8f),
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
