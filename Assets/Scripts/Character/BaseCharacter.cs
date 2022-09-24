using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseCharacter : MonoBehaviour
{
    public enum CharacterState
    {
        IDLE,
        MOVE,
        DYING,
        DEAD
    }


    public Animator Animator { get => animator; }
    public CharacterState State { get => state; set => state = value; }

    [SerializeField]
    protected Animator animator;

    [SerializeField]
    protected WeaponInventory inventory;

    protected CharacterState state = CharacterState.IDLE;

    private Rigidbody[] rigidBodies;

    private void Awake()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
    }

    public void setShooting(bool _shooting)
    {
        animator.SetBool("shooting", _shooting);
    }

    public void setState(CharacterState newState)
    {
        if (newState == state)
            return;

        state = newState;

        switch (state)
        {
            case CharacterState.IDLE:
                {
                    break;
                }
            case CharacterState.MOVE:
                {
                    break;
                }
        }
    }
}
