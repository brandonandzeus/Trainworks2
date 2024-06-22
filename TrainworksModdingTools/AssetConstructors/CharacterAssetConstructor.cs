using BepInEx.Logging;
using HarmonyLib;
using ShinyShoe;
using Spine.Unity;
using System.Collections.Generic;
using System.IO;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Trainworks.AssetConstructors
{
    public class CharacterAssetConstructor : Interfaces.IAssetConstructor
    {
        // Empty material used for static images for characters.
        // Can't set to null directly or will get a ton of nasty warning messages each time its rendered.
        // Can't disable the meshrenderer because its involved in rendering the characters.
        public static Material NullMaterial = new Material(Shader.Find("Shiny Shoe/Character Spine Shader"));

        public GameObject Construct(AssetReference assetRef)
        {
            return CreateCharacterGameObject(assetRef);
        }

        /// <summary>
        /// Create a GameObject for the custom character with the given asset reference.
        /// Used for loading custom character art.
        /// </summary>
        /// <param name="assetRef">Reference containing the asset information</param>
        /// <returns>The GameObject for the custom character specified by the asset reference</returns>
        public static GameObject CreateCharacterGameObject(AssetReference assetRef)
        {
            Sprite sprite = CustomAssetManager.LoadSpriteFromRuntimeKey(assetRef.RuntimeKey);
            if (sprite == null)
            {
                Trainworks.Log(LogLevel.Error, "Could not find sprite with asset runtime key: " + assetRef.RuntimeKey);
                return null;
            }
            var charObj = CreateCharacterGameObject(assetRef, sprite);
            GameObject.DontDestroyOnLoad(charObj);
            return charObj;
        }

        public GameObject Construct(AssetReference assetRef, BundleAssetLoadingInfo bundleInfo)
        {
            var tex = BundleManager.LoadAssetFromBundle(bundleInfo, bundleInfo.SpriteName) as Texture2D;
            if (tex == null)
            {
                Trainworks.Log(LogLevel.Error, "Invalid sprite name when loading asset: " + bundleInfo.SpriteName);
                return null;
            }

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 128f);

            sprite.name = "Sprite_" + Path.GetFileNameWithoutExtension(bundleInfo.SpriteName);

            // Animated sprite asset with multiple spine animations
            if (bundleInfo.SpineAnimationDict != null && bundleInfo.SpineAnimationDict.Count > 0)
            {
                Dictionary<CharacterUI.Anim, GameObject> loadedAnims = new Dictionary<CharacterUI.Anim, GameObject>();
                foreach (var anim_path in bundleInfo.SpineAnimationDict)
                {
                    Trainworks.Log(LogLevel.Debug, "Looking in bundle for... " + anim_path.Value);
                    GameObject gameObject = BundleManager.LoadAssetFromBundle(bundleInfo, anim_path.Value) as GameObject;
                    if (gameObject == null)
                    {
                        Trainworks.Log(LogLevel.Error, "Could not load spine animation anim: " + anim_path.Key + " path: " + anim_path.Value);
                        continue;
                    }
                    loadedAnims.Add(anim_path.Key, gameObject);
                }
                if (loadedAnims.IsNullOrEmpty())
                {
                    Trainworks.Log(LogLevel.Error, "Failed to load any animations for " + bundleInfo.SpriteName);
                    var charObj = CreateCharacterGameObject(assetRef, sprite);
                    GameObject.DontDestroyOnLoad(charObj);
                    return charObj;
                }
                var spineObj = CreateCharacterGameObject(assetRef, sprite, loadedAnims, bundleInfo.BaseName);
                GameObject.DontDestroyOnLoad(spineObj);
                return spineObj;
            }
            // Legacy. Animated sprite asset with a single spine animation.
            else if (bundleInfo.ObjectName != null)
            {
                Trainworks.Log(LogLevel.Debug, "Looking in bundle for... " + bundleInfo.ObjectName);
                GameObject gameObject = BundleManager.LoadAssetFromBundle(bundleInfo, bundleInfo.ObjectName) as GameObject;
                if (gameObject == null)
                {
                    Trainworks.Log(LogLevel.Error, "Could not load spine animations for bundle object: " + bundleInfo.ObjectName);
                    var charObj = CreateCharacterGameObject(assetRef, sprite);
                    GameObject.DontDestroyOnLoad(charObj);
                    return charObj;
                }
                var spineObj = CreateCharacterGameObject(assetRef, sprite, gameObject);
                GameObject.DontDestroyOnLoad(spineObj);
                return spineObj;
            }
            // Sprite asset, but its not a spine animation
            else
            {
                var charObj = CreateCharacterGameObject(assetRef, sprite);
                GameObject.DontDestroyOnLoad(charObj);
                return charObj;
            }

        }

        /// <summary>
        /// Create a GameObject for the custom character with the AssetReference and Sprite
        /// </summary>
        /// <param name="assetRef">Reference containing the asset information</param>
        /// <param name="sprite">Sprite to create character with</param>
        /// <returns>The GameObject for the character</returns>
        private static GameObject CreateCharacterGameObject(AssetReference assetRef, Sprite sprite)
        {
            // TODO this code is fishy... Shouldn't the Character be drawn using the MeshRenderer?

            // Create a new character GameObject by cloning an existing, working character
            // Moving the created game object to be LOUDER if something goes wrong.
            var characterGameObject = GameObject.Instantiate(CustomCharacterManager.TemplateCharacter, new Vector3(5, 5, 0), new Quaternion());

            // Set aside its CharacterState and CharacterUI components for later use
            var characterState = characterGameObject.GetComponentInChildren<CharacterState>();
            var characterUI = characterGameObject.GetComponentInChildren<CharacterUI>();
            var characterUIMesh = characterGameObject.GetComponentInChildren<CharacterUIMesh>(true);

            // Set the name, and hide the UI
            characterUI.HideDetails();
            characterGameObject.name = "Character_" + sprite.name;

            // Make its MeshRenderer active; this is what enables the sprite we're about to attach to show up
            characterGameObject.GetComponentInChildren<MeshRenderer>(true).gameObject.SetActive(true);

            // Delete all the spine anims
            var spine = characterGameObject.GetComponentInChildren<ShinyShoe.CharacterUIMeshSpine>(true);
            foreach (Transform child in spine.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            spine.gameObject.SetActive(false);

            // Set states in the CharacterState and CharacterUI to the sprite to show it ingame
            AccessTools.Field(typeof(CharacterState), "sprite").SetValue(characterState, sprite);
            characterUI.GetSpriteRenderer().sprite = sprite;

            // Disable the meshRenderer otherwise the templateCharacter will be displayed.
            characterUIMesh.meshRenderer.forceRenderingOff = true;
            characterUIMesh.meshRenderer.material = NullMaterial;
            characterUIMesh.meshRenderer.sharedMaterial = NullMaterial;
            characterUIMesh.meshRenderer.materials = new Material[] { NullMaterial };
            characterUIMesh.meshRenderer.sharedMaterials = new Material[] { NullMaterial };

            // Set up the outline Sprite - well, seems like there will be problems here
            var outlineMesh = characterGameObject.GetComponentInChildren<CharacterUIOutlineMesh>(true);
            AccessTools.Field(typeof(CharacterUIOutlineMesh), "outlineData").SetValue(outlineMesh, null);

            return characterGameObject;
        }

        /// <summary>
        /// Create a GameObject for the custom character with the AssetReference, Sprite, and single SkeletonData
        /// </summary>
        /// <param name="assetRef">Reference containing the asset information</param>
        /// <param name="sprite">Sprite to create character with</param>
        /// <param name="skeletonData">GameObject containing the necessary Spine animation data</param>
        /// <returns>The GameObject for the character</returns>
        private static GameObject CreateCharacterGameObject(AssetReference assetRef, Sprite sprite, GameObject skeletonData)
        {
            // Create a new character GameObject by cloning an existing, working character
            var characterGameObject = GameObject.Instantiate(CustomCharacterManager.TemplateCharacter);

            // Set aside its CharacterState and CharacterUI components for later use
            var characterState = characterGameObject.GetComponentInChildren<CharacterState>();
            var characterUI = characterGameObject.GetComponentInChildren<CharacterUI>();

            // Hide the UI
            characterUI.HideDetails();
            characterGameObject.name = "Character_" + skeletonData.name;

            // Hide the quad, ensure the Spine mesh is shown (it should be by default)           
            var Quad = characterGameObject.GetComponentInChildren<ShinyShoe.CharacterUIMesh>(true).gameObject;
            Quad.SetActive(false);

            // Set the sprite for the preview
            Traverse.Create(characterState).Field<Sprite>("sprite").Value = sprite;
            characterUI.GetSpriteRenderer().sprite = sprite;

            // Set the shader
            skeletonData.GetComponent<SkeletonAnimation>().addNormals = true;
            skeletonData.GetComponent<SkeletonAnimation>().skeletonDataAsset.atlasAssets[0].PrimaryMaterial.shader = Shader.Find("Shiny Shoe/Character Spine Shader");

            // Activate the SpineMesh
            var spineMeshes = characterGameObject.GetComponentInChildren<ShinyShoe.CharacterUIMeshSpine>(true);
            spineMeshes.gameObject.SetActive(true);
            //Bounds bounds;
            //spineMeshes.Setup(sprite, true, 0f, characterGameObject.name, out bounds);

            // Skeleton cloning produces superior effects
            var clonedObject = characterGameObject.GetComponentInChildren<SkeletonAnimation>().gameObject;
            clonedObject.name = "Spine GameObject (" + characterGameObject.name + ")";

            var dest = clonedObject.GetComponentInChildren<SkeletonAnimation>();
            var source = skeletonData.GetComponentInChildren<SkeletonAnimation>();

            dest.skeletonDataAsset = source.skeletonDataAsset;

            // Now delete the pre-existing animations
            GameObject.Destroy(spineMeshes.transform.GetChild(1).gameObject);
            GameObject.Destroy(spineMeshes.transform.GetChild(2).gameObject);

            // TODO Set googly eye positions
            // TODO Add in visual effects such as particles

            // Remove our friends
            characterUI.GetComponent<SpriteRenderer>().forceRenderingOff = true;
            dest.gameObject.SetActive(false);

            Trainworks.Log(LogLevel.Debug, "Created spine component for " + characterGameObject.name);

            return characterGameObject;
        }

        private static CharacterUI.Anim[] Anims = new CharacterUI.Anim[] { CharacterUI.Anim.Idle, CharacterUI.Anim.Attack, CharacterUI.Anim.HitReact, CharacterUI.Anim.Idle_Relentless, CharacterUI.Anim.Attack_Spell, CharacterUI.Anim.Death };

        /// <summary>
        /// Create a GameObject for the custom character with the AssetReference, Sprite, and multiple SkeletonData objects.
        /// </summary>
        /// <param name="assetRef">Reference containing the asset information</param>
        /// <param name="sprite">Sprite to create character with</param>
        /// <param name="animations">Dictionary of Spine Animation GameObjects</param>
        /// <param name="name">Base name to give the newly created GameObjects</param>
        /// <returns>The GameObject for the character</returns>
        private static GameObject CreateCharacterGameObject(AssetReference assetRef, Sprite sprite, IDictionary<CharacterUI.Anim, GameObject> animations, string name)
        {
            // Create a new character GameObject by cloning an existing, working character
            var characterGameObject = GameObject.Instantiate(CustomCharacterManager.TemplateCharacter);

            // Set aside its CharacterState and CharacterUI components for later use
            var characterState = characterGameObject.GetComponentInChildren<CharacterState>();
            var characterUI = characterGameObject.GetComponentInChildren<CharacterUI>();

            // Hide the UI
            characterUI.HideDetails();
            characterGameObject.name = "Character_" + name;

            // Hide the quad, ensure the Spine mesh is shown (it should be by default)           
            var Quad = characterGameObject.GetComponentInChildren<ShinyShoe.CharacterUIMesh>(true).gameObject;
            Quad.SetActive(false);

            // Set the sprite for the preview
            Traverse.Create(characterState).Field<Sprite>("sprite").Value = sprite;
            characterUI.GetSpriteRenderer().sprite = sprite;

            foreach (var pair in animations)
            {
                // Set the shader
                pair.Value.GetComponent<SkeletonAnimation>().addNormals = true;
                pair.Value.GetComponent<SkeletonAnimation>().skeletonDataAsset.atlasAssets[0].PrimaryMaterial.shader = Shader.Find("Shiny Shoe/Character Spine Shader");
            }

            // Activate the SpineMesh
            var spineMeshes = characterGameObject.GetComponentInChildren<ShinyShoe.CharacterUIMeshSpine>(true);
            spineMeshes.gameObject.SetActive(true);
            //Bounds bounds;
            //spineMeshes.Setup(sprite, true, 0f, characterGameObject.name, out bounds);

            // Delete the pre-existing animations
            GameObject.Destroy(spineMeshes.transform.GetChild(1).gameObject);
            GameObject.Destroy(spineMeshes.transform.GetChild(2).gameObject);


            var originalObject = characterGameObject.GetComponentInChildren<SkeletonAnimation>().gameObject;
            foreach (var anim in Anims)
            {
                if (!animations.ContainsKey(anim))
                {
                    continue;
                }
                var skeletonData = animations[anim];
                // Skeleton cloning produces superior effects
                var clonedObject = GameObject.Instantiate(originalObject.transform, originalObject.transform.parent);
                clonedObject.name = "Spine GameObject (" + characterGameObject.name + " " + anim.ToString() + ")";

                var dest = clonedObject.GetComponentInChildren<SkeletonAnimation>();
                var source = skeletonData.GetComponentInChildren<SkeletonAnimation>();

                dest.skeletonDataAsset = source.skeletonDataAsset;

                // Remove our friends
                characterUI.GetComponent<SpriteRenderer>().forceRenderingOff = true;
                dest.gameObject.SetActive(false);
            }

            GameObject.Destroy(originalObject);


            Trainworks.Log(LogLevel.Debug, "Created spine component for " + characterGameObject.name);

            return characterGameObject;
        }

        // This is the fix for how to hide/show character templates, because I can't figure out how vanilla accomplishes it
        [HarmonyPatch(typeof(MonsterManager), "InstantiateCharacter")]
        class ShowDisabledImages
        {
            static void Postfix(MonsterManager __instance, ref GameObject __result)
            {
                var a = __result.GetComponentInChildren<CharacterUI>(true);
                if (a != null)
                    a.GetSpriteRenderer().forceRenderingOff = true;

                var c = __result.GetComponentInChildren<SkeletonAnimation>(true);
                if (c != null)
                    c.gameObject.SetActive(true);
            }
        }

        // This is required to reset the UV colours for Mesh based Spine animations
        [HarmonyPatch(typeof(CharacterUIMeshSpine), "Setup")]
        class FixShaderPropertiesOnCustomSpineAnims
        {
            static void Postfix(CharacterUIMeshSpine __instance)
            {
                var matProp = Traverse.Create(__instance).Field<MaterialPropertyBlock>("_matProps").Value = new MaterialPropertyBlock();
                __instance.MeshRenderer.SetPropertyBlock(matProp);
            }
        }


        private static Dictionary<CharacterUI.Anim, string> ANIM_NAMES = new Dictionary<CharacterUI.Anim, string>
        {
            {
                CharacterUI.Anim.Idle,
                "Idle"
            },
            {
                CharacterUI.Anim.Attack,
                "Attack"
            },
            {
                CharacterUI.Anim.HitReact,
                "HitReact"
            },
            {
                CharacterUI.Anim.Idle_Relentless,
                "Idle_Relentless"
            },
            {
                CharacterUI.Anim.Attack_Spell,
                "Spell"
            },
            {
                CharacterUI.Anim.Death,
                "Death"
            }
        };

        // TODO why is this here what is it's purpose.
        [HarmonyPatch(typeof(CharacterUIMeshSpine), "CreateAnimInfo")]
        class FixAnimationState
        {
            static void Prefix(CharacterUIMeshSpine __instance, CharacterUI.Anim animType)
            {
                if (!ANIM_NAMES.TryGetValue(animType, out string value))
                {
                    return;
                }
                SkeletonAnimation[] componentsInChildren = __instance.GetComponentsInChildren<SkeletonAnimation>(includeInactive: true);
                foreach (SkeletonAnimation skeletonAnimation in componentsInChildren)
                {
                    Spine.Animation animation = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(quiet: false).FindAnimation(value);
                    if (animation != null)
                    {
                        if (skeletonAnimation.state == null)
                        {
                            skeletonAnimation.state = new Spine.AnimationState(skeletonAnimation.skeletonDataAsset.GetAnimationStateData());
                        }
                        MeshRenderer component = skeletonAnimation.GetComponent<MeshRenderer>();
                        if (component == null)
                        {
                            return;
                        }

                        return;
                    }
                }
                return;
            }
        }
    }
}
