using NatTraversalTest.MainModule;
using NatTraversalTest.MainModule.Views;
using NatTraversalTest.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace NatTraversalTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(typeof(MainModule.MainModule));
        }
    }
}
