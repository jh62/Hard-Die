using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private float maxRotation = 90f;

    [SerializeField]
    private float speed = 2f;

    [SerializeField]
    private bool locked = false;

    [SerializeField]
    private bool open = false;

    [SerializeField]
    private AudioSource audioSource;

    private Vector3 startingRotation;
    private Vector3 startPosition;

    void Start()
    {
        startingRotation = transform.rotation.eulerAngles;
        startPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (open || other.GetComponent<BaseCharacter>() == null)
            return;

        Open(other.transform.position);
    }

    public void Open(Vector3 from)
    {
        float dot = Vector3.Dot(transform.right, (from - transform.position).normalized);
        StartCoroutine("DoorRotationLogic", dot);
    }

    private IEnumerator DoorRotationLogic(float dot)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (open)
        {
            endRotation = Quaternion.Euler(startingRotation);
            open = false;
        }
        else
        {
            audioSource.pitch = Random.Range(1f, 1.1f);
            audioSource.Play();
            open = true;

            if (dot >= 0)
            {
                endRotation = Quaternion.Euler(new Vector3(0f, startRotation.y - maxRotation, 0f));
            }
            else
            {
                endRotation = Quaternion.Euler(new Vector3(0f, startRotation.y + maxRotation, 0f));
            }
        }

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }
}
