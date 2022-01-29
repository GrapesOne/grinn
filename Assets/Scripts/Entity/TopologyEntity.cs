namespace Entity
{
    public class TopologyEntity
    {
        public string[] cities;
        public ShopEntity[] shops;
    }

    public class ShopEntity
    {
        public int id;
        public string name;
        public string address;
        public string city;
    }
}
