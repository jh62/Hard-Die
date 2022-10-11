using UnityEngine;
using UnityEngine.AI;

public class RagdollActivator : MonoBehaviour
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
        ActivateRagdolls(false, Vector3.zero);
    }

    public void ActivateRagdolls(bool activate, Vector3 normal)
    {
        foreach (Rigidbody body in rigidBodies)
        {
            body.useGravity = activate;
            body.isKinematic = !activate;

            if (normal.magnitude != 0f)
                body.AddForce(normal, ForceMode.Impulse);
        }
    }
}
