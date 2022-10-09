using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EmHelper.Entities {
    [Tracked]
    [CustomEntity("EmHelper/Monumentpressureplate")]
    public class MonumentPressurePlate : Entity {
        public MonumentPressurePlate(EntityData data, Vector2 levelOffset)
            : this(data.Position + levelOffset, data.Int("pattern", 0), data.Bool("onetime", false), data.HexColor("color", Calc.HexToColor("82d9ff")), data.Bool("mute", false), data.Bool("isButton", false), !data.Bool("disable", false)) {
        }

        public MonumentPressurePlate(Vector2 position, int pattern, bool oneUse, Color color, bool mute, bool isButton, bool active)
            : base(position) {
            this.isButton = isButton;
            this.mute = mute;
            Depth = -59;
            this.oneUse = oneUse;
            this.pattern = pattern;
            pressed = new List<Image>();
            solid = new List<Image>();
            all = new List<Image>();
            Collider = new Hitbox(14f, 3f, -7f, -3f);

            staticMover = new StaticMover {
                OnAttach = delegate (Platform p) { Depth = p.Depth + 1; },
                SolidChecker = (Solid s) => CollideCheck(s, Position + Vector2.UnitY),
                JumpThruChecker = (JumpThru jt) => CollideCheck(jt, Position + Vector2.UnitY)
            };
            Add(staticMover);
            staticMover.OnEnable = new Action(OnStaticMoverEnable);
            staticMover.OnDisable = new Action(OnStaticMoverDisable);
            
            activator = new MonumentActivator(color, active, OnToggle);
            if (isButton) {
                Add(activator);
            }
        }

        private void OnToggle(bool activated) {
            UpdateVisuals(activated); 
        }

        private void UpdateVisuals(bool activated) {
            bool enabled = activated && !forceDisabled;
            foreach (Image image in solid) {
                image.Color = enabled ? activator.Index : disabledColor;
                image.Visible = enabled;
            }

            foreach (Image image in solid) {
                image.Visible = enabled;
            }

            foreach (Image image in pressed) {
                image.Visible = !enabled;
            }
        }

        private void OnStaticMoverEnable() {
            Collidable = true;
            forceDisabled = false;
            UpdateVisuals(activator.Activated);
        }

        private void OnStaticMoverDisable() {
            Collidable = false;
            forceDisabled = true;
            UpdateVisuals(false);
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            Color color = Calc.HexToColor("667da5");
            disabledColor = new Color(color.R / 255f * (activator.Index.R / 255f), color.G / 255f * (activator.Index.G / 255f), color.B / 255f * (activator.Index.B / 255f), 1f);
            SetImage();
            UpdateVisuals(activator.Activated);
        }

        public override void Update() {
            base.Update();
            if (turnOff || forceDisabled || !activator.Activated) {
                return;
            }

            bool pressed = CollideCheck<Actor>();
            if (isButton && pressed) {
                ChangePress(true);
            } else if (wasPressed != pressed) {
                wasPressed = pressed;
                ChangePress(pressed);
                UpdateVisuals(!pressed);
            }
        }

        private void ChangePress(bool pressed) {
            if (oneUse) {
                turnOff = true;
            }

            if (!mute) {
                if (pressed) {
                    Audio.Play("event:/game/general/cassette_block_switch_1");
                } else {
                    Audio.Play("event:/game/general/cassette_block_switch_2");
                }
            }

            this.ToggleMonumentActivators(activator.Index);
        }

        private void SetImage() {
            pressed.Add(CreateImage(GFX.Game["objects/monumentpressureplate/pressed"], activator.Index));
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/monumentpressureplate/solid");
            solid.Add(CreateImage(atlasSubtextures[pattern % atlasSubtextures.Count], disabledColor));
        }

        private Image CreateImage(MTexture tex, Color color) {
            Image image = new(tex) {
                Color = color
            };
            image.Origin.X = image.Width / 2f;
            image.Origin.Y = image.Height - 1;
            Add(image);
            all.Add(image);
            return image;
        }

        private bool wasPressed;
        private readonly bool isButton;
        private bool forceDisabled;
        private bool turnOff;
        private readonly bool mute;
        private readonly int pattern;
        private readonly bool oneUse;
        private Color disabledColor;
        private readonly List<Image> pressed;
        private readonly List<Image> solid;
        private readonly List<Image> all;
        private readonly StaticMover staticMover;

        private readonly MonumentActivator activator;
    }
}
