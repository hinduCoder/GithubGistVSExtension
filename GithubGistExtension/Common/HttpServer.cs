using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.PlatformUI;

namespace GithubGistExtension.Common
{
    public class HttpServer : IDisposable
    {
        private Uri _uri;
        private HttpListener _listener;

        public HttpServer(string uri) : this(new Uri(uri))
        {
        }

        public HttpServer(Uri uri)
        {
            _uri = uri;
            _listener = new HttpListener {Prefixes = {uri.AbsoluteUri}};
        }

        public void Listen(Action beforeCallback, Action<HttpListenerContext> callback)
        {
            if (callback == null)
                return;
            _listener.Start();
            beforeCallback?.Invoke();
            callback(_listener.GetContext());
            _listener.Stop();
        }

        public Task ListenAsync(Action beforeCallback, Action<HttpListenerContext> callback)
        {
            return Task.Run(() => Listen(beforeCallback, callback));
        }
        public Task ListenAsync(Action<HttpListenerContext> callback)
        {
            return ListenAsync(null, callback);
        }

        public void Dispose()
        {
            ((IDisposable)_listener).Dispose();
        }
    }
}
