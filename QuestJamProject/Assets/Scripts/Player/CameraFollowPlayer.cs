using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject _target;

    [SerializeField] private bool _itsMnimap;

    [SerializeField] private float _smoothTime;

    private Vector3 _velocity = Vector3.zero;

    void LateUpdate()
    {
        if (_itsMnimap)
        {
            Vector3 followPlayer = Vector3.SmoothDamp(transform.position, _target.transform.position, ref _velocity, _smoothTime);
            transform.position = new Vector3(followPlayer.x, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, _target.transform.position, ref _velocity, _smoothTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }
}
