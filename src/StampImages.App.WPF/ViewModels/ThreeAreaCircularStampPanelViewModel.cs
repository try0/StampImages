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
    public class ThreeAreaCircularStampViewModel : StampPanelBaseViewModel
    {


        /// <summary>
        /// 上段テキスト
        /// </summary>
        public ReactiveProperty<string> TopText { get; } = new ReactiveProperty<string>();
        /// <summary>
        /// 中段テキスト
        /// </summary>
        public ReactiveProperty<string> MiddleText { get; } = new ReactiveProperty<string>(DateTime.Now.ToString("yyyy/MM/dd"));
        /// <summary>
        /// 下段テキスト
        /// </summary>
        public ReactiveProperty<string> BottomText { get; } = new ReactiveProperty<string>();

        /// <summary>
        /// 上段テキストサイズ
        /// </summary>
        public ReactiveProperty<double> TopFontSize { get; } = new ReactiveProperty<double>(27);
        /// <summary>
        /// 中段テキストサイズ
        /// </summary>
        public ReactiveProperty<double> MiddleFontSize { get; } = new ReactiveProperty<double>(27);
        /// <summary>
        /// 下段テキストサイズ
        /// </summary>
        public ReactiveProperty<double> BottomFontSize { get; } = new ReactiveProperty<double>(27);

      




        /// <summary>
        /// コンストラクター
        /// </summary>
        public ThreeAreaCircularStampViewModel() : base()
        {

            TopText.Subscribe(_ => RequestUpdateStampImage());
            MiddleText.Subscribe(_ => RequestUpdateStampImage());
            BottomText.Subscribe(_ => RequestUpdateStampImage());
            TopFontSize.Subscribe(_ => RequestUpdateStampImage());
            MiddleFontSize.Subscribe(_ => RequestUpdateStampImage());
            BottomFontSize.Subscribe(_ => RequestUpdateStampImage());

        }




        protected override void ExecuteClearCommand()
        {
            this.isInitialized = false;

            TopText.Value = null;
            MiddleText.Value = DateTime.Now.ToString("yyyy/MM/dd");
            BottomText.Value = null;

            TopFontSize.Value = 27;
            MiddleFontSize.Value = 27;
            BottomFontSize.Value = 27;

            base.ExecuteClearCommand();

        }

        protected override BaseStamp NewStamp()
        {
            var stamp = new ThreeAreaCircularStamp
            {
                TopText = new StampText
                {
                    Value = TopText.Value,
                    Size = (float)TopFontSize.Value
                },
                MiddleText = new StampText
                {
                    Value = MiddleText.Value,
                    Size = (float)MiddleFontSize.Value
                },
                BottomText = new StampText
                {
                    Value = BottomText.Value,
                    Size = (float)BottomFontSize.Value
                }
            };

            return stamp;
        }
    }
    
}
