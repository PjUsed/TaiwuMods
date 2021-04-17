using GameData;
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
        public int[] GoodPages { get; set; }

        /// <summary>
        /// 手抄数量
        /// </summary>
        public int[] BadPages { get; set; }

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
            var checkBag = StorageCheck.Settings.CheckBag.Value;
            var checkWarehouse = StorageCheck.Settings.CheckWarehouse.Value;
            var showBookInfo = StorageCheck.Settings.ShowBookInfo.Value;

            #region 判断物品类型及功法技艺书的修习进度

            // Key, 大于0时为书籍
            var bookKey = int.Parse(DateFile.instance.GetItemDate(itemId, 32));
            Key = bookKey;
            if(bookKey > 0)
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

            // 物品对应的固定Id，非物品唯一Id
            // 400000~499999为技艺书
            // 500000~699999为功法书真传
            // 700000~899999为功法书手抄，真传+200000
            var itemKey = DateFile.instance.GetItemDate(itemId, 999);
            var itemKeyInt = int.Parse(itemKey);
            if(itemKeyInt >= 700000 && itemKeyInt <= 899999)
            {
                itemKeyInt -= 200000;
                itemKey = itemKeyInt.ToString();
            }
            StorageCheck.ModLogger.LogMessage($"itemKey: {itemKey}");
            if (int.Parse(itemKey) > 0)
            {
                GoodPages = new int[10];
                BadPages = new int[10];
                // 物品是否可堆叠
                var stackable = int.Parse(DateFile.instance.GetItemDate(itemId, 6)) != 0;
                if (checkBag)
                {
                    var keys = DateFile.instance.actorItemsDate[mainActorId].Keys.ToList();
                    // 添加装备物品
                    var equips = new[]
                    {
                        int.Parse(DateFile.instance.GetActorDate(mainActorId, 308, false)),
                        int.Parse(DateFile.instance.GetActorDate(mainActorId, 309, false)),
                        int.Parse(DateFile.instance.GetActorDate(mainActorId, 310, false)),
                    };
                    keys.AddRange(equips.Where(i => i == itemId));
                    foreach (var key in keys)
                    {
                        if (DateFile.instance.GetItemDate(key, 999) == itemKey)
                        {
                            BagCount += stackable ? DateFile.instance.actorItemsDate[mainActorId][key] : 1;
                            BagAvailableUseTimes += int.Parse((Items.GetItem(key) != null) ? DateFile.instance.GetItemDate(key, 901) : DateFile.instance.GetItemDate(key, 902));
                            BagTotalUseTimes += int.Parse((Items.GetItem(key) != null) ? Items.GetItemProperty(key, 902) : DateFile.instance.GetItemDate(key, 902));
                            if (showBookInfo && ItemType != ItemType.Other)
                            {
                                var bookPages = DateFile.instance.GetBookPage(key);
                                var isGoodBook = int.Parse(DateFile.instance.GetItemDate(key, 35)) == 0;
                                for (int i = 0; i < bookPages.Length; i++)
                                {
                                    if (bookPages[i] == 0)
                                    {
                                        continue;
                                    }
                                    if(ItemType == ItemType.SkillBook || isGoodBook)
                                    {
                                        GoodPages[i]++;
                                    }
                                    else
                                    {
                                        BadPages[i]++;
                                    }
                                }
                            }
                        }
                    }
                }
                if (checkWarehouse)
                {
                    foreach (var key in DateFile.instance.actorItemsDate[-999].Keys)
                    {
                        if (DateFile.instance.GetItemDate(key, 999) == itemKey)
                        {
                            WarehouseCount += stackable ? DateFile.instance.actorItemsDate[-999][key] : 1;
                            WarehouseAvailableUseTimes += int.Parse((Items.GetItem(key) != null) ? DateFile.instance.GetItemDate(key, 901) : DateFile.instance.GetItemDate(key, 902));
                            WarehouseTotalUseTimes += int.Parse((Items.GetItem(key) != null) ? Items.GetItemProperty(key, 902) : DateFile.instance.GetItemDate(key, 902));
                            if (showBookInfo && ItemType != ItemType.Other)
                            {
                                var bookPages = DateFile.instance.GetBookPage(key);
                                var isGoodBook = int.Parse(DateFile.instance.GetItemDate(key, 35)) == 0;
                                for (int i = 0; i < bookPages.Length; i++)
                                {
                                    if (bookPages[i] == 0)
                                    {
                                        continue;
                                    }
                                    if (ItemType == ItemType.SkillBook || isGoodBook)
                                    {
                                        GoodPages[i]++;
                                    }
                                    else
                                    {
                                        BadPages[i]++;
                                    }
                                }
                            }
                        }
                    }
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
        public static ItemInfo Get(int itemId) =>_currentItem?.Id == itemId ? _currentItem : new ItemInfo(itemId);

        /// <summary>
        /// 设置变更，当前物品信息重置
        /// </summary>
        public static void ResetCurrentItem()
        {
            _currentItem = null;
        }

        #endregion 公共方法
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
