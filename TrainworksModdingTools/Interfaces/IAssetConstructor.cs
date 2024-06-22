using Trainworks.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Trainworks.Interfaces
{
    public interface IAssetConstructor
    {
        GameObject Construct(AssetReference assetRef);
        GameObject Construct(AssetReference assetRef, BundleAssetLoadingInfo bundleInfo);
    }
}
