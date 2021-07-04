using Reactive.Bindings;
using StampImages.Core;
using System;
using System.Reactive.Linq;
using StampImages.App.WPF.Services;

namespace StampImages.App.WPF.ViewModels
{
    public class ThreeAreaCircularStampViewModel : StampPanelBaseViewModel
    {

        protected override Type StampType => typeof(ThreeAreaCircularStamp);

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
        public ThreeAreaCircularStampViewModel(IConfigurationService cs) : base(cs)
        {

            TopText.Subscribe(_ => RequestUpdateStampImage());
            MiddleText.Subscribe(_ => RequestUpdateStampImage());
            BottomText.Subscribe(_ => RequestUpdateStampImage());
            TopFontSize.Subscribe(_ => RequestUpdateStampImage());
            MiddleFontSize.Subscribe(_ => RequestUpdateStampImage());
            BottomFontSize.Subscribe(_ => RequestUpdateStampImage());

        }

        protected override void LoadStamp(BaseStamp stamp)
        {
            base.LoadStamp(stamp);
            ThreeAreaCircularStamp myStamp = (ThreeAreaCircularStamp)stamp;

            TopText.Value = myStamp.TopText.Value;
            MiddleText.Value = myStamp.MiddleText.Value;
            BottomText.Value = myStamp.BottomText.Value;
            TopFontSize.Value = myStamp.TopText.Size;
            MiddleFontSize.Value = myStamp.MiddleText.Size;
            BottomFontSize.Value = myStamp.BottomText.Size;

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
