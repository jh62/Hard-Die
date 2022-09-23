using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public enum Weapon
    {
        DEAGLE = 0,
        AK47 = 1,
        G36C = 2,
        MP5 = 3
    }

    private int activeIndex = (int)Weapon.DEAGLE;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            next();
        }
    }

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
}
