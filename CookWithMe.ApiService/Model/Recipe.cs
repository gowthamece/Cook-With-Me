using System.ComponentModel.DataAnnotations;

namespace CookWithMe.ApiService.Model
{
    public class Recipe
    {
       
        public string? Ingredients { get; set; }
        public string[] SelectedType { get; set; } = new string[] { "bf", "lu" };
    }
}
