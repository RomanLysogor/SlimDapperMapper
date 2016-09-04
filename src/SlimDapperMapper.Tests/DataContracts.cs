using System;
using System.Collections.Generic;

namespace SlimDapperMapper.Tests
{
    public class Customer
    {
        public int CustomerId;
        public string FirstName;
        public string LastName;
        public List<Order> Orders;
    }

    public class SingleCustomer
    {
        public int CustomerId;
        public string FirstName;
        public string LastName;
        public DateTime Created;
    }

    public class Order
    {
        public int OrderId;
        public int OrderValue;
        public List<Product> Products;
    }

    public class Product
    {
        public int ProductId;
        public string Name;
    }
}
