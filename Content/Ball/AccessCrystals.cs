using Terraria;
using Terraria.ModLoader;
using Pokemon.Content.NPCs.Pokemons;
using Pokemon.Content.Weapons.Mterial;
using Pokemon.Content.Items; // 引入结晶物品命名空间

namespace Pokemon.Content.Ball
{
    /// <summary>
    /// 捕捉宝可梦后奖励属性结晶的基类。继承并实现GetCrystalItemType以自定义不同宝可梦对应的结晶。
    /// </summary>
    public abstract class AccessCrystals
    {
        public abstract int GetCrystalItemType(NPC npc);

        public void GiveCrystalReward(Player player, NPC npc)
        {
            int crystalType = GetCrystalItemType(npc);
            if (crystalType > 0)
            {
                player.QuickSpawnItem(player.GetSource_Misc("PokemonCatch"), crystalType);
            }
        }
    }

    /// <summary>
    /// 示例：捕捉BeldumNPC时奖励SteelCrystal或PsychicCrystal
    /// </summary>
    public class SampleAccessCrystals : AccessCrystals
    {
        public override int GetCrystalItemType(NPC npc)
        {
            if (npc.ModNPC is BeldumNPC)//1
            {
                // 随机获得钢属性或超能属性结晶
                if (Main.rand.NextBool())
                    return ModContent.ItemType<SteelCrystal>();
                else
                    return ModContent.ItemType<PsychicCrystal>();
            }
            else if(npc.ModNPC is Bulbasaur)//2
            {
                if (Main.rand.NextBool())
                    return ModContent.ItemType<EmeraldCrystal>();
                else
                    return ModContent.ItemType<ToxicCrystal>();
            }
            else if(npc.ModNPC is Charmander)//3
            {
                return ModContent.ItemType<FlameCrystal>();
            }
            else if(npc.ModNPC is Gastly)//4
            {
                if (Main.rand.NextBool())
                    return ModContent.ItemType<GhostCrystal>();
                else
                    return ModContent.ItemType<ToxicCrystal>();
            }
            else if(npc.ModNPC is Munchlax)//5
            {
                return ModContent.ItemType<NormalCrystal>();
            }
            else if(npc.ModNPC is Spoink)//6
            {
                return ModContent.ItemType<PsychicCrystal>();
            }
            else if(npc.ModNPC is Squirtle)//7
            {
                return ModContent.ItemType<WaveCrystal>();
            }
            else if(npc.ModNPC is Sunflower)//8
            {
                return ModContent.ItemType<EmeraldCrystal>();
            }
            else if(npc.ModNPC is Taillow)//9
            {
                if (Main.rand.NextBool())
                    return ModContent.ItemType<SkyCrystal>();
                else
                    return ModContent.ItemType<NormalCrystal>();
            }
            else if(npc.ModNPC is Voltorb)//10
            {
                return ModContent.ItemType<ThunderCrystal>();
            }
            else if(npc.ModNPC is Wingull)//11
            {
                if (Main.rand.NextBool())
                    return ModContent.ItemType<WaveCrystal>();
                else
                    return ModContent.ItemType<SkyCrystal>();
            }
            else if(npc.ModNPC is Fomantis)//12
            {
                return ModContent.ItemType<EmeraldCrystal>();
            }
            else if(npc.ModNPC is Trapinch)//13
            {
                return ModContent.ItemType<EarthCrystal>();
            }
            else if (npc.ModNPC is Pikachu)//14
            {
                return ModContent.ItemType<ThunderCrystal>();
            }
            return 0;
        }
    }
}