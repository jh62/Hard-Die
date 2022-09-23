using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacter
{
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private float acceleration = 4.25f;

    public float speed = 0f;

    private Vector3 direction = new Vector3();

    void Update()
    {
        PlayerInput();

        switch (state)
        {
            case CharacterState.IDLE:
                {
                    if (speed != 0f)
                    {
                        setState(CharacterState.MOVE);
                    }
                    break;
                }
            case CharacterState.MOVE:
                {
                    if (speed == 0f)
                    {
                        setState(CharacterState.IDLE);
                    }

                    animator.SetFloat("x", speed * direction.x, .1f, Time.deltaTime);
                    animator.SetFloat("y", speed * direction.z, .1f, Time.deltaTime);
                    break;
                }
        }
    }

    private void PlayerInput()
    {
        float _x = Input.GetAxis("Horizontal");
        float _z = Input.GetAxis("Vertical");
        bool sprinting = Input.GetKey(KeyCode.LeftShift);

        if (_z != 0f || _x != 0f)
        {
            float sprint = sprinting ? acceleration : acceleration * .75f;
            speed = Mathf.Clamp(speed + sprint * Time.deltaTime, 0f, 1f);

            if (!sprinting && speed > .5f)
                speed -= .025f;
        }
        else
        {
            speed *= .217f;
        }


        if (speed > 0f && speed <= Mathf.Epsilon)
        {
            speed = 0f;
        }

        if (speed >= .475f)
        {
            direction = new Vector3(_x, 0f, _z);
            var desiredVelocity = transform.right * direction.x + transform.forward * direction.z;
            controller.Move(desiredVelocity * speed * acceleration * Time.deltaTime);
        }

        bool firing = Input.GetButton("Fire1");

        if (firing && !animator.GetBool("shooting"))
        {
            StartCoroutine("ShootLogic");
            inventory.GetWeapon().Shoot();
        }
    }

    IEnumerator ShootLogic()
    {
        animator.SetBool("shooting", true);
        yield return new WaitForSeconds(.08f);
        inventory.GetWeapon().Shoot();
        yield return new WaitForSeconds(.35f);
        animator.SetBool("shooting", false);
    }
}