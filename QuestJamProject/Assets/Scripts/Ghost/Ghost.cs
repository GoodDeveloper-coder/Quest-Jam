using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    #region Ghost type

    public enum TypesOfGhosts
    {
        Anger, //(red ghosts)
        Depression, //(blue ghosts)
        Anxiety, //(purple ghosts)
        Envy //(green ghosts)
    }

    public TypesOfGhosts _typeOfGhosts;

    [SerializeField] SpriteRenderer _ghostSpriteRenderer;

    [SerializeField] Sprite _angerGhostSprite, _depressionGhostSprite, _anxietyGhostSprite, _envyGhostSprite;
    #endregion

    #region Fields
    [SerializeField] SpriteRenderer _minimapSpriteRenderer;

    [SerializeField] float speed;
    [SerializeField] float waitTime;
    [SerializeField] List<Transform> _moveSpots;

    private int randomSpot;

    private Transform _target;

    public bool _canMove = true;

    #endregion

    #region Monobehaviour Functions

    void Start()
    {
        _ghostSpriteRenderer = GetComponent<SpriteRenderer>();
        _minimapSpriteRenderer.enabled = true;
        InitializeGhost();
        StartCoroutine(Move());
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _canMove = true;
            StartCoroutine(Move());
        }

        if (_canMove == false)
        {
            MoveToPlayer();
        }
    }

    #endregion
    #region Initialize Ghost
    void InitializeGhost()
    {
        switch (_typeOfGhosts)
        {
            case TypesOfGhosts.Anger:
                _ghostSpriteRenderer.sprite = _angerGhostSprite;
                break;

            case TypesOfGhosts.Depression:
                _ghostSpriteRenderer.sprite = _depressionGhostSprite;
                break;

            case TypesOfGhosts.Anxiety:
                _ghostSpriteRenderer.sprite = _anxietyGhostSprite;
                break;


            case TypesOfGhosts.Envy:
                _ghostSpriteRenderer.sprite = _envyGhostSprite;
                break;
        }
    }
    #endregion

    IEnumerator Move()
    {
        while (_canMove)
        {
            randomSpot = Random.Range(0, _moveSpots.Count);
            while (Vector3.Distance(transform.position, _moveSpots[randomSpot].position) > 0.2f)
            {
                if (_canMove)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _moveSpots[randomSpot].position, speed * Time.deltaTime);
                }
                yield return null;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    void MoveToPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 5f * Time.deltaTime);
    }

    public void SetGhostFields(Transform playerTransform)
    {
        _target = playerTransform;
        _canMove = false;
    }
}
