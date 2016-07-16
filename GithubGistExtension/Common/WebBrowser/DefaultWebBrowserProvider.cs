using System;
using System.Diagnostics;

namespace GithubGistExtension.Common
{
    public class DefaultWebBrowserProvider : WebBrowserAuthProviderBase
    {
        public override void OpenBrowser(Uri url)
        {
            Process.Start(url.ToString());
        }

        public override void CloseBrowser()
        {
            
        }
    }
}