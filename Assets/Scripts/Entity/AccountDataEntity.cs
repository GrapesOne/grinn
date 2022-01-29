namespace Entity
{
    public class AccountDataEntity
    {
        public string Id;
        public string SessId;
        public string Login;
        public string Name;
        public int Enabled;
        public int Admin;
        public string Phone;
        public string Card;
        public AccountOrdersEntity[] Orders;
    }
}