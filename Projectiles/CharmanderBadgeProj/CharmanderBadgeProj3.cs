using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Pokemon.Projectiles.GastlyBadgeProj;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.CharmanderBadgeProj
{
    public class CharmanderBadgeProj3 : ModProjectile
    {
       public static bool isjump = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 10; // 弹幕宽度
            Projectile.height = 10; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = 1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 360; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 0.75f; // 发光亮度
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                //找到GastlyBadgeProj3
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<CharmanderBadgeProj1>())
                {
                    if (Main.projectile[i].direction == -1)
                    Projectile.Center = Main.projectile[i].Center + new Vector2(10, 0); // ; // 跟踪GastlyBadgeProj3
                    else if (Main.projectile[i].direction == 1)
                    Projectile.Center = Main.projectile[i].Center - new Vector2(10, 0); // ; // 跟踪GastlyBadgeProj3
                    //Projectile.velocity = Main.projectile[i].velocity;
                }
            }
            Projectile.scale = 0.1f;// 弹幕大小
        }
      
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.damage = 0; // 伤害为0
            //Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            // 更新帧动画
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 1) // 每5帧切换下一帧
            {
                Projectile.frame++;
                Projectile.frame %= Main.projFrames[Projectile.type]; // 循环动画
                Projectile.frameCounter = 0;
            }
            
            if (Projectile.scale < 1.4f)
            {
                Projectile.scale += 0.025f; // 弹幕大小变化
            }
            if(Projectile.scale > 1.4f)
            {
                Projectile.Kill(); // 销毁弹幕
            }
            Projectile.rotation += Main.rand.Next(-10, 10) * 0.1f; // 随机旋转

            NPC target = null;

            // 如果目标是空的或者失活的，那么重新寻找敌人
            if (target == null || !target.active || !target.CanBeChasedBy())
            {
                // 寻找1500像素范围内最近敌人（最多隔两格墙）
                target = CharmanderBadgeProj1.FindTargetWithinRange(player, 1200f);
               
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                //找到GastlyBadgeProj3
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<CharmanderBadgeProj1>())
                {
                    if(target!= null && target.active)
                    {
                        if (Main.projectile[i].position.X < target.position.X)
                            Projectile.Center = Main.projectile[i].Center + new Vector2(10, 0); // ; // 跟踪GastlyBadgeProj3
                        else
                            Projectile.Center = Main.projectile[i].Center - new Vector2(10, 0); // ; // 跟踪GastlyBadgeProj3
                    }
                }
            }
            if (target== null)
            {
                Projectile.Kill(); // 销毁弹幕
            }
            if(Projectile.timeLeft == 320)
            {
                isjump = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }
        

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            NPC target = null; // 先设出目标NPC，默认为空
            if (target == null || !target.active) // 如果目标是空的或者失活的，那么重新寻找敌人
            {
                int t = Projectile.FindTargetWithLineOfSight(1200); // 寻找1500像素范围内最近敌人号码（不隔墙）
                // 这个方法如果在没有敌怪时会返回-1，用来检测是否能找到敌人
                if (t >= 0)
                {
                    target = Main.npc[t]; // 定义这个NPC为目标
                }
            }

            Player player = Main.player[Projectile.owner];
            if(target!= null && target.active)
            //生成新的弹幕
            Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16f,
                ModContent.ProjectileType<CharmanderBadgeProj2>(), // 生成我们自己写的弹幕
                Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.8f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
            
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
                Projectile.scale * 0.8f,
                SpriteEffects.None,
                0);
            return false;
        }
    }
}
