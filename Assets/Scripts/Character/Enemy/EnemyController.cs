using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyController : BaseCharacter
{
    public Perception perception;

    private static float spotDelay = 0f;

    [SerializeField]
    private List<Transform> patrolPoints = default;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Transform headTarget;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] clipVoices;

    [SerializeField]
    private AudioClip[] clipDieVoices;

    [Range(1f, 20f)]
    [SerializeField] private float targetLockSpeed = 4.21f;

    private void Start()
    {
        StartCoroutine("UpdateNavAgentLogic");
        perception.OnTargetEnterTrigger += OnTargetAdquired;
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

                    if (isHit)
                        return;

                    var direction = agent.desiredVelocity.normalized;
                    var speed = agent.desiredVelocity.magnitude;

                    animator.SetFloat("x", speed * direction.x, .1f, Time.deltaTime);
                    animator.SetFloat("y", speed * direction.z, .1f, Time.deltaTime);

                    if (perception.Target != null)
                    {
                        if (agent.remainingDistance < 16f)
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

    public override void OnStateChanged()
    {
        base.OnStateChanged();

        switch (State)
        {
            case CharacterState.DEAD:
                {
                    perception.OnTargetEnterTrigger -= OnTargetAdquired;
                    perception.OnTargetExitTrigger -= OnTargetLost;

                    StopAllCoroutines();

                    agent.enabled = false;
                    playRandomSound(clipDieVoices);
                    break;
                }
        }
    }

    private void FaceTarget()
    {
        var lookSpeed = targetLockSpeed - (targetLockSpeed * perception.Target.getSpeed());
        Vector3 lookPos = perception.Target.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, targetLockSpeed * Time.deltaTime);
    }

    private void OnWeaponFired(Transform other)
    {
        if (perception.Target != null)
            return;

        if (Vector3.Distance(transform.position, other.position) >= 14f)
            return;

        patrolPoints.Clear();
        patrolPoints.Add(other);
        animator.SetBool("Alerted", true);
        Debug.Log("Shots fired!");
    }

    public bool isHit = false;

    public override void Hit(float dammage, Vector3 normal)
    {
        base.Hit(dammage, normal);

        if (isAlive() && !isHit)
            StartCoroutine("HitLogic");
    }

    private void playRandomSound(AudioClip[] source)
    {
        audioSource.clip = source[(int)Random.Range(0, source.Length - 1)];
        audioSource.pitch = Random.Range(.96f, 1.1f);
        audioSource.Play();
    }

    private void OnTargetAdquired(BaseCharacter _target)
    {
        if (Time.time - spotDelay < 3f)
            return;

        spotDelay = Time.time;
        playRandomSound(clipVoices);
    }

    private void OnTargetLost(BaseCharacter _target)
    {
        agent.destination = _target.transform.position;
    }

    IEnumerator HitLogic()
    {
        isHit = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(.77f);
        isHit = false;
        agent.isStopped = false;
    }

    IEnumerator ShootLogic()
    {
        var weapon = Inventory.GetWeapon();

        animator.SetBool("Shooting", true);
        animator.SetInteger("WeaponID", (int)weapon.Id);

        while (!isHit && perception.Target != null && Physics.Raycast(rayOrigin.position, transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore) && hit.collider.CompareTag(perception.Target.tag))
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
                if (patrolPoints.Count > 0)
                {
                    patrolPoints.Clear();
                    animator.SetBool("Alerted", true);
                }

                agent.destination = perception.Target.transform.position;
            }
            else
            {
                // animator.SetBool("Alerted", false);

                if (waitTime > 0f)
                {
                    waitTime -= Time.deltaTime;
                    yield return null;
                }

                if (patrolPoints.Count > 0)
                {
                    if (patrolPointIndex >= patrolPoints.Count)
                        patrolPointIndex = 0;

                    Transform waypoint = patrolPoints[patrolPointIndex];

                    if (Vector3.Distance(transform.position, waypoint.position) <= agent.stoppingDistance)
                    {
                        patrolPointIndex++;
                        waitTime = Random.Range(0f, 5f);
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
