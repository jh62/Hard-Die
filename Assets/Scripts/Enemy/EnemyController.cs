using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyController : BaseCharacter
{
    public Transform target;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Transform headTarget;

    private void Start()
    {
        StartCoroutine("updateNav");
        Debug.Log("Nav");
    }

    IEnumerator updateNav()
    {
        while (agent.enabled)
        {
            if (target != null)
                agent.destination = target.position;
            yield return new WaitForSeconds(.15f);
        }
    }

    void Update()
    {
        // headTarget.position = target.position;        

        switch (State)
        {
            case CharacterState.IDLE:
                {
                    Animator.SetFloat("x", 0f, .1f, Time.deltaTime);
                    Animator.SetFloat("y", 0f, .1f, Time.deltaTime);
                    break;
                }
            case CharacterState.MOVE:
                {
                    // if (target == null)
                    //     return;

                    var direction = agent.desiredVelocity.normalized;
                    var speed = agent.speed;

                    animator.SetFloat("x", speed * direction.x, .1f, Time.deltaTime);
                    animator.SetFloat("y", speed * direction.z, .1f, Time.deltaTime);

                    // if (agent.remainingDistance < agent.stoppingDistance)
                    // {
                    //     agent.angularSpeed = 0f;

                    //     if (!animator.GetBool("Shooting"))
                    //         StartCoroutine("ShootLogic");
                    // }
                    // else
                    // {
                    //     agent.angularSpeed = 120f;
                    // }

                    if (!animator.GetBool("Shooting"))
                        StartCoroutine("ShootLogic");

                    if (agent.angularSpeed <= 0f)
                        FaceTarget();

                    break;
                }
        }
    }

    private void FaceTarget()
    {
        if (!agent.enabled)
            return;

        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 4f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        target = other.transform;
        State = CharacterState.MOVE;
    }

    private void OnTriggerExit(Collider other)
    {
        // if (target != null && target == other)
        // {
        //     target = null;
        //     // headTarget = null;
        //     State = CharacterState.IDLE;
        //     return;
        // }
    }

    IEnumerator ShootLogic()
    {
        var weapon = inventory.GetWeapon();

        animator.SetBool("Shooting", true);
        animator.SetInteger("WeaponID", (int)weapon.Id);
        Debug.Log("Started");

        PlayerController pC = target.GetComponent<PlayerController>();

        while (Physics.Raycast(transform.position, ((target.position + new Vector3(pC.direction.x, 0f, pC.direction.z)) - transform.position).normalized, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore) && hit.collider.CompareTag("Player"))
        {
            Debug.Log("Shooting");
            var targets = new List<BaseCharacter>();
            targets.Add(hit.collider.GetComponent<BaseCharacter>());
            weapon.Shoot(targets);
            // weapon.Shoot(raycastOrigin.position, raycastOrigin.forward);
            yield return new WaitForSeconds(weapon.FireRate);
        }

        animator.SetBool("Shooting", false);
        yield break;
    }
}
