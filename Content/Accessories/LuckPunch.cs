using log4net.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pokemon.Content.Equipment;
using Pokemon.Content.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace Pokemon.Content.Accessories
{
    //吉利拳
    public class LuckPunch : ModItem
    {
        public int attackType = 0;
        public int comboExpireTimer = 0;
        public static bool isShow = true;

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true; // 可装备

            // 作为武器的属性
            Item.damage = 20; // 伤害
            Item.DamageType = DamageClass.Melee; // 伤害类型
            Item.useTime = 20; // 使用时间
            Item.useAnimation = 20; // 使用动画
            Item.useStyle = ItemUseStyleID.Shoot; // 使用方式（挥动/射击等）
            Item.knockBack = 4f; // 击退
            Item.UseSound = SoundID.Item1; // 使用音效
            Item.autoReuse = true; // 自动连发
            Item.shoot = ModContent.ProjectileType<LuckPunchProj>(); // 弹幕类型
            Item.shootSpeed = 10f; // 弹幕速度
            Item.noMelee = false; // 是否不进行近战碰撞
            Item.noUseGraphic = true; // 不显示使用动画
            Item.consumable = false; // 不消耗
            Item.useTurn = false;
        }
        // 合成材料
        public override void AddRecipes()
        {
            // 创建一个新的配方组
            RecipeGroup group = new RecipeGroup(() => "铁锭或铅锭",
                ItemID.IronBar,
                ItemID.LeadBar);
            // 注册配方组
            RecipeGroup.RegisterGroup("Pokemon:IconOrLeadGroup", group);

            CreateRecipe()
               .AddRecipeGroup("Pokemon:IconOrLeadGroup", 16) // 使用配方组
               .AddIngredient(ItemID.Silk, 6) // 丝绸
               .AddIngredient(ItemID.BlackThread, 3) // 黑线
               .AddIngredient(ItemID.Chain, 1) // 链条
               .AddIngredient(ItemID.Coral, 10) // 珊瑚
               .AddIngredient(ItemID.TatteredCloth, 2) // 破布
               .AddTile(TileID.Loom) // 织布机
               .AddTile(TileID.Anvils) // 铁砧
               .Register();
        }
        public override void UpdateInventory(Player player)
        {
            // 只在背包界面，鼠标悬停在本物品并左键点击，且不是当前手持物品时切换
            if (Main.playerInventory&& 
                Main.mouseLeft && 
                Main.mouseLeftRelease&& Main.HoverItem.type == Item.type&& 
                Main.netMode != NetmodeID.Server)   isShow = !isShow;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.04f;
            player.GetCritChance(DamageClass.Generic) += 8;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Language.ActiveCulture.Name == "zh-Hans")
            {
                tooltips.Add(new TooltipLine(Mod, "","[c/fc9768:+4%伤害]\n" + "[c/fc9768:+8%暴击率]"));
                var openTooltip = (new TooltipLine(Mod, "", isShow ?
                "左键点击" + "[c/808A7E:关闭]" + "手持" : "左键点击" + "[c/fc9768:开启]" + "手持"));
                tooltips.Add(openTooltip);
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "", "Equip this item to\n" + "[c/fc9768:+4% damage]\n" + "[c/fc9768:+8% crit chance]"));
                var openTooltip = (new TooltipLine(Mod, "", isShow ?
                    "Left-click to " + "[c/808A7E:close]" + " your hold effect" : "Left-click to " + "[c/fc9768:open]" + " your hold effect"));
                tooltips.Add(openTooltip);
            }
        }
    }
    public class LuckPunchProj : ModProjectile
    {
        public override string Texture => "Pokemon/Content/Accessories/LuckPunch";
        private readonly Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Content/Accessories/LuckPunch_").Value;
        private readonly Texture2D texture_ = ModContent.Request<Texture2D>("Pokemon/Content/Accessories/LuckPunchProj_").Value;

        private bool returning = false;
        private Vector2 startPos;
        private readonly float maxDistance = 120f;
        private readonly float speed = 16f;

        // 新增：记录发射时的角度
        private float fireRotation;

        public override void SetDefaults()
        {
            Projectile.width = 16;// 宽度
            Projectile.height = 16;// 高度
            Projectile.aiStyle = -1;// 无AI
            Projectile.friendly = true;// 友方
            Projectile.hostile = false;// 敌对
            Projectile.penetrate = -1;// 穿透
            Projectile.timeLeft = 60;// 弹幕持续时间
            Projectile.DamageType = DamageClass.Melee;// 伤害类型
            Projectile.ignoreWater = true;// 不与水块碰撞
            Projectile.tileCollide = true;// 碰撞
            Projectile.ownerHitCheck = false;// 取消击退
            Projectile.usesLocalNPCImmunity = true;// 本地NPC免疫
            Projectile.localNPCHitCooldown = -1;// 本地NPC击退冷却
        }

        public override void OnSpawn(IEntitySource source)
        {
            startPos = Projectile.Center;
            Player player = Main.player[Projectile.owner];
            Vector2 dir = Main.MouseWorld - player.MountedCenter;
            if (dir == Vector2.Zero) dir = Vector2.UnitX * player.direction + Vector2.UnitY * -1f;
            dir.Normalize();
            Projectile.velocity = dir * speed;
            fireRotation = dir.ToRotation(); // 记录发射角度
        }

        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
            returning = true;
            for(int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.position, 1, 1, DustID.Smoke, 0, 0, 100, default, 1f);
            }
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Vector2.Distance(Projectile.Center, startPos) > 100f)
            {
                modifiers.Knockback *= 2f;
                modifiers.SetCrit();
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Vector2.Distance(Projectile.Center, startPos) > 100f)
            {
                modifiers.SourceDamage *= 1.5f;
                modifiers.Knockback *= 2f;
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!returning)
            {
                if (Vector2.Distance(Projectile.Center, startPos) >= maxDistance)
                {
                    returning = true;
                }
            }

            if (returning)
            {
                Vector2 toPlayer = player.MountedCenter - Projectile.Center;
                float returnSpeed = speed + 4f;
                if (toPlayer.Length() < returnSpeed)
                {
                    Projectile.Kill();
                    return;
                }
                toPlayer.Normalize();
                Projectile.velocity = toPlayer * returnSpeed;
            }

            // 让手臂始终朝向发射方向
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, fireRotation - MathHelper.PiOver2);
            player.heldProj = Projectile.whoAmI; // 设置持有的投射物为这个投射物
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            //Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            //Texture2D texture_ = ModContent.Request<Texture2D>("Pokemon/Content/Accessories/LuckPunchProj_").Value;
            Vector2 origin = new Vector2(texture.Width / 2 - 10, texture.Height / 2 - 10);

            // 1. 绘制链条
            Vector2 handPos = player.MountedCenter; // 玩家手部中心
            Vector2 projPos = Projectile.Center;
            Vector2 dir = projPos - handPos;
            float chainRotation = fireRotation + MathHelper.PiOver2; // 贴图向上为正，需加90度
            float chainLength = dir.Length();
            dir.Normalize();

            float segmentLength = texture_.Height; // 贴图高度为一段长度
            int segmentCount = (int)(chainLength / segmentLength);

            for (int i = 0; i < segmentCount; i++)
            {
                Vector2 segmentPos = handPos + dir * (i * segmentLength);
                Main.EntitySpriteDraw(
                    texture_,
                    segmentPos - Main.screenPosition,
                    null,
                    lightColor,
                    chainRotation,
                    new Vector2(texture_.Width / 2, texture_.Height),
                    1f,
                    SpriteEffects.None,
                    0
                );
            }

            // 2. 绘制拳头
            SpriteEffects effects;
            float rotationOffset;
            if (player.direction > 0)
            {
                effects = SpriteEffects.None;
                rotationOffset = MathHelper.ToRadians(-225f);
            }
            else
            {
                origin = new Vector2(texture.Width / 2 + 10, texture.Height / 2 - 10);
                effects = SpriteEffects.FlipHorizontally;
                rotationOffset = MathHelper.ToRadians(45f);
            }
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                fireRotation + rotationOffset,
                origin,
                Projectile.scale*0.8f,
                effects,
                0
            );
            return false;
        }
    }
    public class FightMode : ModPlayer
    {
        public const int KuniSpriteWidth = 14;
        public const int KuniHoldAt = 9;
        public const int KuniTotalLength = 52;
        public const int MidExtendLength = 160;
        public const int HighExtendLegth = 240;
        public Vector2 EtimsKuniPos = Vector2.Zero;
        public Vector2 InvaKuniPos = Vector2.Zero;
        public float EtimsKuniRot = 0;
        public float InvaKuniRot = 0;
        public int InvaKuniExtend = 0;
        public float EtimsKuniScale = 1f;
        public bool CanParry = false;

        public override void PostItemCheck()
        {
            if (Player.HeldItem.type == ModContent.ItemType<LuckPunch>() && LuckPunch.isShow)
            {
                if (Player.velocity.Y < 0)
                {
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 3f * MathF.PI / 4f * Player.direction);
                    InvaKuniRot = Player.compositeFrontArm.rotation + MathF.PI / 2f * -Player.direction;

                    //Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, 3f * MathF.PI / 4f * -Player.direction);
                    //EtimsKuniRot = Player.compositeBackArm.rotation + MathF.PI / 2f * Player.direction;

                }
                else if (Player.velocity.Y == 0 && Player.velocity.X != 0)
                {
                    //Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -MathF.PI / 4f * Player.direction);
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathF.PI / 8f * Player.direction);
                    InvaKuniRot = Player.compositeFrontArm.rotation + MathF.PI / 2f * -Player.direction;

                    //Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, -MathF.PI / 4f * Player.direction);
                    //EtimsKuniRot = Player.compositeBackArm.rotation + MathF.PI / 2f * -Player.direction;
                }
                else
                {
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -MathF.PI / 4f * Player.direction);
                    //Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathF.PI / 8f * Player.direction);
                    InvaKuniRot = Player.compositeFrontArm.rotation + MathF.PI / 2f * -Player.direction;

                    //Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathF.PI / 8f * -Player.direction);
                    //EtimsKuniRot = Player.compositeBackArm.rotation + MathF.PI / 2f * -Player.direction;
                }

                InvaKuniPos = Player.GetFrontHandPosition(Player.compositeFrontArm.stretch, Player.compositeFrontArm.rotation);
                //EtimsKuniPos = Player.GetBackHandPosition(Player.compositeBackArm.stretch, Player.compositeBackArm.rotation);
                int fHeight = 56;
                if (Player.bodyFrame.Y == 7 * fHeight || Player.bodyFrame.Y == 8 * fHeight || Player.bodyFrame.Y == 9 * fHeight || Player.bodyFrame.Y == 14 * fHeight || Player.bodyFrame.Y == 15 * fHeight || Player.bodyFrame.Y == 16 * fHeight)
                {
                    if (Player.gravDir == -1)
                    {
                        InvaKuniPos.Y += 2;
                        //EtimsKuniPos.Y += 2;
                    }
                    else
                    {
                        InvaKuniPos.Y -= 2;
                        //EtimsKuniPos.Y -= 2;
                    }
                }
                InvaKuniRot += MathF.PI / 2f;
                EtimsKuniRot += MathF.PI / 2f;
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            base.OnHurt(info);
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            return false;
        }
    }
    public class InvaKuniDraw : PlayerDrawLayer
    {
        private readonly Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Content/Accessories/LuckPunch_").Value;
        private int flagBuffer = 0;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return true;
        }
        //public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = Main.player[drawInfo.drawPlayer.whoAmI];
            if (drawInfo.drawPlayer.JustDroppedAnItem)
            {
                return;
            }
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.TryGetModPlayer(out FightMode fightPlayer))
            {
                bool flag = Main.projectile.Any(proj => proj.active && (proj.type == ModContent.ProjectileType<LuckPunchProj>()));

                // 缓冲逻辑
                if (flag)
                {
                    flagBuffer = 10;
                }
                else if (flagBuffer > 0)
                {
                    flagBuffer--;
                }

                // 判断条件：flag为true或缓冲未结束时都视为flag为true
                bool flagOrBuffer = flag || flagBuffer > 0;

                Color color12 = drawPlayer.GetImmuneAlphaPure(
                    Lighting.GetColor(
                        (int)((double)drawInfo.Position.X + (double)drawPlayer.width * 0.5) / 16,
                        (int)((double)drawInfo.Position.Y + (double)drawPlayer.height * 0.5) / 16,
                        Microsoft.Xna.Framework.Color.White), 0f);

                if (!drawPlayer.HeldItem.IsAir && drawPlayer.HeldItem.type == ModContent.ItemType<LuckPunch>() && !flagOrBuffer && LuckPunch.isShow)
                {
                    Item item = drawPlayer.HeldItem;
                    //Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Content/Accessories/LuckPunch_").Value;
                    if (player.direction == 1)
                    {
                        if (player.velocity.Y < 0)
                        {
                            DrawData value = new DrawData(texture, fightPlayer.InvaKuniPos - Main.screenPosition + new Vector2(16, 16), default, color12, fightPlayer.InvaKuniRot + MathHelper.ToRadians(225f), new Vector2(texture.Width, texture.Height), item.scale * 0.7f, SpriteEffects.None, 0);
                            drawInfo.DrawDataCache.Add(value);
                        }
                        else if (player.velocity.Y == 0 && player.velocity.X != 0)
                        {
                            DrawData value = new DrawData(texture, fightPlayer.InvaKuniPos - Main.screenPosition + new Vector2(8, -21), default, color12, fightPlayer.InvaKuniRot + MathHelper.ToRadians(225f), new Vector2(texture.Width, texture.Height), item.scale * 0.7f, SpriteEffects.None, 0);
                            drawInfo.DrawDataCache.Add(value);
                        }
                        else
                        {
                            DrawData value = new DrawData(texture, fightPlayer.InvaKuniPos - Main.screenPosition + new Vector2(-17, -16), default, color12, fightPlayer.InvaKuniRot + MathHelper.ToRadians(225f), new Vector2(texture.Width, texture.Height), item.scale * 0.7f, SpriteEffects.None, 0);
                            drawInfo.DrawDataCache.Add(value);
                        }

                    }
                    else
                    {
                        if (player.velocity.Y < 0)
                        {
                            DrawData value = new DrawData(texture, fightPlayer.InvaKuniPos - Main.screenPosition + new Vector2(14, 16), default, color12, fightPlayer.InvaKuniRot + MathHelper.ToRadians(-45f), new Vector2(texture.Width, texture.Height), item.scale * 0.7f, SpriteEffects.FlipHorizontally, 0);
                            drawInfo.DrawDataCache.Add(value);
                        }
                        else if (player.velocity.Y == 0 && player.velocity.X != 0)
                        {
                            DrawData value = new DrawData(texture, fightPlayer.InvaKuniPos - Main.screenPosition + new Vector2(-21, 7), default, color12, fightPlayer.InvaKuniRot + MathHelper.ToRadians(-45f), new Vector2(texture.Width, texture.Height), item.scale * 0.7f, SpriteEffects.FlipHorizontally, 0);
                            drawInfo.DrawDataCache.Add(value);
                        }
                        else
                        {
                            DrawData value = new DrawData(texture, fightPlayer.InvaKuniPos - Main.screenPosition + new Vector2(-15, -15), default, color12, fightPlayer.InvaKuniRot + MathHelper.ToRadians(-45f), new Vector2(texture.Width, texture.Height), item.scale * 0.7f, SpriteEffects.FlipHorizontally, 0);
                            drawInfo.DrawDataCache.Add(value);
                        }
                    }
                }
            }
        }
    }
}