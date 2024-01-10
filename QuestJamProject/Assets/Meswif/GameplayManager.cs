using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private GameObject panelHUD;
    [SerializeField] private GameObject panelResetCycle;
    [SerializeField] private GameObject panelScore;

    [SerializeField] private GameObject warp;

    [SerializeField] private GameObject[] timer;

    [SerializeField] private TextMeshProUGUI[] textGhostsLeft;
    [SerializeField] private TextMeshProUGUI textTotalTime;
    [SerializeField] private TextMeshProUGUI textGhostsPerCycle;
    [SerializeField] private TextMeshProUGUI textFinalScore;
    [SerializeField] private TextMeshProUGUI textPersonalBest;
    
    [SerializeField] private PlayerMovement player;
    [SerializeField] private VacumCleaner vacuum;

    [SerializeField] private int mapWidth = 12;
    [SerializeField] private int mapHeight = 12;
    
    [SerializeField] private int ghostTypes = 4;
    [SerializeField] private int ghostsPerColour = 13;
    [SerializeField] private float secondsInCycle = 60f;
    [SerializeField] private float secondsToResetCycle = 2.5f;

    [SerializeField] private GameObject ghostPrefab;

    private Ghost[] ghosts;
    private Vector3[] ghostStartPositions;
    private string[] ghostPaths;

    private bool ticking;
    private float elapsedSeconds;
    private int totalGhostsCaught;
    private int cycles;
    
    // Start is called before the first frame update
    void Start()
    {
        panelResetCycle.SetActive(false);
        panelScore.SetActive(false);
        ghosts = new Ghost[ghostsPerColour * ghostTypes];
        ghostStartPositions = new Vector3[ghosts.Length];
        ghostPaths = new string[ghosts.Length];
        List<Vector3> positions = new List<Vector3>();
        for (int i = -mapWidth; i <= mapWidth; i++) for (int j = -mapHeight; j <= mapHeight; j++) positions.Add(transform.position + Vector3.right * i + Vector3.up * j);
        bool[] occupied = new bool[positions.Count];
        occupied[occupied.Length / 2] = true;
        for (int i = 0; i < ghosts.Length; i++)
        {
            int r;
            do r = Random.Range(0, occupied.Length);
            while (occupied[r]);
            occupied[r] = true;
            Vector3 currentPosition = positions[r];
            ghostStartPositions[i] = currentPosition;
            string path = "";
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
                path += (j == 0 ? "" : ",") + currentPosition.x + "," + currentPosition.y;
            }
            ghostPaths[i] = path;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!ticking) return;
        Ghost ghost = vacuum.GetGhost();
        if (ghost != null)
        {
            bool allCaught = true;
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (ghosts[i] == ghost)
                {
                    Destroy(ghost.gameObject);
                    ghosts[i] = null;
                }
                else allCaught &= ghosts[i] == null;
            }
            if (allCaught)
            {
                ticking = false;
                player.SetLocked(true);
                panelHUD.SetActive(false);
                panelResetCycle.SetActive(false);
                panelScore.SetActive(true);
                warp.SetActive(true);
                int minutes = (int)elapsedSeconds / 60;
                int seconds = (int)elapsedSeconds % 60;
                float ghostsPerCycle = totalGhostsCaught * 1f / cycles;
                float score = ghostsPerCycle * 10000f / elapsedSeconds;
                textTotalTime.text = minutes + (seconds < 10 ? ":0" : ":") + seconds;
                textGhostsPerCycle.text = (int)ghostsPerCycle + "";
                textFinalScore.text = (int)score + "";
                if (PlayerPrefs.HasKey("HighScore"))
                {
                    int previousBest = PlayerPrefs.GetInt("HighScore");
                    if (score < previousBest) textPersonalBest.text = "Personal Best: " + previousBest;
                    else PlayerPrefs.SetInt("HighScore", (int)score);
                }
                else PlayerPrefs.SetInt("HighScore", (int)score);
               return;
            }
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
        int[] ghostsLeft = new int[ghostTypes];
        player.SetLocked(false);
        for (int i = 0; i < ghosts.Length; i++) if (ghosts[i] != null || cycles == 0) ghostsLeft[i % ghostTypes]++;
        for (int i = 0; i < ghostTypes; i++) textGhostsLeft[i].text = ghostsLeft[i] + "";
        for (int i = 0; i < ghosts.Length; i++)
        {
            if (ghosts[i] != null) Destroy(ghosts[i].gameObject);
            if(ghostsLeft[i % ghostTypes] > 0)
            {
                ghosts[i] = Instantiate(ghostPrefab, ghostStartPositions[i], transform.rotation).GetComponent<Ghost>();
                switch (i % ghostTypes)
                {
                    case 0:
                        ghosts[i]._typeOfGhosts = Ghost.TypesOfGhosts.Anger;
                        break;
                    case 1:
                        ghosts[i]._typeOfGhosts = Ghost.TypesOfGhosts.Depression;
                        break;
                    case 2:
                        ghosts[i]._typeOfGhosts = Ghost.TypesOfGhosts.Envy;
                        break;
                    case 3:
                        ghosts[i]._typeOfGhosts = Ghost.TypesOfGhosts.Anxiety;
                        break;
                }
                ghosts[i].InitializeGhost();
                List<Vector3> path = new List<Vector3>();
                string[] split = ghostPaths[i].Split(',');
                Vector3 v = ghostStartPositions[i];
                for (int j = 0; j < split.Length - 1; j += 2)
                {
                    v.x = float.Parse(split[j]);
                    v.y = float.Parse(split[j + 1]);
                    path.Add(v);
                }
                ghosts[i].SetPath(path.ToArray());
            }
        }
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
        for (int i = 0; i < ghosts.Length; i++) if (ghosts[i] != null) ghosts[i].SetLocked();
        panelHUD.SetActive(false);
        panelResetCycle.SetActive(true);
        warp.SetActive(true);
        yield return new WaitForSeconds(secondsToResetCycle);
        player.ResetPosition();
        panelHUD.SetActive(true);
        panelResetCycle.SetActive(false);
        warp.SetActive(false);
        foreach (GameObject t in timer) t.SetActive(true);
        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition.x = player.transform.position.x;
        cameraPosition.y = player.transform.position.y;
        Camera.main.transform.position = cameraPosition;
        StartCoroutine(IStartCycle());
    }
}
