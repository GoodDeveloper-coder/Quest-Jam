using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private GameObject panelHUD;
    [SerializeField] private GameObject panelResetCycle;
    [SerializeField] private GameObject panelScore;

    [SerializeField] private GameObject[] timer;

    [SerializeField] private TextMeshProUGUI textTotalTime;
    [SerializeField] private TextMeshProUGUI textGhostsPerCycle;
    [SerializeField] private TextMeshProUGUI textFinalScore;
    
    [SerializeField] private PlayerMovement player;

    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    
    [SerializeField] private int ghostsPerColour;
    [SerializeField] private float secondsInCycle;
    [SerializeField] private float secondsToResetCycle;

    [SerializeField] private GameObject[] ghostPrefabs;

    private GhostMovement[] ghosts;

    private bool ticking;
    private float elapsedSeconds;
    private int[] ghostsCaughtPerColour;
    private int totalGhostsCaught;
    private int cycles;
    private float finalScore;
    
    // Start is called before the first frame update
    void Start()
    {
        panelResetCycle.SetActive(false);
        panelScore.SetActive(false);
        ghosts = new GhostMovement[ghostsPerColour * ghostPrefabs.Length];
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
            ghosts[i] = Instantiate(ghostPrefabs[i % ghostPrefabs.Length], transform.position + Vector3.right * currentPosition.x + Vector3.up * currentPosition.y, transform.rotation).GetComponent<GhostMovement>();
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
        ghostsCaughtPerColour = new int[ghostPrefabs.Length];
    }

    // Update is called once per frame
    void Update()
    {
        if (!ticking) return;
        bool allCaught = true;
        foreach (GhostMovement g in ghosts) allCaught &= g.GetCaught();
        if(allCaught)
        {
            ticking = false;
            player.SetLocked(true);
            panelHUD.SetActive(false);
            panelResetCycle.SetActive(false);
            panelScore.SetActive(true);
            int minutes = (int)elapsedSeconds / 60;
            int seconds = (int)elapsedSeconds % 60;
            float ghostsPerCycle = totalGhostsCaught * 1f / cycles;
            finalScore = ghostsPerCycle * 10000f / elapsedSeconds;
            textTotalTime.text = minutes + (seconds < 10 ? ":0" : ":") + seconds;
            textGhostsPerCycle.text = (int)ghostsPerCycle + "";
            textFinalScore.text = (int)finalScore + "";
            return;
        }
        elapsedSeconds += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R)) ResetCycle();
    }

    public void StartCycle()
    {
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
        float timeLeft = secondsInCycle;
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            for (int i = 0; i < timer.Length; i++) timer[i].SetActive(i * 1f / timer.Length < timeLeft / secondsInCycle);
            yield return null;
        }
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
        panelHUD.SetActive(true);
        panelResetCycle.SetActive(false);
        StartCoroutine(IStartCycle());
    }
}
