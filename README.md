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

![stamp-20210705221944](https://user-images.githubusercontent.com/17096601/124477676-3f0a0980-dddf-11eb-92ca-6b2e06e659a7.png)

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

![stamp-20210705222034](https://user-images.githubusercontent.com/17096601/124477687-429d9080-dddf-11eb-9e29-b7225389f8ce.png)


## StampImages.App.WPF

netcoreapp3.1  
WPF  
[Prism](https://github.com/PrismLibrary/Prism)  
[ReactiveProperty](https://github.com/runceel/ReactiveProperty)  
[MahApps.Metro](https://github.com/MahApps/MahApps.Metro)  



![StampImages App WPF](https://user-images.githubusercontent.com/17096601/124384844-1960ff80-dd0e-11eb-90a6-54da2271038a.gif)

<!-- [キャプチャー:ScreenToGif](https://github.com/NickeManarin/ScreenToGif) -->


## StampImages.WebApp.Blazor

netcoreapp3.1  
Blazor(Server-Side)  

![image](https://user-images.githubusercontent.com/17096601/124499935-cc5a5780-ddf9-11eb-8eaf-8a212c09da1a.png)

