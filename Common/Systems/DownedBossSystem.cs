using System.Collections;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Pokemon.Common.Systems
{
	// 作为“Boss已击败”标志的容器。
	// 在Boss的OnKill钩子中这样设置标志：
	//    NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);

	// 保存和加载这些标志需要用TagCompounds，相关指南见wiki：https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound
	public class DownedBossSystem : ModSystem
	{
		public static bool downedBeedrill_Mega = false;
		// public static bool downedOtherBoss = false;

		public override void ClearWorld() {
            downedBeedrill_Mega = false;
			// downedOtherBoss = false;
		}

		// 使用TagCompounds保存数据集。
		// 注意：此处提供的tag实例默认总是空的。
		public override void SaveWorldData(TagCompound tag) {
			if (downedBeedrill_Mega) {
				tag["downedBeedrill_Mega"] = true;
			}

			// if (downedOtherBoss) {
			//	tag["downedOtherBoss"] = true;
			// }
		}

		public override void LoadWorldData(TagCompound tag) {
            downedBeedrill_Mega = tag.ContainsKey("downedBeedrill_Mega");
			// downedOtherBoss = tag.ContainsKey("downedOtherBoss");
		}

		public override void NetSend(BinaryWriter writer) {
			// 参数顺序很重要，必须与NetReceive一致
			writer.WriteFlags(downedBeedrill_Mega/*, downedOtherBoss*/);
			// WriteFlags最多支持8个条目，如果需要同步超过8个标志，请再次调用WriteFlags。

			// 如果需要发送大量标志，比如每个物品类型一个标志，可以用BitArray高效发送。详见Utils.SendBitArray文档。
		}

		public override void NetReceive(BinaryReader reader) {
			// 参数顺序很重要，必须与NetSend一致
			reader.ReadFlags(out downedBeedrill_Mega/*, out downedOtherBoss*/);
			// ReadFlags最多支持8个条目，如果需要同步超过8个标志，请再次调用ReadFlags。
		}
	}
}