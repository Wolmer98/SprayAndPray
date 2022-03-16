using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FireChainSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum FireChainSlotType { Weapon, Modifier }

    public static int LastHoveredSlotIndex = -1;
    public int SlotIndex;

    public FireChainSlotType SlotType;

    [SerializeField] private Image m_backgroundImage;

    private void Start()
    {
        m_backgroundImage.color = SlotType == FireChainSlotType.Weapon ? Colors.WeaponColor : Colors.ModifierColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LastHoveredSlotIndex = SlotIndex;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LastHoveredSlotIndex = -1;
    }
}
