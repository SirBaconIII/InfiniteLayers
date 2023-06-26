using UnityEngine;

public static class OverviewRenderModePatch
{
    public static bool GetOverviewContentTint_Prefix(ref Color __result ,MapEntity entity)
    {
        MetaBuildingVariant variant = entity.Variant;
        if (variant.IsBeltTransportBuilding)
        {
            Color[] layerColor = Globals.Resources.LayerColors;
            __result = layerColor[entity.InternalVariant.Height + (entity.Tile_I.z % 2) - 1];
            return false;
        }
        __result = variant.OverviewColor;
        return false;
    }
}