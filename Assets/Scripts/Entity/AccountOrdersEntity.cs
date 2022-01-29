using System;

namespace Entity
{
    public class AccountOrdersEntity
    {
        public long Id;
        public DateTimeOffset Dt;
        public string Address;
        public long Type; 
        public string Amount;
        public long CashType; 
        public string CashAmount;
        public string Status;
        public AccountGoodsEntity[] Goods;
    }
}