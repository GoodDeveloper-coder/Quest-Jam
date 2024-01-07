using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private Rigidbody2D rigidBody;
    private Vector2 origin;
    private Vector2 startPosition;
    private bool moving;
    private bool locked;
    private int direction;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        origin = rigidBody.position;
        startPosition = origin;
    }

    // Update is called once per frame
    void Update()
    {
        if (locked) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            // reset cycle
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // vacuum
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            // vacuum
        }
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
        else if (Input.GetKeyDown(KeyCode.D)) direction = 0;
        else if (Input.GetKeyDown(KeyCode.W)) direction = 1;
        else if (Input.GetKeyDown(KeyCode.A)) direction = 2;
        else if (Input.GetKeyDown(KeyCode.S)) direction = 3;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        
    }

    public void SetLocked(bool l)
    {
        locked = l;
        if (l)
        {
            StopAllCoroutines();
            moving = false;
        }
    }

    public void ResetPosition()
    {
        rigidBody.position = origin;
    }

    private IEnumerator Move(int direction)
    {
        moving = true;
        Vector2 target = rigidBody.position + (new Vector2[] { Vector2.right, Vector2.up, -Vector2.right, -Vector2.up })[direction];
        float distance = (rigidBody.position - target).sqrMagnitude;
        while (distance > float.Epsilon)
        {
            rigidBody.MovePosition(Vector2.MoveTowards(rigidBody.position, target, movementSpeed * Time.deltaTime));
            distance = (rigidBody.position - target).sqrMagnitude;
            yield return null;
        }
        rigidBody.MovePosition(target);
        moving = false;
        startPosition = target;
    }
}
