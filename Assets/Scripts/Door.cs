using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector3 startingRotation;
    public Vector3 startPosition;
    public float maxRotation = 90f;
    private bool open = false;
    private BaseCharacter mob;
    private float speed = 2f;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startingRotation = transform.rotation.eulerAngles;
        startPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && mob != null)
        {
            Open(mob.transform.position);
            Debug.Log("Open");
        }
    }

    // private void OnCollisionEnter(Collision other)
    // {
    //     if (other.collider.TryGetComponent<BaseCharacter>(out mob))
    //     {
    //         if (!open)
    //             Open(other.transform.position);
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.TryGetComponent<BaseCharacter>(out mob))
        {
            if (!open)
                Open(other.transform.position);
        }
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

            if (dot < 0)
            {
                endRotation = Quaternion.Euler(new Vector3(0f, startRotation.y - maxRotation, 0));
            }
            else
            {
                endRotation = Quaternion.Euler(new Vector3(0f, startRotation.y + maxRotation, 0));
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
