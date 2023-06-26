using InfiniteLayers;

public static class BaseMapInteractionModePatch
{
    public static bool GetMaximumAllowedLayer_Prefix(ref int __result)
    {
        __result = InfiniteLayersMain.layerNum;
        return false;
    }
}