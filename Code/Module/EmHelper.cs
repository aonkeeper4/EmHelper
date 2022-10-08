using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Reflection;
using Monocle;
using MonoMod.Utils;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Celeste;
using Celeste.Mod;
using MonoMod.RuntimeDetour;


namespace Celeste.Mod.EmHelper
{
    public class EmHelper : EverestModule
    {

        // Only one alive module instance can exist at any given time.
        public static EmHelper Instance;
        private ILHook PlayerOrig_updateHook; //IL hook, used to modify the player's code so walkelines can interact with triggers

        public EmHelper()
        {
            Instance = this;
        }

        // Set up any hooks, event handlers and your mod in general here.
        // Load runs before Celeste itself has initialized properly.
        public override void Load()
        {
            On.Celeste.LevelLoader.ctor += LevelLoader_ctor;
            On.Monocle.Collider.Collide_Collider += Collider_Collide_Collider;
            On.Celeste.CassetteBlock.BlockedCheck += Walkeline_BlockedCheck;
            PlayerOrig_updateHook = new ILHook(typeof(Player).GetMethod("orig_Update"), Walkeline_triggerupdate); //i want to swallow burning coals, used to let walkelines interact with triggers, have no idea how it works anymore
        }

        private void Walkeline_triggerupdate(ILContext il) {
            ILCursor cursor = new ILCursor(il);

            if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdarg(0), instr => instr.MatchLdloc(10), instr => instr.MatchCall<Entity>("CollideCheck"))) //go to (base.CollideCheck(trigger) instr => instr.MatchLdloc(0), instr => instr.MatchLdloc(10), 
            {
                Logger.Log("EmHelper/EmHelper.cs", $"Modding Player at {cursor.Index} in IL code for Player.Update()");
                cursor.Emit(OpCodes.Ldarg_0); //adds the player to the stack, thanks max!
                cursor.Emit(OpCodes.Ldloc_S, il.Body.Variables[10]); //adds the trigger to the stack, thanks again max!
                cursor.EmitDelegate<Func<bool, Player, Trigger, bool>>(replaceplayertriggercollision);
            }
        }
        private bool replaceplayertriggercollision(bool collision, Player player, Trigger trigger) { //added after the vanilla's player/trigger and returns it || a walkeline/trigger collision
            return collision || checkwalkelinesinsidetrigger(player, trigger);
            
        }
        private bool checkwalkelinesinsidetrigger(Player player, Trigger trigger)
        {
            if (trigger != null) //never too sure
            {
                foreach (Entity entity in player.Scene.Tracker.GetEntities<Entities.Walkeline>()) //i could swap the player with the trigger, but im not sure that will work
                {
                    Entities.Walkeline walkeline = (Entities.Walkeline)entity;
                    if (walkeline != null && trigger.CollideCheck(walkeline) && walkeline.triggerhappy)
                    { //triggerhappy is just a flag in the walke's code to check if it should collide with triggers
                        return true; //there's at least a walkeline inside the trigger
                    }
                }
            }
            return false; //no walkelines inside the trigger

        }

        private void LevelLoader_ctor(On.Celeste.LevelLoader.orig_ctor orig, LevelLoader self, Session session, Vector2? startPosition)
        {
            orig(self, session, startPosition);
            PlayerSprite.CreateFramesMetadata("walkeline");
            PlayerSprite.CreateFramesMetadata("windupwalkeline");


        }

        private static MethodInfo TryActorWiggleUp = typeof(CassetteBlock).GetMethod("TryActorWiggleUp", BindingFlags.Instance | BindingFlags.NonPublic);
        private bool Walkeline_BlockedCheck(On.Celeste.CassetteBlock.orig_BlockedCheck orig, CassetteBlock self)
        {
            Entities.Walkeline walkeline = self.CollideFirst<Entities.Walkeline>();

            if (walkeline != null && !(bool)TryActorWiggleUp.Invoke(self, new object[] { walkeline }))
            {
                return true;
            }
            return orig(self);
        }
        private bool Collider_Collide_Collider(On.Monocle.Collider.orig_Collide_Collider orig, Collider self, Collider collider) //empty collider to stop the player to pick up walkelines
        {
            if (collider is Entities.NoCollider)
                return false;
            return orig(self, collider);
        }



        public override void Unload()
        {
            On.Celeste.LevelLoader.ctor -= LevelLoader_ctor;
            On.Monocle.Collider.Collide_Collider -= Collider_Collide_Collider;
            On.Celeste.CassetteBlock.BlockedCheck -= Walkeline_BlockedCheck;
            if (PlayerOrig_updateHook != null) PlayerOrig_updateHook.Dispose();
        }

    }
}

