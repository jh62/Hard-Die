using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterMovement characterMovement;

    public BaseWeapon weapon;

    void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {
        float _x = Input.GetAxis("Horizontal");
        float _z = Input.GetAxis("Vertical");
        bool sprinting = Input.GetKey(KeyCode.LeftShift);

        characterMovement.Move(_x, _z, sprinting);
    }

    private void LateUpdate()
    {
        bool firing = Input.GetButton("Fire1");

        if (firing)
        {
            characterMovement.setShooting(firing);
            weapon.Shoot();
        }
    }
}
