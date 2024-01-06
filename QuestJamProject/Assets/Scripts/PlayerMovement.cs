using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private Rigidbody2D rigidBody;
    private Vector2 startPosition;
    private bool moving;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        startPosition = rigidBody.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving) return;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Collider2D collider = Physics2D.OverlapBox(rigidBody.position + Vector2.up, Vector2.one / 2, 0);
            if (collider == null || collider.gameObject.tag != "Wall") StartCoroutine(Move(1));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Collider2D collider = Physics2D.OverlapBox(rigidBody.position - Vector2.up, Vector2.one / 2, 0);
            if (collider == null || collider.gameObject.tag != "Wall") StartCoroutine(Move(3));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Collider2D collider = Physics2D.OverlapBox(rigidBody.position - Vector2.right, Vector2.one / 2, 0);
            if (collider == null || collider.gameObject.tag != "Wall") StartCoroutine(Move(2));
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Collider2D collider = Physics2D.OverlapBox(rigidBody.position + Vector2.right, Vector2.one / 2, 0);
            if (collider == null || collider.gameObject.tag != "Wall") StartCoroutine(Move(0));
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //if (!moving || other.gameObject.tag != "Wall") return;
        //StopAllCoroutines();
        //rigidBody.MovePosition(startPosition);
        //moving = false;
    }

    private IEnumerator Move(int direction)
    {
        moving = true;
        Vector3 target = transform.position + (new Vector3[] { Vector3.right, Vector3.up, -Vector3.right, -Vector3.up })[direction];
        Vector2 target2D = new Vector2(target.x, target.y);
        float distance = (transform.position - target).sqrMagnitude;
        while (distance > float.Epsilon)
        {
            rigidBody.MovePosition(Vector2.MoveTowards(rigidBody.position, target2D, movementSpeed * Time.deltaTime));
            distance = (transform.position - target).sqrMagnitude;
            yield return null;
        }
        rigidBody.MovePosition(target2D);
        moving = false;
        startPosition = target2D;
    }
}
