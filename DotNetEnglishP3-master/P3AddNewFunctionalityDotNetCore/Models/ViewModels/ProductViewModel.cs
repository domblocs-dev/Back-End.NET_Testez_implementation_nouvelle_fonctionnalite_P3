using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using P3AddNewFunctionalityDotNetCore.Models.Validation;
using System.Globalization;

namespace P3AddNewFunctionalityDotNetCore.Models.ViewModels
{
    public class ProductViewModel
    {
        [BindNever]
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "MissingName", ErrorMessageResourceType = typeof(Resources.Models.Services.ProductService), AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }

        [Required(ErrorMessageResourceName = "MissingQuantity", ErrorMessageResourceType = typeof(Resources.Models.Services.ProductService), AllowEmptyStrings = false)]
        [RegularExpression(@"^-?\d+$", ErrorMessageResourceName = "QuantityNotAnInteger", ErrorMessageResourceType = typeof(Resources.Models.Services.ProductService))]
        public string Stock { get; set; }

        [Required(ErrorMessageResourceName = "MissingPrice", ErrorMessageResourceType = typeof(Resources.Models.Services.ProductService), AllowEmptyStrings = false)]
        [RegularExpression(@"^\d+(?:[.,]\d+)?$", ErrorMessageResourceName = "PriceNotANumber", ErrorMessageResourceType = typeof(Resources.Models.Services.ProductService))]
        [DecimalByCultureAttribute(ErrorMessageResourceName = "PriceNotANumber", ErrorMessageResourceType = typeof(Resources.Models.Services.ProductService))]
        [NumberGreaterThanZero(ErrorMessageResourceName = "PriceNotGreaterThanZero", ErrorMessageResourceType = typeof(Resources.Models.Services.ProductService))]
        public string Price { get; set; }
    }
}
