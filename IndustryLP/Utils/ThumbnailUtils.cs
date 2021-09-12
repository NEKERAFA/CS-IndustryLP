using ColossalFramework;
using ColossalFramework.UI;
using IndustryLP.UI;
using IndustryLP.Utils.Constants;
using System;
using UnityEngine;

namespace IndustryLP.Utils
{
    /// <summary>
    /// This class is from <see cref="FindIt.GUI.ThumnailManager"/>
    /// </summary>
    internal static class ThumbnailUtils
    {
        // Instances.
        private static GameObject gameObject;
        private static ThumbnailGenerator _generator;
        private static PreviewRenderer _renderer;
        internal static PreviewRenderer Renderer => _renderer;

        /// <summary>
        /// Queues a prefab for rendering.
        /// </summary>
        /// <param name="prefab">Prefab to queue</param>
        /// <param name="prefab">Button for thumbnail</param>
        public static void MakeThumbnail(PrefabInfo prefab, UIButton button)
        {
            // Safety first.
            if (prefab != null)
            {
                // Create the render if there isn't one already.
                if (gameObject == null)
                {
                    Create();
                }

                _generator.MakeThumbnail(prefab, button);
            }
        }


        /// <summary>
        /// Creates our renderer GameObject.
        /// </summary>
        internal static void Create()
        {
            try
            {
                // If no instance already set, create one.
                if (gameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    gameObject = new GameObject("FindItThumbnailRenderer");
                    gameObject.transform.parent = UIView.GetAView().transform;

                    // Add our queue manager and renderer directly to the gameobject.
                    _renderer = gameObject.AddComponent<PreviewRenderer>();
                    _generator = new ThumbnailGenerator();

                    LoggerUtils.Log("thumbnail renderer created");
                }
            }
            catch (Exception e)
            {
                LoggerUtils.Error(e);
            }
        }


        /// <summary>
        /// Cleans up when finished.
        /// </summary>
        internal static void Close()
        {
            UnityEngine.Object.Destroy(_renderer);
            UnityEngine.Object.Destroy(gameObject);

            LoggerUtils.Log("thumbnail renderer destroyed");

            // Let the garbage collector cleanup.
            _generator = null;
            _renderer = null;
            gameObject = null;
        }
    }


    /// <summary>
    /// Creates thumbnail images.
    /// Inspired by Boogieman Sam's FindIt! UI.
    /// </summary>
    public class ThumbnailGenerator
    {
        // Renderer for thumbnail images.
        private PreviewRenderer renderer;


        /// <summary>
        /// Update method - we render a new thumbnail every time this is called.
        /// Called by Unity every frame.
        /// </summary>
        public void MakeThumbnail(PrefabInfo prefab, UIButton button)
        {
            try
            {
                // New thumbnail name.
                string name = prefab.name;
                if (name.EndsWith("_Data"))
                {
                    name = name.Substring(0, name.LastIndexOf("_Data"));
                }

                // Back up original thumbnail icon name.
                string baseIconName = prefab.m_Thumbnail;

                // Attempt to render the thumbnail.
                if (!CreateThumbnail(name, prefab) && !baseIconName.IsNullOrWhiteSpace())
                {
                    // If it failed, restore original icon name.
                    prefab.m_Thumbnail = baseIconName;
                }

                // Assign default 'no thumbnail' thumbnail to any assets without valid thumbnails at this point.
                if (prefab.m_Atlas == null || prefab.m_Thumbnail.IsNullOrWhiteSpace())
                {
                    prefab.m_Atlas = ResourceLoader.GetAtlas("Ingame");
                    prefab.m_Thumbnail = "ToolbarIconProps";
                }

                // Update button sprites with thumbnail.
                if (button != null)
                {
                    button.atlas = prefab.m_Atlas;

                    // Variants.
                    button.normalFgSprite = prefab.m_Thumbnail;
                    button.hoveredFgSprite = prefab.m_Thumbnail + "Hovered";
                    button.pressedFgSprite = prefab.m_Thumbnail + "Pressed";
                    button.disabledFgSprite = prefab.m_Thumbnail + "Disabled";
                    button.focusedFgSprite = null;

                    // Null check for atlas just in case.
                    if (button.atlas != null)
                    {
                        // Refresh panel.
                        //FindIt.instance.scrollPanel.Refresh();
                        LoggerUtils.Warning("EMPTY ATLAS");
                    }
                }
            }
            catch (Exception e)
            {
                // Don't let a single thumnbail exception stop UI processing.
                LoggerUtils.Error("thumbnail failed", e);
            }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public ThumbnailGenerator()
        {
            LoggerUtils.Log("creating thumbnail generator");

            // Get local reference from parent.
            renderer = ThumbnailUtils.Renderer;

            // Size and setting for thumbnail images: 109 x 100, doubled for anti-aliasing.
            renderer.Size = new Vector2(109, 100) * 2f;
            renderer.CameraRotation = 35f;
        }


        /// <summary>
        /// Generates  thumbnail images (normal, focused, hovered, and pressed) for the given prefab.
        /// </summary>
        /// <param name="name">FindIt asset name</param>
        /// <param name="prefab">The prefab to generate thumbnails for</param>
        /// <returns></returns>
        internal bool CreateThumbnail(string name, PrefabInfo prefab)
        {
            // Check for valid data.
            if (prefab == null || name.IsNullOrWhiteSpace())
            {
                return false;
            }

            // Don't need to do anything if the name already matches.
            if (prefab.m_Thumbnail == name)
            {
                return true;
            }

            // Reset zoom.
            renderer.Zoom = 4f;

            // Success flag.
            bool wasRendered = false;

            if (prefab is BuildingInfo building)
            {
                wasRendered = BuildingThumbnail(building);
            }
            else if (prefab is PropInfo prop)
            {
                // Different treatment for props with blend or solid shaders.
                if (prop.m_material.shader == AssetTagList.ShaderBlend || prop.m_material.shader == AssetTagList.ShaderSolid)
                {
                    Texture2D mainTexture = prop.m_material.GetTexture("_MainTex") as Texture2D;
                    Texture2D aci = prop.m_material.GetTexture("_ACIMap") as Texture2D;

                    Texture2D texture = new Texture2D(mainTexture.width, mainTexture.height);
                    ResourceLoader.CopyTexture(mainTexture, texture);
                    Color32[] colors = texture.GetPixels32();

                    if (aci != null)
                    {
                        ResourceLoader.CopyTexture(aci, texture);
                        Color32[] aciColors = texture.GetPixels32();

                        for (int i = 0; i < colors.Length; i++)
                        {
                            colors[i].a -= aciColors[i].r;
                        }

                        texture.SetPixels32(0, 0, texture.width, texture.height, colors);
                        texture.Apply();
                    }

                    ImageUtils.ScaleTexture2(texture, 109 - 10, 100 - 10);
                    texture.name = name;

                    prefab.m_Thumbnail = name;
                    prefab.m_Atlas = ResourceLoader.CreateTextureAtlas("FindItThumbnails_" + name, new string[] { }, null);
                    ResourceLoader.AddTexturesInAtlas(prefab.m_Atlas, ImageUtils.GenerateMissingThumbnailVariants(texture));

                    // Debugging.Message("Generated thumbnails for: " + name);

                    return true;
                }

                wasRendered = PropThumbnail(prop);
            }
            else if (prefab is TreeInfo tree)
            {
                wasRendered = TreeThumbnail(tree);
            }

            // See if we were succesful in rendering.
            if (wasRendered)
            {
                // Back up game's current active texture.
                Texture2D thumbnailTexture = ResourceLoader.ConvertRenderTexture(renderer.Texture);

                // Set names.
                thumbnailTexture.name = name;
                prefab.m_Thumbnail = name;

                // Create new texture atlas and add thumbnail variants.
                prefab.m_Atlas = ResourceLoader.CreateTextureAtlas("FindItThumbnails_" + name, new string[] { }, null);
                ResourceLoader.ResizeTexture(thumbnailTexture, 109, 100);
                ResourceLoader.AddTexturesInAtlas(prefab.m_Atlas, ImageUtils.GenerateMissingThumbnailVariants(thumbnailTexture));

                // Debugging.Message("Generated thumbnails for: " + name);
            }
            else
            {
                // Rendering didn't occur - apply default thumbnail sprite name.
                prefab.m_Thumbnail = "ThumbnailBuildingDefault";
            }

            return wasRendered;
        }


        /// <summary>
        /// Generates a building thumbnail.
        /// </summary>
        /// <param name="building">BuildingInfo target</param>
        /// <returns>True if rendering occured successfully, false otherwise</returns>
        private bool BuildingThumbnail(BuildingInfo building)
        {
            // Safety first!
            if (building?.m_mesh == null || building.m_material == null)
            {
                return false;
            }

            // Ignore buildings with sub-meshes and sub-buildings
            if ((building.m_isCustomContent) && (building.m_subBuildings.Length > 0 || building.m_subMeshes.Length > 0))
            {
                return false;
            }

            // Set mesh and material for render.
            renderer.Mesh = building.m_mesh;
            renderer.Material = building.m_material;
            renderer.IsPropFenceShader = false;

            // If the selected building has colour variations, temporarily set the colour to the default for rendering.
            if (building.m_useColorVariations)
            {
                Color originalColor = building.m_material.color;
                building.m_material.color = building.m_color0;
                renderer.Render();
                building.m_material.color = originalColor;
            }
            else
            {
                // No temporary colour change needed.
                renderer.Render();
            }

            // If we made it this far, all good.
            return true;
        }


        /// <summary>
        /// Generates a prop thumbnail.
        /// </summary>
        /// <param name="prop">PropInfo target</param>
        /// <returns>True if rendering occured successfully, false otherwise</returns>
        private bool PropThumbnail(PropInfo prop)
        {
            // Safety first!
            if (prop?.m_mesh == null || prop.m_material == null)
            {
                return false;
            }

            // Set mesh and material for render.
            renderer.Mesh = prop.m_mesh;
            renderer.Material = prop.m_material;

            // props with prop fence shader need to be handled differently or they are not viewable
            renderer.IsPropFenceShader = false;
            if (!prop.m_isCustomContent)
            {
                if (prop.m_material.shader == renderer.propFenceShader)
                {
                    renderer.IsPropFenceShader = true;
                }
            }

            // If the selected prop has colour variations, temporarily set the colour to the default for rendering.
            if (prop.m_useColorVariations)
            {
                Color originalColor = prop.m_material.color;
                prop.m_material.color = prop.m_color0;
                renderer.Render();
                prop.m_material.color = originalColor;
            }
            else
            {
                // No temporary colour change needed.
                renderer.Render();
            }

            // If we made it this far, all good.
            return true;
        }


        /// <summary>
        /// Generates a tree thumbnail.
        /// </summary>
        /// <param name="building">TreeInfo target</param>
        /// <returns>True if rendering occured successfully, false otherwise</returns>
        private bool TreeThumbnail(TreeInfo tree)
        {
            // Safety first!
            if (tree?.m_mesh == null || tree.m_material == null)
            {
                return false;
            }

            // Set mesh and material for render.
            renderer.Mesh = tree.m_mesh;
            renderer.Material = tree.m_material;
            renderer.IsPropFenceShader = false;

            // Render.
            renderer.Render();

            // If we made it this far, all good.
            return true;
        }
    }
}
