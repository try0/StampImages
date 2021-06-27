using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using StampImages.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using Media = System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;

namespace StampImages.App.WPF.ViewModels
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {

        public static ReactiveProperty<StampPanelBaseViewModel> CurrentTabViewModel { get; }
            = new ReactiveProperty<StampPanelBaseViewModel>();


        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>("StampImages");


        private ThreeAreaCircularStampViewModel vm1 = new ThreeAreaCircularStampViewModel();
        private SquareStampPanelViewModel vm2 = new SquareStampPanelViewModel();

        public ReactiveProperty<bool> IsSelectedThreeAreaStamp { get; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> IsSelectedSquareStamp { get; } = new ReactiveProperty<bool>(false);



        public MainWindowViewModel()
        {
            IsSelectedThreeAreaStamp.Subscribe(val =>
            {
                if (val)
                {
                    CurrentTabViewModel.Value = vm1;
                    CurrentTabViewModel.Value.RequestUpdateStampImage();
                }
            });
            IsSelectedSquareStamp.Subscribe(val =>
            {
                if (val)
                {
                    CurrentTabViewModel.Value = vm2;
                    CurrentTabViewModel.Value.RequestUpdateStampImage();
                }
            });
        }
    }
}
