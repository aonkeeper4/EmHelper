using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.EmHelper.Entities {
    [Tracked]
    [CustomEntity("EmHelper/Monumentswitchblock")]
    public class MonumentSwitchBlock : Solid {
        private readonly MonumentActivator activator;

        public MonumentSwitchBlock(Vector2 position, float width, float height, int pattern, bool active, Color color)
            : base(position, width, height, false) {
            this.pattern = pattern;
            blockHeight = 2;
            pressed = new List<Image>();
            solid = new List<Image>();
            all = new List<Image>();
            SurfaceSoundIndex = 35;
            Add(occluder = new LightOcclude(1f));

            activator = new MonumentActivator(color, active);
            Add(activator);
        }

        public MonumentSwitchBlock(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Width, data.Height, data.Int("pattern", 0), data.Bool("active", false), data.HexColor("color", Calc.HexToColor("82d9ff"))) {
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            Color color = Calc.HexToColor("667da5");
            Color disabledColor = new(color.R / 255f * (activator.Index.R / 255f), color.G / 255f * (activator.Index.G / 255f), color.B / 255f * (activator.Index.B / 255f), 1f);
            scene.Add(side = new BoxSide(this, disabledColor));
            foreach (StaticMover staticMover in staticMovers) {
                if (staticMover.Entity is Spikes spikes) {
                    spikes.EnabledColor = activator.Index;
                    spikes.DisabledColor = disabledColor;
                    spikes.VisibleWhenDisabled = true;
                    spikes.SetSpikeColor(activator.Index);
                }

                if (staticMover.Entity is Spring spring) {
                    spring.DisabledColor = disabledColor;
                    spring.VisibleWhenDisabled = true;
                }
            }

            if (group == null) {
                groupLeader = true;
                group = new List<MonumentSwitchBlock> { this };
                FindInGroup(this);
                float num = float.MaxValue;
                float num2 = float.MinValue;
                float num3 = float.MaxValue;
                float num4 = float.MinValue;
                foreach (MonumentSwitchBlock switchblock in group) {
                    if (switchblock.Left < num) {
                        num = switchblock.Left;
                    }

                    if (switchblock.Right > num2) {
                        num2 = switchblock.Right;
                    }

                    if (switchblock.Bottom > num4) {
                        num4 = switchblock.Bottom;
                    }

                    if (switchblock.Top < num3) {
                        num3 = switchblock.Top;
                    }
                }

                groupOrigin = new Vector2((int)(num + ((num2 - num) / 2f)), (int)num4);
                wigglerScaler = new Vector2(Calc.ClampedMap(num2 - num, 32f, 96f, 1f, 0.2f), Calc.ClampedMap(num4 - num3, 32f, 96f, 1f, 0.2f));
                Add(wiggler = Wiggler.Create(0.3f, 3f, null, false, false));
                foreach (MonumentSwitchBlock switchblock2 in group) {
                    switchblock2.wiggler = wiggler;
                    switchblock2.wigglerScaler = wigglerScaler;
                    switchblock2.groupOrigin = groupOrigin;
                }
            }

            foreach (StaticMover staticMover2 in staticMovers) {
                if (staticMover2.Entity is Spikes spikes2) {
                    spikes2.SetOrigins(groupOrigin);
                }
            }

            for (float num5 = Left; num5 < Right; num5 += 8f) {
                for (float num6 = Top; num6 < Bottom; num6 += 8f) {
                    bool flag = CheckForSame(num5 - 8f, num6);
                    bool flag2 = CheckForSame(num5 + 8f, num6);
                    bool flag3 = CheckForSame(num5, num6 - 8f);
                    bool flag4 = CheckForSame(num5, num6 + 8f);
                    if (flag && flag2 && flag3 && flag4) {
                        if (!CheckForSame(num5 + 8f, num6 - 8f)) {
                            SetImage(num5, num6, 3, 0);
                        } else if (!CheckForSame(num5 - 8f, num6 - 8f)) {
                            SetImage(num5, num6, 3, 1);
                        } else if (!CheckForSame(num5 + 8f, num6 + 8f)) {
                            SetImage(num5, num6, 3, 2);
                        } else if (!CheckForSame(num5 - 8f, num6 + 8f)) {
                            SetImage(num5, num6, 3, 3);
                        } else {
                            SetImage(num5, num6, 1, 1);
                        }
                    } else if (flag && flag2 && !flag3 && flag4) {
                        SetImage(num5, num6, 1, 0);
                    } else if (flag && flag2 && flag3 && !flag4) {
                        SetImage(num5, num6, 1, 2);
                    } else if (flag && !flag2 && flag3 && flag4) {
                        SetImage(num5, num6, 2, 1);
                    } else if (!flag && flag2 && flag3 && flag4) {
                        SetImage(num5, num6, 0, 1);
                    } else if (flag && !flag2 && !flag3 && flag4) {
                        SetImage(num5, num6, 2, 0);
                    } else if (!flag && flag2 && !flag3 && flag4) {
                        SetImage(num5, num6, 0, 0);
                    } else if (flag && !flag2 && flag3 && !flag4) {
                        SetImage(num5, num6, 2, 2);
                    } else if (!flag && flag2 && flag3 && !flag4) {
                        SetImage(num5, num6, 0, 2);
                    }
                }
            }

            UpdateVisualState();
        }

        private void FindInGroup(MonumentSwitchBlock block) {
            foreach (MonumentSwitchBlock switchblock in Scene.Tracker.GetEntities<MonumentSwitchBlock>()) {
                if (switchblock != this && switchblock != block && switchblock.activator.Index == activator.Index && (switchblock.CollideRect(new Rectangle((int)block.X - 1, (int)block.Y, (int)block.Width + 2, (int)block.Height)) || switchblock.CollideRect(new Rectangle((int)block.X, (int)block.Y - 1, (int)block.Width, (int)block.Height + 2))) && !group.Contains(switchblock)) {
                    group.Add(switchblock);
                    FindInGroup(switchblock);
                    switchblock.group = group;
                }
            }
        }

        private bool CheckForSame(float x, float y) {
            foreach (MonumentSwitchBlock switchblock in Scene.Tracker.GetEntities<MonumentSwitchBlock>()) {
                if (switchblock.activator.Index == activator.Index && switchblock.Collider.Collide(new Rectangle((int)x, (int)y, 8, 8))) {
                    return true;
                }
            }

            return false;
        }

        private void SetImage(float x, float y, int tx, int ty) {
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/monumentswitchblock/pressed");
            pressed.Add(CreateImage(x, y, tx, ty, atlasSubtextures[pattern % atlasSubtextures.Count]));
            atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/monumentswitchblock/solid");
            solid.Add(CreateImage(x, y, tx, ty, atlasSubtextures[pattern % atlasSubtextures.Count]));
        }

        private Image CreateImage(float x, float y, int tx, int ty, MTexture tex) {
            Vector2 value = new(x - X, y - Y);
            Image image = new(tex.GetSubtexture(tx * 8, ty * 8, 8, 8, null));
            Vector2 vector = groupOrigin - Position;
            image.Origin = vector - value;
            image.Position = vector;
            image.Color = activator.Index;
            Add(image);
            all.Add(image);
            return image;
        }

        public override void Update() {
            base.Update();
            if (activator.Activated && !Collidable) {
                bool flag = false;
                if (BlockedCheck()) {
                    flag = true;
                }

                if (!flag) {
                    Collidable = true;
                    ShiftSize(-1);
                    EnableStaticMovers();
                    if (groupLeader) {
                        wiggler.Start();
                    }
                }
            } else if (!activator.Activated && Collidable) {
                Collidable = false;
                ShiftSize(1);
                DisableStaticMovers();
            }

            UpdateVisualState();
        }

        public bool BlockedCheck() {
            TheoCrystal theoCrystal = CollideFirst<TheoCrystal>();
            if (theoCrystal != null && !TryActorWiggleUp(theoCrystal)) {
                return true;
            }

            Walkeline walkeline = CollideFirst<Walkeline>();
            if (walkeline != null && !TryActorWiggleUp(walkeline)) {
                return true;
            }

            Player player = CollideFirst<Player>();
            return player != null && !TryActorWiggleUp(player);
        }

        private void UpdateVisualState() {
            if (!Collidable) {
                Depth = 8990;
            } else {
                Player entity = Scene.Tracker.GetEntity<Player>();
                Depth = entity != null && entity.Top >= Bottom - 1f ? 10 : -10;
            }

            foreach (StaticMover staticMover in staticMovers) {
                staticMover.Entity.Depth = Depth + 1;
            }

            side.Depth = Depth + 5;
            side.Visible = blockHeight > 0;
            occluder.Visible = Collidable;
            foreach (Image image in solid) {
                image.Visible = Collidable;
            }

            foreach (Image image2 in pressed) {
                image2.Visible = !Collidable;
            }

            if (groupLeader) {
                Vector2 scale = new(1f + (wiggler.Value * 0.05f * wigglerScaler.X), 1f + (wiggler.Value * 0.15f * wigglerScaler.Y));
                foreach (MonumentSwitchBlock switchblock in group) {
                    foreach (Image image3 in switchblock.all) {
                        image3.Scale = scale;
                    }

                    foreach (StaticMover staticMover2 in switchblock.staticMovers) {
                        if (staticMover2.Entity is Spikes spikes) {
                            foreach (Component component in spikes.Components) {
                                if (component is Image image4) {
                                    image4.Scale = scale;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ShiftSize(int amount) {
            MoveV(amount);
            blockHeight -= amount;
        }

        private bool TryActorWiggleUp(Entity actor) {
            foreach (MonumentSwitchBlock switchblock in group) {
                if (switchblock != this && switchblock.CollideCheck(actor, switchblock.Position + (Vector2.UnitY * 4f))) {
                    return false;
                }
            }

            bool collidable = Collidable;
            Collidable = true;
            for (int i = 1; i <= 4; i++) {
                if (!actor.CollideCheck<Solid>(actor.Position - (Vector2.UnitY * i))) {
                    actor.Position -= Vector2.UnitY * i;
                    Collidable = collidable;
                    return true;
                }
            }

            Collidable = collidable;
            return false;
        }

        private int blockHeight;

        private List<MonumentSwitchBlock> group;

        private readonly int pattern;

        private bool groupLeader;

        private Vector2 groupOrigin;

        private readonly List<Image> pressed;

        private readonly List<Image> solid;

        private readonly List<Image> all;

        private readonly LightOcclude occluder;

        private Wiggler wiggler;

        private Vector2 wigglerScaler;

        private BoxSide side;

        private class BoxSide : Entity {
            public BoxSide(MonumentSwitchBlock block, Color color) {
                this.block = block;
                this.color = color;
            }

            public override void Render() {
                Draw.Rect(block.X, block.Y + block.Height - 8f, block.Width, 8 + block.blockHeight, color);
            }

            private readonly MonumentSwitchBlock block;

            private Color color;
        }
    }
}
