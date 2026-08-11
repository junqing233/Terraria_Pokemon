using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.CharmanderBadgeProj
{
    public class CharmanderBadgeProj5 : ModProjectile
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
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
            Projectile.timeLeft = 360; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 0.75f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 8; //独立无敌帧时间
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
           Projectile.rotation = Main.rand.NextFloat(0, 360); // 随机旋转角度

            NPC nearestEnemy = null;
            float minDistanceToEnemy = float.MaxValue;

            // 遍历敌人，找到距离 CharmanderBadgeProj1 弹幕最近的敌人
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC npc = Main.npc[k];
                if (npc.active && !npc.friendly && npc.damage > 0 && !npc.dontTakeDamage && npc.lifeMax > 5)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (proj.active && proj.type == ModContent.ProjectileType<CharmanderBadgeProj1>() && proj.owner == Projectile.owner)
                        {
                            float distanceToCharmanderBadgeProj1 = npc.Distance(proj.Center);
                            if (distanceToCharmanderBadgeProj1 < minDistanceToEnemy)
                            {
                                minDistanceToEnemy = distanceToCharmanderBadgeProj1;
                                nearestEnemy = npc;
                            }
                            break; // 假设每个 CharmanderBadgeProj1 只会有一个最近的敌人
                        }
                    }
                }
            }

            // 如果找到了最近的敌人，则将本弹幕的中心设置为该敌人的中心
            if (nearestEnemy != null)
            {
                Projectile.Center = nearestEnemy.Center;
            }

        }

        public override void AI()
        {
            //Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            // 更新帧动画
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2) // 每5帧切换下一帧
            {
                timer++;
                Projectile.frame++;
                Projectile.frame %= Main.projFrames[Projectile.type]; // 循环动画
                Projectile.frameCounter = 0;
                if(timer >= 5)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
           // Projectile.damage = Projectile.damage * 2;
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
                Projectile.scale * 0.5f,
                SpriteEffects.None,
                0);
            return false;
        }
    }
}
