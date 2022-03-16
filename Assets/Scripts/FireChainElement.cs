using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FireChainElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static int LastHoveredFireChainIndex = -1;

    public int FireChainIndex;
    public Transform SlotContainer;

    public void OnPointerEnter(PointerEventData eventData)
    {
        LastHoveredFireChainIndex = FireChainIndex;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LastHoveredFireChainIndex = -1;
    }
}
