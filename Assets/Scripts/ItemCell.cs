using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemCell : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Item Item;
    [SerializeField] private Image m_image;

    private bool IsHoveringFireChainSlot => FireChainSlot.LastHoveredFireChainSlotIndex != -1 && FireChainElement.LastHoveredFireChainIndex != -1;

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsHoveringFireChainSlot)
            Inventory.Instance.m_fireChains[FireChainElement.LastHoveredFireChainIndex][FireChainSlot.LastHoveredFireChainSlotIndex] = null;
        else
            Inventory.Instance.m_items.Remove(Item);

        transform.SetParent(Inventory.Instance.transform);
        m_image.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsHoveringFireChainSlot)
        {
            var slottedItem = Inventory.Instance.m_fireChains[FireChainElement.LastHoveredFireChainIndex][FireChainSlot.LastHoveredFireChainSlotIndex];
            if (slottedItem != null)
            {
                Inventory.Instance.AddItem(slottedItem);
            }

            Inventory.Instance.m_fireChains[FireChainElement.LastHoveredFireChainIndex][FireChainSlot.LastHoveredFireChainSlotIndex] = Item;
        }
        else
        {
            Inventory.Instance.AddItem(Item);
        }

        Inventory.Instance.RebuildUI();
        Inventory.Instance.BuildFireSystemChains();
        Destroy(gameObject);
    }

    public void Setup(Item item)
    {
        Item = item;
        if (item.Icon != null)
            m_image.sprite = Utility.CreateSpriteFromTexture(item.Icon);
    }
}
