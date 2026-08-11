using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.MunchlaxProj
{
    public class MunchlaxProj3 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 20; // 弹幕宽度
            Projectile.height = 20; // 弹幕高度
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 120; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 0.2f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 10; //独立无敌帧时间
            base.SetDefaults();
        }

        public override void AI()
        {
            // 更新帧动画
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10) // 每5帧切换下一帧
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if(Projectile.frame >= 4)
                    Projectile.Kill();
            }
            NPC target = FindClosestNPC(1200);
            if (target != null && target.lifeMax > 5)
            {
                Projectile.Center = target.Center + new Vector2(0, -10); // 跟随目标
            }
        }
        private NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;
            float closestDistance = maxDetectDistance;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.dontTakeDamage && npc.lifeMax > 0 && npc.CanBeChasedBy())
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPC = npc;
                    }
                }
            }
            return closestNPC;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life > target.lifeMax / 2)
            {
                if((target.lifeMax / 100) > 0)
                    target.life -= target.lifeMax / 100;
                else
                    target.life -= 1;
                // 显示伤害数字
                CombatText.NewText(target.Hitbox, CombatText.DamagedFriendly,
                    (target.lifeMax / 100) > 0 ? target.lifeMax / 100 : 1, false, true);
            }
        }


        public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor)
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
                Projectile.scale * 0.5f,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}
