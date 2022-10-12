using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacter
{
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private Target targetCheck;

    [SerializeField]
    private float acceleration = 4.25f;

    private float speed = 0f;
    private bool shooting = false;
    private bool reloading = false;

    private Vector3 direction = new Vector3();

    [SerializeField] private bool drawGizmos = false;

    private void Start()
    {
        animator.SetBool("Alerted", true);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(groundCheck.position, 0.1f);

        if (targetCheck.Hit.collider != null)
            Gizmos.DrawLine(rayOrigin.position, targetCheck.Hit.point);
    }

    void Update()
    {
        if (!isAlive() || !controller.enabled)
            return;

        PlayerInput();

        bool isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);

        // bool isGrounded = Physics.Raycast(groundCheck.position, -transform.up, .5f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);

        if (!isGrounded)
            direction.y -= 18f * Time.deltaTime;
        else
            direction.y = -2f * Time.deltaTime;

        controller.Move(transform.up * direction.y * Time.deltaTime);

        switch (State)
        {
            case CharacterState.IDLE:
                {
                    if (speed > 0f)
                    {
                        State = CharacterState.MOVE;
                        break;
                    }

                    animator.SetFloat("x", 0);
                    animator.SetFloat("y", 0);

                    break;
                }
            case CharacterState.MOVE:
                {
                    if (speed <= 0f)
                    {
                        State = CharacterState.IDLE;
                        break;
                    }
                    animator.SetFloat("x", speed * direction.x, .075f, Time.deltaTime);
                    animator.SetFloat("y", speed * direction.z, .075f, Time.deltaTime);
                    break;
                }
        }
    }

    public override void OnStateChanged()
    {
        base.OnStateChanged();

        switch (State)
        {
            case CharacterState.DEAD:
                {
                    GetComponent<MouseLook>().enabled = false;
                    StopAllCoroutines();
                    controller.enabled = false;
                    animator.enabled = false;
                    break;
                }
        }
    }

    private void PlayerInput()
    {
        if (reloading || !controller.enabled)
        {
            speed = 0f;
            return;
        }

        float _x = Input.GetAxis("Horizontal");
        float _z = Input.GetAxis("Vertical");
        bool sprinting = Input.GetKey(KeyCode.LeftShift);

        if (_z != 0f || _x != 0f)
        {
            float sprint = sprinting ? acceleration : acceleration * .75f;
            speed = Mathf.Clamp(speed + sprint * Time.deltaTime, 0f, 1f);

            if (!sprinting && speed > .5f)
                speed = .5f;
        }
        else
        {
            speed *= .217f;
        }


        if (speed <= Mathf.Epsilon)
        {
            speed = 0f;
        }

        if (speed >= .2f)
        {
            direction.Set(_x, direction.y, _z);
            var desiredVelocity = transform.right * direction.x + transform.forward * direction.z;
            controller.Move(desiredVelocity * speed * acceleration * Time.deltaTime);
        }

        shooting = Input.GetButton("Fire1");

        if (shooting && !animator.GetBool("Shooting"))
        {
            StartCoroutine("ShootLogic");
        }

        if (!reloading && Input.GetKeyDown(KeyCode.R))
        {
            var weapon = Inventory.GetWeapon();

            if (weapon.Bullets >= weapon.MaxBullets)
                return;

            if (shooting)
            {
                shooting = false;
                animator.SetBool("Shooting", false);
                StopCoroutine("Shooting");
            }

            reloading = true;

            animator.SetInteger("WeaponID", (int)weapon.Id);
            animator.SetTrigger("Reloading");
            StartCoroutine("ReloadLogic");
        }

        if (Input.GetKeyDown(KeyCode.L) && !reloading && !shooting)
        {
            Inventory.next();
            animator.SetInteger("WeaponID", Inventory.WeaponID);
            animator.SetFloat("x", 0f);
            animator.SetFloat("y", 0f);
        }
    }

    public override void Hit(float dammage, Vector3 normal)
    {
        base.Hit(dammage, normal);
        StartCoroutine("HitLogic");
    }

    IEnumerator HitLogic()
    {
        controller.enabled = false;
        yield return new WaitForSeconds(.27f);
        controller.enabled = true;
    }

    IEnumerator ReloadLogic()
    {
        Inventory.GetWeapon().reload();
        yield return new WaitForSeconds(3.1f);
        reloading = false;
    }

    IEnumerator ShootLogic()
    {
        var weapon = Inventory.GetWeapon();

        if (weapon.Bullets == 0)
            yield break;

        animator.SetBool("Shooting", true);
        animator.SetInteger("WeaponID", (int)weapon.Id);

        while (shooting && weapon.Bullets > 0)
        {
            if (weapon.Bullets == 0)
            {
                weapon.PlayDrySound();
                yield return new WaitForSeconds(weapon.FireRate);
            }

            var target = targetCheck.CheckTargetHit(rayOrigin.position);

            if (target != null)
            {
                target.Hit(1f, transform.forward);
                weapon.HitEffect.transform.position = targetCheck.Hit.point + Vector3.up * UnityEngine.Random.Range(.45f, .5f);
                weapon.HitEffect.transform.forward = transform.forward;
                weapon.HitEffect.Emit(1);
                weapon.PlayFleshHitSound();
            }
            else
            {
                Collider c = targetCheck.Hit.collider;

                if (c != null && c.CompareTag("Window"))
                {
                    c.GetComponent<BreakableWindow>().breakWindow();
                }

                weapon.HitMetalEffect.transform.position = targetCheck.Hit.point;
                weapon.HitMetalEffect.transform.forward = targetCheck.Hit.normal;
                weapon.HitMetalEffect.Emit(1);
                weapon.PlayWallSound();
            }

            weapon.Shoot();
            yield return new WaitForSeconds(weapon.FireRate);
        }

        animator.SetBool("Shooting", false);
        yield break;
    }

    public override float getSpeed()
    {
        return speed;
    }

    public override Vector3 getDirection()
    {
        return direction;
    }
}