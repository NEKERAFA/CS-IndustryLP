using ColossalFramework.UI;
using System.Reflection;
using UnityEngine;

namespace IndustryLP.Utils
{
    /// <summary>
    /// This class loads the textures linked onto the c# assembly and converts into TextureAtlas.
    /// 
    /// This class is from <see cref="MoveIt.ResourceLoader"/>
    /// </summary>
    internal static class ResourceLoader
    {
        /// <summary>
        /// Creates a new texture atlas with the defined sprites
        /// </summary>
        /// <param name="atlasName">The name of the new atlas</param>
        /// <param name="spriteNames">The name of the sprites</param>
        /// <param name="assemblyPath">The path of the sprites in the assembly</param>
        /// <returns>The new <see cref="UITextureAtlas"/> object</returns>
        public static UITextureAtlas CreateTextureAtlas(string atlasName, string[] spriteNames, string assemblyPath)
        {
            var maxSize = 1024;
            var texture2D = new Texture2D(maxSize, maxSize, TextureFormat.ARGB32, false);
            var textures = new Texture2D[spriteNames.Length];

            for (var i = 0; i < spriteNames.Length; i++)
                textures[i] = LoadTextureFromAssembly(assemblyPath + "." + spriteNames[i] + ".png");

           var regions = texture2D.PackTextures(textures, 2, maxSize);

            var textureAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            var material = Object.Instantiate(UIView.GetAView().defaultAtlas.material);
            material.mainTexture = texture2D;
            textureAtlas.material = material;
            textureAtlas.name = atlasName;

            for (var i = 0; i < spriteNames.Length; i++)
            {
                var item = new UITextureAtlas.SpriteInfo
                {
                    name = spriteNames[i],
                    texture = textures[i],
                    region = regions[i]
                };

                textureAtlas.AddSprite(item);
            }

            return textureAtlas;
        }

        /// <summary>
        /// Adds a bunch of textures onto the atlas
        /// </summary>
        /// <param name="atlas">A <see cref="UITextureAtlas"/> object</param>
        /// <param name="newTextures">A array with the <see cref="Texture2D"/> objects to add</param>
        /// <param name="locked">True if the textures will be locked in the atlas</param>
        public static void AddTexturesInAtlas(UITextureAtlas atlas, Texture2D[] newTextures, bool locked = false)
        {
            var textures = new Texture2D[atlas.count + newTextures.Length];

            for (var i = 0; i < atlas.count; i++)
            {
                var texture2D = atlas.sprites[i].texture;

                if (locked)
                {
                    // Locked textures workaround
                    var renderTexture = RenderTexture.GetTemporary(texture2D.width, texture2D.height, 0);
                    Graphics.Blit(texture2D, renderTexture);

                    var active = RenderTexture.active;
                    texture2D = new Texture2D(renderTexture.width, renderTexture.height);
                    RenderTexture.active = renderTexture;
                    texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
                    texture2D.Apply();
                    RenderTexture.active = active;

                    RenderTexture.ReleaseTemporary(renderTexture);
                }

                textures[i] = texture2D;
                textures[i].name = atlas.sprites[i].name;
            }

            for (var i = 0; i < newTextures.Length; i++)
                textures[atlas.count + i] = newTextures[i];

            var regions = atlas.texture.PackTextures(textures, atlas.padding, 4096, false);

            atlas.sprites.Clear();

            for (var i = 0; i < textures.Length; i++)
            {
                var spriteInfo = atlas[textures[i].name];
                atlas.sprites.Add(new UITextureAtlas.SpriteInfo
                {
                    texture = textures[i],
                    name = textures[i].name,
                    border = spriteInfo != null ? spriteInfo.border : new RectOffset(),
                    region = regions[i]
                });
            }

            atlas.RebuildIndexes();
        }

        /// <summary>
        /// Gets a <see cref="UITextureAtlas"/> object that it was loaded
        /// </summary>
        /// <param name="name">The name of the <see cref="UITextureAtlas"/></param>
        /// <returns>The <see cref="UITextureAtlas"/> object</returns>
        public static UITextureAtlas GetAtlas(string name)
        {
            var atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
            for (var i = 0; i < atlases.Length; i++)
                if (atlases[i].name == name)
                    return atlases[i];

            return UIView.GetAView().defaultAtlas;
        }

        /// <summary>
        /// Loads a <see cref="Texture2D"/> from the assembly
        /// </summary>
        /// <param name="path">The path of the texture in the asembly</param>
        /// <returns>The <see cref="Texture2D"/> loaded</returns>
        public static Texture2D LoadTextureFromAssembly(string path)
        {
            var manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);

            var array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, array.Length);

            var texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            texture2D.LoadImage(array);

            return texture2D;
        }

        public static Texture2D ConvertRenderTexture(RenderTexture renderTexture)
        {
            RenderTexture active = RenderTexture.active;
            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;

            return texture2D;
        }

        public static void ResizeTexture(Texture2D texture, int width, int height)
        {
            RenderTexture active = RenderTexture.active;

            texture.filterMode = FilterMode.Trilinear;
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height);
            renderTexture.filterMode = FilterMode.Trilinear;

            RenderTexture.active = renderTexture;
            Graphics.Blit(texture, renderTexture);
            texture.Resize(width, height);
            texture.ReadPixels(new Rect(0, 0, width, width), 0, 0);
            texture.Apply();

            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(renderTexture);
        }

        public static void CopyTexture(Texture2D texture2D, Texture2D dest)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture2D.width, texture2D.height, 0);
            Graphics.Blit(texture2D, renderTexture);

            RenderTexture active = RenderTexture.active;
            RenderTexture.active = renderTexture;
            dest.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
            dest.Apply();
            RenderTexture.active = active;

            RenderTexture.ReleaseTemporary(renderTexture);
        }
    }
}