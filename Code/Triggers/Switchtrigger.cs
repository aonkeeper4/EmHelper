using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.EmHelper.triggers {
    [CustomEntity("EmHelper/Switchtrigger")]
    public class SwitchTrigger : Trigger {
        public SwitchTrigger(EntityData data, Vector2 offset)
            : base(data, offset) {
            index = data.HexColor("color", Calc.HexToColor("82d9ff"));
            oneUse = data.Bool("onetime", false);
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            this.ToggleMonumentActivators(index);
            if (oneUse) {
                RemoveSelf();
            }
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);
            this.ToggleMonumentActivators(index);
        }

        private Color index;
        private readonly bool oneUse;
    }
}
