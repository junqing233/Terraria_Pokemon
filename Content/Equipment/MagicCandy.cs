using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Pokemon.Content.Items;
using Pokemon.Projectiles.MagicCandyProj;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent;
using Terraria.UI;
using System.Collections.Generic;
using ReLogic.Graphics;
using System;
using Terraria.DataStructures;
using Pokemon.Projectiles.BulbasaurBadgeProj;
using Terraria.Audio;
using Terraria.Localization;
using System.Linq;

namespace Pokemon.Content.Equipment
{
    public class MagicCandy : ModItem
    {
        private bool isClick = false;

        public override void SetDefaults()
        {
            Item.useAnimation = 15; // 使用动画持续时间
            Item.useTime = 15; // 使用时间
            Item.useStyle = ItemUseStyleID.EatFood; // 使用方式
            Item.width = 42; // 宽度
            Item.height = 42; // 高度
            Item.value = Item.buyPrice(gold: 1); // 价值
            Item.rare = ItemRarityID.Green; // 稀有度
            Item.shoot = ModContent.ProjectileType<MagicCandyProj1>(); // 射击类型
            Item.shootSpeed = 1f; // 射击速度
            Item.consumable = true; // 可消耗
        }
        // 修改物品提示信息
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Language.ActiveCulture.Name == "zh-Hans")
            {
                tooltips.Add(new TooltipLine(Mod, "", $"按下 {BerryPouchSystem.OpenBerryPouchKeybind.GetAssignedKeys().FirstOrDefault() ?? "未绑定"} 快捷存放"));
                tooltips.Add(new TooltipLine(Mod, "", "【背包生效】"));
                var openTooltip = (new TooltipLine(Mod, "", MagicCandyUI.Visible ?
                    "右键点击" + "[c/87CEFF:关闭]" + "升级面板" : "右键点击" + "[c/87CEFF:打开]" + "升级面板"));
                tooltips.Add(openTooltip);
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "", $"Press {BerryPouchSystem.OpenBerryPouchKeybind.GetAssignedKeys().FirstOrDefault() ?? "unbound"} to store items"));
                tooltips.Add(new TooltipLine(Mod, "", "【Inventory Effect】"));
              var openTooltip = (new TooltipLine(Mod, "", MagicCandyUI.Visible ?
                    "Right-click to " + "[c/87CEFA:close]" + " the upgrade panel" : "Right-click to " + "[c/87CEFA:open]" + " the upgrade panel"));
                tooltips.Add(openTooltip);
            }
        }
        public override bool CanRightClick()
        {
            if (Main.mouseRight && !isClick)
            {
                if (Main.mouseRightRelease)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        MagicCandyUI.Visible = !MagicCandyUI.Visible;
                        if (MagicCandyUI.Visible)
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuOpen); // 播放打开音效
                        else
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuClose); // 播放关闭音效
                        
                    }
                }
            }
            isClick = Main.mouseRightRelease;
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            bool isEquipped_Sunflow = false;
            bool isEquipped_Gastly = false;
            bool isEquipped_Charmander = false;
            bool isEquipped_Bulbasaur = false;
            bool isEquipped_Squirtle = false;
            bool isEquipped_Taillow = false;
            bool isEquipped_Spoink = false;
            bool isEquipped_Beldum = false;
            bool isEquipped_Wingull = false;
            bool isEquipped_Voltorb = false;
            bool isEquipped_Munchlax = false;
            bool isEquipped_Fomantis = false;
            bool isEquipped_Trapinch = false;
            bool isEquipped_Pikachu = false;

            SunflowerBall sunflowerBall = null;
            GastlyBadge gastlyBadge = null;
            CharmanderBadge charmanderBadge = null;
            BulbasaurBadge bulbasaurBadge = null;
            SquirtleBadge squirtleBadge = null;
            TaillowBadge taillowBadge = null;
            SpoinkBadge spoinkBadge = null;
            BeldumBadge beldumBadge = null;
            WingullBadge wingullBadge = null;
            VoltorbBadge voltorbBadge = null;
            MunchlaxBadge munchlaxBadge = null;
            FomantisBadge fomantisBadge = null;
            TrapinchBadge trapinchBadge = null;
            PikachuBadge pikachuBadge = null;

            // 检查MagicCandyUI的物品槽
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item != null)
            {
                if (MagicCandyUI.Instance.itemSlot.item.ModItem is SunflowerBall sunflower)
                {
                    isEquipped_Sunflow = true;
                    sunflowerBall = sunflower;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is GastlyBadge gastly)
                {
                    isEquipped_Gastly = true;
                    gastlyBadge = gastly;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is CharmanderBadge charmander)
                {
                    isEquipped_Charmander = true;
                    charmanderBadge = charmander;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is BulbasaurBadge bulbasaur)
                {
                    isEquipped_Bulbasaur = true;
                    bulbasaurBadge = bulbasaur;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is SquirtleBadge squirtle)
                {
                    isEquipped_Squirtle = true;
                    squirtleBadge = squirtle;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is TaillowBadge taillow)
                {
                    isEquipped_Taillow = true;
                    taillowBadge = taillow;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is SpoinkBadge spoink)
                {
                    isEquipped_Spoink = true;
                    spoinkBadge = spoink;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is BeldumBadge beldum)
                {
                    isEquipped_Beldum = true;
                    beldumBadge = beldum;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is WingullBadge wingull)
                {
                    isEquipped_Wingull = true;
                    wingullBadge = wingull;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is VoltorbBadge voltorb)
                {
                    isEquipped_Voltorb = true;
                    voltorbBadge = voltorb;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is MunchlaxBadge munchlax)
                {
                    isEquipped_Munchlax = true;
                    munchlaxBadge = munchlax;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is FomantisBadge fomantis)
                {
                    isEquipped_Fomantis = true;
                    fomantisBadge = fomantis;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is TrapinchBadge trapinch)
                {
                    isEquipped_Trapinch = true;
                    trapinchBadge = trapinch;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is PikachuBadge pikachu)
                {
                    isEquipped_Pikachu = true;
                    pikachuBadge = pikachu;
                }
            }

            if (!isEquipped_Sunflow &&
                !isEquipped_Gastly &&
                !isEquipped_Charmander &&
                !isEquipped_Bulbasaur &&
                !isEquipped_Squirtle &&
                !isEquipped_Taillow &&
                !isEquipped_Spoink &&
                !isEquipped_Beldum &&
                !isEquipped_Wingull &&
                !isEquipped_Voltorb &&
                !isEquipped_Munchlax &&
                !isEquipped_Fomantis &&
                !isEquipped_Trapinch &&
                !isEquipped_Pikachu)
            {
                if (Language.ActiveCulture.Name == "zh-Hans")
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                        new Color(200, 250, 250), "请先装备上徽章再使用神奇糖果！"); // 显示文本提示
                else
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                        new Color(200, 250, 250), "Please equip a badge first before using the magic candy!"); // 显示文本提示
                return false;
            }

            if ((isEquipped_Sunflow && sunflowerBall.level < 100) ||
                (isEquipped_Gastly && gastlyBadge.level < 100) ||
                (isEquipped_Charmander && charmanderBadge.level < 100) ||
                (isEquipped_Bulbasaur && bulbasaurBadge.level < 100) ||
                (isEquipped_Squirtle && squirtleBadge.level < 100) ||
                (isEquipped_Taillow && taillowBadge.level < 100) ||
                (isEquipped_Spoink && spoinkBadge.level < 100) ||
                (isEquipped_Beldum && beldumBadge.level < 100) ||
                (isEquipped_Wingull && wingullBadge.level < 100) ||
                (isEquipped_Voltorb && voltorbBadge.level < 100) ||
                (isEquipped_Munchlax && munchlaxBadge.level < 100) ||
                (isEquipped_Fomantis && fomantisBadge.level < 100) ||
                (isEquipped_Trapinch && trapinchBadge.level < 100) ||
                (isEquipped_Pikachu && pikachuBadge.level < 100))
            {
                SoundEngine.PlaySound(new SoundStyle("Pokemon/Music/MagicCandy"), player.Center);
                return true; // 若已装备徽章但徽章等级未达到3级，则允许使用
            }
            else
            {
                if (Language.ActiveCulture.Name == "zh-Hans")
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                        new Color(200, 250, 250), "     徽章等级已满\n无法再使用神奇糖果！"); // 显示文本提示
                else
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                        new Color(200, 250, 250), "     Badge level is full\nCannot use magic candy anymore!"); // 显示文本提示
                return false;
            }
        }
    }
    public class MagicCandyUI : UIState
    {
        public UIPanel mainPanel;
        public static bool Visible;
        public UIItemSlotMagicCandy itemSlot;
        public static MagicCandyUI Instance;
        private UITextButton useButton;

        public override void OnInitialize()
        {
            Instance = this;

            mainPanel = new UIPanel();
            mainPanel.SetPadding(10);
            mainPanel.Left.Set(416f, 0f);
            mainPanel.Top.Set(320f, 0f);
            mainPanel.Width.Set(110f, 0f);
            mainPanel.Height.Set(130f, 0f); // 增加高度以容纳按钮
            Append(mainPanel);

            itemSlot = new UIItemSlotMagicCandy();
            itemSlot.Left.Set(20f, 0f);
            itemSlot.Top.Set(5f, 0f);
            itemSlot.Width.Set(50f, 0f);
            itemSlot.Height.Set(50f, 0f);
            mainPanel.Append(itemSlot);

            useButton = new UITextButton(Language.ActiveCulture.Name == "zh-Hans" ? "升级": "Upgrade", 1f);
            useButton.Left.Set(15f, 0f);
            useButton.Top.Set(70f, 0f); // 设置按钮位置
            useButton.Width.Set(50f, 0f);
            useButton.Height.Set(30f, 0f);
            useButton.OnLeftClick += UseButtonClick;
            mainPanel.Append(useButton);
        }
        
        [Obsolete]
        private void UseButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Player player = Main.LocalPlayer;
            bool isEquipped_Sunflow = false;
            bool isEquipped_Gastly = false;
            bool isEquipped_Charmander = false;
            bool isEquipped_Bulbasaur = false;
            bool isEquipped_Squirtle = false;
            bool isEquipped_Taillow = false;
            bool isEquipped_Spoink = false;
            bool isEquipped_Beldum = false;
            bool isEquipped_Wingull = false;
            bool isEquipped_Voltorb = false;
            bool isEquipped_Munchlax = false;
            bool isEquipped_Fomantis = false;
            bool isEquipped_Trapinch = false;
            bool isEquipped_Pikachu = false;

            SunflowerBall sunflowerBall = null;
            GastlyBadge gastlyBadge = null;
            CharmanderBadge charmanderBadge = null;
            BulbasaurBadge bulbasaurBadge = null;
            SquirtleBadge squirtleBadge = null;
            TaillowBadge taillowBadge = null;
            SpoinkBadge spoinkBadge = null;
            BeldumBadge beldumBadge = null;
            WingullBadge wingullBadge = null;
            VoltorbBadge voltorbBadge = null;
            MunchlaxBadge munchlaxBadge = null;
            FomantisBadge fomantisBadge = null;
            TrapinchBadge trapinchBadge = null;
            PikachuBadge pikachuBadge = null;

            // 检查MagicCandyUI的物品槽
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item != null)
            {
                if (MagicCandyUI.Instance.itemSlot.item.ModItem is SunflowerBall sunflower)
                {
                    isEquipped_Sunflow = true;
                    sunflowerBall = sunflower;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is GastlyBadge gastly)
                {
                    isEquipped_Gastly = true;
                    gastlyBadge = gastly;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is CharmanderBadge charmander)
                {
                    isEquipped_Charmander = true;
                    charmanderBadge = charmander;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is BulbasaurBadge bulbasaur)
                {
                    isEquipped_Bulbasaur = true;
                    bulbasaurBadge = bulbasaur;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is SquirtleBadge squirtle)
                {
                    isEquipped_Squirtle = true;
                    squirtleBadge = squirtle;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is TaillowBadge taillow)
                {
                    isEquipped_Taillow = true;
                    taillowBadge = taillow;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is SpoinkBadge spoink)
                {
                    isEquipped_Spoink = true;
                    spoinkBadge = spoink;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is BeldumBadge beldum)
                {
                    isEquipped_Beldum = true;
                    beldumBadge = beldum;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is WingullBadge wingull)
                {
                    isEquipped_Wingull = true;
                    wingullBadge = wingull;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is VoltorbBadge voltorb)
                {
                    isEquipped_Voltorb = true;
                    voltorbBadge = voltorb;
                }else if(MagicCandyUI.Instance.itemSlot.item.ModItem is MunchlaxBadge munchlax)
                {
                    isEquipped_Munchlax = true;
                    munchlaxBadge = munchlax;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is FomantisBadge fomantis)
                {
                    isEquipped_Fomantis = true;
                    fomantisBadge = fomantis;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is TrapinchBadge trapinch)
                {
                    isEquipped_Trapinch = true;
                    trapinchBadge = trapinch;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is PikachuBadge pikachu)
                {
                    isEquipped_Pikachu = true;
                    pikachuBadge = pikachu;
                }
            }

            if (!isEquipped_Sunflow &&
                !isEquipped_Gastly &&
                !isEquipped_Charmander &&
                !isEquipped_Bulbasaur &&
                !isEquipped_Squirtle &&
                !isEquipped_Taillow &&
                !isEquipped_Spoink &&
                !isEquipped_Beldum &&
                !isEquipped_Wingull &&
                !isEquipped_Voltorb &&
                !isEquipped_Munchlax &&
                !isEquipped_Fomantis &&
                !isEquipped_Trapinch &&
                !isEquipped_Pikachu)
            {
                if (Language.ActiveCulture.Name == "zh-Hans")
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                        new Color(200, 250, 250), "请先装备上徽章再使用神奇糖果！"); // 显示文本提示
                else
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                        new Color(200, 250, 250), "Please equip a badge first before using the magic candy!"); // 显示文本提示
                return;
            }
            if (MagicCandyUI.Instance.itemSlot.item != null && !MagicCandyUI.Instance.itemSlot.item.IsAir)
            {
                if ((isEquipped_Sunflow && sunflowerBall.level < 100) ||
                    (isEquipped_Gastly && gastlyBadge.level < 100) ||
                    (isEquipped_Charmander && charmanderBadge.level < 100) ||
                    (isEquipped_Bulbasaur && bulbasaurBadge.level < 100) ||
                    (isEquipped_Squirtle && squirtleBadge.level < 100) ||
                    (isEquipped_Taillow && taillowBadge.level < 100) ||
                    (isEquipped_Spoink && spoinkBadge.level < 100) ||
                    (isEquipped_Beldum && beldumBadge.level < 100) ||
                    (isEquipped_Wingull && wingullBadge.level < 100) ||
                    (isEquipped_Voltorb && voltorbBadge.level < 100) ||
                    (isEquipped_Munchlax && munchlaxBadge.level < 100) ||
                    (isEquipped_Fomantis && fomantisBadge.level < 100) ||
                    (isEquipped_Trapinch && trapinchBadge.level < 100) ||
                    (isEquipped_Pikachu && pikachuBadge.level < 100))
                {
                    // 使用MagicCandy
                    if (player.ConsumeItem(ModContent.ItemType<MagicCandy>()))
                    {
                        SoundEngine.PlaySound(new SoundStyle("Pokemon/Music/MagicCandy"), player.Center);
                        // 发射MagicCandyProj1弹幕
                        Vector2 position = player.Center;
                        Vector2 velocity = Vector2.Zero;
                        int type = ModContent.ProjectileType<MagicCandyProj1>();
                        int damage = 0;
                        float knockBack = 0f;
                        Projectile.NewProjectile(player.GetSource_Misc("MagicCandy"), position, velocity, type, damage, knockBack, player.whoAmI);
                    }
                }
                else
                {
                    if (Language.ActiveCulture.Name == "zh-Hans")
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                            new Color(200, 250, 250), "     徽章等级已满\n无法再使用神奇糖果！"); // 显示文本提示
                    else
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height),
                            new Color(200, 250, 250), "     Badge level is full\nCannot use magic candy anymore!"); // 显示文本提示
                    return;
                }
            }
        }
        private bool isVisible = false;
        private int VisibleTime = 0;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Player player = Main.LocalPlayer;
            //遍历弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.type == ModContent.ProjectileType<MagicCandyProj1>())
                {
                    isVisible = true;
                    break;
                }
            }
            if(isVisible)
            {
                VisibleTime++;
                if(VisibleTime > 60)
                {
                    // 将物品槽的物品放回背包
                    player.QuickSpawnClonedItem(player.GetSource_Misc("MagicCandy"), MagicCandyUI.Instance.itemSlot.item, MagicCandyUI.Instance.itemSlot.item.stack);
                    MagicCandyUI.Instance.itemSlot.item.TurnToAir();
                    // 关闭UI
                    MagicCandyUI.Visible = false;
                    isVisible = false;
                    VisibleTime = 0;
                }
            }
        
            // 检查鼠标是否在UI面板内
            if (mainPanel.IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            if (!Main.playerInventory)
            {
                // 若背包未打开，则关闭UI
                MagicCandyUI.Visible = false;
            }
        }
    }

    public class UITextButton : UITextPanel<string>
    {
        public UITextButton(string text, float textScale = 1, bool large = false) : base(text, textScale, large)
        {
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            BackgroundColor = new Color(73, 94, 171);
            //BackgroundColor = Color.White;
            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick); // 播放音效
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            BackgroundColor = new Color(63, 82, 151);
            //BackgroundColor = Color.White;
        }
    }
    public class UIItemSlotMagicCandy : UIElement
    {
        public Item item;
        public bool isMouseOver = false; // 添加鼠标移入状态跟踪

        public UIItemSlotMagicCandy()
        {
            item = new Item();
            item.SetDefaults(0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // 绘制物品槽的边框
            spriteBatch.Draw(TextureAssets.InventoryBack9.Value, GetDimensions().ToRectangle(), Color.White * 0.72f);
            Texture2D Texture = ModContent.Request<Texture2D>("Pokemon/Textures/UI/MagicCandy/icon_MagicCandy").Value;
            spriteBatch.Draw(Texture, GetDimensions().ToRectangle(), Color.DarkBlue * 0.125f);

            if (!item.IsAir)
            {
                //var item = items[index];
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
                spriteBatch.Draw(TextureAssets.Item[item.type].Value, drawPosition, frame, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                
                
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
                if (!item.IsAir)
                {
                    Main.hoverItemName = this.item.Name;
                    Main.HoverItem = this.item.Clone();
                }

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), Color.White * 0.02f);
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            isMouseOver = true;
            Main.LocalPlayer.mouseInterface = true; // 在物品槽内设置 mouseInterface
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            isMouseOver = false;
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            if (Main.mouseItem.IsAir && !item.IsAir)
            {
                Main.mouseItem = item.Clone();
                item.TurnToAir();
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
            }
            else if (!Main.mouseItem.IsAir && item.IsAir)
            {
                item = Main.mouseItem.Clone();
                Main.mouseItem.TurnToAir();
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
            }
            else if (!Main.mouseItem.IsAir && !item.IsAir)
            {
                Item temp = item.Clone();
                item = Main.mouseItem.Clone();
                Main.mouseItem = temp;
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab); // 播放音效
            }
        }
    }
    public class PokemonModSystemMagicCandy : ModSystem
    {
        private UserInterface magicCandyInterface;
        private MagicCandyUI magicCandyUI;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                magicCandyUI = new MagicCandyUI();
                magicCandyInterface = new UserInterface();
                magicCandyInterface.SetState(magicCandyUI);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (MagicCandyUI.Visible)
            {
                magicCandyInterface?.Update(gameTime);// 更新UI
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                    "Pokemon: Magic Candy UI",
                    delegate
                    {
                        if (MagicCandyUI.Visible)
                        {
                            magicCandyInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}