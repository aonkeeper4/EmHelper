using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.EmHelper.Entities {
    [Tracked]
    public class MonumentActivator : Component {
        // Component that turns Monument entities on and off
        public MonumentActivator()
            : base(false, false) {
        }

        public void Activated(Color Index) {
            foreach (MonumentSwitchBlock switchBlock in Scene.Tracker.GetEntities<MonumentSwitchBlock>()) {
                if (switchBlock.Index == Index) {
                    switchBlock.Activated = !switchBlock.Activated;
                }
            }

            foreach (MonumentPressurePlate pressureplate in Scene.Tracker.GetEntities<MonumentPressurePlate>()) {
                if (pressureplate.Index == Index && pressureplate.IsButton && !pressureplate.CassetteDisable) {
                    pressureplate.ButtonDisable = !pressureplate.ButtonDisable;
                    if (pressureplate.ButtonDisable) {
                        pressureplate.OnDisable();
                    } else {
                        pressureplate.OnEnable();
                    }
                }
            }

            foreach (MonumentSwapBlock swapBlock in Scene.Tracker.GetEntities<MonumentSwapBlock>()) {
                if (swapBlock.Index == Index) {
                    swapBlock.Activated();

                }
            }

            foreach (MonumentBooster bubble in Scene.Tracker.GetEntities<MonumentBooster>()) {
                if (bubble.Index == Index) {
                    bubble.Activated = !bubble.Activated;
                    bubble.Activate(bubble.Activated);

                }
            }

            foreach (MonumentFlipSwitch flipswitch in Scene.Tracker.GetEntities<MonumentFlipSwitch>()) {
                if (flipswitch.Index == Index) {
                    flipswitch.Enable = !flipswitch.Enable;
                    flipswitch.SetSprite(true);

                }
            }

            foreach (MonumentSpikes spikes in Scene.Tracker.GetEntities<MonumentSpikes>()) {
                if (spikes.EnabledColor == Index) {
                    spikes.MonumentEnable = !spikes.MonumentEnable;
                    if (spikes.MonumentEnable) {
                        spikes.OnEnable();
                    } else {
                        spikes.OnDisable();
                    }
                }
            }
        }
    }
}
