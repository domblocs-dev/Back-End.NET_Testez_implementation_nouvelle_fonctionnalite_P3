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

public class ProductIntegrationTests
{

    private readonly IConfiguration _configuration;
    private readonly IStringLocalizer<ProductService> _localizerService;
    private const string ConnectionString = "Server=.;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true";

    [Fact]
    public async Task CreateProduct_3_SaveNewProduct_NewProductInInventory()
    {
        // Arrange
        // Création d'un DbContext EF Core qui pointe vers la base SQL Server
        P3Referential context = CreateDbContext();
        Product product = null;

        try
        {
            // Instanciation des service et repository utilisés
            LanguageService languageService = new LanguageService();
            Cart cart = new Cart();
            ProductRepository productRepository = new ProductRepository(context);
            OrderRepository orderRepository = new OrderRepository(context);
            ProductService productService = new ProductService(cart, productRepository, orderRepository, _localizerService);

            // Instanciation du contrôleur
            ProductController productController = new ProductController(productService, languageService);

            // Création d'un article à travers le formulaire
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Name = "Le produit a sauvegarder",
                Price = "9,99",
                Stock = "10",
                Description = "La description du produit",
                Details = "Les détails du produit"
            };

            // Détermination du nombre initial de produits
            int productsBefore = await context.Product.CountAsync();

            // Act
            productController.Create(productViewModel);

            // Assert
            Assert.Equal(productsBefore + 1, context.Product.Count());

            product = await context.Product
                .Where(p => p.Name == "Le produit a sauvegarder")
                .FirstOrDefaultAsync();
            Assert.NotNull(product);
        }
        finally
        {
            // Nettoyage de la base
            if (product != null)
                await CleanTestData(context, product);
        }



    }



    [Fact]
    public async Task DeleteProduct_4_DeleteNewProduct_NewProductNotInInventory()
    {
        // Arrange
        // Création d'un DbContext EF Core qui pointe vers la base SQL Server
        P3Referential context = CreateDbContext();

        // Instanciation des service et repository utilisés
        LanguageService languageService = new LanguageService();
        Cart cart = new Cart();
        ProductRepository productRepository = new ProductRepository(context);
        OrderRepository orderRepository = new OrderRepository(context);
        ProductService productService = new ProductService(cart, productRepository, orderRepository, _localizerService);

        // Instanciation du contrôleur
        ProductController productController = new ProductController(productService, languageService);

        // Création d'un article à travers le formulaire
        ProductViewModel productViewModel = new ProductViewModel()
        {
            Name = "Le produit a supprimer",
            Price = "9,99",
            Stock = "10",
            Description = "La description du produit",
            Details = "Les détails du produit"
        };

        // Détermination du nombre initial de produits
        int productsBefore = await context.Product.CountAsync();

        // Création du produit qui devra être supprimé
        productController.Create(productViewModel);
        var product = await context.Product
            .Where(p => p.Name == "Le produit a supprimer")
            .FirstOrDefaultAsync();

        // Act
        productController.DeleteProduct(product.Id);

        // Assert
        Assert.Equal(productsBefore, context.Product.Count());

        var productHereAgain = await context.Product
            .Where(p => p.Name == "Le produit a supprimer")
            .FirstOrDefaultAsync();
        Assert.Null(productHereAgain);
    }









    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    /// <returns></returns>
    /// 

    private P3Referential CreateDbContext()
    {
        var optionsContext = new DbContextOptionsBuilder<P3Referential>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new P3Referential(optionsContext, _configuration);
    }

    private async Task CleanTestData(P3Referential context, Product product)
    {
        // Suppression des OrderLine liées au produit du test
        var lines = context.OrderLine.Where(ol => ol.ProductId == product.Id);
        context.OrderLine.RemoveRange(lines);

        // Suppression des Order qui n'ont plus de lignes
        var emptyOrders = context.Order.Where(o => !context.OrderLine.Any(ol => ol.OrderId == o.Id));
        context.Order.RemoveRange(emptyOrders);

        // Suppression du produit du test
        context.Product.Remove(product);

        // Envoie de toutes les suppressions à SQL Server
        await context.SaveChangesAsync();
    }


}


