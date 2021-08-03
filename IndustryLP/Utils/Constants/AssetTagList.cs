using UnityEngine;

namespace IndustryLP.Utils.Constants
{
    internal static class AssetTagList
    {
        public static Shader ShaderBlend => Shader.Find("Custom/Props/Decal/Blend");
        public static Shader ShaderSolid => Shader.Find("Custom/Props/Decal/Solid");
        public static Shader ShaderPropFence => Shader.Find("Custom/Props/Prop/Fence");
        public static Shader ShaderBuildingFence => Shader.Find("Custom/Building/Fence");
        public static Shader ShaderNetworkFence => Shader.Find("Custom/Net/Fence");
    }
}
