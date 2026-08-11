using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pokemon.Buffs;
using Pokemon.Content.DamageClasses;
using Pokemon.Content.Dusts;
using Pokemon.Content.Equipment;
using Pokemon.Content.Items;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.GastlyBadgeProj
{
    public class GastlyBadgeProj1 : ModProjectile
    {

        Player player => Main.player[Projectile.owner];
        private bool isFindTarget = false;//是否找到目标
        private int attackType = 0; // 攻击类型
        private int attackTime = 0; // 攻击间隔
        private int attackShooting_1 = 0; // 黑色目光
        private int attackShooting_2 = 0; // 暗影球
        //private int attackShooting_21 = 0; // 暗影球
        //private int attackShooting_22 = 0; // 暗影球
        private int attackShooting_3 = 0; // 催眠术
        private int attackShooting_4 = 0; //食梦
        GastlyBadge gastlyBadge = null;
        NPC targetNPC = null; // 目标NPC
        private int lastLevel = -1;

        public override void SetDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            Projectile.width = 50; // 弹幕宽度
            Projectile.height = 50; // 弹幕高度
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
        void MoveToTarget(Vector2 targetPos, float MaxSpeed = 20f, float accSpeed = 0.5f)//运用之前学到的惯性追击
        {
            //原理：比较目标和自己的横向或者纵向坐标差，然后给自己的速度加上向着差值变小前进的加速度
            //如果自己的速度坐标差一样，说明自己正在原理目标，需要更大的加速度，这里我设定的是2倍
            if (Projectile.Center.X - targetPos.X < 0f)
                Projectile.velocity.X += Projectile.velocity.X < 0 ? 2 * accSpeed : accSpeed;
            else
                Projectile.velocity.X -= Projectile.velocity.X > 0 ? 2 * accSpeed : accSpeed;

            if (Projectile.Center.Y - targetPos.Y < 0f)
                Projectile.velocity.Y += Projectile.velocity.Y < 0 ? 2 * accSpeed : accSpeed;
            else
                Projectile.velocity.Y -= Projectile.velocity.Y > 0 ? 2 * accSpeed : accSpeed;
            if (Math.Abs(Projectile.velocity.X) > MaxSpeed)//如果横向速度超越最大值，则回到最大值
                Projectile.velocity.X = MaxSpeed * Math.Sign(Projectile.velocity.X);
            if (Math.Abs(Projectile.velocity.Y) > MaxSpeed)//如果纵向速度超越最大值，则回到最大值
                Projectile.velocity.Y = MaxSpeed * Math.Sign(Projectile.velocity.Y);

        }

        public override bool? CanCutTiles()
        {
            return false;//我们不想召唤兽会割草
        }

        //暗影球
        void AttackShooting_2(NPC target)
        {
            attackShooting_2++; // 使用ai[2]作为计时器
            if (attackShooting_2 == (gastlyBadge.level >= 0 && gastlyBadge.level < 6 ? 300 : (gastlyBadge.level >= 6 && gastlyBadge.level < 9 ? 240 : 180))) // 攻击间隔为120帧
            {
                attackShooting_2 = 0; // 重置计时器

                for (int i = 0; i < 6; i++)
                {
                    //生成新的弹幕
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                        Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f,
                        ModContent.ProjectileType<GastlyBadgeProj3>(), // 生成我们自己写的弹幕
                        Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.6f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                        target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
                }
                //生成新的弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                    Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f,
                    ModContent.ProjectileType<GastlyBadgeProj2>(), // 生成我们自己写的弹幕
                    Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.6f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                    target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
            }
        }
        ////暗影球
        //void AttackShooting_21(NPC target)
        //{
        //    attackShooting_21++; // 使用ai[2]作为计时器
        //    if (attackShooting_21 == 300) // 攻击间隔为120帧
        //    {
        //        attackShooting_21 = 0; // 重置计时器

        //        for (int i = 0; i < 6; i++)
        //        {
        //            //生成新的弹幕
        //            Projectile.NewProjectile(Projectile.GetSource_FromAI(),
        //                Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f,
        //                ModContent.ProjectileType<GastlyBadgeProj3>(), // 生成我们自己写的弹幕
        //                Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.6f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
        //                target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
        //        }
        //        //生成新的弹幕
        //        Projectile.NewProjectile(Projectile.GetSource_FromAI(),
        //            Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f,
        //            ModContent.ProjectileType<GastlyBadgeProj2>(), // 生成我们自己写的弹幕
        //            Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.6f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
        //            target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
        //    }
        //}
        ////暗影球
        //void AttackShooting_22(NPC target)
        //{
        //    attackShooting_22++; // 使用ai[2]作为计时器
        //    if (attackShooting_22 == 240) // 攻击间隔为120帧
        //    {
        //        attackShooting_22 = 0; // 重置计时器

        //        for (int i = 0; i < 6; i++)
        //        {
        //            //生成新的弹幕
        //            Projectile.NewProjectile(Projectile.GetSource_FromAI(),
        //                Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f,
        //                ModContent.ProjectileType<GastlyBadgeProj3>(), // 生成我们自己写的弹幕
        //                Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.6f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
        //                target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
        //        }
        //        //生成新的弹幕
        //        Projectile.NewProjectile(Projectile.GetSource_FromAI(),
        //            Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f,
        //            ModContent.ProjectileType<GastlyBadgeProj2>(), // 生成我们自己写的弹幕
        //            Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.6f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
        //            target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
        //    }
        //}
        //黑色目光
        void AttackShooting_1(NPC target)
        {
            attackShooting_1++; // 使用ai[2]作为计时器
            if (attackShooting_1 == 180) // 攻击间隔为120帧
            {
                attackShooting_1 = 0; // 重置计时器
                {
                    //生成新的弹幕
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                        Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f,
                        ModContent.ProjectileType<GastlyBadgeProj4>(), // 生成我们自己写的弹幕
                        Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.8f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                        target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
                }

            }
        }
        //催眠术
        void AttackShooting_3(NPC target)
        {
            attackShooting_3++; // 使用ai[2]作为计时器
            if (attackShooting_3 == 180) // 攻击间隔为120帧
            {
                attackShooting_3 = 0; // 重置计时器

                //生成新的弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                    Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16f,
                    ModContent.ProjectileType<GastlyBadgeProj7>(), // 生成我们自己写的弹幕
                    Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.8f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                    target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
            }
        }
        //食梦
        void AttackShooting_4(NPC target)
        {
            attackShooting_4++; // 使用ai[2]作为计时器
            if (attackShooting_4 == 180) // 攻击间隔为120帧
            {
                for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center + new Vector2(-Projectile.width / 2, 0),
                    Projectile.width, Projectile.height, ModContent.DustType<GastlyDust_2>(),
                    Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[dust].velocity = new Vector2(0, 0);
                    Main.dust[dust].scale = 0.5f; // 设置大小
                    Main.dust[dust].velocity *= 0.1f; // 设置速度
                    //Main.dust[dust].fadeIn = 4f + (float)Main.rand.Next(10) * 0.2f;
                }
                attackShooting_4 = 0; // 重置计时器

                for (int i = 0; i < Main.rand.Next(1, 4); i++)
                {
                    Vector2 vector = new Vector2(Main.rand.Next(0, 0), Main.rand.Next(0, 2));
                    //生成新的弹幕
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                        target.Center, vector,
                        ModContent.ProjectileType<GastlyBadgeProj8>(), // 生成我们自己写的弹幕
                        Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.2f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                        target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
                }
                for (int i = 0; i < Main.rand.Next(1, 4); i++)
                {
                    Vector2 vector2 = new Vector2(Main.rand.Next(0, 0), Main.rand.Next(-2, 0));
                    //生成新的弹幕
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                       target.Center, vector2,
                       ModContent.ProjectileType<GastlyBadgeProj8>(), // 生成我们自己写的弹幕
                       Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.2f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                       target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
                }
            }
        }

      
        public override void AI()
        {
            //for (int i = 0; i < player.armor.Length; i++)
            //{
            //    if (player.armor[i].ModItem is GastlyBadge gastly)
            //    {
            //        gastlyBadge = gastly;
            //        break;
            //    }
            //}
            // 获取 PokeRadar 实例
            PokeRadar pokeRadar = null;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is PokeRadar radar)
                {
                    pokeRadar = radar;
                    break;
                }
            }

            if (pokeRadar != null)
            {
                for (int j = 0; j < PokeRadar.MaxItems; j++)
                {
                    if (pokeRadar.items[j] != null && !pokeRadar.items[j].IsAir)
                    if (pokeRadar.items[j].ModItem is GastlyBadge gastly)
                    {
                        gastlyBadge = gastly;
                        break;
                    }
                }
            }
                Projectile.damage = 0;
            if (player.HasBuff<BuffsGastlyBadge>()) // 如果玩家有召唤物BUFF
                Projectile.timeLeft = 2; // 维持住弹幕的时间

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
                int t = Projectile.FindTargetWithLineOfSight(1200); // 寻找1500像素范围内最近敌人号码（不隔墙）
                // 这个方法如果在没有敌怪时会返回-1，用来检测是否能找到敌人
                if (t >= 0)
                {
                    target = Main.npc[t]; // 定义这个NPC为目标
                    targetNPC = target; // 记录目标NPC
                }
            }

            if (target != null && target.active) // 如果目标不为空且存活在此处执行攻击性AI
            {
                if (Vector2.Distance(player.Center, target.Center) > 1600) // 如果找到的目标距离玩家太远了
                {
                    Vector2 p = Vector2.Lerp(Projectile.Center, player.Center, 0.1f);
                    Projectile.velocity = p - Projectile.Center; // 直接强制回归，不要继续攻击了
                    return; // 我们的AI就不需要继续往下走了
                }

                attackTime++;

                Vector2 mypos = player.Center + new Vector2(0, -100);
                float dis = Projectile.Distance(mypos); // 到玩家中心的距离

                if (dis > 1200) // 距离玩家过远时加速回归
                {
                    Vector2 p = Vector2.Lerp(Projectile.Center, player.Center, 0.1f);
                    Projectile.velocity = p - Projectile.Center;
                }
                else if (dis > 620) // 中程时，作惯性追击运动
                {
                    MoveToTarget(mypos, 10, 0.3f); // 对着目标做追击运动
                }
                else if (dis > 20)
                {
                    MoveToTarget(mypos, 8, 0.32f); // 对着目标做追击运动

                }
                else
                {
                    Standby();
                }
               
                isFindTarget = false;
                //遍历弹幕
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.owner == Projectile.owner && proj.type == ModContent.ProjectileType<GastlyBadgeProj4>())
                    {
                        isFindTarget = true;
                    }
                }
                if (gastlyBadge != null)
                {
                    if (lastLevel != gastlyBadge.level)
                    {
                        // 等级发生变化，重置所有攻击相关状态
                        attackType = 0;
                        attackTime = 0;
                        attackShooting_1 = 0;
                        attackShooting_2 = 0;
                        attackShooting_3 = 0;
                        attackShooting_4 = 0;
                        isFindTarget = false;
                        // 其他需要重置的状态也可以加上
                        lastLevel = gastlyBadge.level;
                    }
                }
                if (gastlyBadge != null && gastlyBadge.level >= 0 && gastlyBadge.level < 6)
                {
                    AttackShooting_2(target);// 暗影球
                    attackTime = 0;
                }else if(gastlyBadge != null && gastlyBadge.level >= 6 && gastlyBadge.level < 9)
                {
                    switch (attackType)
                    {
                        case 0:
                            AttackShooting_2(target);// 暗影球
                            if (attackTime == 180)
                            {
                                if (!isFindTarget)
                                    attackType = 1; // 攻击类型
                                else
                                    attackType = 0; // 攻击类型
                                attackTime = 0;
                            }
                            break;
                        case 1:
                            AttackShooting_1(target); // 黑色目光
                            if (attackTime == 180)
                            {
                                attackType = 0; // 攻击类型加1
                                attackTime = 0;
                            }
                            break;
                    }
                }else if(gastlyBadge != null && gastlyBadge.level >= 9 && gastlyBadge.level < 12)
                {
                    switch (attackType)
                    {
                        case 0:
                            AttackShooting_2(target);// 暗影球
                            if (attackTime == 180)
                            {
                                // 计算目标NPC的头部位置
                                Vector2 targetHeadPosition = target.Center - new Vector2(0, target.height / 2);
                                // 计算弹幕到目标头部的向量
                                Vector2 direction = targetHeadPosition - Projectile.Center;
                                //计算距离
                                float distance = (float)Math.Sqrt(direction.X * direction.X);
                                if (distance > 300 && !target.HasBuff(ModContent.BuffType<BuffsGastlyBadgeProj7>()))
                                    attackType = 2; // 攻击类型
                                //else if (target.HasBuff(ModContent.BuffType<BuffsGastlyBadgeProj7>()))
                                //    attackType = 3; // 攻击类型
                                else if (!isFindTarget)
                                    attackType = 1; // 攻击类型
                                else
                                    attackType = 0; // 攻击类型
                                attackTime = 0;
                            }
                            break;
                        case 1:
                            AttackShooting_1(target); // 黑色目光
                            if (attackTime == 180)
                            {
                                // 计算目标NPC的头部位置
                                Vector2 targetHeadPosition = target.Center - new Vector2(0, target.height / 2);
                                // 计算弹幕到目标头部的向量
                                Vector2 direction = targetHeadPosition - Projectile.Center;
                                //计算距离
                                float distance = (float)Math.Sqrt(direction.X * direction.X);
                                //if (target.HasBuff(ModContent.BuffType<BuffsGastlyBadgeProj7>()))
                                //    attackType = 3; // 攻击类型加1
                                //else
                                if (distance > 300 && target.HasBuff(ModContent.BuffType<BuffsGastlyBadgeProj7>()))
                                    attackType = 2; // 攻击类型加1
                                else
                                    attackType = 0; // 攻击类型加1
                                attackTime = 0;
                            }
                            break;
                        case 2:
                            AttackShooting_3(target); // 催眠术
                            if (attackTime == 180)
                            {
                                attackType = 0; // 攻击类型加1
                                attackTime = 0;
                            }
                            break;
                    }
                }
                else if(gastlyBadge != null && gastlyBadge.level >= 12)
                {
                    switch (attackType)
                    {
                        case 0:
                            AttackShooting_2(target);// 暗影球
                            if (attackTime == 180)
                            {
                                // 计算目标NPC的头部位置
                                Vector2 targetHeadPosition = target.Center - new Vector2(0, target.height / 2);
                                // 计算弹幕到目标头部的向量
                                Vector2 direction = targetHeadPosition - Projectile.Center;
                                //计算距离
                                float distance = (float)Math.Sqrt(direction.X * direction.X);
                                if (distance > 300 && !target.HasBuff(ModContent.BuffType<BuffsGastlyBadgeProj7>()))
                                    attackType = 2; // 攻击类型
                                else if (target.HasBuff(ModContent.BuffType<BuffsGastlyBadgeProj7>()))
                                    attackType = 3; // 攻击类型
                                else if (!isFindTarget)
                                    attackType = 1; // 攻击类型
                                else
                                    attackType = 0; // 攻击类型
                                attackTime = 0;
                            }
                            break;
                        case 1:
                            AttackShooting_1(target); // 黑色目光
                            if (attackTime == 180)
                            {
                                // 计算目标NPC的头部位置
                                Vector2 targetHeadPosition = target.Center - new Vector2(0, target.height / 2);
                                // 计算弹幕到目标头部的向量
                                Vector2 direction = targetHeadPosition - Projectile.Center;
                                //计算距离
                                float distance = (float)Math.Sqrt(direction.X * direction.X);
                                if (target.HasBuff(ModContent.BuffType<BuffsGastlyBadgeProj7>()))
                                    attackType = 3; // 攻击类型加1
                                else if (distance > 300 && target.HasBuff(ModContent.BuffType<BuffsGastlyBadgeProj7>()))
                                    attackType = 2; // 攻击类型加1
                                else
                                    attackType = 0; // 攻击类型加1
                                attackTime = 0;
                            }
                            break;
                        case 2:
                            AttackShooting_3(target); // 催眠术
                            if (attackTime == 180)
                            {
                                attackType = 3; // 攻击类型加1
                                attackTime = 0;
                            }
                            break;
                        case 3:
                            AttackShooting_4(target); // 食梦
                            if (attackTime == 180)
                            {
                                attackType = 0; // 攻击类型加1
                                attackTime = 0;
                            }
                            break;
                    }
                }
            }
            else // 否则说明没目标了，执行回归待机运动
            {
                Vector2 myposOffset = new Vector2(0, -80);
                Rectangle myposRectangle;
                // 定义矩形的宽度和高度
                float width = 40;
                float height = 50;

                if (Main.keyState.IsKeyDown(Keys.D)) // 如果按下d键
                {
                    myposOffset = new Vector2(40, -80);
                }
                else if (Main.keyState.IsKeyDown(Keys.A)) // 如果按下a键
                {
                    myposOffset = new Vector2(-40, -80);
                }

                // 计算矩形的左上角和右下角
                Vector2 topLeft = player.Center + myposOffset - new Vector2(width / 2, height / 2);

                // 创建矩形区域
                myposRectangle = new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)width, (int)height);

                // 将 Vector2 转换为 Point
                Point projectilePoint = new Point((int)Projectile.Center.X, (int)Projectile.Center.Y);

                //判断子弹是否在这个矩形区域内
                if (!myposRectangle.Contains(projectilePoint))
                {
                    float dis = Vector2.Distance(Projectile.Center, player.Center + myposOffset); // 到玩家中心的距离
                    if (dis > 1200) // 距离玩家过远时加速回归
                    {
                        Vector2 p = Vector2.Lerp(Projectile.Center, player.Center + myposOffset, 0.1f);
                        Projectile.velocity = p - Projectile.Center;
                    }
                    else if (dis > 620) // 中程时，作惯性追击运动
                    {
                        MoveToTarget(player.Center + myposOffset, 8, 0.3f); // 对着目标做追击运动
                    }
                    else if (dis > 40)
                    {
                        MoveToTarget(player.Center + myposOffset, 6, 0.3f); // 对着目标做追击运动
                    }
                    else
                    {
                        Standby();
                    }
                }
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] > 2)
            {
                // 更新帧计数器
                Projectile.ai[0]++;
                Projectile.ai[1] = 0;
            }
        }
        void Standby()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.velocity.Length() <= 5)
                Projectile.velocity *= 0.8f; // 减速
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
                if (Projectile.velocity.X <= -6 || (Main.keyState.IsKeyDown(Keys.A) && player.velocity.X <= -1 && Projectile.Center.X + 100 > player.Center.X))
                {
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 4) + 4; // 使用第5到第8帧（向左移动）
                }
                else if (Projectile.velocity.X >= 6 || (Main.keyState.IsKeyDown(Keys.D) && player.velocity.X >= 1 && Projectile.Center.X - 100 < player.Center.X))
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

            if (Projectile.velocity.Length() > 6 || Main.keyState.IsKeyDown(Keys.D) || Main.keyState.IsKeyDown(Keys.A))
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
                {
                    float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                    Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, oldcenter, rectangle, MyColor * factor,
                        Projectile.oldRot[i],
                        new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                        new Vector2(0.8f),
                        SpriteEffects.None, 0);
                }

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                rectangle,
                lightColor,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                new Vector2(0.8f),
                SpriteEffects.None,
                0);

            return false;
        }

    }
}

