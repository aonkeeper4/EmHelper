using Celeste.Mod.EmHelper.Module;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EmHelper.Entities {
    [Tracked]
    [CustomEntity(new string[]
    {
        "EmHelper/MonumentspikesUp = LoadUp",
        "EmHelper/MonumentspikesDown = LoadDown",
        "EmHelper/MonumentspikesLeft = LoadLeft",
        "EmHelper/MonumentspikesRight = LoadRight"
    })]
    public class MonumentSpikes : Entity {
        private readonly MonumentActivator activator;
        private bool forceDisabled;

        public static Entity LoadUp(Level level, LevelData levelData, Vector2 offset, EntityData entityData) {
            entityData.Values["type"] = entityData.Attr("type", "default");
            return new MonumentSpikes(entityData, offset, Directions.Up);
        }

        public static Entity LoadDown(Level level, LevelData levelData, Vector2 offset, EntityData entityData) {
            entityData.Values["type"] = entityData.Attr("type", "default");
            return new MonumentSpikes(entityData, offset, Directions.Down);
        }

        public static Entity LoadLeft(Level level, LevelData levelData, Vector2 offset, EntityData entityData) {
            entityData.Values["type"] = entityData.Attr("type", "default");
            return new MonumentSpikes(entityData, offset, Directions.Left);
        }

        public static Entity LoadRight(Level level, LevelData levelData, Vector2 offset, EntityData entityData) {
            entityData.Values["type"] = entityData.Attr("type", "default");
            return new MonumentSpikes(entityData, offset, Directions.Right);
        }

        public MonumentSpikes(Vector2 position, int size, Directions direction, string type, Color color, bool active)
            : base(position) {
            Color disablecolor = Calc.HexToColor("667da5");
            disabledColor = new Color(disablecolor.R / 255f * (color.R / 255f), disablecolor.G / 255f * (color.G / 255f), disablecolor.B / 255f * (color.B / 255f), 1f);
            Depth = -1;
            this.direction = direction;
            this.size = size;
            overrideType = type;
            switch (direction) {
                case Directions.Up:
                    Collider = new Hitbox(size, 3f, 0f, -3f);
                    Add(new LedgeBlocker(_ => CheckGravity(false)));
                    break;
                case Directions.Down:
                    Collider = new Hitbox(size, 3f, 0f, 0f);
                    Add(new LedgeBlocker(_ => CheckGravity(true)));
                    break;
                case Directions.Left:
                    Collider = new Hitbox(3f, size, -3f, 0f);
                    Add(new LedgeBlocker(null));
                    break;
                case Directions.Right:
                    Collider = new Hitbox(3f, size, 0f, 0f);
                    Add(new LedgeBlocker(null));
                    break;
            }

            Add(new PlayerCollider(new Action<Player>(OnCollide), null, null));
            Add(new StaticMover {
                OnShake = new Action<Vector2>(OnShake),
                SolidChecker = new Func<Solid, bool>(IsRiding),
                JumpThruChecker = new Func<JumpThru, bool>(IsRiding),
                OnEnable = new Action(OnStaticMoverEnable),
                OnDisable = new Action(OnStaticMoverDisable)
            });

            activator = new MonumentActivator(color, active, OnToggle);
            Add(activator);
        }

        private static bool CheckGravity(bool inverted) => GravityHelperImports.IsPlayerInverted != null && GravityHelperImports.IsPlayerInverted() == inverted;

        public MonumentSpikes(EntityData data, Vector2 offset, Directions dir)
            : this(data.Position + offset, GetSize(data, dir), dir, data.Attr("type", "default"), data.HexColor("color", Calc.HexToColor("82d9ff")), data.Bool("active", true)) {
        }

        public void SetSpikeColor(Color color) {
            foreach (Component component in Components) {
                if (component is Image image) {
                    image.Color = color;
                }
            }
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            AreaData areaData = AreaData.Get(scene);
            spikeType = areaData.Spike;
            if (!string.IsNullOrEmpty(overrideType) && !overrideType.Equals("default")) {
                spikeType = overrideType;
            }

            string str = direction.ToString().ToLower();
            if (spikeType == "tentacles") {
                for (int i = 0; i < size / 16; i++) {
                    AddTentacle(i);
                }

                if (size / 8 % 2 == 1) {
                    AddTentacle((size / 16) - 0.5f);
                    return;
                }
            } else {
                List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("danger/spikes/" + spikeType + "_" + str);
                for (int j = 0; j < size / 8; j++) {
                    Image image = new(Calc.Random.Choose(atlasSubtextures));
                    switch (direction) {
                        case Directions.Up:
                            image.JustifyOrigin(0.5f, 1f);
                            image.Position = (Vector2.UnitX * (j + 0.5f) * 8f) + Vector2.UnitY;
                            break;
                        case Directions.Down:
                            image.JustifyOrigin(0.5f, 0f);
                            image.Position = (Vector2.UnitX * (j + 0.5f) * 8f) - Vector2.UnitY;
                            break;
                        case Directions.Left:
                            image.JustifyOrigin(1f, 0.5f);
                            image.Position = (Vector2.UnitY * (j + 0.5f) * 8f) + Vector2.UnitX;
                            break;
                        case Directions.Right:
                            image.JustifyOrigin(0f, 0.5f);
                            image.Position = (Vector2.UnitY * (j + 0.5f) * 8f) - Vector2.UnitX;
                            break;
                    }

                    Add(image);
                }
            }

            OnToggle(activator.Activated);
        }

        private void AddTentacle(float i) {
            Sprite sprite = GFX.SpriteBank.Create("tentacles");
            sprite.Play(Calc.Random.Next(3).ToString(), true, true);
            sprite.Position = ((direction is Directions.Up or Directions.Down) ? Vector2.UnitX : Vector2.UnitY) * (i + 0.5f) * 16f;
            sprite.Scale.X = Calc.Random.Choose(-1, 1);
            sprite.SetAnimationFrame(Calc.Random.Next(sprite.CurrentAnimationTotalFrames));
            if (direction == Directions.Up) {
                sprite.Rotation = -1.57079637f;
                Sprite sprite2 = sprite;
                float num = sprite2.Y;
                sprite2.Y = num + 1f;
            } else if (direction == Directions.Right) {
                sprite.Rotation = 0f;
                Sprite sprite3 = sprite;
                float num = sprite3.X;
                sprite3.X = num - 1f;
            } else if (direction == Directions.Left) {
                sprite.Rotation = 3.14159274f;
                Sprite sprite4 = sprite;
                float num = sprite4.X;
                sprite4.X = num + 1f;
            } else if (direction == Directions.Down) {
                sprite.Rotation = 1.57079637f;
                Sprite sprite5 = sprite;
                float num = sprite5.Y;
                sprite5.Y = num - 1f;
            }

            sprite.Rotation += 1.57079637f;
            Add(sprite);
        }

        public void OnToggle(bool activated) {
            bool enabled = activated && !forceDisabled;
            Active = Collidable = enabled;
            SetSpikeColor(enabled ? activator.Index : disabledColor);
        }

        public void OnStaticMoverEnable() {
            forceDisabled = false;
            OnToggle(activator.Activated);
        }

        public void OnStaticMoverDisable() {
            forceDisabled = true;
            OnToggle(false);
        }

        private void OnShake(Vector2 amount) {
            imageOffset += amount;
        }

        public override void Render() {
            Vector2 position = Position;
            Position += imageOffset;
            base.Render();
            Position = position;
        }

        public void SetOrigins(Vector2 origin) {
            foreach (Component component in Components) {
                if (component is Image image) {
                    Vector2 vector = origin - Position;
                    image.Origin = image.Origin + vector - image.Position;
                    image.Position = vector;
                }
            }
        }

        private void OnCollide(Player player) {
            var inverted = GravityHelperImports.IsPlayerInverted?.Invoke() ?? false;

            switch (direction) {
                case Directions.Up:
                    if (!inverted && player.Speed.Y >= 0f && player.Bottom <= Bottom ||
                        inverted && player.Speed.Y <= 0f) {
                        player.Die(new Vector2(0f, -1f), false, true);
                        return;
                    }

                    break;
                case Directions.Down:
                    if (!inverted && player.Speed.Y <= 0f ||
                        inverted && player.Speed.Y >= 0f && player.Top >= Top) {
                        player.Die(new Vector2(0f, 1f), false, true);
                        return;
                    }

                    break;
                case Directions.Left:
                    if (player.Speed.X >= 0f) {
                        player.Die(new Vector2(-1f, 0f), false, true);
                        return;
                    }

                    break;
                case Directions.Right:
                    if (player.Speed.X <= 0f) {
                        player.Die(new Vector2(1f, 0f), false, true);
                    }

                    break;
                default:
                    return;
            }
        }

        private static int GetSize(EntityData data, Directions dir) {
            return dir > Directions.Down ? data.Height : data.Width;
        }

        private bool IsRiding(Solid solid) {
            return direction switch {
                Directions.Up => CollideCheckOutside(solid, Position + Vector2.UnitY),
                Directions.Down => CollideCheckOutside(solid, Position - Vector2.UnitY),
                Directions.Left => CollideCheckOutside(solid, Position + Vector2.UnitX),
                Directions.Right => CollideCheckOutside(solid, Position - Vector2.UnitX),
                _ => false,
            };
        }

        private bool IsRiding(JumpThru jumpThru) {
            return direction == Directions.Up && CollideCheck(jumpThru, Position + Vector2.UnitY);
        }

        private readonly Directions direction;

        private Vector2 imageOffset;

        private readonly int size;

        private readonly string overrideType;

        private string spikeType;

        private Color disabledColor;

        public enum Directions {
            Up,
            Down,
            Left,
            Right
        }
    }
}
