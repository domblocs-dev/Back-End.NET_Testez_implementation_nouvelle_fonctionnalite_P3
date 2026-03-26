using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        public string Stock { get; set; }

        public string Price { get; set; }
    }
}
