# StampImages
印鑑画像 日付印 職印 スタンプ ただの画像


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


<!-- ![image](https://user-images.githubusercontent.com/17096601/123366674-91fbe980-d5b3-11eb-9b77-f5f4064f9e82.png) -->
