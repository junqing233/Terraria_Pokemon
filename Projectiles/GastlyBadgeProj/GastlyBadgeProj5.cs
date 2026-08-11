using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.GastlyBadgeProj
{
    public class GastlyBadgeProj5 : ModProjectile
    {
        private int AttackTimer = 0;
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 4; // 弹幕宽度
            Projectile.height = 4; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 360; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 0.75f; // 发光亮度
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            // 遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                // 找到GastlyBadgeProj4
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<GastlyBadgeProj4>())
                {
                    Projectile.Center = Main.projectile[i].Center;
                    break; // 找到一个GastlyBadgeProj4后退出循环
                }
            }
            Projectile.velocity = Vector2.Zero;
            Projectile.damage = 0;
            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            if(Projectile.scale < 1f)
            {
                timer++;
                if (timer >= 2)
                {
                    Projectile.scale += 0.05f;
                    timer = 0;
                }
            }
            Vector2 center = Vector2.Zero;
            bool foundGastlyBadgeProj4 = false;
            NPC target = null;
            Player player = Main.player[Projectile.owner];
            // 遍历所有弹幕，找到GastlyBadgeProj4的中心位置
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                // 找到GastlyBadgeProj4
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<GastlyBadgeProj4>())
                {
                    center = Main.projectile[i].Center;
                    foundGastlyBadgeProj4 = true;
                    break; // 找到一个GastlyBadgeProj4后退出循环
                }
            }
           
            if (!foundGastlyBadgeProj4)
            {
                // 如果没有找到GastlyBadgeProj4，则弹幕消失
                Projectile.Kill();
                return;
            }

            // 计算与GastlyBadgeProj4的距离
            float distanceToGastlyBadgeProj4 = (center - Projectile.Center).Length();

            if (distanceToGastlyBadgeProj4 >= 40)
            {
                // 如果超出40像素范围，向中心移动
                Vector2 direction = center - Projectile.Center;
                direction.Normalize();
                Projectile.velocity = direction * 10; // 调整速度以向中心移动
            } 
            else
            {
                // 在40像素范围内，寻找敌人
                int t = Projectile.FindTargetWithLineOfSight(1200); // 寻找1200像素范围内的最近敌人（不隔墙）

                if (t >= 0)
                {
                    // 计算与敌人的方向
                    Vector2 directionToTarget = Main.npc[t].Center - center;
                    directionToTarget.Normalize();
                    target = Main.npc[t]; // 定义这个NPC为目标
                    // 偏移方向为敌人方向，但限制在40像素范围内
                    Projectile.Center = center + directionToTarget * 20;
                    AttackTimer++;
                    Projectile.timeLeft = 180; // 弹幕存在时间
                    if (AttackTimer >= 60)
                    {
                        AttackTimer = 0;
                        //生成新的弹幕
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                            Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16f,
                            ModContent.ProjectileType<GastlyBadgeProj6>(), // 生成我们自己写的弹幕
                            Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.55f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                            target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
                    }
                }
                else
                {
                    Projectile.scale -= 0.025f;
                    // 如果没有找到敌人，则保持在GastlyBadgeProj4中心
                    Projectile.Center = center;
                }
            }
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
