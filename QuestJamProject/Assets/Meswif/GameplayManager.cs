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

    [SerializeField] private TextMeshProUGUI[] textGhostsLeft;
    [SerializeField] private TextMeshProUGUI textTotalTime;
    [SerializeField] private TextMeshProUGUI textGhostsPerCycle;
    [SerializeField] private TextMeshProUGUI textFinalScore;
    
    [SerializeField] private PlayerMovement player;
    [SerializeField] private VacumCleaner vacuum;

    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    
    [SerializeField] private int ghostsPerColour;
    [SerializeField] private float secondsInCycle;
    [SerializeField] private float secondsToResetCycle;

    [SerializeField] private GameObject[] ghostPrefabs;

    private Ghost[] ghosts;

    private bool ticking;
    private float elapsedSeconds;
    private bool[] ghostsCaught;
    private int totalGhostsCaught;
    private int cycles;
    private float finalScore;
    
    // Start is called before the first frame update
    void Start()
    {
        /*
        panelResetCycle.SetActive(false);
        panelScore.SetActive(false);
        ghosts = new Ghost[ghostsPerColour * ghostPrefabs.Length];
        List<Vector3> positions = new List<Vector3>();
        for (int i = -mapWidth; i <= mapWidth; i++) for (int j = -mapHeight; j <= mapHeight; j++) positions.Add(transform.position + Vector3.right * i + Vector3.up * j);
        bool[] occupied = new bool[positions.Count];
        occupied[occupied.Length / 2] = true;
        for (int i = 0; i < ghosts.Length; i++)
        {
            List<Vector3> path = new List<Vector3>();
            int r;
            do r = Random.Range(0, occupied.Length);
            while (occupied[r]);
            occupied[r] = true;
            Vector3 currentPosition = positions[r];
            ghosts[i] = Instantiate(ghostPrefabs[i % ghostPrefabs.Length], currentPosition, transform.rotation).GetComponent<Ghost>();
            for (int j = 0; j < 40; j++)
            {
                Vector3 newPosition = currentPosition;
                do
                {
                    newPosition.x = Random.Range(-mapWidth, mapWidth);
                    newPosition.y = Random.Range(-mapHeight, mapHeight);
                    newPosition = Vector3.MoveTowards(currentPosition, newPosition, Mathf.Min(mapWidth, mapHeight) / Random.Range(2f, 4f));
                }
                while (Mathf.Abs(newPosition.x) > mapWidth && Mathf.Abs(newPosition.y) > mapHeight);
                currentPosition = newPosition;
                path.Add(currentPosition);
            }
            ghosts[i].SetPath(path.ToArray());
            ghosts[i].SetLocked(true);
        }
        ghostsCaught = new bool[ghosts.Length];
        */
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (!ticking) return;
        Ghost ghost = vacuum.GetGhost();
        if (ghost != null)
        {
            ghost.StopAllCoroutines();
            ghost.SetCaught(true);
            bool allCaught = true;
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (!ghostsCaught[i]) ghostsCaught[i] = ghosts[i] == ghost;
                allCaught &= ghostsCaught[i];
            }
            ghost.gameObject.SetActive(false);
            if (allCaught)
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
        }
        elapsedSeconds += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R)) ResetCycle();
        */
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
        /*
        int[] ghostsLeft = new int[ghostPrefabs.Length];
        player.SetLocked(false);
        for (int i = 0; i < ghosts.Length; i++)
        {
            if (!ghostsCaught[i])
            {
                ghostsLeft[i % ghostPrefabs.Length]++;
                ghosts[i].SetLocked(false);
                ghosts[i].ResetTimeLoop();
            }
        }
        for (int i = 0; i < ghostPrefabs.Length; i++) textGhostsLeft[i].text = ghostsLeft[i] + "";
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
        */
        yield return null;
        StartCoroutine(IResetCycle());
    }

    private IEnumerator IResetCycle()
    {
        /*
        ticking = false;
        player.SetLocked(true);
        foreach (Ghost ghost in ghosts) ghost.SetLocked(true);
        panelHUD.SetActive(false);
        panelResetCycle.SetActive(true);
        yield return new WaitForSeconds(secondsToResetCycle);
        player.ResetPosition();
        for (int i = 0; i < ghostPrefabs.Length; i++)
        {
            bool allCaught = true;
            for (int j = i; j < ghosts.Length; j += ghostPrefabs.Length) allCaught &= ghostsCaught[j];
            if (!allCaught)
            {
                for (int j = i; j < ghosts.Length; j += ghostPrefabs.Length)
                {
                    ghosts[j].gameObject.SetActive(true);
                    ghosts[i].SetCaught(false);
                }
            }
        }
        panelHUD.SetActive(true);
        panelResetCycle.SetActive(false);
        foreach (GameObject t in timer) t.SetActive(true);
        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition.x = player.transform.position.x;
        cameraPosition.y = player.transform.position.y;
        Camera.main.transform.position = cameraPosition;
        */
        yield return null;
        StartCoroutine(IStartCycle());
    }
}
