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

namespace Pokemon.Projectiles.GastlyBadgeProj
{
    public class GastlyBadgeProj3 : ModProjectile
    {
        public static int Gasji = 0;
        private bool VelocityChange = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 20; // 弹幕宽度
            Projectile.height = 20; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = 6; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 120; // 存在时间，单位为帧
            Projectile.alpha = 10; // 透明度
            Projectile.light = 0.75f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 2; //独立无敌帧时间
            base.SetDefaults();
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(4);// 随机选择帧
            Projectile.rotation = Projectile.velocity.X * 0.01f; // 随机旋转
        }
        
        public override void AI()
        {
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
                //遍历弹幕
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.owner == Projectile.owner && proj.type == ModContent.ProjectileType<GastlyBadgeProj2>())
                    {
                        //计算移动到目标弹幕的距离
                        float distance = Vector2.Distance(proj.Center, Projectile.Center);
                        if(distance < 300)
                        {
                            //如果距离小于100，则向目标弹幕方向移动
                            if (distance > 20)
                            {
                                Projectile.velocity = (proj.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10;
                            }
                            else
                            {
                                Gasji++;
                                Projectile.Kill();
                            }
                        }
                    }
                }
            }
            
        }
       
        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                DustID.BubbleBurst_Purple, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default(Color), 1.5f);
            Main.dust[dustIndex].noGravity = true;
            Main.dust[dustIndex].scale = 2f;
            Main.dust[dustIndex].color = Color.DarkGreen;
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
