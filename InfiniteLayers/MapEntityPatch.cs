using static MapEntity;
using Unity.Mathematics;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

public static class MapEntityPatch
{
    public static bool DrawStatic_BeltEndCaps_Prefix(MapEntity __instance, MeshBuilder builder, Belts_LinkedEntity[] linked, MetaBuildingInternalVariant.BeltIO[] ioSlots, LODBaseMesh[] capMesh, LODBaseMesh[] capMeshWithBorder, LODBaseMesh[] capMeshBorderOnly)
    {
        MethodInfo computeHasPriority = typeof(MapEntity).GetMethod("ComputeHasPriority", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo drawStatic_Seperator = typeof(MapEntity).GetMethod("DrawStatic_Seperator", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo drawStatic_EndCap = typeof(MapEntity).GetMethod("DrawStatic_EndCap", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo drawStatic_BeltStands = typeof(MapEntity).GetMethod("DrawStatic_BeltStands", BindingFlags.Instance | BindingFlags.NonPublic);

        for (int i = 0; i < ioSlots.Length; i++)
        {
            MetaBuildingInternalVariant.BeltIO ioSlot = ioSlots[i];
            Belts_LinkedEntity link = linked[i];
            MetaBuildingInternalVariant.BeltIO otherSlot = link.Slot;
            int layer = (__instance.Tile_I.z + ioSlot.Position_L.z) % 2;
            if (ioSlot.Seperators)
            {
                if (link.Entity != null && link.Slot.Seperators)
                {
                    if ((bool)computeHasPriority.Invoke(__instance, new object[] {link.Entity} )) 
                    {
                        drawStatic_Seperator.Invoke(__instance, new object[] { builder, ioSlot, Globals.Resources.BuildingSeperatorsShared });
                    }
                }
                else
                {
                    drawStatic_Seperator.Invoke(__instance, new object[] { builder, ioSlot, Globals.Resources.BuildingSeperators });
                }
            }
            switch (ioSlot.IOType)
            {
                case MetaBuildingInternalVariant.BeltIOType.Regular:
                    if (link.Entity == null)
                    {
                        drawStatic_EndCap.Invoke(__instance, new object[] { builder, ioSlot, capMesh[layer] });
                    }
                    break;
                case MetaBuildingInternalVariant.BeltIOType.ElevatedBorder:
                    if (link.Entity == null)
                    {
                        drawStatic_EndCap.Invoke(__instance, new object[] { builder, ioSlot, capMeshWithBorder[layer] });
                    }
                    else if (otherSlot.IOType != MetaBuildingInternalVariant.BeltIOType.ElevatedBorder && !otherSlot.Seperators)
                    {
                        drawStatic_EndCap.Invoke(__instance, new object[] { builder, ioSlot, capMeshBorderOnly[layer] });
                    }
                    break;
            }
            bool shouldDrawStand = link.Entity == null || (bool)computeHasPriority.Invoke(__instance, new object[] { link.Entity }) || otherSlot.StandType == MetaBuildingInternalVariant.BeltStandType.None;
            if (link.Entity != null && shouldDrawStand)
            {
                Grid.Direction direction_I = __instance.I_From_L_Direction(ioSlot.Direction_L);
                int2 tile_L = ioSlot.Position_L.xy;
                int2 pos_I = __instance.I_From_L(in tile_L) * 2;
                int2 pos_Stand = pos_I * 2 + Grid.DirectionToUnitVector(direction_I);
                shouldDrawStand = ((direction_I != 0 && direction_I != Grid.Direction.Left) ? (shouldDrawStand && pos_Stand.y % (2 * Globals.Resources.DistanceBetweenStands) == 1) : (shouldDrawStand && pos_Stand.x % (2 * Globals.Resources.DistanceBetweenStands) == 1));
            }
            int3 standTile = ioSlot.Position_L;
            MetaBuildingInternalVariant.BeltStandType standType = ioSlot.StandType;
            MetaBuildingInternalVariant.BeltStandType beltStandType = standType;
            if (beltStandType != 0 && beltStandType == MetaBuildingInternalVariant.BeltStandType.Normal && shouldDrawStand)
            {
                ref int3 position_L = ref ioSlot.Position_L;
                LODBaseMesh[] beltCapStandsNormal = Globals.Resources.BeltCapStandsNormal;
                drawStatic_BeltStands.Invoke(__instance, new object[] { builder, position_L, beltCapStandsNormal, ioSlot.Direction_L });
            }
        }
        return false;
    }

    public static bool DrawStatic_BaseMesh_Prefix(MapEntity __instance, MeshBuilder builder)
    {
        if (!__instance.InternalVariant.HasMainMesh)
        {
            return false;
        }
        if (__instance.InternalVariant.IndividualMainMeshPerLayer)
        {
            LOD4Mesh[] meshes = __instance.InternalVariant.MainMeshPerLayerLOD;
            int meshIndex = __instance.Tile_I.z % (meshes.Length - 1);
            if (meshIndex < meshes.Length)
            {
                Mesh mesh = meshes[meshIndex].GetMesh(builder.TargetLOD);
                float3 position = __instance.Island.W_From_I(in __instance.Tile_I);
                builder.AddTranslateRotate(mesh, in position, __instance.Rotation_G);
            }
        }
        else
        {
            LOD4Mesh mainMeshLOD = __instance.InternalVariant.MainMeshLOD;
            float3 position = __instance.Island.W_From_I(in __instance.Tile_I);
            builder.AddTranslateRotate(mainMeshLOD, in position, __instance.Rotation_G);
        }
        return false;
    }

    public static bool DrawStatic_FluidEndCapsAndStands_Prefix(MapEntity __instance, MeshBuilder builder)
    {
        MethodInfo drawStatic_EndCap = typeof(MapEntity).GetMethod("DrawStatic_EndCap", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo drawStatic_PipeStand = typeof(MapEntity).GetMethod("DrawStatic_PipeStand", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo drawStatic_GetStandHeight_L = typeof(MapEntity).GetMethod("DrawStatic_GetStandHeight_L", BindingFlags.Instance | BindingFlags.NonPublic);

        MetaBuildingInternalVariant.FluidContainerConfig[] containers = __instance.InternalVariant.FluidContainers;
        VisualThemeBaseResources resources = Singleton<GameCore>.G.Theme.BaseResources;
        for (int containerIndex = 0; containerIndex < containers.Length; containerIndex++)
        {
            MetaBuildingInternalVariant.FluidIO[] connections = containers[containerIndex].Connections;
            List<Fluids_LinkedContainer> linkedContainers = __instance.Fluids_GetConnectedContainers(containers[containerIndex]);
            FluidContainer container = __instance.Fluids_GetContainerByIndex(containerIndex);
            int connectionIndex = 0;
            while (connectionIndex < connections.Length)
            {
                MetaBuildingInternalVariant.FluidIO connection = connections[connectionIndex];
                int layer = (connection.Position_L.z + __instance.Tile_I.z) % 2;
                Fluids_LinkedContainer link = linkedContainers.Find((Fluids_LinkedContainer linked) => linked.FromConnectionIndex == connectionIndex);
                if (link == null)
                {
                    drawStatic_EndCap.Invoke(__instance, new object[] { builder, connection, (connection.IOType == MetaBuildingInternalVariant.FluidIOType.Building) ? resources.PipeBuildingStandsAndEndCap[layer] : resources.PipeStandsAndEndCap[layer] });
                }
                else if (container.HasRightToUpdate(link.Container))
                {
                    if (link.FromConnection.IOType == MetaBuildingInternalVariant.FluidIOType.Pipe && link.ToConnection.IOType == MetaBuildingInternalVariant.FluidIOType.Pipe)
                    {
                        drawStatic_PipeStand.Invoke(__instance, new object[] { builder, drawStatic_GetStandHeight_L.Invoke(__instance, new object[] { connection.Position_L }), connection.Position_L, connection.Direction_L});
                    }
                    else
                    {
                        drawStatic_EndCap.Invoke(__instance, new object[] { builder, connection, resources.PipeBuildingConnector });
                    }
                }
                int num = connectionIndex + 1;
                connectionIndex = num;
            }
        }
        return false;
    }
}

