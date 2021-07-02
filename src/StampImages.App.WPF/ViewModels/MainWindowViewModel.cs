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
        /// <summary>
        /// 選択中タブのViewModel
        /// </summary>
        public static ReactiveProperty<StampPanelBaseViewModel> CurrentTabViewModel { get; }
            = new ReactiveProperty<StampPanelBaseViewModel>();


        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>("StampImages");

        public ThreeAreaCircularStampViewModel VM1 { get; set; }
        public SquareStampPanelViewModel VM2 { get; set; }

        public ReactiveProperty<bool> IsSelectedThreeAreaStamp { get; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> IsSelectedSquareStamp { get; } = new ReactiveProperty<bool>(false);


        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="vm1"></param>
        /// <param name="vm2"></param>
        public MainWindowViewModel(ThreeAreaCircularStampViewModel vm1, SquareStampPanelViewModel vm2)
        {
            VM1 = vm1;
            VM2 = vm2;
            IsSelectedThreeAreaStamp.Subscribe(val =>
            {
                if (val)
                {
                    CurrentTabViewModel.Value = VM1;
                    CurrentTabViewModel.Value.RequestUpdateStampImage();
                }
            });
            IsSelectedSquareStamp.Subscribe(val =>
            {
                if (val)
                {
                    CurrentTabViewModel.Value = VM2;
                    CurrentTabViewModel.Value.RequestUpdateStampImage();
                }
            });
        }
    }
}
