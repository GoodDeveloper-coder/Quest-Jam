using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenu : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 span;
    [SerializeField] private GameObject[] ghosts;

    private bool leftToRight;
    private bool moving;
    private int currentGhost;
    
    // Start is called before the first frame update
    void Start()
    {
        leftToRight = Random.Range(0, 2) == 0;
        Vector3 p = transform.position;
        p.x = leftToRight ? -Mathf.Abs(span.x) : Mathf.Abs(span.x);
        transform.position = p;
        if (!leftToRight)
        {
            Vector3 s = transform.localScale;
            s.x *= -1;
            transform.localScale = s;
        }
        GetComponent<Animator>().SetFloat("Speed", 1f);
        currentGhost = Random.Range(0, ghosts.Length);
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].GetComponent<Animator>().SetFloat("Speed", 1f);
            ghosts[i].SetActive(i == currentGhost);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving)
        {
            moving = true;
            StartCoroutine(Move());
        }
    }

    private IEnumerator Move()
    {
        Vector3 p = transform.position;
        float y;
        do y = Random.Range(-Mathf.Abs(span.y), Mathf.Abs(span.y));
        while (Mathf.Abs(y - p.y) < Mathf.Abs(span.y) / 4);
        p.y = y;
        transform.position = p;
        yield return null;
        if (leftToRight)
        {
            while (p.x < Mathf.Abs(span.x))
            {
                p.x += Mathf.Abs(moveSpeed) * Time.deltaTime;
                transform.position = p;
                yield return null;
            }
        }
        else
        {
            while (p.x > -Mathf.Abs(span.x))
            {
                p.x -= Mathf.Abs(moveSpeed) * Time.deltaTime;
                transform.position = p;
                yield return null;
            }
        }
        leftToRight = !leftToRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
        int r;
        do r = Random.Range(0, ghosts.Length);
        while (r == currentGhost);
        currentGhost = r;
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].SetActive(true);
            ghosts[i].GetComponent<Animator>().SetFloat("Speed", 1f);
            ghosts[i].SetActive(i == currentGhost);
        }
        moving = false;
    }
}
