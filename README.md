# StampImages
印鑑画像 日付印 職印 スタンプ ただの画像


```C#
StampImageFactory stampImageFactory = new StampImageFactory(new StampImageFactoryConfig());
var stamp = new ThreeAreaCircularStamp
{
    TopText = new StampText { Value = "所属部門", Size = 22 },
    MiddleText = new StampText { Value = DateTime.Now.ToString("yyyy.MM.dd"), Size = 30 },
    BottomText = new StampText { Value = "ユーザー名", Size = 25 }
};
stampImageFactory.Save(stamp, "./stamp.png");
```
![inkan_128](https://user-images.githubusercontent.com/17096601/123622146-df43b980-d846-11eb-9613-b4641b14fd77.png)


```C#
StampImageFactory stampImageFactory = new StampImageFactory(new StampImageFactoryConfig());
var stamp = new SquareStamp
{
    EdgeType = StampEdgeType.DOUBLE,
    Text = new StampText { Value = "承認", Size = 60 },
};
stamp.EffectTypes.Add(StampEffectType.NOISE);

var bitmap = stampImageFactory.Create(stamp);
bitmap.Save("./stamp_sq.png");
```

![inkan_sq_256](https://user-images.githubusercontent.com/17096601/123621690-72302400-d846-11eb-96b2-f63a3a75174d.png)

<!-- ![image](https://user-images.githubusercontent.com/17096601/123366674-91fbe980-d5b3-11eb-9b77-f5f4064f9e82.png) -->
