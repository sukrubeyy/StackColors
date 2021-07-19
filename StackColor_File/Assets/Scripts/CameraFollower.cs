using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] Transform target;
    float deltaZ;
    private void Start()
    {
        deltaZ = transform.position.z-target.position.z;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y,
        target.position.z + deltaZ), 1f);
    }

    public void SetTarget(Transform targetPos)
    {
        target = targetPos;
    }
}
