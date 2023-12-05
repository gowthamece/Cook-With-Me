using System.Net.Http;
using System.Text;
using static CookWithMe.Web.Components.Pages.LetsCook;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CookWithMe.Web
{
    public class OpenAIChatClient(HttpClient httpClient)
    {
        public async Task<string> GetRecipeAsync(Recipe recipe)
        {
          //  var body = Newtonsoft.Json.JsonConvert.SerializeObject(recipe);

          //  var content = new StringContent(body, Encoding.UTF8, "application/json");

            var result= await httpClient.PostAsJsonAsync("/recipe", recipe);
           var response= await result.Content.ReadAsStringAsync();
            return response.ToString();
        }
    }
}
