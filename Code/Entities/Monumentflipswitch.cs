using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.EmHelper.Entities
{
    [Tracked]
    [CustomEntity("EmHelper/Monumentflipswitch")]
    public class Monumentflipswitch : Entity
    {
        public Monumentflipswitch(Vector2 position, bool onlyEnable, bool onlyDisable, Color color, bool mute, int pattern) : base(position)
        {
            this.pattern = pattern;
            this.mute = mute;
            this.Index = color;
            this.Enable = false;
            this.onlyEnable = onlyEnable;
            this.onlyDisable = onlyDisable;
            base.Collider = new Hitbox(16f, 24f, -8f, -12f);
            base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
            base.Add(new HoldableCollider(new Action<Holdable>(this.OnHoldable), null));
            string spritepath = "monumentflipswitch" + this.pattern.ToString();
            base.Add(this.sprite = GFX.SpriteBank.Create(spritepath));
            base.Depth = 2000;

            this.monumentactivator = new MonumentActivator();
            base.Add(this.monumentactivator);
        }

        public Monumentflipswitch(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("onlyEnable", false), data.Bool("onlyDisable", false), data.HexColor("color", Calc.HexToColor("82d9ff")), data.Bool("mute", false), data.Int("pattern", 0))
        {
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            this.sprite.Color = Index;
            this.SetSprite(false);
        }

        public void SetSprite(bool animate)
        {
            if (animate)
            {
                if (this.playSounds && !this.mute)
                {
                    Audio.Play(this.Enable ? "event:/game/09_core/switch_to_cold" : "event:/game/09_core/switch_to_hot", this.Position);
                }
                if (this.Usable)
                {
                    this.sprite.Play(this.Enable ? "ice" : "hot", false, false);
                }
                else
                {
                    if (this.playSounds && !this.mute)
                    {
                        Audio.Play("event:/game/09_core/switch_dies", this.Position);
                    }
                    this.sprite.Play(this.Enable ? "iceOff" : "hotOff", false, false);
                }
            }
            else if (this.Usable)
            {
                this.sprite.Play(this.Enable ? "iceLoop" : "hotLoop", false, false);
            }
            else
            {
                this.sprite.Play(this.Enable ? "iceOffLoop" : "hotOffLoop", false, false);
            }
            this.playSounds = false;
        }

        private void OnPlayer(Player player) //copied the vanilla one... sigh
        {
            if (this.Usable && this.cooldownTimer <= 0f)
            {
                this.playSounds = true;
                this.monumentactivator.Activated(this.Index);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                this.cooldownTimer = 1f;
            }
        }
        private void OnHoldable(Holdable holdable)
        {
            if (this.Usable && this.cooldownTimer <= 0f)
            {
                this.playSounds = true;
                this.monumentactivator.Activated(this.Index);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                this.cooldownTimer = 1f;
            }
        }
        
        public override void Update()
        {
            base.Update();
            if (this.cooldownTimer > 0f)
            {
                this.cooldownTimer -= Engine.DeltaTime;
            }
        }

        private bool Usable
        {
            get
            {
                return (!this.onlyEnable || this.Enable) && (!this.onlyDisable || !this.Enable);
            }
        }

        private const float Cooldown = 1f;


        private float cooldownTimer;

        private bool mute; //stfu

        private bool onlyEnable; //only one way


        private bool onlyDisable; //only other way


        private bool playSounds;

        public Color Index; //the color

        public bool Enable;  //icemode, it can also activate on disable

        private Sprite sprite;

        private int pattern; //only ahestetic wise

        private MonumentActivator monumentactivator;
    }
}
