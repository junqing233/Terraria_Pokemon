using Terraria;
using Terraria.ModLoader;

namespace Pokemon.Content.DamageClasses
{
    public class PokemonDamageClass : DamageClass
    {
        // 这是一个示例伤害类别，旨在展示当前功能的所有特性，并解释如何创建你自己的伤害类别（如果需要）。
        // 有关如何将状态加成应用于特定伤害类别的信息，请参考 ExampleMod/Content/Items/Accessories/ExampleStatBonusAccessory。
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            // 这个方法允许你的伤害类别默认从其他类别的状态加成中受益，以及通用状态加成。
            // 简要总结一下 DamageClass 使用的两个非标准伤害类别名称：
            // Default 是默认的伤害类别。它不从任何类别特定的状态加成或通用状态加成中受益。
            // 有许多物品和弹射物使用这个，例如投掷的水和骨手套的骨头。
            // Generic 另一方面，从所有通用状态加成中受益，但不从任何其他特定状态加成中受益；它是所有非 Default 伤害类别构建的基础。
            //if (damageClass == DamageClass.Generic)
            //    return StatInheritanceData.Full;// 我们希望从所有通用状态加成中受益，因此返回一个全满的 StatInheritanceData。

            return new StatInheritanceData(
                damageInheritance: 0f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0f,
                knockbackInheritance: 0f
            );
            // 现在，你可能会问，我们刚才做了什么？让我们看看……
            // StatInheritanceData 是一个结构体，你需要为每个给定的结果返回其中一个。
            // 通常，后一种情况会写成 "StatInheritanceData.None"，而不是手动输入...
            // ...但是为了清晰起见，我们已经手动写出并标记了每个参数；它们应该是不言自明的。
            // 为了说明这些返回值如何工作，每个值都像一个百分比，0f 表示 0%，1f 表示 100%，依此类推。
            // 返回值指示你的类别将从所指定的伤害类别中获得多少该状态加成。
            // 如果你创建一个不带任何参数的 StatInheritanceData，所有参数将被设置为 1f。
            // 例如，如果我们为 DamageClass.Ranged 提出一个假设的替代返回值...
            /*
            if (damageClass == DamageClass.Ranged)
                return new StatInheritanceData(
                    damageInheritance: 1f,
                    critChanceInheritance: -1f,
                    attackSpeedInheritance: 0.4f,
                    armorPenInheritance: 2.5f,
                    knockbackInheritance: 0f
                );
            */
            // 这将允许我们的自定义类别从以下远程状态加成中受益：
            // - 伤害，100% 效果
            // - 攻击速度，40% 效果
            // - 暴击几率，-100% 效果（这意味着任何专门提高远程暴击几率的状态加成将按相同数量降低我们自定义类别的暴击几率）
            // - 抗甲穿透，250% 效果

            // 注意：这些值没有硬上限。请注意并小心，你设置的任何值都可能导致意外后果，
            // 我们不对由此造成的任何临时或永久性损坏负责，包括对你、你的角色或你的世界的损坏。
            // 为了引用非原版伤害类别，请使用 "ModContent.GetInstance<TargetDamageClassHere>()" 而不是 "DamageClass.XYZ"。
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            // 这个方法允许你的伤害类别从其他类别的效果中受益并能够激活其他类别的效果（例如幽灵弹、熔岩之石）基于返回 true。
            // 注意，与上面的状态继承方法不同，你不需要在这个方法中考虑通用加成。
            // 为了这个示例，我们将我们的类别设置为能够激活近战和魔法特定的效果。
            //if (damageClass == DamageClass.Melee)
            //    return true;
            //if (damageClass == DamageClass.Magic)
            //    return true;
            //if (damageClass == DamageClass.Ranged)
            //    return true;
            //if (damageClass == DamageClass.Summon)
            //    return true;
            //if (damageClass == DamageClass.Throwing)
            //    return true;
            
            return false;
        }

        //public override void SetDefaultStats(Player player)
        //{
        //    // 这个方法允许你为示例伤害类别设置默认的状态加成。
        //    // 在这里，我们将我们的示例伤害类别设置为拥有比普通情况下更高的暴击几率和抗甲穿透。
        //    player.GetCritChance<PokemonDamageClass>() += 4;
        //    player.GetArmorPenetration<PokemonDamageClass>() += 10;
        //    // 这些类型的加成也存在于伤害（GetDamage）、击退（GetKnockback）和攻击速度（GetAttackSpeed）中。
        //    // 你将在整个 Terraria 中看到这些用法，既用于原版类别也用于我们的示例类别。请熟悉这些用法。
        //}

        // 这个属性允许你决定你的伤害类别是否可以使用标准暴击计算。
        // 注意：将其设置为 false 将也会阻止暴击几率提示行的显示。
        // 这种阻止行为会覆盖 ShowStatTooltipLine 中的设置，因此要小心！
        //public override bool UseStandardCritCalcs => true;

        //public override bool ShowStatTooltipLine(Player player, string lineName)
        //{
        //    // 这个方法允许你阻止某些常见的状态提示行出现在与这个 DamageClass 关联的物品上。
        //    // 你可以使用的四个行名称是 "Damage"、"CritChance"、"Speed" 和 "Knockback"。所有四个默认为 true，因此将被显示。例如...
        //    if (lineName == "Speed")
        //        return false;

        //    return true;
        //    // 请注意，这个钩子不会永远存在；只到即将进行的提示行整体重做为止。
        //    // 一旦发生这种情况，将展示一种更好的、更灵活的实现方法，这个钩子将被移除。
        //}
    }
}
