using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Buffs;
using Pokemon.Content.DamageClasses;
using Pokemon.Content.Equipment;
using Pokemon.Content.Items;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.FomantisBadgeProj
{
    public class FomantisBadgeProj4 : ModProjectile
    {
        private int timer = 0;
        private bool isfindtarget = false;//是否找到目标
        private int shootTimer = 0; // 发射计时器
        private bool fixedAbovePlayer = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 30; // 弹幕宽度
            Projectile.height = 30; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 360; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 2f; // 发光亮度
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.damage = 0; // 伤害为0
            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            if (Projectile.scale < 1f)
            {
                timer++;
                if (timer > 2)
                {
                    Projectile.scale += 0.05f;
                    timer = 0;
                }
            }

            Player player = Main.player[Projectile.owner];

            if (!player.HasBuff(ModContent.BuffType<BuffsFomantisBadge>()))
            {
                Projectile.Kill();
                return;
            }

            // 目标位置：玩家上方60像素
            Vector2 targetPos = player.Center + new Vector2(0, -120);

            // 缓动到目标位置
            float speed = 12f;
            Vector2 toTarget = targetPos - Projectile.Center;
            
            if (toTarget.Length() > 12f)
            {
                Projectile.velocity = toTarget.SafeNormalize(Vector2.Zero) * speed;
            }
            else
            {
                // 到达后锁定在玩家上方并跟随
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = targetPos;
                fixedAbovePlayer = true;
            }
            // 每2秒发射一个FomantisBadgeProj4_弹幕
            if (fixedAbovePlayer && isfindtarget)
            {
                shootTimer++;
                if (shootTimer >= 120)
                {
                    shootTimer = 0;
                    if (Main.myPlayer == Projectile.owner) // 只在本地玩家发射，防止多次
                    {
                        Projectile.NewProjectile(
                            Projectile.GetSource_FromAI(),
                            Projectile.Center,
                            Vector2.Zero,
                            ModContent.ProjectileType<FomantisBadgeProj4_>(),
                            Projectile.originalDamage,
                            0f,
                            Projectile.owner
                        );
                    }
                }
            }
            else
            {
                shootTimer = 120; // 到达前不发射，防止提前计时
            }
            // 其余逻辑可保留
            NPC target = FomantisBadgeProj1.FindTargetWithinRange(player, 1200);
            if (target != null)
            {
                isfindtarget = true;
            }
            else
            {
                Projectile.scale -= 0.02f;
                if (Projectile.scale < 0.1f)
                {
                    Projectile.Kill();
                }
                isfindtarget = false;
            }

            if (isfindtarget)
            {
                Projectile.timeLeft = 180;
            }
        }

        [Obsolete]
        public override void Kill(int timeLeft)
        {
            
        }

        public override bool PreDraw(ref Color lightColor)
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
                Color.White,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }
    }
    public class FomantisBadgeProj4_ : ModProjectile
    {
        public override string Texture => "Pokemon/Projectiles/FomantisBadgeProj/FomantisBadgeProj4";

        private Vector2 startPos;
        private Vector2 controlPos;
        private Vector2 targetPos;
        private float t = 0f;
        private bool healed = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
            Projectile.alpha = 1;
            Projectile.light = 0.5f;
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.damage = 0;
            Projectile.scale = 0.5f;
            startPos = Projectile.Center;
            Player player = Main.player[Projectile.owner];
            targetPos = player.Center;
            // 控制点：中点左右随机偏移，形成弧线
            float side = Main.rand.NextBool() ? 1f : -1f;
            controlPos = (startPos + targetPos) / 2 + new Vector2(80f * side, -80f);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            targetPos = player.Center;
            // 贝塞尔插值
            t += 0.025f; // 控制移动速度
            if (t > 1f) t = 1f;
            Vector2 pos = (1 - t) * (1 - t) * startPos + 2 * (1 - t) * t * controlPos + t * t * targetPos;
            Projectile.Center = pos;

            // 到达玩家附近回血
            if (!healed && Vector2.Distance(Projectile.Center, player.Center) < 32f)
            {
                if (Projectile.originalDamage / 4 < 1)
                {
                    //恢复玩家血量
                    player.statLife += 1; // 恢复生命值
                    player.HealEffect(1); // 显示恢复生命的效果
                }
                else
                {
                    //恢复玩家血量
                    player.statLife += Projectile.originalDamage / 4; // 恢复生命值
                    player.HealEffect(Projectile.originalDamage / 4); // 显示恢复生命的效果
                }
                //player.statLife += Projectile.originalDamage;
                //player.HealEffect(Projectile.originalDamage, true);
                healed = true;
                Projectile.Kill();
            }
        }

        [Obsolete]
        public override void Kill(int timeLeft)
        {
        }

        public override bool PreDraw(ref Color lightColor)
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
                Color.White,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                Projectile.scale * 0.5f,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}
