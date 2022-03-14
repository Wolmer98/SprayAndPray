using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private GameObject m_itemCellPrefab;

    public List<Item> m_items = new List<Item>();
    private List<List<Item>> m_fireChains = new List<List<Item>>();

    void Start()
    {
        // Temp.
        var testFireChain = new List<Item>();
        foreach (var item in m_items)
        {
            testFireChain.Add(item);
        }
        m_fireChains.Add(testFireChain);

        BuildFireChains();
    }

    void Update()
    {
        
    }

    public void AddItem(Item item)
    {
        m_items.Add(item);
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void BuildFireChains()
    {
        var fireSystem = GameManager.Instance.Player.GetComponent<FireSystem>();
        fireSystem.FireChains.Clear();

        var modifierMethods = new FireModifiers();
        var targetMethods = new TargetMethods();

        for (int i = 0; i < m_fireChains.Count; i++)
        {
            var fireChain = m_fireChains[i];
            var fireSystemChain = new FireSystem.FireChain();

            for (int j = 0; j < fireChain.Count; j++)
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