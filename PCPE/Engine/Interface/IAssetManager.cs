using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPE.Engine.Interface
{
    public interface IAssetManager<AssetType>
    {
        public AssetType GetAsset(string identifier);

        public bool AddAsset(string identifier, AssetType asset);

        public bool DeleteAsset(string identifier);
    }
}
