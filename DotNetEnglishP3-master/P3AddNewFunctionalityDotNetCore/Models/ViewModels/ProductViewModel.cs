using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using P3AddNewFunctionalityDotNetCore.Models.Validation;
using System.Globalization;
using ResourcesProductService = P3AddNewFunctionalityDotNetCore.Resources.Models.Services.ProductService;


namespace P3AddNewFunctionalityDotNetCore.Models.ViewModels
{
    public class ProductViewModel
    {
        [BindNever]
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "MissingName", ErrorMessageResourceType = typeof(ResourcesProductService), AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }

        [Required(ErrorMessageResourceName = "MissingQuantity", ErrorMessageResourceType = typeof(ResourcesProductService), AllowEmptyStrings = false)]
        [RegularExpression(@"^-?\d+$", ErrorMessageResourceName = "QuantityNotAnInteger", ErrorMessageResourceType = typeof(ResourcesProductService))]
        [Range(
            1,
            int.MaxValue,
            ErrorMessageResourceName = "QuantityNotGreaterThanZero",
            ErrorMessageResourceType = typeof(ResourcesProductService))]
        public string Stock { get; set; }

        [Required(ErrorMessageResourceName = "MissingPrice", ErrorMessageResourceType = typeof(ResourcesProductService), AllowEmptyStrings = false)]
        [DecimalByCultureAttribute(ErrorMessageResourceName = "PriceNotANumber", ErrorMessageResourceType = typeof(ResourcesProductService))]
        [NumberGreaterThanZero(ErrorMessageResourceName = "PriceNotGreaterThanZero", ErrorMessageResourceType = typeof(ResourcesProductService))]
        public string Price { get; set; }
    }
}
