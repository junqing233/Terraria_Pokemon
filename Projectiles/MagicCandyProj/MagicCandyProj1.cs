using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;
using Pokemon.Content.Items;
using Pokemon.Content.Equipment;

namespace Pokemon.Projectiles.MagicCandyProj
{
    public class MagicCandyProj1 : ModProjectile
    {
        //加新徽章需要添加神奇糖果的使用对象
        Player player => Main.player[Projectile.owner];
        private bool isshow = false;

        public override void SetDefaults()
        {
            Projectile.width = 62; // 弹幕宽度
            Projectile.height = 62; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.penetrate = -1; // 无限穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 600; // 存在时间无限
            Projectile.alpha = 100; // 透明度
            Projectile.light = 0.75f; // 发光亮度
            Projectile.aiStyle = -1; // 不使用原版AI
            base.SetDefaults();
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5; // 设置动画帧数
            base.SetStaticDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.damage = 0; // 伤害为0
            Projectile.Center = player.Center + new Vector2(0, -60); // 弹幕初始位置
            Projectile.velocity = Vector2.Zero; // 弹幕初始速度
        }

        public override bool? CanCutTiles()
        {
            return false;//我们不想召唤兽会割草
        }

        public override void AI()
        {
            // 更新当前帧
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 12) // 每12帧切换一次
            {
                Projectile.frameCounter = 0; // 重置计数器
                Projectile.frame++; // 切换帧
                if (Projectile.frame >= Main.projFrames[Projectile.type]) // 确保根据数量循环切换
                {
                    Projectile.Kill(); // 销毁弹幕
                }
            }

            // 检查装备的物品
            bool isEquipped_SunflowerBall = false;
            bool isEquipped_GastlyBadge = false;
            bool isEquipped_CharmanderBadge = false;
            bool isEquipped_BulbasaurBadge = false;
            bool isEquipped_SquirtleBadge = false;
            bool isEquipped_TaillowBadge = false;
            bool isEquipped_SpoinkBadge = false;
            bool isEquipped_BeldumBadge = false;
            bool isEquipped_WingullBadge = false;
            bool isEquipped_VoltorbBadge = false;
            bool isEquipped_MunchlaxBadge = false;
            bool isEquipped_FomantisBadge = false;
            bool isEquipped_TrapinchBadge = false;
            bool isEquipped_PikachuBadge = false;

            // 检查MagicCandyUI的物品槽
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item != null)
            {
                if (MagicCandyUI.Instance.itemSlot.item.ModItem is SunflowerBall)
                {
                    isEquipped_SunflowerBall = true;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is GastlyBadge)
                {
                    isEquipped_GastlyBadge = true;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is CharmanderBadge)
                {
                    isEquipped_CharmanderBadge = true;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is BulbasaurBadge)
                {
                    isEquipped_BulbasaurBadge = true;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is SquirtleBadge)
                {
                    isEquipped_SquirtleBadge = true;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is TaillowBadge)
                {
                    isEquipped_TaillowBadge = true;
                }
                else if (MagicCandyUI.Instance.itemSlot.item.ModItem is SpoinkBadge)
                {
                    isEquipped_SpoinkBadge = true;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is BeldumBadge)
                {
                    isEquipped_BeldumBadge = true;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is WingullBadge)
                {
                    isEquipped_WingullBadge = true;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is VoltorbBadge)
                {
                    isEquipped_VoltorbBadge = true;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is MunchlaxBadge)
                {
                    isEquipped_MunchlaxBadge = true;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is FomantisBadge)
                {
                    isEquipped_FomantisBadge = true;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is TrapinchBadge)
                {
                    isEquipped_TrapinchBadge = true;
                }else if (MagicCandyUI.Instance.itemSlot.item.ModItem is PikachuBadge)
                {
                    isEquipped_PikachuBadge = true;
                }
            }

            //向日种子效果
            if (isEquipped_SunflowerBall)
            {
                // 升级向日种子
                SunflowerBall sunflowerBall = GetPlayerSunflowerBall();
                if (sunflowerBall != null)
                {
                    if (sunflowerBall.level == 5 && !isshow)
                    {
                        sunflowerBall.level++; // 等级
                        sunflowerBall.Item.damage = sunflowerBall.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "向日种子升级了\n学会了烦恼种子！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (sunflowerBall.level == 8 && !isshow)
                    {
                        sunflowerBall.level++; // 等级
                        sunflowerBall.Item.damage = sunflowerBall.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "向日种子升级了\n学会了吸取！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (sunflowerBall.level == 11 && !isshow)
                    {
                        sunflowerBall.level++; // 等级
                        sunflowerBall.Item.damage = sunflowerBall.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "向日种子升级了\n学会了撞击！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        sunflowerBall.level++; // 等级
                        sunflowerBall.Item.damage = sunflowerBall.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "向日种子升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            //鬼斯效果
            if (isEquipped_GastlyBadge)
            {
                // 升级鬼斯
                GastlyBadge gastlyBadge = GetPlayerGastlyBadge();
                if (gastlyBadge != null)
                {
                    if (gastlyBadge.level == 5 && !isshow)
                    {
                        gastlyBadge.level++; // 等级
                        gastlyBadge.Item.damage = gastlyBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "鬼斯升级了\n学会了黑色目光！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (gastlyBadge.level == 8 && !isshow)
                    {
                        gastlyBadge.level++; // 等级
                        gastlyBadge.Item.damage = gastlyBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "鬼斯升级了\n学会了催眠术！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (gastlyBadge.level == 11 && !isshow)
                    {
                        gastlyBadge.level++; // 等级
                        gastlyBadge.Item.damage = gastlyBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "鬼斯升级了\n学会了食梦！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        gastlyBadge.level++; // 等级
                        gastlyBadge.Item.damage = gastlyBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "鬼斯升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            //小火龙效果
            if (isEquipped_CharmanderBadge)
            {
                // 升级小火龙
                CharmanderBadge charmanderBadge = GetPlayerCharmanderBadge();
                if (charmanderBadge != null)
                {
                    if (charmanderBadge.level == 5 && !isshow)
                    {
                        charmanderBadge.level++; // 等级
                        charmanderBadge.Item.damage = charmanderBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "小火龙升级了\n学会了合金爪！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (charmanderBadge.level == 8 && !isshow)
                    {
                        charmanderBadge.level++; // 等级
                        charmanderBadge.Item.damage = charmanderBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "小火龙升级了\n学会了抓！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (charmanderBadge.level == 11 && !isshow)
                    {
                        charmanderBadge.level++; // 等级
                        charmanderBadge.Item.damage = charmanderBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "小火龙升级了\n学会了剑舞！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        charmanderBadge.level++; // 等级
                        charmanderBadge.Item.damage = charmanderBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "小火龙升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            //妙蛙种子效果
            if (isEquipped_BulbasaurBadge)
            {
                // 升级妙蛙种子
                BulbasaurBadge bulbasaurBadge = GetPlayerBulbasaurBadge();
                if (bulbasaurBadge != null)
                {
                    if (bulbasaurBadge.level == 5 && !isshow)
                    {
                        bulbasaurBadge.level++; // 等级
                        bulbasaurBadge.Item.damage = bulbasaurBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "妙蛙种子升级了\n学会了毒粉！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (bulbasaurBadge.level == 8 && !isshow)
                    {
                        bulbasaurBadge.level++; // 等级
                        bulbasaurBadge.Item.damage = bulbasaurBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "妙蛙种子升级了\n学会了寄生种子！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (bulbasaurBadge.level == 11 && !isshow)
                    {
                        bulbasaurBadge.level++; // 等级
                        bulbasaurBadge.Item.damage = bulbasaurBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "妙蛙种子升级了\n学会了守住！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        bulbasaurBadge.level++; // 等级
                        bulbasaurBadge.Item.damage = bulbasaurBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "妙蛙种子升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            //杰尼龟效果
            if (isEquipped_SquirtleBadge)
            {
                // 升级杰尼龟
                SquirtleBadge squirtleBadge = GetPlayerSquirtleBadge();
                if (squirtleBadge != null)
                {
                    if (squirtleBadge.level == 5 && !isshow)
                    {
                        squirtleBadge.level++; // 等级
                        squirtleBadge.Item.damage = squirtleBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "杰尼龟升级了\n学会了冰雹！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (squirtleBadge.level == 8 && !isshow)
                    {
                        squirtleBadge.level++; // 等级
                        squirtleBadge.Item.damage = squirtleBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "杰尼龟升级了\n学会了撞击！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (squirtleBadge.level == 11 && !isshow)
                    {
                        squirtleBadge.level++; // 等级
                        squirtleBadge.Item.damage = squirtleBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "杰尼龟升级了\n学会了水环流！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        squirtleBadge.level++; // 等级
                        squirtleBadge.Item.damage = squirtleBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "杰尼龟升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            //傲骨燕效果
            if (isEquipped_TaillowBadge)
            {
                // 升级傲骨燕
                TaillowBadge taillowBadge = GetPlayerTaillowBadge();
                if (taillowBadge != null)
                {
                    if (taillowBadge.level == 5 && !isshow)
                    {
                        taillowBadge.level++; // 等级
                        taillowBadge.Item.damage = taillowBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "傲骨燕升级了\n学会了燕返！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (taillowBadge.level == 8 && !isshow)
                    {
                        taillowBadge.level++; // 等级
                        taillowBadge.Item.damage = taillowBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "傲骨燕升级了\n学会了翅膀攻击！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (taillowBadge.level == 11 && !isshow)
                    {
                        taillowBadge.level++; // 等级
                        taillowBadge.Item.damage = taillowBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "傲骨燕升级了\n学会了羽栖！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        taillowBadge.level++; // 等级
                        taillowBadge.Item.damage = taillowBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "傲骨燕升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            //跳跳猪效果
            if (isEquipped_SpoinkBadge)
            {
                // 升级跳跳猪
                SpoinkBadge spoinkBadge = GetPlayerSpoinkBadge();
                if (spoinkBadge != null)
                {
                    if (spoinkBadge.level == 5 && !isshow)
                    {
                        spoinkBadge.level++; // 等级
                        spoinkBadge.Item.damage = spoinkBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "跳跳猪升级了\n学会了精神冲击！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (spoinkBadge.level == 8 && !isshow)
                    {
                        spoinkBadge.level++; // 等级
                        spoinkBadge.Item.damage = spoinkBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "跳跳猪升级了\n学会了弹跳！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (spoinkBadge.level == 11 && !isshow)
                    {
                        spoinkBadge.level++; // 等级
                        spoinkBadge.Item.damage = spoinkBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "跳跳猪升级了\n学会了魔法反射！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        spoinkBadge.level++; // 等级
                        spoinkBadge.Item.damage = spoinkBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "跳跳猪升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            if(isEquipped_BeldumBadge)
            {
                // 升级铁哑铃
                BeldumBadge beldumBadge = GetPlayerBeldumBadge();
                if (beldumBadge != null)
                {
                    if (beldumBadge.level == 5 && !isshow)
                    {
                        beldumBadge.level++; // 等级
                        beldumBadge.Item.damage = beldumBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "铁哑铃升级了\n学会了铁头！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (beldumBadge.level == 8 && !isshow)
                    {
                        beldumBadge.level++; // 等级
                        beldumBadge.Item.damage = beldumBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "铁哑铃升级了\n学会了铁壁！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (beldumBadge.level == 11 && !isshow)
                    {
                        beldumBadge.level++; // 等级
                        beldumBadge.Item.damage = beldumBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "铁哑铃升级了\n学会了意念头锤！"); // 显示文本提示
                        isshow = true;
                    }else if (!isshow)
                    {
                        beldumBadge.level++; // 等级
                        beldumBadge.Item.damage = beldumBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "铁哑铃升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            if(isEquipped_WingullBadge)
            {
                // 升级长翅鸥
                WingullBadge wingullBadge = GetPlayerWingullBadge();
                if (wingullBadge != null)
                {
                    if (wingullBadge.level == 5 && !isshow)
                    {
                        wingullBadge.level++; // 等级
                        wingullBadge.Item.damage = wingullBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "长翅鸥升级了\n学会了电光一闪！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (wingullBadge.level == 8 && !isshow)
                    {
                        wingullBadge.level++; // 等级
                        wingullBadge.Item.damage = wingullBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "长翅鸥升级了\n学会了翅膀攻击！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (wingullBadge.level == 11 && !isshow)
                    {
                        wingullBadge.level++; // 等级
                        wingullBadge.Item.damage = wingullBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "长翅鸥升级了\n学会了白雾！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        wingullBadge.level++; // 等级
                        wingullBadge.Item.damage = wingullBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "长翅鸥升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            if(isEquipped_VoltorbBadge)
            {
                // 升级雷电球
                VoltorbBadge voltorbBadge = GetPlayerVoltorbBadge();
                if (voltorbBadge != null)
                {
                    if (voltorbBadge.level == 5 && !isshow)
                    {
                        voltorbBadge.level++; // 等级
                        voltorbBadge.Item.damage = voltorbBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "雷电球升级了\n学会了高速星星！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (voltorbBadge.level == 8 && !isshow)
                    {
                        voltorbBadge.level++; // 等级
                        voltorbBadge.Item.damage = voltorbBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "雷电球升级了\n学会了雷球！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (voltorbBadge.level == 11 && !isshow)
                    {
                        voltorbBadge.level++; // 等级
                        voltorbBadge.Item.damage = voltorbBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "雷电球升级了\n学会了打雷！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        voltorbBadge.level++; // 等级
                        voltorbBadge.Item.damage = voltorbBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "雷电球升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            if(isEquipped_MunchlaxBadge)
            {
                // 升级小卡比兽
                MunchlaxBadge munchlaxBadge = GetPlayerMunchlaxBadge();
                if (munchlaxBadge != null)
                {
                    if (munchlaxBadge.level == 5 && !isshow)
                    {
                        munchlaxBadge.level++; // 等级
                        munchlaxBadge.Item.damage = munchlaxBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "小卡比兽升级了\n学会了逐步击破！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (munchlaxBadge.level == 8 && !isshow)
                    {
                        munchlaxBadge.level++; // 等级
                        munchlaxBadge.Item.damage = munchlaxBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "小卡比兽升级了\n学会了变圆！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (munchlaxBadge.level == 11 && !isshow)
                    {
                        munchlaxBadge.level++; // 等级
                        munchlaxBadge.Item.damage = munchlaxBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "小卡比兽升级了\n学会了泰山压顶！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        munchlaxBadge.level++; // 等级
                        munchlaxBadge.Item.damage = munchlaxBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "小卡比兽升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            if (isEquipped_FomantisBadge)
            {
                // 升级伪螳草
                FomantisBadge fomantisBadge = GetPlayerFomantisBadge();
                if (fomantisBadge != null)
                {
                    if (fomantisBadge.level == 5 && !isshow)
                    {
                        fomantisBadge.level++; // 等级
                        fomantisBadge.Item.damage = fomantisBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "伪螳草升级了\n学会了十字剪！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (fomantisBadge.level == 8 && !isshow)
                    {
                        fomantisBadge.level++; // 等级
                        fomantisBadge.Item.damage = fomantisBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "伪螳草升级了\n学会了光合作用！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (fomantisBadge.level == 11 && !isshow)
                    {
                        fomantisBadge.level++; // 等级
                        fomantisBadge.Item.damage = fomantisBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "伪螳草升级了\n学会了叶刃！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        fomantisBadge.level++; // 等级
                        fomantisBadge.Item.damage = fomantisBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "伪螳草升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            if (isEquipped_TrapinchBadge)
            {
                // 升级大颚蚁
                TrapinchBadge trapinchBadge = GetPlayerTrapinchBadge();
                if (trapinchBadge != null)
                {
                    if (trapinchBadge.level == 5 && !isshow)
                    {
                        trapinchBadge.level++; // 等级
                        trapinchBadge.Item.damage = trapinchBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "大颚蚁升级了\n学会了岩崩！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (trapinchBadge.level == 8 && !isshow)
                    {
                        trapinchBadge.level++; // 等级
                        trapinchBadge.Item.damage = trapinchBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "大颚蚁升级了\n学会了咬碎！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (trapinchBadge.level == 11 && !isshow)
                    {
                        trapinchBadge.level++; // 等级
                        trapinchBadge.Item.damage = trapinchBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "大颚蚁升级了\n学会了大地之力！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        trapinchBadge.level++; // 等级
                        trapinchBadge.Item.damage = trapinchBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "大颚蚁升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }

            if (isEquipped_PikachuBadge)
            {
                // 升级皮卡丘
                PikachuBadge pikachuBadge = GetPlayerPikachuBadge();
                if (pikachuBadge != null)
                {
                    if (pikachuBadge.level == 5 && !isshow)
                    {
                        pikachuBadge.level++; // 等级
                        pikachuBadge.Item.damage = pikachuBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "皮卡丘升级了\n学会了电光一闪！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (pikachuBadge.level == 8 && !isshow)
                    {
                        pikachuBadge.level++; // 等级
                        pikachuBadge.Item.damage = pikachuBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "皮卡丘升级了\n学会了十万伏特！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (pikachuBadge.level == 11 && !isshow)
                    {
                        pikachuBadge.level++; // 等级
                        pikachuBadge.Item.damage = pikachuBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                        (int)player.position.Y - 40, player.width, player.height), Color.White, "皮卡丘升级了\n学会了影子分身！"); // 显示文本提示
                        isshow = true;
                    }
                    else if (!isshow)
                    {
                        pikachuBadge.level++; // 等级
                        pikachuBadge.Item.damage = pikachuBadge.level; // 伤害加倍
                        CombatText.NewText(new Rectangle((int)player.position.X,
                       (int)player.position.Y - 40, player.width, player.height), Color.White, "皮卡丘升级了"); // 显示文本提示
                        isshow = true;
                    }
                }
            }
            // 重置 isshow 标志位
            //isshow = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Rectangle rectangle = new Rectangle(
                0,
                texture.Height / Main.projFrames[Type] * Projectile.frame,
                texture.Width,
                texture.Height / Main.projFrames[Type]
            );

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                rectangle,
                lightColor,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                new Vector2(2),
                SpriteEffects.None,
                0);

            return false;
        }

        private SunflowerBall GetPlayerSunflowerBall()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is SunflowerBall sunflowerBall)
            {
                return sunflowerBall;
            }
            return null;
        }

        private GastlyBadge GetPlayerGastlyBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is GastlyBadge gastlyBadge)
            {
                return gastlyBadge;
            }
            return null;
        }

        private CharmanderBadge GetPlayerCharmanderBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is CharmanderBadge charmanderBadge)
            {
                return charmanderBadge;
            }
            return null;
        }

        private BulbasaurBadge GetPlayerBulbasaurBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is BulbasaurBadge bulbasaurBadge)
            {
                return bulbasaurBadge;
            }
            return null;
        }

        private SquirtleBadge GetPlayerSquirtleBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is SquirtleBadge squirtleBadge)
            {
                return squirtleBadge;
            }
            return null;
        }

        private TaillowBadge GetPlayerTaillowBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is TaillowBadge taillowBadge)
            {
                return taillowBadge;
            }
            return null;
        }

        private SpoinkBadge GetPlayerSpoinkBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is SpoinkBadge spoinkBadge)
            {
                return spoinkBadge;
            }
            return null;
        }

        private BeldumBadge GetPlayerBeldumBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is BeldumBadge beldumBadge)
            {
                return beldumBadge;
            }
            return null;
        }

        private WingullBadge GetPlayerWingullBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is WingullBadge wingullBadge)
            {
                return wingullBadge;
            }
            return null;
        }

        private VoltorbBadge GetPlayerVoltorbBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is VoltorbBadge voltorbBadge)
            {
                return voltorbBadge;
            }
            return null;
        }

        private MunchlaxBadge GetPlayerMunchlaxBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is MunchlaxBadge munchlaxBadge)
            {
                return munchlaxBadge;
            }
            return null;
        }

        private FomantisBadge GetPlayerFomantisBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is FomantisBadge fomantisBadge)
            {
                return fomantisBadge;
            }
            return null;
        }

        private TrapinchBadge GetPlayerTrapinchBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is TrapinchBadge trapinchBadge)
            {
                return trapinchBadge;
            }
            return null;
        }

        private PikachuBadge GetPlayerPikachuBadge()
        {
            if (MagicCandyUI.Instance != null && MagicCandyUI.Instance.itemSlot.item.ModItem is PikachuBadge pikachuBadge)
            {
                return pikachuBadge;
            }
            return null;
        }
    }
}