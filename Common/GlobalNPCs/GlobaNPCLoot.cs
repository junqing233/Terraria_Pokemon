using Terraria;
using Terraria.ModLoader;
using Pokemon.Content.Items;
using Pokemon.Content.Equipment;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;

namespace Pokemon.Common.GlobalNPCs
{
    public class GlobalNPCLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            //if (npc.type == NPCID.Demon)// 恶魔
            //{
            //    // 普通模式掉落规则
            //    LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            //    notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SunflowerBall>(), 30, 1, 1));//掉落
            //    npcLoot.Add(notExpertRule);

            //    // 专家模式掉落规则
            //    LeadingConditionRule expertRule = new LeadingConditionRule(new Conditions.IsExpert());
            //    expertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SunflowerBall>(), 19, 1, 1));//掉落
            //    npcLoot.Add(expertRule);
            //}
            //if (npc.type == NPCID.VoodooDemon)// 巫毒恶魔
            //{
            //    // 普通模式掉落规则
            //    LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            //    notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GastlyBadge>(), 23, 1, 1));//掉落
            //    npcLoot.Add(notExpertRule);

            //    // 专家模式掉落规则
            //    LeadingConditionRule expertRule = new LeadingConditionRule(new Conditions.IsExpert());
            //    expertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GastlyBadge>(), 16, 1, 1));//掉落
            //    npcLoot.Add(expertRule);
            //}
            //base.ModifyNPCLoot(npc, npcLoot);
        }

        public override void Load()
        {
            // 注册全局 NPC 行为
            ModContent.GetInstance<GlobalNPCLoot>();
        }
    }
}
