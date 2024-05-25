using UnityEngine;

namespace Unity.Game.Shared
{
    public static class TypesUtility
    {
        public struct RendererIndexData
        {
            public Renderer Renderer;
            public int MaterialIndex;

            public RendererIndexData(Renderer renderer, int index)
            {
                Renderer = renderer;
                MaterialIndex = index;
            }
        }
    }
}


