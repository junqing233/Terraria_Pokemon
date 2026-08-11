using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Pokemon.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Pokemon.Projectiles.GastlyBadgeProj
{
    public class GastlyBadgeProj8 : ModProjectile
    {
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
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 360; // 存在时间，单位为帧
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
            Projectile.scale = Main.rand.Next(3, 5) / 10f; // 随机大小
        }

        public override void AI()
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
            Player player = Main.player[Projectile.owner]; // 定义玩家
            Vector2 directionToPlayer = player.Center - Projectile.Center;
            directionToPlayer.Normalize(); // 归一化方向
            
            if (Projectile.velocity.Length() < 21f)
            {
                Projectile.velocity += directionToPlayer * 0.1f; // 设置向玩家的速度
                Projectile.position = Projectile.position + Projectile.velocity; // 弹幕跟随玩家
            }
            else
            {
                Projectile.velocity += directionToPlayer * 0.05f; // 设置向玩家的速度
                Projectile.position = Projectile.position + Projectile.velocity; // 弹幕跟随玩家
            }
            if (Projectile.position.Distance(player.position) <= 1200f && Projectile.timeLeft < 300)
            {
                Projectile.velocity = directionToPlayer * 4f; // 设置向玩家的速度
                Projectile.position = Projectile.position + Projectile.velocity; // 弹幕跟随玩家
            }
            if (Projectile.position.Distance(player.position) <= 30f)
            {
                if(Projectile.originalDamage / 4 < 1)
                {
                    //恢复玩家血量
                    player.statLife += 1; // 恢复生命值
                    player.HealEffect(1); // 显示恢复生命的效果
                }else
                {
                    //恢复玩家血量
                    player.statLife += Projectile.originalDamage / 4; // 恢复生命值
                    player.HealEffect(Projectile.originalDamage / 4); // 显示恢复生命的效果
                }
               
                int dust = Dust.NewDust(player.Center + new Vector2(-player.width / 2, 0), player.width, player.height, 
                    ModContent.DustType<GastlyDust_2>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].velocity = new Vector2(0, 0);
                Main.dust[dust].scale = 0.2f; // 设置大小
                Main.dust[dust].velocity *= 0.1f; // 设置速度
                Projectile.Kill(); // 消失
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = 0; // 伤害为0

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, Projectile.width + Main.rand.Next(-40, 40),
                    Projectile.height + Main.rand.Next(-40, 40), DustID.PurpleCrystalShard, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
                Main.dust[dust].color = Color.White; // 设置颜色
                Main.dust[dust].noGravity = true; // 让灰尘不受重力影响
                Main.dust[dust].scale = 1f; // 设置大小
                Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.05f; // 设置渐入时间
            }
        }

        public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor)
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
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                Main.EntitySpriteDraw(texture, oldcenter, rectangle, MyColor * factor,
                    Projectile.oldRot[i],
                    new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                    Projectile.scale * 1f,
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
