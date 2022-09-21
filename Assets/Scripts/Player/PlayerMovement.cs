using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0f;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private CharacterController controller;

    void Start()
    {

    }

    float pistolWeight = 0f;
    float rifleWeight = 0f;
    float unnarmedWeight = 1f;

    public int weapon = 0;


    void Update()
    {
        // animator.SetLayerWeight(1, pistolWeight);
        // animator.SetLayerWeight(2, rifleWeight);
        // animator.SetLayerWeight(3, unnarmedWeight);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (z != 0f || x != 0f)
        {
            bool sprinting = Input.GetKey(KeyCode.LeftShift);
            float sprint = sprinting ? 8f : 4f;
            speed = Mathf.Clamp(speed + sprint * Time.deltaTime, 0f, 1f);

            if (!sprinting && speed > .5f)
                speed -= .025f;
        }
        else
        {
            speed *= .907f;
        }


        if (speed < Mathf.Epsilon)
        {
            speed = 0f;
            Debug.Log("Epsiloneado");
        }

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        animator.SetFloat("x", speed * x);
        animator.SetFloat("y", speed * z);
    }
}
