using Prism.Mvvm;
using Reactive.Bindings;
using StampImages.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StampImages.App.WPF.ViewModels
{
    public class SquareStampPanelViewModel : StampPanelBaseViewModel
    {

        public ReactiveProperty<string> Text { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<int> FontSize { get; } = new ReactiveProperty<int>(30);

        public SquareStampPanelViewModel() : base()
        {

            Text.Subscribe(_ => RequestUpdateStampImage());
            FontSize.Subscribe(_ => RequestUpdateStampImage());
        }

        protected override BaseStamp NewStamp()
        {
            var stamp = new SquareStamp()
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
