using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.EmHelper.triggers
{
    [CustomEntity("EmHelper/Switchtrigger")]
    public class Switchtrigger : Trigger
    {

        public Switchtrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            this.Index = data.HexColor("color", Calc.HexToColor("82d9ff"));
            this.onetime = data.Bool("onetime", false);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            Activate();
            if (onetime)
            {
                base.RemoveSelf();
            }
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            Activate();
            if (onetime)
            {
                base.RemoveSelf();
            }
        }

        public void Activate()
        {
            foreach (Entity entity in base.Scene.Tracker.GetEntities<Entities.Monumentswitchblock>())
            {
                Entities.Monumentswitchblock switchBlock = (Entities.Monumentswitchblock)entity;
                if (switchBlock.Index == this.Index)
                {
                    switchBlock.Activated = !switchBlock.Activated;

                }
            }
            foreach (Entity entity in base.Scene.Tracker.GetEntities<Entities.MonumentBooster>())
            {
                Entities.MonumentBooster bubble = (Entities.MonumentBooster)entity;
                if (bubble.Index == this.Index)
                {
                    bubble.active = !bubble.active;
                    bubble.Activate(bubble.active);

                }
            }
            foreach (Entity entity in base.Scene.Tracker.GetEntities<Entities.Monumentpressureplate>())
            {
                Entities.Monumentpressureplate pressureplate = (Entities.Monumentpressureplate)entity;
                if (pressureplate.Index == this.Index && pressureplate.isButton && !pressureplate.cassettedisable)
                {
                    pressureplate.buttondisable = !pressureplate.buttondisable;
                    if (pressureplate.buttondisable)
                    {
                        pressureplate.OnDisable();

                    }
                    else { pressureplate.OnEnable(); }
                }
            }
            foreach (Entity entity in base.Scene.Tracker.GetEntities<Entities.Monumentswapblock>())
            {
                Entities.Monumentswapblock swapBlock = (Entities.Monumentswapblock)entity;
                if (swapBlock.Index == this.Index)
                {
                    swapBlock.Activated();

                }
            }
            foreach (Entity entity in base.Scene.Tracker.GetEntities<Entities.Monumentflipswitch>())
            {
                Entities.Monumentflipswitch flipswitch = (Entities.Monumentflipswitch)entity;
                if (flipswitch.Index == this.Index)
                {
                    flipswitch.Enable = !flipswitch.Enable;
                    flipswitch.SetSprite(true);

                }
            }
            foreach (Entity entity in base.Scene.Tracker.GetEntities<Entities.Monumentspikes>())
            {
                Entities.Monumentspikes spikes = (Entities.Monumentspikes)entity;
                if (spikes.EnabledColor == this.Index)
                {
                    spikes.monumentenable = !spikes.monumentenable;
                    if (spikes.monumentenable)
                    {
                        spikes.OnEnable();
                    }
                    else { spikes.OnDisable(); }

                }
            }

        }
        private Color Index; //color to activate
        private bool onetime;



    }
}

