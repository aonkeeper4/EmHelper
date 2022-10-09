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
            this.onlyEnable = onlyEnable;
            this.onlyDisable = onlyDisable;
            Collider = new Hitbox(16f, 24f, -8f, -12f);
            Add(new PlayerCollider(new Action<Player>(OnPlayer), null, null));
            Add(new HoldableCollider(new Action<Holdable>(OnHoldable), null));
            string spritepath = "monumentflipswitch" + this.pattern.ToString();
            Add(sprite = GFX.SpriteBank.Create(spritepath));
            Depth = 2000;

            activator = new MonumentActivator(color, false, (activated) => SetSprite(true));
            Add(activator);
        }

        public MonumentFlipSwitch(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Bool("onlyEnable", false), data.Bool("onlyDisable", false), data.HexColor("color", Calc.HexToColor("82d9ff")), data.Bool("mute", false), data.Int("pattern", 0)) {
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            sprite.Color = activator.Index;
            SetSprite(false);
        }

        public void SetSprite(bool animate) {
            if (animate) {
                if (playSounds && !mute) {
                    Audio.Play(activator.Activated ? "event:/game/09_core/switch_to_cold" : "event:/game/09_core/switch_to_hot", Position);
                }

                if (Usable) {
                    sprite.Play(activator.Activated ? "ice" : "hot", false, false);
                } else {
                    if (playSounds && !mute) {
                        Audio.Play("event:/game/09_core/switch_dies", Position);
                    }

                    sprite.Play(activator.Activated ? "iceOff" : "hotOff", false, false);
                }
            } else if (Usable) {
                sprite.Play(activator.Activated ? "iceLoop" : "hotLoop", false, false);
            } else {
                sprite.Play(activator.Activated ? "iceOffLoop" : "hotOffLoop", false, false);
            }

            playSounds = false;
        }

        private void OnPlayer(Player player)
        {
            if (Usable && cooldownTimer <= 0f) {
                playSounds = true;
                this.ToggleMonumentActivators(activator.Index);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                cooldownTimer = 1f;
            }
        }

        private void OnHoldable(Holdable holdable) {
            if (Usable && cooldownTimer <= 0f) {
                playSounds = true;
                this.ToggleMonumentActivators(activator.Index);
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

        private bool Usable => (!onlyEnable || activator.Activated) && (!onlyDisable || !activator.Activated);


        private float cooldownTimer;

        private readonly bool mute;

        private readonly bool onlyEnable;

        private readonly bool onlyDisable;

        private bool playSounds;

        private readonly Sprite sprite;

        private readonly int pattern;

        private readonly MonumentActivator activator;
    }
}
