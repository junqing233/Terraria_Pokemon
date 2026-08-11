using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.MunchlaxProj
{
    public class MunchlaxProj4 : ModProjectile
    {
        private Player player;
        private int particleCount = 0;
        private float rotationAngle = 0f;

        public override string Texture => "Pokemon/Projectiles/BlankProj/BlankProj1";

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
            Projectile.timeLeft = 240; // 存在时间，单位为帧
            Projectile.alpha = 255; // 透明度
            Projectile.light = 0.2f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 10; //独立无敌帧时间
            base.SetDefaults();
        }

        public override void AI()
        {
            Projectile.damage = 0;
            // 获取玩家实例
            player = Main.player[Projectile.owner];

            // 增加玩家防御力
            player.statDefense *= 1.2f;

            // 初始化粒子数量
            if (particleCount == 0)
            {
                //particleCount = Main.rand.Next(2, 4); // 随机生成 2 到 3 个粒子
                particleCount = 3; // 固定生成粒子数量
            }

            // 计算粒子绕玩家旋转
            rotationAngle += 0.15f; // 每帧增加旋转角度
            for (int i = 0; i < particleCount; i++)
            {
                float angle = rotationAngle + MathHelper.TwoPi / particleCount * i;
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 50f; // 半径为 50 像素
                Vector2 particlePosition = player.Center + offset;

                // 生成灰色粒子
                Dust dust = Dust.NewDustPerfect(particlePosition, DustID.WhiteTorch, Vector2.Zero, 1, Color.Silver, 2f);
                dust.noGravity = true; // 无视重力
            }
        }

        public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor)
        {
            return false; // 不绘制弹幕
        }
    }
}
