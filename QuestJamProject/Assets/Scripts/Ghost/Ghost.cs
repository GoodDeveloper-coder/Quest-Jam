using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour, IEnemy
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

    [SerializeField] float _speed;
    [SerializeField] float _waitTime;
    public List<Transform> _moveSpots;

    private int randomSpot;

    private Transform _target;

    public bool _canMove = true;

    private Vector3 _startPos;

    #endregion

    #region Monobehaviour Functions

    void Start()
    {
        _startPos = transform.position;
        _ghostSpriteRenderer = GetComponent<SpriteRenderer>();
        _minimapSpriteRenderer.enabled = true;
        //InitializeGhost();
        //StartCoroutine(Move());
    }

    void Update()
    {
        /*
        if (Input.GetMouseButtonUp(0))
        {
            _canMove = true;
            StartCoroutine(Move());
        }
        */

        if (_canMove == false)
        {
            MoveToVacumCleaner();
        }
    }

    #endregion
    #region Initialize Ghost
    public void InitializeGhost()
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

        StartCoroutine(Move());
    }
    #endregion

    public IEnumerator Move()
    {
        while (_canMove)
        {
            randomSpot = Random.Range(0, _moveSpots.Count);
            while (Vector3.Distance(transform.position, _moveSpots[randomSpot].position) > 0.2f)
            {
                if (_canMove)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _moveSpots[randomSpot].position, _speed * Time.deltaTime);
                }
                yield return null;
            }
            yield return new WaitForSeconds(_waitTime);
        }
    }

    public void MoveToVacumCleaner()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _canMove = true;
            StartCoroutine(Move());
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 5f * Time.deltaTime);
        }
    }

    public void SetGhostFields(Transform suckUpPosition)
    {
        _target = suckUpPosition;
        _canMove = false;
    }

    public Vector3 GetGhostStartPos()
    {
        return _startPos;
    }
}
