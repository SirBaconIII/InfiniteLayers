using MelonLoader;
using HarmonyLib;
using System.Reflection;
using InfiniteLayers;

[assembly: MelonInfo(typeof(InfiniteLayersMain), "Infinite Layers", "1.0.1", "SirBaconIII")]
[assembly: MelonGame("tobspr Games", "shapez 2")]
namespace InfiniteLayers
{
    public class InfiniteLayersMain : MelonMod
    {
        public static int layerNum = 100;
        
        public override void OnInitializeMelon()
        {            
            HarmonyLib.Harmony harmony = this.HarmonyInstance;

            MethodInfo getMaximumAllowedLayer = typeof(BaseMapInteractionMode).GetMethod("GetMaximumAllowedLayer");
            MethodInfo getMaximumAllowedLayerPrefix = typeof(BaseMapInteractionModePatch).GetMethod("GetMaximumAllowedLayer_Prefix");

            MethodInfo OnEnable = typeof(MetaGameMode).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo OnEnablePostfix = typeof(MetaGameModePatch).GetMethod("OnEnable_Postfix");

            MethodInfo getOverviewContentTint = typeof(OverviewRenderMode).GetMethod("GetOverviewContentTint", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo getOverviewContentTintPrefix = typeof(OverviewRenderModePatch).GetMethod("GetOverviewContentTint_Prefix");

            harmony.Patch(getMaximumAllowedLayer, prefix: new HarmonyMethod(getMaximumAllowedLayerPrefix));
            harmony.Patch(OnEnable, postfix: new HarmonyMethod(OnEnablePostfix));

            harmony.Patch(getOverviewContentTint, prefix: new HarmonyMethod(getOverviewContentTintPrefix));
        }
    }
}
