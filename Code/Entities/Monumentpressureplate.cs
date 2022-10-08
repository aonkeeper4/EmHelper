using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EmHelper.Entities
{
    [Tracked]
    [CustomEntity("EmHelper/Monumentpressureplate")]
    class Monumentpressureplate : Entity
    {
        public Monumentpressureplate(EntityData data, Vector2 levelOffset) : this(data.Position + levelOffset, data.Int("pattern", 0), data.Bool("onetime", false), data.HexColor("color", Calc.HexToColor("82d9ff")), data.Bool("mute", false), data.Bool("isButton", false), data.Bool("disable", false)) { }
        public Monumentpressureplate(Vector2 position, int pattern, bool onetime, Color color, bool mute, bool isButton, bool ButtonDisable) : base(position)
        {

            this.isButton = isButton;
            this.buttondisable = ButtonDisable; 
            Console.WriteLine(buttondisable);
            this.mute = mute;
            base.Depth = -59;
            this.onetime = onetime;
            this.pattern = pattern;
            this.pressed = new List<Image>();
            this.solid = new List<Image>();
            this.all = new List<Image>();
            this.Index = color;
            base.Collider = new Hitbox(14f, 3f, -7f, -3f);

            this.staticMover = new StaticMover();
            this.staticMover.OnAttach = delegate (Platform p) { base.Depth = p.Depth + 1; };
            this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position + Vector2.UnitY));
            this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position + Vector2.UnitY));
            base.Add(this.staticMover);
            this.staticMover.OnEnable = new Action(this.OnStaticMoverEnable);
            this.staticMover.OnDisable = new Action(this.OnStaticMoverDisable);

            this.monumentactivator = new MonumentActivator();
            base.Add(this.monumentactivator);
        }

        private void OnStaticMoverEnable()
        {
            this.cassettedisable = false;
            if (this.buttondisable)
            {
                OnDisable();

            }
            else { OnEnable(); }

        }
        private void OnStaticMoverDisable()
        {
            this.cassettedisable = true;
            OnDisable();
        }
        public void OnEnable()
        {
            foreach (Image image in this.solid) //normal color
            {
                image.Color = (Index);
            }
            this.Collidable = true;
            this.pressureactivated = false;
            this.prevstate = this.pressureactivated;
            UpdateVisualState();

        }

        public void OnDisable()
        {
            foreach (Image image in this.solid) //normal color
            {
                image.Color = (disabledcolor);
            }
            this.Collidable = false;
            if (this.buttondisable)
            {
                UpdateVisualState(true);
            }
            else { UpdateVisualState(); }

        }


        
        public void trigger()
        {
            if (this.onetime) { this.turnoff = true; } //onetime button
            this.prevstate = this.pressureactivated;
            if (this.pressureactivated && !this.mute) { Audio.Play("event:/game/general/cassette_block_switch_1"); } else if (!this.mute) { Audio.Play("event:/game/general/cassette_block_switch_2"); }
            UpdateVisualState();
            this.monumentactivator.Activated(this.Index);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Color color = Calc.HexToColor("667da5");
            disabledcolor = new Color((float)color.R / 255f * ((float)this.Index.R / 255f), (float)color.G / 255f * ((float)this.Index.G / 255f), (float)color.B / 255f * ((float)this.Index.B / 255f), 1f);
            this.SetImage();
            foreach (Image image in this.solid) //normal color
            {
                image.Color = (Index);
            }
            foreach (Image image2 in this.pressed) //disabled
            {
                image2.Color = (disabledcolor);
            }
            if (this.buttondisable)
            {
                UpdateVisualState(true);
            }
            else { UpdateVisualState(); }
        }

        public override void Update()
        {
            base.Update();
            if (this.turnoff || this.cassettedisable || this.buttondisable) { return; }

            List<Entity> actors = base.Scene.Tracker.GetEntities<Actor>();
            this.pressureactivated = false;
            foreach (Entity actor in actors)
            {

                bool flag = Collide.Check(actor, this);
                if (flag)
                {
                    this.pressureactivated = true;
                    break;
                }
            }

            if (prevstate != pressureactivated)
            {
                trigger();

            }
        }

        private void SetImage()
        {
            this.pressed.Add(this.CreateImage(GFX.Game["objects/monumentpressureplate/pressed"]));
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/monumentpressureplate/solid");
            this.solid.Add(this.CreateImage(atlasSubtextures[this.pattern % atlasSubtextures.Count]));
        }

        private Image CreateImage(MTexture tex)
        {
            Image image = new Image(tex);
            image.Color = this.Index;
            image.Origin.X = image.Width / 2f;
            image.Origin.Y = image.Height - 1;
            base.Add(image);
            this.all.Add(image);
            return image;
        }

        private void UpdateVisualState()
        {

            foreach (Image image in this.solid)
            {

                if (this.turnoff) { image.Visible = false; }
                else { image.Visible = !this.pressureactivated; }



            }
            foreach (Image image2 in this.pressed)
            {
                if (this.turnoff) { image2.Visible = true; }
                else { image2.Visible = this.pressureactivated; }

            }
        }

        private void UpdateVisualState(bool showpressed) //same method but pressed when it's deactivated (used for buttons)
        {

            foreach (Image image in this.solid)
            {
                image.Visible = !showpressed;

            }
            foreach (Image image2 in this.pressed)
            {
                image2.Visible = showpressed;

            }
        }

        public void SetColor(Color color)
        {
            foreach (Monocle.Component component in base.Components)
            {
                Image image = component as Image;
                if (image != null)
                {
                    image.Color = color;
                }
            }
        }

        public bool buttondisable = false; // useful if it's a button
        public bool isButton = false;
        public bool cassettedisable = false;
        private bool turnoff = false;
        private bool mute = false;
        private int pattern;
        private bool onetime = false;
        public bool pressureactivated = false; //pressure plate only
        public bool prevstate = false; //pressure plate stuff
        public Color Index;
        private Color disabledcolor;
        private List<Image> pressed;
        private List<Image> solid;
        private List<Image> all;
        private StaticMover staticMover;

        private MonumentActivator monumentactivator;
    }
}
