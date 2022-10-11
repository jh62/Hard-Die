using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    [SerializeField]
    private float accel = 4.25f;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private CharacterController controller;

    private float speed = 0f;

    public void Move(float _x, float _z, bool sprinting)
    {
        if (_z != 0f || _x != 0f)
        {
            float sprint = sprinting ? accel : accel * .75f;
            speed = Mathf.Clamp(speed + sprint * Time.deltaTime, 0f, 1f);

            if (!sprinting && speed > .5f)
                speed -= .025f;
        }
        else
        {
            speed *= .217f;
        }


        if (speed != 0f && speed <= Mathf.Epsilon)
        {
            speed = 0f;
        }

        if (speed >= .475f)
        {
            Vector3 move = transform.right * _x + transform.forward * _z;
            controller.Move(move * accel * speed * Time.deltaTime);
        }

        animator.SetFloat("x", speed * _x, .1f, Time.deltaTime);
        animator.SetFloat("y", speed * _z, .1f, Time.deltaTime);
    }

    public void setShooting(bool _shooting)
    {
        animator.SetBool("shooting", _shooting);
    }
}
