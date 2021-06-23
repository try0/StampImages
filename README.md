# StampImages
印鑑画像　日付印 ただの画像


```C#
StampImageFactory stampImageFactory = new StampImageFactory();
var stamp = new Stamp
{
    TopText = new StampText { Value = "所属部門", Font = StampText.GetDefaultFont(22) },
    MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Font = StampText.GetDefaultFont(30) },
    BottomText = new StampText { Value = "ユーザー名", Font = StampText.GetDefaultFont(25) }
};
stampImageFactory.Save(stamp, "./inkan.png");
```
![inkan_128](https://user-images.githubusercontent.com/17096601/122756682-b0bb6100-d2d1-11eb-9d28-512188c739f3.png)

<!-- ![image](https://user-images.githubusercontent.com/17096601/123105829-1430c480-d473-11eb-8879-8d0b5ca1d285.png) -->
