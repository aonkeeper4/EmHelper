using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.EmHelper.Entities {
    [Tracked]
    public class MonumentActivator : Component {
        public Color Index;
        public bool Activated;
        public Action<bool> OnToggle;

        public MonumentActivator(Color index, bool startActive = false, Action<bool> onToggle = null)
            : base(false, false) {
            Index = index;
            Activated = startActive;
            OnToggle = onToggle;
        }

        public void Toggle(Color index) {
            if (index == Index) {
                Activated = !Activated;
                OnToggle?.Invoke(Activated);
            }
        }
    }
}
