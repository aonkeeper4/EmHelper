using Celeste.Mod.EmHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.EmHelper {
    public static class Utils {
        public static void ToggleMonumentActivators(this Entity entity, Color index) {
            foreach (MonumentActivator activator in entity.Scene.Tracker.GetComponents<MonumentActivator>()) {
                activator.Toggle(index);
            }
        }
    }
}
