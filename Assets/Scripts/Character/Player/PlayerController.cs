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
        health = MaxHealth;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(groundCheck.position, 0.1f);
    }

    void Update()
    {
        if (!isAlive())
            return;

        PlayerInput();

        // Physics.CheckCapsule(groundCheck.position,)
        bool isGrounded = Physics.CheckSphere(groundCheck.position, 0.07f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);

        if (!isGrounded)
        {
            direction.y -= 9f * Time.deltaTime;
        }
        else
        {
            if (direction.y < 0f)
                direction.y = -2f;
        }

        controller.Move(transform.up * direction.y);

        switch (State)
        {
            case CharacterState.IDLE:
                {
                    if (speed != 0f)
                    {
                        State = CharacterState.MOVE;
                        break;
                    }
                    break;
                }
            case CharacterState.MOVE:
                {
                    if (speed <= 0f)
                    {
                        State = CharacterState.IDLE;
                        break;
                    }
                    animator.SetFloat("x", speed * direction.x, .1f, Time.deltaTime);
                    animator.SetFloat("y", speed * direction.z, .1f, Time.deltaTime);
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
        if (reloading)
            return;

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


        if (speed > 0f && speed <= Mathf.Epsilon)
        {
            speed = 0f;
        }

        if (speed >= .2f)
        {
            direction.Set(_x, direction.y, _z);
            // direction = new Vector3(_x, direction.y, _z);
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
            if (shooting)
            {
                shooting = false;
                animator.SetBool("Shooting", false);
                StopCoroutine("Shooting");
            }

            reloading = true;

            var weapon = inventory.GetWeapon();
            animator.SetInteger("WeaponID", (int)weapon.Id);
            animator.SetTrigger("Reloading");
            weapon.reload();
            StartCoroutine("ReloadLogic");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            inventory.next();
        }
    }

    IEnumerator ReloadLogic()
    {
        yield return new WaitForSeconds(3f);
        reloading = false;
    }

    IEnumerator ShootLogic()
    {
        var weapon = inventory.GetWeapon();

        animator.SetBool("Shooting", true);
        animator.SetInteger("WeaponID", (int)weapon.Id);

        while (shooting)
        {
            var target = targetCheck.CheckTargetHit(rayOrigin.position);

            if (target != null)
                target.Hit(1f, transform.forward);
            else
            {
                Collider c = targetCheck.Hit.collider;

                if (c != null && c.CompareTag("Window"))
                {
                    c.GetComponent<BreakableWindow>().breakWindow();
                }
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