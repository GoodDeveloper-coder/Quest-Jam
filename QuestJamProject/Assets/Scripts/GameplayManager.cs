using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private GameObject panelHUD;
    [SerializeField] private GameObject panelResetCycle;
    [SerializeField] private GameObject panelScore;

    [SerializeField] private TextMeshProUGUI textTimeLeft;
    [SerializeField] private TextMeshProUGUI textTotalTime;
    [SerializeField] private TextMeshProUGUI textGhostsPerCycle;
    [SerializeField] private TextMeshProUGUI textFinalScore;
    
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
    private float finalScore;
    
    // Start is called before the first frame update
    void Start()
    {
        panelResetCycle.SetActive(false);
        panelScore.SetActive(false);
        ghosts = new GhostMovement[ghostsPerColour * 4];
        List<Vector2> positions = new List<Vector2>();
        for (int i = -mapWidth; i <= mapWidth; i++) for (int j = -mapHeight; j <= mapHeight; j++) positions.Add(new Vector2(i, j));
        bool[] occupied = new bool[positions.Count];
        occupied[occupied.Length / 2] = true;
        for (int i = 0; i < ghosts.Length; i++)
        {
            List<Vector2> path = new List<Vector2>();
            int r;
            do r = Random.Range(0, occupied.Length);
            while (occupied[r]);
            occupied[r] = true;
            bool b = Random.Range(0, 2) == 0;
            int pr = (int)(b ? positions[r].x : positions[r].y);
            Vector2 currentPosition = positions[r];
            ghosts[i] = Instantiate(ghostPrefab, transform.position + Vector3.right * currentPosition.x + Vector3.up * currentPosition.y, transform.rotation).GetComponent<GhostMovement>();
            for (int j = 0; j < 40; j++)
            {
                Vector2 newPosition = currentPosition;
                if (b ? j % 2 == 0 : j % 2 == 1)
                {
                    do r = Random.Range(-mapWidth, mapWidth);
                    while (r == pr);
                    currentPosition.x = r;
                    pr = (int)currentPosition.y;
                }
                else
                {
                    do r = Random.Range(-mapHeight, mapHeight);
                    while (r == pr);
                    currentPosition.y = r;
                    pr = (int)currentPosition.x;
                }
                path.Add(currentPosition);
            }
            ghosts[i].SetPath(path.ToArray());
            ghosts[i].SetMoving(false);
        }
        StartCoroutine(IStartCycle());
    }

    // Update is called once per frame
    void Update()
    {
        if (ticking) elapsedSeconds += Time.deltaTime;
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
        panelHUD.SetActive(false);
        panelResetCycle.SetActive(true);
        yield return new WaitForSeconds(secondsToResetCycle);
        player.ResetPosition();
        foreach (GhostMovement ghost in ghosts) ghost.ResetPosition();
        panelHUD.SetActive(false);
        panelResetCycle.SetActive(false);
        StartCoroutine(IStartCycle());
    }

    private void EndGame()
    {
        ticking = false;
        player.SetLocked(true);
        panelHUD.SetActive(false);
        panelResetCycle.SetActive(false);
        panelScore.SetActive(true);
        float ghostsPerCycle = totalGhostsCaught * 1f / cycles;
        finalScore = ghostsPerCycle * 10000f / elapsedSeconds;
        textTotalTime.text = (int)elapsedSeconds + "";
        textGhostsPerCycle.text = (int)ghostsPerCycle + "";
        textFinalScore.text = (int)finalScore + "";
    }
}
