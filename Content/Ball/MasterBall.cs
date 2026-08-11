using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.Content.DamageClasses;
using Pokemon.Content.Items;
using Pokemon.Content.NPCs.Pokemons;
using Pokemon.Content.NPCs.TownNPCs;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pokemon.Content.Ball
{
    public class MasterBall : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = ModContent.GetInstance<PokeBallDamageClass>(); // 伤害类型
            Item.width = 22;
            Item.height = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.shoot = ModContent.ProjectileType<MasterBallProj>();
            Item.shootSpeed = 15f;
            Item.noUseGraphic = true;
            Item.consumable = true;
            Item.noMelee = true;
            Item.maxStack = 9999;
        }
        public override void AddRecipes()
        {
            CreateRecipe(20)
               .AddIngredient(ItemID.Wood, 1) // 木材
               .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(
                          source,
                          position,
                          velocity,
                          type,
                          damage,
                          knockback,
                          player.whoAmI, 0f, 1f
                           );
            return false;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            spriteBatch.Draw(
                texture,
                position,
                null,
                drawColor,
                0f,
                origin,
                scale * 0.8f, // 缩放0.8倍
                SpriteEffects.None,
                0f
            );
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height - texture.Height * 0.5f);
            spriteBatch.Draw(
                texture,
                position + new Vector2(0, 4),
                null,
                lightColor,
                rotation,
                texture.Size() * 0.5f,
                scale * 0.8f, // 缩放0.8倍
                SpriteEffects.None,
                0f
            );
            return false;
        }
    }
    public class MasterBallProj : ModProjectile
    {
        public override string Texture => "Pokemon/Content/Ball/MasterBall";
        private enum CatchState
        {
            Normal,
            CatchAnim,
            RotateAnim,
            Judge,
            ReturnToPlayer // 新增
        }

        private CatchState state = CatchState.Normal;
        private int animFrame = 0;
        private int animCounter = 0;
        private int rotatePhase = 0;
        private int rotateCounter = 0;
        private bool judged = false;

        private const int CatchAnimFrames = 5;
        private const int RotateTimes = 3;
        private static readonly float RotateAngle = MathHelper.ToRadians(45f);
        private bool isHit = false;

        private int catchedNpcWhoAmI = -1;
        private int badgeItemType = 0; // 新增：记录要生成的徽章类型
        private NPC catchedNpc = null;

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.aiStyle = 2;
            Projectile.scale = 0.8f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 240;
        }
        public override void OnSpawn(IEntitySource source)
        {
            ProfessorSamuelOak.isFirstBall = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;//我们不想召唤兽会割草
        }
        [System.Obsolete]
        public override void Kill(int timeLeft)
        {
            if (!isHit && Main.myPlayer == Projectile.owner && Projectile.ai[1] == 1f)
            {
                int itemIndex = Item.NewItem(
                    Projectile.GetSource_Death(),
                    Projectile.Center,
                    ModContent.ItemType<MasterBall>(),
                    1,
                    noGrabDelay: true
                );
                if (itemIndex >= 0 && itemIndex < Main.maxItems)
                {
                    Main.item[itemIndex].velocity = -Vector2.UnitY;
                }
            }
            if ((isHit && catchedNpc == null) || Projectile.ai[1] == 0f)
            {
                //粒子效果
                for (int i = 0; i < 3; i++)
                {
                    int dustType = DustID.PurpleTorch;
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 1, default, 1f);
                    Main.dust[dustIndex].velocity *= 1f;
                    Main.dust[dustIndex].scale = 0.5f;
                    Main.dust[dustIndex].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    Main.dust[dustIndex].noGravity = true;
                }
                for (int i = 0; i < 3; i++)
                {
                    int dustType = DustID.WhiteTorch;
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 1, default, 1f);
                    Main.dust[dustIndex].velocity *= 1f;
                    Main.dust[dustIndex].scale = 0.5f;
                    Main.dust[dustIndex].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    Main.dust[dustIndex].noGravity = true;
                }
            }
            if(catchedNpc != null)
            {
                catchedNpc.hide = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture;
            Rectangle rectangle;
            Vector2 origin;
            if (state == CatchState.CatchAnim)
            {
                texture = ModContent.Request<Texture2D>("Pokemon/Content/Ball/MasterBallProj_").Value;
                int frameHeight = texture.Height / CatchAnimFrames;
                rectangle = new Rectangle(0, animFrame * frameHeight, texture.Width, frameHeight);
                origin = new Vector2(texture.Width / 2f, frameHeight / 2f);
            }
            else
            {
                texture = TextureAssets.Projectile[Type].Value;
                int frame = Projectile.frame;
                int frameHeight = texture.Height / Main.projFrames[Type];
                rectangle = new Rectangle(0, frame * frameHeight, texture.Width, frameHeight);
                origin = new Vector2(texture.Width / 2f, frameHeight / 2f);
            }

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                rectangle,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0);
            return false;
        }
        private static readonly AccessCrystals crystalsReward = new SampleAccessCrystals();
        public override void AI()
        {
            if(state != CatchState.Normal)
            {
                Projectile.timeLeft = 2;
            }
            if (state == CatchState.CatchAnim || state == CatchState.RotateAnim)
            {
                if (catchedNpcWhoAmI != -1 && catchedNpcWhoAmI < Main.maxNPCs)
                {
                    NPC npc = Main.npc[catchedNpcWhoAmI];
                    if (npc != null && npc.active)
                    {
                        catchedNpc = npc;
                        npc.Center = Projectile.Center;
                        npc.hide = true;
                        npc.velocity = Vector2.Zero;
                    }
                }
            }
            if (state == CatchState.Judge && !judged)
            {
                if (catchedNpcWhoAmI != -1 && catchedNpcWhoAmI < Main.maxNPCs)
                {
                    NPC npc = Main.npc[catchedNpcWhoAmI];
                    if (npc != null && npc.active)
                    {
                        catchedNpc = npc;
                        float catchChance = 1f;
                        bool success = Main.rand.NextFloat() < catchChance;
                        if (success)
                        {
                            // 不立刻生成物品
                            state = CatchState.ReturnToPlayer;
                            // 让npc消失
                            npc.active = false;
                            return;
                        }
                        else
                        {
                            npc.velocity = -Vector2.UnitY * 2;
                            npc.hide = false;
                        }
                    }
                }
                Projectile.Kill();
                judged = true;
                return;
            }

            switch (state)
            {
                case CatchState.CatchAnim:
                    Projectile.velocity = Vector2.Zero;
                    Projectile.penetrate = -1;
                    Projectile.tileCollide = false;
                    animCounter++;
                    if (animCounter > 6)
                    {
                        animCounter = 0;
                        animFrame++;
                        if (animFrame >= CatchAnimFrames)
                        {
                            state = CatchState.RotateAnim;
                            animFrame = 0;
                            rotatePhase = 0;
                            rotateCounter = 0;
                            Projectile.rotation = 0f;
                        }
                    }
                    break;
                case CatchState.RotateAnim:
                    Projectile.velocity = Vector2.Zero;
                    Projectile.tileCollide = false;
                    float t = rotateCounter / 10f;
                    if (rotatePhase % 2 == 0)
                        Projectile.rotation = MathHelper.Lerp(0, -RotateAngle, t);
                    else
                        Projectile.rotation = MathHelper.Lerp(0, RotateAngle, t);

                    rotateCounter++;
                    if (rotateCounter >= 10)
                    {
                        rotateCounter = 0;
                        rotatePhase++;
                        if (rotatePhase >= RotateTimes * 2)
                        {
                            state = CatchState.Judge;
                        }
                    }
                    break;
                case CatchState.ReturnToPlayer:
                    {
                        Player player = Main.player[Projectile.owner];
                        Projectile.rotation += Projectile.velocity.Length() * 0.01f;
                        Projectile.tileCollide = false;
                        float speed = 24f;
                        Vector2 toPlayer = player.Center - Projectile.Center;
                        float dist = toPlayer.Length();
                        if (dist < 21f)
                        {
                            // 到达玩家身边，生成物品并消失
                            if (player.whoAmI == Main.myPlayer && badgeItemType != 0)
                            {
                                player.QuickSpawnItem(player.GetSource_Misc("PokeballCatch"), badgeItemType);
                                // 结晶奖励
                                if (catchedNpc != null)
                                    crystalsReward.GiveCrystalReward(player, catchedNpc);
                            }
                            Projectile.Kill();
                            return;
                        }
                        Projectile.velocity = toPlayer.SafeNormalize(Vector2.Zero) * speed;
                        break;
                    }
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return !target.hide && !target.friendly;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            isHit = true;
            if (state == CatchState.Normal)
            {
                // 判断捕捉对象类型，设置对应徽章
                if (target.ModNPC is BeldumNPC)
                    badgeItemType = ModContent.ItemType<BeldumBadge>();
                else if (target.ModNPC is Bulbasaur)
                    badgeItemType = ModContent.ItemType<BulbasaurBadge>();
                else if (target.ModNPC is Charmander)
                    badgeItemType = ModContent.ItemType<CharmanderBadge>();
                else if(target.ModNPC is Gastly)
                    badgeItemType = ModContent.ItemType<GastlyBadge>();
                else if (target.ModNPC is Munchlax)
                    badgeItemType = ModContent.ItemType<MunchlaxBadge>();
                else if (target.ModNPC is Spoink)
                    badgeItemType = ModContent.ItemType<SpoinkBadge>();
                else if (target.ModNPC is Squirtle)
                    badgeItemType = ModContent.ItemType<SquirtleBadge>();
                else if (target.ModNPC is Sunflower)
                    badgeItemType = ModContent.ItemType<SunflowerBall>();
                else if (target.ModNPC is Taillow)
                    badgeItemType = ModContent.ItemType<TaillowBadge>();
                else if (target.ModNPC is Voltorb)
                    badgeItemType = ModContent.ItemType<VoltorbBadge>();
                else if (target.ModNPC is Wingull)
                    badgeItemType = ModContent.ItemType<WingullBadge>();
                else if (target.ModNPC is Fomantis)
                    badgeItemType = ModContent.ItemType<FomantisBadge>();
                else if (target.ModNPC is Trapinch)
                    badgeItemType = ModContent.ItemType<TrapinchBadge>();
                else if (target.ModNPC is Pikachu)
                    badgeItemType = ModContent.ItemType<PikachuBadge>();//14
                // 你可以继续扩展更多宝可梦和徽章

                if (badgeItemType != 0)
                {
                    state = CatchState.CatchAnim;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.netUpdate = true;
                    catchedNpcWhoAmI = target.whoAmI;
                }
            }
        }
    }
}