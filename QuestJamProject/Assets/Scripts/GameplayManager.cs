using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    
    [SerializeField] private float timeLimit;

    [SerializeField] private GameObject ghostPrefab;

    private GameObject[] ghosts;
    
    // Start is called before the first frame update
    void Start()
    {
        if (ghosts == null) return;
        ghosts = new GameObject[10];
        Vector2[] positions = new Vector2[(mapWidth + mapHeight - 1) * 2];
        bool[] occupied = new bool[positions.Length];
        for (int i = 0; i < mapWidth; i++)
        {
            positions[i] = new Vector2(i + (1f - mapWidth) / 2, mapHeight * -0.5f);
            positions[i + mapWidth] = new Vector2(i + (1f - mapWidth) / 2, mapHeight * 0.5f);
        }
        for (int i = 1; i < mapHeight - 1; i++)
        {
            positions[i + mapWidth * 2] = new Vector2(mapWidth * -0.5f, i + (1f - mapHeight) / 2);
            positions[i + mapWidth * 2 + mapHeight - 1] = new Vector2(mapWidth * -0.5f, i + (1f - mapHeight) / 2);
        }
        for (int i = 0; i < ghosts.Length; i++)
        {
            List<Vector2> path = new List<Vector2>();
            int r;
            do r = Random.Range(0, positions.Length);
            while (occupied[r]);
            occupied[r] = true;
            bool b = r < mapWidth * 2;
            int pr = r < mapWidth ? 0 : r < mapWidth * 2 ? mapWidth : r < mapWidth * 2 + mapHeight ? 1 : mapHeight - 1;
            Vector2 currentPosition = positions[r];
            ghosts[i] = Instantiate(ghostPrefab, transform.position + Vector3.right * currentPosition.x + Vector3.up * currentPosition.y, transform.rotation);
            for (int j = 0; j < 40; j++)
            {
                Vector2 newPosition = currentPosition;
                if (b ? j % 2 == 0 : j % 2 == 1)
                {
                    do r = Random.Range(0, mapHeight);
                    while (r == pr);
                    currentPosition.y = r - mapHeight / 2f;
                    pr = (int)currentPosition.x;
                }
                else
                {
                    do r = Random.Range(0, mapWidth);
                    while (r == pr);
                    currentPosition.x = r - mapWidth / 2f;
                    pr = (int)currentPosition.y;
                }
                path.Add(currentPosition);
            }
            ghosts[i].GetComponent<GhostMovement>().SetPath(path.ToArray());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
