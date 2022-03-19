using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemCell : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item Item;
    [SerializeField] private Image m_image;

    private bool IsHoveringFireChainSlot => FireChainElement.LastHoveredFireChainIndex != -1 && FireChainSlot.LastHoveredSlotIndex != -1;

    private bool DoesItemMatchSlotType()
    {
        if (!IsHoveringFireChainSlot)
            return false;

        var slotType = Inventory.Instance.GetSlotType(FireChainElement.LastHoveredFireChainIndex, FireChainSlot.LastHoveredSlotIndex);

        switch (Item)
        {
            case WeaponItem when slotType == FireChainSlot.FireChainSlotType.Weapon: return true;
            case ModifierItem when slotType == FireChainSlot.FireChainSlotType.Modifier: return true;
        }

        return false;
    }

    private bool DoesFireChainContainItem()
    {
        if (!IsHoveringFireChainSlot)
            return false;

        var fireChain = Inventory.Instance.m_fireChains[FireChainElement.LastHoveredFireChainIndex];
        return fireChain.Contains(Item);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameManager.Instance.HoverPopup.HideHoverPopup();

        if (IsHoveringFireChainSlot)
            Inventory.Instance.m_fireChains[FireChainElement.LastHoveredFireChainIndex][FireChainSlot.LastHoveredSlotIndex] = null;
        else
        {
            int siblingIndex = transform.GetSiblingIndex();
            var itemCellCopy = Instantiate(gameObject, transform.parent);
            itemCellCopy.transform.SetSiblingIndex(siblingIndex);
        }

        transform.SetParent(Inventory.Instance.transform);
        m_image.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsHoveringFireChainSlot && DoesItemMatchSlotType() && !DoesFireChainContainItem())
        {
            Inventory.Instance.m_fireChains[FireChainElement.LastHoveredFireChainIndex][FireChainSlot.LastHoveredSlotIndex] = Item;
            Inventory.Instance.BuildFireChainUI();
        }

        Inventory.Instance.BuildFireSystemChains();
        Destroy(gameObject);
    }

    public void Setup(Item item)
    {
        Item = item;
        if (item.Icon != null)
            m_image.sprite = Utility.CreateSpriteFromTexture(item.Icon);

        switch (item)
        {
            case WeaponItem: m_image.color = Colors.WeaponColor; break;
            case ModifierItem: m_image.color = Colors.ModifierColor; break;
            default: m_image.color = Color.white; break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.HoverPopup.ShowHoverPopup(Item.DisplayName, Item.DisplayDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.HoverPopup.HideHoverPopup();
    }
}
