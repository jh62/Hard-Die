using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perception : MonoBehaviour
{
    public Action<BaseCharacter> OnTargetEnterTrigger;
    public Action<BaseCharacter> OnTargetExitTrigger;

    public BaseCharacter Target { get => target; }

    private BaseCharacter target;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        target = other.GetComponent<BaseCharacter>();
        OnTargetEnterTrigger?.Invoke(target);
    }

    private void OnTriggerExit(Collider other)
    {
        if (target == null || !other.CompareTag(target.tag))
            return;

        OnTargetExitTrigger?.Invoke(target);
        target = null;
    }
}
