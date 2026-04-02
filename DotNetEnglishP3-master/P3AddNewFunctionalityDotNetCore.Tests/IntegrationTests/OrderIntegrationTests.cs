using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests.IntegrationTests;


public class OrderIntegrationTests
{
    private readonly IConfiguration _configuration;
    private readonly IStringLocalizer<OrderController> _localizerOrderController;
    private readonly IStringLocalizer<ProductService> _localizerProductService;
    private const string ConnectionString = "Server=.;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true";

    [Fact]
    public async Task OrderSaveProduct_1_SaveOrder_DecreaseProductStock()
    {
        // => Arrange
        // Création d'un DbContext EF Core qui pointe vers la base SQL Server
        P3Referential context = CreateDbContext();
        Product product = null;


        try
        {
            // Instanciation des service et repository utilisés
            Cart cart = new Cart();
            OrderRepository orderRepository = new OrderRepository(context);
            ProductRepository productRepository = new ProductRepository(context);
            ProductService productService = new ProductService(cart, productRepository, orderRepository, _localizerProductService);
            OrderService orderService = new OrderService(cart, orderRepository, productService);
            LanguageService languageService = new LanguageService();

            // Instanciation des contrôleurs
            OrderController orderController = new OrderController(cart, orderService, _localizerOrderController);
            ProductController productController = new ProductController(productService, languageService);

            // Création d'un article de test dans l'inventaire
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Name = "Le produit a commander 1",
                Price = "9,99",
                Stock = "10",
                Description = "La description du produit",
                Details = "Les détails du produit"
            };
            productController.Create(productViewModel);

            // Recherche de l'article dans l'inventaire et récupération de sa quantité initiale
            product = await context.Product
                .Where(p => p.Name == "Le produit a commander 1")
                .FirstOrDefaultAsync();
            int productInitialQuantity = product.Quantity;

            // Ajout de l'article au panier
            cart.AddItem(product, 2);

            // Création d'une commande
            OrderViewModel orderViewModel = CreateOrderViewModel(product.Id, 2);

            // => Act : passage de la commande
            orderController.Index(orderViewModel);

            // => Assert
            var productFinalQuantity = await context.Product.FindAsync(product.Id);
            Assert.Equal(productInitialQuantity - 2, productFinalQuantity.Quantity);
        }
        finally
        {
            // Nettoyage de la base
            if (product != null)
                await CleanTestData(context, product);
        }


    }



    [Fact]
    public async Task OrderSaveProduct_2_SaveOrder_ClearCart()
    {
        // ==> Arrange
        // Création d'un DbContext EF Core qui pointe vers la base SQL Server
        P3Referential context = CreateDbContext();
        Product product = null;

        try
        {
            // Instanciation des service et repository utilisés
            Cart cart = new Cart();
            OrderRepository orderRepository = new OrderRepository(context);
            ProductRepository productRepository = new ProductRepository(context);
            ProductService productService = new ProductService(cart, productRepository, orderRepository, _localizerProductService);
            OrderService orderService = new OrderService(cart, orderRepository, productService);
            LanguageService languageService = new LanguageService();

            // Instanciation des contrôleurs
            OrderController orderController = new OrderController(cart, orderService, _localizerOrderController);
            ProductController productController = new ProductController(productService, languageService);

            // Création d'un article de test dans l'inventaire
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Name = "Le produit a commander 2",
                Price = "9,99",
                Stock = "10",
                Description = "La description du produit",
                Details = "Les détails du produit"
            };
            productController.Create(productViewModel);

            // Recheche de l'article dans l'inventaire
            product = await context.Product
                .Where(p => p.Name == "Le produit a commander 2")
                .FirstOrDefaultAsync();

            // Ajout de l'article au panier
            cart.AddItem(product, 2);

            // Création d'une commande
            OrderViewModel orderViewModel = CreateOrderViewModel(product.Id, 2);

            // ==> Act : passage de la commande
            orderController.Index(orderViewModel);

            // ==> Assert
            Assert.Empty(cart.Lines);
        }
        finally
        {
            // Nettoyage de la base
            if (product != null)
                await CleanTestData(context, product);
        }

    }





    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    /// <returns></returns>

    private P3Referential CreateDbContext()
    {
        var optionsContext = new DbContextOptionsBuilder<P3Referential>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new P3Referential(optionsContext, _configuration);
    }

    private OrderViewModel CreateOrderViewModel(int id, int quantity)
    {
        return new OrderViewModel
        {
            Lines = new List<CartLine>
            {
                new CartLine
                {
                    Product = new Product { Id = id },
                    Quantity = quantity
                }
            },
            Name = "Mon nom",
            Address = "Mon adresse",
            City = "Ma ville",
            Zip = "12345",
            Country = "Mon pays"
        };
    }

    private async Task CleanTestData(P3Referential context, Product product)
    {
        // Suppression des OrderLine liées au produit du test
        var lines = context.OrderLine.Where(ol => ol.ProductId == product.Id);
        context.OrderLine.RemoveRange(lines);

        // Suppression des Order liées à ces OrderLine
        var orders = context.Order.Where(o => context.OrderLine.Any(ol => ol.OrderId == o.Id && ol.ProductId == product.Id));
        context.Order.RemoveRange(orders);

        // Suppression du produit du test
        context.Product.Remove(product);

        // Envoie de toutes les suppressions à SQL Server
        await context.SaveChangesAsync();
    }

}