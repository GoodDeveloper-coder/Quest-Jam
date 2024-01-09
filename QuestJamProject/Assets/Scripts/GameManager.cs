using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] GameObject _fadeIn;
    [SerializeField] GameObject _fadeOut;

    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private List<Transform> _ghostsSpawnPositions;
    [SerializeField] private List<Transform> _ghostMoveSpots;

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
        SpawnGhosts();

        Score = 0;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(RestartCycle());
        }
    }


    //Use this void to add score
    public void AddScore(int score)
    {
        Score += score;
    }

    #region Restart Cycle

    public IEnumerator RestartCycle()
    {
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
        foreach (Transform spawnPos in _ghostsSpawnPositions)
        {
            int spawnOrNo = Random.Range(0, 2);

            if (spawnOrNo == 1)
            {
                int typeOfGhostInt = Random.Range(0, 4);

                Ghost.TypesOfGhosts typeOfGhost = new Ghost.TypesOfGhosts();

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

                Ghost ghost = Instantiate(_ghostPrefab, spawnPos.position, Quaternion.identity).GetComponent<Ghost>();
                ghost._typeOfGhosts = typeOfGhost;

                foreach(Transform moveSpot in _ghostMoveSpots)
                {
                    int moveSpotChance = Random.Range(0, 3);
                    Debug.Log(moveSpotChance);
                    if (moveSpotChance == 1)
                    {
                        ghost._moveSpots.Add(moveSpot);
                    }
                }

                if (ghost._moveSpots.Count >= 0)
                {
                    ghost._moveSpots.Add(_ghostMoveSpots[Random.Range(0, _ghostMoveSpots.Count)]);
                }

                ghost.InitializeGhost();
            }
        }
    } 


    #endregion

}
