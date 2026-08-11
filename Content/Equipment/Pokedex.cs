using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pokemon.Projectiles.TrainerGoldCardProj;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.Elements;
using System;
using Terraria.GameContent;
using ReLogic.Graphics;
using System.Linq;

namespace Pokemon.Content.Equipment
{
    public class Pokedex : ModItem
    {
        private bool isClick = false;

        // 宝可梦图鉴
        public override void SetDefaults()
        {
            Item.width = 48; // 宽度
            Item.height = 44; // 高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Green; // 稀有度
        }

        public override bool CanRightClick()
        {
            if (Main.mouseRight && !isClick)
            {
                if (Main.mouseRightRelease)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        PokedexUI.SetVisible(!PokedexUI.Visible);
                    }
                }
            }
            isClick = Main.mouseRightRelease;
            return false;
        }
        // 修改物品提示信息
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Language.ActiveCulture.Name == "zh-Hans")
            {
                tooltips.Add(new TooltipLine(Mod, "", "一本记载着宝可梦的书"));
                tooltips.Add(new TooltipLine(Mod, "", "【背包生效】"));
                var openTooltip = (new TooltipLine(Mod, "", PokedexUI.Visible ?
                    "右键点击" + "[c/C0522D:关闭]" + "图鉴" : "右键点击" + "[c/C0522D:打开]" + "图鉴"));
                tooltips.Add(openTooltip);
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "", $"Press {BerryPouchSystem.OpenBerryPouchKeybind.GetAssignedKeys().FirstOrDefault() ?? "unbound"} to store items"));
                tooltips.Add(new TooltipLine(Mod, "", "【Inventory Effect】"));
                var openTooltip = (new TooltipLine(Mod, "", PokedexUI.Visible ?
                      "Right-click to " + "[c/C0522D:close]" + " Pokedex" : "Right-click to " + "[c/C0522D::open]" + " Pokedex"));
                tooltips.Add(openTooltip);
            }
        }
        // 在物品栏中绘制物品前的处理
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture;
            // 计算缩放比例和绘制位置

            if (PokedexUI.Visible)
            {
                texture = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/Pokedex_Open").Value;

                float textureScale = Math.Min((float)Item.width / texture.Width, (float)Item.height / texture.Height);
                Vector2 drawPosition = position + new Vector2(Item.width / 2f, Item.height / 2f) - texture.Size() * textureScale / 2f;

                spriteBatch.Draw(texture, drawPosition + new Vector2(-20f, -18f), null, drawColor, 0f, Vector2.Zero, textureScale * 0.72f, SpriteEffects.None, 0f);
            }
            else
            {
                texture = TextureAssets.Item[Item.type].Value;
                spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            }

            return false; // 返回 false 以防止默认绘制
        }
    }
    public class PokedexUI : UIState
    {
        private UIPanel mainPanel;// 主面板
        private UIGrid pokemonGrid;// 宝可梦网格
        private UIScrollbar scrollbar;// 滚动条
        public static bool Visible;// 是否可见

        private List<PokemonData> pokemonList_1;// 宝可梦列表
        private List<PokemonData> pokemonList_2;// 宝可梦列表
        private PokemonData highlightedPokemon;// 高亮宝可梦
        private string hoveredPokemonName;// 鼠标悬停宝可梦名称
        private bool isGridHovered = true;// 是否处于宝可梦网格，是就绘制头文字

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            mainPanel.SetPadding(0);
            mainPanel.Left.Set(630f, 0f);
            mainPanel.Top.Set(180f, 0f);
            mainPanel.Width.Set(500f, 0f);
            mainPanel.Height.Set(700f, 0f);
            Append(mainPanel);

            // 初始化宝可梦数据
            pokemonList_1 = new List<PokemonData>
            {
                new PokemonData("妙蛙种子", "Pokemon/Textures/UI/Pokedex/BulbasaurBadge",
                "妙蛙种子是一种四足的宝可梦，外表类似蟾蜍。\n它有着鲜红色的眼睛，白色的瞳孔和巩膜，头顶上长\n着一对凸起的耳朵。\n\n" +
                "【飞叶快刀】：向四周抖落4片(2级及以上为6片)绿叶,绿\n叶在短暂飘动后变成利刃切割敌人\n\n" +
                "【毒粉】：发射一团携带着毒素的粉末,击中敌人后粉末会\n分散开并持续伤害敌人\n\n" +
                "【寄生种子】：向上方发射4枚种子,种子落地后快速长大\n并释放光团攻击,光团在击中敌人后治疗玩家\n\n" +
                "【守住】：发出一圈叶绿屏障,屏障可以抵挡敌方弹幕,\n使用技能后妙蛙种子会更爱与玩家待在一起"),
                new PokemonData("小火龙", "Pokemon/Textures/UI/Pokedex/CharmanderBadge", "这是小火龙的描述。"),
                new PokemonData("杰尼龟", "Pokemon/Textures/UI/Pokedex/SquirtleBadge", "这是杰尼龟的描述。"),
                new PokemonData("铁哑铃", "Pokemon/Textures/UI/Pokedex/BeldumBadge", "这是铁哑铃的描述。"),
                new PokemonData("鬼斯", "Pokemon/Textures/UI/Pokedex/GastlyBadge", "这是鬼斯的描述。"),
                new PokemonData("跳跳猪", "Pokemon/Textures/UI/Pokedex/SpoinkBadge", "这是跳跳猪的描述。"),
                new PokemonData("向日种子", "Pokemon/Textures/UI/Pokedex/SunflowerBall", "这是向日种子的描述。"),
                new PokemonData("傲骨燕", "Pokemon/Textures/UI/Pokedex/TaillowBadge", "这是傲骨燕的描述。"),
                new PokemonData("长翅鸥", "Pokemon/Textures/UI/Pokedex/WingullBadge", "这是长翅鸥的描述。"),
                new PokemonData("雷电球", "Pokemon/Textures/UI/Pokedex/VoltorbBadge", "这是雷电球的描述。"),
                new PokemonData("小卡比兽", "Pokemon/Textures/UI/Pokedex/MunchlaxBadge", "这是小卡比兽的描述。"),
                new PokemonData("伪螳草", "Pokemon/Textures/UI/Pokedex/FomantisBadge", "这是伪螳草的描述。"),
                new PokemonData("大颚蚁", "Pokemon/Textures/UI/Pokedex/TrapinchBadge", "这是大颚蚁的描述。"),
                new PokemonData("皮卡丘", "Pokemon/Textures/UI/Pokedex/PikachuBadge", "这是皮卡丘的描述。")//13
                // 添加更多宝可梦数据
            };

            // 初始化宝可梦数据
            pokemonList_2 = new List<PokemonData>
            {
                new PokemonData("Bulbasaur", "Pokemon/Textures/UI/Pokedex/BulbasaurBadge",
                "Wonderful frog seeds, seed-type dreams, are born with a \nseed growing on their backs.That kind of child will also \ngrow with Pokémon, so experts can still be sure if henis \na plant or an animal.\n"
                +"[Flying Leaf Blade]: Shake off 4 pieces (6 pieces for level \n2 and above) green leaves in all directions, and the green \nleaves will turn into sharp blades to cut enemies after a \nshort flutter.\n"
                +"[Poison Powder]: Fires a cloud of powder carrying toxins, \nwhich will disperse and damage enemies over time \nupon impact.\n"
                +"[Parasitic Seeds]: Shoots 4 seeds upwards, the seeds grow \nrapidly when they landnand and unleash a light attack, the \nlight group heals the player after hitting the enemy.\n"
                +"[Hold]: Send out a ring of chlorophyte barriers, the barrier \ncan resist the enemy barrage, after using the skill, the Frog \nSeed will prefer to stay with the player"),
                new PokemonData("Charmander", "Pokemon/Textures/UI/Pokedex/CharmanderBadge", "This is the description of Charmander."),
                new PokemonData("Squirtle", "Pokemon/Textures/UI/Pokedex/SquirtleBadge", "This is the description of Squirtle."),
                new PokemonData("Beldum", "Pokemon/Textures/UI/Pokedex/BeldumBadge", "This is the description of Beldum."),
                new PokemonData("Gastly", "Pokemon/Textures/UI/Pokedex/GastlyBadge", "This is the description of Gastly."),
                new PokemonData("Spoink", "Pokemon/Textures/UI/Pokedex/SpoinkBadge", "This is the description of Spoink."),
                new PokemonData("Sunflower", "Pokemon/Textures/UI/Pokedex/SunflowerBall", "This is the description of Sunflower."),
                new PokemonData("Taillow", "Pokemon/Textures/UI/Pokedex/TaillowBadge", "This is the description of Taillow."),
                new PokemonData("Wingull", "Pokemon/Textures/UI/Pokedex/WingullBadge", "This is the description of Wingull."),
                new PokemonData("Voltorb", "Pokemon/Textures/UI/Pokedex/VoltorbBadge", "This is the description of Voltorb."),
                new PokemonData("Munchlax", "Pokemon/Textures/UI/Pokedex/MunchlaxBadge", "This is the description of Munchlax."),
                new PokemonData("Fomantis", "Pokemon/Textures/UI/Pokedex/FomantisBadge", "This is the description of Fomantis."),
                new PokemonData("Trapinch", "Pokemon/Textures/UI/Pokedex/TrapinchBadge", "This is the description of Trapinch."),
                new PokemonData("Pikachu", "Pokemon/Textures/UI/Pokedex/PikachuBadge", "This is the description of Pikachu.")//14
                // 添加更多宝可梦数据
            };

            // 创建网格布局
            pokemonGrid = new UIGrid();
            pokemonGrid.Width.Set(0, 1f);
            pokemonGrid.Height.Set(620, 0f);
            pokemonGrid.Top.Set(60f, 0f);
            pokemonGrid.Left.Set(20f, 0f);
            pokemonGrid.ListPadding = 10f;
            mainPanel.Append(pokemonGrid);

            // 创建滚动条
            scrollbar = new UIScrollbar();
            scrollbar.SetView(100f, 1000f); // 设置滚动条视图
            scrollbar.Height.Set(608f, 0f);
            scrollbar.Left.Set(470f, 0f);
            scrollbar.Top.Set(66f, 0f);
            mainPanel.Append(scrollbar);
            pokemonGrid.SetScrollbar(scrollbar);

            if(Language.ActiveCulture.Name == "zh-Hans")
                // 添加宝可梦缩略图到网格
                foreach (var pokemon in pokemonList_1)
                {
                    // 创建缩略图面板
                    var pokemonThumbnail = new UIPanel();
                    pokemonThumbnail.Width.Set(80f, 0f);
                    pokemonThumbnail.Height.Set(80f, 0f);
                    pokemonThumbnail.SetPadding(10);// 内边距

                    // 显示宝可梦名称
                    var pokemonImage = new UIImage(ModContent.Request<Texture2D>(pokemon.ImagePath));
                    pokemonImage.Top.Set(-45f, 0f);
                    pokemonImage.Left.Set(-45f, 0f);

                    // 限制物品图标大小
                    float scale = Math.Min(0.5f, 80f / (((Texture2D)ModContent.Request<Texture2D>(pokemon.ImagePath)).Width + ((Texture2D)ModContent.Request<Texture2D>(pokemon.ImagePath)).Height) * 2); // 48f 是物品图标大小的最大限制
                    pokemonImage.ImageScale = scale; // 缩放比例

                    pokemonThumbnail.Append(pokemonImage);

                    pokemonThumbnail.OnLeftClick += (evt, element) =>
                    {
                        ShowPokemonDetails(pokemon);
                    };
                    pokemonThumbnail.OnMouseOver += (evt, element) =>
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick); // 播放音效
                        hoveredPokemonName = pokemon.Name;
                        pokemonThumbnail.BackgroundColor = new Color(73, 94, 171); // 更改背景颜色
                    };
                    pokemonThumbnail.OnMouseOut += (evt, element) =>
                    {
                        hoveredPokemonName = null;
                        pokemonThumbnail.BackgroundColor = new Color(63, 82, 151); // 恢复背景颜色
                    };
                    pokemonGrid.Add(pokemonThumbnail);
                }
            else
                foreach (var pokemon in pokemonList_2)
                {
                    // 创建缩略图面板
                    var pokemonThumbnail = new UIPanel();
                    pokemonThumbnail.Width.Set(80f, 0f);
                    pokemonThumbnail.Height.Set(80f, 0f);
                    pokemonThumbnail.SetPadding(10);// 内边距

                    // 显示宝可梦名称
                    var pokemonImage = new UIImage(ModContent.Request<Texture2D>(pokemon.ImagePath));
                    pokemonImage.Top.Set(-45f, 0f);
                    pokemonImage.Left.Set(-45f, 0f);

                    // 限制物品图标大小
                    float scale = Math.Min(0.5f, 80f / (((Texture2D)ModContent.Request<Texture2D>(pokemon.ImagePath)).Width + ((Texture2D)ModContent.Request<Texture2D>(pokemon.ImagePath)).Height) * 2); // 48f 是物品图标大小的最大限制
                    pokemonImage.ImageScale = scale; // 缩放比例

                    pokemonThumbnail.Append(pokemonImage);

                    pokemonThumbnail.OnLeftClick += (evt, element) =>
                    {
                        ShowPokemonDetails(pokemon);
                    };
                    pokemonThumbnail.OnMouseOver += (evt, element) =>
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick); // 播放音效
                        hoveredPokemonName = pokemon.Name;
                        pokemonThumbnail.BackgroundColor = new Color(73, 94, 171); // 更改背景颜色
                    };
                    pokemonThumbnail.OnMouseOut += (evt, element) =>
                    {
                        hoveredPokemonName = null;
                        pokemonThumbnail.BackgroundColor = new Color(63, 82, 151); // 恢复背景颜色
                    };
                    pokemonGrid.Add(pokemonThumbnail);
                }
            // 添加关闭按钮
            var closeButton = new UIImageButton(ModContent.Request<Texture2D>("Pokemon/Textures/UI/CoolDown"));
            closeButton.Left.Set(466f, 0f); // 设置按钮位置
            closeButton.Top.Set(10f, 0f);
            closeButton.OnLeftClick += (evt, element) => SetVisible(false); // 点击关闭UI
            mainPanel.Append(closeButton);
        }

        public static void SetVisible(bool visible)
        {
            Visible = visible;
            if (visible)
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuOpen); // 播放打开音效
            }
            else
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuClose); // 播放关闭音效
            }
        }

        private void ShowPokemonDetails(PokemonData pokemon)
        {
            // 清空网格并显示宝可梦详细信息
            mainPanel.RemoveAllChildren();
            isGridHovered = false;

            var pokemonName = new UIText(pokemon.Name);
            pokemonName.Left.Set(215f, 0f);
            pokemonName.Top.Set(10f, 0f);
            mainPanel.Append(pokemonName);

            var pokemonImage = new UIImage(ModContent.Request<Texture2D>(pokemon.ImagePath));
            pokemonImage.Left.Set(170f, 0f);
            pokemonImage.Top.Set(60f, 0f);
            mainPanel.Append(pokemonImage);

            var pokemonDescription = new UIText(pokemon.Description);
            pokemonDescription.Left.Set(20f, 0f);
            pokemonDescription.Top.Set(200f, 0f);
            mainPanel.Append(pokemonDescription);

            var backButton = new UITextButton(Language.ActiveCulture.Name == "zh-Hans" ? "返回" : "Back");
            backButton.Left.Set(210f, 0f);
            backButton.Top.Set(640f, 0f);
            backButton.OnLeftClick += (evt, element) =>
            {
                ShowPokemonGrid();
            };
            mainPanel.Append(backButton);
        }

        public void ShowPokemonGrid()
        {
            // 清空详细信息并重新显示网格
            mainPanel.RemoveAllChildren();
            mainPanel.Append(pokemonGrid);
            mainPanel.Append(scrollbar); // 重新添加滚动条
            PokemonModSystemPokedex pokemonModSystemPokedex = new PokemonModSystemPokedex();
            pokemonModSystemPokedex.needsResetToGrid = true;

            // 重新添加关闭按钮
            var closeButton = new UIImageButton(ModContent.Request<Texture2D>("Pokemon/Textures/UI/CoolDown"));
            closeButton.Left.Set(466f, 0f); // 设置按钮位置
            closeButton.Top.Set(10f, 0f);
            closeButton.OnLeftClick += (evt, element) => SetVisible(false); // 点击关闭UI
            mainPanel.Append(closeButton);
            //显示图鉴文字
            isGridHovered = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
            {
                return;
            }

            base.Draw(spriteBatch);

            // 绘制白色直线
            Texture2D whitePixel = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White * 0.4f });
            spriteBatch.Draw(whitePixel, new Rectangle((int)mainPanel.Left.Pixels, (int)mainPanel.Top.Pixels + 40, (int)mainPanel.Width.Pixels, 2), Color.White);

            if (isGridHovered)
            {
                var title = Language.ActiveCulture.Name == "zh-Hans" ? "宝可梦图鉴" : "Pokédex";
                var font = FontAssets.MouseText.Value;
                var textSize = font.MeasureString(title);
                // 添加分隔符提示并动态变化颜色
                float lineLerpFactor = (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) / 2; // 动态变化颜色

                // 动态变化颜色
                Color color = Color.Lerp(Color.LightSteelBlue, Color.White, lineLerpFactor); // 这里只做了粉色到白色的变化

                spriteBatch.DrawString(font, title, new Vector2(mainPanel.Left.Pixels + 20, mainPanel.Top.Pixels + 10), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
                
            if (!string.IsNullOrEmpty(hoveredPokemonName))
            {
                Vector2 mousePosition = new Vector2(Main.mouseX + 20, Main.mouseY + 20);
                Utils.DrawBorderString(spriteBatch, hoveredPokemonName, mousePosition, Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Visible)
            {
                return;
            }

            base.Update(gameTime);
            // 检查鼠标是否在UI面板内
            if (mainPanel.IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            if (!Main.playerInventory)
            {
                // 若背包未打开，则关闭UI
                SetVisible(false);
            }
        }
    }
   
    public class PokemonModSystemPokedex : ModSystem
    {
        private UserInterface pokedexInterface;
        private PokedexUI pokedexUI;
        public bool needsResetToGrid = false;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                pokedexUI = new PokedexUI();
                pokedexUI.Activate();
                pokedexInterface = new UserInterface();
                pokedexInterface.SetState(pokedexUI);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (PokedexUI.Visible)
            {
                pokedexInterface?.Update(gameTime); // 更新UI
                needsResetToGrid = false;
            }
            else if (!needsResetToGrid)
            {
                pokedexUI.ShowPokemonGrid();
            }
        }
       
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                    "Pokemon: Pokedex UI",
                    delegate
                    {
                        if (PokedexUI.Visible)
                        {
                            pokedexInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
    public class PokemonData
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }

        public PokemonData(string name, string imagePath, string description)
        {
            Name = name;
            ImagePath = imagePath;
            Description = description;
        }
    }
}

