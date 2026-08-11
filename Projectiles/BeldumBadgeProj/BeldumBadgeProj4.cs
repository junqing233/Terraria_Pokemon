using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.BeldumBadgeProj
{
    public class BeldumBadgeProj4 : ModProjectile
    {
        private bool isHit = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 8; // 弹幕宽度
            Projectile.height = 8; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 45; // 存在时间，单位为帧
            Projectile.alpha = 100; // 透明度
            Projectile.light = 0.5f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 10; //独立无敌帧时间
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            // 遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                // 找到GastlyBadgeProj4
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BeldumBadgeProj3>())
                {
                    Projectile.Center = Main.projectile[i].Center;
                    break; // 找到一个GastlyBadgeProj4后退出循环
                }
            }
            Projectile.damage = 0;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if(Main.rand.Next(4) < 1)
            modifiers.SetCrit(); // 设置爆击
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.damage = Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.2f);
            // 追踪范围
            float trackingRange = 200f;
            NPC target = null;
            float closestDistance = trackingRange;

            // 寻找最近的敌人
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = npc;
                    }
                }
            }

            // 如果找到目标，调整弹幕的速度和方向
            if (target != null && !isHit)
            {
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * Projectile.velocity.Length(), 0.5f);
            }

            Projectile.velocity *= 1.03f; // 速度增加
            Projectile.rotation = Projectile.velocity.ToRotation(); // 速度方向
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            isHit = true;
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

            Color MyColor = Color.DeepPink * 0.8f;
            MyColor.A = 0;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                Main.EntitySpriteDraw(texture, oldcenter, rectangle, Color.DeepPink * factor * 0.8f,
                    Projectile.oldRot[i],
                    new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                    Projectile.scale * 0.45f,
                    SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                rectangle,
                lightColor * 0.5f,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                Projectile.scale * 0.4f,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}
