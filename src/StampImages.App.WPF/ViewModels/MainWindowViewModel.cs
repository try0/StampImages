using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using StampImages.Core;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;


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
        public CircularStampPanelViewModel VM3 { get; set; }

        public ReactiveProperty<bool> IsSelectedThreeAreaStamp { get; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> IsSelectedSquareStamp { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsSelectedCircularStamp { get; } = new ReactiveProperty<bool>(false);

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="vm1"></param>
        /// <param name="vm2"></param>
        public MainWindowViewModel(
            ThreeAreaCircularStampViewModel vm1, 
            SquareStampPanelViewModel vm2,
            CircularStampPanelViewModel vm3)
        {
            VM1 = vm1;
            VM2 = vm2;
            VM3 = vm3;
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
            IsSelectedCircularStamp.Subscribe(val =>
            {
                if (val)
                {
                    CurrentTabViewModel.Value = VM3;
                    CurrentTabViewModel.Value.RequestUpdateStampImage();
                }
            });
        }
    }
}
