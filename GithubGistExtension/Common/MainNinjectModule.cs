using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Extensions.Conventions;

namespace GithubGistExtension.Common
{
    public class MainNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<DTE>().ToProvider(typeof (VSServiceProvider)).InSingletonScope();
            Kernel.Bind<IMenuCommandService>().ToProvider(typeof(VSServiceProvider)).InSingletonScope();
            Kernel.Bind(x => x.From(Assembly.GetExecutingAssembly()).IncludingNonePublicTypes().SelectAllClasses().InheritedFrom(typeof(ICommand)).BindAllInterfaces());

            Kernel.Bind<IWebBrowserAuthProvider>().To<VSWebBrowserProvider>();
            Kernel.Bind<GithubGistApi>().ToSelf();
        }
    }

    public class VSServiceProvider : IProvider
    {
        public object Create(IContext context)
        {
            IServiceProvider provider = context.Kernel.Get<Package>();
            return provider.GetService(context.Request.Service);
        }

        public Type Type { get; }
    }
}