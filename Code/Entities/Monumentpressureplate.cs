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
            : this(data.Position + levelOffset, data.Int("pattern", 0), data.Bool("onetime", false), data.HexColor("color", Calc.HexToColor("82d9ff")), data.Bool("mute", false), data.Bool("isButton", false), data.Bool("disable", false)) {
        }

        public MonumentPressurePlate(Vector2 position, int pattern, bool onetime, Color color, bool mute, bool isButton, bool ButtonDisable)
            : base(position) {
            IsButton = isButton;
            this.ButtonDisable = ButtonDisable;
            this.mute = mute;
            Depth = -59;
            this.onetime = onetime;
            this.pattern = pattern;
            pressed = new List<Image>();
            solid = new List<Image>();
            all = new List<Image>();
            Index = color;
            Collider = new Hitbox(14f, 3f, -7f, -3f);

            staticMover = new StaticMover {
                OnAttach = delegate (Platform p) { Depth = p.Depth + 1; },
                SolidChecker = (Solid s) => CollideCheck(s, Position + Vector2.UnitY),
                JumpThruChecker = (JumpThru jt) => CollideCheck(jt, Position + Vector2.UnitY)
            };
            Add(staticMover);
            staticMover.OnEnable = new Action(OnStaticMoverEnable);
            staticMover.OnDisable = new Action(OnStaticMoverDisable);

            monumentactivator = new MonumentActivator();
            Add(monumentactivator);
        }

        private void OnStaticMoverEnable() {
            CassetteDisable = false;
            if (ButtonDisable) {
                OnDisable();
            } else {
                OnEnable();
            }
        }

        private void OnStaticMoverDisable() {
            CassetteDisable = true;
            OnDisable();
        }

        public void OnEnable() {
            foreach (Image image in solid)
            {
                image.Color = Index;
            }

            Collidable = true;
            pressureactivated = false;
            prevstate = pressureactivated;
            UpdateVisualState();
        }

        public void OnDisable() {
            foreach (Image image in solid)
            {
                image.Color = disabledcolor;
            }

            Collidable = false;
            if (ButtonDisable) {
                UpdateVisualState(true);
            } else {
                UpdateVisualState();
            }
        }

        private void Trigger() {
            if (onetime) {
                turnoff = true;
            } //onetime button

            prevstate = pressureactivated;
            if (pressureactivated && !mute) {
                Audio.Play("event:/game/general/cassette_block_switch_1");
            } else if (!mute) {
                Audio.Play("event:/game/general/cassette_block_switch_2");
            }

            UpdateVisualState();
            monumentactivator.Activated(Index);
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            Color color = Calc.HexToColor("667da5");
            disabledcolor = new Color(color.R / 255f * (Index.R / 255f), color.G / 255f * (Index.G / 255f), color.B / 255f * (Index.B / 255f), 1f);
            SetImage();
            foreach (Image image in solid)
            {
                image.Color = Index;
            }

            foreach (Image image2 in pressed)
            {
                image2.Color = disabledcolor;
            }

            if (ButtonDisable) {
                UpdateVisualState(true);
            } else {
                UpdateVisualState();
            }
        }

        public override void Update() {
            base.Update();
            if (turnoff || CassetteDisable || ButtonDisable) {
                return;
            }

            List<Entity> actors = Scene.Tracker.GetEntities<Actor>();
            pressureactivated = false;
            foreach (Entity actor in actors) {
                bool flag = Collide.Check(actor, this);
                if (flag) {
                    pressureactivated = true;
                    break;
                }
            }

            if (prevstate != pressureactivated) {
                Trigger();
            }
        }

        private void SetImage() {
            pressed.Add(CreateImage(GFX.Game["objects/monumentpressureplate/pressed"]));
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/monumentpressureplate/solid");
            solid.Add(CreateImage(atlasSubtextures[pattern % atlasSubtextures.Count]));
        }

        private Image CreateImage(MTexture tex) {
            Image image = new(tex) {
                Color = Index
            };
            image.Origin.X = image.Width / 2f;
            image.Origin.Y = image.Height - 1;
            Add(image);
            all.Add(image);
            return image;
        }

        // Used for pressure plates
        private void UpdateVisualState() {
            foreach (Image image in solid) {
                image.Visible = !turnoff && !pressureactivated;
            }

            foreach (Image image2 in pressed) {
                image2.Visible = turnoff || pressureactivated;
            }
        }

        // Used for buttons
        private void UpdateVisualState(bool showpressed) {
            foreach (Image image in solid) {
                image.Visible = !showpressed;
            }

            foreach (Image image2 in pressed) {
                image2.Visible = showpressed;
            }
        }

        public void SetColor(Color color) {
            foreach (Component component in Components) {
                if (component is Image image) {
                    image.Color = color;
                }
            }
        }

        public bool ButtonDisable = false; // useful if it's a button
        public bool IsButton = false;
        public bool CassetteDisable = false;
        private bool turnoff = false;
        private readonly bool mute = false;
        private readonly int pattern;
        private readonly bool onetime = false;
        private bool pressureactivated = false; //pressure plate only
        private bool prevstate = false; //pressure plate stuff
        public Color Index;
        private Color disabledcolor;
        private readonly List<Image> pressed;
        private readonly List<Image> solid;
        private readonly List<Image> all;
        private readonly StaticMover staticMover;

        private readonly MonumentActivator monumentactivator;
    }
}
