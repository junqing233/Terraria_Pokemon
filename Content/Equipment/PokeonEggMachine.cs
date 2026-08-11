using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.Equipment;
using Pokemon.Content.Items;
using Pokemon.Content.Props;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.Graphics.Effects;
using Pokemon.Content.Tools;
using Pokemon.Content.Accessories;
using ReLogic.Graphics;
using System.Linq;
using System;
using Terraria.GameContent;
using ReLogic.Content;


namespace Pokemon.Content.Equipment
{
    class PokeonEggMachine : ModTile
    {
        public override string Texture => "Pokemon/Content/Equipment/PokeonEggMachine";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            AnimationFrameHeight = 54;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(200, 200, 200), Language.GetText(Language.ActiveCulture.Name == "zh-Hans" ? "宝可梦扭蛋" : "Pokemon Egg Hatching Machine"));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemID.None;
            player.mouseInterface = true;

            //Player player = Main.LocalPlayer; // 获取本地玩家
            player.noThrow = 2; // 禁止投掷
            player.cursorItemIconEnabled = true;// 显示物品图标
            player.cursorItemIconID = ItemID.None; // 物品图标ID
            player.mouseInterface = true; // 鼠标接口开启

            // 我们可以通过获取方块样式并查找对应的物品掉落来确定光标上显示的物品。
            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
        }

        public override bool RightClick(int i, int j)
        {
            Vector2 tilePosition = new Vector2(i * 16, j * 16); // 瓦片的位置
            ModContent.GetInstance<EggMachineUISystem>().ToggleUI(tilePosition);
            return true;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if(ModContent.GetInstance<EggMachineUISystem>() != null)
            {
                if (ModContent.GetInstance<EggMachineUISystem>().IsUIVisible())
                {
                    frame = 1; // 第二帧
                }
                else
                {
                    frame = 0; // 第一帧
                }
            }
            
        }
    }

    class PokeonEggMachineItem : ModItem
    {
        public override string Texture => "Pokemon/Content/Equipment/PokeonEggMachineItem";

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<PokeonEggMachine>();
        }
    }
    
    public class EggMachineUI : UIState
    {
        public UIPanel mainPanel;
        private UITextPanel<string> buyPokemonButton;
        private UITextPanel<string> buyItemButton;
        private UITextPanel<string> buyVanillaItemButton; // 新增按钮
        private UITextPanel<string> buyModItemButton; // 新增按钮
        private UITextPanel<string> modSwitchButton; // 新增按钮
        private Vector2 tilePosition;
        private string hoverText; // 用于存储提示词
        private List<string> loadedModsWithItems; // 存储加载的模组名称
        private int currentModIndex; // 当前模组索引
        private bool isOverModItemButton = false; // 新增变量，用于判断是否鼠标悬停在模组物品按钮上
        private bool isOverModSwitchButton = false; // 新增变量，用于判断是否鼠标悬停在模组切换按钮上

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            mainPanel.SetPadding(10);
            mainPanel.Left.Set(650f, 0f);
            mainPanel.Top.Set(200f, 0f);
            mainPanel.Width.Set(460f, 0f);
            mainPanel.Height.Set(220f, 0f);
            Append(mainPanel);

            buyPokemonButton = new UITextPanel<string>(Language.ActiveCulture.Name == "zh-Hans" ? "抽取宝可梦" : "Catch Pokemon", 1f);
            buyPokemonButton.Width.Set(80f, 0f);
            buyPokemonButton.Height.Set(50f, 0f);
            buyPokemonButton.Top.Set(140f, 0f);
            buyPokemonButton.Left.Set(20f, 0f);
            buyPokemonButton.OnLeftClick += BuyPokemonButton_OnClick;
            buyPokemonButton.OnMouseOver += BuyPokemonButton_OnMouseOver;
            buyPokemonButton.OnMouseOut += BuyPokemonButton_OnMouseOut;
            mainPanel.Append(buyPokemonButton);

            buyItemButton = new UITextPanel<string>(Language.ActiveCulture.Name == "zh-Hans" ? "抽取道具" : "Catch Pokemon Item", 1f);
            buyItemButton.Width.Set(80f, 0f);
            buyItemButton.Height.Set(50f, 0f);
            buyItemButton.Top.Set(140f, 0f);
            buyItemButton.Left.Set(160f, 0f);
            buyItemButton.OnLeftClick += BuyItemButton_OnClick;
            buyItemButton.OnMouseOver += BuyItemButton_OnMouseOver;
            buyItemButton.OnMouseOut += BuyItemButton_OnMouseOut;
            mainPanel.Append(buyItemButton);

            buyVanillaItemButton = new UITextPanel<string>(Language.ActiveCulture.Name == "zh-Hans" ? "抽取原版物品" : "Catch Vanilla Item", 1f); // 新增按钮
            buyVanillaItemButton.Width.Set(80f, 0f);
            buyVanillaItemButton.Height.Set(50f, 0f);
            buyVanillaItemButton.Top.Set(140f, 0f);
            buyVanillaItemButton.Left.Set(280f, 0f);
            buyVanillaItemButton.OnLeftClick += BuyVanillaItemButton_OnClick;
            buyVanillaItemButton.OnMouseOver += BuyVanillaItemButton_OnMouseOver;
            buyVanillaItemButton.OnMouseOut += BuyVanillaItemButton_OnMouseOut;
            mainPanel.Append(buyVanillaItemButton);

            buyModItemButton = new UITextPanel<string>(Language.ActiveCulture.Name == "zh-Hans" ? "抽取模组物品" : "Catch Mod Item", 1f); // 新增按钮
            buyModItemButton.Width.Set(80f, 0f);
            buyModItemButton.Height.Set(50f, 0f);
            buyModItemButton.Top.Set(10f, 0f); // 设置在 buyItemButton 上方 40 像素处
            buyModItemButton.Left.Set(80f, 0f);
            buyModItemButton.OnLeftClick += BuyModItemButton_OnClick;
            buyModItemButton.OnMouseOver += BuyModItemButton_OnMouseOver;
            buyModItemButton.OnMouseOut += BuyModItemButton_OnMouseOut;
            mainPanel.Append(buyModItemButton);

            modSwitchButton = new UITextPanel<string>(Language.ActiveCulture.Name == "zh-Hans" ? "未加载模组" : "No Mod Loaded", 1f); // 新增按钮
            modSwitchButton.Width.Set(120f, 0f);
            modSwitchButton.Height.Set(50f, 0f);
            modSwitchButton.Top.Set(10f, 0f); // 设置在 buyModItemButton 上方 40 像素处
            modSwitchButton.Left.Set(220f, 0f);
            modSwitchButton.OnRightClick += ModSwitchButton_OnRightClick;
            modSwitchButton.OnMouseOver += ModSwitchButton_OnMouseOver;
            modSwitchButton.OnMouseOut += ModSwitchButton_OnMouseOut;
            mainPanel.Append(modSwitchButton);

            // 初始化加载的模组列表
            loadedModsWithItems = new List<string>();
            currentModIndex = 0;
            LoadModsWithItems();
        }
        private void LoadModsWithItems()
        {
            foreach (var mod in ModLoader.Mods)
            {
                if (mod.GetContent<ModItem>().Any() )
                {
                    loadedModsWithItems.Add(mod.Name);
                }
            }

            if (loadedModsWithItems.Count > 0)
            {
                modSwitchButton.SetText(loadedModsWithItems[currentModIndex]);
            }
            else
            {
                modSwitchButton.SetText(Language.ActiveCulture.Name == "zh-Hans" ? "未加载模组" : "No Mod Loaded");
            }
        }

        private void ModSwitchButton_OnRightClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (loadedModsWithItems.Count > 0)
            {
                currentModIndex = (currentModIndex + 1) % loadedModsWithItems.Count;
                modSwitchButton.SetText(loadedModsWithItems[currentModIndex]);
            }
        }
        public void SetTilePosition(Vector2 position)
        {
            tilePosition = position;
        }

        private void BuyPokemonButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Player player = Main.LocalPlayer;
            int ticketType = ModContent.ItemType<PekemonTicket>();
            int ticketCount = player.CountItem(ticketType);

            if (ticketCount > 0) // 检查玩家是否有宝可梦奖券
            {
                player.ConsumeItem(ticketType); // 消耗一个宝可梦奖券

                // 定义可能的物品ID数组
                int[] possibleItems = new int[]
                {
                    ModContent.ItemType<SunflowerBall>(),//向日种子
                    ModContent.ItemType<BulbasaurBadge>(),//妙蛙种子
                    ModContent.ItemType<CharmanderBadge>(),//小火龙
                    ModContent.ItemType<GastlyBadge>(),// 鬼斯
                    ModContent.ItemType<SquirtleBadge>(),//杰尼龟
                    ModContent.ItemType<TaillowBadge>(),//傲骨燕
                    ModContent.ItemType<SpoinkBadge>(),//跳跳猪
                    ModContent.ItemType<BeldumBadge>(),//铁哑铃
                    ModContent.ItemType<WingullBadge>(),//长翅鸥
                    ModContent.ItemType<VoltorbBadge>(),//雷电球
                    ModContent.ItemType<MunchlaxBadge>(),//小卡比兽
                    ModContent.ItemType<FomantisBadge>(),//伪螳草
                    ModContent.ItemType<TrapinchBadge>(),//大颚蚁
                    ModContent.ItemType<PikachuBadge>(),//皮卡丘
                };

                // 从数组中随机选择一个物品ID
                int selectedItem = possibleItems[Main.rand.Next(possibleItems.Length)];

                Vector2 itemPosition = tilePosition - new Vector2(0, 8); // 瓦片中心
                Item.NewItem(null, itemPosition, 1, 1, selectedItem, 1);
            }
            else
            {
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y + 30, player.width, player.height),
                    Color.Gold * 0.72f, Language.ActiveCulture.Name == "zh-Hans" ? "你的宝可梦奖卷不够喵！" : "Your Pokémon Ticket isn't enough!"); // 显示文本提示
            }
        }

        private void BuyItemButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Player player = Main.LocalPlayer;
            int ticketType = ModContent.ItemType<PropTicket>();
            int ticketCount = player.CountItem(ticketType);

            if (ticketCount > 0) // 检查玩家是否有道具奖券
            {
                player.ConsumeItem(ticketType); // 消耗一个道具奖券

                // 定义可能的物品ID数组和对应的概率
                (int itemType, float probability)[] primaryItems = new (int, float)[]
                {
                    (ModContent.ItemType<BerryPouch>(), 0.02f),//树果袋
                    (ModContent.ItemType<BoulderBadge>(), 0.08f),//灰色徽章
                    (ModContent.ItemType<CascadeBadge>(), 0.08f),//蓝色徽章
                    (ModContent.ItemType<EarthBadge>(), 0.08f),//绿色徽章
                    (ModContent.ItemType<Expshare>(), 0.05f),//学习装置
                    (ModContent.ItemType<ForcedExerciser>(), 0.02f),//强制锻炼器
                    (ModContent.ItemType<MagicCandy>(), 0.08f),//神奇糖果
                    (ModContent.ItemType<MarshBadge>(), 0.08f),//粉红徽章
                    (ModContent.ItemType<RainbowBadge>(), 0.08f),//彩虹徽章
                    (ModContent.ItemType<SoulBadge>(), 0.08f),//黄金徽章
                    (ModContent.ItemType<ThunderBadge>(), 0.08f),//橘色徽章
                    (ModContent.ItemType<VolcanoBadge>(), 0.08f),//深红徽章
                    (ModContent.ItemType<PropTicket>(),0.01f),//宝可梦奖券
                    (ModContent.ItemType<Nugget>(),0.08f),//金色珠
                    (ModContent.ItemType<WrapTightlyHookItem>(),0.04f),//紧缠钩爪
                    (ModContent.ItemType<Leftovers>(),0.06f)//剩饭
                };

                (int itemType, float probability)[] secondaryItems = new (int, float)[]
                {
                    (ItemID.GoodieBag, 0.0625f),//礼袋
                    (ItemID.Present, 0.0625f),//礼物
                    (ItemID.BluePresent, 0.0625f),//蓝礼物
                    (ItemID.GreenPresent, 0.0625f),//绿礼物
                    (ItemID.YellowPresent, 0.0625f),//黄礼物
                    (ItemID.HerbBag, 0.0625f),//草药袋
                    (ItemID.SmokeBomb,0.0625f),//烟雾弹
                    (ItemID.Football,0.0625f),//橄榄球
                    (ItemID.DefenderMedal,0.0625f),//护卫奖章
                    (ItemID.PoopBlock,0.0625f),//臭臭
                    (ItemID.Pigronata,0.0625f),//猪龙彩罐
                    (ItemID.PlatinumCoin, 0.0625f),//铂金币
                    (ItemID.GoldCoin,0.0625f),//金币
                    (ModContent.ItemType<PropTicket>(),0.0625f),//道具奖券
                    (ItemID.SilverCoin,0.0625f),//银币
                    (ItemID.CopperCoin,0.0625f)//铜币
                };

                (int itemType, float probability)[] tertiaryItems = new (int, float)[]
                {
                    (2334, 0.08f),//木匣//前
                    (2335, 0.08f),//铁匣//前
                    (2336, 0.08f),//金匣//前
                    (3208, 0.07f),//丛林匣//前
                    (3206, 0.06f),//天空匣//前
                    (3203, 0.07f),//腐化匣//前
                    (3204, 0.07f),//猩红匣//前
                    (3207, 0.06f),//神圣匣//前
                    (3205,0.06f),//地牢匣//前
                    (4405,0.06f),//冰冻匣//前
                    (3982,0.001f),//污损匣//后
                    (3985,0.001f),//天蓝匣//后
                    (3981,0.001f),//钛金匣//后
                    (3980,0.001f),//秘银匣//后
                    (3979,0.001f),//珍珠木匣//后
                    (5002,0.06f),//海洋匣//前
                    (4877,0.06f),//黑曜石匣//前
                    (4407,0.06f),//绿洲匣//前
                    (5003,0.001f),//海边匣//后
                    (4878,0.001f),//狱石匣//后
                    (4408,0.001f),//幻象匣//后
                    (4406,0.001f),//针木匣//后
                    (3984,0.001f),//围栏匣//后
                    (3986,0.001f),//天赐匣//后
                    (3983,0.001f),//血匣//后
                    (3987,0.001f)//荆棘匣//后
                };

                // 根据概率随机选择一个大类
                float primaryProbability = 0.2f;
                float secondaryProbability = 0.6f;

                float randomValue = Main.rand.NextFloat();
                int selectedItem;

                if (randomValue < primaryProbability)//自定义道具类
                {
                    // 从 primaryItems 中随机选择一个物品ID
                    selectedItem = SelectRandomItem(primaryItems);
                    Vector2 finalItemPosition = tilePosition - new Vector2(0, 8); // 瓦片中心
                    Item.NewItem(null, finalItemPosition, 1, 1, selectedItem, 1);
                }
                else if (randomValue < primaryProbability + secondaryProbability)//其他道具类
                {
                    // 从 secondaryItems 中随机选择 2 到 4 个物品ID
                    int itemCount = Main.rand.Next(2, 5);
                    for (int i = 0; i < itemCount; i++)
                    {
                        selectedItem = SelectRandomItem(secondaryItems);
                        Vector2 itemPosition = tilePosition - new Vector2(0, 8); // 瓦片中心
                        Item.NewItem(null, itemPosition, 1, 1, selectedItem, 1);
                    }
                }
                else /*if(NPC.downedFishron || NPC.downedMartians || NPC.down)*///血肉之墙已被击败
                //宝匣类
                {
                    // 从 tertiaryItems 中随机选择 1 到 2 个物品ID
                    int itemCount = Main.rand.Next(1, 3);
                    for (int i = 0; i < itemCount; i++)
                    {
                        selectedItem = SelectRandomItem(tertiaryItems);
                        Vector2 itemPosition = tilePosition - new Vector2(0, 8); // 瓦片中心
                        Item.NewItem(null, itemPosition, 1, 1, selectedItem, 1);
                    }
                }
            }
            else
            {
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y + 30, player.width, player.height),
                    Color.SkyBlue * 0.8f, Language.ActiveCulture.Name == "zh-Hans" ? "你的道具奖卷不够喵！" : "Your Prop Ticket isn't enough!"); // 显示文本提示); // 显示文本提示
            }
        }

        private void BuyVanillaItemButton_OnClick(UIMouseEvent evt, UIElement listeningElement) // 新增方法
        {
            Player player = Main.LocalPlayer;
            if (player.BuyItem(100000)) // 10金币
            {
                // 从1到5452中随机选择一个物品ID
                int selectedItem = Main.rand.Next(1, 5453);

                Vector2 itemPosition = tilePosition - new Vector2(0, 8); // 瓦片中心
                Item.NewItem(null, itemPosition, 1, 1, selectedItem, 1);
            }
            else
            {
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y + 30, player.width, player.height),
                    Color.Red * 0.8f, Language.ActiveCulture.Name == "zh-Hans" ? "你的金币不够喵！" : "Your money isn't enough!"); // 显示文本提示
            }
        }
        private void BuyModItemButton_OnClick(UIMouseEvent evt, UIElement listeningElement) // 新增方法
        {
            Player player = Main.LocalPlayer;
            int ticketType = ModContent.ItemType<ModTicket>();
            int ticketCount = player.CountItem(ticketType);

            if (loadedModsWithItems.Count > 0 && ticketCount > 0)
            {
                player.ConsumeItem(ticketType); // 消耗一个模组奖券
                string currentModName = loadedModsWithItems[currentModIndex];
                Mod currentMod = ModLoader.GetMod(currentModName);

                if (currentMod != null)
                {
                    // 获取当前模组中所有物品的编号
                    int minItemID = int.MaxValue;
                    int maxItemID = int.MinValue;

                    foreach (var item in currentMod.GetContent<ModItem>())
                    {
                        int itemType = item.Type;
                        if (itemType < minItemID)
                        {
                            minItemID = itemType;
                        }
                        if (itemType > maxItemID)
                        {
                            maxItemID = itemType;
                        }
                    }

                    // 在这些编号范围内随机选择一个物品
                    int selectedItem = Main.rand.Next(minItemID, maxItemID + 1);

                    Vector2 itemPosition = tilePosition - new Vector2(0, 8); // 瓦片中心
                    Item.NewItem(null, itemPosition, 1, 1, selectedItem, 1);
                }
                else
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height),
                        Color.Red * 0.8f, Language.ActiveCulture.Name == "zh-Hans" ? $"模组 {currentModName} 未加载！" : $"Mod {currentModName} is not loaded!"); // 显示文本提示
                }
            }
            else
            {
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y + 30, player.width, player.height),
                    Color.MediumPurple * 0.8f, Language.ActiveCulture.Name == "zh-Hans" ? "你的模组奖卷不够喵！" : "Your Mod Ticket isn't enough!"); // 显示文本提示); // 显示文本提示
            }
        }
        private int SelectRandomItem((int itemType, float probability)[] items)
        {
            float totalProbability = 0f;
            foreach (var item in items)
            {
                totalProbability += item.probability;
            }

            float randomValue = Main.rand.NextFloat() * totalProbability;
            foreach (var item in items)
            {
                if (randomValue < item.probability)
                {
                    return item.itemType;
                }
                randomValue -= item.probability;
            }

            return items[0].itemType; // 默认返回第一个物品
        }

        private void BuyPokemonButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            buyPokemonButton.BorderColor = Color.Yellow * 0.8f;
            hoverText = Language.ActiveCulture.Name == "zh-Hans" ? "消耗一张宝可梦奖券" : "Consume a Pokémon Ticket"; // 设置提示词
        }

        private void BuyPokemonButton_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            buyPokemonButton.BorderColor = Color.Black;
            hoverText = null; // 清除提示词
        }
        private void BuyModItemButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement) // 新增方法
        {
            buyModItemButton.BorderColor = Color.Yellow * 0.8f;
            hoverText = Language.ActiveCulture.Name == "zh-Hans" ? "消耗一张模组奖券" : "Consume a Mod Ticket"; // 设置提示词
            isOverModItemButton = true;
        }
        private void BuyModItemButton_OnMouseOut(UIMouseEvent evt, UIElement listeningElement) // 新增方法
        {
            buyModItemButton.BorderColor = Color.Black;
            hoverText = null; // 清除提示词
            isOverModItemButton = false;
        }
        private void BuyItemButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            buyItemButton.BorderColor = Color.Yellow * 0.8f;
            hoverText = Language.ActiveCulture.Name == "zh-Hans" ? "消耗一张道具奖券" : "Consume a Prop Ticket"; // 设置提示词
        }

        private void BuyItemButton_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            buyItemButton.BorderColor = Color.Black;
            hoverText = null; // 清除提示词
        }

        private void BuyVanillaItemButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement) // 新增方法
        {
            buyVanillaItemButton.BorderColor = Color.Yellow * 0.8f;
            hoverText = Language.ActiveCulture.Name == "zh-Hans" ? "消耗10金币" : "Consume 10 Gold Coins"; // 设置提示词
        }

        private void BuyVanillaItemButton_OnMouseOut(UIMouseEvent evt, UIElement listeningElement) // 新增方法
        {
            buyVanillaItemButton.BorderColor = Color.Black;
            hoverText = null; // 清除提示词
        }
        private void ModSwitchButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            modSwitchButton.BorderColor = Color.Yellow * 0.8f;
            hoverText = Language.ActiveCulture.Name == "zh-Hans" ? "右键切换模组" : "Right-click to switch mod"; // 设置提示词
            isOverModSwitchButton = true;
        }

        private void ModSwitchButton_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            modSwitchButton.BorderColor = Color.Black;
            hoverText = null; // 清除提示词
            isOverModSwitchButton = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // 检查鼠标是否在UI面板内
            if (mainPanel.IsMouseHovering)
            {
                Player player = Main.LocalPlayer;
                player.mouseInterface = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            // 获取UI面板的左边中点和右边中点
            Vector2 leftCenter1 = new Vector2(mainPanel.Left.Pixels, mainPanel.Top.Pixels + mainPanel.Height.Pixels / 2);
            Vector2 rightCenter1 = new Vector2(mainPanel.Left.Pixels + mainPanel.Width.Pixels, mainPanel.Top.Pixels + mainPanel.Height.Pixels / 2);
            Vector2 leftCenter2 = new Vector2(mainPanel.Left.Pixels, mainPanel.Top.Pixels + mainPanel.Height.Pixels / 2 + 10);
            Vector2 rightCenter2 = new Vector2(mainPanel.Left.Pixels + mainPanel.Width.Pixels, mainPanel.Top.Pixels + mainPanel.Height.Pixels / 2 + 10);
            Vector2 leftCenter3 = new Vector2(mainPanel.Left.Pixels, mainPanel.Top.Pixels + mainPanel.Height.Pixels / 2 - 10);
            Vector2 rightCenter3 = new Vector2(mainPanel.Left.Pixels + mainPanel.Width.Pixels, mainPanel.Top.Pixels + mainPanel.Height.Pixels / 2 - 10);

            // 绘制红线
            DrawLine(spriteBatch, leftCenter3, rightCenter3, Color.Red);
            // 绘制红线
            DrawLine(spriteBatch, leftCenter2, rightCenter2, Color.Red);
            // 绘制白线
            DrawLine(spriteBatch, leftCenter1, rightCenter1, Color.White);

            if (isOverModSwitchButton)
                // 绘制当前选择的模组图标
                if (loadedModsWithItems.Count > 0)
                {
                    string currentModName = loadedModsWithItems[currentModIndex];
                    Mod currentMod = ModLoader.GetMod(currentModName);
                    if (currentMod != null && currentModName != "ModLoader")
                    {
                        // 尝试从模组的资源中加载图标
                        Texture2D modIcon = null;
                        try//尝试从模组的资源中加载图标
                        {
                            modIcon = currentMod.Assets.Request<Texture2D>("icon", AssetRequestMode.ImmediateLoad).Value;
                        }
                        catch//如果加载失败
                        {
                            // 如果加载失败，可以使用一个默认图标或忽略
                        }

                        if (modIcon != null)
                        {
                            Vector2 mousePosition = Main.MouseScreen;
                            Vector2 drawPosition;
                            drawPosition = mousePosition + new Vector2(-58f, -84f);
                            spriteBatch.Draw(modIcon, drawPosition, new Rectangle(0, 0, modIcon.Width, modIcon.Height), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        }
                    }
                }

            if (!string.IsNullOrEmpty(hoverText))
            {
                Vector2 mousePosition = Main.MouseScreen;
                Vector2 textSize = Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(hoverText);
                Vector2 drawPosition;
                if (isOverModItemButton || isOverModSwitchButton)//判断是否移入模组按钮
                    drawPosition = mousePosition + new Vector2(-16f, -64f);
                else
                    drawPosition = mousePosition + new Vector2(-16f, 48f);
                spriteBatch.DrawString(Terraria.GameContent.FontAssets.MouseText.Value, hoverText, drawPosition, Color.White);
            }

            // 绘制UI图标
            Texture2D PokeonEggMachineUI = ModContent.Request<Texture2D>("Pokemon/Textures/UI/PokeonEggMachine/PokeonEggMachineUI").Value;
            spriteBatch.Draw(PokeonEggMachineUI, new Vector2((int)mainPanel.Left.Pixels + 200, (int)mainPanel.Top.Pixels + 86), 
                new Rectangle(0, 0, PokeonEggMachineUI.Width, PokeonEggMachineUI.Height), Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

        }
        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 2), null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }

    public class EggMachineUISystem : ModSystem
    {
        private UserInterface eggMachineInterface;
        internal EggMachineUI eggMachineUI;
        private Vector2 tilePosition;
        private const float MaxDistance = 100f; // 最大距离

        public override void Load()
        {
            if (!Main.dedServ)
            {
                eggMachineUI = new EggMachineUI();
                eggMachineInterface = new UserInterface();
                tilePosition = Vector2.Zero; // 初始化 tilePosition
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (eggMachineInterface?.CurrentState != null)
            {
                eggMachineInterface.Update(gameTime);

                // 检查玩家与瓦片的距离
                Player player = Main.LocalPlayer;
                if (tilePosition != Vector2.Zero) // 确保 tilePosition 已被正确设置
                {
                    float distance = Vector2.Distance(player.Center, tilePosition);
                    if (distance > MaxDistance)
                    {
                        ToggleUI(); // 关闭UI面板
                    }
                }
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                    "Pokemon: Egg Machine UI",
                    delegate
                    {
                        if (eggMachineInterface?.CurrentState != null)
                        {
                            eggMachineInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public void ToggleUI(Vector2? position = null)
        {
            // 打开或关闭UI面板
            if (eggMachineInterface.CurrentState == null)
            {
                // 打开UI面板
                eggMachineInterface.SetState(eggMachineUI);
                Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuOpen); // 播放打开音效
                // 设置瓦片位置
                if (position.HasValue)
                {
                    // 传入位置参数
                    tilePosition = position.Value;
                    // 设置UI面板位置
                    eggMachineUI.SetTilePosition(tilePosition);
                }
            }
            else
            {
                // 关闭UI面板
                eggMachineInterface.SetState(null);
                Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuClose); // 播放关闭音效
            }
        }

        public bool IsUIVisible()
        {
            return eggMachineInterface?.CurrentState != null;
        }
    }
}