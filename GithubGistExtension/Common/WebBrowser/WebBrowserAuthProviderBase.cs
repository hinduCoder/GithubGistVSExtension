using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GithubGistExtension.Common
{
    public abstract class WebBrowserAuthProviderBase : IWebBrowserAuthProvider
    {
        public async Task<string> GetResultFromQueryStringAsync(Uri url, string parameterName)
        {
            var httpServer = new HttpServer(url);
            string result = null;
            await httpServer.ListenAsync(context => result = context.Request.QueryString[parameterName]);
            return result;
        }

        public abstract void OpenBrowser(Uri url);
        public abstract void CloseBrowser();
    }
}