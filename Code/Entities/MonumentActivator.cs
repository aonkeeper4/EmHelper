using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.EmHelper.Entities
{
    [Tracked(false)] //script that turns on and off monument entities

    public class MonumentActivator : Component
    {
        public MonumentActivator () : base (true, true)
        {

        }

        public void Activated(Color Index)
        {

            foreach (Entity entity in base.Scene.Tracker.GetEntities<Monumentswitchblock>())
            {
                Monumentswitchblock switchBlock = (Monumentswitchblock)entity;
                if (switchBlock.Index == Index)
                {
                    switchBlock.Activated = !switchBlock.Activated;

                }
            }
            foreach (Entity entity in base.Scene.Tracker.GetEntities<Monumentpressureplate>())
            {
                Monumentpressureplate pressureplate = (Monumentpressureplate)entity;
                if (pressureplate.Index == Index && pressureplate.isButton && !pressureplate.cassettedisable)
                {
                    pressureplate.buttondisable = !pressureplate.buttondisable;
                    if (pressureplate.buttondisable)
                    {
                        pressureplate.OnDisable();

                    }
                    else { pressureplate.OnEnable(); }
                }
            }

            foreach (Entity entity in base.Scene.Tracker.GetEntities<Monumentswapblock>())
            {
                Monumentswapblock swapBlock = (Monumentswapblock)entity;
                if (swapBlock.Index == Index)
                {
                    swapBlock.Activated();

                }
            }
            foreach (Entity entity in base.Scene.Tracker.GetEntities<MonumentBooster>())
            {
                MonumentBooster bubble = (MonumentBooster)entity;
                if (bubble.Index == Index)
                {
                    bubble.active = !bubble.active;
                    bubble.Activate(bubble.active);

                }
            }
            foreach (Entity entity in base.Scene.Tracker.GetEntities<Monumentflipswitch>())
            {
                Monumentflipswitch flipswitch = (Monumentflipswitch)entity;
                if (flipswitch.Index == Index)
                {
                    flipswitch.Enable = !flipswitch.Enable;
                    flipswitch.SetSprite(true);

                }
            }
            foreach (Entity entity in base.Scene.Tracker.GetEntities<Monumentspikes>())
            {
                Monumentspikes spikes = (Monumentspikes)entity;
                if (spikes.EnabledColor == Index)
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



    }
}
