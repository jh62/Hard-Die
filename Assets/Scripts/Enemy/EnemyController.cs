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

    void Update()
    {
        if (target == null)
            return;

        headTarget.position = target.position;

        switch (state)
        {
            case BaseCharacter.CharacterState.IDLE:
                {
                    Animator.SetFloat("x", 0f, .1f, Time.deltaTime);
                    Animator.SetFloat("y", 0f, .1f, Time.deltaTime);
                    break;
                }
            case BaseCharacter.CharacterState.MOVE:
                {
                    agent.destination = target.position;
                    var direction = agent.desiredVelocity.normalized;
                    var speed = agent.speed;

                    animator.SetFloat("x", speed * direction.x, .1f, Time.deltaTime);
                    animator.SetFloat("y", speed * direction.z, .1f, Time.deltaTime);
                    break;
                }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.transform;
            setState(CharacterState.MOVE);
            return;
        }

        target = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        target = null;
        headTarget = null;
        setState(CharacterState.IDLE);
    }
}
