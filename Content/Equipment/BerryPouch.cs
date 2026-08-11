using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.GameContent;
using Terraria.DataStructures;
using System.Linq;
using Terraria.ModLoader.IO;
using System;
using ReLogic.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Terraria.Localization;

namespace Pokemon.Content.Equipment
{
    // 树果袋
    public class BerryPouch : ModItem
    {
        private bool isClick = false;
        public static int MaxItems = 106; // 最大存放物品数量
        public Item[] items = new Item[MaxItems]; // 存放物品的数组

        // 自定义顺序
        private readonly List<int> customOrder = new List<int>
            {
                0, 97, 73, 93, 39, 89, 33, 85,
                4, 81, 45, 69, 77, 53, 57, 49,
                2, 25, 65, 21, 7, 15, 29, 61,
                37, 3, 95, 99, 8, 87, 51, 79,
                1, 71, 17, 63, 31, 5, 55, 23,
                91, 83, 75, 67, 47, 43, 27, 59,
                41, 35, 19, 6, 101, 103, 64, 32,
                18, 14, 24, 68, 28, 12, 52, 60,
                56, 80, 72, 48, 84, 10, 88, 36,
                92, 42, 96, 76, 100, 40, 104, 11,
                102, 22, 38, 44, 62, 30, 46, 50,
                70, 78, 86, 94, 26, 58, 9, 34,
                66, 20, 74, 13, 82, 54, 90, 16,
                98, 105
            };

        // 一键存入玩家背包第二行开始的40个物品（跳过收藏物品）
        public void StoreAllFromInventory(Player player)
        {
            // 只处理背包的第2行及以后（索引10~49，共40格）
            for (int i = 10; i < 50; i++)
            {
                Item item = player.inventory[i];
                if (item != null && !item.IsAir && !item.favorited && item.type != ModContent.ItemType<BerryPouch>())
                {
                    if (AddItem(item))
                    {
                        player.inventory[i].TurnToAir();
                    }
                }
            }
        }

        // 一键取出所有物品到玩家背包（只填满背包为止，按 customOrder 顺序）
        public void TakeAllToInventory(Player player)
        {
            foreach (int idx in customOrder)
            {
                if (items[idx] != null && !items[idx].IsAir)
                {
                    // 找到玩家背包第一个空位
                    int emptySlot = -1;
                    for (int j = 10; j < 50; j++)
                    {
                        if (player.inventory[j] == null || player.inventory[j].IsAir)
                        {
                            emptySlot = j;
                            break;
                        }
                    }
                    if (emptySlot == -1)
                    {
                        // 背包已满，停止
                        break;
                    }
                    player.inventory[emptySlot] = items[idx].Clone();
                    items[idx].TurnToAir(); // 正确清除
                }
            }
        }
        private (int category, object[] secondary) GetItemCategory(Item item)
        {
            // 0:近战武器
            if (item.DamageType == DamageClass.Melee && item.pick == 0 && item.axe == 0 && item.hammer == 0 && item.damage > 0 && item.ammo == 0)
                return (0, new object[] { item.rare, item.damage, item.value });
            // 1:远程武器
            if (item.DamageType == DamageClass.Ranged && item.damage > 0 && item.ammo == 0)
                return (1, new object[] { item.rare, item.damage, item.value });
            // 2:魔法武器
            if (item.DamageType == DamageClass.Magic && item.damage > 0 && item.ammo == 0)
                return (2, new object[] { item.rare, item.damage, item.value });
            // 3:召唤武器
            if (item.DamageType == DamageClass.Summon && item.damage > 0 && item.ammo == 0)
                return (3, new object[] { item.rare, item.damage, item.value });
            // 4:其他有伤害的（不属于上述类型且不是弹药）
            if (item.damage > 0 && item.ammo == 0)
                return (4, new object[] { item.rare, item.damage, item.value });
            // 5:其他物品
            return (5, new object[] { item.value, item.rare, item.stack });
        }

        public void SortItems()
        {
            var validItems = items.Where(i => i != null && !i.IsAir).ToList();

            // 分类和排序
            validItems.Sort((a, b) =>
            {
                var ca = GetItemCategory(a);
                var cb = GetItemCategory(b);
                if (ca.category != cb.category)
                    return ca.category.CompareTo(cb.category);
                for (int i = 0; i < ca.secondary.Length; i++)
                {
                    int cmp = 0;
                    if (ca.secondary[i] is string sa && cb.secondary[i] is string sb)
                        cmp = string.Compare(sb, sa, StringComparison.OrdinalIgnoreCase); // 字符串降序
                    else if (ca.secondary[i] is IComparable ica && cb.secondary[i] is IComparable icb)
                        cmp = icb.CompareTo(ica); // 数值降序
                    if (cmp != 0) return cmp;
                }
                return a.type.CompareTo(b.type); // 最后按ID
            });

            // 清空所有槽位
            for (int i = 0; i < items.Length; i++)
                items[i] = new Item();

            // 按 customOrder 顺序重新放入物品
            for (int i = 0; i < validItems.Count && i < customOrder.Count; i++)
                items[customOrder[i]] = validItems[i].Clone();
        }
        // 设置默认属性
        public override void SetDefaults()
        {
            Item.width = 42; // 宽度
            Item.height = 42; // 高度
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
            var savedTextures = items.Where(item => item != null).Select(item => item.type).ToList();

            tag["items"] = savedItems;
            tag["textures"] = savedTextures;
        }

        // 加载物品数据
        public override void LoadData(TagCompound tag)
        {
            var loadedItems = tag.GetList<TagCompound>("items");
            var loadedTextures = tag.GetList<int>("textures");

            for (int i = 0; i < loadedItems.Count; i++)
            {
                if (i >= items.Length)
                    break;

                items[i] = ItemIO.Load(loadedItems[i]);

                // 确保物品纹理已加载
                if (items[i] != null && !items[i].IsAir)
                {
                    if (i < loadedTextures.Count)
                    {
                        int itemType = loadedTextures[i];
                        if (itemType >= 0 && itemType < TextureAssets.Item.Length)
                        {
                            if (!TextureAssets.Item[itemType].IsLoaded)
                            {
                                // 使用正确的路径格式请求原版物品的纹理
                                TextureAssets.Item[itemType] = ModContent.Request<Texture2D>($"Terraria/Images/Item_{itemType}");
                            }
                        }
                    }
                }
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
                        ModContent.GetInstance<BerryPouchSystem>().ToggleUI(this); // 打开树果袋UI
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
                tooltips.Add(new TooltipLine(Mod, "", $"按下 {BerryPouchSystem.OpenBerryPouchKeybind.GetAssignedKeys().FirstOrDefault() ?? "未绑定"} 快捷存放"));
                tooltips.Add(new TooltipLine(Mod, "", "【背包生效】"));
                var openTooltip = (new TooltipLine(Mod, "", ModContent.GetInstance<BerryPouchSystem>().IsUIVisible() ?
                    "右键点击" + "[c/FFEE00:关闭]" + "树果袋" : "右键点击" + "[c/FFEE00:打开]" + "树果袋"));
                tooltips.Add(openTooltip);
            }else
            {
                tooltips.Add(new TooltipLine(Mod, "", $"Press {BerryPouchSystem.OpenBerryPouchKeybind.GetAssignedKeys().FirstOrDefault() ?? "unbound"} to store items"));
                tooltips.Add(new TooltipLine(Mod, "", "【Inventory Effect】"));
                var openTooltip = (new TooltipLine(Mod, "", ModContent.GetInstance<BerryPouchSystem>().IsUIVisible() ?
                    "Right-click to " + "[c/FFEE00:close]" + " Berry Pouch" : "Right-click to " + "[c/FFEE00:open]" + " Berry Pouch"));
                tooltips.Add(openTooltip);
            }
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

        // 在物品栏中绘制物品前的处理
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture;
            // 计算缩放比例和绘制位置
           
            if (ModContent.GetInstance<BerryPouchSystem>().IsUIVisible())
            {
                texture = ModContent.Request<Texture2D>("Pokemon/Content/Equipment/BerryPouch_Open").Value;
                
                float textureScale = Math.Min((float)Item.width / texture.Width, (float)Item.height / texture.Height);
                Vector2 drawPosition = position + new Vector2(Item.width / 2f, Item.height / 2f) - texture.Size() * textureScale / 2f;

                spriteBatch.Draw(texture, drawPosition + new Vector2(-18f, -18f), null, drawColor, 0f, Vector2.Zero, textureScale * 0.8f, SpriteEffects.None, 0f);
            }
            else
            {
                texture = TextureAssets.Item[Item.type].Value;
                spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            }

            return false; // 返回 false 以防止默认绘制
        }

        public override void UpdateInventory(Player player)
        {
            base.UpdateInventory(player);

            // 检查鼠标是否指向玩家背包中的物品
            if (Main.playerInventory && ModContent.GetInstance<BerryPouchSystem>().IsUIVisible())
            {
                // 如果鼠标指向的物品有效且有物品
                if (Main.mouseItem != null && Main.mouseItem.type != ModContent.ItemType<BerryPouch>())
                {
                    // 检查鼠标是否在 UI 面板内
                    var berryPouchUI = ModContent.GetInstance<BerryPouchSystem>().berryPouchUI;
                    if (!berryPouchUI.mainPanel.ContainsPoint(Main.MouseScreen))
                    {
                        // 检查鼠标左键点击并且UI面板未隐藏
                        if (BerryPouchSystem.OpenBerryPouchKeybind.Current)
                        {
                            // 获取鼠标指向的物品
                            Item hoverItem = Main.HoverItem;
                            if (hoverItem != null&& !hoverItem.favorited && !hoverItem.IsAir && hoverItem.type != ModContent.ItemType<BerryPouch>())
                            {
                                // 设置标志以在绘制阶段绘制自定义光标
                                ModContent.GetInstance<BerryPouchSystem>().ShouldDrawCustomCursor = true;
                            }
                            // 检查武器是否正在挥舞
                            if (Main.LocalPlayer.itemAnimation == 0 && Main.mouseLeftRelease && !Main.mouseItem.favorited)
                            {
                                // 添加物品到数组中
                                if (AddItem(Main.mouseItem))
                                {
                                    Main.mouseItem.TurnToAir(); // 清空鼠标指向的物品
                                }
                            }
                        }
                    }
                }
            }

        }
    }
   
    // UIElement 树果袋物品槽
    public class UIItemSlot : UIElement
    {
        private Item[] items;
        private int index;
        private BerryPouch berryPouch;
        public bool isMouseOver = false; // 添加鼠标移入状态跟踪
        public static bool isDrawHeader = false; // 是否绘制标提图标变化

        public UIItemSlot(Item[] items, int index, BerryPouch berryPouch)
        {
            this.items = items;// 物品数组
            this.index = index;// 物品槽索引
            this.berryPouch = berryPouch;// 树果袋实例
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
            float borderThickness = 1f; // 边框的厚度
            Color borderColor = Color.Black; // 边框的颜色

            // 绘制边框
            spriteBatch.Draw(borderTexture, new Rectangle(borderRectangle.X, borderRectangle.Y, (int)borderRectangle.Width, (int)borderThickness), borderColor); // 顶部边框
            spriteBatch.Draw(borderTexture, new Rectangle(borderRectangle.X, borderRectangle.Y + borderRectangle.Height - (int)borderThickness, (int)borderRectangle.Width, (int)borderThickness), borderColor); // 底部边框
            spriteBatch.Draw(borderTexture, new Rectangle(borderRectangle.X, borderRectangle.Y + (int)borderThickness, (int)borderThickness, borderRectangle.Height - 2 * (int)borderThickness), borderColor); // 左侧边框
            spriteBatch.Draw(borderTexture, new Rectangle(borderRectangle.X + borderRectangle.Width - (int)borderThickness, borderRectangle.Y + (int)borderThickness, (int)borderThickness, borderRectangle.Height - 2 * (int)borderThickness), borderColor);

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
                float scale = Math.Min(1f, 30f / (texture.Width + texture.Height)*2); // 48f 是物品图标大小的最大限制

                var frame = Main.itemAnimations[item.type]?.GetFrame(texture) ?? texture.Frame();
                var drawPosition = GetDimensions().Position() + new Vector2(25f) - frame.Size() * 0.5f * scale; // 调整绘制位置以适应缩放
                spriteBatch.Draw(texture, drawPosition, frame, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                if (item.stack > 1) // 如果物品数量大于 1
                {
                    // 绘制物品数量
                    var font = FontAssets.ItemStack.Value;
                    var textSize = font.MeasureString(item.stack.ToString());
                    var textPosition = drawPosition + new Vector2(frame.Width * 0.5f - textSize.X * 0.5f, frame.Height * 0.5f - textSize.Y * 0.5f);
                    spriteBatch.DrawString(font, item.stack.ToString(), textPosition + new Vector2(0f, 16f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
            if (isMouseOver) // 如果鼠标移入物品槽，绘制一个半透明的覆盖层来防止点击
            {
                if (index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir)
                {
                    ////输出调试物品类型
                    //if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.G))
                    //{
                    //    Main.NewText("物品类型：" + items[index].type);
                    //}

                    Main.hoverItemName = this.items[index].Name;
                    Main.HoverItem = this.items[index].Clone();
                }

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), Color.White * 0.05f);
            }
        }

        [Obsolete]
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl))
            {
                // 快速丢弃物品到垃圾桶
                if (index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir)
                {
                    Main.LocalPlayer.trashItem = items[index].Clone();
                    berryPouch.RemoveItem(index);
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
                }
                
            }
            else if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
            {
                // 快速放入玩家背包
                if (index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir)
                {
                    Main.LocalPlayer.QuickSpawnClonedItem(Main.LocalPlayer.GetSource_Misc("BerryPouch"), items[index], items[index].stack);
                    berryPouch.RemoveItem(index);
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
                }
            }
            else
            {
                if (Main.mouseItem.IsAir && index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir) // 如果鼠标没有物品并且物品槽有物品
                {
                    Main.mouseItem = items[index].Clone();
                    berryPouch.RemoveItem(index);
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
                }
                else if (!Main.mouseItem.IsAir && (index >= 0 && index < items.Length && (items[index] == null || items[index].IsAir))) // 如果鼠标有物品并且物品槽没有物品
                {
                    if (Main.mouseItem.type != ModContent.ItemType<BerryPouch>())
                    {
                        items[index] = Main.mouseItem.Clone(); // 放入物品
                        Main.mouseItem.TurnToAir(); // 清空鼠标携带的物品
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
                    }
                }
                else if (!Main.mouseItem.IsAir && index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir) // 如果鼠标有物品并且物品槽有物品
                {
                    if (Main.mouseItem.type != ModContent.ItemType<BerryPouch>())
                    {
                        if (Main.mouseItem.type == items[index].type && Main.mouseItem.stack < items[index].maxStack)
                        {
                            items[index].stack += Main.mouseItem.stack; // 合并物品
                            Main.mouseItem.TurnToAir(); // 清空鼠标携带的物品
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
                        }
                        else
                        {
                            var temp = items[index].Clone();
                            items[index] = Main.mouseItem.Clone();
                            Main.mouseItem = temp;
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
                        }
                    }
                }
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            if (!Main.mouseItem.IsAir && index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir) // 如果鼠标有物品并且物品槽有物品
            {
                //如果物品可以被堆叠
                if (Main.mouseItem.type == items[index].type && Main.mouseItem.stack < items[index].maxStack) // 如果鼠标上的物品类型与物品槽中的物品类型相同
                {
                    // 增加鼠标上物品的堆叠数量
                    Main.mouseItem.stack++;
                    //减少物品槽中的物品堆叠数
                    items[index].stack--;
                    // 如果物品槽中的物品堆叠数为0，移除物品槽中的物品
                    if (items[index].stack <= 0)
                    {
                        berryPouch.RemoveItem(index);
                    }
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick); // 播放音效
                }
            }
            else if (Main.mouseItem.IsAir && index >= 0 && index < items.Length && items[index] != null && !items[index].IsAir) // 如果鼠标没有物品并且物品槽有物品
            {
                if (items[index].stack > 1)
                {
                    // 获得一个物品
                    Main.mouseItem = items[index].Clone();
                    Main.mouseItem.stack = 1;
                    // 减少物品槽中的物品堆叠数
                    items[index].stack--;
                    // 如果物品槽中的物品堆叠数为0，移除物品槽中的物品
                    if (items[index].stack <= 0)
                    {
                        berryPouch.RemoveItem(index);
                    }
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick); // 播放音效
                }
                else
                {
                    var temp = items[index].Clone();
                    items[index] = Main.mouseItem.Clone();
                    Main.mouseItem = temp;
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick); // 播放音效
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
            //base.Update(gameTime);

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

    //UIPanel 继承自 UIElement，可以添加子元素，并提供拖动、缩放等功能
    public class DraggableUIPanel : UIPanel
    {
        private bool dragging;
        private Vector2 offset;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }

        // 左键按下事件处理：开始拖动
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            // 检查鼠标是否在物品槽或进度条范围内
            bool isInItemSlotOrScrollbar = IsMouseInItemSlotOrScrollbar(evt.MousePosition, this);

            // 如果鼠标不在物品槽或进度条范围内，开始拖动
            if (!isInItemSlotOrScrollbar)
            {
                offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels); // 计算偏移量
                dragging = true; // 开始拖动
            }
        }

        // 递归检查鼠标是否在物品槽或进度条范围内
        private bool IsMouseInItemSlotOrScrollbar(Vector2 mousePosition, UIElement element)
        {
            foreach (var child in element.Children)
            {
                if (child is UIItemSlot itemSlot && itemSlot.ContainsPoint(mousePosition))
                {
                    return true;
                }
                if (child is UIScrollbar scrollbar && scrollbar.ContainsPoint(mousePosition))
                {
                    return true;
                }
                if (child is CustomHeaderPanel header && header.ContainsPoint(mousePosition))
                {
                    return false;
                }
                if (IsMouseInItemSlotOrScrollbar(mousePosition, child))
                {
                    return true;
                }
            }
            return false;
        }

        // 左键释放事件处理：结束拖动
        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false; // 结束拖动
        }

        // 更新事件处理：处理拖动逻辑
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Main.playerInventory)
            {
                // 隐藏面板
                ModContent.GetInstance<BerryPouchSystem>().HideUI();
            }
            
            if (dragging) // 如果正在拖动
            {
                float newLeft = Main.mouseX - offset.X;
                float newTop = Main.mouseY - offset.Y;

                Left.Set(newLeft, 0f); // 设置新的左位置
                Top.Set(newTop, 0f); // 设置新的顶位置
                Recalculate(); // 重新计算位置
            }
            Player player = Main.player[Main.myPlayer];
            //获得鼠标位置
            Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);
            //如果鼠标移入面板，阻止物品使用
            if (mousePos.X > Left.Pixels && mousePos.X < Left.Pixels + Width.Pixels && mousePos.Y > Top.Pixels && mousePos.Y < Top.Pixels + Height.Pixels
                || BerryPouchSystem.OpenBerryPouchKeybind.Current
                )
            {
                player.mouseInterface = true;
            }
        }
    }
    public class CustomHeaderPanel : UIPanel
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //base.DrawSelf(spriteBatch);

            // 自定义绘制逻辑
            var dimensions = GetDimensions();
            var backgroundColor = new Color(73, 94, 171); // 自定义背景颜色
            var borderColor = Color.LightBlue*0.2f; // 自定义边框颜色
            var titleColor = Color.White*0.2f; // 自定义标题颜色
            
            // 绘制背景
            //spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimensions.ToRectangle(), backgroundColor);

            // 绘制顶部底边框
            var borderThickness = 2;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)dimensions.X -10, (int)dimensions.Y + (int)dimensions.Height - borderThickness, (int)dimensions.Width + 20, borderThickness), titleColor); // 底部边框
            
            Texture2D texture1 = ModContent.Request<Texture2D>("Pokemon/Textures/UI/BerryPouch/BerryPouchHead1").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>("Pokemon/Textures/UI/BerryPouch/BerryPouchHead2").Value;

            if (ModContent.GetInstance<BerryPouchSystem>().ShouldDrawCustomCursor)
            {
                spriteBatch.Draw(texture2, new Rectangle((int)dimensions.X + 269, (int)dimensions.Y - 10, texture1.Width, texture1.Height),Color.White * 0.6f);
            }else
                spriteBatch.Draw(texture1, new Rectangle((int)dimensions.X + 269, (int)dimensions.Y - 10, texture1.Width, texture1.Height), Color.White * 0.6f);

            // 绘制标题文本
            var title = Language.ActiveCulture.Name == "zh-Hans" ? "树果袋" : "Berry Pouch";
            var font = FontAssets.MouseText.Value;
            var textSize = font.MeasureString(title);
            var textPosition = new Vector2(dimensions.X + (dimensions.Width - textSize.X) / 2 - 270, dimensions.Y + (dimensions.Height - textSize.Y) / 2);
            // 添加分隔符提示并动态变化颜色
            float lineLerpFactor = (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2) + 1) / 2; // 动态变化颜色

            // 动态变化颜色
            Color color1 = Color.Lerp(Color.LightSteelBlue, Color.White, lineLerpFactor); // 这里只做了粉色到白色的变化
            
            //正弦函数设置颜色变化
            spriteBatch.DrawString(font, title, textPosition, color1);
        }
    }
    
    //UIState 继承自 UIElement，提供 UI 状态切换、绘制等功能
    public class BerryPouchUI : UIState
    {
        internal DraggableUIPanel mainPanel; // 将访问修饰符更改为 internal
        private CustomHeaderPanel headerPanel; // 头部面板
        private UIGrid itemGrid;
        private UIScrollbar scrollbar;
        private BerryPouch berryPouch;
        private UIImageButton closeButton;
        private UIImageButton sortButton; // 在 BerryPouchUI 类中添加字段
        private UIImageButton storeAllButton;
        private UIImageButton takeAllButton;

        public BerryPouchUI()
        {
            mainPanel = new DraggableUIPanel(); // 创建可拖动面板
            mainPanel.SetPadding(10); // 设置内边距
            mainPanel.Left.Set(680f, 0f); // 设置左侧位置
            mainPanel.Top.Set(140f, 0f); // 设置顶部位置
            mainPanel.Width.Set(620f, 0f); // 设置宽度为原来的1.5倍
            mainPanel.Height.Set(455f, 0f); // 设置高度
            Append(mainPanel); // 添加到 UI 状态中

            // 创建头部面板
            headerPanel = new CustomHeaderPanel();
            headerPanel.Width.Set(0, 1f);
            headerPanel.Height.Set(30f, 0f);
            headerPanel.Top.Set(0f, 0f);
            mainPanel.Append(headerPanel);

            // 创建关闭按钮
            closeButton = new UIImageButton(ModContent.Request<Texture2D>("Pokemon/Textures/UI/CoolDown"));
            closeButton.Width.Set(24, 0f);
            closeButton.Height.Set(24, 0f);
            closeButton.Top.Set(-2f, 0f);
            closeButton.Left.Set(-23f, 1f); // 右上角位置
            closeButton.OnLeftClick += CloseButton_OnClick; // 绑定点击事件
            mainPanel.Append(closeButton); // 添加到头部面板

            // 创建物品网格
            itemGrid = new UIGrid(); // 使用自定义的 UIGrid
            itemGrid.Width.Set(-20f, 1f); // 设置宽度
            itemGrid.Height.Set(-40f, 1f); // 设置高度，减去头部和尾部面板的高度
            itemGrid.Top.Set(40f, 0f); // 设置顶部位置，避开头部面板
            itemGrid.ListPadding = 5.3f; // 设置列表间隔
            mainPanel.Append(itemGrid); // 添加到主面板中

            // 创建滚动条
            scrollbar = new UIScrollbar(); // 创建滚动条
            scrollbar.SetView(100f, 1000f); // 设置滚动条视图
            scrollbar.Height.Set(-40f, 1f); // 设置高度，减去头部和尾部面板的高度
            scrollbar.Top.Set(40f, 0f); // 设置顶部位置，避开头部面板
            scrollbar.Left.Set(-20f, 1f); // 设置左侧位置
            mainPanel.Append(scrollbar); // 添加到主面板中

            itemGrid.SetScrollbar(scrollbar); // 设置物品网格的滚动条
            
            // 创建整理按钮
            sortButton = new UIImageButton(ModContent.Request<Texture2D>("Pokemon/Textures/UI/BerryPouch/ButtonFavorite")); // 可自定义图标
            sortButton.Width.Set(24, 0f);
            sortButton.Height.Set(24, 0f);
            sortButton.Top.Set(-4f, 0f);
            sortButton.Left.Set(-60f, 1f); // 右上角，靠近关闭按钮
            sortButton.OnLeftClick += SortButton_OnClick;
            mainPanel.Append(sortButton);

            // 一键存入按钮
            storeAllButton = new UIImageButton(ModContent.Request<Texture2D>("Pokemon/Textures/UI/BerryPouch/ButtonStoreAll")); // 你需要准备一张合适的图标
            storeAllButton.Width.Set(24, 0f);
            storeAllButton.Height.Set(24, 0f);
            storeAllButton.Top.Set(-4f, 0f);
            storeAllButton.Left.Set(-90f, 1f); // 右上角，靠近整理按钮
            storeAllButton.OnLeftClick += StoreAllButton_OnClick;
            mainPanel.Append(storeAllButton);

            // 一键取出按钮
            takeAllButton = new UIImageButton(ModContent.Request<Texture2D>("Pokemon/Textures/UI/BerryPouch/ButtonTakeAll")); // 你需要准备一张合适的图标
            takeAllButton.Width.Set(24, 0f);
            takeAllButton.Height.Set(24, 0f);
            takeAllButton.Top.Set(-4f, 0f);
            takeAllButton.Left.Set(-120f, 1f); // 右上角，靠近存入按钮
            takeAllButton.OnLeftClick += TakeAllButton_OnClick;
            mainPanel.Append(takeAllButton);
        }
        private void StoreAllButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            berryPouch?.StoreAllFromInventory(Main.LocalPlayer);
            SetBerryPouch(berryPouch); // 刷新UI
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Grab);
        }

        private void TakeAllButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            berryPouch?.TakeAllToInventory(Main.LocalPlayer);
            SetBerryPouch(berryPouch); // 刷新UI
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Grab);
        }
        private void SortButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            berryPouch?.SortItems();
            SetBerryPouch(berryPouch); // 刷新UI
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuTick);
        }
        private void CloseButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            // 隐藏面板
            ModContent.GetInstance<BerryPouchSystem>().HideUI();
        }

        // 设置物品袋
        public void SetBerryPouch(BerryPouch berryPouch)
        {
            this.berryPouch = berryPouch; // 设置物品袋
            itemGrid.Clear(); // 清空物品网格

            for (int i = 0; i < BerryPouch.MaxItems; i++)
            {
                var itemSlot = new UIItemSlot(berryPouch.items, i, berryPouch); // 创建物品槽
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
    public class BerryPouchSystem : ModSystem
    {
        private UserInterface berryPouchInterface;
        internal BerryPouchUI berryPouchUI;
        public static ModKeybind OpenBerryPouchKeybind; // 添加自定义按键绑定
        public bool ShouldDrawCustomCursor { get; set; } // 添加标志

        private float cursorScale = 1.0f; // 初始缩放因子
        private bool scalingUp = true; // 缩放方向

        // 加载事件处理：初始化物品袋界面
        public override void Load()
        {
            if (!Main.dedServ) // 如果不是服务器模式
            {
                berryPouchUI = new BerryPouchUI(); // 创建物品袋界面
                berryPouchInterface = new UserInterface(); // 创建用户界面
            }
            OpenBerryPouchKeybind = KeybindLoader.RegisterKeybind(Mod, Language.ActiveCulture.Name == "zh-Hans" ? "树果袋快捷存放" : "Berry Pouch Quick-Deposit", "B"); // 注册自定义按键绑定，默认是 B 键
        }

        // 卸载事件处理：清理自定义按键绑定
        public override void Unload()
        {
            OpenBerryPouchKeybind = null;
        }

        // 更新 UI
        public override void UpdateUI(GameTime gameTime)
        {
            if (berryPouchInterface?.CurrentState != null) // 如果当前状态不为空
            {
                berryPouchInterface.Update(gameTime); // 更新 UI
            }

            // 更新缩放因子
            if (scalingUp)
            {
                cursorScale += 0.003f;
                if (cursorScale >= 1.03f)
                {
                    scalingUp = false;
                }
            }
            else
            {
                cursorScale -= 0.003f;
                if (cursorScale <= 0.97f)
                {
                    scalingUp = true;
                }
            }
        }

        // 修改界面层：插入自定义的物品袋界面层
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory")); // 找到库存界面层
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                    "Pokemon: Berry Pouch",
                    delegate
                    {
                        if (berryPouchInterface?.CurrentState != null) // 如果当前状态不为空
                        {
                            berryPouchInterface.Draw(Main.spriteBatch, new GameTime()); // 绘制 UI
                        }
                        return true;
                    },
                    InterfaceScaleType.UI) // 设置界面缩放类型
                );
            }

            // 添加一个新的绘制层，用于绘制自定义光标
            layers.Add(new LegacyGameInterfaceLayer(
                "Pokemon: Custom Cursor",
                delegate
                {
                    // 绘制自定义光标
                    if (ShouldDrawCustomCursor)
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Textures/UI/BerryPouch/BerryPouchHead1").Value;
                        Vector2 position = Main.MouseScreen + new Vector2(-3f, -9f);
                        Main.spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, cursorScale * 0.88f, SpriteEffects.None, 0f);
                        ShouldDrawCustomCursor = false; // 重置标志
                    }
                    return true;
                },
                InterfaceScaleType.UI) // 设置界面缩放类型
            );
        }

        // 切换 UI 状态：打开或关闭物品袋界面
        public void ToggleUI(BerryPouch berryPouch)
        {
            if (berryPouchInterface.CurrentState == null) // 如果当前状态为空
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuOpen); // 播放打开音效
                berryPouchUI.SetBerryPouch(berryPouch); // 设置物品袋
                berryPouchInterface.SetState(berryPouchUI); // 设置 UI 状态为物品袋界面
            }
            else
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuClose); // 播放关闭音效
                berryPouchInterface.SetState(null); // 关闭物品袋界面
            }
        }

        // 隐藏 UI
        public void HideUI()
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuClose); // 播放关闭音效
            berryPouchInterface.SetState(null); // 关闭物品袋界面
        }

        // 检查 UI 是否可见
        public bool IsUIVisible()
        {
            return berryPouchInterface?.CurrentState != null;
        }
    }
}