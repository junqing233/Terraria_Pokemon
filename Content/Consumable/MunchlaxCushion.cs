using Microsoft.Xna.Framework;
using Pokemon.Content.Weapons.Mterial;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Pokemon.Content.Consumable
{
    public class MunchlaxCushion : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 46;   
            Item.height = 44;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 1);
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<MunchlaxCushionProj>();
            Item.shootSpeed = 8f;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Language.ActiveCulture.Name == "zh-Hans") // 检查是否为简体中文
                tooltips.Add(new TooltipLine(Mod, "MunchlaxCushionTooltip", "一款看上去就很舒适的抱枕\n扔出可以恢复生命值[c/337ea9:20%~80%]的抱枕"));
            else
                tooltips.Add(new TooltipLine(Mod, "MunchlaxCushionTooltip", "A cushion that looks extremely comfortable.\nThrow it to restore [c/337ea9:20%~80%] of your max life."));
        }
        //随便写的合成
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.Silk, 1) //丝绸
                .AddIngredient(ItemID.BlackThread, 1) // 黑线
                .AddIngredient(ModContent.ItemType<NormalCrystal>(), 1)// 普通晶
                .AddTile(TileID.Loom) // 织布机
                .Register();
        }
    }
    public class MunchlaxCushionProj : ModProjectile
    {
        public override string Texture => "Pokemon/Content/Consumable/MunchlaxCushion";
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 2; // 类似抛物线
        }

        public override void AI()
        {
            // 只在本地玩家检测，防止多人模式多次触发
            if (Main.netMode != NetmodeID.MultiplayerClient || Main.myPlayer == Main.LocalPlayer.whoAmI)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && !player.dead && Projectile.Hitbox.Intersects(player.Hitbox) && Projectile.timeLeft < 550)
                    {
                        ReHP(player);
                        Projectile.Kill();
                        break; // 只允许一个玩家拾取
                    }
                }
            }
        }
        [Obsolete]
        public override void Kill(int timeLeft)
        {
            // 定义要生成的 dust 颜色和数量
            (Color color, int count)[] dustConfigs = new (Color, int)[]
            {
                (default, 3),           // 默认色
                (Color.DeepSkyBlue, 1), // 天蓝色
                (Color.Goldenrod, 1)    // 金色
            };

            foreach (var (color, count) in dustConfigs)
            {
                for (int i = 0; i < count; i++)
                {
                    int dustType = DustID.WhiteTorch;
                    int dustIndex = Dust.NewDust(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        dustType,
                        0f, 0f, 1, color, 1f
                    );
                    Main.dust[dustIndex].scale = 1.5f;
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].noLight = true;
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        private static void ReHP(Player target)
        {
            int minHeal = (int)(target.statLifeMax2 * 0.2f);
            int maxHeal = (int)(target.statLifeMax2 * 0.8f);
            float t = (float)Math.Pow(Main.rand.NextFloat(), 2.5);
            int healAmount = minHeal + (int)((maxHeal - minHeal) * t);

            target.statLife += healAmount;
            target.HealEffect(healAmount);
            if (target.statLife > target.statLifeMax2)
                target.statLife = target.statLifeMax2;
        }
    }
}
