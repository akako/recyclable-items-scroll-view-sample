using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public class RecyclableItemsScrollContent : UIBehaviour
{
    public Padding padding;
    public int spacing;
    public Direction direction;

    [SerializeField, Range(0, 20)]
    int instantateItemCount = 7;

    List<RectTransform> items = new List<RectTransform>();
    float diffPreFramePosition = 0f;
    int currentItemNo = 0;
    List<float> positionCaches = new List<float>();
    IRecyclableItemsScrollContentDataProvider dataProvider;

    public enum Direction
    {
        Vertical,
        Horizontal,
    }

    RectTransform rectTransform;

    protected RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            return rectTransform;
        }
    }

    float AnchoredPosition
    {
        get
        {
            return direction == Direction.Vertical ? 
                -RectTransform.anchoredPosition.y :
                RectTransform.anchoredPosition.x;
        }
    }

    protected override void Start()
    {
        var scrollRect = GetComponentInParent<ScrollRect>();
        scrollRect.horizontal = direction == Direction.Horizontal;
        scrollRect.vertical = direction == Direction.Vertical;
        scrollRect.content = RectTransform;
    }

    void Update()
    {
        if (null == dataProvider)
        {
            return;
        }

        while (true)
        {
            var itemScale = GetItemScale(currentItemNo);
            if (itemScale <= 0 || AnchoredPosition - diffPreFramePosition >= -(itemScale + spacing) * 2)
            {
                break;
            }

            var item = items[0];
            items.RemoveAt(0);
            diffPreFramePosition -= itemScale + spacing;
            items.Add(GetItem(currentItemNo + instantateItemCount, item));

            currentItemNo++;
        }

        while (true)
        {
            var itemScale = GetItemScale(currentItemNo + instantateItemCount - 1);
            if (itemScale <= 0 || AnchoredPosition - diffPreFramePosition <= -(itemScale + spacing) * 1)
            {
                break;
            }

            var item = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);

            currentItemNo--;

            diffPreFramePosition += GetItemScale(currentItemNo) + spacing;
            items.Insert(0, GetItem(currentItemNo, item));
        }
    }

    public void Initialize(IRecyclableItemsScrollContentDataProvider dataProvider)
    {
        this.dataProvider = dataProvider;

        if (items.Count == 0)
        {
            for (var i = 0; i < instantateItemCount; i++)
            {
                items.Add(GetItem(i, null));
            }
        }
        else
        {
            positionCaches.Clear();
            for (var i = 0; i < instantateItemCount; i++)
            {
                var item = items[0];
                items.RemoveAt(0);
                items.Add(GetItem(currentItemNo + i, item));
            }
        }

        var rectTransform = GetComponent<RectTransform>();
        var delta = rectTransform.sizeDelta;
        if (direction == Direction.Vertical)
        {
            delta.y = padding.top + padding.bottom;
            for (var i = 0; i < dataProvider.DataCount; i++)
            {
                delta.y += GetItemScale(i) + spacing;
            }
        }
        else
        {
            delta.x = padding.left + padding.right;
            for (var i = 0; i < dataProvider.DataCount; i++)
            {
                delta.x += GetItemScale(i) + spacing;
            }
        }
        rectTransform.sizeDelta = delta;
    }

    float GetItemScale(int index)
    {
        if (null == dataProvider || dataProvider.DataCount == 0)
        {
            return 0;
        }
        return dataProvider.GetItemScale(Math.Max(0, Math.Min(index, dataProvider.DataCount - 1)));
    }

    RectTransform GetItem(int index, RectTransform recyclableItem)
    {
        if (null == dataProvider || index < 0 || dataProvider.DataCount <= index)
        {
            if (null != recyclableItem)
            {
                recyclableItem.gameObject.SetActive(false);
            }
            return recyclableItem;
        }
        var item = dataProvider.GetItem(index, recyclableItem);
        if (item != recyclableItem)
        {
            item.SetParent(transform, false);
        }
        item.anchoredPosition = GetPosition(index);
        item.gameObject.SetActive(true);
        return item;
    }

    public void Reflesh()
    {
        Initialize(dataProvider);
    }

    float GetPositionCache(int index)
    {
        for (var i = positionCaches.Count; i <= index; i++)
        {
            positionCaches.Add(i == 0 ? (direction == Direction.Vertical ? padding.top : padding.left) : (positionCaches[i - 1] + GetItemScale(i - 1) + spacing));
        }
        return positionCaches[index];
    }

    Vector2 GetPosition(int index)
    {
        if (index < 0)
        {
            return Vector2.zero;
        }
        return direction == Direction.Vertical ? new Vector2(0, -GetPositionCache(index)) : new Vector2(GetPositionCache(index), 0);
    }

    [System.Serializable]
    public class Padding
    {
        public int top = 0;
        public int right = 0;
        public int bottom = 0;
        public int left = 0;
    }
}
