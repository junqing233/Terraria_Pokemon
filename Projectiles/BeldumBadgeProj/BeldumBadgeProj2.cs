using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pokemon.Buffs;
using Pokemon.Content.DamageClasses;
using Pokemon.Content.Dusts;
using Pokemon.Content.Equipment;
using Pokemon.Content.Items;
using Pokemon.Projectiles.BulbasaurBadgeProj;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.BeldumBadgeProj
{
    public class BeldumBadgeProj2 : ModProjectile
    {
        //private float gotoX;// 目标X坐标
        //private const float spacing = 55;// 弹幕间距

        Player player => Main.player[Projectile.owner];
        //private int attackType = 0; // 攻击类型
        //private int attackTime = 0; // 攻击间隔
        //private int attackShooting_1 = 0; // 猛撞
        private int attackShooting_2 = 0; // 铁头
        private bool isattackShooting_2 = false; // 是否正在攻击铁头
        NPC targetNPC = null; // 目标NPC

        public override void SetDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            Projectile.width = 15; // 弹幕宽度
            Projectile.height = 25; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 无限穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 120; // 存在时间无限
            Projectile.alpha = 100; // 透明度
            Projectile.light = 0.75f; // 发光亮度
            Projectile.minion = true; // 设置为召唤物
            Projectile.minionSlots = 0f; // 占用一个召唤栏位
            Projectile.aiStyle = -1;//不使用原版AI
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 10; //独立无敌帧时间
            base.SetDefaults();
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;//设置动画帧数
            base.SetStaticDefaults();
        }
       
        public override bool? CanCutTiles()
        {
            return false;//我们不想召唤兽会割草
        }

        private Vector2 dashDirection; // 用于存储冲刺的方向
        private bool isDashing = false; // 用于判断是否正在冲刺
        private float dashDistance = 0f; // 用于存储冲刺的距离
        private int dashTimer = 0; // 用于计时冲刺
        private Vector2 currentDirection; // 用于存储当前的方向
        
        void AttackShooting_1(NPC target)
        {
            // 使 NPC 始终朝向敌人
            Vector2 currentDirection = target.Center - Projectile.Center;

            currentDirection.Normalize(); // 归一化方向向量

            // 每10秒冲刺一次
            if (dashTimer <= 0 && !isDashing
                && Projectile.Distance(target.Center) < (target.width * 5)) // 如果计时器为0且当前不在冲刺中
            {
                // 计算并存储冲刺的方向
                dashDirection = currentDirection; // 保存当前的方向
                isDashing = true; // 开始冲刺
                dashDistance = 0f; // 重置冲刺距离
                dashTimer = 120; // 冲刺计时器设置为360帧（6秒）
            }
            
            if (isDashing) // 如果正在冲刺
            {
                // 旋转弹幕
                if (Projectile.velocity.X > 0)
                    Projectile.rotation = Projectile.velocity.ToRotation();
                else
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

                Projectile.damage = Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 1f);
                
                // 设置 NPC 的速度为 16 像素，使用存储的方向
                Projectile.velocity = dashDirection * (12f+target.velocity.Length()); // 使用方向进行冲刺

                // 增加实际冲刺经过的距离
                dashDistance += Projectile.velocity.Length(); // 计算冲刺距离

                // 检查是否达到冲刺距离
                if (dashDistance >= 20 + target.width*4) // 冲刺600像素
                {
                    isDashing = false; // 停止冲刺
                }
            }
            else
            {
                Projectile.damage = 0;
                //朝向敌人
                if(Projectile.Center.X - target.Center.X > 0f)
                    Projectile.rotation = (Projectile.Center - target.Center).ToRotation();
                else
                    Projectile.rotation = (-Projectile.Center + target.Center).ToRotation();
                
                dashTimer--; // 计时器减一
                // 如果不冲刺，保持朝向玩家但速度为0

                //接近敌人
                Vector2 vector = target.Center - Projectile.Center;
                vector.Normalize();
                //如果距离敌人超过50像素
                if(Vector2.Distance(Projectile.Center, target.Center) > target.width*10)
                {
                    if(vector.X > 0)
                    Projectile.velocity = vector * ((target.velocity.X*0.4f)*1.5f+8);
                    else
                    Projectile.velocity = vector * ((-target.velocity.X*0.4f)*1.5f+8);
                    
                }else if(Vector2.Distance(Projectile.Center, target.Center) > target.width * 5)
                {
                    if (vector.X > 0)
                        Projectile.velocity = vector * ((target.velocity.X * 0.4f) * 2f+4);
                    else
                        Projectile.velocity = vector * ((-target.velocity.X * 0.4f) * 2f+4);
                }
                else if (Vector2.Distance(Projectile.Center, target.Center) > target.width * 3)
                {
                    if (vector.X > 0)
                        Projectile.velocity = vector * ((target.velocity.X * 0.4f) * 2f + 2);
                    else
                        Projectile.velocity = vector * ((-target.velocity.X * 0.4f) * 2f + 2);
                }
                else
                    Projectile.velocity *= 0.92f;
            }
        }

        void AttackShooting_2(NPC target)
        {
            isattackShooting_2 = false; // 重置意念头锤发射标志
            int foundProjectiles = 0; // 记录找到的弹幕数量

            // 遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == Projectile.owner && proj.type == ModContent.ProjectileType<BeldumBadgeProj5>())
                {
                    foundProjectiles++; // 增加找到的弹幕数量
                    if (foundProjectiles >= 3) // 如果找到的弹幕数量达到3个
                    {
                        isattackShooting_2 = true; // 标志意念头锤发射标志为真
                        break; // 退出循环
                    }
                }
            }
            if (!isattackShooting_2) // 检查是否已经发射了3个弹幕
            {
                attackShooting_2++; // 使用ai[2]作为计时器
                if (attackShooting_2 == 180) // 攻击间隔为180帧
                {
                    attackShooting_2 = 0; // 重置计时器
                    {
                        //生成新的弹幕
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                            Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f,
                            ModContent.ProjectileType<BeldumBadgeProj5>(), // 生成我们自己写的弹幕
                            Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.2f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                            target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
                    }
                }
            }
        }

        private int UpdateCount = 0;
        private bool isIncreasing = true; // 新增方向标记
        private int TimerCount = 0;

        public override void AI()
        {
            //if (player.HasBuff<BuffsBeldumBadge>()) // 如果玩家有召唤物BUFF
            //    Projectile.timeLeft = 2; // 维持住弹幕的时间

            NPC target = null; // 先设出目标NPC，默认为空
            targetNPC = null;
            // 这一段是当你的召唤兽设定了右键锁敌情况下必须要写的部分,防止进行寻敌判定
            if (player.HasMinionAttackTargetNPC)
            {
                target = Main.npc[player.MinionAttackTargetNPC]; // 让目标为鼠标锁住的敌人
                float between = Vector2.Distance(target.Center, Projectile.Center);
                // 小于2000防止锁住太远的敌人
                if (between < 2000f)
                {
                    target = null;
                    targetNPC = null;
                }
            }

            if (target == null || !target.active) // 如果目标是空的或者失活的，那么重新寻找敌人
            {
                target = BeldumBadgeProj2.FindTargetWithinRange(player, 1200f);
                targetNPC = target; // 记录目标NPC
            }
            if (target != null && target.active && player.HasBuff<BuffsBeldumBadge>()) // 如果目标不为空且存活
            {
                TimerCount = 0;
                Projectile.timeLeft = 2; // 维持住弹幕的时间
                Projectile.alpha = 100; // 透明度
            }
            else
            {
                Projectile.velocity = Vector2.Zero; // 弹幕速度为0

                if (isIncreasing)
                {
                    UpdateCount += 10;
                    if (UpdateCount >= 250)
                    {
                        isIncreasing = false; // 到达上限切换方向
                    }
                }
                else
                {
                    UpdateCount -= 10;
                    if (UpdateCount <= 0)
                    {
                        isIncreasing = true; // 到达下限切换方向
                    }
                }
                Projectile.alpha = UpdateCount;
                Projectile.rotation = 0;
                TimerCount++;
                if(TimerCount > 180)
                {
                    Projectile.Kill(); // 弹幕消失
                }else
                Projectile.timeLeft = 2; // 弹幕存在时间无限
            }
                
            if (target != null && target.active && player.HasBuff<BuffsBeldumBadge>()) // 如果目标不为空且存活在此处执行攻击性AI
            {
                AttackShooting_1(target); // 执行攻击逻辑
                AttackShooting_2(target); // 执行铁头攻击逻辑
            }
            else // 否则说明没目标了，执行回归待机运动
            {
                Projectile.damage = 0;
                Projectile.velocity = Vector2.Zero; // 弹幕速度为0
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] > 2)
            {
                // 更新帧计数器
                Projectile.ai[0]++;
                Projectile.ai[1] = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            ProjectileID.Sets.TrailingMode[Type] = 2;//设置尾迹模式为2，即尾迹为圆形
            ProjectileID.Sets.TrailCacheLength[Type] = 8;//设置尾迹缓存长度为5，即最多保留5个尾迹

            if (targetNPC != null && targetNPC.active)
            {
                if (targetNPC.position.X < Projectile.position.X)
                {
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 4) + 4; // 使用第5到第8帧（向左移动）
                }
                else
                {
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 4) + 8; // 使用第9到第12帧（向右移动）
                }
            }
            else
            {
                // 根据速度方向选择帧数
                if (Projectile.velocity.X < -0)
                {
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 4) + 4; // 使用第5到第8帧（向左移动）
                }
                else if (Projectile.velocity.X >= 0)
                {
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 4) + 8; // 使用第9到第12帧（向右移动）
                }
                else if (Projectile.velocity.X > -6 && Projectile.velocity.X < 6)
                {
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 4); // 使用第1到第4帧（不动）
                }
            }

            Rectangle rectangle = new Rectangle(
                0,
                texture.Height / Main.projFrames[Type] * Projectile.frame,
                texture.Width,
                texture.Height / Main.projFrames[Type]
            );

           
            Color MyColor = new Color(160, 100, 160)* 0.5f;
            MyColor.A = 0;

            if (Projectile.velocity.Length() > 8)
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
                {
                    float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                    Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, oldcenter + new Vector2(0, -5), rectangle, MyColor * factor * Projectile.Opacity,
                        Projectile.oldRot[i],
                        new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                        new Vector2(0.8f),
                        SpriteEffects.None, 0);
                }

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition + new Vector2(0,-5),
                rectangle,
                lightColor * Projectile.Opacity,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                new Vector2(0.8f),
                SpriteEffects.None,
                0);

            return false;
        }
        public static NPC FindTargetWithinRange(Player player, float range)
        {
            NPC target = null;
            float closestDistance = float.MaxValue;

            // 遍历所有活跃的NPC
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                // 检查NPC是否活跃且不是玩家控制的角色，并且可以被追逐
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    // 计算玩家到NPC的距离
                    float distance = Vector2.Distance(player.Center, npc.Center);

                    // 如果距离在指定范围内，则进一步检查是否最多隔两格墙
                    if (distance <= range && CanSeeThroughTwoWalls(player.Center, npc.Center))
                    {
                        // 如果距离比当前最近的距离更近，则更新目标
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            target = npc;
                        }
                    }
                }
            }

            return target; // 返回找到的目标NPC，如果没有找到则返回null
        }

        // 检查是否可以通过最多两格墙看到目标
        private static bool CanSeeThroughTwoWalls(Vector2 playerCenter, Vector2 npcCenter)
        {
            //遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.owner == Main.myPlayer &&
                    projectile.type == ModContent.ProjectileType<BeldumBadgeProj1>())
                {
                    Vector2 line = npcCenter - projectile.Center;
                    float length = line.Length();
                    line.Normalize();

                    int wallCount = 0;

                    for (float f = 0; f < length; f += 1f)
                    {
                        Vector2 position = projectile.Center + line * f;
                        int x = (int)(position.X / 16);
                        int y = (int)(position.Y / 16);

                        // 检查当前位置是否是墙壁
                        if (WorldGen.SolidTile(x, y) && Main.tile[x, y].HasTile && Main.tile[x, y].TileType > 0)
                        {
                            wallCount++;
                            if (wallCount > 100)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}

