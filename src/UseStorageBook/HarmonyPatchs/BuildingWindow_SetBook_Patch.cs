﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UseStorageBook.Extensions;
using UseStorageBook.Models;

namespace UseStorageBook.HarmonyPatchs
{
    [HarmonyPatch(typeof(BuildingWindow), "SetBook")]
    public class BuildingWindow_SetBook_Patch
    {
        /// <summary>
        /// 重写SetBook方法
        /// </summary>
        private static bool Prefix()
        {
            if (!UseStorageBook.IsEnable)
                return true;

            UseStorageBook.ModLogger.LogMessage("Load books...");
            var books = Book.GetAllBooks().Where(x => x.IsMatch(UseStorageBook.Settings)).ToList();

            SetBook(books);
            return false;
        }

        #region 书籍相关方法

        /// <summary>
        /// 设置书籍, 用于替换原始方法
        /// </summary>
        private static void SetBook(List<Book> books)
        {
            var studySkillTyp = BuildingWindow.instance.studySkillTyp;

            //Traverse.Create(BuildingWindow.instance).Method("RemoveBook");
            ReflectionMethods.Invoke(BuildingWindow.instance, "RemoveBook");

            // 复制原始代码(部分修改)
            List<int> itemSort = DateFile.instance.GetItemSort(books.ConvertAll(x => x.Id));
            for (int j = 0; j < itemSort.Count; j++)
            {
                int num3 = itemSort[j];
                if (int.Parse(DateFile.instance.GetItemDate(num3, 31)) != studySkillTyp)
                {
                    continue;
                }

                GameObject gameObject = Object.Instantiate(BuildingWindow.instance.bookIcon, Vector3.zero, Quaternion.identity);
                gameObject.name = "Item," + num3;
                gameObject.transform.SetParent(BuildingWindow.instance.bookHolder, worldPositionStays: false);
                gameObject.GetComponent<Toggle>().group = BuildingWindow.instance.bookHolder.GetComponent<ToggleGroup>();
                Image component = gameObject.transform.Find("ItemBack").GetComponent<Image>();
                SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(component, "itemBackSprites", int.Parse(DateFile.instance.GetItemDate(num3, 4)));
                component.color = DateFile.instance.LevelColor(int.Parse(DateFile.instance.GetItemDate(num3, 8)));
                GameObject gameObject2 = gameObject.transform.Find("ItemIcon").gameObject;
                gameObject2.name = "ItemIcon," + num3;
                SingletonObject.getInstance<DynamicSetSprite>().SetImageSprite(gameObject2.GetComponent<Image>(), "itemSprites", int.Parse(DateFile.instance.GetItemDate(num3, 98)));
                int num4 = int.Parse(DateFile.instance.GetItemDate(num3, 901));
                int num5 = int.Parse(DateFile.instance.GetItemDate(num3, 902));
                gameObject.transform.Find("ItemHpText").GetComponent<Text>().text = $"{DateFile.instance.Color3(num4, num5)}{num4}</color>/{num5}";
                int[] bookPage = DateFile.instance.GetBookPage(num3);
                Transform transform = gameObject.transform.Find("PageBack");
                for (int k = 0; k < transform.childCount; k++)
                {
                    if (bookPage[k] == 1)
                    {
                        transform.GetChild(k).GetComponent<Image>().color = new Color(0.392156869f, 0.784313738f, 0f, 1f);
                    }
                }
            }
        }

        #endregion
    }
}
