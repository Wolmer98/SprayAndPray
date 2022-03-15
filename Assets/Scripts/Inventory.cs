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

    [SerializeField] private int m_fireChainAmount;
    [SerializeField] private int m_fireChainLength;

    public List<Item> m_items = new List<Item>();
    public List<Item[]> m_fireChains = new List<Item[]>();

    public void Init()
    {
        for (int i = 0; i < m_fireChainAmount; i++)
        {
            m_fireChains.Add(new Item[m_fireChainLength]);
        }

        m_fireChains[0][0] = GameManager.Instance.StartWeapon;

        RebuildUI();
        BuildFireSystemChains();
    }

    public void AddItem(Item item)
    {
        m_items.Add(item);
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

    private void BuildInventoryUI()
    {
        Utility.DestroyAllChildren(m_unequippedParent);

        foreach (var item in m_items)
        {
            var itemCell = Instantiate(m_itemCellPrefab, m_unequippedParent);
            itemCell.Setup(item);
        }
    }

    private void BuildFireChainUI()
    {
        Utility.DestroyAllChildren(m_fireChainsParent);

        int fireChainIndex = 0;
        foreach (var fireChain in m_fireChains)
        {
            var fireChainElement = Instantiate(m_fireChainPrefab, m_fireChainsParent);
            fireChainElement.FireChainIndex = fireChainIndex;

            for (int i = 0; i < m_fireChainLength; i++)
            {
                var fireChainSlot = Instantiate(m_fireChainSlotPrefab, fireChainElement.transform.GetChild(1)); // eww
                fireChainSlot.FireChainSlotIndex = i;
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