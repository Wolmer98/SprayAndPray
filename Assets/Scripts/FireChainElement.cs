using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FireChainElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static int LastHoveredFireChainIndex;

    public int FireChainIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        LastHoveredFireChainIndex = FireChainIndex;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LastHoveredFireChainIndex = -1;
    }
}
