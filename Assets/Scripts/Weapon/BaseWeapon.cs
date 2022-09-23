using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    public float bullets = 0f;
    public float magazines = 0f;
    public float fireRate = .5f;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    public void Shoot()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Emit(1);
        }
    }
}
