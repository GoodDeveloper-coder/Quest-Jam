using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private GameObject panelStartCycle;
    [SerializeField] private GameObject panelResetCycle;
    [SerializeField] private GameObject panelScore;

    [SerializeField] private TextMeshProUGUI textTimeLeft;
    
    [SerializeField] private PlayerMovement player;

    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    
    [SerializeField] private int ghostsPerColour;
    [SerializeField] private float secondsInCycle;
    [SerializeField] private float secondsToResetCycle;

    [SerializeField] private GameObject ghostPrefab;

    private GhostMovement[] ghosts;

    private bool ticking;
    private float elapsedSeconds;
    private int totalGhostsCaught;
    private int cycles;
    
    // Start is called before the first frame update
    void Start()
    {
        panelResetCycle.SetActive(false);
        panelScore.SetActive(false);
        ghosts = new GhostMovement[ghostsPerColour * 4];
        List<Vector2> positions = new List<Vector2>();
        for (int i = -mapWidth; i <= mapWidth; i++)
        {
            positions.Add(new Vector2(i, -mapHeight));
            positions.Add(new Vector2(i, mapHeight));
        }
        for (int i = 1 - mapHeight; i <= mapHeight - 1; i++)
        {
            positions.Add(new Vector2(-mapWidth, i));
            positions.Add(new Vector2(mapWidth, i));
        }
        bool[] occupied = new bool[positions.Count];
        for (int i = 0; i < ghosts.Length; i++)
        {
            List<Vector2> path = new List<Vector2>();
            int r;
            do r = Random.Range(0, occupied.Length);
            while (occupied[r]);
            occupied[r] = true;
            bool b = r < mapWidth * 2 + 1;
            int pr = b ? (r % 2 == 0 ? -mapHeight : mapHeight) : (r % 2 == 0 ? mapWidth : -mapWidth);
            Vector2 currentPosition = positions[r];
            path.Add(currentPosition);
            ghosts[i] = Instantiate(ghostPrefab, transform.position + Vector3.right * currentPosition.x + Vector3.up * currentPosition.y, transform.rotation).GetComponent<GhostMovement>();
            for (int j = 0; j < 40; j++)
            {
                Vector2 newPosition = currentPosition;
                if (b ? j % 2 == 0 : j % 2 == 1)
                {
                    do r = Random.Range(-mapHeight, mapHeight);
                    while (r == pr);
                    currentPosition.y = r;
                    pr = (int)currentPosition.x;
                }
                else
                {
                    do r = Random.Range(-mapWidth, mapWidth);
                    while (r == pr);
                    currentPosition.x = r;
                    pr = (int)currentPosition.y;
                }
                path.Add(currentPosition);
            }
            ghosts[i].SetPath(path.ToArray());
            ghosts[i].SetMoving(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ticking) elapsedSeconds += Time.deltaTime;
    }

    public void StartCycle()
    {
        StopAllCoroutines();
        StartCoroutine(IStartCycle());
    }

    public void ResetCycle()
    {
        StopAllCoroutines();
        StartCoroutine(IResetCycle());
    }

    private IEnumerator IStartCycle()
    {
        player.SetLocked(false);
        foreach (GhostMovement ghost in ghosts) ghost.SetMoving(true);
        cycles++;
        ticking = true;
        yield return new WaitForSeconds(secondsInCycle);
        ticking = false;
        panelResetCycle.SetActive(true);
        StartCoroutine(IResetCycle());
    }

    private IEnumerator IResetCycle()
    {
        ticking = false;
        player.SetLocked(true);
        foreach (GhostMovement ghost in ghosts) ghost.SetMoving(false);
        yield return new WaitForSeconds(secondsToResetCycle);
        player.ResetPosition();
        panelStartCycle.SetActive(true);
        panelResetCycle.SetActive(false);
    }

    private void EndGame()
    {
        ticking = false;
        panelStartCycle.SetActive(false);
        panelResetCycle.SetActive(false);
        panelScore.SetActive(false);
    }
}
