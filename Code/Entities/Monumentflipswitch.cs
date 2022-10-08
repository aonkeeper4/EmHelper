using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.EmHelper.Entities {
    [Tracked]
    [CustomEntity("EmHelper/Monumentflipswitch")]
    public class MonumentFlipSwitch : Entity {
        public MonumentFlipSwitch(Vector2 position, bool onlyEnable, bool onlyDisable, Color color, bool mute, int pattern)
            : base(position) {
            this.pattern = pattern;
            this.mute = mute;
            Index = color;
            Enable = false;
            this.onlyEnable = onlyEnable;
            this.onlyDisable = onlyDisable;
            Collider = new Hitbox(16f, 24f, -8f, -12f);
            Add(new PlayerCollider(new Action<Player>(OnPlayer), null, null));
            Add(new HoldableCollider(new Action<Holdable>(OnHoldable), null));
            string spritepath = "monumentflipswitch" + this.pattern.ToString();
            Add(sprite = GFX.SpriteBank.Create(spritepath));
            Depth = 2000;

            monumentActivator = new MonumentActivator();
            Add(monumentActivator);
        }

        public MonumentFlipSwitch(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Bool("onlyEnable", false), data.Bool("onlyDisable", false), data.HexColor("color", Calc.HexToColor("82d9ff")), data.Bool("mute", false), data.Int("pattern", 0)) {
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            sprite.Color = Index;
            SetSprite(false);
        }

        public void SetSprite(bool animate) {
            if (animate) {
                if (playSounds && !mute) {
                    Audio.Play(Enable ? "event:/game/09_core/switch_to_cold" : "event:/game/09_core/switch_to_hot", Position);
                }

                if (Usable) {
                    sprite.Play(Enable ? "ice" : "hot", false, false);
                } else {
                    if (playSounds && !mute) {
                        Audio.Play("event:/game/09_core/switch_dies", Position);
                    }

                    sprite.Play(Enable ? "iceOff" : "hotOff", false, false);
                }
            } else if (Usable) {
                sprite.Play(Enable ? "iceLoop" : "hotLoop", false, false);
            } else {
                sprite.Play(Enable ? "iceOffLoop" : "hotOffLoop", false, false);
            }

            playSounds = false;
        }

        private void OnPlayer(Player player)
        {
            if (Usable && cooldownTimer <= 0f) {
                playSounds = true;
                monumentActivator.Activated(Index);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                cooldownTimer = 1f;
            }
        }

        private void OnHoldable(Holdable holdable) {
            if (Usable && cooldownTimer <= 0f) {
                playSounds = true;
                monumentActivator.Activated(Index);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                cooldownTimer = 1f;
            }
        }

        public override void Update() {
            base.Update();
            if (cooldownTimer > 0f) {
                cooldownTimer -= Engine.DeltaTime;
            }
        }

        private bool Usable => (!onlyEnable || Enable) && (!onlyDisable || !Enable);


        private float cooldownTimer;

        private readonly bool mute;

        private readonly bool onlyEnable;

        private readonly bool onlyDisable;

        private bool playSounds;

        public Color Index;

        public bool Enable;

        private readonly Sprite sprite;

        private readonly int pattern;

        private readonly MonumentActivator monumentActivator;
    }
}
