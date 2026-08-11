using log4net.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pokemon.Buffs;
using Pokemon.Content.Items;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Pokemon.Content.Equipment
{
    // 宝可梦战斗仪
    public class PokeRadar : ModItem
    {
        private bool isClick = false;
        public static int MaxItems = 5; // 最大存放物品数量
        public Item[] items = new Item[MaxItems]; // 存放物品的数组

        // 自定义顺序
        private readonly List<int> customOrder = new List<int>
            {
                0,1,2,3,4
            };

        // 设置默认属性
        public override void SetDefaults()
        {
            Item.width = 48; // 宽度
            Item.height = 48; // 高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Green; // 稀有度
        }

        // 物品创建时初始化物品数组
        public override void OnCreated(ItemCreationContext context)
        {
            InitializeItems();// 初始化物品数组
        }

        // 保存物品数据
        public override void SaveData(TagCompound tag)
        {
            var savedItems = items.Where(item => item != null).Select(item => ItemIO.Save(item)).ToList();
            tag["items"] = savedItems;
            
        }

        // 加载物品数据
        public override void LoadData(TagCompound tag)
        {
            var loadedItems = tag.GetList<TagCompound>("items");
           
            for (int i = 0; i < loadedItems.Count; i++)
            {
                if (i >= items.Length)
                    break;

                items[i] = ItemIO.Load(loadedItems[i]);
            }
        }

        // 右键点击事件处理
        public override bool CanRightClick()
        {
            if (Main.mouseRight && !isClick)
            {
                if (Main.mouseRightRelease)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        ModContent.GetInstance<PokeRadarSystem>().ToggleUI(this); // 打开宝可梦雷达UI

                        if(ModContent.GetInstance<PokeRadarSystem>().IsUIVisible())
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuOpen); // 播放打开音效
                        else
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuClose); // 播放关闭音效
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
                tooltips.Add(new TooltipLine(Mod, "", "使你能够驱使宝可梦并进行战斗！"));
                tooltips.Add(new TooltipLine(Mod, "", "【背包生效】"));

                //没有打开宝可梦战斗仪时，显示右键提示
                if (!ModContent.GetInstance<PokeRadarSystem>().IsUIVisible())
                {
                    var openTooltip = new TooltipLine(Mod, "", "右键点击" + "[c/FF5555:打开]" + "战斗仪");
                    tooltips.Add(openTooltip);
                }
                else
                {
                    var closeTooltip = new TooltipLine(Mod, "", "右键点击" + "[c/FF5555:关闭]" + "战斗仪");
                    tooltips.Add(closeTooltip);
                }
            }else
            {
                tooltips.Add(new TooltipLine(Mod, "", "Allows you to summon and fight Pokémon!"));
                tooltips.Add(new TooltipLine(Mod, "", "【Bag Effective】"));

                //没有打开宝可梦战斗仪时，显示右键提示
                if (!ModContent.GetInstance<PokeRadarSystem>().IsUIVisible())
                {
                    var openTooltip = new TooltipLine(Mod, "", "Right-click to " + "[c/FF5555:open]" + " the battle radar");
                    tooltips.Add(openTooltip);
                }
                else
                {
                    var closeTooltip = new TooltipLine(Mod, "", "Right-click to " + "[c/FF5555:close]" + " the battle radar");
                    tooltips.Add(closeTooltip);
                }
            }
                
            //// 添加更多颜色变化效果的提示信息
            //var additionalTooltip = new TooltipLine(Mod, "", "这是一个[c/00FF00:绿色]和[c/0000FF:蓝色]的示例。");
            //tooltips.Add(additionalTooltip);
        }
        // 在物品栏中绘制物品前的处理
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture;
            // 计算缩放比例和绘制位置

            if (ModContent.GetInstance<PokeRadarSystem>().IsUIVisible())
            {
                texture = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/PokeRadar_Open").Value;

                float textureScale = Math.Min((float)Item.width / texture.Width, (float)Item.height / texture.Height);
                Vector2 drawPosition = position + new Vector2(Item.width / 2f, Item.height / 2f) - texture.Size() * textureScale / 2f;

                spriteBatch.Draw(texture, drawPosition + new Vector2(-16.3f, -16.3f), null, drawColor, 0f, Vector2.Zero, textureScale * 0.68f, SpriteEffects.None, 0f);
            }
            else
            {
                texture = TextureAssets.Item[Item.type].Value;
                spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale*1.2f, SpriteEffects.None, 0f);
            }

            return false; // 返回 false 以防止默认绘制
        }

        // 初始化物品数组
        private void InitializeItems()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new Item();
                items[i].SetDefaults(0); // 设置为空物品
            }
        }

        // 添加物品到数组中
        public bool AddItem(Item item)
        {
            foreach (int index in customOrder)
            {
                // 如果当前槽位为空，放入物品
                if (items[index] == null || items[index].IsAir)
                {
                    items[index] = item.Clone(); // 放入物品
                    // 调试输出
                    //Main.NewText("添加物品到槽位：" + index);
                    return true;
                }
            }
            return false;
        }

        // 从数组中移除物品
        public bool RemoveItem(int index)
        {
            if (index >= 0 && index < items.Length && !items[index].IsAir)
            {
                items[index].TurnToAir(); // 清空物品
                return true;
            }
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            //if (Main.mouseItem.type == ModContent.ItemType<PokeRadar>() && Main.mouseLeftRelease)
            //{
            //    if (PlayerHasPokeRadar() && IsMouseInInventory())
            //    {
            //        // 阻止物品放入操作，保持物品在鼠标上
            //        Main.mouseLeftRelease = false; // 阻止放入操作
            //    }
            //}

            //// 检查背包界面是否关闭
            //if (Main.mouseItem.type == ModContent.ItemType<PokeRadar>())
            //{
            //    if (Main.mouseLeft && Main.mouseLeftRelease)
            //    {
            //        //Main.cursorOverride = 6; // 设置鼠标样式为丢弃物品
            //        Main.LocalPlayer.trashItem = Main.mouseItem.Clone();
            //        Main.mouseItem.TurnToAir(); // 物品消失
            //    }
            //}
            if (HasMoreThanOnePokeRadar())
            {
                // 遍历背包，找到 PokeRadar 并将其物品槽中的物品移动到玩家背包，然后删除 PokeRadar
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    if (player.inventory[i].ModItem is PokeRadar pokeRadar)
                    {
                        for (int j = 0; j < pokeRadar.items.Length; j++)
                        {
                            if (pokeRadar.items[j] != null && !pokeRadar.items[j].IsAir)
                            {
                                // 将物品复制到背包
                                Item item = pokeRadar.items[j].Clone();
                                player.QuickSpawnItem(player.GetSource_FromThis(), item);
                                RemoveItem(j);
                            }
                        }
                        pokeRadar.InitializeItems(); // 初始化物品槽
                        //Main.mouseItem = player.inventory[i];
                        player.inventory[i].TurnToAir();
                    }
                }
            }
        }

        //// 检查鼠标是否在背包栏内
        //private bool IsMouseInInventory()
        //{
        //    int inventoryX = Main.screenWidth / 2 - 1000;
        //    int inventoryY = Main.screenHeight / 2 - 520;
        //    int inventoryWidth = 650; // 背包栏的宽度
        //    int inventoryHeight = 270; // 背包栏的高度

        //    Rectangle inventoryRect = new Rectangle(inventoryX, inventoryY, inventoryWidth, inventoryHeight);

        //    // 检查鼠标位置是否在背包栏内
        //    return inventoryRect.Contains(Main.mouseX, Main.mouseY);
        //}

        // 检查玩家背包中是否已经存在 PokeRadar 物品
        private bool PlayerHasPokeRadar()
        {
            Player player = Main.LocalPlayer;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == ModContent.ItemType<PokeRadar>())
                {
                    return true;
                }
            }
            return false;
        }
        //检查是否有两个及其以上宝可梦战斗仪
        public bool HasMoreThanOnePokeRadar()
        {
            int count = 0;
            Player player = Main.LocalPlayer;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == ModContent.ItemType<PokeRadar>())
                {
                    count++;
                }
            }
            return count > 1;
        }

        // 在物品栏中放置物品前的处理
        public override bool CanPickup(Player player)
        {
            if (PlayerHasPokeRadar())
            {
                return false;
            }
            return true;
        }
    }

    // UIElement 宝可梦战斗仪物品槽
    public class UIItemSlotp : UIElement
    {
        private Item[] items;
        private int index;
        private PokeRadar pokeRadar;
        public bool isMouseOver = false; // 添加鼠标移入状态跟踪
        public static bool isDrawHeader = false; // 是否绘制标提图标变化
        internal static bool isEquipedBulbasaur = false;//妙蛙种子是否装备
        internal static bool isEquipedCharmander = false;// 火恐龙是否装备
        internal static bool isEquipedSquirtle = false;// 杰尼龟是否装备
        internal static bool isEquipedGastly = false;// 鬼斯是否装备
        internal static bool isEquipedTaillow = false;// 傲骨燕是否装备
        internal static bool isEquipedSunflower = false;// 向日种子是否装备
        internal static bool isEquipedSpoink = false; // 跳跳猪是否装备
        internal static bool isEquipedBeldum = false; // 铁哑铃是否装备
        internal static bool isEquipedWingull = false; // 长翅鸥是否装备
        internal static bool isEquipedVoltorb = false; // 雷电球是否装备
        internal static bool isEquipedMunchlax = false; // 小卡比兽是否装备
        internal static bool isEquipedFomantis = false; // 伪螳草是否装备
        internal static bool isEquipedTrapinch = false; // 大颚蚁否装备
        internal static bool isEquipedPikachu = false; // 皮卡丘是否装备
        internal static bool isEquipedSprigatito = false; // 新叶喵是否装备

        public UIItemSlotp(Item[] items, int index, PokeRadar pokeRadar)
        {
            this.items = items;// 物品数组
            this.index = index;// 物品槽索引
            this.pokeRadar = pokeRadar;// 宝可梦雷达实例
            Width.Set(52f, 0f);
            Height.Set(52f, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // 绘制物品槽的边框
            spriteBatch.Draw(TextureAssets.InventoryBack9.Value, GetDimensions().ToRectangle(), Color.White * 0.72f);

            // 绘制物品槽周围的线条
            // 使用 Terraria 自带的方法绘制边框
            Texture2D borderTexture = TextureAssets.BlackTile.Value;
            Rectangle borderRectangle = GetDimensions().ToRectangle();
            float borderThickness = 2f; // 边框的厚度
            Color borderColor = Color.Black*0.4f; // 边框的颜色

            // 绘制边框
            spriteBatch.Draw(borderTexture, new Rectangle(borderRectangle.X, borderRectangle.Y, (int)borderRectangle.Width, (int)borderThickness), borderColor); // 顶部边框
            spriteBatch.Draw(borderTexture, new Rectangle(borderRectangle.X, borderRectangle.Y + borderRectangle.Height - (int)borderThickness, (int)borderRectangle.Width, (int)borderThickness), borderColor); // 底部边框
            spriteBatch.Draw(borderTexture, new Rectangle(borderRectangle.X, borderRectangle.Y + (int)borderThickness, (int)borderThickness, borderRectangle.Height - 2 * (int)borderThickness), borderColor); // 左侧边框
            spriteBatch.Draw(borderTexture, new Rectangle(borderRectangle.X + borderRectangle.Width - (int)borderThickness, borderRectangle.Y + (int)borderThickness, (int)borderThickness, borderRectangle.Height - 2 * (int)borderThickness), borderColor);
            
            Texture2D Texture = ModContent.Request<Texture2D>("Pokemon/Textures/UI/Radar/icon_Radar").Value;
            spriteBatch.Draw(Texture, GetDimensions().ToRectangle(), Color.DarkBlue*0.1f);
            
            if (index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir)
            {
                var item = items[index];
                var texture = TextureAssets.Item[item.type].Value;
                // 检查纹理是否已初始化
                if (texture == null || texture.IsDisposed || texture.Width == 0 || texture.Height == 0)
                {
                    return; // 纹理未初始化，跳过绘制
                }
                // 限制物品图标大小
                float scale = Math.Min(1f, 30f / (texture.Width + texture.Height) * 2); // 48f 是物品图标大小的最大限制

                var frame = Main.itemAnimations[item.type]?.GetFrame(texture) ?? texture.Frame();
                var drawPosition = GetDimensions().Position() + new Vector2(25f) - frame.Size() * 0.5f * scale; // 调整绘制位置以适应缩放
                spriteBatch.Draw(texture, drawPosition, frame, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (isMouseOver) // 如果鼠标移入物品槽，绘制一个半透明的覆盖层来防止点击
            {
                if (index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir)
                {
                    Main.hoverItemName = this.items[index].Name;
                    Main.HoverItem = this.items[index].Clone();
                }

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), Color.White * 0.05f);
            }
            if (!Main.playerInventory)
            {
                // 在屏幕边缘绘制
                Texture2D arrowTexture = ModContent.Request<Texture2D>("Pokemon/Textures/UI/Radar/icon_Radar_Screen").Value;
                Vector2 arrowPosition = new Vector2(Main.screenWidth / 2 - arrowTexture.Width / 2, 0);
                spriteBatch.Draw(arrowTexture, arrowPosition, Color.White);
            }
        }

        //private void UpdataEquiped()
        //{
        //    //更新装备状态
        //    if (items[index].type == ModContent.ItemType<BulbasaurBadge>())
        //        isEquipedBulbasaur = false;
        //    if (items[index].type == ModContent.ItemType<CharmanderBadge>())
        //        isEquipedCharmander = false;
        //    if (items[index].type == ModContent.ItemType<SquirtleBadge>())
        //        isEquipedSquirtle = false;
        //    if (items[index].type == ModContent.ItemType<GastlyBadge>())
        //        isEquipedGastly = false;
        //    if (items[index].type == ModContent.ItemType<TaillowBadge>())
        //        isEquipedTaillow = false;
        //    if (items[index].type == ModContent.ItemType<SunflowerBall>())
        //        isEquipedSunflower = false;
        //    if (items[index].type == ModContent.ItemType<SpoinkBadge>())
        //        isEquipedSpoink = false;
        //    if (items[index].type == ModContent.ItemType<BeldumBadge>())
        //        isEquipedBeldum = false;
        //}
        ////限制，不能装备相同宝可梦
        //private void limitEquiped()
        //{
        //    // 放入限制，不能装备相同宝可梦
        //    if (isEquipedBulbasaur && Main.mouseItem.type == ModContent.ItemType<BulbasaurBadge>())
        //        return;
        //    if (isEquipedCharmander && Main.mouseItem.type == ModContent.ItemType<CharmanderBadge>())
        //        return;
        //    if (isEquipedSquirtle && Main.mouseItem.type == ModContent.ItemType<SquirtleBadge>())
        //        return;
        //    if (isEquipedGastly && Main.mouseItem.type == ModContent.ItemType<GastlyBadge>())
        //        return;
        //    if (isEquipedTaillow && Main.mouseItem.type == ModContent.ItemType<TaillowBadge>())
        //        return;
        //    if (isEquipedSunflower && Main.mouseItem.type == ModContent.ItemType<SunflowerBall>())
        //        return;
        //    if (isEquipedSpoink && Main.mouseItem.type == ModContent.ItemType<SpoinkBadge>())
        //        return;
        //    if (isEquipedBeldum && Main.mouseItem.type == ModContent.ItemType<BeldumBadge>())
        //        return;
        //}

        //private void limitEquipedOnlyPokemons()
        //{
        //    // 放入限制，只能放入宝可梦
        //    if (Main.mouseItem.type != ModContent.ItemType<BulbasaurBadge>() &&
        //        Main.mouseItem.type != ModContent.ItemType<CharmanderBadge>() &&
        //        Main.mouseItem.type != ModContent.ItemType<SquirtleBadge>() &&
        //        Main.mouseItem.type != ModContent.ItemType<GastlyBadge>() &&
        //        Main.mouseItem.type != ModContent.ItemType<TaillowBadge>() &&
        //        Main.mouseItem.type != ModContent.ItemType<SunflowerBall>() &&
        //        Main.mouseItem.type != ModContent.ItemType<SpoinkBadge>() &&
        //        Main.mouseItem.type != ModContent.ItemType<BeldumBadge>())
        //        return;
        //}

        [Obsolete]
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
           
            if (Main.keyState.IsKeyDown(Keys.LeftControl) || Main.keyState.IsKeyDown(Keys.RightControl))
            {
                // 快速丢弃物品到垃圾桶
                if (index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir)
                {
                    //更新装备状态
                    //if (items[index].type == ModContent.ItemType<BulbasaurBadge>())
                    //    isEquipedBulbasaur = false;
                    //if (items[index].type == ModContent.ItemType<CharmanderBadge>())
                    //    isEquipedCharmander = false;
                    //if (items[index].type == ModContent.ItemType<SquirtleBadge>())
                    //    isEquipedSquirtle = false;
                    //if (items[index].type == ModContent.ItemType<GastlyBadge>())
                    //    isEquipedGastly = false;
                    //if (items[index].type == ModContent.ItemType<TaillowBadge>())
                    //    isEquipedTaillow = false;
                    //if (items[index].type == ModContent.ItemType<SunflowerBall>())
                    //    isEquipedSunflower = false;
                    //if (items[index].type == ModContent.ItemType<SpoinkBadge>())
                    //    isEquipedSpoink = false;
                    //if (items[index].type == ModContent.ItemType<BeldumBadge>())
                    //    isEquipedBeldum = false;

                    //更新装备状态
                    //UpdataEquiped();

                    //快捷放进垃圾桶
                    Main.LocalPlayer.trashItem = items[index].Clone();
                    pokeRadar.RemoveItem(index);
                    
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
                }

            }
            else if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
            {
                // 快速放入玩家背包
                if (index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir)
                {
                    //更新装备状态
                    //if (items[index].type == ModContent.ItemType<BulbasaurBadge>())
                    //    isEquipedBulbasaur = false;
                    //if (items[index].type == ModContent.ItemType<CharmanderBadge>())
                    //    isEquipedCharmander = false;
                    //if (items[index].type == ModContent.ItemType<SquirtleBadge>())
                    //    isEquipedSquirtle = false;
                    //if (items[index].type == ModContent.ItemType<GastlyBadge>())
                    //    isEquipedGastly = false;
                    //if (items[index].type == ModContent.ItemType<TaillowBadge>())
                    //    isEquipedTaillow = false;
                    //if (items[index].type == ModContent.ItemType<SunflowerBall>())
                    //    isEquipedSunflower = false;
                    //if (items[index].type == ModContent.ItemType<SpoinkBadge>())
                    //    isEquipedSpoink = false;
                    //if (items[index].type == ModContent.ItemType<BeldumBadge>())
                    //    isEquipedBeldum = false;

                    //更新装备状态
                    //UpdataEquiped();

                    // 快捷放回背包
                    Main.LocalPlayer.QuickSpawnClonedItem(Main.LocalPlayer.GetSource_Misc("PokeRadar"), items[index], items[index].stack);
                    pokeRadar.RemoveItem(index);
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
                }
            }
            else
            {
                if (Main.mouseItem.IsAir && index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir) 
                    // 如果鼠标没有物品并且物品槽有物品
                {
                    //更新装备状态
                    //if (items[index].type == ModContent.ItemType<BulbasaurBadge>())
                    //    isEquipedBulbasaur = false;
                    //if (items[index].type == ModContent.ItemType<CharmanderBadge>())
                    //    isEquipedCharmander = false;
                    //if (items[index].type == ModContent.ItemType<SquirtleBadge>())
                    //    isEquipedSquirtle = false;
                    //if (items[index].type == ModContent.ItemType<GastlyBadge>())
                    //    isEquipedGastly = false;
                    //if (items[index].type == ModContent.ItemType<TaillowBadge>())
                    //    isEquipedTaillow = false;
                    //if (items[index].type == ModContent.ItemType<SunflowerBall>())
                    //    isEquipedSunflower = false;
                    //if (items[index].type == ModContent.ItemType<SpoinkBadge>())
                    //    isEquipedSpoink = false;
                    //if (items[index].type == ModContent.ItemType<BeldumBadge>())
                    //    isEquipedBeldum = false;

                    //更新装备状态
                    //UpdataEquiped();

                    //拿出物品
                    Main.mouseItem = items[index].Clone();
                    pokeRadar.RemoveItem(index);
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
                }
                else if (!Main.mouseItem.IsAir && (index >= 0 && index < items.Length && (items[index] == null || items[index].IsAir))) 
                    // 如果鼠标有物品并且物品槽没有物品
                {
                    // 放入限制，不能装备相同宝可梦
                    if (isEquipedBulbasaur && Main.mouseItem.type == ModContent.ItemType<BulbasaurBadge>())
                        return;
                    if (isEquipedCharmander && Main.mouseItem.type == ModContent.ItemType<CharmanderBadge>())
                        return;
                    if (isEquipedSquirtle && Main.mouseItem.type == ModContent.ItemType<SquirtleBadge>())
                        return;
                    if (isEquipedGastly && Main.mouseItem.type == ModContent.ItemType<GastlyBadge>())
                        return;
                    if (isEquipedTaillow && Main.mouseItem.type == ModContent.ItemType<TaillowBadge>())
                        return;
                    if (isEquipedSunflower && Main.mouseItem.type == ModContent.ItemType<SunflowerBall>())
                        return;
                    if (isEquipedSpoink && Main.mouseItem.type == ModContent.ItemType<SpoinkBadge>())
                        return;
                    if (isEquipedBeldum && Main.mouseItem.type == ModContent.ItemType<BeldumBadge>())
                        return;
                    if (isEquipedWingull && Main.mouseItem.type == ModContent.ItemType<WingullBadge>())
                        return;
                    if (isEquipedVoltorb && Main.mouseItem.type == ModContent.ItemType<VoltorbBadge>())
                        return;
                    if (isEquipedMunchlax && Main.mouseItem.type == ModContent.ItemType<MunchlaxBadge>())
                        return;
                    if (isEquipedFomantis && Main.mouseItem.type == ModContent.ItemType<FomantisBadge>())
                        return;
                    if (isEquipedTrapinch && Main.mouseItem.type == ModContent.ItemType<TrapinchBadge>())
                        return;
                    if (isEquipedPikachu && Main.mouseItem.type == ModContent.ItemType<PikachuBadge>())
                        return;
                    //if (!isEquipedSprigatito && Main.mouseItem.type == ModContent.ItemType<SprigatitoBadge>())
                    //    return;

                        // 放入限制，不能装备相同宝可梦
                        //limitEquiped();

                        // 放入限制，只能放入宝可梦
                        if (Main.mouseItem.type != ModContent.ItemType<BulbasaurBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<CharmanderBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<SquirtleBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<GastlyBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<TaillowBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<SunflowerBall>() &&
                        Main.mouseItem.type != ModContent.ItemType<SpoinkBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<BeldumBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<WingullBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<VoltorbBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<MunchlaxBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<FomantisBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<TrapinchBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<PikachuBadge>() 
                        //&&
                        //Main.mouseItem.type != ModContent.ItemType<SprigatitoBadge>()
                        )
                        return;

                    // 放入限制，只能放入宝可梦
                    //limitEquipedOnlyPokemons();

                    //放入物品
                    items[index] = Main.mouseItem.Clone(); // 放入物品
                    Main.mouseItem.TurnToAir(); // 清空鼠标携带的物品
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效

                }
                else if (!Main.mouseItem.IsAir && index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir) 
                    // 如果鼠标有物品并且物品槽有物品
                {
                    //放入限制，只能放入宝可梦
                    if (Main.mouseItem.type != ModContent.ItemType<BulbasaurBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<CharmanderBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<SquirtleBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<GastlyBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<TaillowBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<SunflowerBall>() &&
                        Main.mouseItem.type != ModContent.ItemType<SpoinkBadge>()&&
                        Main.mouseItem.type != ModContent.ItemType<BeldumBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<WingullBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<VoltorbBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<MunchlaxBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<FomantisBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<TrapinchBadge>() &&
                        Main.mouseItem.type != ModContent.ItemType<PikachuBadge>() 
                        //&&
                        //Main.mouseItem.type != ModContent.ItemType<SprigatitoBadge>()
                        )
                        return;

                    // 放入限制，只能放入宝可梦
                    //limitEquipedOnlyPokemons();

                    // 放入限制，不能装备相同宝可梦
                    if (isEquipedBulbasaur && Main.mouseItem.type == ModContent.ItemType<BulbasaurBadge>())
                        return;
                    if (isEquipedCharmander && Main.mouseItem.type == ModContent.ItemType<CharmanderBadge>())
                        return;
                    if (isEquipedSquirtle && Main.mouseItem.type == ModContent.ItemType<SquirtleBadge>())
                        return;
                    if (isEquipedGastly && Main.mouseItem.type == ModContent.ItemType<GastlyBadge>())
                        return;
                    if (isEquipedTaillow && Main.mouseItem.type == ModContent.ItemType<TaillowBadge>())
                        return;
                    if (isEquipedSunflower && Main.mouseItem.type == ModContent.ItemType<SunflowerBall>())
                        return;
                    if (isEquipedSpoink && Main.mouseItem.type == ModContent.ItemType<SpoinkBadge>())
                        return;
                    if (isEquipedBeldum && Main.mouseItem.type == ModContent.ItemType<BeldumBadge>())
                        return;
                    if (isEquipedWingull && Main.mouseItem.type == ModContent.ItemType<WingullBadge>())
                        return;
                    if (isEquipedVoltorb && Main.mouseItem.type == ModContent.ItemType<VoltorbBadge>())
                        return;
                    if (isEquipedMunchlax && Main.mouseItem.type == ModContent.ItemType<MunchlaxBadge>())
                        return;
                    if (isEquipedFomantis && Main.mouseItem.type == ModContent.ItemType<FomantisBadge>())
                        return;
                    if (isEquipedTrapinch && Main.mouseItem.type == ModContent.ItemType<TrapinchBadge>())
                        return;
                    if (isEquipedPikachu && Main.mouseItem.type == ModContent.ItemType<PikachuBadge>())
                        return;
                    //if (!isEquipedSprigatito && Main.mouseItem.type == ModContent.ItemType<SprigatitoBadge>())
                    //    return;

                    // 放入限制，不能装备相同宝可梦
                    //limitEquiped();

                    ////装备宝可梦，更新装备状态
                    //if (items[index].type == ModContent.ItemType<BulbasaurBadge>())
                    //    isEquipedBulbasaur = false;
                    //if (items[index].type == ModContent.ItemType<CharmanderBadge>())
                    //    isEquipedCharmander = false;
                    //if (items[index].type == ModContent.ItemType<SquirtleBadge>())
                    //    isEquipedSquirtle = false;
                    //if (items[index].type == ModContent.ItemType<GastlyBadge>())
                    //    isEquipedGastly = false;
                    //if (items[index].type == ModContent.ItemType<TaillowBadge>())
                    //    isEquipedTaillow = false;
                    //if (items[index].type == ModContent.ItemType<SunflowerBall>())
                    //    isEquipedSunflower = false;
                    //if (items[index].type == ModContent.ItemType<SpoinkBadge>())
                    //    isEquipedSpoink = false;
                    //if (items[index].type == ModContent.ItemType<BeldumBadge>())
                    //    isEquipedBeldum = false;

                    //更新装备状态
                    //UpdataEquiped();

                    // 交换物品
                    var temp = items[index].Clone();
                    items[index] = Main.mouseItem.Clone();
                    Main.mouseItem = temp;
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
                }
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            isMouseOver = true;
            isDrawHeader = true;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            isMouseOver = false;
            isDrawHeader = false;
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Main.player[Main.myPlayer];

            // 如果玩家背包没有PokeRadar
            PokeRadar pokeRadar_ = null;
            for (int i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++)
            {
                if (Main.player[Main.myPlayer].inventory[i].ModItem is PokeRadar radar)
                {
                    pokeRadar_ = radar;
                    break;
                }
            }
            if (pokeRadar_ == null)
            {
                if (ModContent.GetInstance<PokeRadarSystem>().IsUIVisible())
                {
                    ModContent.GetInstance<PokeRadarSystem>().ToggleUI(pokeRadar); // 关闭宝可梦雷达UI
                }
            }
            // 获得鼠标位置
            Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);

            // 获取面板的矩形区域
            Rectangle panelRect = GetDimensions().ToRectangle();

            // 如果鼠标移入面板，阻止物品使用
            if (panelRect.Contains(mousePos.ToPoint()))
            {
                player.mouseInterface = true;
            }

            if (!player.HasBuff(ModContent.BuffType<BuffsBulbasaurBadge>()))
                isEquipedBulbasaur = false;
            else
                isEquipedBulbasaur = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsCharmanderBadge>()))
                isEquipedCharmander = false;
            else
                isEquipedCharmander = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsSquirtleBadge>()))
                isEquipedSquirtle = false;
            else
                isEquipedSquirtle = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsGastlyBadge>()))
                isEquipedGastly = false;
            else
                isEquipedGastly = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsTaillowBadge>()))
                isEquipedTaillow = false;
            else
                isEquipedTaillow = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsSunflowerBall>()))
                isEquipedSunflower = false;
            else
                isEquipedSunflower = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsSpoinkBadge>()))
                isEquipedSpoink = false;
            else
                isEquipedSpoink = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsBeldumBadge>()))
                isEquipedBeldum = false;
            else
                isEquipedBeldum = true;
            if(!player.HasBuff(ModContent.BuffType<BuffsWingullBadge>()))
                isEquipedWingull = false;
            else
                isEquipedWingull = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsVoltorbBadge>()))
                isEquipedVoltorb = false;
            else
                isEquipedVoltorb = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsMunchlaxBadge>()))
                isEquipedMunchlax = false;
            else
                isEquipedMunchlax = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsFomantisBadge>()))
                isEquipedFomantis = false;
            else
                isEquipedFomantis = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsTrapinchBadge>()))
                isEquipedTrapinch = false;
            else
                isEquipedTrapinch = true;
            if (!player.HasBuff(ModContent.BuffType<BuffsPikachuBadge>()))
                isEquipedPikachu = false;
            else
                isEquipedPikachu = true;
            //if (!player.HasBuff(ModContent.BuffType<BuffsSprigatitoBadge>()))
            //    isEquipedSprigatito = false;
            //else
            //    isEquipedSprigatito = true;

            if (BulbasaurBadge.isEquippedtoBackpack)//1
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        BulbasaurBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (CharmanderBadge.isEquippedtoBackpack)//2
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        CharmanderBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (SquirtleBadge.isEquippedtoBackpack)//3
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        SquirtleBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (GastlyBadge.isEquippedtoBackpack)//4
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        GastlyBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (TaillowBadge.isEquippedtoBackpack)//5
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        TaillowBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (SunflowerBall.isEquippedtoBackpack)//6
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        SunflowerBall.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (SpoinkBadge.isEquippedtoBackpack)//7
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        SpoinkBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (BeldumBadge.isEquippedtoBackpack)//8
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        BeldumBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (WingullBadge.isEquippedtoBackpack)//9
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        WingullBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (VoltorbBadge.isEquippedtoBackpack)//10
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        VoltorbBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (MunchlaxBadge.isEquippedtoBackpack)//11
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        MunchlaxBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (FomantisBadge.isEquippedtoBackpack)//12
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        FomantisBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (TrapinchBadge.isEquippedtoBackpack)//13
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        TrapinchBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            if (PikachuBadge.isEquippedtoBackpack)//14
            {
                for (int i = 0, len = 5; i < len; i++)
                {
                    if (items[i] != null && items[i].IsAir)
                    {
                        items[i] = Main.mouseItem.Clone();
                        Main.mouseItem.TurnToAir();
                        PikachuBadge.isEquippedtoBackpack = false;
                        break;
                    }
                }
            }
            //if (SprigatitoBadge.isEquippedtoBackpack)//15
            //{
            //    for (int i = 0, len = 5; i < len; i++)
            //    {
            //        if (items[i] != null && items[i].IsAir)
            //        {
            //            items[i] = Main.mouseItem.Clone();
            //            Main.mouseItem.TurnToAir();
            //            SprigatitoBadge.isEquippedtoBackpack = false;
            //            break;
            //        }
            //    }
            //}

            if (!player.dead && items[index] != null && !items[index].IsAir)
            {
                //Player player = Main.player[Main.myPlayer];
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
                    if (items[index].ModItem is BulbasaurBadge bulbasaurBadge)
                    {//1
                        if(!player.HasBuff(ModContent.BuffType<BuffsBulbasaurBadge>()))
                        bulbasaurBadge.UpdateAccessory(Main.LocalPlayer, false);// 妙蛙种子的附加效果
                        bulbasaurBadge.Item.damage = bulbasaurBadge.level;
                        isEquipedBulbasaur = true;
                    }

                    if (items[index].ModItem is CharmanderBadge charmanderBadge)
                    {//2
                        if(!player.HasBuff(ModContent.BuffType<BuffsCharmanderBadge>()))
                        charmanderBadge.UpdateAccessory(Main.LocalPlayer, false);// 小火龙的附加效果
                        charmanderBadge.Item.damage = charmanderBadge.level;
                        isEquipedCharmander = true;
                    }

                    if (items[index].ModItem is SquirtleBadge squirtleBadge)
                    {//3
                        if(!player.HasBuff(ModContent.BuffType<BuffsSquirtleBadge>()))
                        squirtleBadge.UpdateAccessory(Main.LocalPlayer, false);// 杰尼龟的附加效果
                        squirtleBadge.Item.damage = squirtleBadge.level;
                        isEquipedSquirtle = true;
                    }
                    if (items[index].ModItem is GastlyBadge gastlyBadge)
                    {//4
                        if(!player.HasBuff(ModContent.BuffType<BuffsGastlyBadge>()))
                        gastlyBadge.UpdateAccessory(Main.LocalPlayer, false);// 鬼斯的附加效果
                        gastlyBadge.Item.damage = gastlyBadge.level;
                        isEquipedGastly = true;
                    }
                    if (items[index].ModItem is TaillowBadge taillowBadge)
                    {//5
                        if(!player.HasBuff(ModContent.BuffType<BuffsTaillowBadge>()))
                        taillowBadge.UpdateAccessory(Main.LocalPlayer, false);// 傲骨燕的附加效果
                        taillowBadge.Item.damage = taillowBadge.level;
                        isEquipedTaillow = true;
                    }
                    if (items[index].ModItem is SunflowerBall sunflowerBall)
                    {//6
                        if(!player.HasBuff(ModContent.BuffType<BuffsSunflowerBall>()))
                        sunflowerBall.UpdateAccessory(Main.LocalPlayer, false);// 向日种子的附加效果
                        sunflowerBall.Item.damage = sunflowerBall.level;
                        isEquipedSunflower = true;
                    }
                    if (items[index].ModItem is SpoinkBadge spoinkBadge)
                    {//7
                        if(!player.HasBuff(ModContent.BuffType<BuffsSpoinkBadge>()))
                        spoinkBadge.UpdateAccessory(Main.LocalPlayer, false);// 跳跳猪的附加效果
                        spoinkBadge.Item.damage = spoinkBadge.level;
                        isEquipedSpoink = true;
                    }
                    if (items[index].ModItem is BeldumBadge beldumBadge)
                    {//8
                        if (!player.HasBuff(ModContent.BuffType<BuffsBeldumBadge>()))
                        beldumBadge.UpdateAccessory(Main.LocalPlayer, false);// 铁哑铃的附加效果
                        beldumBadge.Item.damage = beldumBadge.level;
                        isEquipedBeldum = true;
                    }
                    if (items[index].ModItem is WingullBadge wingullBadge)
                    {//9
                        if(!player.HasBuff(ModContent.BuffType<BuffsWingullBadge>()))
                        wingullBadge.UpdateAccessory(Main.LocalPlayer, false);// 长翅鸥的附加效果
                        wingullBadge.Item.damage = wingullBadge.level;
                        isEquipedBeldum = true;
                    }
                    if (items[index].ModItem is VoltorbBadge voltorbBadge)
                    {//10
                        if(!player.HasBuff(ModContent.BuffType<BuffsVoltorbBadge>()))
                        voltorbBadge.UpdateAccessory(Main.LocalPlayer, false);// 雷电球的附加效果
                        voltorbBadge.Item.damage = voltorbBadge.level;
                        isEquipedVoltorb = true;
                    }
                    if (items[index].ModItem is MunchlaxBadge munchlaxBadge)
                    {//11
                        if(!player.HasBuff(ModContent.BuffType<BuffsMunchlaxBadge>()))
                        munchlaxBadge.UpdateAccessory(Main.LocalPlayer, false);// 小卡比兽的附加效果
                        munchlaxBadge.Item.damage = munchlaxBadge.level;
                        isEquipedMunchlax = true;
                    }
                    if (items[index].ModItem is FomantisBadge fomantisBadge)
                    {//12
                        if(!player.HasBuff(ModContent.BuffType<BuffsFomantisBadge>()))
                        fomantisBadge.UpdateAccessory(Main.LocalPlayer, false);// 伪螳草的附加效果
                        fomantisBadge.Item.damage = fomantisBadge.level;
                        isEquipedFomantis = true;
                    }
                    if (items[index].ModItem is TrapinchBadge trapinchBadge)
                    {//13
                        if(!player.HasBuff(ModContent.BuffType<BuffsTrapinchBadge>()))
                        trapinchBadge.UpdateAccessory(Main.LocalPlayer, false);// 大颚蚁的附加效果
                        trapinchBadge.Item.damage = trapinchBadge.level;
                        isEquipedTrapinch = true;
                    }
                    if (items[index].ModItem is PikachuBadge pikachuBadge)
                    {//14
                        if(!player.HasBuff(ModContent.BuffType<BuffsPikachuBadge>()))
                        pikachuBadge.UpdateAccessory(Main.LocalPlayer, false);// 皮卡丘的附加效果
                        pikachuBadge.Item.damage = pikachuBadge.level;
                        isEquipedPikachu = true;
                    }
                    //if (items[index].ModItem is SprigatitoBadge sprigatitoBadge)
                    //{//15
                    //    if(!player.HasBuff(ModContent.BuffType<BuffsSprigatitoBadge>()))
                    //    sprigatitoBadge.UpdateAccessory(Main.LocalPlayer, false);// 新叶喵的附加效果
                    //    sprigatitoBadge.Item.damage = sprigatitoBadge.level;
                    //    isEquipedSprigatito = true;
                    //}
                }
            }
            if (isMouseOver && Main.mouseItem.IsAir && index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir)
            {
                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl)) // 如果按下 Ctrl 键
                {
                    Main.cursorOverride = 6; // 设置鼠标样式为丢弃物品
                }
                else if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)) // 如果按下 Shift 键
                {
                    Main.cursorOverride = 8; // 设置鼠标样式为快速放入背包
                }
                else
                {
                    Main.cursorOverride = -1; // 恢复默认鼠标样式
                }
            }
        }
    }

    //UIState 继承自 UIElement，提供 UI 状态切换、绘制等功能
    public class PokeRadarUI : UIState
    {
        internal UIPanel mainPanelp; // 将访问修饰符更改为 internal
        //private CustomHeaderPanelP headerPanel; // 头部面板
        private UIGrid itemGrid;
        private PokeRadar pokeRadar;
        //private UIImageButton closeButton;

        public PokeRadarUI()
        {
            mainPanelp = new UIPanel(); // 创建面板
            mainPanelp.SetPadding(10); // 设置内边距
            mainPanelp.Left.Set(685f, 0f); // 设置左侧位置
            mainPanelp.Top.Set(20f, 0f); // 设置顶部位置
            mainPanelp.Width.Set(380f, 0f); // 设置宽度
            mainPanelp.Height.Set(110f, 0f); // 设置高度
            Append(mainPanelp); // 添加到 UI 状态中

            // 创建物品网格
            itemGrid = new UIGrid(); // 使用自定义的 UIGrid
            itemGrid.Width.Set(-20f, 1f); // 设置宽度
            itemGrid.Height.Set(-40f, 1f); // 设置高度，减去头部和尾部面板的高度
            itemGrid.Top.Set(20f, 0f); // 设置顶部位置，避开头部面板
            itemGrid.Left.Set(10f, 0f); // 设置左侧位置
            itemGrid.ListPadding = 20f; // 设置列表间隔
            mainPanelp.Append(itemGrid); // 添加到主面板中
        }

        // 设置物品袋
        public void SetPokeRadar(PokeRadar pokeRadar)
        {
            this.pokeRadar = pokeRadar; // 设置物品袋
            itemGrid.Clear(); // 清空物品网格

            for (int i = 0; i < PokeRadar.MaxItems; i++)
            {
                var itemSlot = new UIItemSlotp(pokeRadar.items, i, pokeRadar); // 创建物品槽
                itemGrid.Add(itemSlot); // 添加到物品网格中
            }
        }

        // 初始化事件处理
        public override void OnInitialize()
        {
            // 初始化已经在构造函数中完成
        }
    }

    // ModSystem 继承自 Mod，提供 Mod 系统相关功能
    public class PokeRadarSystem : ModSystem
    {
        private UserInterface pokeRadarInterface;
        internal PokeRadarUI pokeRadarUI;
        //public bool ShouldDrawCustomCursor { get; set; } // 添加标志

        //public override void OnWorldLoad()
        //{
        //    Player player = Main.player[Main.myPlayer];
        //    // 遍历玩家的物品栏，查找 PokeRadar 实例
        //    foreach (var item in player.inventory)
        //    {
        //        if (item.ModItem is PokeRadar pokeRadar)
        //        {
        //            // 如果数组中有物品，则显示宝可梦雷达UI
        //            if (pokeRadar.items.Any(i => i != null && !i.IsAir))
        //            {
        //                ToggleUI(pokeRadar);
        //            }
        //        }
        //    }
        //    base.OnWorldLoad();
        //}
        // 加载事件处理：初始化物品袋界面
        public override void Load()
        {
            if (!Main.dedServ) // 如果不是服务器模式
            {
                pokeRadarUI = new PokeRadarUI(); // 创建物品袋界面
                pokeRadarInterface = new UserInterface(); // 创建用户界面
            }
        }

        // 更新 UI
        public override void UpdateUI(GameTime gameTime)
        {
            if (pokeRadarInterface?.CurrentState != null) // 如果当前状态不为空
            {
                pokeRadarInterface.Update(gameTime); // 更新 UI
            }

            if (pokeRadarInterface?.CurrentState != null) // 如果当前状态不为空
            {
                pokeRadarInterface.Update(gameTime); // 更新 UI
            }

            float targetTop = Main.playerInventory ? 20f : -pokeRadarUI.mainPanelp.Height.Pixels;
            float currentTop = pokeRadarUI.mainPanelp.Top.Pixels;
            float moveSpeed = 10f; // 调整移动速度

            if (Math.Abs(currentTop - targetTop) > moveSpeed)
            {
                if (currentTop < targetTop)
                {
                    currentTop += moveSpeed;
                }
                else
                {
                    currentTop -= moveSpeed;
                }
            }
            else
            {
                currentTop = targetTop;
            }

            pokeRadarUI.mainPanelp.Top.Set(currentTop, 0f);
            pokeRadarUI.Recalculate();
        }

        // 修改界面层：插入自定义的物品袋界面层
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory")); // 找到库存界面层
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                    "Pokemon: PokeRadar",
                    delegate
                    {
                        if (pokeRadarInterface?.CurrentState != null) // 如果当前状态不为空
                        {
                            pokeRadarInterface.Draw(Main.spriteBatch, new GameTime()); // 绘制 UI
                        }
                        return true;
                    },
                    InterfaceScaleType.UI) // 设置界面缩放类型
                );
            }
        }

        // 切换 UI 状态：打开或关闭物品袋界面
        public void ToggleUI(PokeRadar pokeRadar)
        {
            if (pokeRadarInterface.CurrentState == null) // 如果当前状态为空
            {
                //Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuOpen); // 播放打开音效
                pokeRadarUI.SetPokeRadar(pokeRadar); // 设置物品袋
                pokeRadarInterface.SetState(pokeRadarUI); // 设置 UI 状态为物品袋界面
            }
            else
            {
                //Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuClose); // 播放关闭音效
                pokeRadarInterface.SetState(null); // 关闭物品袋界面
            }
        }

        // 隐藏 UI
        public void HideUI()
        {
            //Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuClose); // 播放关闭音效
            pokeRadarInterface.SetState(null); // 关闭物品袋界面
        }

        // 检查 UI 是否可见
        public bool IsUIVisible()
        {
            return pokeRadarInterface?.CurrentState != null;
        }
    }
}
