using System;
using EnvDTE;

namespace GithubGistExtension.Common
{
    public class VSWebBrowserProvider : WebBrowserAuthProviderBase
    {
        private Window _browserWindow;
        private DTE DTE { get; }

        public VSWebBrowserProvider(DTE dte)
        {
            DTE = dte;
        }

        public override void OpenBrowser(Uri url)
        {
            _browserWindow = DTE.ItemOperations.Navigate(url.ToString(), vsNavigateOptions.vsNavigateOptionsNewWindow);
        }

        public override void CloseBrowser()
        {
            _browserWindow.Close();
        }
    }
}