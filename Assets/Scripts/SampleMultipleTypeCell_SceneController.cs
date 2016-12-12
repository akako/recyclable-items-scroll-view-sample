using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleMultipleTypeCell_SceneController : MonoBehaviour, IRecyclableItemsScrollContentDataProvider
{
    [SerializeField]
    RecyclableItemsScrollContent scrollContent;
    [SerializeField]
    Cell itemPrefab;
    [SerializeField]
    Cell2 item2Prefab;

    List<string> data = new List<string>();
    List<Cell> cellPool = new List<Cell>();
    List<Cell2> cell2Pool = new List<Cell2>();

    void Start()
    {
        // データを準備（とりあえず1000個！）
        for (var i = 1; i <= 1000; i++)
        {
            // 2種類の文字列をてきとーに選択
            if (Random.Range(0, 5) > 0)
            {
                data.Add("Cell");
            }
            else
            {
                data.Add("Cell2");
            }
        }

        // リストビューのコンテンツをイニシャライズ
        scrollContent.Initialize(this);
    }

    #region IRecyclableItemsScrollContentDataProvider methods.

    public int DataCount { get { return data.Count; } }

    public float GetItemScale(int index)
    {
        GameObject cell = null;
        if ("Cell" == data[index])
        {
            cell = itemPrefab.gameObject;
        }
        else
        {
            cell = item2Prefab.gameObject;
        }
        return cell.GetComponent<RectTransform>().sizeDelta.y;
    }

    public RectTransform GetItem(int index, RectTransform recyclableItem)
    {
        if ("Cell" == data[index])
        {
            if (null != recyclableItem && null == recyclableItem.GetComponent<Cell>())
            {
                // 渡されてきたitemが別種のセルだった場合は、退避させておく
                cell2Pool.Add(recyclableItem.GetComponent<Cell2>());
                recyclableItem = null;
            }
            if (null == recyclableItem)
            {
                if (cellPool.Count > 0)
                {
                    // プールしているセルがあるなら使う
                    recyclableItem = cellPool[0].GetComponent<RectTransform>();
                    cellPool.RemoveAt(0);
                }
                else
                {
                    // 無ければ生成
                    recyclableItem = Instantiate(itemPrefab).GetComponent<RectTransform>();
                }
            }
            recyclableItem.GetComponent<Cell>().text.text = data[index];
        }
        else
        {
            if (null != recyclableItem && null == recyclableItem.GetComponent<Cell2>())
            {
                // 渡されてきたitemが別種のセルだった場合は、退避させておく
                cellPool.Add(recyclableItem.GetComponent<Cell>());
                recyclableItem = null;
            }
            if (null == recyclableItem)
            {
                if (cell2Pool.Count > 0)
                {
                    // プールしているセルがあるなら使う
                    recyclableItem = cell2Pool[0].GetComponent<RectTransform>();
                    cell2Pool.RemoveAt(0);
                }
                else
                {
                    // 無ければ生成
                    recyclableItem = Instantiate(item2Prefab).GetComponent<RectTransform>();
                }
            }
            recyclableItem.GetComponent<Cell2>().textForCell2.text = data[index];
        }

        return recyclableItem;
    }

    #endregion
}
