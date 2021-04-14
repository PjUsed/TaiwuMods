using System.Collections.Generic;
using System.Linq;

namespace UseStorageBook.Models
{
    /// <summary>
    /// 书本
    /// </summary>
    internal class Book
    {
        #region 属性

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 来源：0背包，1仓库
        /// </summary>
        public int Source { get; set; }

        /// <summary>
        /// 品级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 物品类型
        /// <para>大于等于17为功法书籍</para>
        /// </summary>
        public int ItemType { get; set; }

        /// <summary>
        /// 阅读状态：0未读，1已读，2读完
        /// </summary>
        public int ReadStatus { get; set; }

        /// <summary>
        /// 类型：0真传，1手抄
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 功法类型
        /// </summary>
        public int GongFaType { get; set; }

        /// <summary>
        /// 门派
        /// </summary>
        public int Sect { get; set; }

        #endregion 属性

        #region 构造器

        /// <summary>
        /// 构造函数，初始化书本
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="source">来源：0背包，1仓库</param>
        public Book(int id, int source = 0)
        {
            var df = DateFile.instance;
            var itemId = int.Parse(df.GetItemDate(id, 999));
            var gongFaId = int.Parse(df.presetitemDate[itemId][32]);
            var readPages = BuildingWindow.instance.studySkillTyp >= 17
                ? (df.gongFaBookPages.ContainsKey(gongFaId) ? df.gongFaBookPages[gongFaId].Sum() : 0)
                : (df.skillBookPages.ContainsKey(gongFaId) ? df.skillBookPages[gongFaId].Sum() : 0);

            Id = id;
            Source = source;
            Level = int.Parse(df.presetitemDate[itemId][8]) - 1;
            ItemType = int.Parse(df.presetitemDate[itemId][31]);
            ReadStatus = readPages <= 0 ? 0 : readPages < 10 ? 1 : 2;

            if (BuildingWindow.instance.studySkillTyp >= 17)
            {
                Type = int.Parse(df.presetitemDate[itemId][35]) != 1 ? 0 : 1;
                GongFaType = int.Parse(df.gongFaDate[gongFaId][1]);
                Sect = int.Parse(df.gongFaDate[gongFaId][3]);
            }
        }

        #endregion 构造器

        #region 公共方法

        /// <summary>
        /// 是否符合设置
        /// </summary>
        /// <param name="settings">设置</param>
        /// <returns>true为符合，false相反</returns>
        public bool IsMatch(Settings settings)
        {
            // 背包 / 仓库
            if (!settings.Source.Value[Source])
            {
                return false;
            }

            // 品级
            if (!settings.Level.Value[Level])
            {
                return false;
            }

            // 阅读状态
            if (!settings.Status.Value[ReadStatus])
            {
                return false;
            }

            // 技艺书籍
            if (ItemType != 17)
                return true;

            #region 功法书籍

            // 真传 / 手抄
            if (!settings.Type.Value[Type])
            {
                return false;
            }

            // 功法类型
            if (!settings.GongFa.Value[GongFaType])
            {
                return false;
            }

            // 门派
            if (!settings.Sect.Value[Sect])
            {
                return false;
            }

            #endregion 功法书籍

            return true;
        }

        /// <summary>
        /// 获取所有书籍
        /// </summary>
        /// <returns>书籍列表</returns>
        public static List<Book> GetAllBooks()
        {
            var df = DateFile.instance;
            var mainId = df.MianActorID();
            var studySkillTyp = BuildingWindow.instance.studySkillTyp;
            var list = df.GetActorItems(mainId, 5).Keys
                .Where(x => int.Parse(df.GetItemDate(x, 31)) == studySkillTyp)
                .Select(x => new Book(x))
                .ToList();
            for (int i = 0; i < 3; i++)
            {
                var x = int.Parse(df.GetActorDate(mainId, 308 + i, true));
                if (x > 0 && int.Parse(df.GetItemDate(x, 31)) == studySkillTyp)
                    list.Add(new Book(x));
            }

            // 添加仓库书籍
            list.AddRange(df.GetActorItems(-999, 5, false).Keys
                .Where(x => int.Parse(df.GetItemDate(x, 31)) == studySkillTyp)
                .Select(x => new Book(x, 1)));

            return list;
        }

        #endregion 公共方法
    }
}
