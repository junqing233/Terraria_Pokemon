using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.MunchlaxProj
{
    public class MunchlaxProj2 : ModProjectile
    {
        private bool returningToMunchlaxProj1 = false; // 是否正在返回
        private Vector2 munchlaxProj1Position; // MunchlaxProj1 的位置

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
            Projectile.tileCollide = false; // 与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 120; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 0.2f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 10; //独立无敌帧时间
            base.SetDefaults();
        }

        public override void AI()
        {
            bool isFindingMunchlaxProj1 = false;
            // 击中敌人后，找到 MunchlaxProj1 的位置
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<MunchlaxProj1>() && proj.owner == Projectile.owner)
                {
                    munchlaxProj1Position = proj.Center;
                    isFindingMunchlaxProj1 = true;
                    break;
                }
            }
            if(!isFindingMunchlaxProj1)
                Projectile.Kill();
            if (returningToMunchlaxProj1)
            {
                // 朝向 MunchlaxProj1 的位置移动
                Vector2 direction = munchlaxProj1Position - Projectile.Center;
                float speed = 26f; // 返回速度
                direction.Normalize();
                Projectile.velocity = direction * speed;

                // 如果接近 MunchlaxProj1，则销毁
                if (Vector2.Distance(Projectile.Center, munchlaxProj1Position) < 10f)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                // 追踪最近的敌人
                NPC target = FindClosestNPC(200f);
                if (target != null)
                {
                    Vector2 direction = target.Center - Projectile.Center;
                    float speed = 26f; // 追踪速度
                    direction.Normalize();
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * speed, 0.1f); // 平滑调整速度
                }
                // 正常旋转逻辑
                Projectile.rotation = Projectile.velocity.ToRotation();
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(target.lifeMax > 5 && !target.friendly)
            returningToMunchlaxProj1 = true; // 设置为返回状态
            target.velocity = Projectile.velocity * 0.12f; // 减速
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                Main.EntitySpriteDraw(texture, oldcenter, rectangle, Color.White * factor,
                    Projectile.oldRot[i],
                    new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                    Projectile.scale * 0.4f,
                    SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                rectangle,
                lightColor,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                Projectile.scale * 0.4f,
                SpriteEffects.None,
                0);
            return false;
        }
    }
}
