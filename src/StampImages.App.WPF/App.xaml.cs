using Prism.Ioc;
using StampImages.App.WPF.Services;
using StampImages.App.WPF.ViewModels;
using StampImages.App.WPF.Views;
using System.Windows;

namespace StampImages.App.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }



        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IConfigurationService, ConfigurationService>();

            containerRegistry.Register<ThreeAreaCircularStampViewModel>();
            containerRegistry.Register<SquareStampPanelViewModel>(); 
            containerRegistry.Register<CircularStampPanelViewModel>();
        }
    }
}
