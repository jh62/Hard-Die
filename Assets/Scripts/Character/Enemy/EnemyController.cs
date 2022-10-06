using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyController : BaseCharacter
{
    public Perception perception;

    [SerializeField]
    private List<Transform> patrolPoints = default;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Transform headTarget;

    [Range(1f, 20f)]
    [SerializeField] private float targetLockSpeed = 4.21f;

    private void Start()
    {
        StartCoroutine("UpdateNavAgentLogic");
        perception.OnTargetExitTrigger += OnTargetLost;
        BaseWeapon.OnWeaponFired += OnWeaponFired;
    }

    void Update()
    {
        // headTarget.position = target.position;        

        switch (State)
        {
            case CharacterState.IDLE:
                {
                    if (perception.Target != null || agent.destination != null)
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
                    if (perception.Target == null && agent.destination == null)
                    {
                        State = CharacterState.IDLE;
                        return;
                    }

                    var direction = agent.desiredVelocity.normalized;
                    var speed = agent.desiredVelocity.magnitude;

                    animator.SetFloat("x", speed * direction.x, .1f, Time.deltaTime);
                    animator.SetFloat("y", speed * direction.z, .1f, Time.deltaTime);

                    if (perception.Target != null)
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

        var lookSpeed = targetLockSpeed - (targetLockSpeed * perception.Target.getSpeed());
        Vector3 lookPos = perception.Target.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, targetLockSpeed * Time.deltaTime);
    }

    private void OnWeaponFired(Transform transform)
    {
        if (perception.Target != null)
            return;

        patrolPoints.Clear();
        patrolPoints.Add(transform);
        animator.SetBool("Alerted", true);
        // agent.destination = transform.position;
        Debug.Log("Shots fired!");
    }

    private void OnTargetLost(BaseCharacter _target)
    {
        agent.destination = _target.transform.position;
    }

    private void OnDrawGizmos()
    {
        if (perception.Target == null)
            return;

        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 25f);
    }

    IEnumerator ShootLogic()
    {
        var weapon = inventory.GetWeapon();

        animator.SetBool("Shooting", true);
        animator.SetInteger("WeaponID", (int)weapon.Id);

        while (perception.Target != null && Physics.Raycast(rayOrigin.position, transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore) && hit.collider.CompareTag(perception.Target.tag))
        {
            perception.Target.Hit(1f, hit.normal);
            weapon.Shoot();
            yield return new WaitForSeconds(weapon.FireRate);
        }

        animator.SetBool("Shooting", false);
        yield break;
    }

    IEnumerator UpdateNavAgentLogic()
    {
        int patrolPointIndex = 0;
        float waitTime = 0f;

        while (isAlive())
        {
            if (perception.Target != null)
            {
                animator.SetBool("Alerted", true);
                agent.destination = perception.Target.transform.position;
            }
            else
            {
                // animator.SetBool("Alerted", false);

                if (waitTime > 0f)
                    waitTime -= Time.deltaTime;

                if (patrolPoints.Count > 0)
                {
                    if (patrolPointIndex >= patrolPoints.Count)
                        patrolPointIndex = 0;

                    Transform waypoint = patrolPoints[patrolPointIndex];

                    if (Vector3.Distance(transform.position, waypoint.position) < agent.stoppingDistance)
                    {
                        patrolPointIndex++;
                        waitTime = Random.Range(0f, 10f);
                    }

                    agent.destination = waypoint.position;
                }
            }

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
