using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.MunchlaxProj
{
    public class MunchlaxProj5 : ModProjectile
    {
        private bool hasLanded = false; // 是否已经落地
        private NPC targetNPC; // 目标敌人
        private bool isHitTarget = false; // 是否击中目标
        private List<NPC> hitNPCs = new List<NPC>(); // 存储被击中的所有 NPC


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 240; // 弹幕宽度
            Projectile.height = 240; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 240; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 0.75f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = -1; //独立无敌帧时间
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            targetNPC = FindClosestNPC(1200);
        }

        public override void AI()
        {
            if (!hasLanded)
            {
                // 模拟下落
                Projectile.velocity.Y += 0.5f; // 重力加速度
                if (Projectile.velocity.Y > 16f) // 最大下落速度
                {
                    Projectile.velocity.Y = 16f;
                }
               
                if (targetNPC != null)
                    Projectile.position.X = targetNPC.Center.X - 120;
            }
            if (hasLanded)
            {
                foreach (NPC npc in hitNPCs)
                {
                    if (npc != null && npc.active)
                    {
                        npc.velocity = Vector2.Zero; // 停止 NPC 移动

                        // 如果 NPC 在空中，将其压到地面
                        if (!Collision.SolidCollision(npc.position + new Vector2(0, 16), npc.width, npc.height))
                        {
                            npc.position.Y += 16; // 向下移动
                        }
                    }
                }

                if (Projectile.alpha < 100)
                {
                    Projectile.alpha += 2;
                }
            }


            // 更新帧动画
            if (Projectile.frame < 11)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 10) // 每 10 帧切换下一帧
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
            }
            else
            {
                Projectile.frame = 11;
            }

            if(Projectile.timeLeft <= 60)
            {
                Projectile.timeLeft = 60;
                Projectile.scale -= 0.05f;
                if(Projectile.scale < 0.5f)
                {
                    Projectile.Kill();
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.lifeMax > 5 && !hitNPCs.Contains(target))
            {
                hitNPCs.Add(target); // 添加到列表
                Projectile.tileCollide = true; // 允许与物块碰撞
                isHitTarget = true;
            }
        }

        private NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;
            float closestDistance = maxDetectDistance;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.dontTakeDamage && npc.lifeMax > 0 && npc.CanBeChasedBy())
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPC = npc;
                    }
                }
            }
            return closestNPC;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 如果弹幕垂直方向碰撞
            if (Projectile.velocity.Y != oldVelocity.Y && isHitTarget)
            {
                hasLanded = true; // 标记弹幕已落地
                Projectile.velocity.Y = 0; // 停止垂直移动
            }
            return false; // 不销毁弹幕
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
                Projectile.Center - Main.screenPosition + new Vector2(0,80),
                rectangle,
                lightColor*Projectile.Opacity,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                Projectile.scale * 2f,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}
