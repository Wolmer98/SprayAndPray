using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FireChainSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static int LastHoveredFireChainSlotIndex;
    public int FireChainSlotIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        LastHoveredFireChainSlotIndex = FireChainSlotIndex;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LastHoveredFireChainSlotIndex = -1;
    }
}
