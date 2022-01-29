namespace Entity
{
    public class AccountEntity
    {
        public string Email;
        public string Pw;
        public string PwConfirm;
        public string Name;
        public string Phone;
        public string Card;
        public bool Terms;
        public string Id;
        public string SessId;
        public int Enabled;
        public int Admin;
        public int ShopId = 601; //TODO set this placeholder based on actual user shop choice
        public AccountOrdersEntity[] Orders;
    }
}