using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
using Terraria.WorldBuilding;

namespace Pokemon.Projectiles.CharmanderBadgeProj
{
    public class CharmanderBadgeProj1 : ModProjectile
    {
        Player player => Main.player[Projectile.owner];
        NPC targetNPC = null; // 目标NPC
        
        private const float terminalVelocity = 10;// 终端速度
        private float gotoX;// 目标X坐标
        private const float spacing = 70;// 弹幕间距
        private const float maxSpeedX = 8;// 最大水平速度
        private bool returnToPlayer;// 是否返回玩家
        private const float gravityAcceleration = .2f;// 重力加速度
        private int standtime = 0;// 站立时间
        private bool isstand = false;// 是否站立
        private int standtime_1 = 0;// 站立时间1
        private bool attackstop = false;// 是否停止攻击

        private int attackType = 0; // 攻击类型
        private int attackTime = 0; // 攻击间隔
        private int attackShooting_1 = 0; // 火花
        private int attackShooting_2 = 0; // 合金爪
        private int attackShooting_3 = 0; // 抓
        private bool isattackShooting_3 = false; // 是否正在抓
        private bool isattackShooting_3xuan = false; // 抓时的大拖尾
        private int attackShooting_4 = 0; // 剑舞
        CharmanderBadge charmanderBadge = null;
        private int lastLevel = -1;

        public override void SetDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            Projectile.width = 15; // 弹幕宽度
            Projectile.height = 23; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = true; // 允许与瓷砖碰撞
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
            Main.projFrames[Projectile.type] = 21;//设置动画帧数
            base.SetStaticDefaults();
        }

        public override bool? CanCutTiles()
        {
            return false;//我们不想召唤兽会割草
        }

        //火花
        void AttackShooting_1(NPC target)
        {
            attackShooting_1++; // 作为计时器
            // 如果投射物需要返回玩家
            if (returnToPlayer)
            {
                // 计算返回玩家的速度向量
                Projectile.velocity = (player.Top - Projectile.Center) * .06f;

                // 如果距离玩家顶部小于200，则停止返回
                if ((player.Top - Projectile.Center).Length() < 100 && Projectile.position.Y - player.position.Y < 20)
                {
                    returnToPlayer = false;
                }

                // 不允许碰撞方块
                Projectile.tileCollide = false;

                Projectile.rotation += Projectile.velocity.X * 0.3f + Projectile.velocity.Y * 0.25f; // 旋转弹幕
            }
            else
            {
                Projectile.rotation = Projectile.velocity.X * 0.05f; // 旋转弹幕
                                                                     // 如果投射物与玩家的距离超过1500，则需要返回玩家
                if ((player.Center - Projectile.Center).Length() > 1200 || Projectile.position.Y - player.position.Y > 800)
                {
                    returnToPlayer = true;
                }

                // 允许碰撞方块
                Projectile.tileCollide = true;

                // 增加重力，确保投射物的垂直速度不超过终端速度
                if (Projectile.velocity.Y < terminalVelocity)
                {
                    Projectile.velocity.Y += gravityAcceleration * 4f;
                }

                if (target.position.X > player.position.X && target.position.X - player.position.X < 600)
                {
                    attackstop = false;
                    // 计算投射物应该移动到的X位置
                    gotoX = target.Center.X + target.direction * (target.width / 2 + (spacing + 120)) - new Vector2(-40, 0).X;
                }
                else if (target.position.X < player.position.X && player.position.X - target.position.X < 600)
                {
                    attackstop = false;
                    gotoX = target.Center.X + target.direction * (target.width / 2 + (spacing + 120)) - new Vector2(40, 0).X;
                }
                else if (player.velocity.X != 0)
                {
                    attackstop = false;
                    gotoX = player.Center.X + -player.direction * (target.width / 2 + (spacing + 80));
                }
                else if (player.velocity.X == 0)
                {
                    attackstop = true;
                    gotoX = player.Center.X + -player.direction * (target.width / 2 + (spacing + 80));
                    Projectile.frame = 13;
                }

                // 计算水平速度向量
                Projectile.velocity.X = (gotoX - Projectile.Center.X) * .2f;

                // 如果水平速度超过最大速度，则限制其速度
                if (Math.Abs(Projectile.velocity.X) > maxSpeedX)
                {
                    Projectile.velocity.X = Projectile.velocity.X > 0 ? maxSpeedX : -maxSpeedX;
                }
            }
            //if (attackShooting_1 < 50 && attackShooting_1 > 40)
            if (attackShooting_1 == (charmanderBadge.level >= 0 && charmanderBadge.level < 6 ? 240 : (charmanderBadge.level >= 6 && charmanderBadge.level < 9 ? 180 : 120))) // 攻击间隔为120帧
            {
                attackShooting_1 = 0; // 重置计时器

                //生成新的弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                    Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16f,
                    ModContent.ProjectileType<CharmanderBadgeProj3>(), // 生成我们自己写的弹幕
                    Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.8f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                    target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
            }
        }
       
        //合金爪
        void AttackShooting_2(NPC target)
        {
            attackShooting_2++; // 计时器
            //********************************
            // 如果投射物需要返回玩家
            if (returnToPlayer)
            {
                // 计算返回玩家的速度向量
                Projectile.velocity = (player.Top - Projectile.Center) * .06f;

                // 如果距离玩家顶部小于200，则停止返回
                if ((player.Top - Projectile.Center).Length() < 100 && Projectile.position.Y - player.position.Y < 20)
                {
                    returnToPlayer = false;
                }

                // 不允许碰撞方块
                Projectile.tileCollide = false;

                Projectile.rotation += Projectile.velocity.X * 0.3f + Projectile.velocity.Y * 0.25f; // 旋转弹幕
            }
            else
            {
                Projectile.rotation = Projectile.velocity.X * 0.05f; // 旋转弹幕
                                                                     // 如果投射物与玩家的距离超过1500，则需要返回玩家
                if ((player.Center - Projectile.Center).Length() > 1200 || Projectile.position.Y - player.position.Y > 800)
                {
                    returnToPlayer = true;
                }

                // 允许碰撞方块
                Projectile.tileCollide = true;

                // 增加重力，确保投射物的垂直速度不超过终端速度
                if (Projectile.velocity.Y < terminalVelocity)
                {
                    Projectile.velocity.Y += gravityAcceleration * 4f;
                }

                if (target.position.X > player.position.X && target.position.X - player.position.X < 1200 && !attackstop)
                {
                    // 计算投射物应该移动到的X位置
                    gotoX = target.Center.X + target.direction * (target.width / 2 + (spacing + 120)) - new Vector2(-40, 0).X;
                }
                else if (target.position.X < player.position.X && player.position.X - target.position.X < 600 && !attackstop)
                {
                    gotoX = target.Center.X + target.direction * (target.width / 2 + (spacing + 120)) - new Vector2(40, 0).X;
                }

                // 计算水平速度向量
                Projectile.velocity.X = (gotoX - Projectile.Center.X) * .2f;

                // 如果水平速度超过最大速度，则限制其速度
                if (Math.Abs(Projectile.velocity.X) > maxSpeedX)
                {
                    Projectile.velocity.X = Projectile.velocity.X > 0 ? maxSpeedX : -maxSpeedX;
                }
            }
            if (attackShooting_2 < 120 && attackShooting_2 > 105)
            {
                //Projectile.velocity.Y = -4 + Main.rand.Next(-2, 2);
                gotoX = target.Center.X + -target.direction * (target.width / 2 + (spacing - 80));
                attackstop = true;
            }else
            {
                attackstop = false;
            }
            if (attackShooting_2 == 120) // 攻击间隔为120帧
            {
                attackShooting_2 = 0; // 重置计时器

                //生成新的弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                    Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 0f,
                    ModContent.ProjectileType<CharmanderBadgeProj4>(), // 生成我们自己写的弹幕
                    Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 1f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                    target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
            }
        }

        //抓
        void AttackShooting_3(NPC target)
        {
            attackShooting_3++;
            if(attackShooting_3 > 10)
            {
                // 不允许碰撞方块
                Projectile.tileCollide = false;
                Vector2 directionToTarget = target.Center - Projectile.Center;
                directionToTarget.Normalize(); // 归一化方向
                                               // 计算目标NPC的头部位置
                Vector2 targetHeadPosition = target.Center;
                // 计算弹幕到目标头部的向量
                Vector2 direction = targetHeadPosition - Projectile.Center;
                //计算距离
                float distance1 = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
                //Projectile.velocity = Vector2.Normalize(target.Center - Projectile.Center) * 26f; // 跟踪目标

                if (distance1 > 60)
                {

                    isattackShooting_3 = true; // 标记为正在抓
                                               // 设置弹幕新的速度和穿透能力
                    Projectile.velocity = directionToTarget * 16f; // 设置向目标的速度
                }
                else if (distance1 <= 10)
                {
                    isattackShooting_3xuan = true;
                    //isattackShooting_3 = true; // 标记为正在抓
                    // 生成一个随机角度
                    float randomAngle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    // 计算新的位置，以目标NPC中心为圆心，150像素为半径
                    Vector2 newPosition = target.Center + new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle)) * 400f;
                    // 计算新的方向向量
                    Vector2 newDirection = newPosition - Projectile.Center;
                    // 归一化方向向量
                    newDirection.Normalize();
                    // 设置新的速度
                    Projectile.velocity = newDirection * 12f;
                }

                Projectile.rotation += Projectile.velocity.Length() * 2f; // 旋转弹幕

                if (target.active && !target.friendly && !target.dontTakeDamage && target.damage > 0 && target.lifeMax > 5)
                {
                    //计算距离
                    float distance = Vector2.Distance(target.Center, Projectile.Center);
                    if (distance < (target.width / 2 + target.height / 2) && isattackShooting_3)
                    {
                        attackShooting_3 = 0; // 重置计时器
                        isattackShooting_3xuan = false;
                        //生成新的弹幕
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                            Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 0f,
                            ModContent.ProjectileType<CharmanderBadgeProj5>(), // 生成我们自己写的弹幕
                            Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.6f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                            target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
                        isattackShooting_3 = false; // 标记为抓完
                    }
                }
            }
        }

        //剑舞
        void AttackShooting_4(NPC target)
        {
            attackShooting_4++; // 计时器
            // 如果投射物需要返回玩家
            if (returnToPlayer)
            {
                // 计算返回玩家的速度向量
                Projectile.velocity = (player.Top - Projectile.Center) * .06f;

                // 如果距离玩家顶部小于200，则停止返回
                if ((player.Top - Projectile.Center).Length() < 100 && Projectile.position.Y - player.position.Y < 20)
                {
                    returnToPlayer = false;
                }

                // 不允许碰撞方块
                Projectile.tileCollide = false;

                Projectile.rotation += Projectile.velocity.X * 0.3f + Projectile.velocity.Y * 0.25f; // 旋转弹幕
            }
            else
            {
                Projectile.rotation = Projectile.velocity.X * 0.05f; // 旋转弹幕
                                                                     // 如果投射物与玩家的距离超过1500，则需要返回玩家
                if ((player.Center - Projectile.Center).Length() > 1200 || Projectile.position.Y - player.position.Y > 800)
                {
                    returnToPlayer = true;
                }

                // 允许碰撞方块
                Projectile.tileCollide = true;

                // 增加重力，确保投射物的垂直速度不超过终端速度
                if (Projectile.velocity.Y < terminalVelocity)
                {
                    Projectile.velocity.Y += gravityAcceleration * 4f;
                }

                if (target.position.X > player.position.X && target.position.X - player.position.X < 1000)
                {
                    attackstop = false;
                    // 计算投射物应该移动到的X位置
                    gotoX = target.Center.X + target.direction * (target.width / 2 + (spacing + 120)) - new Vector2(-40, 0).X;
                }
                else if (target.position.X < player.position.X && player.position.X - target.position.X < 600)
                {
                    attackstop = false;
                    gotoX = target.Center.X + target.direction * (target.width / 2 + (spacing + 120)) - new Vector2(40, 0).X;
                }
                else if (player.velocity.X != 0)
                {
                    attackstop = false;
                    gotoX = player.Center.X + -player.direction * (target.width / 2 + (spacing + 80));
                }
                else if (player.velocity.X == 0)
                {
                    attackstop = true;
                    gotoX = player.Center.X + -player.direction * (target.width / 2 + (spacing + 80));
                    Projectile.frame = 13;
                }

                // 计算水平速度向量
                Projectile.velocity.X = (gotoX - Projectile.Center.X) * .2f;

                // 如果水平速度超过最大速度，则限制其速度
                if (Math.Abs(Projectile.velocity.X) > maxSpeedX)
                {
                    Projectile.velocity.X = Projectile.velocity.X > 0 ? maxSpeedX : -maxSpeedX;
                }
            }
            if (attackShooting_4 == 120)
            {
                if(!player.HasBuff(ModContent.BuffType<BuffsCharmanderBadgeProj6>()))
                {
                    player.AddBuff(ModContent.BuffType<BuffsCharmanderBadgeProj6>(), 240); // 给玩家添加BUFF
                }
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center + new Vector2(-Projectile.width / 2, 0),
                    Projectile.width, Projectile.height, ModContent.DustType<CharmanderDust>(),
                    Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[dust].velocity = new Vector2(0, 0);
                    Main.dust[dust].scale = 0.2f; // 设置大小
                    Main.dust[dust].velocity *= 0.1f; // 设置速度
                    Main.dust[dust].fadeIn = 4f + (float)Main.rand.Next(10) * 0.2f;
                }
                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(player.Center + new Vector2(-player.width / 2, -player.height / 2),
                    player.width, player.height, ModContent.DustType<CharmanderDust>(),
                    player.velocity.X * 0.5f, player.velocity.Y * 0.5f);
                    Main.dust[dust].velocity = new Vector2(0, 0);
                    Main.dust[dust].scale = 0.4f; // 设置大小
                    Main.dust[dust].velocity *= 0.1f; // 设置速度
                    Main.dust[dust].fadeIn = 4f + (float)Main.rand.Next(10) * 0.2f;
                }
                attackShooting_4 = 0; // 重置计时器
                for (int i = 0; i < 2 + Main.rand.Next(1, 3); i++)
                //生成新的弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                    Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 0f,
                    ModContent.ProjectileType<CharmanderBadgeProj6>(), // 生成我们自己写的弹幕
                    Projectile.originalDamage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.6f), Projectile.knockBack, Projectile.owner, // 为接下来生成的弹幕提供主人
                    target.whoAmI); // 传入敌人的号码，为接下来生成的弹幕提供目标
            }
        }
       
        public override void AI()
        {
            //如果玩家有召唤物BUFF，则维持住弹幕的时间
            if (player.HasBuff<BuffsCharmanderBadge>())
            Projectile.timeLeft = 2;

            Projectile.damage = 0;

            // 定义目标NPC
            NPC target = null;

            // 如果目标是空的或者失活的，那么重新寻找敌人
            if (target == null || !target.active || !target.CanBeChasedBy())
            {
                isattackShooting_3xuan = false; // 标记为抓完
                // 寻找1500像素范围内最近敌人（最多隔两格墙）
                target = CharmanderBadgeProj1.FindTargetWithinRange(player, 1200f);
                targetNPC = target; // 记录目标NPC
                
            }
            if (target != null && target.active) // 如果目标不为空且存活在此处执行攻击性AI
            {
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
                        if (pokeRadar.items[j].ModItem is CharmanderBadge charmander)
                        {
                            charmanderBadge = charmander;
                            break;
                        }
                    }
                }
                isattackShooting_3xuan = true; // 标记为抓完
                if (CharmanderBadgeProj3.isjump)
                {
                    Projectile.velocity.Y = -8 + Main.rand.Next(-2, 2);
                    CharmanderBadgeProj3.isjump = false;
                }
               
                attackTime++;
                if (charmanderBadge != null)
                {
                    if (lastLevel != charmanderBadge.level)
                    {
                        // 等级发生变化，重置所有攻击相关状态
                        attackType = 0;
                        attackTime = 0;
                        attackShooting_1 = 0;
                        attackShooting_2 = 0;
                        attackShooting_3 = 0;
                        isattackShooting_3 = false;
                        isattackShooting_3xuan = false;
                        attackShooting_4 = 0;
                        standtime = 0;
                        isstand = false;
                        standtime_1 = 0;
                        attackstop = false;
                        // 其他需要重置的状态也可以加上
                        lastLevel = charmanderBadge.level;
                    }
                }
                if (charmanderBadge != null && charmanderBadge.level >= 0 && charmanderBadge.level < 6)
                {
                    AttackShooting_1(target); // 火花攻击
                    attackTime = 0;
                }
                else if(charmanderBadge != null && charmanderBadge.level >= 6 && charmanderBadge.level < 9)
                {
                    switch (attackType)
                    {
                        case 0:
                            AttackShooting_1(target); // 火花攻击
                            if (attackTime == 180)
                            {
                                attackType = 1;
                                attackTime = 0;
                            }
                            break;
                        case 1:
                            AttackShooting_2(target); // 合金爪攻击
                            if (attackTime == 120)
                            {
                                attackType = 0;
                                attackTime = 0;
                            }
                            break;
                    }
                }
                else if(charmanderBadge != null && charmanderBadge.level >= 9 && charmanderBadge.level < 12)
                {
                    switch (attackType)
                    {
                        case 0:
                            AttackShooting_1(target); // 火花攻击
                            if (attackTime == 120)
                            {
                                attackType = 1;
                                attackTime = 0;
                            }
                            break;
                        case 1:
                            AttackShooting_2(target); // 合金爪攻击
                            if (attackTime == 120)
                            {
                                attackType = 2;
                                attackTime = 0;
                            }
                            break;
                        case 2:
                            AttackShooting_3(target); // 抓攻击
                            if (attackTime == 120)
                            {
                                attackType = 0;
                                attackTime = 0;
                            }
                            break;
                    }
                }
                else if(charmanderBadge != null && charmanderBadge.level >= 12)
                {
                    switch (attackType)
                    {
                        case 0:
                            AttackShooting_1(target); // 火花攻击
                            if (attackTime == 120)
                            {
                                attackType = 1;
                                attackTime = 0;
                            }
                            break;
                        case 1:
                            AttackShooting_2(target); // 合金爪攻击
                            if (attackTime == 120)
                            {
                                attackType = 2;
                                attackTime = 0;
                            }
                            break;
                        case 2:
                            AttackShooting_3(target); // 抓攻击
                            if (attackTime == 120)
                            {
                                attackType = 3;
                                attackTime = 0;
                            }
                            break;
                        case 3:
                            AttackShooting_4(target); // 剑舞攻击
                            if (attackTime == 120)
                            {
                                attackType = 0;
                                attackTime = 0;
                            }
                            break;
                    }
                }
            }
            else
            {
                // 如果投射物需要返回玩家
                if (returnToPlayer)
                {
                    
                    // 计算返回玩家的速度向量
                    Projectile.velocity = (player.Top - Projectile.Center) * .06f;

                    // 如果距离玩家顶部小于200，则停止返回
                    if ((player.Top - Projectile.Center).Length() < 100 && Projectile.position.Y - player.position.Y < 20)
                    {
                        returnToPlayer = false;
                        //isattackShooting_3xuan = false; // 标记为抓完
                    }

                    // 不允许碰撞方块
                    Projectile.tileCollide = false;

                    Projectile.rotation += Projectile.velocity.X * 0.3f + Projectile.velocity.Y * 0.25f; // 旋转弹幕
                }
                else
                {
                    Projectile.rotation = Projectile.velocity.X * 0.05f; // 旋转弹幕
                    // 如果投射物与玩家的距离超过1500，则需要返回玩家
                    if ((player.Center - Projectile.Center).Length() > 600 || Projectile.position.Y - player.position.Y > 340)
                    {
                        returnToPlayer = true;
                    }
                    
                    // 允许碰撞方块
                    Projectile.tileCollide = true;

                    // 增加重力，确保投射物的垂直速度不超过终端速度
                    if (Projectile.velocity.Y < terminalVelocity)
                    {
                        Projectile.velocity.Y += gravityAcceleration * 4f;
                    }
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
                        int equippedIndex = -1;
                        for (int j = 0; j < PokeRadar.MaxItems; j++)
                        {
                            if (pokeRadar.items[j] != null && !pokeRadar.items[j].IsAir)
                            if (pokeRadar.items[j].type == ModContent.ItemType<CharmanderBadge>())
                            {
                                equippedIndex = j;
                                break;
                            }
                        }

                        if (equippedIndex != -1)
                        {
                            int count = 0;
                            bool hasItemInPreviousSlot = false;
                            for (int i = 0; i < equippedIndex; i++)
                            {
                                if (pokeRadar.items[i] != null && !pokeRadar.items[i].IsAir)
                                {
                                    if (pokeRadar.items[i].type == ModContent.ItemType<TaillowBadge>() ||
                                        pokeRadar.items[i].type == ModContent.ItemType<BulbasaurBadge>() ||
                                        pokeRadar.items[i].type == ModContent.ItemType<SquirtleBadge>() ||
                                        pokeRadar.items[i].type == ModContent.ItemType<SpoinkBadge>() ||
                                        pokeRadar.items[i].type == ModContent.ItemType<VoltorbBadge>() ||
                                        pokeRadar.items[i].type == ModContent.ItemType<MunchlaxBadge>() ||
                                        pokeRadar.items[i].type == ModContent.ItemType<FomantisBadge>() ||
                                        pokeRadar.items[i].type == ModContent.ItemType<TrapinchBadge>() ||
                                        pokeRadar.items[i].type == ModContent.ItemType<PikachuBadge>())
                                        count++;
                                    hasItemInPreviousSlot = true;
                                    if (pokeRadar.items[i].type == ModContent.ItemType<CharmanderBadge>())
                                        break;
                                }
                            }

                            if (hasItemInPreviousSlot)
                            {
                                if (player.direction == 1 && player.velocity.X >= 0)
                                    gotoX = player.Center.X + -player.direction * (player.width / 2 + spacing) - new Vector2(-40 + count * 25, 0).X;
                                else if (player.direction == -1 && player.velocity.X <= 0)
                                    gotoX = player.Center.X + -player.direction * (player.width / 2 + spacing) - new Vector2(40 - count * 25, 0).X;
                            }
                            else
                            {
                                if (player.direction == 1 && player.velocity.X >= 0)
                                    gotoX = player.Center.X + -player.direction * (player.width / 2 + spacing) - new Vector2(-40, 0).X;
                                else if (player.direction == -1 && player.velocity.X <= 0)
                                    gotoX = player.Center.X + -player.direction * (player.width / 2 + spacing) - new Vector2(40, 0).X;
                            }
                        }
                        else
                        {
                            if (player.direction == 1 && player.velocity.X >= 0)
                                gotoX = player.Center.X + -player.direction * (player.width / 2 + spacing) - new Vector2(-40, 0).X;
                            else if (player.direction == -1 && player.velocity.X <= 0)
                                gotoX = player.Center.X + -player.direction * (player.width / 2 + spacing) - new Vector2(40, 0).X;
                        }
                    }
                    else
                    {
                        if (player.direction == 1 && player.velocity.X >= 0)
                            gotoX = player.Center.X + -player.direction * (player.width / 2 + spacing) - new Vector2(-40, 0).X;
                        else if (player.direction == -1 && player.velocity.X <= 0)
                            gotoX = player.Center.X + -player.direction * (player.width / 2 + spacing) - new Vector2(40, 0).X;
                    }
                    // 计算水平速度向量
                    Projectile.velocity.X = (gotoX - Projectile.Center.X) * .2f;

                    // 如果水平速度超过最大速度，则限制其速度
                    if (Math.Abs(Projectile.velocity.X) > maxSpeedX)
                    {
                        Projectile.velocity.X = Projectile.velocity.X > 0 ? maxSpeedX : -maxSpeedX;
                    }
                }
            }
            if (Projectile.velocity.X < 1 && Projectile.velocity.X > -1)
            {
                standtime++;
                if (standtime > 10)
                {
                    isstand = true;
                    standtime_1++;
                    if (standtime_1 > 210)
                    {
                        standtime_1 = 0;
                    }
                }
            }else
            {
                standtime = 0;
                isstand = false;
            }

            if(Projectile.velocity.Y >= 0.85)//空中
            {
                if (player.velocity.X != 0)
                {
                    if (Projectile.velocity.X > 0)
                        Projectile.rotation += Projectile.velocity.X * 0.3f - Projectile.velocity.Y * 0.25f; // 旋转弹幕
                    else
                        Projectile.rotation += Projectile.velocity.X * 0.3f + Projectile.velocity.Y * 0.25f; // 旋转弹幕

                }
            }
            else
            {
                Projectile.ai[1]++;
            }
            
            if (Projectile.ai[1] > 0 && !isstand)
            {
                // 更新帧计数器
                Projectile.ai[0]++;
                Projectile.ai[1] = 0;

            }
            else if (Projectile.ai[1] > 0 && isstand)
            {
                Projectile.ai[0] += 1;
                Projectile.ai[1] = 0;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) // 处理墙壁的碰撞
        {
            // 计算投射物中心点的方块坐标
            Point origin = new Vector2(Projectile.Center.X + (Projectile.width / 2) * Projectile.spriteDirection, Projectile.Bottom.Y).ToTileCoordinates();
            Point point;

            // 检查是否碰撞到固体方块
            if ((oldVelocity.X != Projectile.velocity.X)
                && WorldUtils.Find(origin, Searches.Chain(new Searches.Down(1), new GenCondition[] { new Conditions.IsSolid() }), out point))
            {
                // 获取前方方块的高度信息
                int blockHeight = GetHeightOfBlock(point);

                Projectile.velocity.Y = -blockHeight * 2;

                // 确保跳跃速度不达到一个过小的值
                if (Projectile.velocity.Y > -12)
                {
                    Projectile.velocity.Y = -12;
                }

                // 确保跳跃速度不达到一个过大的值
                if (Projectile.velocity.Y < -24)
                {
                    Projectile.velocity.Y = -24;
                }
            }
            // 不取消投射物
            return false;
        }
        private int GetHeightOfBlock(Point blockPosition)
        {
    //        // 获取方块的类型
    //        int tileType = Main.tile[blockPosition.X, blockPosition.Y].TileType;

    //        // 定义一个高度映射表
    //        Dictionary<int, int> tileHeightMap = new Dictionary<int, int>
    //{
    //    //{ TileID.Stone, 16 },
    //    //{ TileID.Sand, 4 },
    //    //{ TileID.Dirt, 8 },
    //    //{ TileID.Lead, 16 }
    //    // 其他方块类型和对应的高度
    //};

    //        // 根据方块类型返回高度信息
    //        if (tileHeightMap.TryGetValue(tileType, out int height))
    //        {
    //            return height;
    //        }

            // 如果方块类型不在映射表中，返回默认高度
            return (int)(Projectile.position.Y - player.position.Y) / 16; // 默认高度
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) // 这里的fallThrough参数用来控制是否可以穿透墙壁
        {
            // 根据玩家与投射物的垂直距离设置是否可以穿透墙壁
            fallThrough = Main.player[Projectile.owner].Center.Y - Projectile.Center.Y > 64;

            // 调用基类的 TileCollideStyle 方法
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;

            if (targetNPC != null && targetNPC.active)
            {
                if (targetNPC.position.X < Projectile.position.X)
                {
                    if (returnToPlayer)
                        Projectile.frame = 12;
                    else
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 4) + 13; // 使用第5到第8帧（向左移动）
                }
                else
                {
                    if (returnToPlayer)
                        Projectile.frame = 12;
                    else
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 4) + 17; // 使用第9到第12帧（向右移动）
                }
            }
            else
            {
                // 根据速度方向选择帧数
                if (Projectile.velocity.X < -0.4)
                {
                    if (returnToPlayer)
                        Projectile.frame = 12;
                    else
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 4) + 13; // 使用第5到第8帧（向左移动）
                }
                else if (Projectile.velocity.X > 0.4)
                {
                    if (returnToPlayer)
                        Projectile.frame = 12;
                    else
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 4) + 17; // 使用第9到第12帧（向右移动）
                }
                else if (Projectile.velocity.X < 0.4 && Projectile.velocity.X > -0.4 && 
                    Projectile.velocity.Y == 0 && isstand && standtime_1 < 180)
                {
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 6); // 使用第1到第4帧（不动）
                }
                else if (Projectile.velocity.X < 0.4 && Projectile.velocity.X > -0.4 && 
                    Projectile.velocity.Y == 0 && isstand && standtime_1 >= 180)
                {
                    Projectile.frame = (int)(Projectile.ai[0] / 5 % 12); // 使用第7到第10帧（站立）
                }
            }

            Rectangle rectangle = new Rectangle(
                0,
                texture.Height / Main.projFrames[Type] * Projectile.frame,
                texture.Width,
                texture.Height / Main.projFrames[Type]
            );

            Color MyColor = new Color(160, 100, 160) * 0.5f;
            MyColor.A = 0;

            if (Projectile.velocity.Length() > 9 || returnToPlayer)
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
                {
                    float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                    Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, oldcenter, rectangle, MyColor * factor,
                        Projectile.oldRot[i],
                        new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                        new Vector2(0.4f),
                        SpriteEffects.None, 0);
                }
            if (isattackShooting_3xuan && attackType == 2)
            {
                for (int i = 0; i < (ProjectileID.Sets.TrailCacheLength[Type]); i++)
                {
                    float factor = 1 - (float)i / (ProjectileID.Sets.TrailCacheLength[Type]);
                    Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, oldcenter, rectangle, MyColor * factor,
                        Projectile.oldRot[i],
                        new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                        new Vector2(0.8f),
                        SpriteEffects.None, 0);
                }
            }
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                rectangle,
                lightColor,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                new Vector2(0.4f),
                SpriteEffects.None,
                0);

            return false;
        }

        [Obsolete]
        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
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
                    projectile.type == ModContent.ProjectileType<CharmanderBadgeProj1>())
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
                            if (wallCount > 10)
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
