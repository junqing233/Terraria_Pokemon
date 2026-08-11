using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Content.Tools
{
	internal class WrapTightlyHookItem : ModItem
	{
		public override void SetDefaults() {
			// 从紫水晶钩子复制属性
			Item.CloneDefaults(ItemID.DiamondHook);
			Item.shootSpeed = 26f; // 定义钩子的射速。
			Item.shoot = ModContent.ProjectileType<WrapTightlyHookProj>(); // 使用此物品时发射钩子的弹幕。
            
			// 如果不使用 Item.CloneDefaults()，则必须设置以下值以确保钩子正常工作：
			// Item.useStyle = ItemUseStyleID.None;
			// Item.useTime = 0;
			// Item.useAnimation = 0;
		}

        
        public override bool CanUseItem(Player player)
        {
            // 每次使用物品，减少玩家最大生命值20%
            int reduce = (int)(player.statLifeMax2 * 0.08f);
            player.statLife -= reduce;
            
            // 如果生命值为0或更低，直接杀死玩家
            if (player.statLife <= 0)
                player.KillMe(PlayerDeathReason.ByCustomReason($"{player.name}被钩爪勒死了！"), 9999, 0);

            return base.CanUseItem(player);
        }
        //// 请参阅 Content/ExampleRecipes.cs 以获取有关配方创建的详细说明。
        //public override void AddRecipes() {
        //	CreateRecipe()
        //		.AddIngredient<ExampleItem>()
        //		.AddTile<Tiles.Furniture.ExampleWorkbench>()
        //		.Register();
        //}
    }

	internal class WrapTightlyHookProj : ModProjectile
	{
		private static Asset<Texture2D> chainTexture;

		public override void Load() // 在加载此内容时，调试被调用一次。
        {   
			// 这是我们将用来表示钩子链条的贴图路径。确保更新它。
			chainTexture = ModContent.Request<Texture2D>("Pokemon/Content/Tools/WrapTightlyHookChain");
		}


		public override void SetStaticDefaults()
		{
			// 如果希望每个玩家只有一份该钩子弹幕，请取消注释此部分。
			ProjectileID.Sets.SingleGrappleHook[Type] = true;
		}


		public override void SetDefaults() 
		{
			Projectile.CloneDefaults(ProjectileID.GemHookDiamond); // 复制紫水晶钩子弹幕的属性。
		}

        // 用于可以在飞行中发射多个钩子的钩子：双钩、网枪、鱼钩、静态钩、月亮钩。
        //public override bool? CanUseGrapple(Player player) {
        //	int hooksOut = 0;
        //	foreach (var projectile in Main.ActiveProjectiles) {
        //		if (projectile.owner == Main.myPlayer && projectile.type == Projectile.type) {
        //			hooksOut++;
        //		}
        //	}

        //	return hooksOut <= 2;
        //}

        // 用于杀死最旧的钩子。对于在射出时杀死最旧的钩子，而不是在新的钩子附着时：如骷髅手。
        // 你也可以像双钩、月亮钩一样更改弹幕。
        // public override void UseGrapple(Player player, ref int type) {
        //     int hooksOut = 0;
        //     int oldestHookIndex = -1;
        //     int oldestHookTimeLeft = 100000;
        //     foreach (var otherProjectile in Main.ActiveProjectiles) {
        //         if (otherProjectile.owner == player.whoAmI && otherProjectile.type == type) {
        //             hooksOut++;
        //             if (otherProjectile.timeLeft < oldestHookTimeLeft) {
        //                 oldestHookIndex = otherProjectile.whoAmI;
        //                 oldestHookTimeLeft = otherProjectile.timeLeft;
        //             }
        //         }
        //     }
        //     if (hooksOut > 1) {
        //         Main.projectile[oldestHookIndex].Kill();
        //     }
        // }
        public override bool? CanCutTiles()
        {
            return false;//我们不想割草
        }
        // 紫水晶钩子的范围是300，静态钩子是600。
        public override float GrappleRange() 
		{
			return 800f;
		}
        public override void AI()
        {
			Projectile.damage = 1; // 定义钩子的伤害。
            
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(ModContent.BuffType<Buffs.BuffsSunflowerProj3>(), 15);
        }
        public override void NumGrappleHooks(Player player, ref int numHooks)
		{
			numHooks = 1; // 可发射的钩子数量
		}

		// 默认值是11，月亮钩是24
		public override void GrappleRetreatSpeed(Player player, ref float speed) 
		{
			speed = 48f; // 钩子达到最大射程后返回的速度
		}

		public override void GrapplePullSpeed(Player player, ref float speed) 
		{
			speed = 21f; // 玩家被拉向钩子弹幕着陆位置的速度
		}

		// 调整玩家被拉向的位置。将使他们悬挂在距被抓目标50像素的距离。
		public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY) 
		{
			Vector2 dirToPlayer = Projectile.DirectionTo(player.Center);
			float hangDist = 10f;
			grappleX += dirToPlayer.X * hangDist;
			grappleY += dirToPlayer.Y * hangDist;
		}

		// 可以自定义此钩子可以附着到的瓷砖，或强制/阻止附着，就像松鼠钩子也附着于树。
		public override bool? GrappleCanLatchOnTo(Player player, int x, int y) {
			// 默认情况下，钩子返回 null，以应用给定瓷砖位置的原版条件（此瓷砖位置可以是空气或调节瓷砖！）
			// 如果您希望在此处返回 true，请确保检查 Main.tile[x, y].HasUnactuatedTile（以及 Main.tileSolid[Main.tile[x, y].TileType] 和/或 Main.tile[x, y].HasTile）

			// 我们让这个钩子像松鼠钩子一样附着到树上

			// 树干不能被调节，所以我们不需要在这里检查。
			//Tile tile = Main.tile[x, y];
			//if (
			//	//tile.TileType == TileID.Plants
   //             //ModContent.TileType<Furnitures.Paintings.VigorothMoverPainting>() == tile.TileType
   //             )
			//{
			//	return true;
			//}
			
			// 在任何其他情况下，像普通钩子一样工作
			return null;
		}
        public override bool PreDraw(ref Color lightColor)
        {
			Player player = Main.player[Projectile.owner]; // 获取玩家
            // 定位钩爪头部的纹理
            Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Content/Tools/WrapTightlyHookProj").Value;

            // 计算钩爪头部的绘制位置
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            // 获取光照颜色
            Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);

            // 确定是否水平翻转钩爪头部
            SpriteEffects spriteEffects = (Projectile.Center.X > player.Center.X) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // 绘制钩爪头部
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, Projectile.rotation, texture.Size() / 2, 0.7f, spriteEffects, 0);

            return false; // 返回 false 阻止原版绘制
        }

        public override bool PreDrawExtras()
        {
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter; // 获取玩家中心位置
            Vector2 center = Projectile.Center; // 获取弹幕中心位置
            Vector2 directionToPlayer = playerCenter - Projectile.Center; // 获取弹幕指向玩家的向量
            float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2; // 获取弹幕的链条旋转角度
            float distanceToPlayer = directionToPlayer.Length(); // 获取弹幕到玩家的距离

            // 定义一个链条的隐藏距离，例如10像素
            float hideDistance = 8f;

            // 如果距离小于隐藏距离，则不绘制链条
            if (distanceToPlayer <= hideDistance)
            {
                return false;
            }

            // 减少距离以隐藏靠近钩爪头的部分链条
            distanceToPlayer -= hideDistance;

            // 计算新的起点，距离钩爪头 hideDistance 像素的位置
            Vector2 start = center + directionToPlayer * hideDistance / distanceToPlayer;

            // 更新中心位置为新的起点
            center = start;

            // 布尔变量用于控制翻转
            bool flipHorizontal = false;

            while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer)) // 确保链条不会超出屏幕范围
            {
                directionToPlayer /= distanceToPlayer; // 获取单位向量
                directionToPlayer *= chainTexture.Height() * 0.8f; // 乘以链环长度

                center += directionToPlayer; // 更新绘制位置
                directionToPlayer = playerCenter - center; // 更新距离
                distanceToPlayer = directionToPlayer.Length(); // 更新距离

                // 计算新的旋转角度
                chainRotation = (playerCenter - center).ToRotation() - MathHelper.PiOver2;

                Color drawColor = Lighting.GetColor((int)center.X / 16, (int)center.Y / 16); // 获取链环颜色

                // 绘制链条
                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
                    chainTexture.Value.Bounds, drawColor, chainRotation,
                    chainTexture.Size() * 0.72f, 0.8f, flipHorizontal ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

                // 切换翻转状态
                flipHorizontal = !flipHorizontal;
            }

            // 停止原版的绘制默认链条
            return false;
        }
    }
}
