﻿using Reactive.Bindings;
using StampImages.App.WPF.Services;
using StampImages.Core;
using System;

namespace StampImages.App.WPF.ViewModels
{
    public class CircularStampPanelViewModel : StampPanelBaseViewModel
    {

        protected override Type StampType => typeof(CircularStamp);

        /// <summary>
        /// スタンプテキスト
        /// </summary>
        public ReactiveProperty<string> Text { get; } = new ReactiveProperty<string>();

        /// <summary>
        /// フォントサイズ
        /// </summary>
        public ReactiveProperty<float> FontSize { get; } = new ReactiveProperty<float>(70);



        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="cs"></param>
        public CircularStampPanelViewModel(IConfigurationService cs) : base(cs)
        {

            Text.Subscribe(_ => RequestUpdateStampImage());
            FontSize.Subscribe(_ => RequestUpdateStampImage());
        }

        protected override void LoadStamp(BaseStamp stamp)
        {
            base.LoadStamp(stamp);
            CircularStamp myStamp = (CircularStamp)stamp;

            Text.Value = myStamp.Text.Value;
            FontSize.Value = myStamp.Text.Size;

        }

        protected override BaseStamp NewStamp()
        {
            var stamp = new CircularStamp()
            {
                Text = new StampText()
                {
                    Value = Text.Value,
                    Size = FontSize.Value
                },

            };

            return stamp;
        }

        protected override void ExecuteClearCommand()
        {

            this.isInitialized = false;

            Text.Value = null;
            FontSize.Value = 30;

            base.ExecuteClearCommand();
        }
    }
}
