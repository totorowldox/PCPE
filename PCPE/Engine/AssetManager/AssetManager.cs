using PCPE.Engine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPE.Engine.AssetManager
{
    public class AssetManager<AssetType> : IAssetManager<AssetType>
    {
        protected Dictionary<string, AssetType> _assets = new();

        public virtual AssetType GetAsset(string identifier)
        {
            return _assets.TryGetValue(identifier, out AssetType val) ? val : default;
        }

        public virtual bool AddAsset(string identifier, AssetType asset)
        {
            return _assets.TryAdd(identifier, asset);
        }

        public virtual bool DeleteAsset(string identifier)
        {
            return _assets.Remove(identifier);
        }
    }
}
