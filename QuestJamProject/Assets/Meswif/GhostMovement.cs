using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    
    private Vector2[] path;
    private int pathIndex;
    private Vector3 origin;
    private Vector3 target;
    private bool moving;
    private bool inVacuum;
    private bool caught;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (path == null || !moving || caught) return;
        transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
        if ((transform.position - target).magnitude <= float.Epsilon)
        {
            transform.position = new Vector3(target.x, target.y, transform.position.z);
            pathIndex = (pathIndex + 1) % path.Length;
            target = Vector3.right * path[pathIndex].x + Vector3.up * path[pathIndex].y;
        }
    }

    public void SetPath(Vector2[] p)
    {
        path = p;
        target = Vector3.right * path[0].x + Vector3.up * path[0].y;
    }

    public void SetMoving(bool m)
    {
        moving = m;
    }

    public void ResetPosition()
    {
        transform.position = origin;
        pathIndex = 0;
    }

    public void SetCaught(bool c)
    {
        caught = c;
    }

    public bool GetCaught()
    {
        return caught;
    }
}