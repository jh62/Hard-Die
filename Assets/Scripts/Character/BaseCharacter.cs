using System;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    public Action<BaseCharacter> OnDead;

    public enum CharacterState
    {
        IDLE,
        MOVE,
        DEAD
    }


    public Animator Animator { get => animator; }
    public CharacterState State
    {
        get => state;
        set
        {
            if (state != value)
            {
                state = value;
                OnStateChanged();
            }
        }
    }
    // public IAnimationEvent AnimationEvents;

    [Range(1, 20)]
    public int MaxHealth = 10;

    [SerializeField]
    protected Animator animator;

    [SerializeField]
    protected WeaponInventory inventory;

    [SerializeField]
    protected Transform rayOrigin;

    // [SerializeField]
    // protected Transform raycastOrigin;

    protected int health;

    private CharacterState state = CharacterState.IDLE;
    private Rigidbody[] rigidBodies;

    private void Awake()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        // inventory.OnWeaponSwitched += OnWeaponSwitched;

        // if (AnimationEvents != null)
        // {
        //     AnimationEvents.OnAnimationEventKey += OnAnimationEventKey;
        //     AnimationEvents.OnAnimationEvent += OnAnimationEvent;
        // }
    }

    private void Start()
    {
        health = MaxHealth;
    }

    public abstract float getSpeed();
    public abstract Vector3 getDirection();

    public bool isAlive()
    {
        return State != CharacterState.DEAD;
    }

    public virtual void Hit(float dammage, Vector3 normal)
    {
        health--;

        if (health <= 0)
        {
            State = CharacterState.DEAD;
            GetComponent<Health>().ActivateRagdolls(true, normal);
            return;
        }

        // animator.SetFloat("normalX", normal.x);
        // animator.SetFloat("normalY", normal.z);
        animator.SetTrigger("Hit");
    }

    public virtual void OnStateChanged()
    {
        switch (State)
        {
            case CharacterState.DEAD:
                {
                    OnDead?.Invoke(this);
                    animator.enabled = false;
                    foreach (var component in GetComponents<Collider>())
                        component.enabled = false;
                    break;
                }
        }
    }

    public virtual void setShooting(bool _shooting)
    {
        animator.SetBool("shooting", _shooting);
    }

    private void OnWeaponSwitched(int weaponId)
    {
        animator.SetInteger("WeaponID", weaponId);
    }

    // public abstract void OnAnimationEventKey(string _event);
    // public abstract void OnAnimationEvent(AnimationEvent _event);
}
