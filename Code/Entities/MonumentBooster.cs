using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.EmHelper.Entities {
    [Tracked]
    [CustomEntity("EmHelper/Monumentbooster")]
    public class MonumentBooster : Actor {
        private readonly MonumentActivator activator;

        public MonumentBooster(EntityData data, Vector2 levelOffset)
            : this(data.Position + levelOffset, data.Int("pattern", 0), data.Bool("red", false), data.HexColor("color", Calc.HexToColor("82d9ff")), data.Bool("active", true)) {
        }

        public MonumentBooster(Vector2 position, int pattern, bool red, Color color, bool active)
            : base(position) {
            monumentOutline = new List<Image>();
            this.red = red;
            this.pattern = pattern;

            Add(activator = new MonumentActivator(color, active, (activated) => OnToggle(activated)));
        }

        public override void Awake(Scene scene) {
            SetImage();
            foreach (Image image in monumentOutline)
            {
                image.Color = activator.Index;
            }

            OnToggle(activator.Activated);
        }

        private void SetImage() {
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/monumentbooster/outline");
            monumentOutline.Add(CreateImage(atlasSubtextures[pattern % atlasSubtextures.Count]));
        }

        private Image CreateImage(MTexture tex) {
            Image image = new(tex) {
                Color = activator.Index
            };
            image.Origin.X = image.Width / 2f;
            image.Origin.Y = image.Height / 2f;
            Add(image);
            return image;
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            bubble = new Booster(Position, red);
            Scene.Add(bubble);
            bubble.Position = Position;

        }

        public void OnToggle(bool flag) {
            if (!bubble.BoostingPlayer) {
                bubble.Collidable = flag;
                bubble.Visible = flag;
                foreach (Image image in monumentOutline) {
                    image.Visible = !flag;
                }
            } else {
                // If the player is inside the bubble when we activate, delay activation
                activationQueue = true;
                queueflag = flag;
            }
        }

        public override void Update() {
            base.Update();
            if (activationQueue && !bubble.BoostingPlayer) {
                OnToggle(queueflag); //activates it later
                activationQueue = false;
                queueflag = false;
            }
        }

        private bool activationQueue = false;
        private bool queueflag = false;
        private Booster bubble;
        private readonly int pattern = 0;
        private readonly bool red = false;
        private readonly List<Image> monumentOutline;
    }
}
