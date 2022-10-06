using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public enum Weapon
    {
        DEAGLE,
        AK47,
        G36C,
        MP5
    }

    private int activeIndex = 0;

    public void switchTo(int weaponIndex)
    {
        foreach (int idx in Enum.GetValues(typeof(Weapon)))
        {
            bool active = idx == weaponIndex;
            transform.GetChild(idx).gameObject.SetActive(active);

            if (active)
                activeIndex = idx;
        }
    }

    public void switchTo(Weapon weapon)
    {
        switchTo((int)weapon);
    }

    public void next()
    {
        activeIndex++;

        if (activeIndex >= Enum.GetNames(typeof(Weapon)).Length)
            activeIndex = 0;

        switchTo(activeIndex);
    }

    public BaseWeapon GetWeapon()
    {
        return transform.GetChild(activeIndex).GetComponent<BaseWeapon>();
    }
}
