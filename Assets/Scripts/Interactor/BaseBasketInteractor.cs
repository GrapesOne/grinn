using System;
using System.Collections.Generic;
using System.Linq;
using Entity;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;

namespace Interactor
{
    public abstract class BaseBasketInteractor
    {
        protected ISubject<ProductEntity> onAddToBasket = new Subject<ProductEntity>();
        public IObservable<ProductEntity> OnAddToBasket => onAddToBasket;

        protected ISubject<ProductEntity> onRemoveFromBasket = new Subject<ProductEntity>();
        public IObservable<ProductEntity> OnRemoveFromBasket => onRemoveFromBasket;

        protected ISubject<int> onCleanBasket = new Subject<int>();
        public IObservable<int> OnCleanBasket => onCleanBasket;

        protected ISubject<ProductEntity> onChangeQuantity = new Subject<ProductEntity>();
        public IObservable<ProductEntity> OnChangeQuantity => onChangeQuantity;


        protected int count;
        protected Dictionary<Id, ProductEntity> basketProducts = new Dictionary<Id, ProductEntity>();

        public Dictionary<Id, ProductEntity> GetBasketProducts() => basketProducts;

        public int GetBasketCount() => basketProducts.Count;

        public Id AddToBasket(Id id, ProductEntity entity)
        {
            if (basketProducts.ContainsKey(id)) return id;
            basketProducts.Add(id, entity);
            onAddToBasket.OnNext(entity);
            return id;
        }

        public Id AddToBasket(ProductEntity entity)
        {
            var id = new Id {Value = $"{count++:00000}"};
            basketProducts.Add(id, entity);
            onAddToBasket.OnNext(entity);
            return id;
        }

        public void CleanBasket()
        {
            count = 0;
            basketProducts.Clear();
            onCleanBasket.OnNext(0);
        }

        public void RemoveFromBasket(Id? id)
        {
            if (id == null) return;
            var rem = basketProducts[id.Value];
            basketProducts.Remove(id.Value);
            onChangeQuantity.OnNext(rem);
            onRemoveFromBasket.OnNext(rem);
            if (basketProducts.Count == 0) onCleanBasket.OnNext(0);
        }

        public bool HasId(Id id) => basketProducts.ContainsKey(id);

        public Id? HasProduct(ProductEntity product)
        {
            foreach (var entity in
                basketProducts.Where(entity => entity.Value.Id.Equals(product.Id)))
                return entity.Key;
            return null;
        }


        public void ChangeQuantity(Id? id, float quantity)
        {
            if (id == null) return;
            basketProducts[id.Value].OrderQuantity =
                Mathf.Clamp(quantity, basketProducts[id.Value].Min, basketProducts[id.Value].Max);
            onChangeQuantity.OnNext(basketProducts[id.Value]);
        }

        public void SetBasketProducts(Dictionary<string, ProductEntity> basket)
        {
            CleanBasket();
            if (basket == null) return;
            foreach (var entity in basket) AddToBasket(entity.Value);
        }

        protected void OnSuccess(string val)
        {
            Debug.LogWarning(val);
        }

        protected void OnError(string val)
        {
            Debug.LogWarning(val);
        }


        public struct Id
        {
            private string val;

            public string Value
            {
                set
                {
                    if (val.IsNullOrWhitespace()) val = "";
                    if (val.Length == 5) return;
                    if (value.Length == 5) val = value;
                }
                get => val.IsNullOrWhitespace() ? "" : val;
            }

            public override bool Equals(object obj) => Equals(obj as Id? ?? default);
            public bool Equals(Id p) => GetType() == p.GetType() && Value == p.Value;

            public override int GetHashCode() => Value.GetHashCode();

            public static bool operator ==(Id lhs, Id rhs) => lhs.Equals(rhs);

            public static bool operator !=(Id lhs, Id rhs) => !(lhs == rhs);

            public override string ToString()
            {
                return Value;
            }
        }
    }
}