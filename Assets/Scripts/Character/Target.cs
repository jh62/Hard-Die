using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public string targetTag = default;
    private List<BaseCharacter> targets = new List<BaseCharacter>();

    void Start()
    {
        BoxCollider box = GetComponent<BoxCollider>();

        if (box == null || !box.isTrigger)
            Debug.Log("Warning! Target script needs a trigger collider to work!");
    }

    public List<BaseCharacter> getTargets()
    {
        return targets;
    }

    // public RaycastHit getClosest()
    // {
    //     RaycastHit hit = default;
    //     BaseCharacter target = null;

    //     foreach (var t in targets)
    //     {
    //         if (!t.isAlive())
    //             continue;

    //         if (target == null || Vector2.Distance(transform.position, t.transform.position) < Vector2.Distance(transform.position, target.transform.position))
    //         {
    //             if (Physics.Raycast(transform.position, (t.transform.position - transform.position).normalized, out hit, Mathf.Infinity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
    //             {
    //                 if (hit.collider is ITargetable)
    //                     target = t;
    //             }
    //         }
    //     }

    //     return hit;
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag))
            return;

        BaseCharacter mob = other.GetComponent<BaseCharacter>();

        if (mob == null || !mob.isAlive())
            return;

        mob.OnDead += RemoveTarget;
        targets.Add(mob);

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(targetTag))
            return;

        BaseCharacter mob = other.GetComponent<BaseCharacter>();

        if (mob == null)
            return;

        mob.OnDead -= RemoveTarget;
        targets.Remove(mob);
    }

    private void RemoveTarget(BaseCharacter target)
    {
        targets.Remove(target);
    }
}
