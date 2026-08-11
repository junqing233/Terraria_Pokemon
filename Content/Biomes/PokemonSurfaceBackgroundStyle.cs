using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace Pokemon.Content.Biomes
{
    /// <summary>
    /// 定义宝可梦地表生物群系的自定义背景风格。
    /// 通过重写 ModSurfaceBackgroundStyle 的方法，指定不同距离的背景贴图和动画。
    /// </summary>
    public class PokemonSurfaceBackgroundStyle : ModSurfaceBackgroundStyle
    {
        /// <summary>
        /// 控制远景背景（如山脉）的淡入淡出效果，实现平滑过渡。
        /// </summary>
        /// <param name="fades">每个背景槽的淡入值</param>
        /// <param name="transitionSpeed">淡入淡出速度</param>
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot) // 当前生物群系的背景槽
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else // 其他生物群系的背景槽
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        /// <summary>
        /// 返回远景背景贴图的槽位（如山脉等远处背景）。
        /// </summary>
        public override int ChooseFarTexture()
        {
            //return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceFar");
            //return -1;
            if (++SurfaceFrameCounter_ > 12) // 每12帧切换一次动画帧
            {
                SurfaceFrame_ = (SurfaceFrame_ + 1) % 12;
                SurfaceFrameCounter_ = 0;
            }
            switch (SurfaceFrame_)
            {
                case 0:
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid0");
                case 1:
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid1");
                case 2:
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid2");
                case 3:
                    // 也可以使用完整路径
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid3");
                case 4:
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid4");
                case 5:
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid5");
                case 6:
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid6");
                case 7:
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid7");
                case 8:
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid8");
                case 9:
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid9");
                case 10:
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid10");
                case 11:
                    return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid11");
                default:
                    return -1;
            }
        }
        
        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            //Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid01").Value;
            ////spriteBatch.Draw(texture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * 0.5f);
            //Rectangle rectangle = new Rectangle(//因为手动绘制需要自己填写帧图框,所以要先算出来
            //   0,//这个框的左上角的水平坐标(填0就好)
            //   0,//框的左上角的纵向坐标
            //   texture.Width, //框的宽度(材质宽度即可)
            //   texture.Height//框的高度（用材质高度除以帧数得到单帧高度）
            //   );
            //spriteBatch.Draw(texture,//第一个参数是材质
            //    new Vector2(Main.screenWidth / 2, Main.screenHeight / 2),//注意，绘制时的位置是以屏幕左上角为0点
            //                                                     //因此要用弹幕世界坐标减去屏幕左上角的坐标
            //    rectangle,//第三个参数就是帧图选框了
            //    Color.White,//第四个参数是颜色，这里我们用自带的lightcolor，可以受到自然光照影响
            //                       //Color.White,
            //    0,//第五个参数是贴图旋转方向
            //    new Vector2(texture.Width / 2, texture.Height / 2),
            //    //第六个参数是贴图参照原点的坐标，这里写为贴图单帧的中心坐标，这样旋转和缩放都是围绕中心
            //    new Vector2(1),//第七个参数是缩放，X是水平倍率，Y是竖直倍率
            //    SpriteEffects.None,
            //    //第八个参数是设置图片翻转效果，需要手动判定并设置spriteeffects
            //    0);//第九个参数是绘制层级，但填0就行了，不太好使);
            return true;
        }
        // 用于中景背景动画的帧计数器和当前帧
        private static int SurfaceFrameCounter;
        private static int SurfaceFrame;
        private static int SurfaceFrameCounter_;
        private static int SurfaceFrame_;


        /// <summary>
        /// 返回中景背景贴图的槽位，并实现简单的帧动画（4帧循环）。
        /// </summary>
        public override int ChooseMiddleTexture()
        {
            //if (++SurfaceFrameCounter_ > 12) // 每12帧切换一次动画帧
            //{
            //    SurfaceFrame_ = (SurfaceFrame_ + 1) % 4;
            //    SurfaceFrameCounter_ = 0;
            //}
            //switch (SurfaceFrame_)
            //{
            //    case 0:
            //        return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid0");
            //    case 1:
            //        return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid1");
            //    case 2:
            //        return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid2");
            //    case 3:
            //        // 也可以使用完整路径
            //        return BackgroundTextureLoader.GetBackgroundSlot("Pokemon/Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid3");
            //    default:
            //        return -1;
            //}
            return -1
                //BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceMid011")
                ;
        }

        /// <summary>
        /// 返回近景背景贴图的槽位（如草地、树木等近处背景）。
        /// </summary>
        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            scale = 0.8f; // 让近景背景不缩放
            parallax = 0.0; // 让近景背景不随玩家移动
            //return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose");
            if (++SurfaceFrameCounter > 14) // 每12帧切换一次动画帧
            {
                SurfaceFrame = (SurfaceFrame + 1) % 11;
                SurfaceFrameCounter = 0;
            }
            switch (SurfaceFrame)
            {
                case 0:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose0");
                case 1:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose1");
                case 2:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose2");
                case 3:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose3");
                case 4:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose4");
                case 5:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose5");
                case 6:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose6");
                case 7:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose7");
                case 8:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose8");
                case 9:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose9");
                case 10:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose10");
                case 11:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose11");
                default:
                    return -1;
            }
            //return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/PokemonBiomeSurfaceClose0");
        }
    }
}