# StampImages

<!-- ![stamp_images](https://user-images.githubusercontent.com/17096601/125440678-31126fea-f356-4262-8fee-485c5ac01ace.png) -->

印鑑画像 日付印 データネーム印 職印 スタンプ はんこ ただの画像  
WPF/Blazor/VSTO 画像ファイル 勉強用


## [StampImages.Core](https://github.com/try0/StampImages/tree/main/src/StampImages.Core)  

[![Nuget](https://img.shields.io/nuget/v/StampImages.Core)](https://www.nuget.org/packages/StampImages.Core/)

netstandard2.0  
[System.Drawing.Common](https://www.nuget.org/packages/System.Drawing.Common/)


--- 


![stamp-20210705221944](https://user-images.githubusercontent.com/17096601/124477676-3f0a0980-dddf-11eb-92ca-6b2e06e659a7.png)

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


--- 


![stamp-20210705222034](https://user-images.githubusercontent.com/17096601/124477687-429d9080-dddf-11eb-9e29-b7225389f8ce.png)

```C#
StampImageFactory stampImageFactory = new StampImageFactory(new StampImageFactoryConfig());
var stamp = new RectangleStamp
{
    Color = Color.DarkCyan,
    EdgeType = StampEdgeType.Double,
    Text = new StampText { Value = "承認", Size = 60 },
};
stamp.EffectTypes.Add(StampEffectType.NOISE);

using (stamp)
using (var bitmap = stampImageFactory.Create(stamp))
{
    bitmap.Save("./stamp_sq.png", ImageFormat.Png);
}
```


--- 


![stamp_images](https://user-images.githubusercontent.com/17096601/125439174-af5be80d-0eec-449b-b639-f57e2de5033c.png)
[Stardos Stencil](https://fonts.google.com/specimen/Stardos+Stencil) (OFL)

```C#
StampImageFactory stampImageFactory = new StampImageFactory();
var stamp = new RectangleStamp
{
    Size = new Size(680, 140),
    Color = ColorTranslator.FromHtml("#1f456e"),
    IsFillColor = true,
    EdgeType = StampEdgeType.Double,
    EdgeWidth = 5,
    EdgeRadius = 0,
    Text = new StampText { Value = "Stamp Images", Size = 70, FontFamily = new FontFamily("Stardos Stencil") },

};
stamp.EffectTypes.Add(StampEffectType.Grunge);

using (stamp)
using (var bitmap = stampImageFactory.Create(stamp))
{
    bitmap.Save("./stamp_images.png");
}
```


--- 

## [StampImages.App.WPF](https://github.com/try0/StampImages/tree/main/src/StampImages.App.WPF)

netcoreapp3.1  
WPF  
[Prism](https://github.com/PrismLibrary/Prism)  
[ReactiveProperty](https://github.com/runceel/ReactiveProperty)  
[MahApps.Metro](https://github.com/MahApps/MahApps.Metro)  



![StampImages App WPF](https://user-images.githubusercontent.com/17096601/124384844-1960ff80-dd0e-11eb-90a6-54da2271038a.gif)

<!-- [キャプチャー:ScreenToGif](https://github.com/NickeManarin/ScreenToGif) -->

--- 

## [StampImages.WebApp.Blazor](https://github.com/try0/StampImages/tree/main/src/StampImages.WebApp.Blazor)

netcoreapp3.1  
Blazor(Server-Side)  


[Index.razor](https://github.com/try0/StampImages/blob/main/src/StampImages.WebApp.Blazor/Pages/Index.razor)
![StampImages WebApp Blazor](https://user-images.githubusercontent.com/17096601/124589108-91f1c880-de94-11eb-8398-20e89e30ad91.gif)

--- 

## [StampImages.OfficeAddIn.Excel](https://github.com/try0/StampImages/tree/main/src/StampImages.OfficeAddIn.Excel)

![ExcelAddIn](https://user-images.githubusercontent.com/17096601/149352839-5f9375b2-5cfc-45de-bf7a-b0dde832f4f2.gif)



