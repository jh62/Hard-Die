using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyController : BaseCharacter
{
    public BaseCharacter target;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Transform headTarget;

    [Range(1f, 20f)]
    [SerializeField] private float targetLockSpeed = 4.21f;

    private void Start()
    {
        StartCoroutine("UpdateNavAgentLogic");
    }

    void Update()
    {
        // headTarget.position = target.position;        

        switch (State)
        {
            case CharacterState.IDLE:
                {
                    if (target != null || agent.destination != null)
                    {
                        State = CharacterState.MOVE;
                        return;
                    }

                    Animator.SetFloat("x", 0f, .1f, Time.deltaTime);
                    Animator.SetFloat("y", 0f, .1f, Time.deltaTime);
                    break;
                }
            case CharacterState.MOVE:
                {
                    if (target == null && agent.destination == null)
                    {
                        State = CharacterState.IDLE;
                        return;
                    }

                    var direction = agent.desiredVelocity.normalized;
                    var speed = agent.speed;

                    animator.SetFloat("x", speed * direction.x, .1f, Time.deltaTime);
                    animator.SetFloat("y", speed * direction.z, .1f, Time.deltaTime);

                    if (target != null)
                    {
                        if (agent.remainingDistance < 8f)
                        {
                            agent.angularSpeed = 0f;
                            FaceTarget();

                            if (!animator.GetBool("Shooting"))
                                StartCoroutine("ShootLogic");
                        }
                    }
                    else
                    {
                        agent.angularSpeed = 120f;
                    }

                    break;
                }
        }
    }

    private void FaceTarget()
    {
        // if (!agent.enabled || target == null)
        //     return;

        var lookSpeed = targetLockSpeed - (targetLockSpeed * target.getSpeed());
        Vector3 lookPos = target.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, targetLockSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        target = other.GetComponent<BaseCharacter>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (target != null && other.CompareTag(target.tag))
        {
            target = null;
            agent.destination = other.transform.position;
            // State = CharacterState.IDLE;
            return;
        }
    }

    private void OnDrawGizmos()
    {
        if (target == null)
            return;

        PlayerController pC = target.GetComponent<PlayerController>();
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 25f);
    }

    IEnumerator ShootLogic()
    {
        var weapon = inventory.GetWeapon();

        animator.SetBool("Shooting", true);
        animator.SetInteger("WeaponID", (int)weapon.Id);

        while (target != null && Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore) && hit.collider.CompareTag(target.tag))
        {
            target.Hit(1f, hit.normal);
            weapon.Shoot();
            yield return new WaitForSeconds(weapon.FireRate);
        }

        Debug.Log("Ended");
        animator.SetBool("Shooting", false);
        yield break;
    }

    IEnumerator UpdateNavAgentLogic()
    {
        while (isAlive())
        {
            if (target != null)
                agent.destination = target.transform.position;

            yield return new WaitForSeconds(.15f);
        }
    }

    public override float getSpeed()
    {
        return agent.speed;
    }

    public override Vector3 getDirection()
    {
        return agent.desiredVelocity.normalized;
    }
}
