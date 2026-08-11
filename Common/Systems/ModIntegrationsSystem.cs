using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Pokemon.Common.Systems
{
	//展示如何使用 Mod.Call 与其他模组进行集成/兼容性支持
	//Mod.Call 的详细介绍在这里：https://github.com/tModLoader/tModLoader/wiki/Expert-Cross-Mod-Content#call-aka-modcall-intermediate

	//这仅展示了一种实现集成的方式，你可以自由探索其他选项和其他模组的示例
	//你需要查找模组开发者提供的资源，了解他们希望你如何添加模组兼容性
	//这可能包括他们的主页、工坊页面、维基、GitHub、Discord、其他联系方式等
	//如果模组是开源的，你可以访问其代码分发平台（通常是 GitHub），并在其中的 Mod 类中查找 "Call"

	//此外，除了此处展示的示例外，ExampleMod 还与 Census Mod（https://steamcommunity.com/sharedfiles/filedetails/?id=2687866031）进行了集成

	//该集成仅通过本地化文件完成，查找 ".hjson" 文件中的 "Census.SpawnCondition"
	public class ModIntegrationsSystem : ModSystem
	{
		private static readonly Version BossChecklistAPIVersion = new Version(1, 6); // 版本设置

		public override void PostSetupContent()
		{
			// 大多数情况下，模组要求你使用 PostSetupContent 钩子来调用它们的方法。这保证了各种数据都已初始化并正确设置
			// Boss Checklist 在其自己的 UI 中显示有关 Boss 的全面信息。我们可以自定义它：
			// https://forums.terraria.org/index.php?threads/.50668/
			DoBossChecklistIntegration();

			// 我们可以在这里通过遵循相同模式与其他模组进行集成。一些模组开发者可能更喜欢为每个集成的模组使用一个 ModSystem，或其他设计。
		}

		private void DoBossChecklistIntegration()
		{

			// 模组的主页链接到其自己的维基，其中解释了调用方法：https://github.com/JavidPack/BossChecklist/wiki/%5B1.4.4%5D-Boss-Log-Entry-Mod-Call
			// 如果我们导航维基，可以找到 "LogBoss" 方法，在这种情况下我们需要它
			// 此方法的一个特性是它会在指定 NPC 类型的本地化文件中为其生成信息条目，因此确保在你的模组运行一次后访问本地化文件进行编辑
			if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist) || bossChecklist.Version < BossChecklistAPIVersion)
			{
				return;
			}

			// 对于某些消息，模组可能在发布时没有它们，因此我们需要验证该方法的最后迭代版本是什么时候被添加的，在这种情况下是 1.6
			// 通常模组开发者会以某种方式提供这些信息，或者可以通过 GitHub 的提交历史/代码审查找到
			//if (bossChecklistMod.Version < new Version(1, 6)) 
			//{
			//	return;
			//}

			// "LogBoss" 方法需要许多参数，定义如下：
			// 你的条目键可以被其他开发者用来提交模组协作数据到你的条目。一旦定义不应更改
			string internalName = "BeedrillMega";
			// 值根据 Boss 进度推断得出，详见维基
			float weight = 6.2f;
			// 用于跟踪进度
			Func<bool> downed = () => DownedBossSystem.downedBeedrill_Mega;
			// Boss 的 NPC 类型
			int bossType = ModContent.NPCType<Content.NPCs.Bosses.Beedrill_Mega.Beedrill_Mega>();
			// 用于召唤 Boss 的物品（如果有的话）
			//int spawnItem = ModContent.ItemType<Content.Items.Consumables.MinionBossSummonItem>();
			// 类似于战利品的物品，如遗物、雕像、面具、宠物
			//List<int> collectibles = new List<int>()
			//{
			//	ItemID.Wood,
			//	ModContent.ItemType<Content.Pets.MinionBossPet.MinionBossPetItem>(),
			//	ModContent.ItemType<Content.Items.Placeable.Furniture.MinionBossTrophy>(),
			//	ModContent.ItemType<Content.Items.Armor.Vanity.MinionBossMask>()
			//};

			// 默认情况下，它会绘制 Boss 的第一帧，如果不需要自定义绘制可以省略
			// 但我们需要绘制最佳生物图鉴纹理，因此我们创建代码来绘制居中在预期位置的纹理
			var customPortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
			   {
				   Texture2D texture = ModContent.Request<Texture2D>("Pokemon/Content/NPCs/Bosses/Beedrill_Mega/Beedrill_Mega_Bestiary_").Value;
				   int frameCount = 12;
				   int frameWidth = texture.Width;
				   int frameHeight = texture.Height / frameCount;
				   // 每5帧切换一次动画帧
				   int frame = (int)((Main.GameUpdateCount / 5) % frameCount);
				   Rectangle sourceRect = new Rectangle(0, frameHeight * frame, frameWidth, frameHeight);

				   Vector2 centered = new Vector2(rect.X + rect.Width / 2 - frameWidth / 2, rect.Y + rect.Height / 2 - frameHeight / 2);
				   sb.Draw(texture, centered, sourceRect, color);
			   };

			bossChecklist.Call(
				"LogBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
					//["spawnItems"] = spawnItem,
					//["collectibles"] = collectibles,// 收集物品列表
					["customPortrait"] = customPortrait,// 自定义图标显示法
					["displayName"] = Language.GetText("超级大针蜂"),// 显示名称
					["spawnInfo"] = Language.GetText("挑战坂木时由板木召唤"),// 召唤信息
					// 根据需要添加其他可选参数，这些参数可以从维基中推断得出
				}
			);

			// 其他 Boss 或其他 Mod.Call 可以在这里进行。
		}
	}
}
