using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Pokemon.Content.Accessories
{
    public class ShinyCharm : ModItem
    {
        private readonly Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Content/Accessories/ShinyCharm_P").Value;
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 44;
            Item.accessory = true;
            Item.vanity = true;//允许在时装栏生效
            Item.defense = 5;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(platinum: 5);
            Item.hasVanityEffects = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Language.ActiveCulture.Name == "zh-Hans") // 检查是否为简体中文
                tooltips.Add(new TooltipLine(Mod, "ShinyCharmTooltip", "[c/8ad6cc:捐赠者物品]"));
            else
                tooltips.Add(new TooltipLine(Mod, "ShinyCharmTooltip", "[c/8ad6cc:Donator Item]"));
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            int frameCount = 13;
            int currentFrame = (int)((Main.GameUpdateCount / 8) % frameCount);
            //Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Content/Accessories/ShinyCharm_P").Value;
            Rectangle sourceRectangle = new Rectangle(0, currentFrame * (texture.Height / frameCount), texture.Width, texture.Height / frameCount);
            spriteBatch.Draw(texture, position + new Vector2(0, 0), sourceRectangle, drawColor, 0f, origin, scale * 1f, SpriteEffects.None, 0f);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            //Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Content/Accessories/ShinyCharm_P").Value;
            int totalFrames = 13;
            int frameHeight = texture.Height / totalFrames;
            int currentFrame = (int)(Main.GameUpdateCount / 8 % totalFrames);
            Rectangle sourceRectangle = new Rectangle(0, currentFrame * frameHeight, texture.Width, frameHeight);
            Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, texture.Height / 2 - Item.height / 2);

            // 以帧6最不透明，其余帧逐渐变透明
            float maxAlpha = 1.0f; // 最不透明
            float minAlpha = 0.4f; // 最透明
            float alpha;
            if (currentFrame == 6)
                alpha = maxAlpha;
            else
                alpha = MathHelper.Lerp(maxAlpha, minAlpha, Math.Abs(currentFrame - 6) / 6f);

            Color drawColor = Color.White * alpha;

            spriteBatch.Draw(
                texture,
                drawPosition + new Vector2(0, frameHeight * 5.5f),
                sourceRectangle,
                drawColor,
                rotation,
                new Vector2(texture.Width / 2, frameHeight / 2),
                scale * 1f,
                SpriteEffects.None,
                0f
            );
            return false;
        }
        public override void PostUpdate()
        {
            int totalFrames = 13;
            int currentFrame = (int)((Main.GameUpdateCount / 8) % totalFrames);

            // 以帧6最亮，其余帧逐渐减弱
            float maxIntensity = 1.2f; // 最亮时的强度
            float minIntensity = 0.4f; // 最暗时的强度
            float intensity;

            if (currentFrame == 6)
                intensity = maxIntensity;
            else
                // 距离6帧越远越暗，线性插值
                intensity = MathHelper.Lerp(maxIntensity, minIntensity, Math.Abs(currentFrame - 6) / 6f);

            Lighting.AddLight(Item.Center, 0.40f * intensity, 0.60f * intensity, 0.85f * intensity);
        }
        //合成配方
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SkyBlueDye, 1)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        //饰品效果
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //同时响应装备栏和时装栏
            player.GetModPlayer<ShinyCharmPlayer>().hasEffect = true;
        }
        public override void UpdateVanity(Player player)
        {
            //时装栏专用触发逻辑
            player.GetModPlayer<ShinyCharmPlayer>().hasEffect = true;
        }
    }
    //ModPlayer 类处理特效
    public class ShinyCharmPlayer : ModPlayer
    {
        public bool hasEffect;

        public override void ResetEffects() => hasEffect = false;

        public override void PostUpdateEquips()
        {
            if (!hasEffect) return;

            //增强粒子系统
            Vector2 particlePos = Player.Center + new Vector2(
                Main.rand.Next(-Player.width / 2, Player.width / 2),
                -Player.height / 2 - 10
            );

            //双重粒子特效
            if (Main.rand.NextBool(2))//提高粒子生成频率
            {
                //金色主粒子
                Dust goldDust = Dust.NewDustPerfect(
                    particlePos,
                    DustID.GoldFlame,
                    new Vector2(0, -Main.rand.NextFloat(0.5f, 1.5f)),
                    Scale: Main.rand.NextFloat(1.0f, 1.8f)
                );
                goldDust.noGravity = true;

                //辅助闪光粒子
                if (Main.rand.NextBool(5))
                {
                    Dust sparkle = Dust.NewDustPerfect(
                        particlePos + new Vector2(0, 10),
                        DustID.Electric,
                        new Vector2(0, -Main.rand.NextFloat(0.2f, 0.8f)),
                        Scale: 0.6f
                    );
                    sparkle.noLight = true;
                }
            }
            //添加光效
            if (Main.rand.NextBool(10))
            {
                Lighting.AddLight(Player.Center, new Vector3(1f, 0.9f, 0.3f));
            }
        }
    }
}
