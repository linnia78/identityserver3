using IdentityModel.Client;
using MvcClient.Library.Utility;
using Newtonsoft.Json.Linq;
using Shared;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcClient.Controllers
{
    public class AccessWebapiController : Controller
    {
        //calls webapi's identity endpoint using the requested access token
        private async Task<string> CallApi(string token)
        {
            var client = new HttpClient();
            client.SetBearerToken(token);

            var json = await client.GetStringAsync(IdentityServerSetting.WEB_API_RESOURCE_URL);
            return JArray.Parse(json).ToString();
        }

        //get token from identityserver
        private async Task<TokenResponse> GetTokenAsync()
        {
            var client = new TokenClient(
                IdentityServerSetting.IDENTITY_SERVER_TOKEN_URL,
                IdentityServerSetting.WEB_API_CLIENT_ID,
                IdentityServerSetting.WEB_API_CLIENT_SECRET);

            return await client.RequestClientCredentialsAsync("siteApi");
        }

        //call webapi on behalf of the mvcclient
        public async Task<ActionResult> ClientCredentials()
        {
            var response = await GetTokenAsync();
            var result = await CallApi(response.AccessToken);

            ViewBag.Json = result;
            return View("ApiResult");
        }

        //call webapi on behalf of the user
        public async Task<ActionResult> UserCredentials()
        {
            var token = HttpUtility.GetAccessToken();
            var result = await CallApi(token);

            ViewBag.Json = result;
            return View("ApiResult");
        }
    }
}