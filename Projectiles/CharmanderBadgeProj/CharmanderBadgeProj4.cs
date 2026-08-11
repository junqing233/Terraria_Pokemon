using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.CharmanderBadgeProj
{
    public class CharmanderBadgeProj4 : ModProjectile
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 40; // 弹幕宽度
            Projectile.height = 40; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 60; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 0.5f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 8; //独立无敌帧时间
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
           Projectile.rotation = Main.rand.NextFloat(0, 360); // 随机旋转角度
            //遍历敌人
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC npc = Main.npc[k];
                if (npc.active && !npc.friendly && npc.Distance(Projectile.Center) < 1200f && npc.damage > 0 && !npc.dontTakeDamage
                    && npc.lifeMax > 5)
                {
                    Projectile.Center = npc.Center; // 跟踪目标
                    break;
                }
            }
        }
      
        public override void AI()
        {
            //Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            // 更新帧动画
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4) // 每5帧切换下一帧
            {
                timer++;
                Projectile.frame++;
                Projectile.frame %= Main.projFrames[Projectile.type]; // 循环动画
                Projectile.frameCounter = 0;
                if(timer >= 6)
                {
                    Projectile.damage = 0;
                    Projectile.frame = 5;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Projectile.damage = 0;
        }
        

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            ////粒子效果
            //for (int i = 0; i < 10; i++)
            //{
            //    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
            //        DustID.Clentaminator_Purple, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default(Color), 1.5f);
            //    Main.dust[dustIndex].noGravity = true;
            //    Main.dust[dustIndex].scale = 1.5f;
            //    Main.dust[dustIndex].color = Color.Purple;
            //}
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
                Projectile.scale * 1.2f,
                SpriteEffects.None,
                0);
            return false;
        }
    }
}
