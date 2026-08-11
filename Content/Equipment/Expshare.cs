using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Pokemon.Content.Equipment
{
    public class Expshare : ModItem
    {
        //学习装置
        public static bool ExtraExerciseEnabled = true; // 是否启用额外增加
        private bool isExtraExercise = false; // 是否额外增加
        private bool isDraw = false; // 是否绘制

        public override void SetDefaults()
        {
            Item.width = 44; // 宽度
            Item.height = 40; // 高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Green; // 稀有度
        }

        public override bool CanRightClick()
        {
            if (Main.mouseRight && !isExtraExercise)
            {
                if (Main.mouseRightRelease)
                {
                    ExtraExerciseEnabled = !ExtraExerciseEnabled;
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
                }
            }
            isExtraExercise = Main.mouseRightRelease;
            
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Language.ActiveCulture.Name == "zh-Hans")
            {
                tooltips.Add(new TooltipLine(Mod, "", "该装置可以让强制锻炼器击杀敌人获得的击败数增加\n第二只宝可梦额外获得相同经验"));
                tooltips.Add(new TooltipLine(Mod, "", "每多装备1个宝可梦徽章击败数额外增加1点"));
                tooltips.Add(new TooltipLine(Mod, "", "【背包生效】"));
                string state = ExtraExerciseEnabled ? "[c/88EE88:关闭]" : "[c/88EE88:打开]";
                tooltips.Add(new TooltipLine(Mod, "", $"右键点击{state}该装置"));
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "", "This device increases the number of defeats obtained by killing enemies with the Forced Exerciser"));
                tooltips.Add(new TooltipLine(Mod, "", "Each Pokemon Badge adds 1 point to the number of defeats"));
                tooltips.Add(new TooltipLine(Mod, "", "[c/88EE88:Inventory Effect]"));
                string state = ExtraExerciseEnabled ? "[c/88EE88:Disable]" : "[c/88EE88:Enable]";
                tooltips.Add(new TooltipLine(Mod, "", $"Right-click to {state} this device"));
            }

        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/Expshare_Frame").Value;
            int frameHeight = texture.Height / 2; // 假设有两帧
            int frameY = ExtraExerciseEnabled ? 0 : frameHeight;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, position, sourceRectangle, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false; // 返回 false 以防止默认绘制
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/Expshare_Frame").Value;
            int frameHeight = texture.Height / 2; // 假设有两帧
            int frameY = ExtraExerciseEnabled ? 0 : frameHeight;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 position = new Vector2(Item.position.X - Main.screenPosition.X + Item.width / 2, Item.position.Y - Main.screenPosition.Y + Item.height / 2);
            spriteBatch.Draw(texture, position, sourceRectangle, lightColor, rotation, new Vector2(texture.Width / 2, frameHeight / 2), scale, SpriteEffects.None, 0f);
            return false; // 返回 false 以防止默认绘制
        }
    }
}

