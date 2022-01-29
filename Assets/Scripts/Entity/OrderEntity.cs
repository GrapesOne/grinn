namespace Entity
{
    public class OrderEntity
    {
        public string id;
        public string sessid;
        public int id_shop;
        public Basket basket;
    }

    public class Basket
    {
        public Item[] items;
        public string orderType;
        public string orderAddress;
        public string orderCashType;
        public float orderCashAmount;
        public string orderDeliveryDate;
        public string orderDeliveryTime;
        public string orderComment;
        public bool hasCard;
        public float amount;
        public bool deliveryAvail;
        public float delivery;
        public float total;
        public bool orderAvail;
        public bool cashAvail;
    }

    public class Item
    {
        public string id;
        public string name;
        public string image;
        public string measure;
        public float quantity;
        public float max;
        public float min;
        public float step;
        public string regprice;
        public string actprice;
        public string pickup;
        public string fimage;
        public string price;
        public float amount;
    }
}