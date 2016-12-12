using UnityEngine;
using System.Collections;

public interface IRecyclableItemsScrollContentDataProvider
{
    int DataCount { get; }

    float GetItemScale(int index);

    RectTransform GetItem(int index, RectTransform recyclableItem);
}
