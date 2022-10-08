using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EmHelper.Entities
{
    [Tracked]
    [CustomEntity(new string[]
    {
        "EmHelper/MonumentspikesUp = LoadUp",
        "EmHelper/MonumentspikesDown = LoadDown",
        "EmHelper/MonumentspikesLeft = LoadLeft",
        "EmHelper/MonumentspikesRight = LoadRight"
    })]

    public class Monumentspikes : Entity
    {
        public static Entity LoadUp(Level level, LevelData levelData, Vector2 offset, EntityData entityData)
        {
            entityData.Values["type"] = entityData.Attr("type", "default");
            return new Monumentspikes(entityData, offset, Monumentspikes.Directions.Up);
        }

        public static Entity LoadDown(Level level, LevelData levelData, Vector2 offset, EntityData entityData)
        {
            entityData.Values["type"] = entityData.Attr("type", "default");
            return new Monumentspikes(entityData, offset, Monumentspikes.Directions.Down);
        }

        public static Entity LoadLeft(Level level, LevelData levelData, Vector2 offset, EntityData entityData)
        {
            entityData.Values["type"] = entityData.Attr("type", "default");
            return new Monumentspikes(entityData, offset, Monumentspikes.Directions.Left);
        }

        public static Entity LoadRight(Level level, LevelData levelData, Vector2 offset, EntityData entityData)
        {
            entityData.Values["type"] = entityData.Attr("type", "default");
            return new Monumentspikes(entityData, offset, Monumentspikes.Directions.Right);
        }

        public Monumentspikes(Vector2 position, int size, Monumentspikes.Directions direction, string type, Color color, bool active) : base(position)
        {
            this.monumentenable = active;
            this.EnabledColor = color;
            Color disablecolor = Calc.HexToColor("667da5");
            this.DisabledColor = new Color((float)disablecolor.R / 255f * ((float)this.EnabledColor.R / 255f), (float)disablecolor.G / 255f * ((float)this.EnabledColor.G / 255f), (float)disablecolor.B / 255f * ((float)this.EnabledColor.B / 255f), 1f);
            base.Depth = -1;
            this.Direction = direction;
            this.size = size;
            this.overrideType = type;
            switch (direction)
            {
                case Monumentspikes.Directions.Up:
                    base.Collider = new Hitbox((float)size, 3f, 0f, -3f);
                    base.Add(new LedgeBlocker(null));
                    break;
                case Monumentspikes.Directions.Down:
                    base.Collider = new Hitbox((float)size, 3f, 0f, 0f);
                    break;
                case Monumentspikes.Directions.Left:
                    base.Collider = new Hitbox(3f, (float)size, -3f, 0f);
                    base.Add(new LedgeBlocker(null));
                    break;
                case Monumentspikes.Directions.Right:
                    base.Collider = new Hitbox(3f, (float)size, 0f, 0f);
                    base.Add(new LedgeBlocker(null));
                    break;
            }
            base.Add(this.pc = new PlayerCollider(new Action<Player>(this.OnCollide), null, null));
            base.Add(new StaticMover
            {
                OnShake = new Action<Vector2>(this.OnShake),
                SolidChecker = new Func<Solid, bool>(this.IsRiding),
                JumpThruChecker = new Func<JumpThru, bool>(this.IsRiding),
                OnEnable = new Action(this.OnStaticMoverEnable),
                OnDisable = new Action(this.OnStaticMoverDisable)
            });


        }

        public Monumentspikes(EntityData data, Vector2 offset, Monumentspikes.Directions dir) : this(data.Position + offset, Monumentspikes.GetSize(data, dir), dir, data.Attr("type", "default"), data.HexColor("color", Calc.HexToColor("82d9ff")), data.Bool("active", true))
        {
        }

        public void SetSpikeColor(Color color)
        {
            foreach (Component component in base.Components)
            {
                Image image = component as Image;
                if (image != null)
                {
                    image.Color = color;
                }
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            AreaData areaData = AreaData.Get(scene);
            this.spikeType = areaData.Spike;
            if (!string.IsNullOrEmpty(this.overrideType) && !this.overrideType.Equals("default"))
            {
                this.spikeType = this.overrideType;
            }
            string str = this.Direction.ToString().ToLower();
            if (this.spikeType == "tentacles")
            {
                for (int i = 0; i < this.size / 16; i++)
                {
                    this.AddTentacle((float)i);
                }
                if (this.size / 8 % 2 == 1)
                {
                    this.AddTentacle((float)(this.size / 16) - 0.5f);
                    return;
                }
            }
            else
            {
                List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("danger/spikes/" + this.spikeType + "_" + str);
                for (int j = 0; j < this.size / 8; j++)
                {
                    Image image = new Image(Calc.Random.Choose(atlasSubtextures));
                    switch (this.Direction)
                    {
                        case Monumentspikes.Directions.Up:
                            image.JustifyOrigin(0.5f, 1f);
                            image.Position = Vector2.UnitX * ((float)j + 0.5f) * 8f + Vector2.UnitY;
                            break;
                        case Monumentspikes.Directions.Down:
                            image.JustifyOrigin(0.5f, 0f);
                            image.Position = Vector2.UnitX * ((float)j + 0.5f) * 8f - Vector2.UnitY;
                            break;
                        case Monumentspikes.Directions.Left:
                            image.JustifyOrigin(1f, 0.5f);
                            image.Position = Vector2.UnitY * ((float)j + 0.5f) * 8f + Vector2.UnitX;
                            break;
                        case Monumentspikes.Directions.Right:
                            image.JustifyOrigin(0f, 0.5f);
                            image.Position = Vector2.UnitY * ((float)j + 0.5f) * 8f - Vector2.UnitX;
                            break;
                    }
                    base.Add(image);
                }
            }
            if (this.monumentenable)
            {
                OnEnable();
            }
            else { OnDisable(); }
        }

        private void AddTentacle(float i)
        {
            Sprite sprite = GFX.SpriteBank.Create("tentacles");
            sprite.Play(Calc.Random.Next(3).ToString(), true, true);
            sprite.Position = ((this.Direction == Monumentspikes.Directions.Up || this.Direction == Monumentspikes.Directions.Down) ? Vector2.UnitX : Vector2.UnitY) * (i + 0.5f) * 16f;
            sprite.Scale.X = (float)Calc.Random.Choose(-1, 1);
            sprite.SetAnimationFrame(Calc.Random.Next(sprite.CurrentAnimationTotalFrames));
            if (this.Direction == Monumentspikes.Directions.Up)
            {
                sprite.Rotation = -1.57079637f;
                Sprite sprite2 = sprite;
                float num = sprite2.Y;
                sprite2.Y = num + 1f;
            }
            else if (this.Direction == Monumentspikes.Directions.Right)
            {
                sprite.Rotation = 0f;
                Sprite sprite3 = sprite;
                float num = sprite3.X;
                sprite3.X = num - 1f;
            }
            else if (this.Direction == Monumentspikes.Directions.Left)
            {
                sprite.Rotation = 3.14159274f;
                Sprite sprite4 = sprite;
                float num = sprite4.X;
                sprite4.X = num + 1f;
            }
            else if (this.Direction == Monumentspikes.Directions.Down)
            {
                sprite.Rotation = 1.57079637f;
                Sprite sprite5 = sprite;
                float num = sprite5.Y;
                sprite5.Y = num - 1f;
            }
            sprite.Rotation += 1.57079637f;
            base.Add(sprite);
        }

        public void OnStaticMoverEnable()
        {
            this.staticmoverenable = true;
            OnEnable();
        }

        public void OnEnable()
        {
            if (this.staticmoverenable && this.monumentenable)
            {
                this.Active = (this.Visible = (this.Collidable = true));
                this.SetSpikeColor(this.EnabledColor);
            }



        }

        public void OnStaticMoverDisable()
        {
            this.staticmoverenable = false;
            OnDisable();
        }

        public void OnDisable()
        {
            this.Active = (this.Collidable = false);

            this.SetSpikeColor(this.DisabledColor);

        }



        private void OnShake(Vector2 amount)
        {
            this.imageOffset += amount;
        }

        public override void Render()
        {
            Vector2 position = this.Position;
            this.Position += this.imageOffset;
            base.Render();
            this.Position = position;
        }

        public void SetOrigins(Vector2 origin)
        {
            foreach (Component component in base.Components)
            {
                Image image = component as Image;
                if (image != null)
                {
                    Vector2 vector = origin - this.Position;
                    image.Origin = image.Origin + vector - image.Position;
                    image.Position = vector;
                }
            }
        }

        private void OnCollide(Player player)
        {
            switch (this.Direction)
            {
                case Monumentspikes.Directions.Up:
                    if (player.Speed.Y >= 0f && player.Bottom <= base.Bottom)
                    {
                        player.Die(new Vector2(0f, -1f), false, true);
                        return;
                    }
                    break;
                case Monumentspikes.Directions.Down:
                    if (player.Speed.Y <= 0f)
                    {
                        player.Die(new Vector2(0f, 1f), false, true);
                        return;
                    }
                    break;
                case Monumentspikes.Directions.Left:
                    if (player.Speed.X >= 0f)
                    {
                        player.Die(new Vector2(-1f, 0f), false, true);
                        return;
                    }
                    break;
                case Monumentspikes.Directions.Right:
                    if (player.Speed.X <= 0f)
                    {
                        player.Die(new Vector2(1f, 0f), false, true);
                    }
                    break;
                default:
                    return;
            }
        }

        private static int GetSize(EntityData data, Monumentspikes.Directions dir)
        {
            if (dir > Monumentspikes.Directions.Down)
            {
                int num = dir - Monumentspikes.Directions.Left;
                return data.Height;
            }
            return data.Width;
        }

        private bool IsRiding(Solid solid)
        {
            switch (this.Direction)
            {
                case Monumentspikes.Directions.Up:
                    return base.CollideCheckOutside(solid, this.Position + Vector2.UnitY);
                case Monumentspikes.Directions.Down:
                    return base.CollideCheckOutside(solid, this.Position - Vector2.UnitY);
                case Monumentspikes.Directions.Left:
                    return base.CollideCheckOutside(solid, this.Position + Vector2.UnitX);
                case Monumentspikes.Directions.Right:
                    return base.CollideCheckOutside(solid, this.Position - Vector2.UnitX);
                default:
                    return false;
            }
        }

        private bool IsRiding(JumpThru jumpThru)
        {
            Monumentspikes.Directions direction = this.Direction;
            return direction == Monumentspikes.Directions.Up && base.CollideCheck(jumpThru, this.Position + Vector2.UnitY);
        }

        public const string TentacleType = "tentacles";

        public Monumentspikes.Directions Direction;

        private PlayerCollider pc;

        private Vector2 imageOffset;

        private int size;

        public bool monumentenable = false;
        public bool staticmoverenable = true;

        private string overrideType;

        private string spikeType;

        public Color EnabledColor;

        public Color DisabledColor;

        public enum Directions
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
