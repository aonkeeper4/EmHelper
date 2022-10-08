using Celeste.Mod.Entities;
using Celeste.Mod.PandorasBox;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Linq;

namespace Celeste.Mod.EmHelper.Entities {
    [Tracked]
    [CustomEntity("EmHelper/Walkeline")]
    public class Walkeline : Actor {
        //use the other one
        public Walkeline(EntityData data, Vector2 levelOffset)
            : this(data.Position + levelOffset, data.HexColor("haircolor", Calc.HexToColor("212121")), data.Bool("left", false), data.Bool("weak", false), data.Bool("dangerous", false), data.Bool("ally", false), data.Bool("bouncy", false), data.Bool("smart", false), data.Bool("mute", false), data.Bool("nobackpack", false), data.Bool("idle", false), data.Attr("deathflag", "WalkelineIsDead"), data.Bool("triggerhappy", false)) {
        }

        //left: it faces left on start, fragile: it dies if it touches the player, dangerous: kills the player if touched, ally: the player dies if it dies, smart: turns left and right like a red koopa (avoids falling, triggerhappy: interacts with triggers
        public Walkeline(Vector2 position, Color haircolor, bool left, bool weak, bool dangerous, bool ally, bool bouncy, bool smart, bool mute, bool nobackpack, bool idle, string deathflag, bool triggerhappy)
            : base(position) {
            this.TriggerHappy = triggerhappy;
            hairColor = haircolor;
            this.deathflag = deathflag;
            this.idle = idle;
            isMute = mute;
            this.bouncy = bouncy;
            this.left = left; //if true faces left
            this.ally = ally;
            this.dangerous = dangerous;
            this.weak = weak;
            this.smart = smart;
            this.noBackpack = nobackpack;
            Collider = new Hitbox(8f, 11f, -4f, -11f); //damage hitbox
            Add(new PlayerCollider(new Action<Player>(OnPlayer), null, null));
            onCollideH = new Collision(OnCollideH);
            onCollideV = new Collision(OnCollideV);
            Add(new VertexLight(Collider.Center, Color.White, 1f, 32, 64));

            //holdable shenanigans to interact with other entities
            Add(holdable = new Holdable(0.1f));
            holdable.PickupCollider = new Hitbox(0f, 0f); // Dummy hitbox to allow for holdable interactions without being picked up
            holdable.OnHitSeeker = new Action<Seeker>(HitSeeker);
            holdable.OnHitSpring = new Func<Spring, bool>(HitSpring);

            //pandorabox crossover, the pandora's box interactions only work if it's installed
            pandyInstalled = Everest.Modules.Any((EverestModule mod) => mod.GetType().Name.Equals("PandorasBoxMod"));
            if (pandyInstalled) {
                CreatePipeInteraction();
            }
        }

        private void CreatePipeInteraction() {
            MarioClearPipeInteraction interaction = new(new Vector2(0, 8f)) {
                OnPipeEnter = OnPipeEnter,
                OnPipeExit = OnPipeExit,
                OnPipeBlocked = OnPipeBlocked,
                OnPipeUpdate = OnPipeUpdate,
                CanEnterPipe = CanEnterPipe
            };
            Add(interaction);
        }

        private bool CanEnterPipe(Entity entity, MarioClearPipeHelper.Direction direction) {
            return OnGround(1);
        }

        private void OnPipeBlocked(Entity entity, MarioClearPipeInteraction interaction) {
            Die();
        }

        private void OnPipeUpdate(Entity entity, MarioClearPipeInteraction interaction) {
            if (dead) {
                interaction.ExitEarly = true;
            }
        }

        private void OnPipeEnter(Entity entity, MarioClearPipeInteraction interaction) {
            inPipe = true;
        }

        private void OnPipeExit(Entity entity, MarioClearPipeInteraction interaction) {
            inPipe = false;
            speed = interaction.DirectionVector * interaction.CurrentClearPipe.TransportSpeed;
            walker.Sprite.Scale.X = speed.X > 0 ? 1 : -1;
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            walker = new BadelineDummy(Position) {
                Floatness = 0f //it shouldn't float?
            };
            if (noBackpack) {
                GFX.SpriteBank.CreateOn(walker.Sprite, "windupwalkeline");
                walker.Add(spinningHairSprite = GFX.SpriteBank.Create("windupwspinninghair"));
            } else {
                GFX.SpriteBank.CreateOn(walker.Sprite, "walkeline");
                walker.Add(spinningHairSprite = GFX.SpriteBank.Create("wspinninghair"));
            }

            Scene.Add(walker);
            walker.Sprite.Scale.X = left ? -1 : 1;
            walker.Sprite.Play("walk");
            spinningHairSprite.Play("idle"); //only used for pipe shenanigans
            walker.Hair.Color = hairColor;
            spinningHairSprite.Color = hairColor;

            //audio
            walker.Sprite.OnFrameChange = delegate (string anim) {
                int currentAnimationFrame = walker.Sprite.CurrentAnimationFrame;
                if ((anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim == "runSlow" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || (anim == "runFast" && (currentAnimationFrame == 0 || currentAnimationFrame == 6))) {
                    if (!isMute) {
                        Audio.Play("event:/char/badeline/footstep", walker.Position);
                    }
                }
            };
        }

        private void OnPlayer(Player player) {
            if (!dead && !inPipe) {
                if (bouncy) {
                    player.PointBounce(Center);
                    if (bounceSfxDelay <= 0f) {
                        if (!isMute) {
                            Audio.Play("event:/game/general/crystalheart_bounce", walker.Position);
                        }

                        bounceSfxDelay = 0.1f;
                    }

                    Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                }

                if (dangerous) {
                    player.Die((player.Position - Position).SafeNormalize(), false, true);
                }
                else if (weak) {
                    Die();
                }
            }
        }

        public void HitSeeker(Seeker seeker) {
            Die();
        }

        public bool HitSpring(Spring spring) {
            if (spring.Orientation == Spring.Orientations.Floor && speed.Y >= 0f) {
                MoveTowardsX(spring.CenterX, 4f, null);
                speed.Y = -160f;
                noGravityTimer = 0.15f;
                return true;
            }

            if (spring.Orientation == Spring.Orientations.WallLeft && speed.X <= 0f) {
                MoveTowardsY(spring.CenterY + 5f, 4f, null);
                speed.X = 220f;
                speed.Y = -80f;
                noGravityTimer = 0.1f;
                walker.Sprite.Scale.X = 1;
                return true;
            }

            if (spring.Orientation == Spring.Orientations.WallRight && speed.X >= 0f) {
                MoveTowardsY(spring.CenterY + 5f, 4f, null);
                speed.X = -220f;
                speed.Y = -80f;
                noGravityTimer = 0.1f;
                walker.Sprite.Scale.X = -1;
                return true;
            }

            return false;
        }

        protected override void OnSquish(CollisionData data) {
            if (!TrySquishWiggle(data) && !SaveData.Instance.Assists.Invincible && !dead) {
                Die();
            }
        }

        public void Die() {
            if (!dead) {
                if (ally) {
                    Player entity = SceneAs<Level>().Tracker.GetEntity<Player>();
                    if (entity != null) {
                        entity.Die(-Vector2.UnitX * (float)entity.Facing, false, true);
                    }
                }

                Audio.Play("event:/char/madeline/death", Position);
                (Scene as Level).Session.SetFlag(deathflag, true);
                Add(new DeathEffect(hairColor, new Vector2?(Center - Position)));
                walker.RemoveSelf();
                holdable.RemoveSelf();
                dead = true; //the death effect doesnt appear if i delete the entity, so it just becomes invisible and deactivates
                Depth = -1000000;
                Collidable = false;
                AllowPushing = false;
                TriggerHappy = false;
            }
        }

        public override void Update() {
            base.Update();
            if (dead) {
                return;
            }

            holdable.CheckAgainstColliders();
            if (OnGround()) {
                if (speed.Y > 0f) {
                    speed.Y = 0f;
                }

                if (walker.Sprite.Scale.X == 1 && !idle) {
                    speed.X = 40;
                } else if (!idle) {
                    speed.X = -40;
                }

                if (speed.X == 0f) {
                    if (!inPipe) {
                        spinningHairSprite.Play("idle");
                        walker.Sprite.Play("idle");
                    } else {
                        walker.Sprite.Play("spin");
                        spinningHairSprite.Play("spin");

                    }
                } else if (!inPipe) {
                    walker.Sprite.Play("walk");
                    spinningHairSprite.Play("idle");
                } else {
                    walker.Sprite.Play("spin");
                    spinningHairSprite.Play("spin");

                }
            } else {
                float num = 800f;
                if (Math.Abs(speed.Y) <= 30f) {
                    num *= 0.5f;
                }

                float num2 = 350f;
                if (speed.Y < 0f) {
                    num2 *= 0.5f;
                }

                speed.X = Calc.Approach(speed.X, 0f, num2 * Engine.DeltaTime);
                if (noGravityTimer > 0f) {
                    noGravityTimer -= Engine.DeltaTime;
                } else {
                    speed.Y = Calc.Approach(speed.Y, 200f, num * Engine.DeltaTime);
                }

                if (!inPipe) {
                    walker.Sprite.Play("idle");
                    spinningHairSprite.Play("idle");
                } else {
                    walker.Sprite.Play("spin");
                    spinningHairSprite.Play("spin");

                }
            }

            if (Top > SceneAs<Level>().Bounds.Bottom) {
                Die();
            } else if (Right > SceneAs<Level>().Bounds.Right && walker.Sprite.Scale.X == 1) {
                Flip();
            } else if (Left < SceneAs<Level>().Bounds.Left && walker.Sprite.Scale.X == -1) {
                Flip();
            }

            if (OnGround()) {
                CheckSmartFlip();
            }  //check it again because i don't want it to flip in midair

            bounceSfxDelay -= Engine.DeltaTime; //sound timer
            MoveH(speed.X * Engine.DeltaTime, onCollideH, null);
            MoveV(speed.Y * Engine.DeltaTime, onCollideV, null);
            walker.Position = Position;
            spinningHairSprite.Scale.X = walker.Sprite.Scale.X;

        }

        private void OnCollideH(CollisionData data) {
            if (data.Hit is DashSwitch) {
                (data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitX * Math.Sign(speed.X));
            }

            Flip();
            if (Math.Abs(speed.X) > 100f) {
                ImpactParticles(data.Direction);
            }
        }
        private void OnCollideV(CollisionData data) {
            if (data.Hit is DashSwitch) {
                (data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitX * Math.Sign(speed.Y));
            }

            if (speed.Y > 160f) {
                ImpactParticles(data.Direction);
            }
        }

        private void CheckSmartFlip() {
            if (smart && OnGround() && !idle) {
                if (walker.Sprite.Scale.X == 1) {
                    if (!OnGround(Position + new Vector2(8, 0))) {
                        Flip();
                    }
                } else {
                    if (!OnGround(Position + new Vector2(-8, 0))) {
                        Flip();
                    }
                }
            }
        }

        private void Flip() {
            if (!inPipe) {
                walker.Sprite.Scale.X = -walker.Sprite.Scale.X;
            }
        }

        private void ImpactParticles(Vector2 dir) {
            float direction;
            Vector2 position;
            Vector2 positionRange;
            if (dir.X > 0f) {
                direction = 3.14159274f;
                position = new Vector2(Right, Y - 4f);
                positionRange = Vector2.UnitY * 6f;
            } else if (dir.X < 0f) {
                direction = 0f;
                position = new Vector2(Left, Y - 4f);
                positionRange = Vector2.UnitY * 6f;
            } else if (dir.Y > 0f) {
                direction = -1.57079637f;
                position = new Vector2(X, Bottom);
                positionRange = Vector2.UnitX * 6f;
            } else {
                direction = 1.57079637f;
                position = new Vector2(X, Top);
                positionRange = Vector2.UnitX * 6f;
            }

            SceneAs<Level>().Particles.Emit(TheoCrystal.P_Impact, 12, position, positionRange, direction);
        }

        private BadelineDummy walker;
        private Color hairColor;
        private Sprite spinningHairSprite; //used for pipes
        private readonly bool left = true;
        private readonly bool idle = false;
        private Vector2 speed = new(0f, 0f);

        private readonly bool weak = false; //dies if it touches the player
        private readonly bool noBackpack = false;
        private readonly bool dangerous = false; //the player dies if touched
        private readonly bool ally = false; //the player dies if it dies
        private readonly bool bouncy = false; //the player bounces if touched
        private bool dead = false; //used for post - death stuff, it doesn't get "deleted"
        private readonly bool smart = false; //turn left or right, doesnt fall, like a red koopa
        private readonly bool isMute = false; //the walk sound is annoying
        private readonly Collision onCollideH;
        private readonly Collision onCollideV;
        //holdable stuff
        private readonly Holdable holdable;
        private float noGravityTimer;
        //bounce sound
        private float bounceSfxDelay;
        //pandorasbox crossover stuff
        private readonly bool pandyInstalled;
        private bool inPipe;
        //deathflag
        private readonly string deathflag = "WalkelineIsDead"; // bugged - Kahuna
        public bool TriggerHappy = false; //interacts with triggers
    }
}
