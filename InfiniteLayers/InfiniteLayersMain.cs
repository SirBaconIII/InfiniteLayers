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

            //These two allow you to reach and place in higher layers
            MethodInfo getMaximumAllowedLayer = typeof(BaseMapInteractionMode).GetMethod("GetMaximumAllowedLayer");
            MethodInfo getMaximumAllowedLayerPrefix = typeof(BaseMapInteractionModePatch).GetMethod("GetMaximumAllowedLayer_Prefix");

            MethodInfo OnEnable = typeof(MetaGameMode).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo OnEnablePostfix = typeof(MetaGameModePatch).GetMethod("OnEnable_Postfix");

            //These draw the meshes
            MethodInfo drawStatic_BeltEndCaps = typeof(MapEntity).GetMethod("DrawStatic_BeltEndCaps", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo drawStatic_BeltEndCapsPrefix = typeof(MapEntityPatch).GetMethod("DrawStatic_BeltEndCaps_Prefix");

            MethodInfo drawStatic_BaseMesh = typeof(MapEntity).GetMethod("DrawStatic_BaseMesh", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo drawStatic_BaseMeshPrefix = typeof(MapEntityPatch).GetMethod("DrawStatic_BaseMesh_Prefix");

            MethodInfo drawStatic_FluidEndCapsAndStands = typeof(MapEntity).GetMethod("DrawStatic_FluidEndCapsAndStands", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo drawStatic_FluidEndCapsAndStandsPrefix = typeof(MapEntityPatch).GetMethod("DrawStatic_FluidEndCapsAndStands_Prefix");

            MethodInfo drawStatic_EndCaps_BeltPortReceiverEntity = typeof(BeltPortReceiverEntity).GetMethod("DrawStatic_EndCaps", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo drawStatic_EndCaps_BeltPortSenderEntity = typeof(BeltPortSenderEntity).GetMethod("DrawStatic_EndCaps", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo drawStatic_EndCaps_BeltPortEntityPrefix = typeof(BeltPortEntityPatch).GetMethod("DrawStatic_EndCaps_BeltPortEntity_Prefix");

            //Whatever this is
            MethodInfo getOverviewContentTint = typeof(OverviewRenderMode).GetMethod("GetOverviewContentTint", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo getOverviewContentTintPrefix = typeof(OverviewRenderModePatch).GetMethod("GetOverviewContentTint_Prefix");

            harmony.Patch(getMaximumAllowedLayer, prefix: new HarmonyMethod(getMaximumAllowedLayerPrefix));
            harmony.Patch(OnEnable, postfix: new HarmonyMethod(OnEnablePostfix));

            harmony.Patch(drawStatic_BaseMesh, prefix: new HarmonyMethod(drawStatic_BaseMeshPrefix));
            harmony.Patch(drawStatic_BeltEndCaps, prefix: new HarmonyMethod(drawStatic_BeltEndCapsPrefix));
            harmony.Patch(drawStatic_FluidEndCapsAndStands, prefix: new HarmonyMethod(drawStatic_FluidEndCapsAndStandsPrefix));
            harmony.Patch(drawStatic_EndCaps_BeltPortReceiverEntity, prefix: new HarmonyMethod(drawStatic_EndCaps_BeltPortEntityPrefix));
            harmony.Patch(drawStatic_EndCaps_BeltPortSenderEntity, prefix: new HarmonyMethod(drawStatic_EndCaps_BeltPortEntityPrefix));

            harmony.Patch(getOverviewContentTint, prefix: new HarmonyMethod(getOverviewContentTintPrefix));
        }
    }
}
