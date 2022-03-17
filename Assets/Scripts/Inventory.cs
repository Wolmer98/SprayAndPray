using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    private static Inventory m_instance;
    public static Inventory Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<Inventory>();

            return m_instance;
        }
    }

    [SerializeField] private Transform m_unequippedParent;
    [SerializeField] private Transform m_fireChainsParent;

    [SerializeField] private ItemCell m_itemCellPrefab;
    [SerializeField] private FireChainElement m_fireChainPrefab;
    [SerializeField] private FireChainSlot m_fireChainSlotPrefab;

    [SerializeField] private int m_fireChainStartAmount;
    [SerializeField] private int m_fireChainLength;

    public List<Item> m_items = new List<Item>();
    public List<Item[]> m_fireChains = new List<Item[]>();

    public void Init()
    {
        for (int i = 0; i < m_fireChainStartAmount; i++)
        {
            CreateNewFireChain();
        }

        AddItem(GameManager.Instance.StartWeapon);
        m_fireChains[0][0] = GameManager.Instance.StartWeapon;

        RebuildUI();
        BuildFireSystemChains();
    }

    public void CreateNewFireChain()
    {
        m_fireChains.Add(new Item[m_fireChainLength]);
        BuildFireChainUI();
    }

    public void AddItem(Item item)
    {
        m_items.Add(item);
    }

    public FireChainSlot.FireChainSlotType GetSlotType(int chainIndex, int slotIndex)
    {
        if (chainIndex == -1 || slotIndex == -1)
            return FireChainSlot.FireChainSlotType.Weapon;

        var fireChain = m_fireChainsParent.GetChild(chainIndex);
        var slot = fireChain.GetComponent<FireChainElement>().SlotContainer.GetChild(slotIndex);
        return slot.GetComponentInChildren<FireChainSlot>().SlotType;
    }

    private void OnEnable()
    {
        RebuildUI();
    }

    public void RebuildUI()
    {
        BuildInventoryUI();
        BuildFireChainUI();
    }

    public void BuildInventoryUI()
    {
        Utility.DestroyAllChildren(m_unequippedParent);

        foreach (var item in m_items)
        {
            var itemCell = Instantiate(m_itemCellPrefab, m_unequippedParent);
            itemCell.Setup(item);
        }
    }

    public void BuildFireChainUI()
    {
        Utility.DestroyAllChildren(m_fireChainsParent);

        int fireChainIndex = 0;
        foreach (var fireChain in m_fireChains)
        {
            var fireChainElement = Instantiate(m_fireChainPrefab, m_fireChainsParent);
            fireChainElement.FireChainIndex = fireChainIndex;

            for (int i = 0; i < m_fireChainLength; i++)
            {
                var fireChainSlot = Instantiate(m_fireChainSlotPrefab, fireChainElement.SlotContainer); // eww
                fireChainSlot.SlotIndex = i;
                fireChainSlot.SlotType = i == 0 ? FireChainSlot.FireChainSlotType.Weapon : FireChainSlot.FireChainSlotType.Modifier;

                if (fireChain[i] != null)
                {
                    var itemCell = Instantiate(m_itemCellPrefab, fireChainSlot.transform);
                    itemCell.Setup(fireChain[i]);
                }
            }

            fireChainIndex++;
        }
    }

    public void BuildFireSystemChains()
    {
        var fireSystem = GameManager.Instance.Player.GetComponent<FireSystem>();
        fireSystem.FireChains.Clear();

        var modifierMethods = new FireModifiers();
        var targetMethods = new TargetMethods();

        for (int i = 0; i < m_fireChains.Count; i++)
        {
            var fireChain = m_fireChains[i];
            var fireSystemChain = new FireSystem.FireChain();

            for (int j = 0; j < fireChain.Length; j++)
            {
                var item = fireChain[j];
                
                if (item is WeaponItem weaponItem)
                {
                    fireSystemChain.OriginalFireRequest = weaponItem.FireChain.OriginalFireRequest;
                    fireSystemChain.OriginalCooldown = weaponItem.FireChain.OriginalCooldown;

                    var targetMethod = targetMethods.GetType().GetMethod(weaponItem.TargetMethodName);
                    fireSystemChain.OriginalFireRequest.GetTargetPosition = (Vector3 inPosition) =>
                    {
                        object[] args = { inPosition };
                        return (Vector3)targetMethod.Invoke(fireSystemChain, args);
                    };
                }
                else if (item is ModifierItem modifierItem)
                {
                    var modifierMethod = modifierMethods.GetType().GetMethod(modifierItem.ModifierMethodName);
                    fireSystemChain.FireModifiers.Add((FireSystem.FireRequest fireRequest) => 
                    {
                        object[] args = { fireRequest };
                        return (FireSystem.FireRequest)modifierMethod.Invoke(fireSystemChain, args);
                    });
                }
            }

            fireSystem.FireChains.Add(fireSystemChain);
        }
    }
}