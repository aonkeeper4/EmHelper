using MonoMod.ModInterop;
using System;

// ReSharper disable UnassignedField.Global

namespace Celeste.Mod.EmHelper.Module {
    /// <summary>
    /// Import methods defined <see href="https://github.com/swoolcock/GravityHelper/blob/develop/GravityHelperExports.cs">GravityHelperExports</see>.
    /// </summary>
    [ModImportName("GravityHelper")]
    public static class GravityHelperImports {
        public static Func<bool> IsPlayerInverted;
    }
}