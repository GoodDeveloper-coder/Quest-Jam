using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] GameObject _fadeIn;
    [SerializeField] GameObject _fadeOut;

    [SerializeField] List<GameObject> _objectToOpenIfWin;

    [SerializeField] List<GameObject> _objectToResetIfWin;

    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private List<Transform> _ghostsSpawnPositions;
    [SerializeField] private List<Transform> _ghostMoveSpots;

    [SerializeField] private List<Transform> _ghostPatruleMarshroots;

    [SerializeField] private GameObject _player;

    [SerializeField] private Timer _timer;

    [SerializeField] List<TextMeshProUGUI> _countOfGhostsUIText;

    [SerializeField] TextMeshProUGUI _timeTakenUIText;
    [SerializeField] TextMeshProUGUI _ghostsCaughtPerCycleUIText;
    [SerializeField] TextMeshProUGUI _finalScoreUIText;
    [SerializeField] TextMeshProUGUI _personalBestUIText;

    private float totalSeconds;
    private int totalGhostsCaught;
    private int cycles;
    private bool paused;

    private Vector3 _playerStartPos;

    private int _score;

    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            _scoreText.text = $"Score: {_score}";
        }
    }

    private void Start()
    {
        cycles = 1;

        SpawnGhosts();

        Score = 0;

        _playerStartPos = _player.transform.position;

        SetUICountOfGhosts();
    }


    private void Update()
    {
        if (!paused) totalSeconds += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(RestartCycle());
        }

        CheckPlayerWin();
    }


    //Use this void to add score
    public void AddScore(int score)
    {
        Score += score;
    }

    #region Restart Cycle

    public IEnumerator RestartCycle()
    {
        paused = true;
        StartCoroutine(StartFades());

        yield return new WaitForSeconds(1f);
        
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject ghost in ghosts)
        {
            Ghost ghostScript = ghost.GetComponent<Ghost>();
            if (ghostScript != null)
            {
                ghost.transform.position = ghost.GetComponent<Ghost>().GetGhostStartPos();
            }
        }

        _player.transform.position = _playerStartPos;
        _timer.Being(_timer.Duration);
        cycles++;
        paused = false;
    }

    #endregion

    #region Fades

    public IEnumerator StartFades()
    {
        _fadeIn.SetActive(false);
        _fadeIn.SetActive(true);
        yield return new WaitForSeconds(1f);
        _fadeOut.SetActive(true);
        _fadeIn.SetActive(false);
        yield return new WaitForSeconds(1f);
        _fadeOut.SetActive(false);
    }

    #endregion

    #region Spawn Ghosts

    void SpawnGhosts()
    {
        int indexOfPatruleMarshroot = 0;

        int numberInOrder = 1;

        foreach (Transform spawnPos in _ghostsSpawnPositions)
        {
            int spawnOrNo = Random.Range(0, 2);

            if (spawnOrNo == 1)
            {
                int typeOfGhostInt = Random.Range(0, 4);

                Ghost.TypesOfGhosts typeOfGhost = new Ghost.TypesOfGhosts();

                switch (numberInOrder)
                {
                    case 1:
                        typeOfGhost = Ghost.TypesOfGhosts.Anger;
                        numberInOrder++;
                        break;

                    case 2:
                        typeOfGhost = Ghost.TypesOfGhosts.Depression;
                        numberInOrder++;
                        break;

                    case 3:
                        typeOfGhost = Ghost.TypesOfGhosts.Anxiety;
                        numberInOrder++;
                        break;

                    case 4:
                        typeOfGhost = Ghost.TypesOfGhosts.Envy;
                        numberInOrder = 1;
                        break;
                }
                /*
                switch (typeOfGhostInt)
                {
                    case 0:
                        typeOfGhost = Ghost.TypesOfGhosts.Anger;
                        break;

                    case 1:
                        typeOfGhost = Ghost.TypesOfGhosts.Depression;
                        break;

                    case 2:
                        typeOfGhost = Ghost.TypesOfGhosts.Anxiety;
                        break;

                    case 3:
                        typeOfGhost = Ghost.TypesOfGhosts.Envy;
                        break;
                }
                */
                Ghost ghost = Instantiate(_ghostPrefab, spawnPos.position, Quaternion.identity).GetComponent<Ghost>();
                ghost._typeOfGhosts = typeOfGhost;

                /*
                foreach(Transform moveSpot in _ghostMoveSpots)
                {
                    int moveSpotChance = Random.Range(0, 3);
                    Debug.Log(moveSpotChance);
                    if (moveSpotChance == 1)
                    {
                        if (ghost._moveSpots.Count < 2)
                        {
                            ghost._moveSpots.Add(moveSpot);
                        }
                    }
                }

                if (ghost._moveSpots.Count <= 0)
                {
                    ghost._moveSpots.Add(_ghostMoveSpots[Random.Range(0, _ghostMoveSpots.Count)]);
                }
                */

                indexOfPatruleMarshroot += 2;

                ghost.transform.position = _ghostPatruleMarshroots[indexOfPatruleMarshroot - Random.Range(1, 3)].position;

                ghost._moveSpots.Add(_ghostPatruleMarshroots[indexOfPatruleMarshroot - 2]);
                ghost._moveSpots.Add(_ghostPatruleMarshroots[indexOfPatruleMarshroot - 1]);

                ghost.InitializeGhost();
            }
        }
    } 


    #endregion

    void CheckPlayerWin()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Enemy");

        if (ghosts.Count() == 0)
        {
            foreach(GameObject winObject in _objectToOpenIfWin) 
            {
                winObject.SetActive(true);
            }

            foreach (GameObject winObject in _objectToResetIfWin)
            {
                winObject.SetActive(false);
            }

            foreach(GameObject ghostToReset in ghosts)
            {
                ghostToReset.SetActive(false);
            }

            int m = (int)totalSeconds / 60;
            int s = (int)totalSeconds % 60;
            float ghostsPerCycle = totalGhostsCaught * 1f / cycles;
            int score = (int)(ghostsPerCycle * 10000f / totalSeconds);
            _timeTakenUIText.text = m + (s < 10 ? ":0" : ":") + s;
            _ghostsCaughtPerCycleUIText.text = (int)ghostsPerCycle + "";
            _finalScoreUIText.text = score + "";
            if (PlayerPrefs.HasKey("HighScore"))
            {
                int highScore = PlayerPrefs.GetInt("HighScore");
                if (score <= highScore) _personalBestUIText.text = "Personal best: " + highScore;
                else PlayerPrefs.SetInt("HighScore", score);
            }
            else PlayerPrefs.SetInt("HighScore", score);
        }
    }

    public void UpdateUICountOfGhosts(Ghost.TypesOfGhosts typeOfGhost)
    {

        int countOfAngerGhosts;

        int countOfDepressionGhosts;

        int countOfAnxietyGhosts;

        int countOfEnvyGhosts;

        switch (typeOfGhost)
        {
            case Ghost.TypesOfGhosts.Anger:
                int.TryParse(_countOfGhostsUIText[0].text, out countOfAngerGhosts);

                _countOfGhostsUIText[0].text = $"{countOfAngerGhosts - 1}";
                break;

            case Ghost.TypesOfGhosts.Depression:
                int.TryParse(_countOfGhostsUIText[1].text, out countOfDepressionGhosts);

                _countOfGhostsUIText[1].text = $"{countOfDepressionGhosts - 1}";
                break;

            case Ghost.TypesOfGhosts.Anxiety:
                int.TryParse(_countOfGhostsUIText[2].text, out countOfAnxietyGhosts);

                _countOfGhostsUIText[2].text = $"{countOfAnxietyGhosts - 1}";
                break;

            case Ghost.TypesOfGhosts.Envy:
                int.TryParse(_countOfGhostsUIText[3].text, out countOfEnvyGhosts);

                _countOfGhostsUIText[3].text = $"{countOfEnvyGhosts - 1}";
                break;
        }
         
    }

    void SetUICountOfGhosts()
    {

        Ghost[] allGhosts = GameObject.FindObjectsOfType<Ghost>();
        foreach (Ghost ghost in allGhosts)
        {
            switch (ghost._typeOfGhosts)
            {
                case Ghost.TypesOfGhosts.Anger:
                    int countOfAngerGhosts;
                    int.TryParse(_countOfGhostsUIText[0].text, out countOfAngerGhosts);

                    _countOfGhostsUIText[0].text = $"{countOfAngerGhosts + 1}";
                    break;

                case Ghost.TypesOfGhosts.Depression:
                    int countOfDepressionGhosts;
                    int.TryParse(_countOfGhostsUIText[1].text, out countOfDepressionGhosts);

                    _countOfGhostsUIText[1].text = $"{countOfDepressionGhosts + 1}";
                    break;

                case Ghost.TypesOfGhosts.Anxiety:
                    int countOfAnxietyGhosts;
                    int.TryParse(_countOfGhostsUIText[2].text, out countOfAnxietyGhosts);

                    _countOfGhostsUIText[2].text = $"{countOfAnxietyGhosts + 1}";
                    break;

                case Ghost.TypesOfGhosts.Envy:
                    int countOfEnvyGhosts;
                    int.TryParse(_countOfGhostsUIText[3].text, out countOfEnvyGhosts);

                    _countOfGhostsUIText[3].text = $"{countOfEnvyGhosts + 1}";
                    break;
            }
        }
    }
}
