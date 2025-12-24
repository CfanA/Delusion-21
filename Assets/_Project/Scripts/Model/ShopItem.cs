using System;

namespace Model
{
    [Serializable]
    public class ShopItem
    {
        public JokerData JokerSource; // 商品本体
        public int Price;             // 售价
        public bool IsSold;           // 是否卖掉了

        public ShopItem(JokerData data)
        {
            JokerSource = data;
            // 默认价格等于配置表里的价格，也可以在这里加随机波动
            Price = data.Price > 0 ? data.Price : 5; 
            IsSold = false;
        }
    }
}