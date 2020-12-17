using Hosted.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hosted.Client.Pages
{
    public partial class DynamicForm
    {
        Form form;
        internal static readonly Dictionary<string, string> ElementValues = new Dictionary<string, string>();

        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        IHttpClientFactory HttpClientFactory { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        internal string serverResponse;

        protected override async Task OnInitializedAsync()
        {
            var client = HttpClientFactory.CreateClient("PublicServerAPI");
            var st = await client.GetStringAsync("Form");
            form = JsonConvert.DeserializeObject<Form>(st, settings: new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        private async Task Submit()
        {
            try
            {
                // await Http.PostJsonAsync<string>("Form", ElementValues); // wouldn't allow getting response
                var strElementValues = JsonConvert.SerializeObject(ElementValues);
                var response = await Http.PostAsync("Form", new StringContent(strElementValues, encoding: Encoding.UTF8, mediaType: "application/json"));
                serverResponse = response.IsSuccessStatusCode ?
                    await response.Content.ReadAsStringAsync() :
                    response.StatusCode.ToString();
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }
}
