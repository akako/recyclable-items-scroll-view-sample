using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSimple_SceneController : MonoBehaviour, IRecyclableItemsScrollContentDataProvider
{
    [SerializeField]
    RecyclableItemsScrollContent scrollContent;
    [SerializeField]
    Cell itemPrefab;

    List<string> data = new List<string>();

    void Start()
    {
        // データを準備（とりあえず1000個！）
        for (var i = 1; i <= 1000; i++)
        {
            data.Add(string.Format("Cell{0:0000}", i));
        }

        // リストビューのコンテンツをイニシャライズ
        scrollContent.Initialize(this);
    }

    #region IRecyclableItemsScrollContentDataProvider methods.

    public int DataCount { get { return data.Count; } }

    public float GetItemScale(int index)
    {
        // セルの高さを返す
        return itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    public RectTransform GetItem(int index, RectTransform recyclableItem)
    {
        // indexの位置にあるセルを読み込む処理

        if (null == recyclableItem)
        {
            // 初回ロード時はinstantateItemCountで指定した回数分だけitemがnullで来るので、ここで生成してあげる
            // 以降はitemが再利用されるため、Reflesh()しない限りnullは来ない
            recyclableItem = Instantiate(itemPrefab).GetComponent<RectTransform>();
        }

        // セルの内容書き換え
        recyclableItem.GetComponent<Cell>().text.text = data[index];

        return recyclableItem;
    }

    #endregion
}
