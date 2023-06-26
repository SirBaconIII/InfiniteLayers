using System.Reflection;
using Unity.Mathematics;
using static MapEntity;

public static class BeltPortEntityPatch
{
    public static bool DrawStatic_EndCaps_BeltPortEntity_Prefix(MapEntity __instance, MeshBuilder builder)
    {
        DrawStatic_EndCaps_OGCopy(__instance, builder);
        LOD2Mesh mesh = __instance.InternalVariant.SupportMeshesInternalLOD[1 + (__instance.Tile_I.z % 2)];
        float3 tile_L = new float3(0);
        float3 position = __instance.W_From_L(in tile_L);
        builder.AddTranslateRotate(mesh, in position, __instance.Rotation_G);

        return false;
    }

    private static void DrawStatic_EndCaps_OGCopy(MapEntity __instance, MeshBuilder builder)
    {
        MethodInfo drawStatic_BeltEndCaps = typeof(MapEntity).GetMethod("DrawStatic_BeltEndCaps", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo drawStatic_FluidEndCapsAndStands = typeof(MapEntity).GetMethod("DrawStatic_FluidEndCapsAndStands", BindingFlags.Instance | BindingFlags.NonPublic);

        GameResources resources = Globals.Resources;
        Belts_LinkedEntity[] linked = __instance.Belts_GetInputConnections();
        MetaBuildingInternalVariant.BeltIO[] beltInputs = __instance.InternalVariant.BeltInputs;
        LODBaseMesh[] beltCapInput = resources.BeltCapInput;
        LODBaseMesh[] capMesh = beltCapInput;
        beltCapInput = resources.BeltCapInputWithBorder;
        LODBaseMesh[] capMeshWithBorder = beltCapInput;
        beltCapInput = resources.BeltCapInputBorderOnly;
        drawStatic_BeltEndCaps.Invoke(__instance, new object[] { builder, linked, beltInputs, capMesh, capMeshWithBorder, beltCapInput });
        Belts_LinkedEntity[] linked2 = __instance.Belts_GetOutputConnections();
        MetaBuildingInternalVariant.BeltIO[] beltOutputs = __instance.InternalVariant.BeltOutputs;
        beltCapInput = resources.BeltCapOutput;
        LODBaseMesh[] capMesh2 = beltCapInput;
        beltCapInput = resources.BeltCapOutputWithBorder;
        LODBaseMesh[] capMeshWithBorder2 = beltCapInput;
        beltCapInput = resources.BeltCapOutputBorderOnly;
        drawStatic_BeltEndCaps.Invoke(__instance, new object[] { builder, linked2, beltOutputs, capMesh2, capMeshWithBorder2, beltCapInput });
        drawStatic_FluidEndCapsAndStands.Invoke(__instance, new object[] { builder });
    }
}