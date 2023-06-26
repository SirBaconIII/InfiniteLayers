using InfiniteLayers;
using System;

public static class MetaGameModePatch
{
    public static void OnEnable_Postfix(MetaGameMode __instance)
    {
        Array.Resize(ref __instance.LayerUnlocks, InfiniteLayersMain.layerNum);
    }
}