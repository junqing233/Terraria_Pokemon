using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.Biomes;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Content
{
    public class ExampleModMenu : ModMenu
    {
        private const string menuAssetPath = "Pokemon/Assets/Textures/Menu"; // 创建一个常量变量用于表示贴图路径，这样就不用多次手写路径了

        private Asset<Texture2D> sunTexture;
        private Asset<Texture2D> moonTexture;

        public override void Load()
        {
            sunTexture = ModContent.Request<Texture2D>($"{menuAssetPath}/PokemonSun");
            moonTexture = ModContent.Request<Texture2D>($"{menuAssetPath}/PokemonMoon");
        }

        public override Asset<Texture2D> Logo => base.Logo;

        public override Asset<Texture2D> SunTexture => sunTexture;

        public override Asset<Texture2D> MoonTexture => moonTexture;

        /*
		在 ExampleMod 中，我们预加载了所有“额外”贴图，推荐做法见 https://github.com/tModLoader/tModLoader/wiki/Assets#asset-loading-timing。
		当然也可以按需加载贴图，这在某些极少用到的大贴图场景下很有用。示例代码如下：
		private Asset<Texture2D> moonTexture;
		public override Asset<Texture2D> MoonTexture => moonTexture ??= ModContent.Request<Texture2D>($"{menuAssetPath}/ExampliumMoon");
		*/

        public override int Music => MusicLoader.GetMusicSlot("Pokemon/Assets/Music/PokemonTheme");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<PokemonSurfaceBackgroundStyle>();

        public override string DisplayName => "宝可梦·旅程";

        private static readonly string[] PikaSounds = new[]
        {
            "Pokemon/Assets/Sounds/Pika_0",
            "Pokemon/Assets/Sounds/Pika_1",
            "Pokemon/Assets/Sounds/Pika_2",
            "Pokemon/Assets/Sounds/Pika_3",
            "Pokemon/Assets/Sounds/Pika_4"
        };
        private int RandCount = 0;
        public override void OnSelected()
        {
            SoundEngine.PlaySound(new SoundStyle(PikaSounds[RandCount]));
            RandCount++;
            if (RandCount > 4)
            {
                RandCount = 0;
            }
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            // 加载自定义Logo贴图
            Texture2D logoTexture = ModContent.Request<Texture2D>("Pokemon/Assets/Textures/Backgrounds/Pokemon_Logo").Value;

            // 以logoDrawCenter为中心绘制，支持缩放和旋转
            Vector2 origin = new Vector2(logoTexture.Width / 2f, logoTexture.Height / 2f);
            spriteBatch.Draw(
                logoTexture,
                logoDrawCenter,
                null,
                drawColor, // 使用默认颜色
                logoRotation,
                origin,
                logoScale,
                SpriteEffects.None,
                0f
            );
            // 返回false，阻止默认Logo绘制
            return false;
        }
    }
}
