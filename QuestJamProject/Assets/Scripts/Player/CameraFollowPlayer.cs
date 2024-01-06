using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject _target;

    [SerializeField] private float _smoothTime;

    private Vector3 _velocity = Vector3.zero;

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _target.transform.position, ref _velocity, _smoothTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
