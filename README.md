# StampImages
印鑑画像 日付印 職印 スタンプ ただの画像  
WPF/画像ファイル 勉強用


## StampImages.Core  

[![Nuget](https://img.shields.io/nuget/v/StampImages.Core)](https://www.nuget.org/packages/StampImages.Core/)

netstandard2.0  
[System.Drawing.Common](https://www.nuget.org/packages/System.Drawing.Common/)



```C#
StampImageFactory stampImageFactory = new StampImageFactory(new StampImageFactoryConfig());
var stamp = new ThreeAreaCircularStamp
{
    TopText = new StampText { Value = "所属部門", Size = 22 },
    MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Size = 30 },
    BottomText = new StampText { Value = "ユーザー名", Size = 25 }
};

using (stamp)
{
    stampImageFactory.Save(stamp, "./stamp.png");
}
```
![inkan_128](https://user-images.githubusercontent.com/17096601/123622146-df43b980-d846-11eb-9613-b4641b14fd77.png)


```C#
StampImageFactory stampImageFactory = new StampImageFactory(new StampImageFactoryConfig());
var stamp = new RectangleStamp
{
    Color = Color.DarkCyan,
    EdgeType = StampEdgeType.DOUBLE,
    Text = new StampText { Value = "承認", Size = 60 },
};
stamp.EffectTypes.Add(StampEffectType.NOISE);

using (stamp)
using (var bitmap = stampImageFactory.Create(stamp))
{
    bitmap.Save("./stamp_sq.png", ImageFormat.Png);
}
```

![inkan_sq_256](https://user-images.githubusercontent.com/17096601/124340915-b5e2af00-dbf3-11eb-9983-e359d25247f6.png)


## StampImages.App.WPF

netcoreapp3.1  
WPF  
[Prism](https://github.com/PrismLibrary/Prism)  
[ReactiveProperty](https://github.com/runceel/ReactiveProperty)  
[MahApps.Metro](https://github.com/MahApps/MahApps.Metro)  



![StampImages App WPF](https://user-images.githubusercontent.com/17096601/124350613-830adc00-dc30-11eb-9619-f3c16feaa3ab.gif)

<!-- [キャプチャー:ScreenToGif](https://github.com/NickeManarin/ScreenToGif) -->

