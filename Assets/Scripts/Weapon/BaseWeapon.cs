using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseWeapon : MonoBehaviour
{
    public static Action<Transform> OnWeaponFired;

    public enum WeaponID
    {
        PISTOL = 0,
        MP5 = 1,
        G36C = 2,
        AK47 = 3,
    }

    public WeaponID Id { get => id; }
    public float FireRate { get => fireRate; set => fireRate = value; }

    [SerializeField] private int bullets = 0;
    [SerializeField] private int magazines = 0;
    [SerializeField] private float fireRate = .5f;
    [SerializeField] private WeaponID id = WeaponID.PISTOL;

    [SerializeField] private AudioSource audioSource = default;
    [SerializeField] private AudioClip[] fireSounds = default;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    private void Start()
    {
    }

    public void reload()
    {
        if (magazines == 0)
            return;

        magazines--;
        bullets = 30;
    }

    public void Shoot()
    {
        // if (bullets == 0)
        //     return;

        PlayFireSound();
        muzzleFlash.Emit(1);
        OnWeaponFired?.Invoke(transform);
    }

    public void Shoot(List<BaseCharacter> targets)
    {
        RaycastHit hit = default;
        BaseCharacter target = null;

        foreach (var t in targets)
        {
            if (!t.isAlive())
                continue;

            if (target == null || Vector2.Distance(transform.position, t.transform.position) < Vector2.Distance(transform.position, target.transform.position))
            {
                if (Physics.Raycast(transform.position, (transform.position - t.transform.position).normalized, out hit, Mathf.Infinity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider != null)
                        target = t;
                }
            }
        }

        if (target != null)
        {
            target.Hit(1f, hit.normal);
        }

        PlayFireSound();
        muzzleFlash.Emit(1);
    }

    private void PlayFireSound()
    {
        audioSource.clip = fireSounds[Random.Range(0, fireSounds.Length - 1)];
        audioSource.pitch = Random.Range(1f, 1.07f);
        audioSource.Play();
    }
}
