using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    public float bullets = 0f;
    public float magazines = 0f;
    public float fireRate = .5f;

    private bool busy = false;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    public void Shoot()
    {

        muzzleFlash.Emit(1);
    }
}
