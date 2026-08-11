using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Buffs;
using Pokemon.Content.DamageClasses;
using Pokemon.Content.Equipment;
using Pokemon.Content.Items;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Projectiles.FomantisBadgeProj
{
    public class FomantisBadgeProj5 : ModProjectile
    {
        private enum SwordState { Idle, MoveToEnemy, Slashing, Return }
        private SwordState state = SwordState.Idle;
        private float idleFloatTimer = 0f;
        private Vector2 idleOffset = new Vector2(0, -60);
        private Vector2 slashStartPos;
        private Vector2 slashEndPos;
        private float slashT;
        private NPC currentTarget;
        private Vector2 velocity = Vector2.Zero;
        FomantisBadge fomantisBadge = null;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false; // 敌方伤害
            Projectile.width = 30; // 弹幕宽度
            Projectile.height = 30; // 弹幕高度
            Projectile.knockBack = 0;
            Projectile.friendly = true; // 友方弹幕
            Projectile.tileCollide = false; // 不与瓷砖碰撞
            Projectile.DamageType = ModContent.GetInstance<PokemonDamageClass>(); // 伤害类型
            Projectile.penetrate = -1; // 穿透
            Projectile.ignoreWater = true; // 无视液体
            Projectile.timeLeft = 360; // 存在时间，单位为帧
            Projectile.alpha = 1; // 透明度
            Projectile.light = 0.5f; // 发光亮度
            Projectile.usesLocalNPCImmunity = true; //独立无敌帧
            Projectile.localNPCHitCooldown = 10; //独立无敌帧时间
            base.SetDefaults();
        }
  
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            // 获取 PokeRadar 实例
            PokeRadar pokeRadar = null;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is PokeRadar radar)
                {
                    pokeRadar = radar;
                    break;
                }
            }
            if (pokeRadar != null)
            {
                for (int j = 0; j < PokeRadar.MaxItems; j++)
                {
                    if (pokeRadar.items[j] != null && !pokeRadar.items[j].IsAir)
                        if (pokeRadar.items[j].type == ModContent.ItemType<FomantisBadge>())
                        {
                            if (pokeRadar.items[j].ModItem is FomantisBadge fomantis)
                            {
                                fomantisBadge = fomantis;
                            }
                            break;
                        }
                }
            }
            Projectile.damage = fomantisBadge.Item.damage + (int)(player.GetWeaponDamage(player.inventory[player.selectedItem]) * 0.8f);
            if (state == SwordState.Idle)
            {
                Projectile.rotation = -MathHelper.PiOver4;
                idleFloatTimer += 0.05f;
                Projectile proj1 = FindFomantisBadgeProj1();
                if (proj1 != null)
                {
                    Vector2 basePos = proj1.Center + idleOffset;
                    float floatY = (float)Math.Sin(idleFloatTimer) * 8f;
                    // 平滑插值到目标位置
                    Projectile.Center = Vector2.Lerp(Projectile.Center, basePos + new Vector2(0, floatY), 0.15f);
                }
                // 寻找敌人
                NPC target = FomantisBadgeProj1.FindTargetWithinRange(player, 1200);
                if (target != null)
                {
                    currentTarget = target;
                    state = SwordState.MoveToEnemy;
                }
            }else
            if (state == SwordState.MoveToEnemy)
            {
                if (currentTarget == null || !currentTarget.active)
                {
                    state = SwordState.Return;
                    return;
                }
                Vector2 toEnemy = (currentTarget.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                Projectile.rotation = toEnemy.ToRotation() + MathHelper.PiOver4; // 剑尖朝向
                //Projectile.Center = Vector2.Lerp(Projectile.Center, currentTarget.Center - toEnemy * (currentTarget.width / 2f + 60f), 0.2f);
                Vector2 targetPos = currentTarget.Center - toEnemy * (currentTarget.width / 2f + 60f);
                Vector2 toTarget = targetPos - Projectile.Center;
                float maxSpeed = 18f;
                velocity += toTarget.SafeNormalize(Vector2.Zero) * 2.2f;
                if (velocity.Length() > maxSpeed)
                    velocity = velocity.SafeNormalize(Vector2.Zero) * maxSpeed;
                if (toTarget.Length() < 12f)
                    velocity *= 0.7f;
                Projectile.Center += velocity;
                if (Vector2.Distance(Projectile.Center, currentTarget.Center - toEnemy * 60f) < 2f + currentTarget.width / 2f)
                {
                    // 记录斩击起点和终点
                    float angle = toEnemy.ToRotation();
                    // 斩击起点和终点在敌人中心两侧
                    float radius = Math.Max(120f, currentTarget.width / 2f + 80f);
                    Vector2 dir = angle.ToRotationVector2();
                    slashStartPos = currentTarget.Center + dir.RotatedBy(-MathHelper.PiOver2) * radius;
                    slashEndPos = currentTarget.Center + dir.RotatedBy(MathHelper.PiOver2) * radius;
                    slashT = 0f;
                    state = SwordState.Slashing;
                }
            }else
            if (state == SwordState.Slashing)
            {
                velocity = Vector2.Zero;
                // 不再因为目标死亡提前 return
                slashT += 0.05f;
                if (slashT > 1f) slashT = 1f;

                // 斩击轨迹
                Vector2 center = currentTarget != null ? currentTarget.Center : slashEndPos; // 若目标已死，保持最后位置
                float angleStart = (slashStartPos - center).ToRotation();
                float angleEnd = (slashEndPos - center).ToRotation();
                float angle = MathHelper.Lerp(angleStart, angleEnd, slashT);
                Projectile.Center = Vector2.SmoothStep(slashStartPos, slashEndPos, slashT);

                // 剑旋转模拟斩击
                Projectile.rotation = angle + MathHelper.PiOver4 + MathHelper.Lerp(-0.5f, 0.5f, slashT);

                if (slashT >= 1f)
                {
                    // 斩击完后，重新寻找目标
                    NPC nextTarget = FomantisBadgeProj1.FindTargetWithinRange(player, 1200);
                    if (nextTarget != null)
                    {
                        currentTarget = nextTarget;
                        state = SwordState.MoveToEnemy;
                    }
                    else
                    {
                        state = SwordState.Return;
                    }
                }
            }
            else
            if (state == SwordState.Return)
            {
                Projectile proj1 = FindFomantisBadgeProj1();
                if (proj1 != null)
                {
                    Vector2 basePos = proj1.Center + idleOffset;
                    Projectile.Center = Vector2.Lerp(Projectile.Center, basePos, 0.12f);
                    if (Vector2.Distance(Projectile.Center, basePos) < 64f)
                    {
                        state = SwordState.Idle;
                    }
                }
            }

            if (!player.HasBuff(ModContent.BuffType<BuffsFomantisBadge>()))
            {
                Projectile.Kill();
                return;
            }
            else
                Projectile.timeLeft = 360;
        }
        private Projectile FindFomantisBadgeProj1()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Projectile.owner && p.type == ModContent.ProjectileType<FomantisBadgeProj1>())
                    return p;
            }
            return null;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return true;
        }
        public override bool? CanDamage()
        {
            return state == SwordState.Slashing;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            ProjectileID.Sets.TrailingMode[Type] = 2;//设置尾迹模式为2，即尾迹为圆形
            ProjectileID.Sets.TrailCacheLength[Type] = 4;//设置尾迹缓存长度为5，即最多保留5个尾迹

            Rectangle rectangle = new Rectangle(
                 0,
                 texture.Height / Main.projFrames[Type] * Projectile.frame,
                 texture.Width,
                 texture.Height / Main.projFrames[Type]
             );

            Color MyColor = Color.White;
            MyColor.A = 0;
            if(state == SwordState.Slashing)
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                Main.EntitySpriteDraw(texture, oldcenter, rectangle, MyColor * factor,
                    Projectile.oldRot[i],
                    new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                    Projectile.scale,
                    SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                rectangle,
                Color.White,
                Projectile.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}
