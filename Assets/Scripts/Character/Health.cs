using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    [Tooltip("The animator that controls the bones")]
    [SerializeField] private Animator animator;

    [Tooltip("The character controller, if there is one.")]
    [SerializeField] private CharacterController controller;

    [Tooltip("The navigation agent, if there is one.")]
    [SerializeField] private NavMeshAgent agent;

    [Tooltip("All the bones with Rigid Bodies.")]
    [SerializeField] private Rigidbody[] rigidBodies;

    void Awake()
    {
        ActivateRagdolls(false);
    }

    public void ActivateRagdolls(bool activate)
    {
        if (controller != null)
            controller.enabled = !activate;

        if (agent != null)
            agent.enabled = !activate;

        if (animator != null)
            animator.enabled = !activate;

        foreach (Rigidbody body in rigidBodies)
        {
            body.useGravity = activate;
            body.isKinematic = !activate;
        }
    }
}
