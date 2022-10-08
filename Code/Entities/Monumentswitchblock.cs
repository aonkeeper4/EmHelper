using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.EmHelper.Entities
{
    [Tracked]
    [CustomEntity("EmHelper/Monumentswitchblock")]
    public class Monumentswitchblock : Solid //im pretty sure i murdered the cassette's code and frankensteined it back together
    {

        public Monumentswitchblock(Vector2 position, EntityID id, float width, float height, int pattern, bool active, Color color) : base(position, width, height, false)
        {
            this.pattern = pattern;
            this.blockHeight = 2;
            this.pressed = new List<Image>();
            this.solid = new List<Image>();
            this.all = new List<Image>();
            this.SurfaceSoundIndex = 35;
            this.Index = color;
            this.Activated = active;
            this.ID = id;
            base.Add(this.occluder = new LightOcclude(1f));
        }


        public Monumentswitchblock(EntityData data, Vector2 offset, EntityID id) : this(data.Position + offset, id, (float)data.Width, (float)data.Height, data.Int("pattern", 0), data.Bool("active", false), data.HexColor("color", Calc.HexToColor("82d9ff")))
        {
        }


        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Color color = Calc.HexToColor("667da5");
            Color disabledColor = new Color((float)color.R / 255f * ((float)this.Index.R / 255f), (float)color.G / 255f * ((float)this.Index.G / 255f), (float)color.B / 255f * ((float)this.Index.B / 255f), 1f);
            scene.Add(this.side = new Monumentswitchblock.BoxSide(this, disabledColor));
            foreach (StaticMover staticMover in this.staticMovers)
            {
                Spikes spikes = staticMover.Entity as Spikes;
                if (spikes != null)
                {
                    spikes.EnabledColor = this.Index;
                    spikes.DisabledColor = disabledColor;
                    spikes.VisibleWhenDisabled = true;
                    spikes.SetSpikeColor(this.Index);
                }
                Spring spring = staticMover.Entity as Spring;
                if (spring != null)
                {
                    spring.DisabledColor = disabledColor;
                    spring.VisibleWhenDisabled = true;
                }
                Monumentspikes monumentspikes = staticMover.Entity as Monumentspikes;
                if (monumentspikes != null)
                {
                    if (monumentspikes.EnabledColor == this.Index)
                    {
                        monumentspikes.monumentenable = this.Activated; //it fixes a stupid interaction with a active spike on a deactivated switchblock

                    }
                }
            }
            if (this.group == null)
            {
                this.groupLeader = true;
                this.group = new List<Monumentswitchblock>();
                this.group.Add(this);
                this.FindInGroup(this);
                float num = float.MaxValue;
                float num2 = float.MinValue;
                float num3 = float.MaxValue;
                float num4 = float.MinValue;
                foreach (Monumentswitchblock switchblock in this.group)
                {
                    if (switchblock.Left < num)
                    {
                        num = switchblock.Left;
                    }
                    if (switchblock.Right > num2)
                    {
                        num2 = switchblock.Right;
                    }
                    if (switchblock.Bottom > num4)
                    {
                        num4 = switchblock.Bottom;
                    }
                    if (switchblock.Top < num3)
                    {
                        num3 = switchblock.Top;
                    }
                }
                this.groupOrigin = new Vector2((float)((int)(num + (num2 - num) / 2f)), (float)((int)num4));
                this.wigglerScaler = new Vector2(Calc.ClampedMap(num2 - num, 32f, 96f, 1f, 0.2f), Calc.ClampedMap(num4 - num3, 32f, 96f, 1f, 0.2f));
                base.Add(this.wiggler = Wiggler.Create(0.3f, 3f, null, false, false));
                foreach (Monumentswitchblock switchblock2 in this.group)
                {
                    switchblock2.wiggler = this.wiggler;
                    switchblock2.wigglerScaler = this.wigglerScaler;
                    switchblock2.groupOrigin = this.groupOrigin;
                }
            }
            foreach (StaticMover staticMover2 in this.staticMovers)
            {
                Spikes spikes2 = staticMover2.Entity as Spikes;
                if (spikes2 != null)
                {
                    spikes2.SetOrigins(this.groupOrigin);
                }
            }
            for (float num5 = base.Left; num5 < base.Right; num5 += 8f)
            {
                for (float num6 = base.Top; num6 < base.Bottom; num6 += 8f)
                {
                    bool flag = this.CheckForSame(num5 - 8f, num6);
                    bool flag2 = this.CheckForSame(num5 + 8f, num6);
                    bool flag3 = this.CheckForSame(num5, num6 - 8f);
                    bool flag4 = this.CheckForSame(num5, num6 + 8f);
                    if (flag && flag2 && flag3 && flag4)
                    {
                        if (!this.CheckForSame(num5 + 8f, num6 - 8f))
                        {
                            this.SetImage(num5, num6, 3, 0);
                        }
                        else if (!this.CheckForSame(num5 - 8f, num6 - 8f))
                        {
                            this.SetImage(num5, num6, 3, 1);
                        }
                        else if (!this.CheckForSame(num5 + 8f, num6 + 8f))
                        {
                            this.SetImage(num5, num6, 3, 2);
                        }
                        else if (!this.CheckForSame(num5 - 8f, num6 + 8f))
                        {
                            this.SetImage(num5, num6, 3, 3);
                        }
                        else
                        {
                            this.SetImage(num5, num6, 1, 1);
                        }
                    }
                    else if (flag && flag2 && !flag3 && flag4)
                    {
                        this.SetImage(num5, num6, 1, 0);
                    }
                    else if (flag && flag2 && flag3 && !flag4)
                    {
                        this.SetImage(num5, num6, 1, 2);
                    }
                    else if (flag && !flag2 && flag3 && flag4)
                    {
                        this.SetImage(num5, num6, 2, 1);
                    }
                    else if (!flag && flag2 && flag3 && flag4)
                    {
                        this.SetImage(num5, num6, 0, 1);
                    }
                    else if (flag && !flag2 && !flag3 && flag4)
                    {
                        this.SetImage(num5, num6, 2, 0);
                    }
                    else if (!flag && flag2 && !flag3 && flag4)
                    {
                        this.SetImage(num5, num6, 0, 0);
                    }
                    else if (flag && !flag2 && flag3 && !flag4)
                    {
                        this.SetImage(num5, num6, 2, 2);
                    }
                    else if (!flag && flag2 && flag3 && !flag4)
                    {
                        this.SetImage(num5, num6, 0, 2);
                    }
                }
            }
            this.UpdateVisualState();
        }


        private void FindInGroup(Monumentswitchblock block)
        {
            foreach (Entity entity in base.Scene.Tracker.GetEntities<Monumentswitchblock>())
            {
                Monumentswitchblock switchblock = (Monumentswitchblock)entity;
                if (switchblock != this && switchblock != block && switchblock.Index == this.Index && (switchblock.CollideRect(new Rectangle((int)block.X - 1, (int)block.Y, (int)block.Width + 2, (int)block.Height)) || switchblock.CollideRect(new Rectangle((int)block.X, (int)block.Y - 1, (int)block.Width, (int)block.Height + 2))) && !this.group.Contains(switchblock))
                {
                    this.group.Add(switchblock);
                    this.FindInGroup(switchblock);
                    switchblock.group = this.group;
                }
            }
        }


        private bool CheckForSame(float x, float y)
        {
            foreach (Entity entity in base.Scene.Tracker.GetEntities<Monumentswitchblock>())
            {
                Monumentswitchblock switchblock = (Monumentswitchblock)entity;
                if (switchblock.Index == this.Index && switchblock.Collider.Collide(new Rectangle((int)x, (int)y, 8, 8)))
                {
                    return true;
                }
            }
            return false;
        }


        private void SetImage(float x, float y, int tx, int ty)
        {
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/monumentswitchblock/pressed");
            this.pressed.Add(this.CreateImage(x, y, tx, ty, atlasSubtextures[this.pattern % atlasSubtextures.Count]));
            atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/monumentswitchblock/solid");
            this.solid.Add(this.CreateImage(x, y, tx, ty, atlasSubtextures[this.pattern % atlasSubtextures.Count]));
        }


        private Image CreateImage(float x, float y, int tx, int ty, MTexture tex)
        {
            Vector2 value = new Vector2(x - base.X, y - base.Y);
            Image image = new Image(tex.GetSubtexture(tx * 8, ty * 8, 8, 8, null));
            Vector2 vector = this.groupOrigin - this.Position;
            image.Origin = vector - value;
            image.Position = vector;
            image.Color = this.Index;
            base.Add(image);
            this.all.Add(image);
            return image;
        }


        public override void Update()
        {
            base.Update();
            if (this.Activated && !this.Collidable)
            {
                bool flag = false;
                if (BlockedCheck())
                {
                    flag = true;
                }

                if (!flag)
                {

                    this.Collidable = true;
                    this.ShiftSize(-1);
                    this.EnableStaticMovers();
                    if (this.groupLeader)
                    {
                        this.wiggler.Start();
                    }

                }
            }
            else if (!this.Activated && this.Collidable)
            {
                this.Collidable = false;
                this.ShiftSize(1);
                this.DisableStaticMovers();

            }
            this.UpdateVisualState();
        }

        public bool BlockedCheck()
        {
            TheoCrystal theoCrystal = base.CollideFirst<TheoCrystal>();
            if (theoCrystal != null && !this.TryActorWiggleUp(theoCrystal))
            {
                return true;
            }
            Walkeline walkeline = base.CollideFirst<Walkeline>();
            if (walkeline != null && !this.TryActorWiggleUp(walkeline))
            {
                return true;
            }
            Player player = base.CollideFirst<Player>();
            return player != null && !this.TryActorWiggleUp(player);
        }


        private void UpdateVisualState()
        {
            if (!this.Collidable)
            {
                base.Depth = 8990;
            }
            else
            {
                Player entity = base.Scene.Tracker.GetEntity<Player>();
                if (entity != null && entity.Top >= base.Bottom - 1f)
                {
                    base.Depth = 10;
                }
                else
                {
                    base.Depth = -10;
                }
            }
            foreach (StaticMover staticMover in this.staticMovers)
            {
                staticMover.Entity.Depth = base.Depth + 1;
            }
            this.side.Depth = base.Depth + 5;
            this.side.Visible = (this.blockHeight > 0);
            this.occluder.Visible = this.Collidable;
            foreach (Image image in this.solid)
            {
                image.Visible = this.Collidable;
            }
            foreach (Image image2 in this.pressed)
            {
                image2.Visible = !this.Collidable;
            }
            if (this.groupLeader)
            {
                Vector2 scale = new Vector2(1f + this.wiggler.Value * 0.05f * this.wigglerScaler.X, 1f + this.wiggler.Value * 0.15f * this.wigglerScaler.Y);
                foreach (Monumentswitchblock switchblock in this.group)
                {
                    foreach (Image image3 in switchblock.all)
                    {
                        image3.Scale = scale;
                    }
                    foreach (StaticMover staticMover2 in switchblock.staticMovers)
                    {
                        Spikes spikes = staticMover2.Entity as Spikes;
                        if (spikes != null)
                        {
                            foreach (Monocle.Component component in spikes.Components)
                            {
                                Image image4 = component as Image;
                                if (image4 != null)
                                {
                                    image4.Scale = scale;
                                }
                            }
                        }
                    }
                }
            }
        }


        public void SetActivatedSilently(bool activated)
        {
            this.Collidable = activated;
            this.Activated = activated;
            this.UpdateVisualState();
            if (activated)
            {
                base.EnableStaticMovers();
                return;
            }
            this.ShiftSize(2);
            base.DisableStaticMovers();
        }

        public void Finish()
        {
            this.Activated = false;
        }


        public void WillToggle()
        {
            this.ShiftSize(this.Collidable ? 1 : -1);
            this.UpdateVisualState();
        }


        private void ShiftSize(int amount)
        {
            base.MoveV((float)amount);
            this.blockHeight -= amount;
        }


        private bool TryActorWiggleUp(Entity actor)
        {
            foreach (Monumentswitchblock switchblock in this.group)
            {
                if (switchblock != this && switchblock.CollideCheck(actor, switchblock.Position + Vector2.UnitY * 4f))
                {
                    return false;
                }
            }
            bool collidable = this.Collidable;
            this.Collidable = true;
            for (int i = 1; i <= 4; i++)
            {
                if (!actor.CollideCheck<Solid>(actor.Position - Vector2.UnitY * (float)i))
                {
                    actor.Position -= Vector2.UnitY * (float)i;
                    this.Collidable = collidable;
                    return true;
                }
            }
            this.Collidable = collidable;
            return false;
        }


        public Monumentswitchblock(EntityData data, Vector2 offset) : this(data, offset, new EntityID(data.Level.Name, data.ID))
        {
        }


        public Color Index;


        public bool Activated;


        public Monumentswitchblock.Modes Mode;


        public EntityID ID;


        private int blockHeight;


        private List<Monumentswitchblock> group;

        private int pattern;

        private bool groupLeader;


        private Vector2 groupOrigin;


        private List<Image> pressed;


        private List<Image> solid;


        private List<Image> all;


        private LightOcclude occluder;


        private Wiggler wiggler;


        private Vector2 wigglerScaler;


        private Monumentswitchblock.BoxSide side;


        public enum Modes
        {

            Solid,

            Leaving,

            Disabled,

            Returning
        }


        private class BoxSide : Entity
        {

            public BoxSide(Monumentswitchblock block, Color color)
            {
                this.block = block;
                this.color = color;
            }


            public override void Render()
            {
                Draw.Rect(this.block.X, this.block.Y + this.block.Height - 8f, this.block.Width, (float)(8 + this.block.blockHeight), this.color);
            }


            private Monumentswitchblock block;


            private Color color;
        }
    }
}

