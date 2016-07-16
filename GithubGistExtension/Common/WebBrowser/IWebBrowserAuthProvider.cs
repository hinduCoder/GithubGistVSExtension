using System;
using System.Threading.Tasks;

namespace GithubGistExtension.Common
{
    public interface IWebBrowserAuthProvider
    {
        Task<string> GetResultFromQueryStringAsync(Uri url, string parameter);
        void OpenBrowser(Uri url);
        void CloseBrowser();
    }
}