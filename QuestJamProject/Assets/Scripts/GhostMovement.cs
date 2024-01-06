using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    
    private Vector2[] path;
    private int pathIndex;
    private Vector3 target;
    private bool moving;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (path == null || !moving) return;
        transform.position += Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
        if ((transform.position - target).magnitude <= float.Epsilon)
        {
            transform.position = new Vector3(target.x, target.y, transform.position.z);
            pathIndex = (pathIndex + 1) % path.Length;
            target = transform.position + Vector3.right * path[pathIndex].x + Vector3.up * path[pathIndex].y;
        }
    }
    
    public void SetPath(Vector2[] p)
    {
        path = p;
        target = transform.position + Vector3.right * path[0].x + Vector3.up * path[0].y;
    }

    public void SetMoving(bool m)
    {
        //moving = m;
        //if (m) pathIndex = 1;
    }
}
