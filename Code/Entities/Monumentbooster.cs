using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EmHelper.Entities
{
    [Tracked]
    [CustomEntity("EmHelper/Monumentbooster")]
    public class MonumentBooster : Actor
    {
        public MonumentBooster(EntityData data, Vector2 levelOffset) : this(data.Position + levelOffset, data.Int("pattern", 0), data.Bool("red", false), data.HexColor("color", Calc.HexToColor("82d9ff")), data.Bool("active", true)) { }
        public MonumentBooster(Vector2 position, int pattern, bool red, Color color, bool active) : base(position)
        {
            this.monumentoutline = new List<Image>();
            this.red = red;
            this.pattern = pattern;
            this.Index = color;
            this.active = active;
        }

        public override void Awake(Scene scene)
        {
            this.SetImage();
            foreach (Image image in this.monumentoutline) //normal color
            {
                image.Color = (Index);
            }
            Activate(this.active);
        }

        private void SetImage()
        {
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/monumentbooster/outline");
            this.monumentoutline.Add(this.CreateImage(atlasSubtextures[this.pattern % atlasSubtextures.Count]));
        }

        private Image CreateImage(MTexture tex)
        {
            Image image = new Image(tex);
            image.Color = this.Index;
            image.Origin.X = image.Width / 2f;
            image.Origin.Y = image.Height / 2f;
            base.Add(image);
            return image;
        }


        //added to scene
        public override void Added(Scene scene)
        {
            base.Added(scene);
            this.bubble = new Booster(Position, red);
            Scene.Add(this.bubble);
            this.bubble.Position = this.Position;

        }

        public void Activate(bool flag)
        {

            if (!this.bubble.BoostingPlayer)
            {
                bubble.Collidable = flag;
                bubble.Visible = flag;
                foreach (Image image in this.monumentoutline)
                {
                    image.Visible = !flag;

                }
            }
            else //if the player is inside the booster when activating the color
            {
                activationqueue = true;
                queueflag = flag;
            }

        }



        public override void Update()
        {
            base.Update();
            if (activationqueue && !this.bubble.BoostingPlayer)
            {
                Activate(queueflag); //activates it later
                activationqueue = false;
                queueflag = false;
            }

        }

        private bool activationqueue = false;
        private bool queueflag = false;
        public Color Index;
        private Booster bubble;
        private int pattern = 0;
        private bool red = false;
        public bool active;
        private List<Image> monumentoutline;
    }
}
