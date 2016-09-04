using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlimDapperMapper.Tests
{
    public class MapperCommonTests
    {
        private void CustomerParser(Customer c, Order o, Product p, IDictionary<object, Customer> lookup)
        {
            Customer customer = null;
            if (!lookup.TryGetValue(c.CustomerId, out customer))
            {
                lookup.Add(c.CustomerId, customer = c);
            }

            if (o != null)
            {
                if (customer.Orders == null) customer.Orders = new List<Order>();
                if (!customer.Orders.Any(order => order.OrderId.Equals(o.OrderId)))
                {
                    customer.Orders.Add(o);
                }

                if (p != null)
                {
                    var customerOrder = customer.Orders.FirstOrDefault(order => order.OrderId.Equals(o.OrderId));
                    if (customerOrder != null)
                    {
                        if (customerOrder.Products == null) customerOrder.Products = new List<Product>();
                        if (!customerOrder.Products.Any(product => product.ProductId.Equals(p.ProductId)))
                        {
                            customerOrder.Products.Add(p);
                        }
                    }
                }
            }
        }

        [Fact]
        public void SimpleleObject_OneEntity_Test()
        {
            // Arrange
            SlimDapperMapper.Configuration.RegisterLookupConvention<SingleCustomer>(t => "CustomerId");
            var customerFromDb = new List<IDictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    {"CustomerId",1 },
                    {"LastName", "John" },
                    {"FirstName", "Doe" },
                    {"Created", DateTime.Now }
                }
            };

            // Act
            var customers = SlimDapperMapper.AutoMapper.Map<SingleCustomer>(customerFromDb, (c, dict) =>
             {
                 if (c != null)
                 {
                     if (!dict.ContainsKey(c.CustomerId))
                     {
                         dict.Add(c.CustomerId, c);
                     }
                 }
             });

            // Assert
            Assert.Equal(customers.Count, 1);
            Assert.Equal(customers.First().CustomerId, 1);
            Assert.Equal(customers.First().FirstName, "Doe");
            Assert.Equal(customers.First().LastName, "John");
            Assert.True(customers.First().Created != default(DateTime));
        }

        [Fact]
        public void SimpleleObject_TwoEntities_Test()
        {
            // Arrange
            SlimDapperMapper.Configuration.RegisterLookupConvention<SingleCustomer>(t => "CustomerId");
            var customersFromDb = new List<IDictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    {"CustomerId",1 },
                    {"LastName", "John" },
                    {"FirstName", "Doe" },
                    {"Created", DateTime.Now }
                },
                new Dictionary<string, object>()
                {
                    {"CustomerId",2 },
                    {"LastName", "Dagger" },
                    {"FirstName", "Peter" },
                    {"Created", DateTime.Now }
                }
            };

            // Act
            var customers = SlimDapperMapper.AutoMapper.Map<SingleCustomer>(customersFromDb, (c, dict) =>
            {
                if (c != null)
                {
                    if (!dict.ContainsKey(c.CustomerId))
                    {
                        dict.Add(c.CustomerId, c);
                    }
                }
            });

            // Assert
            Assert.Equal(customers.Count, 2);
            Assert.True(customers.Any(c => c.CustomerId == 1));
            Assert.Equal(customers.First(c => c.CustomerId == 1).FirstName, "Doe");
            Assert.Equal(customers.First(c => c.CustomerId == 1).LastName, "John");
            Assert.True(customers.First(c => c.CustomerId == 1).Created != default(DateTime));

            Assert.True(customers.Any(c => c.CustomerId == 2));
            Assert.Equal(customers.First(c => c.CustomerId == 2).FirstName, "Peter");
            Assert.Equal(customers.First(c => c.CustomerId == 2).LastName, "Dagger");
            Assert.True(customers.First(c => c.CustomerId == 2).Created != default(DateTime));
        }

        [Fact]
        public void NestedObject_OneEntity_Test()
        {
            // Arrange
            SlimDapperMapper.Configuration.RegisterLookupConvention<Customer>(t => t.Name + "Id");
            var fullCustomerDataFromDb = new List<IDictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    { "CustomerId", 1 },
                    { "LastName", "John" },
                    { "FirstName", "Doe" },
                    { "Created", DateTime.Now },
                    { "OrderId", 10 },
                    { "OrderValue", 100 },
                    { "ProductId", 101 },
                    { "Name", "Pizza Mozarella" }
                },
                new Dictionary<string, object>()
                {
                    { "CustomerId", 1 },
                    { "LastName", "John" },
                    { "FirstName", "Doe" },
                    { "Created", DateTime.Now },
                    { "OrderId", 11 },
                    { "OrderValue", 111 },
                    { "ProductId", 102 },
                    { "Name", "Pizza Chelentano" }
                },
                new Dictionary<string, object>()
                {
                    { "CustomerId", 1 },
                    { "LastName", "John" },
                    { "FirstName", "Doe" },
                    { "Created", DateTime.Now },
                    { "OrderId", 12 },
                    { "OrderValue", 112 },
                    { "ProductId", 103 },
                    { "Name", "Sushi" }
                }
                , new Dictionary<string, object>()
                {
                    { "CustomerId", 1 },
                    { "LastName", "John" },
                    { "FirstName", "Doe" },
                    { "Created", DateTime.Now },
                    { "OrderId", 12 },
                    { "OrderValue", 112 },
                    { "ProductId", 104 },
                    { "Name", "Coca Cola" }
                }
            };

            // Act
            var customers = SlimDapperMapper.AutoMapper.Map<Customer, Order, Product>(fullCustomerDataFromDb, CustomerParser);

            // Assert
            Assert.Equal(customers.Count, 1);
            Assert.True(customers.Any(c => c.CustomerId == 1));
            var customer = customers.First(c => c.CustomerId == 1);
            Assert.Equal(customer.FirstName, "Doe");
            Assert.Equal(customer.LastName, "John");

            Assert.Equal(customer.Orders.Count, 3);
            Assert.True(customer.Orders.Any(o => o.OrderId == 10));
            Assert.Equal(customer.Orders.First(o => o.OrderId == 10).OrderValue, 100);
            Assert.Equal(customer.Orders.First(o => o.OrderId == 10).Products.Count, 1);
            Assert.Equal(customer.Orders.First(o => o.OrderId == 10).Products.First().ProductId, 101);
            Assert.Equal(customer.Orders.First(o => o.OrderId == 10).Products.First().Name, "Pizza Mozarella");

            Assert.True(customer.Orders.Any(o => o.OrderId == 11));
            Assert.Equal(customer.Orders.First(o => o.OrderId == 11).OrderValue, 111);
            Assert.Equal(customer.Orders.First(o => o.OrderId == 11).Products.Count, 1);
            Assert.Equal(customer.Orders.First(o => o.OrderId == 11).Products.First().ProductId, 102);
            Assert.Equal(customer.Orders.First(o => o.OrderId == 11).Products.First().Name, "Pizza Chelentano");

            Assert.True(customer.Orders.Any(o => o.OrderId == 12));
            Assert.Equal(customer.Orders.First(o => o.OrderId == 12).OrderValue, 112);
            Assert.Equal(customer.Orders.First(o => o.OrderId == 12).Products.Count, 2);
            Assert.True(customer.Orders.First(o => o.OrderId == 12).Products.Any(p => p.ProductId == 103));
            Assert.True(customer.Orders.First(o => o.OrderId == 12).Products.Any(p => p.ProductId == 104));
            Assert.Equal(customer.Orders.First(o => o.OrderId == 12).Products.First(p => p.ProductId == 103).Name, "Sushi");
            Assert.Equal(customer.Orders.First(o => o.OrderId == 12).Products.First(p => p.ProductId == 104).Name, "Coca Cola");
        }

        [Fact]
        public void NestedObject_TwoEntities_Test()
        {
            // Arrange
            SlimDapperMapper.Configuration.RegisterLookupConvention<Customer>(t => t.Name + "Id");
            var fullCustomerDataFromDb = new List<IDictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    { "CustomerId", 1 },
                    { "LastName", "John" },
                    { "FirstName", "Doe" },
                    { "Created", DateTime.Now },
                    { "OrderId", 10 },
                    { "OrderValue", 100 },
                    { "ProductId", 101 },
                    { "Name", "Pizza Mozarella" }
                },
                new Dictionary<string, object>()
                {
                    { "CustomerId", 1 },
                    { "LastName", "John" },
                    { "FirstName", "Doe" },
                    { "Created", DateTime.Now },
                    { "OrderId", 11 },
                    { "OrderValue", 101 },
                    { "ProductId", 102 },
                    { "Name", "Pizza Chelentano" }
                },
                new Dictionary<string, object>()
                {
                    { "CustomerId", 2 },
                    { "LastName", "Daniels" },
                    { "FirstName", "Jack" },
                    { "OrderId", 12 },
                    { "OrderValue", 110 },
                    { "ProductId", 103 },
                    { "Name", "Sushi" }
                }
                , new Dictionary<string, object>()
                {
                    { "CustomerId", 2 },
                    { "LastName", "Daniels" },
                    { "FirstName", "Jack" },
                    { "OrderId", 13 },
                    { "OrderValue", 111 },
                    { "ProductId", 104 },
                    { "Name", "Coca Cola" }
                }
            };

            // Act
            var customers = SlimDapperMapper.AutoMapper.Map<Customer, Order, Product>(fullCustomerDataFromDb, CustomerParser);

            // Assert
            Assert.Equal(customers.Count, 2);
            Assert.True(customers.Any(c => c.CustomerId == 1));
            var customer1 = customers.First(c => c.CustomerId == 1);
            Assert.Equal(customer1.FirstName, "Doe");
            Assert.Equal(customer1.LastName, "John");

            Assert.Equal(customer1.Orders.Count, 2);
            Assert.True(customer1.Orders.Any(o => o.OrderId == 10));
            Assert.Equal(customer1.Orders.First(o => o.OrderId == 10).OrderValue, 100);
            Assert.Equal(customer1.Orders.First(o => o.OrderId == 10).Products.Count, 1);
            Assert.Equal(customer1.Orders.First(o => o.OrderId == 10).Products.First().ProductId, 101);
            Assert.Equal(customer1.Orders.First(o => o.OrderId == 10).Products.First().Name, "Pizza Mozarella");

            Assert.True(customer1.Orders.Any(o => o.OrderId == 11));
            Assert.Equal(customer1.Orders.First(o => o.OrderId == 11).OrderValue, 101);
            Assert.Equal(customer1.Orders.First(o => o.OrderId == 11).Products.Count, 1);
            Assert.Equal(customer1.Orders.First(o => o.OrderId == 11).Products.First().ProductId, 102);
            Assert.Equal(customer1.Orders.First(o => o.OrderId == 11).Products.First().Name, "Pizza Chelentano");

            Assert.True(customers.Any(c => c.CustomerId == 2));
            var customer2 = customers.First(c => c.CustomerId == 2);
            Assert.Equal(customer2.FirstName, "Jack");
            Assert.Equal(customer2.LastName, "Daniels");

            Assert.Equal(customer2.Orders.Count, 2);
            Assert.True(customer2.Orders.Any(o => o.OrderId == 12));
            Assert.Equal(customer2.Orders.First(o => o.OrderId == 12).OrderValue, 110);
            Assert.Equal(customer2.Orders.First(o => o.OrderId == 12).Products.Count, 1);
            Assert.True(customer2.Orders.First(o => o.OrderId == 12).Products.Any(p => p.ProductId == 103));
            Assert.Equal(customer2.Orders.First(o => o.OrderId == 12).Products.First(p => p.ProductId == 103).Name, "Sushi");

            Assert.True(customer2.Orders.Any(o => o.OrderId == 13));
            Assert.Equal(customer2.Orders.First(o => o.OrderId == 13).OrderValue, 111);
            Assert.Equal(customer2.Orders.First(o => o.OrderId == 13).Products.Count, 1);
            Assert.Equal(customer2.Orders.First(o => o.OrderId == 13).Products.First(p => p.ProductId == 104).Name, "Coca Cola");
        }
    }
}
