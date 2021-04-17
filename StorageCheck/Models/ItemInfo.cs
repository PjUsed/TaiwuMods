using GameData;
using System.Collections.Generic;
using System.Linq;

namespace StorageCheck.Models
{
    /// <summary>
    /// 物品信息
    /// </summary>
    public class ItemInfo
    {
        #region 私有字段

        /// <summary>
        /// 当前物品
        /// </summary>
        private static ItemInfo _currentItem;

        #endregion 私有字段

        #region 公共属性

        /// <summary>
        /// 物品Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 背包(包括装备栏)数量
        /// </summary>
        public int BagCount { get; set; }

        /// <summary>
        /// 背包(包括装备栏)总使用次数（总耐久）
        /// </summary>
        public int BagTotalUseTimes { get; set; }

        /// <summary>
        /// 背包(包括装备栏)可使用次数（剩余耐久）
        /// </summary>
        public int BagAvailableUseTimes { get; set; }

        /// <summary>
        /// 背包(包括装备栏)数量
        /// </summary>
        public int WarehouseCount { get; set; }

        /// <summary>
        /// 背包(包括装备栏)总使用次数（总耐久）
        /// </summary>
        public int WarehouseTotalUseTimes { get; set; }

        /// <summary>
        /// 背包(包括装备栏)可使用次数（剩余耐久）
        /// </summary>
        public int WarehouseAvailableUseTimes { get; set; }

        /// <summary>
        /// 物品类型：0 => 其他; 1 => 功法书籍; 2 => 技艺书籍
        /// </summary>
        public ItemType ItemType { get; set; }

        /// <summary>
        /// 书本每页状态
        /// </summary>
        public int[] BookStatus { get; set; }

        /// <summary>
        /// 修习进度
        /// </summary>
        public int[] LearnProcess { get; set; }

        /// <summary>
        /// 真传数量，技艺书视为真传
        /// </summary>
        public int[] GoodPages { get; set; } = new int[10];

        /// <summary>
        /// 手抄数量
        /// </summary>
        public int[] BadPages { get; set; } = new int[10];

        /// <summary>
        /// Key, 大于0时为书籍
        /// </summary>
        public int Key { get; set; }

        #endregion 公共属性

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="itemId">物品Id</param>
        public ItemInfo(int itemId)
        {
            Id = itemId;

            var mainActorId = DateFile.instance.MianActorID();

            #region 判断物品类型及功法技艺书的修习进度

            // Key, 大于0时为书籍
            var bookKey = int.Parse(DateFile.instance.GetItemDate(itemId, 32));
            Key = bookKey;
            if (bookKey > 0)
            {
                var bookType = int.Parse(DateFile.instance.GetItemDate(itemId, 31));
                BookStatus = DateFile.instance.GetBookPage(itemId);
                if (bookType == 17)
                {
                    ItemType = ItemType.GongfaBook;
                    LearnProcess = DateFile.instance.gongFaBookPages.ContainsKey(bookKey) ? DateFile.instance.gongFaBookPages[bookKey] : new int[10];
                }
                else
                {
                    ItemType = ItemType.SkillBook;
                    LearnProcess = DateFile.instance.skillBookPages.ContainsKey(bookKey) ? DateFile.instance.skillBookPages[bookKey] : new int[10];
                }
            }
            else
            {
                ItemType = ItemType.Other;
            }

            #endregion 判断物品类型及功法技艺书的修习进度

            #region 获取背包和仓库物品状态

            if (StorageCheck.Settings.CheckBag.Value)
            {
                // 包含宝物栏物品
                var keys = DateFile.instance.actorItemsDate[mainActorId].Keys
                    .Concat(new[]
                    {
                            int.Parse(DateFile.instance.GetActorDate(mainActorId, 308, false)),
                            int.Parse(DateFile.instance.GetActorDate(mainActorId, 309, false)),
                            int.Parse(DateFile.instance.GetActorDate(mainActorId, 310, false)),
                    });
                var (count, avail, total, good, bad) = GetItemsInfoMatchId(keys, itemId, ItemType);
                BagCount += count;
                BagAvailableUseTimes += avail;
                BagTotalUseTimes += total;
                for (int i = 0; i < 10; i++)
                {
                    GoodPages[i] += good[i];
                    BadPages[i] += bad[i];
                }
            }
            if (StorageCheck.Settings.CheckWarehouse.Value)
            {
                var (count, avail, total, good, bad) = GetItemsInfoMatchId(DateFile.instance.actorItemsDate[-999].Keys, itemId, ItemType);
                WarehouseCount += count;
                WarehouseAvailableUseTimes += avail;
                WarehouseTotalUseTimes += total;
                for (int i = 0; i < 10; i++)
                {
                    GoodPages[i] += good[i];
                    BadPages[i] += bad[i];
                }
            }

            #endregion 获取背包和仓库物品状态

            _currentItem = this;
        }

        #endregion 构造函数

        #region 公共方法

        /// <summary>
        /// 获取物品信息
        /// </summary>
        /// <param name="itemId">物品Id</param>
        /// <returns>物品信息</returns>
        public static ItemInfo Get(int itemId) => _currentItem != null && IsSameItem(_currentItem.Id, itemId) ? _currentItem : new ItemInfo(itemId);

        /// <summary>
        /// 设置变更，当前物品信息重置
        /// </summary>
        public static void ResetCurrentItem() => _currentItem = null;

        #endregion 公共方法

        #region 私有方法

        /// <summary>
        /// 是否为同一个物品
        /// <para>400000~499999为技艺书</para>
        /// <para>500000~699999为功法书真传</para>
        /// <para>700000~899999为功法书手抄，真传+200000</para>
        /// </summary>
        /// <param name="item1Key">物品1Key</param>
        /// <param name="item2Key">物品2Key</param>
        /// <returns>true为物品相同，false相反</returns>
        private static bool IsSameItem(string item1Key, string item2Key)
        {
            if(int.TryParse(item1Key, out var id1) && int.TryParse(item2Key, out var id2))
            {
                if (id1 == id2)
                    return true;
                else if (id1 >= 500000 && id1 < 700000 && id1 + 200000 == id2)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 是否为同一个物品
        /// </summary>
        /// <param name="item1Id">物品1Id</param>
        /// <param name="item2Id">物品2Id</param>
        /// <returns>true为物品相同，false相反</returns>
        private static bool IsSameItem(int item1Id, int item2Id) => IsSameItem(DateFile.instance.GetItemDate(item1Id, 999), DateFile.instance.GetItemDate(item2Id, 999));

        /// <summary>
        /// 获取物品集合中指定物品Id的所有物品信息
        /// </summary>
        /// <param name="items">物品集合</param>
        /// <param name="itemId">指定物品Id</param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        private static (int Count, int Available, int Total, int[] Good, int[] Bad) GetItemsInfoMatchId(IEnumerable<int> items, int itemId, ItemType itemType)
        {
            int count = 0, avail = 0, total = 0;
            int[] good = new int[10], bad = new int[10];
            // 物品对应的固定Id，非物品唯一Id
            var itemKey = DateFile.instance.GetItemDate(itemId, 999);
            if(int.Parse(itemKey) > 0)
            {
                // 物品是否可堆叠
                var stackable = int.Parse(DateFile.instance.GetItemDate(itemId, 6)) != 0;
                foreach (var key in items.Where(k => IsSameItem(DateFile.instance.GetItemDate(k, 999), itemKey)))
                {
                    count += stackable ? DateFile.instance.actorItemsDate[-999][key] : 1;
                    avail += int.Parse((Items.GetItem(key) != null) ? DateFile.instance.GetItemDate(key, 901) : DateFile.instance.GetItemDate(key, 902));
                    total += int.Parse((Items.GetItem(key) != null) ? Items.GetItemProperty(key, 902) : DateFile.instance.GetItemDate(key, 902));
                    if (StorageCheck.Settings.ShowBookInfo.Value && itemType != ItemType.Other)
                    {
                        var bookPages = DateFile.instance.GetBookPage(key);
                        var isGoodBook = int.Parse(DateFile.instance.GetItemDate(key, 35)) == 0;
                        for (int i = 0; i < bookPages.Length; i++)
                        {
                            if (bookPages[i] == 0)
                            {
                                continue;
                            }
                            if (itemType == ItemType.SkillBook || isGoodBook)
                            {
                                good[i]++;
                            }
                            else
                            {
                                bad[i]++;
                            }
                        }
                    }
                }
            }
            return (count, avail, total, good, bad);
        }

        #endregion 私有方法
    }

    /// <summary>
    /// 物品类型
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// 其他
        /// </summary>
        Other,

        /// <summary>
        /// 功法书籍
        /// </summary>
        GongfaBook,

        /// <summary>
        /// 技艺书籍
        /// </summary>
        SkillBook,
    }
}
