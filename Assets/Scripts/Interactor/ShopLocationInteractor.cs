using System.Collections.Generic;
using System.Linq;
using Entity;
using UnityEngine;

namespace Interactor
{
    public class ShopLocationInteractor
    {
        private static string shopLocationKey = "ShopLocation";
        private static string shopLocationIdKey = "ShopLocationId";
        private static string shopLocationNameKey = "ShopLocationName";
        private static string shopLocationCityKey = "ShopLocationCity";
        private static string reset = "ShopLocationReset";

       
        public bool HasLocation()
        {
            return PlayerPrefs.HasKey(shopLocationKey) && PlayerPrefs.HasKey(reset);
        }

        public void SetShopLocation(ShopEntity shop)
        {
            PlayerPrefs.SetInt(shopLocationIdKey, shop.id);
            PlayerPrefs.SetString(shopLocationKey, shop.address);
            PlayerPrefs.SetString(reset, shop.address);
            PlayerPrefs.SetString(shopLocationNameKey, shop.name);
            PlayerPrefs.SetString(shopLocationCityKey, shop.city);
            PlayerPrefs.Save();
        }
        
        public void SetShopLocation(int storeId, string storeName)
        {
            PlayerPrefs.SetInt(shopLocationIdKey, storeId);
            PlayerPrefs.SetString(shopLocationKey, storeName);
            PlayerPrefs.SetString(reset, storeName);
            PlayerPrefs.Save();
        }

        public void DeleteShopLocation()
        {
            Debug.LogWarning("Deleted shop location.");
            PlayerPrefs.DeleteKey(shopLocationKey);
        }

        private TopologyEntity topology;
        public void SetTopology(TopologyEntity topologyEntity) => topology = topologyEntity;
        public IEnumerable<string> GetCityShops()
        {
            var current =  PlayerPrefs.GetString(shopLocationCityKey, "");
            if(topology == null) 
                JsonUtility.Deserialize<TopologyEntity>(PlayerPrefs.GetString("Topology"), 
                    t => topology = t);
            foreach (var city in topology.cities)
                if (city == current)
                    return (from shop in topology.shops where shop.city == city select shop.address).ToList();
            return new List<string>();
        }

        public string ShopName()
        {
            if (HasLocation()) return PlayerPrefs.GetString(shopLocationKey);
            
            Debug.LogError("No shop location set yet.");
            return null;
        }
        
        public int ShopId()
        {
            if (HasLocation())
                return PlayerPrefs.GetInt(shopLocationIdKey, 601);
            Debug.LogError("No shop location set yet.");
            return 0;
        }
    }
}