using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Linq;
using Celeste.Mod.PandorasBox;


namespace Celeste.Mod.EmHelper.Entities
{
    [Tracked]
    [CustomEntity("EmHelper/Walkeline")]
    public class Walkeline : Actor
    {
        public Walkeline(EntityData data, Vector2 levelOffset) : this(data.Position + levelOffset, data.HexColor("haircolor", Calc.HexToColor("212121")), data.Bool("left", false), data.Bool("weak", false), data.Bool("dangerous", false), data.Bool("ally", false), data.Bool("bouncy", false), data.Bool("smart", false), data.Bool("mute", false), data.Bool("nobackpack", false), data.Bool("idle", false), data.Attr("deathflag", "WalkelineIsDead"), data.Bool("triggerhappy", false)) { } //use the other one
        public Walkeline(Vector2 position, Color haircolor, bool left, bool weak, bool dangerous, bool ally, bool bouncy, bool smart, bool mute, bool nobackpack, bool idle, string deathflag, bool triggerhappy) : base(position)
        {//left: it faces left on start, fragile: it dies if it touches the player, dangerous: kills the player if touched, ally: the player dies if it dies, smart: turns left and right like a red koopa (avoids falling, triggerhappy: interacts with triggers
            this.triggerhappy = triggerhappy;
            this.HairColor = haircolor;
            this.deathflag = deathflag;
            this.idle = idle;
            this.ismute = mute;
            this.bouncy = bouncy;
            this.left = left; //if true faces left
            this.ally = ally;
            this.dangerous = dangerous;
            this.weak = weak;
            this.smart = smart;
            this.nobackpack = nobackpack;
            base.Collider = new Hitbox(8f, 11f, -4f, -11f); //damage hitbox
            base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
            this.onCollideH = new Collision(this.OnCollideH);
            this.onCollideV = new Collision(this.OnCollideV);
            base.Add(new VertexLight(base.Collider.Center, Color.White, 1f, 32, 64));
            //holdable shenanigans to interact with other entities
            base.Add(this.Hold = new Holdable(0.1f));
            this.Hold.PickupCollider = new NoCollider();
            this.Hold.OnHitSeeker = new Action<Seeker>(this.HitSeeker);
            this.Hold.OnHitSpring = new Func<Spring, bool>(this.HitSpring);
            //pandorabox crossover, the pandora's box interactions only work if it's installed
            this.pandyinstalled = Everest.Modules.Any((EverestModule mod) => mod.GetType().Name.Equals("PandorasBoxMod"));
            if (this.pandyinstalled)
            {
                createpipeinteraction();
            }
            this.inpipe = false;


        }

        private void createpipeinteraction() //all this pipe stuff only happens if pandora's box is installed
        {
            MarioClearPipeInteraction interaction = new MarioClearPipeInteraction(new Vector2(0, 8f));
            interaction.OnPipeEnter = OnPipeEnter;
            interaction.OnPipeExit = OnPipeExit;
            interaction.OnPipeBlocked = OnPipeBlocked;
            interaction.OnPipeUpdate = OnPipeUpdate;
            interaction.CanEnterPipe = CanEnterPipe;
            this.Add(interaction);

        }

        private bool CanEnterPipe(Entity entity, MarioClearPipeHelper.Direction direction)
        {
            if (entity != this) { return false; }
            if (this.OnGround(1)) { return true; }

            return false;

        }

        private void OnPipeBlocked(Entity entity, MarioClearPipeInteraction interaction)
        {
            if (entity != this) { return; }
            this.Die();
        }

        private void OnPipeUpdate(Entity entity, MarioClearPipeInteraction interaction)
        {
            if (entity != this) { return; }
            if (this.dead) { interaction.ExitEarly = true; }

        }

        private void OnPipeEnter(Entity entity, MarioClearPipeInteraction interaction)
        {
            if (entity != this) { return; }
            this.inpipe = true;
        }

        private void OnPipeExit(Entity entity, MarioClearPipeInteraction interaction)
        {
            if (entity != this) { return; }
            this.inpipe = false;

            this.speed = interaction.DirectionVector * interaction.CurrentClearPipe.TransportSpeed;
            if (this.speed.X > 0)
            {
                this.walker.Sprite.Scale.X = 1;
            }
            else { this.walker.Sprite.Scale.X = -1; }


        }



        //added to scene
        public override void Added(Scene scene)
        {
            base.Added(scene); 
            this.walker = new BadelineDummy(Position); //add the sprite to this position at the start of the scene
            this.walker.Floatness = 0f; //it shouldn't float?
            if (this.nobackpack) //different sprites
            {
                GFX.SpriteBank.CreateOn(this.walker.Sprite, "windupwalkeline");
                this.walker.Add(this.spinninghairsprite = GFX.SpriteBank.Create("windupwspinninghair"));
            }
            else
            {
                GFX.SpriteBank.CreateOn(this.walker.Sprite, "walkeline");
                this.walker.Add(this.spinninghairsprite = GFX.SpriteBank.Create("wspinninghair"));
            }
            Scene.Add(walker);
            if (left) { this.walker.Sprite.Scale.X = -1; } else { this.walker.Sprite.Scale.X = 1; }
            this.walker.Sprite.Play("walk"); //walking on the moon
            this.spinninghairsprite.Play("idle"); //only used for pipe shenanigans
            this.walker.Hair.Color = this.HairColor;//haircolor
            this.spinninghairsprite.Color = this.HairColor;

            //audio
            this.walker.Sprite.OnFrameChange = delegate (string anim)
            {
                int currentAnimationFrame = this.walker.Sprite.CurrentAnimationFrame;
                if ((anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim == "runSlow" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim == "runFast" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)))
                {
                    if (!this.ismute)
                    {
                        Audio.Play("event:/char/badeline/footstep", this.walker.Position);
                    }

                }
            };
        }


        //collide with player
        private void OnPlayer(Player player)
        { //used when it's touching the player
            if (!this.dead && !this.inpipe)
            {
                if (this.bouncy)
                {
                    player.PointBounce(base.Center); //bounce
                    if (this.bounceSfxDelay <= 0f)
                    {
                        if (!this.ismute)
                        {
                            Audio.Play("event:/game/general/crystalheart_bounce", this.walker.Position);
                        }
                        this.bounceSfxDelay = 0.1f;
                    }
                    Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                }

                if (this.dangerous) { player.Die((player.Position - this.Position).SafeNormalize(), false, true); } //if you want to kill the player
                else if (this.weak) { Die(); }
            }
        }

        public void HitSeeker(Seeker seeker) // :-(
        {
            this.Die();
        }

        public bool HitSpring(Spring spring)
        {
            if (spring.Orientation == Spring.Orientations.Floor && this.speed.Y >= 0f)
            {
                base.MoveTowardsX(spring.CenterX, 4f, null);
                this.speed.Y = -160f;
                this.noGravityTimer = 0.15f;
                return true;
            }
            if (spring.Orientation == Spring.Orientations.WallLeft && this.speed.X <= 0f)
            {
                base.MoveTowardsY(spring.CenterY + 5f, 4f, null);
                this.speed.X = 220f;
                this.speed.Y = -80f;
                this.noGravityTimer = 0.1f;
                this.walker.Sprite.Scale.X = 1;
                return true;
            }
            if (spring.Orientation == Spring.Orientations.WallRight && this.speed.X >= 0f)
            {
                base.MoveTowardsY(spring.CenterY + 5f, 4f, null);
                this.speed.X = -220f;
                this.speed.Y = -80f;
                this.noGravityTimer = 0.1f;
                this.walker.Sprite.Scale.X = -1;
                return true;
            }

            return false;
        }
        //crushed
        protected override void OnSquish(CollisionData data)
        {
            if (!base.TrySquishWiggle(data) && !SaveData.Instance.Assists.Invincible && !this.dead)
            {
                this.Die();
            }
        }

        //rip
        public void Die()
        {
            if (!this.dead)
            {
                if (this.ally)
                {   //kills the player
                    Player entity = base.SceneAs<Level>().Tracker.GetEntity<Player>();
                    if (entity != null)
                    {
                        entity.Die(-Vector2.UnitX * (float)entity.Facing, false, true);
                    }
                }

                Audio.Play("event:/char/madeline/death", this.Position);
                (base.Scene as Level).Session.SetFlag(deathflag, true);
                base.Add(new DeathEffect(this.HairColor, new Vector2?(base.Center - this.Position)));
                this.walker.RemoveSelf();
                this.Hold.RemoveSelf();
                this.dead = true; //the death effect doesnt appear if i delete the entity, so it just becomes invisible and deactivates
                base.Depth = -1000000;
                base.Collider = new NoCollider();
                this.AllowPushing = false;
                this.triggerhappy = false;
            }
        }


        public override void Update()
        {
            base.Update();
            if (this.dead) { return; } //do nothing ^
            Hold.CheckAgainstColliders();
            if (this.OnGround())
            {
                if (this.speed.Y > 0f)
                {
                    this.speed.Y = 0f;
                }
                if (this.walker.Sprite.Scale.X == 1 && !this.idle) { this.speed.X = 40; } else if (!this.idle) { this.speed.X = -40; }
                if (this.speed.X == 0f)
                {
                    if (!this.inpipe)
                    {
                        this.spinninghairsprite.Play("idle");
                        this.walker.Sprite.Play("idle");
                    }
                    else
                    {
                        this.walker.Sprite.Play("spin");
                        this.spinninghairsprite.Play("spin");

                    }
                }
                else if (!this.inpipe)
                {
                    this.walker.Sprite.Play("walk");
                    this.spinninghairsprite.Play("idle");
                }
                else
                {
                    this.walker.Sprite.Play("spin");
                    this.spinninghairsprite.Play("spin");

                }
            }
            else //if onair, copied from theocrystal
            {
                float num = 800f;
                if (Math.Abs(this.speed.Y) <= 30f)
                {
                    num *= 0.5f;
                }
                float num2 = 350f;
                if (this.speed.Y < 0f)
                {
                    num2 *= 0.5f;
                }
                this.speed.X = Calc.Approach(this.speed.X, 0f, num2 * Engine.DeltaTime);
                if (this.noGravityTimer > 0f)
                {
                    this.noGravityTimer -= Engine.DeltaTime;
                }
                else
                {
                    this.speed.Y = Calc.Approach(this.speed.Y, 200f, num * Engine.DeltaTime);
                }
                if (!this.inpipe)
                {
                    this.walker.Sprite.Play("idle");
                    this.spinninghairsprite.Play("idle");
                }
                else
                {
                    this.walker.Sprite.Play("spin");
                    this.spinninghairsprite.Play("spin");


                }

            }

            if (base.Top > (float)base.SceneAs<Level>().Bounds.Bottom)
            {
                this.Die();
            }

            else if (base.Right > (float)base.SceneAs<Level>().Bounds.Right && this.walker.Sprite.Scale.X == 1)
            {
                flip();
            }
            else if (base.Left < (float)base.SceneAs<Level>().Bounds.Left && this.walker.Sprite.Scale.X == -1)
            {
                flip();
            }

            if (this.OnGround()) { this.CheckSmartFlip(); }  //check it again because i don't want it to flip in midair
            this.bounceSfxDelay -= Engine.DeltaTime; //sound timer
            base.MoveH(this.speed.X * Engine.DeltaTime, this.onCollideH, null);
            base.MoveV(this.speed.Y * Engine.DeltaTime, this.onCollideV, null);
            this.walker.Position = this.Position;
            this.spinninghairsprite.Scale.X = this.walker.Sprite.Scale.X;

        }

        private void OnCollideH(CollisionData data)
        {
            if (data.Hit is DashSwitch)
            {
                (data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitX * (float)Math.Sign(this.speed.X));
            }
            flip();
            if (Math.Abs(this.speed.X) > 100f)
            {
                this.ImpactParticles(data.Direction);
            }

        }
        private void OnCollideV(CollisionData data)
        {
            if (data.Hit is DashSwitch)
            {
                (data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitX * (float)Math.Sign(this.speed.Y));
            }
            if (this.speed.Y > 160f)
            {
                this.ImpactParticles(data.Direction);
            }
        }

        private void CheckSmartFlip()
        {

            if (this.smart && this.OnGround() && !this.idle)
            {
                if (this.walker.Sprite.Scale.X == 1)
                {
                    if (!this.OnGround(this.Position + new Vector2(8, 0))) { flip(); }
                }
                else
                {
                    if (!this.OnGround(this.Position + new Vector2(-8, 0))) { flip(); }
                }
            }
        }

        private void flip() //turns around
        {
            if (!this.inpipe)
            {
                this.walker.Sprite.Scale.X = -this.walker.Sprite.Scale.X;
            }

        }

        private void ImpactParticles(Vector2 dir)
        {
            float direction;
            Vector2 position;
            Vector2 positionRange;
            if (dir.X > 0f)
            {
                direction = 3.14159274f;
                position = new Vector2(base.Right, base.Y - 4f);
                positionRange = Vector2.UnitY * 6f;
            }
            else if (dir.X < 0f)
            {
                direction = 0f;
                position = new Vector2(base.Left, base.Y - 4f);
                positionRange = Vector2.UnitY * 6f;
            }
            else if (dir.Y > 0f)
            {
                direction = -1.57079637f;
                position = new Vector2(base.X, base.Bottom);
                positionRange = Vector2.UnitX * 6f;
            }
            else
            {
                direction = 1.57079637f;
                position = new Vector2(base.X, base.Top);
                positionRange = Vector2.UnitX * 6f;
            }
            base.SceneAs<Level>().Particles.Emit(TheoCrystal.P_Impact, 12, position, positionRange, direction);
        }


        public BadelineDummy walker; //to create the sprite
        public Color HairColor; //haircolor
        private Sprite spinninghairsprite; //used for pipes
        public bool left = true;
        public bool idle = false;
        public Vector2 speed = new Vector2(0f, 0f);
        
        public bool weak = false; //dies if it touches the player
        public bool nobackpack = false;
        public bool dangerous = false; //the player dies if touched
        public bool ally = false; //the player dies if it dies
        public bool bouncy = false; //the player bounces if touched
        private bool dead = false; //used for post - death stuff, it doesn't get "deleted"
        private bool smart = false; //turn left or right, doesnt fall, like a red koopa
        private bool ismute = false; //the walk sound is annoying
        private Collision onCollideH;
        private Collision onCollideV;
        //holdable stuff
        public Holdable Hold;
        private float noGravityTimer;
        //bounce sound
        private float bounceSfxDelay;
        //pandorasbox crossover stuff
        private bool pandyinstalled;
        private bool inpipe;
        //deathflag
        private string deathflag = "WalkelineIsDead";
        public bool triggerhappy = false; //interacts with triggers
    }
}
