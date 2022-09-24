using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    [SerializeField] private float bullets = 0f;
    [SerializeField] private float magazines = 0f;
    [SerializeField] private float fireRate = .5f;

    [SerializeField] private AudioSource audioSource = default;
    [SerializeField] private AudioClip[] fireSounds = default;

    private Task taskNextSound;
    private bool busy = false;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    private void Start()
    {
        next();
    }

    public void Shoot()
    {
        next();
        audioSource.Play();
        muzzleFlash.Emit(1);
    }

    private void next()
    {
        audioSource.clip = fireSounds[Random.Range(0, fireSounds.Length - 1)];
        audioSource.pitch = Random.Range(1f, 1.07f);
    }
}
