//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Pokemon.Content.Equipment;
//using Pokemon.Content.Items;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace Pokemon.Content.Equipment
//{
//    public class ExperienceOrb : ModProjectile
//    {
//        public override void SetDefaults()
//        {
//            Projectile.width = 10;
//            Projectile.height = 10;
//            Projectile.friendly = true;
//            Projectile.tileCollide = false;
//            Projectile.timeLeft = 600;
//            Projectile.penetrate = -1;
//            Projectile.aiStyle = 0;
//        }

//        public override void AI()
//        {
//            Projectile.rotation += 0.05f;
//            Player player = Main.player[Main.myPlayer];
//            float pickupRange = 20f;

//            // 查找第一个装备的宠物徽章
//            var badge = FindFirstEquippedBadge(player);

//            if (badge != null && badge.Item.damage < 100)
//            {
//                // 吸附效果
//                Vector2 toPlayer = player.Center - Projectile.Center;
//                if (toPlayer.Length() < 200f)
//                    Projectile.velocity += toPlayer.SafeNormalize(Vector2.Zero) * 0.4f;
//                Projectile.velocity *= 0.95f;

//                // 拾取
//                if (Vector2.Distance(Projectile.Center, player.Center) < pickupRange)
//                {
//                    // 增加经验
//                    AddExpToBadge(badge, (int)Projectile.ai[0]);
//                    // 文本提示
//                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height),
//                        new Color(200, 250, 250), (int)Projectile.ai[0]);
//                    Projectile.Kill(); // 拾取后消失
//                }
//            }
//            // 不满足条件时，弹幕只会缓慢减速漂浮
//            else
//            {
//                Projectile.velocity *= 0.95f;
//            }
//        }

//        [System.Obsolete]
//        public override void Kill(int timeLeft)
//        {
//            //粒子效果
//            int dustType = DustID.Electric;
//            Color color = Color.Green;
//            if (Projectile.ai[0] >= 100)
//            {
//                dustType = DustID.Firework_Yellow;
//                color = Color.Yellow;
//            }
//            else if (Projectile.ai[0] >= 10)
//            {
//                dustType = DustID.BubbleBurst_White;
//                color = Color.White;
//            }

//            int dustIndex = Dust.NewDust(Projectile.position, 0, 0, dustType, 0, 0, 150, color, 1f);
//            Main.dust[dustIndex].velocity *= 0.2f;
//            Main.dust[dustIndex].noGravity = true;
//        }
//        public override bool PreDraw(ref Color lightColor)
//        {
//            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
//            Color drawColor = Color.White;
//            int frame = 0;
//            if (Projectile.ai[0] >= 100)
//            {
//                Projectile.scale = 1.5f;
//                drawColor = Color.Gold;
//                frame = 2; // 第三帧
//            }
//            else if (Projectile.ai[0] >= 10)
//            {
//                Projectile.scale = 1.2f;
//                drawColor = Color.Silver;
//                frame = 1; // 第二帧
//            }
//            // 普通经验球用默认颜色和第一帧

//            // 计算帧区域
//            int frameCount = 3;
//            int frameHeight = texture.Height / frameCount;
//            Rectangle sourceRect = new Rectangle(0, frame * frameHeight, texture.Width, frameHeight);

//            Main.EntitySpriteDraw(
//                texture,
//                Projectile.Center - Main.screenPosition,
//                sourceRect,
//                drawColor,
//                Projectile.rotation,
//                new Vector2(texture.Width / 2f, frameHeight / 2f),
//                Projectile.scale,
//                SpriteEffects.None,
//                0
//            );
//            return false; // 阻止默认绘制
//        }

//        private ModItem FindFirstEquippedBadge(Player player)
//        {
//            // 遍历PokeRadar的items数组，找到第一个徽章
//            foreach (var item in player.inventory)
//            {
//                if (item.ModItem is PokeRadar radar)
//                {
//                    foreach (var badge in radar.items)
//                    {
//                        if (badge != null && !badge.IsAir && badge.ModItem is BulbasaurBadge)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is CharmanderBadge)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is SquirtleBadge)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is GastlyBadge)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is TaillowBadge)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is SunflowerBall)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is SpoinkBadge)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is BeldumBadge)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is WingullBadge)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is VoltorbBadge)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is MunchlaxBadge)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is FomantisBadge)
//                            return badge.ModItem;
//                        if (badge != null && !badge.IsAir && badge.ModItem is TrapinchBadge)// 13
//                            return badge.ModItem;
//                    }
//                }
//            }
//            return null;
//        }

//        private void AddExpToBadge(ModItem badge, int exp)
//        {
//            dynamic b = badge;
//            b.exp += exp;
//            // 计算当前等级升级所需经验：40 + (当前等级-1)*10
//            while (b.level < 100)
//            {
//                int needExp = 40 + (b.level - 1) * 10;
//                if (b.exp >= needExp)
//                {
//                    b.exp -= needExp;
//                    b.level++;
//                    // 添加提示
//                    Player player = Main.player[Main.myPlayer];

//                    if(b.level == 6 || b.level == 9 || b.level == 12)
//                    {
//                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
//                        new Color(200, 250, 250), "宝可梦升到了" + b.level + "级，学会了新技能！"); // 显示文本提示
//                    }else
//                    {
//                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
//                        new Color(200, 250, 250), "宝可梦升到了" + b.level + "级！"); // 显示文本提示
//                    }
//                }
//                else
//                {
//                    break;
//                }
//            }
//        }
//    }
//    public class ExperienceOrbLoot : GlobalNPC
//    {
//        public override void OnKill(NPC npc)
//        {
//            if (Main.netMode == NetmodeID.Server) return; // 只在客户端生成

//            if (!PlayerHasEquippedBadge(Main.LocalPlayer))
//                return;

//            int totalExp = npc.lifeMax / 30;

//            // 计算各类经验球数量
//            int big = totalExp / 100;
//            int mid = (totalExp % 100) / 10;
//            int small = totalExp % 10;

//            // 生成100点经验球
//            for (int i = 0; i < big; i++)
//            {
//                Projectile.NewProjectile(
//                    npc.GetSource_Death(),
//                    npc.Center,
//                    Main.rand.NextVector2Circular(2, 2),
//                    ModContent.ProjectileType<ExperienceOrb>(),
//                    0, 0, Main.myPlayer, 100 // ai[0]=100经验
//                );
//            }
//            // 生成10点经验球
//            for (int i = 0; i < mid; i++)
//            {
//                Projectile.NewProjectile(
//                    npc.GetSource_Death(),
//                    npc.Center,
//                    Main.rand.NextVector2Circular(2, 2),
//                    ModContent.ProjectileType<ExperienceOrb>(),
//                    0, 0, Main.myPlayer, 10 // ai[0]=10经验
//                );
//            }
//            // 生成1点经验球
//            for (int i = 0; i < small; i++)
//            {
//                Projectile.NewProjectile(
//                    npc.GetSource_Death(),
//                    npc.Center,
//                    Main.rand.NextVector2Circular(2, 2),
//                    ModContent.ProjectileType<ExperienceOrb>(),
//                    0, 0, Main.myPlayer, 1 // ai[0]=1经验
//                );
//            }
//        }

//        // 判断玩家是否装备了徽章
//        private bool PlayerHasEquippedBadge(Player player)
//        {
//            foreach (var item in player.inventory)
//            {
//                if (item.ModItem is PokeRadar radar)
//                {
//                    foreach (var badge in radar.items)
//                    {
//                        if (badge != null && !badge.IsAir && badge.ModItem is BulbasaurBadge)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is CharmanderBadge)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is SquirtleBadge)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is GastlyBadge)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is TaillowBadge)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is SunflowerBall)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is SpoinkBadge)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is BeldumBadge)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is WingullBadge)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is VoltorbBadge)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is MunchlaxBadge)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is FomantisBadge)
//                            return true;
//                        if (badge != null && !badge.IsAir && badge.ModItem is TrapinchBadge)// 13
//                            return true;
//                        // 其他徽章同理，或用基类/接口判断
//                    }
//                }
//            }
//            return false;
//        }
//    }
//}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Content.Equipment
{
    public class ExperienceOrb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
            Projectile.rotation += 0.05f;
            float pickupRange = 20f;

            Player nearestPlayer = null;
            float nearestDist = float.MaxValue;

            // 遍历所有玩家，找最近的活着的玩家
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead)
                {
                    float dist = Vector2.Distance(Projectile.Center, p.Center);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearestPlayer = p;
                    }
                }
            }

            if (nearestPlayer != null)
            {
                // 获取前两个徽章
                var badges = FindFirstTwoEquippedBadges(nearestPlayer);
                if (badges.Count > 0 && badges[0].Item.damage < 100)
                {
                    // 吸附效果
                    Vector2 toPlayer = nearestPlayer.Center - Projectile.Center;
                    if (toPlayer.Length() < 200f)
                        Projectile.velocity += toPlayer.SafeNormalize(Vector2.Zero) * 0.4f;
                    Projectile.velocity *= 0.95f;

                    // 拾取
                    if (nearestDist < pickupRange)
                    {
                        if (nearestPlayer.whoAmI == Main.myPlayer)
                        {
                            // 经验分配
                            if (Expshare.ExtraExerciseEnabled && badges.Count > 1)
                            {
                                AddExpToBadge(badges[0], (int)Projectile.ai[0]);
                                AddExpToBadge(badges[1], (int)Projectile.ai[0]);
                            }
                            else
                            {
                                AddExpToBadge(badges[0], (int)Projectile.ai[0]);
                            }
                            CombatText.NewText(new Rectangle((int)nearestPlayer.position.X, (int)nearestPlayer.position.Y, nearestPlayer.width, nearestPlayer.height),
                                new Color(200, 250, 250), (int)Projectile.ai[0]);
                        }
                        Projectile.Kill();
                    }
                }
                else
                {
                    Projectile.velocity *= 0.95f;
                }
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }
        }

        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            //粒子效果
            int dustType = DustID.Electric;
            Color color = Color.Green;
            if (Projectile.ai[0] >= 100)
            {
                dustType = DustID.Firework_Yellow;
                color = Color.Yellow;
            }
            else if (Projectile.ai[0] >= 10)
            {
                dustType = DustID.BubbleBurst_White;
                color = Color.White;
            }

            int dustIndex = Dust.NewDust(Projectile.position, 0, 0, dustType, 0, 0, 150, color, 1f);
            Main.dust[dustIndex].velocity *= 0.2f;
            Main.dust[dustIndex].noGravity = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Color drawColor = Color.White;
            int frame = 0;
            if (Projectile.ai[0] >= 100)
            {
                Projectile.scale = 1.5f;
                drawColor = Color.Gold;
                frame = 2; // 第三帧
            }
            else if (Projectile.ai[0] >= 10)
            {
                Projectile.scale = 1.2f;
                drawColor = Color.Silver;
                frame = 1; // 第二帧
            }
            // 普通经验球用默认颜色和第一帧

            // 计算帧区域
            int frameCount = 3;
            int frameHeight = texture.Height / frameCount;
            Rectangle sourceRect = new Rectangle(0, frame * frameHeight, texture.Width, frameHeight);

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRect,
                drawColor,
                Projectile.rotation,
                new Vector2(texture.Width / 2f, frameHeight / 2f),
                Projectile.scale,
                SpriteEffects.None,
                0
            );
            return false; // 阻止默认绘制
        }

        private ModItem FindFirstEquippedBadge(Player player)
        {
            // 遍历PokeRadar的items数组，找到第一个徽章
            foreach (var item in player.inventory)
            {
                if (item.ModItem is PokeRadar radar)
                {
                    foreach (var badge in radar.items)
                    {
                        if (badge != null && !badge.IsAir && badge.ModItem is BulbasaurBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is CharmanderBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is SquirtleBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is GastlyBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is TaillowBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is SunflowerBall)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is SpoinkBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is BeldumBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is WingullBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is VoltorbBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is MunchlaxBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is FomantisBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is TrapinchBadge)
                            return badge.ModItem;
                        if (badge != null && !badge.IsAir && badge.ModItem is PikachuBadge)// 14
                            return badge.ModItem;
                    }
                }
            }
            return null;
        }
        // 获取前两个徽章
        private List<ModItem> FindFirstTwoEquippedBadges(Player player)
        {
            var result = new List<ModItem>();
            foreach (var item in player.inventory)
            {
                if (item.ModItem is PokeRadar radar)
                {
                    foreach (var badge in radar.items)
                    {
                        if (badge != null && !badge.IsAir &&
                            (badge.ModItem is BulbasaurBadge ||
                             badge.ModItem is CharmanderBadge ||
                             badge.ModItem is SquirtleBadge ||
                             badge.ModItem is GastlyBadge ||
                             badge.ModItem is TaillowBadge ||
                             badge.ModItem is SunflowerBall ||
                             badge.ModItem is SpoinkBadge ||
                             badge.ModItem is BeldumBadge ||
                             badge.ModItem is WingullBadge ||
                             badge.ModItem is VoltorbBadge ||
                             badge.ModItem is MunchlaxBadge ||
                             badge.ModItem is FomantisBadge ||
                             badge.ModItem is TrapinchBadge ||
                             badge.ModItem is PikachuBadge))//14
                        {
                            result.Add(badge.ModItem);
                            if (result.Count == 2)
                                return result;
                        }
                    }
                }
            }
            return result;
        }
        private void AddExpToBadge(ModItem badge, int exp)
        {
            dynamic b = badge;
            b.exp += exp;
            // 计算当前等级升级所需经验：40 + (当前等级-1)*10
            while (b.level < 100)
            {
                int needExp = 40 + (b.level - 1) * 10;
                if (b.exp >= needExp)
                {
                    b.exp -= needExp;
                    b.level++;
                    // 添加提示
                    Player player = Main.player[Main.myPlayer];

                    if (b.level == 6 || b.level == 9 || b.level == 12)
                    {
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                        new Color(200, 250, 250), "宝可梦升到了" + b.level + "级，学会了新技能！"); // 显示文本提示
                    }
                    else
                    {
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                        new Color(200, 250, 250), "宝可梦升到了" + b.level + "级！"); // 显示文本提示
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    public class ExperienceOrbLoot : GlobalNPC
    {
        public override bool CheckDead(NPC npc)
        {
            Player player = Main.player[Main.myPlayer];
            if (PlayerHasEquippedBadge(player))
            {
                int totalExp = npc.lifeMax / 30;
                int big = totalExp / 100;
                int mid = (totalExp % 100) / 10;
                int small = totalExp % 10;

                for (int i = 0; i < big; i++)
                {
                    Projectile.NewProjectile(
                        npc.GetSource_Death(),
                        npc.Center,
                        Main.rand.NextVector2Circular(2, 2),
                        ModContent.ProjectileType<ExperienceOrb>(),
                        0, 0, Main.myPlayer, 100
                    );
                }

                for (int i = 0; i < mid; i++)
                {
                    Projectile.NewProjectile(
                        npc.GetSource_Death(),
                        npc.Center,
                        Main.rand.NextVector2Circular(2, 2),
                        ModContent.ProjectileType<ExperienceOrb>(),
                        0, 0, Main.myPlayer, 10
                    );
                }
                for (int i = 0; i < small; i++)
                {
                    Projectile.NewProjectile(
                        npc.GetSource_Death(),
                        npc.Center,
                        Main.rand.NextVector2Circular(2, 2),
                        ModContent.ProjectileType<ExperienceOrb>(),
                        0, 0, Main.myPlayer, 1
                    );
                }
            }
            return base.CheckDead(npc);
        }
        // 判断玩家是否装备了徽章
        private bool PlayerHasEquippedBadge(Player player)
        {
            foreach (var item in player.inventory)
            {
                if (item.ModItem is PokeRadar radar)
                {
                    foreach (var badge in radar.items)
                    {
                        if (badge != null && !badge.IsAir &&
                            (badge.ModItem is BulbasaurBadge ||
                             badge.ModItem is CharmanderBadge ||
                             badge.ModItem is SquirtleBadge ||
                             badge.ModItem is GastlyBadge ||
                             badge.ModItem is TaillowBadge ||
                             badge.ModItem is SunflowerBall ||
                             badge.ModItem is SpoinkBadge ||
                             badge.ModItem is BeldumBadge ||
                             badge.ModItem is WingullBadge ||
                             badge.ModItem is VoltorbBadge ||
                             badge.ModItem is MunchlaxBadge ||
                             badge.ModItem is FomantisBadge ||
                             badge.ModItem is TrapinchBadge ||
                             badge.ModItem is PikachuBadge))// 14
                            return true;
                    }
                }
            }
            return false;
        }
    }
}