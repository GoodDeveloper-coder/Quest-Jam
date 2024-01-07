using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;

    [SerializeField] float speed;
    [SerializeField] float waitTime;
    [SerializeField] List<Transform> _moveSpots;

    private int randomSpot;

    private Transform _target;

    public bool _canMove = true;

    void Start()
    {
        _spriteRenderer.enabled = true;
        StartCoroutine(Move());
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _canMove = true;
            StartCoroutine(Move());
        }

        if (_canMove == false)
        {
            MoveToPlayer();
        }
    }

    IEnumerator Move()
    {
        while (_canMove)
        {
            randomSpot = Random.Range(0, _moveSpots.Count);
            while (Vector3.Distance(transform.position, _moveSpots[randomSpot].position) > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _moveSpots[randomSpot].position, speed * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    void MoveToPlayer()
    {
        StopCoroutine(Move());
        transform.position = Vector2.MoveTowards(transform.position, _target.transform.position, 3f * Time.deltaTime);
    }

    public void SetGhostFields(Transform playerTransform)
    {
        _target = playerTransform;
        _canMove = false;
    }
}
