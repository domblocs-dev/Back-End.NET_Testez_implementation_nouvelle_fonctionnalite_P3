using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Resources;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {

        private readonly ResourceManager _resourceManager = 
            new ResourceManager("P3AddNewFunctionalityDotNetCore.Resources.Models.Services.ProductService", 
                typeof(P3AddNewFunctionalityDotNetCore.Resources.Models.Services.ProductService).Assembly);

        /// <summary>
        ///  Vérifier qu'un produit ne peux pas être créé sans un nom, et que le message d'erreur retourné est correct
        /// </summary>

        [Fact]
        public void CheckProductViewModel_DoitdRenvoyerError_SiNameIsMissing()
        {
            // Arrange
            ProductViewModel newProduct = GetCorrectProductViewModel();
            newProduct.Name = null;
            var validationContext = new ValidationContext(newProduct);
            var validationResults = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(newProduct, validationContext, validationResults, validateAllProperties: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.ErrorMessage == GetResourceMessage("MissingName"));
        }


        /// <summary>
        /// Vérifier qu'un produit ne peux pas être créé sans un prix
        /// </summary>

        [Fact]
        public void CheckProductViewModel_DoitdRenvoyerError_SiPriceIsMissing()
        {
            // Arrange
            ProductViewModel newProduct = GetCorrectProductViewModel();
            newProduct.Price = null;
            var validationContext = new ValidationContext(newProduct);
            var validationResults = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(newProduct, validationContext, validationResults, validateAllProperties: true);


            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.ErrorMessage == GetResourceMessage("MissingPrice"));

        }





        // TODO write test methods to ensure a correct coverage of all possibilities


        /// <summary>
        /// Get a product with all its properties correctly set
        /// <returns>a valid fully initialized ProductViewModel</returns>
        /// </summary>
        private ProductViewModel GetCorrectProductViewModel()
        {
            return new ProductViewModel
            {
                Id = 1,
                Description = "Description du produit",
                Name = "Nom du produit",
                Details = "Details",
                Stock = 5.ToString(),
                Price = 22.20.ToString()
            };
        }

        /// <summary>
        /// Retrieves the localized string associated with the specified resource name.
        /// </summary>
        /// <param name="stringResource">The name of the string resource to retrieve. Cannot be null or empty.</param>
        /// <returns>The localized string corresponding to the specified resource name, or null if the resource is not found.</returns>
        private string GetResourceMessage(string stringResource)
        {

            return _resourceManager.GetString(stringResource);
        }


        }
}