using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private HumanBodyController movementController;

    void Update()
    {
        movementController.setShooting(Input.GetButton("Fire1"));

        float _x = Input.GetAxis("Horizontal");
        float _z = Input.GetAxis("Vertical");
        bool sprinting = Input.GetKey(KeyCode.LeftShift);

        movementController.Move(_x, _z, sprinting);
    }
}
