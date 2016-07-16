using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ninject;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace GithubGistExtension.Common
{
    public class GithubGistApi
    {
        private string _token;
        private string _clientId = "f12075cc0610132510bf"; //TODO config file
        private string _clientSecret = "d8352b2eb67171a848893ce40f0053d22f5ab6f4";
        private Uri _redirectUrl = new Uri("http://localhost:50001");
        private RestClient _client;
        [Inject]
        private IWebBrowserAuthProvider BrowserAuthProvider { get; set; }

        public GithubGistApi()
        {
            _client = new RestClient("https://api.github.com");
            _client.AddHandler("application/json", new JsonDeserializer());
        }

        public GithubGistApi(string token)
        {
            _token = token;
        }

        public async Task AuthorizeAsync()
        {
            if (_token != null)
                return;
            var uriBuilder = new UriBuilder("https://github.com/login/oauth/authorize")
            {
                Query = $"client_id={_clientId}&redirect_url={_redirectUrl}&scope={"gist"}"
            };

            var code = await GetCodeFromBrowser(uriBuilder);
            var restClient = new RestClient("https://github.com/login/oauth/access_token");
            restClient.AddHandler("application/json", new JsonDeserializer());
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(new AuthRequest
            {
                client_id = _clientId,
                client_secret = _clientSecret,
                code = code,
                redirect_uri = _redirectUrl.ToString()
            });
            var response = await restClient.ExecuteTaskAsync<AuthResponse>(request);
            _token = response.Data.AccessToken;
            _client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_token, "token");
        }

        private async Task<string> GetCodeFromBrowser(UriBuilder uriBuilder)
        {
            BrowserAuthProvider.OpenBrowser(uriBuilder.Uri);
            var code = await BrowserAuthProvider.GetResultFromQueryStringAsync(_redirectUrl, "code");
            BrowserAuthProvider.CloseBrowser();
            return code;
        }

        private struct AuthRequest
        {
            public string client_id { get; set; } 
            public string client_secret { get; set; }
            public string code { get; set; }
            public string redirect_uri { get; set; }
        }
        private struct AuthResponse
        {
             public String AccessToken { get; set; }
        }

        public async Task<string> CreateGistAsync(bool secret, params GistFile[] files)
        {
            var request = new RestRequest("gists", Method.POST);
            var filesObject = new JsonObject();
            foreach (var file in files)
            {
                filesObject.Add(file.Name, new JsonObject { new KeyValuePair<string, object>("content", file.Text) });
            }
            request.AddJsonBody(new
            {
                @public = !secret,
                files = filesObject
            });
            var result = await _client.ExecuteTaskAsync<CreateGistResponse>(request);

            return result.Data.HtmlUrl;
        }

        private struct CreateGistResponse
        {
            public string HtmlUrl { get; set; }
        }
        public Task<string> CreateGistAsync(bool secret, string fileName, string text)
        {
            return CreateGistAsync(secret, new GistFile(fileName, text));
        }

        private void CheckAuthorization()
        {
            if (_token == null)
                throw new InvalidOperationException("Not authorized");
        }
    }

    public struct GistFile
    {
        public GistFile(string name, string text)
        {
            Name = name;
            Text = text;
        }

        public string Name { get; set; }
        public string Text { get; set; }
    }
}