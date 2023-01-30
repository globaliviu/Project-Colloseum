using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<AmmoSupply> ammos = new List<AmmoSupply>();

    public static Inventory main;


    private void Awake()
    {
        main = this;
    }


    public int GetAmmoAmount(AmmoType _type)
    {

        foreach (var a in ammos)
        {
            if (a.ammoType == _type)
                return a.currentAmount;
        }

        return 0;
    }

    public void ConsumeAmmo(AmmoType _type)
    {
        foreach (var a in ammos)
        {
            if (a.ammoType == _type)
            {
                a.ConsumeAmmo();
                break;
            }

        }
    }
    public int TakeAmmo(AmmoType _type, int _amount)
    {
        foreach (var a in ammos)
        {
            if (a.ammoType == _type)
            {
                return a.TakeAmmo(_amount);
                
            }

        }

        return 0;
    }

    public AmmoSupply GetAmmoSupply(AmmoType _type)
    {
        foreach (var a in ammos)
        {
            if (a.ammoType == _type)
            {
                return a;
            }
        }
        Debug.LogWarning($"No such ammo type {_type}");
        return null;
    }
}
