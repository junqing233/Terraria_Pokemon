using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.BulbasaurBadgeProj
{
    public class BulbasaurBadgeProj5 : ModProjectile
    {
        public override string Texture => "Pokemon/Projectiles/BulbasaurBadgeProj/BulbasaurBadgeProj5";

        private NPC target; // 用于保存目标NPC
        private int timer = 0; // 计时器，用于控制弹幕的持续时间
        private const int NumSegments = 10; // 定义弹幕的节数
        private Vector2[] segmentsPosition = new Vector2[NumSegments]; // 保存每一节的位置
        private Vector2 Center; // 弹幕中心位置
        private int time = 0; // 计时器，用于控制弹幕的持续时间
        private bool isdraw = false; // 是否绘制
        

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 18; // 弹幕宽度
            Projectile.height = 18; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 600; // 存在时间，单位为帧
            Projectile.alpha = 10; // 透明度
            Projectile.light = 0.5f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 120; //独立无敌帧时间

            // 初始化每一节的位置
            for (int i = 0; i < NumSegments; i++)
            {
                segmentsPosition[i] = Vector2.Zero;
            }

            base.SetDefaults();
        }

        public override void AI()
        {
            float distance1 = Vector2.Distance(Projectile.Center, Center); // 计算距离
            // 获取玩家挂载中心位置
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            //isfindprojectile = false; // 重置标志位
            Player player = Main.player[Projectile.owner];
            //遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                //找到BulbasaurBadgeProj1弹幕
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BulbasaurBadgeProj1>())
                {
                    //获取弹幕中心
                    Center = Main.projectile[i].Center;
                    //isfindprojectile = true;
                    break; //找到后立即退出循环
                }
            }

            if (!player.HasBuff(ModContent.BuffType<Buffs.BuffsBulbasaurBadge>()))
            {
                Projectile.Kill();
                return;
            }

            float radius = Vector2.Distance(Center, Projectile.Center);

            //遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].hostile && Main.projectile[i].active)
                {
                    float distance = Vector2.Distance(Main.projectile[i].Center, Center); // 计算距离
                    if (distance < radius && distance > radius / 2)
                    {
                        // 随机选择左转或右转90度
                        float rotationAngle = Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2;
                        Main.projectile[i].velocity = Main.projectile[i].velocity.RotatedBy(rotationAngle); // 旋转弹幕的速度方向
                        Main.projectile[i].hostile = false; // 击中后将其变为友方弹幕
                        Main.projectile[i].friendly = true; // 击中后将其变为友方弹幕
                        time += 1; // 击中后计时器加一
                    }
                }
            }

            // 如果目标是空的或者失活的，那么重新寻找敌人
            if (target == null || !target.active || !target.CanBeChasedBy())
            {
                int t = Projectile.FindTargetWithLineOfSight(1200); // 寻找1500像素范围内最近敌人号码（不隔墙）
                if (t >= 0)
                {
                    target = Main.npc[t]; // 定义这个NPC为目标
                }
            }

            // 如果目标不为空且存活在此处执行攻击性AI
            if (target != null && target.active && target.damage > 0)
            {
                timer++; // 计时器加一

                if (timer > 35)
                {
                    //Projectile.timeLeft = 34;
                    isdraw = true;
                    if (Projectile.timeLeft > 60 && time <= 16)
                        Projectile.Center = Center + new Vector2(0, 175);
                }
                else
                {
                    Projectile.position += new Vector2(0, 5); // 向下移动
                }

                // 计算目标方向的角度
                float targetAngle = (float)Math.Atan2(target.Center.Y - Center.Y, target.Center.X - Center.X);

                // 更新每一节的位置
                segmentsPosition[0] = Center;
                for (int i = 1; i < NumSegments; i++)
                {
                    segmentsPosition[i] = segmentsPosition[i - 1] + new Vector2(SegmentLength * (float)Math.Cos(targetAngle), SegmentLength * (float)Math.Sin(targetAngle));
                }
            }
            else
            {
                isdraw = false;
                Projectile.position -= new Vector2(0, 5); // 向上移动
                if (distance1 < 10)
                {
                    timer = 0; // 计时器归零
                    Projectile.Kill();
                }
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.timeLeft = 60;
                isdraw = false;
                Projectile.position -= new Vector2(0, 5); // 向上移动
                if (distance1 < 10)
                {
                    timer = 0; // 计时器归零
                    Projectile.Kill();
                }
            }

            if (time > 32)
            {
                isdraw = false;
                Projectile.position -= new Vector2(0, 5); // 向上移动
                if (distance1 < 10)
                {
                    Projectile.Kill();
                }
            }
            // 遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                // 找到BulbasaurBadgeProj1弹幕
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BulbasaurBadgeProj1>())
                {
                    // 获取弹幕中心
                    Projectile.velocity = Main.projectile[i].velocity; // 弹幕速度与挂载的玩家一致
                    break; // 找到后立即退出循环
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter; // 玩家中心
            Vector2 projectileCenter = Projectile.Center; // 当前弹幕中心位置

            // 遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                // 找到BulbasaurBadgeProj1弹幕
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BulbasaurBadgeProj1>())
                {
                    // 获取弹幕中心
                    playerCenter = Main.projectile[i].Center;
                    break; // 找到后立即退出循环
                }
            }

            // 计算圆的半径
            float radius = Vector2.Distance(playerCenter, projectileCenter);

            // 遍历所有敌人
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.Hitbox.Intersects(targetHitbox))
                {
                    Vector2 targetCenter = npc.Center; // 目标中心
                    float distance = Vector2.Distance(targetCenter, playerCenter); // 计算目标与玩家的距离

                    if (distance <= radius) // 如果目标在圆内
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 处理击中NPC后的逻辑
        }

        private const float SegmentLength = 20f; // 每一节的长度

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter; // 玩家中心
            Vector2 projectileCenter = Projectile.Center; // 当前弹幕中心位置

            // 遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                // 找到BulbasaurBadgeProj1弹幕
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BulbasaurBadgeProj1>())
                {
                    // 获取弹幕中心
                    playerCenter = Main.projectile[i].Center;
                    break; // 找到后立即退出循环
                }
            }

            // 计算圆的半径
            float radius = Vector2.Distance(playerCenter, projectileCenter);

            // 计算线段的数量
            Texture2D lineTexture = ModContent.Request<Texture2D>("Pokemon/Projectiles/BulbasaurBadgeProj/BulbasaurBadgeProj5").Value;
            int segments = 360; // 绘制360个段，每个段对应1度

            for (int iw = 0; iw < segments; iw++)
            {
                // 计算每一段的角度
                float segmentAngle1 = MathHelper.ToRadians(iw); // 将角度转换为弧度

                // 计算每一段的位置
                Vector2 position = playerCenter + new Vector2(radius, 0).RotatedBy(segmentAngle1);
                
                // 绘制线段贴图
                Main.spriteBatch.Draw(
                    lineTexture,
                    position - Main.screenPosition, // 绘制的位置
                    null,
                    Color.White, // 选择颜色
                    segmentAngle1 - MathHelper.PiOver2 * 2, // 使用更新后的角度
                    new Vector2(lineTexture.Width / 2, lineTexture.Height / 2), // 适应中心点
                    0.5f, // 缩放
                    SpriteEffects.None,
                    0 // 层级
                );
            }
                Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Projectiles/BulbasaurBadgeProj/BulbasaurBadgeProj5").Value;
                Player player = Main.player[Projectile.owner];
                
                if(isdraw)
                // 遍历每一节鞭子
                for (int i = 0; i < NumSegments - 1; i++)
                {
                    // 计算每一节的角度
                    float segmentAngle = (float)Math.Atan2(segmentsPosition[i + 1].Y - segmentsPosition[i].Y, segmentsPosition[i + 1].X - segmentsPosition[i].X);

                    // 计算绘制的位置
                    Vector2 drawPos = segmentsPosition[i] - Main.screenPosition;

                    // 绘制鞭子的每一节
                    Main.spriteBatch.Draw(
                        texture,
                        drawPos,
                        null,
                        Color.White,
                        segmentAngle,
                        new Vector2(texture.Width / 2, texture.Height / 2),
                        0.7f, // 缩放
                        SpriteEffects.None,
                        0 // 层级
                    );
                }
                return false;
            }
        }
    }

