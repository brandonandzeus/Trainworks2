using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Trainworks.Utilities
{
    /// <summary>
    /// Utility class for modifying the default TMP_SpriteAsset.
    /// This controls what ions can be displayed in tooltips and card text among other things.
    /// 
    /// After Build is called by the framework one TMP_SpriteAsset containing all of the icons
    /// used in each mod is created and added as a fallback to the default TMP_SpriteAsset.
    /// </summary>
    public static class TMP_SpriteAssetUtils
    {
        public static Dictionary<string, Texture2D> Icons = new Dictionary<string, Texture2D>();
        public static Texture2D CustomSpriteAtlas { get; private set; } = null;
        // 10x10
        private static readonly int MAX_TEXTURE_SIZE = 240;

        /// <summary>
        /// Add a custom text icon. The icon needs to be 24x24 exactly.
        /// If using BuildersV2.StatusEffectDataBuilder the status effect icon is added automatically.
        /// 
        /// To use add the html tag with name sprite and property name="iconname" within your card text or tooltip text.
        /// </summary>
        /// <param name="fullpath">Full path to the asset to add.</param>
        /// <param name="name">Optional name, if not given the filename is used. </param>
        /// <returns>True if the asset was successfully added.</returns>
        public static bool AddTextIcon(String fullpath, String name = null)
        {
            if (CustomSpriteAtlas != null)
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Error, "SpriteAsset has already been built. Please add icons during your Plugin's Initialize method only.");
                return false;
            }
            byte[] data = File.ReadAllBytes(fullpath);
            Texture2D texture = new Texture2D(1, 1)
            {
                name = name ?? Path.GetFileNameWithoutExtension(fullpath)
            };
            UnityEngine.ImageConversion.LoadImage(texture, data);
            if (Icons.ContainsKey(texture.name))
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Error, "Icon with name: " + texture.name + " has already been added to texture atlas.");
                return false;
            }
            if (texture.width != 24 || texture.height != 24)
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Error, string.Format("Icon is of incorrect size, must be 24x24, found size {0}x{1}", texture.width, texture.height));
                return false;
            }
            Icons.Add(texture.name, texture);
            return true;
        }

        /// <summary>
        /// Builds a SpriteAtlas and SpriteAsset and registers it with the default SpriteAsset used by MT.
        /// 
        /// Do not try to call this directly! It is called once after all Plugins have had their Initialize method executed.
        /// </summary>
        internal static void Build()
        {
            if (Icons.Count <= 0)
                return;

            CustomSpriteAtlas = new Texture2D(MAX_TEXTURE_SIZE, MAX_TEXTURE_SIZE);

            Texture2D[] textures = Icons.Values.ToArray();
            Rect[] rects = CustomSpriteAtlas.PackTextures(Icons.Values.ToArray(), 0);

            TMP_SpriteAsset spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            spriteAsset.name = "Trainworks Sprite Atlas";
            spriteAsset.spriteSheet = CustomSpriteAtlas;
            spriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite"));
            spriteAsset.material.mainTexture = CustomSpriteAtlas;
            spriteAsset.spriteInfoList = new List<TMP_Sprite>();

            for (int j = 0; j < rects.Length; j++)
            {
                var texture = textures[j];
                // Given in texture coordinates?
                var rect = rects[j];

                TMP_Sprite sprite = new TMP_Sprite();
                sprite.x = rect.x * CustomSpriteAtlas.width;
                sprite.y = rect.y * CustomSpriteAtlas.height;
                sprite.sprite = Sprite.Create(CustomSpriteAtlas, new Rect(sprite.x, sprite.y, texture.width, texture.height), new Vector2(0.5f, 0.5f), 128f);
                sprite.name = texture.name;
                sprite.unicode = 0;
                sprite.width = texture.width;
                sprite.height = texture.height;
                sprite.xOffset = 0;
                sprite.yOffset = 20; // Hardcoded constant to line up with the rest of the text.
                sprite.xAdvance = texture.width;
                sprite.scale = 1;

                spriteAsset.spriteInfoList.Add(sprite);
            }

            spriteAsset.UpdateLookupTables();

            // Add SpriteAsset to fallbacks.
            var fallbacks = TMP_Settings.defaultSpriteAsset.fallbackSpriteAssets;
            if (fallbacks == null)
            {
                fallbacks = TMP_Settings.defaultSpriteAsset.fallbackSpriteAssets = new List<TMP_SpriteAsset>();
            }
            fallbacks.Add(spriteAsset);
        }
    }
}
