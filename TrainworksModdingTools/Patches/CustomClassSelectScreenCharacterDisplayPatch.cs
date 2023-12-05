using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Trainworks.Managers;
using BepInEx.Logging;
using UnityEngine.AddressableAssets;
using ShinyShoe;
using Spine.Unity;

namespace Trainworks.Patches
{
  // This patch displays custom characters on the clan select screen
    [HarmonyPatch(typeof(ClassSelectionScreen), "RefreshCharacters")]
    public class CustomClanSelectScreenPatch
    {
        private static HashSet<String> ObjectsToRemove = new HashSet<String>
        {
            "HairFx", "LeftSymbolFx", "RightSymbolFx", // Sentient Extra Stuff.
            "WyldentenGlow_RHand", "WyldentenGlow_LHand", "WyldentenGlow_Mask" // Wyldenten Extra Stuff.
        };

        static void Prefix(ref bool __state, ref ClassSelectCharacterDisplay[] ___characterDisplays)
        {
            __state = false;
            if (___characterDisplays == null)
            {
                __state = true;
            }
        }


        
        private static GameObject UpdateCharacterGameObject(CharacterState characterState, AssetReference assetRef)
        {
            var runtimeKey = assetRef.RuntimeKey;
            if (BundleManager.RuntimeKeyToBundleInfo.ContainsKey(runtimeKey))
            {
                var bundleInfo = BundleManager.RuntimeKeyToBundleInfo[runtimeKey];
                var tex = BundleManager.LoadAssetFromBundle(bundleInfo, bundleInfo.SpriteName) as Texture2D;
                if (tex == null)
                {
                    return null;
                }

                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 128f);
                sprite.name = "Sprite_" + bundleInfo.SpriteName.Replace("assets/", "").Replace(".png", "");
                if (bundleInfo.ObjectName == null)
                {
                    UpdateCharacterDisplayGameObject(characterState, sprite);
                    return null;
                }

                GameObject gameObject = BundleManager.LoadAssetFromBundle(bundleInfo, bundleInfo.ObjectName) as GameObject;
                if (gameObject == null)
                {
                    Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Could not find skeletonData for " + bundleInfo.ObjectName);
                    UpdateCharacterDisplayGameObject(characterState, sprite);
                    return null;
                }

                gameObject = GameObject.Instantiate(gameObject);
                UpdateCharacterDisplayGameObject(characterState, sprite, gameObject);
            }
            else if (CustomAssetManager.RuntimeKeyToAssetInfo.ContainsKey(runtimeKey))
            {
                var sprite = CustomAssetManager.LoadSpriteFromRuntimeKey(runtimeKey);
                UpdateCharacterDisplayGameObject(characterState, sprite);
                return null;
            }
            return null;
        }

        private static void UpdateCharacterDisplayGameObject(CharacterState characterState, Sprite sprite)
        {
            // TODO this code is fishy... Shouldn't the Character be drawn using the MeshRenderer?
            // Set aside its CharacterState and CharacterUI components for later use
            var characterUI = characterState.GetComponentInChildren<CharacterUI>();
            var characterUIMesh = characterState.GetComponentInChildren<CharacterUIMesh>(true);

            // Set the name, and hide the UI
            characterUI.HideDetails();
            characterState.name = "Character_" + sprite.name + "_ClassSelect";

            // Make its MeshRenderer active; this is what enables the sprite we're about to attach to show up
            //characterState.GetComponentInChildren<MeshRenderer>(true).gameObject.SetActive(true);

            // Delete all the spine anims
            var spine = characterState.GetComponentInChildren<ShinyShoe.CharacterUIMeshSpine>(true);
            foreach (Transform child in spine.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            spine.gameObject.SetActive(false);

            // Set states in the CharacterState and CharacterUI to the sprite to show it ingame
            AccessTools.Field(typeof(CharacterState), "sprite").SetValue(characterState, sprite);
            characterUI.GetSpriteRenderer().sprite = sprite;
            characterUI.GetSpriteRenderer().enabled = true;


            // Disable the meshRenderer otherwise the templateCharacter will be displayed.
            characterUIMesh.meshRenderer.forceRenderingOff = true;
            characterUIMesh.meshRenderer.enabled = false;
            characterUIMesh.meshRenderer.material = null;
            characterUIMesh.meshRenderer.sharedMaterial = null;
            characterUIMesh.meshRenderer.materials = Array.Empty<Material>();
            characterUIMesh.meshRenderer.sharedMaterials = Array.Empty<Material>();

            // Remove Extra GameObjects that are specific to cloned Champions.
            // The Sentient's hair glow effects, and others really.
            foreach (Transform c in characterUI.transform)
            {
                if (ObjectsToRemove.Contains(c.gameObject.name))
                {
                    GameObject.Destroy(c.gameObject);
                }       
            }

            // Set up the outline Sprite - well, seems like there will be problems here
            var outlineMesh = characterState.GetComponentInChildren<CharacterUIOutlineMesh>(true);
            AccessTools.Field(typeof(CharacterUIOutlineMesh), "outlineData").SetValue(outlineMesh, null);

            return;
        }

        private static void UpdateCharacterDisplayGameObject(CharacterState characterState, Sprite sprite, GameObject skeletonData)
        {
            // Set aside its CharacterState and CharacterUI components for later use
            var characterUI = characterState.GetComponentInChildren<CharacterUI>();

            // Hide the UI
            characterState.gameObject.SetActive(true);
            characterUI.HideDetails();
            characterState.name = "Character_" + sprite.name + "_ClassSelect";

            // Hide the quad, ensure the Spine mesh is shown (it should be by default)           
            var Quad = characterState.GetComponentInChildren<ShinyShoe.CharacterUIMesh>(true).gameObject;
            Quad.SetActive(false);

            // Set the sprite for the preview
            Traverse.Create(characterState).Field<Sprite>("sprite").Value = sprite;
            characterUI.GetSpriteRenderer().sprite = sprite;
            characterUI.GetSpriteRenderer().enabled = true;

            // Set the shader
            skeletonData.GetComponent<SkeletonAnimation>().addNormals = true;
            skeletonData.GetComponent<SkeletonAnimation>().skeletonDataAsset.atlasAssets[0].PrimaryMaterial.shader = Shader.Find("Shiny Shoe/Character Spine Shader");

            // Activate the SpineMesh
            var spineMeshes = characterState.GetComponentInChildren<ShinyShoe.CharacterUIMeshSpine>(true);
            spineMeshes.gameObject.SetActive(true);
            //Bounds bounds;
            //spineMeshes.Setup(sprite, true, 0f, characterGameObject.name, out bounds);

            // Skeleton cloning produces superior effects
            var clonedObject = characterState.GetComponentInChildren<SkeletonAnimation>().gameObject;
            clonedObject.name = "Spine GameObject (" + characterState.name + ")";
            clonedObject.SetActive(true);

            var dest = clonedObject.GetComponentInChildren<SkeletonAnimation>();
            var source = skeletonData.GetComponentInChildren<SkeletonAnimation>();


            dest.skeletonDataAsset = source.skeletonDataAsset;
            

            // Destroy the evidence
            GameObject.Destroy(skeletonData.gameObject);

            // Now delete the pre-existing animations
            GameObject.Destroy(spineMeshes.transform.GetChild(1).gameObject);
            GameObject.Destroy(spineMeshes.transform.GetChild(2).gameObject);

            // Remove Extra GameObjects that are specific to cloned Champions.
            // The Sentient's hair glow effects, and others really.
            foreach (Transform c in characterUI.transform)
            {
                if (ObjectsToRemove.Contains(c.gameObject.name))
                {
                    GameObject.Destroy(c.gameObject);
                }
            }

            // TODO Set googly eye positions
            // TODO Add in visual effects such as particles

            // Remove our friends
            characterUI.GetComponent<SpriteRenderer>().forceRenderingOff = true;
            dest.gameObject.SetActive(false);

            //Trainworks.Log(BepInEx.Logging.LogLevel.Debug, "Created spine component for " + characterGameObject.name);
        }

        static void Postfix(ref bool __state, ref ClassSelectCharacterDisplay[] ___characterDisplays, ClassSelectionIconUI ___mainClassSelectionUI,
            ClassSelectionIconUI ___subClassSelectionUI, Transform ___charactersRoot, ClassSelectionScreen __instance)
        {
            if (!__state)
                return;

            int customClassCount = CustomClassManager.CustomClassData.Values.Count;
            int totalClassCount = ProviderManager.SaveManager.GetAllGameData().GetAllClassDatas().Count;
            int vanillaClassCount = totalClassCount - customClassCount;

            // "totalClassCount + 1" to account for the random slot
            var characterDisplaysNew = new ClassSelectCharacterDisplay[(totalClassCount + 1) * 2];

            // Do not change. If this is changed then random effects will show up on the character
            // Most champions have additional components for special effects which need to be removed.
            // Awoken main (for some reason duplicating Hornbreaker Prince doesn't change the texture...).
            var characterDisplayMain = ___characterDisplays[1];
            // Awoken sub
            var characterDisplaySub = ___characterDisplays[vanillaClassCount + 1 + 1];

            // Setup is main classes then sub classes.
            AccessTools.Field(typeof(ClassSelectCharacterDisplay), "clanIndex").SetValue(___characterDisplays[vanillaClassCount], totalClassCount + 1);
            AccessTools.Field(typeof(ClassSelectCharacterDisplay), "clanIndex").SetValue(___characterDisplays[2 * vanillaClassCount + 1], totalClassCount + 1);

            for (int i = 0; i < vanillaClassCount; i++)
            {
                characterDisplaysNew[i] = ___characterDisplays[i];
                characterDisplaysNew[i + totalClassCount + 1] = ___characterDisplays[i + vanillaClassCount + 1];
            }
            characterDisplaysNew[totalClassCount] = ___characterDisplays[vanillaClassCount];
            characterDisplaysNew[2 * totalClassCount + 1] = ___characterDisplays[2 * vanillaClassCount + 1];

            int j = 0;
            foreach (ClassData customClassData in CustomClassManager.CustomClassData.Values)
            {
                int clanIndex = vanillaClassCount + j;

                var customMainCharacterDisplay = GameObject.Instantiate(characterDisplayMain, ___charactersRoot);
                customMainCharacterDisplay.name = customClassData.name + " main";
                characterDisplaysNew[clanIndex] = customMainCharacterDisplay;
                AccessTools.Field(typeof(ClassSelectCharacterDisplay), "clanIndex").SetValue(characterDisplaysNew[clanIndex], clanIndex + 1);

                var customSubCharacterDisplay = GameObject.Instantiate(characterDisplaySub, ___charactersRoot);
                customSubCharacterDisplay.name = customClassData.name + " sub";
                characterDisplaysNew[clanIndex + totalClassCount + 1] = customSubCharacterDisplay;
                AccessTools.Field(typeof(ClassSelectCharacterDisplay), "clanIndex").SetValue(characterDisplaysNew[clanIndex + totalClassCount + 1], clanIndex + 1);

                var mainCharacterIDs = CustomClassManager.CustomClassSelectScreenCharacterIDsMain[customClassData.GetID()];
                CharacterState[] characterStates = customMainCharacterDisplay.GetComponentsInChildren<CharacterState>(true);
                for (int k = 0; k < characterStates.Length; k++)
                {
                    var characterState = characterStates[k];
                    if (mainCharacterIDs == null || k >= mainCharacterIDs.Length)
                    {
                        characterState.gameObject.SetActive(false);
                        continue;
                    }
                        
                    var mainCharacterData = CustomCharacterManager.GetCharacterDataByID(mainCharacterIDs[k]);

                    var assetRef = mainCharacterData.characterPrefabVariantRef;
                    UpdateCharacterGameObject(characterState, assetRef);
                    AccessTools.Field(typeof(ClassSelectCharacterDisplay), "characters").SetValue(customMainCharacterDisplay, null);
                }

                var subCharacterIDs = CustomClassManager.CustomClassSelectScreenCharacterIDsSub[customClassData.GetID()];
                characterStates = customSubCharacterDisplay.GetComponentsInChildren<CharacterState>(true);
                for (int k = 0; k < characterStates.Length; k++)
                {
                    var characterState = characterStates[k];
                    if (subCharacterIDs == null || k >= subCharacterIDs.Length)
                    {
                        characterState.gameObject.SetActive(false);
                        continue;
                    }

                    var subCharacterData = CustomCharacterManager.GetCharacterDataByID(subCharacterIDs[k]);

                    var assetRef = subCharacterData.characterPrefabVariantRef;
                    UpdateCharacterGameObject(characterState, assetRef);
                    AccessTools.Field(typeof(ClassSelectCharacterDisplay), "characters").SetValue(customMainCharacterDisplay, null);
                }

                j++;
            }

            ___characterDisplays = characterDisplaysNew;
        }
    }
}