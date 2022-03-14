using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data/WeaponItem", menuName = "Data/WeaponItem")]
public class WeaponItem : Item
{
    public FireSystem.FireChain FireChain;
    public string TargetMethodName;
}
