using UnityEngine;
using ColossalFramework.UI;
using System.Reflection;

namespace IndustryLP.Common
{
    internal static class ResourceLoader
    {
        /// <summary>
        /// Creates a new <see cref="UITextureAtlas"/> from a group of <see cref="Texture2D"/> stored in the assembly Manifest 
        /// </summary>
        /// <param name="atlasName">The name of the new atlas</param>
        /// <param name="spriteNames">The name of the textures to add to the atlas</param>
        /// <param name="assemblyPath">The path of the sprites in the assembly</param>
        /// <returns></returns>
        public static UITextureAtlas CreateTextureAtlas(string atlasName, string[] spriteNames, string assemblyPath)
        {
            int maxSize = 1024;
            Texture2D texture2D = new Texture2D(maxSize, maxSize, TextureFormat.ARGB32, false);
            Texture2D[] textures = new Texture2D[spriteNames.Length];
            Rect[] regions = new Rect[spriteNames.Length];

            for (int i = 0; i < spriteNames.Length; i++)
                textures[i] = LoadTextureFromAssembly(assemblyPath + spriteNames[i] + ".png");

            regions = texture2D.PackTextures(textures, 2, maxSize);

            UITextureAtlas textureAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            Material material = Object.Instantiate(UIView.GetAView().defaultAtlas.material);
            material.mainTexture = texture2D;
            textureAtlas.material = material;
            textureAtlas.name = atlasName;

            for (int i = 0; i < spriteNames.Length; i++)
            {
                UITextureAtlas.SpriteInfo item = new UITextureAtlas.SpriteInfo
                {
                    name = spriteNames[i],
                    texture = textures[i],
                    region = regions[i],
                };

                textureAtlas.AddSprite(item);
            }

            return textureAtlas;
        }

        /// <summary>
        /// Adds a bunch of <see cref="Texture2D"/> objects into a <see cref="UITextureAtlas"/>
        /// </summary>
        /// <param name="atlas">A <see cref="UITextureAtlas"/> object</param>
        /// <param name="newTextures">A array of <see cref="Texture2D"/></param>
        /// <param name="locked">Sets the textures locked</param>
        public static void AddTexturesInAtlas(UITextureAtlas atlas, Texture2D[] newTextures, bool locked = false)
        {
            Texture2D[] textures = new Texture2D[atlas.count + newTextures.Length];

            for (int i = 0; i < atlas.count; i++)
            {
                Texture2D texture2D = atlas.sprites[i].texture;

                if (locked)
                {
                    // Locked textures workaround
                    RenderTexture renderTexture = RenderTexture.GetTemporary(texture2D.width, texture2D.height, 0);
                    Graphics.Blit(texture2D, renderTexture);

                    RenderTexture active = RenderTexture.active;
                    texture2D = new Texture2D(renderTexture.width, renderTexture.height);
                    RenderTexture.active = renderTexture;
                    texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
                    texture2D.Apply();
                    RenderTexture.active = active;

                    RenderTexture.ReleaseTemporary(renderTexture);
                }

                textures[i] = texture2D;
                textures[i].name = atlas.sprites[i].name;
            }

            for (int i = 0; i < newTextures.Length; i++)
                textures[atlas.count + i] = newTextures[i];

            Rect[] regions = atlas.texture.PackTextures(textures, atlas.padding, 4096, false);

            atlas.sprites.Clear();

            for (int i = 0; i < textures.Length; i++)
            {
                UITextureAtlas.SpriteInfo spriteInfo = atlas[textures[i].name];
                atlas.sprites.Add(new UITextureAtlas.SpriteInfo
                {
                    texture = textures[i],
                    name = textures[i].name,
                    border = (spriteInfo != null) ? spriteInfo.border : new RectOffset(),
                    region = regions[i]
                });
            }

            atlas.RebuildIndexes();
        }

        /// <summary>
        /// Gets a <see cref="UITextureAtlas"/> object that it was loaded 
        /// </summary>
        /// <param name="name">The name of the atlas</param>
        /// <returns>A <see cref="UITextureAtlas"/> object</returns>
        public static UITextureAtlas GetAtlas(string name)
        {
            UITextureAtlas[] atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
            for (int i = 0; i < atlases.Length; i++)
            {
                if (atlases[i].name == name)
                    return atlases[i];
            }

            return UIView.GetAView().defaultAtlas;
        }

        /// <summary>
        /// Loads a <see cref="Texture2D"/> object from the assembly manifest
        /// </summary>
        /// <param name="path">The path of the resource</param>
        /// <returns>A <see cref="Texture2D"/> object</returns>
        private static Texture2D LoadTextureFromAssembly(string path)
        {
            var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);

            byte[] data = new byte[resource.Length];
            resource.Read(data, 0, data.Length);

            Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            texture2D.LoadImage(data);

            return texture2D;
        }
    }
}
