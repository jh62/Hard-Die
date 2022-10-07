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
    public float FireRate { get => fireRate; }
    public int Bullets { get => bullets; }
    public int MaxBullets { get => maxBullets; }

    public ParticleSystem HitEffect { get => hitEffect; }
    public ParticleSystem HitMetalEffect { get => hitMetalEffect; }

    [SerializeField] private int bullets = 0;
    [SerializeField] private int magazines = 0;
    [SerializeField] private float fireRate = .5f;
    [SerializeField] private WeaponID id = WeaponID.PISTOL;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] fireSounds;
    [SerializeField] private AudioClip drySound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip[] fleshHitSounds;
    [SerializeField] private AudioClip[] wallHitSounds;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    [SerializeField]
    private ParticleSystem hitEffect;

    [SerializeField]
    private ParticleSystem hitMetalEffect;

    private AudioSource audioSourceImpact;

    private int maxBullets;


    private void Awake()
    {
        audioSourceImpact = gameObject.AddComponent<AudioSource>();
        audioSourceImpact.playOnAwake = false;

    }

    private void Start()
    {
        maxBullets = bullets;
    }

    public void reload()
    {
        if (magazines == 0)
            return;

        magazines--;
        bullets = maxBullets;
        PlayReloadSound();
    }

    public void Shoot()
    {
        if (bullets == 0)
        {
            PlayDrySound();
            return;
        }

        bullets--;
        PlayFireSound();
        muzzleFlash.Emit(1);
        OnWeaponFired?.Invoke(transform);
    }

    public void PlayFireSound()
    {
        audioSource.clip = fireSounds[Random.Range(0, fireSounds.Length - 1)];
        audioSource.pitch = Random.Range(1f, 1.07f);
        audioSource.Play();
    }

    public void PlayDrySound()
    {
        audioSource.clip = drySound;
        audioSource.Play();
    }

    public void PlayReloadSound()
    {
        audioSource.clip = reloadSound;
        audioSource.Play();
    }

    public void PlayFleshHitSound()
    {
        audioSourceImpact.clip = fleshHitSounds[Random.Range(0, fleshHitSounds.Length - 1)];
        audioSourceImpact.pitch = Random.Range(1f, 1.07f);
        audioSourceImpact.Play();
    }

    public void PlayWallSound()
    {
        audioSourceImpact.clip = wallHitSounds[Random.Range(0, wallHitSounds.Length - 1)];
        audioSourceImpact.pitch = Random.Range(1f, 1.07f);
        audioSourceImpact.Play();
    }
}
