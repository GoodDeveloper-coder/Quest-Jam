using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost2 : MonoBehaviour, IEnemy
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
    [SerializeField] List<Transform> _moveSpots;

    private int randomSpot;

    private Transform _target;

    public bool _canMove = true;

    private Vector3[] path;
    private int pathIndex;
    private bool caught;
    private bool locked;

    #endregion

    #region Monobehaviour Functions

    void Start()
    {
        _ghostSpriteRenderer = GetComponent<SpriteRenderer>();
        _minimapSpriteRenderer.enabled = true;
        InitializeGhost();
        //StartCoroutine(Move());

        locked = true;
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

        if (caught || locked) return;

        if (_canMove == false)
        {
            StopAllCoroutines();
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
    }
    #endregion

    public IEnumerator Move()
    {
        while (_canMove && !locked)
        {
            /*
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
            */
            // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // the ghost needs to use a preset path, otherwise if they always choose a random point then this will not work with the time loop effect (they are supposed to follow the same path every cycle)
            // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            while (Vector3.Distance(transform.position, path[pathIndex]) > float.Epsilon)
            {
                if (_canMove && !locked) transform.position = Vector3.MoveTowards(transform.position, path[pathIndex], _speed * Time.deltaTime);
                yield return null;
            }
            pathIndex = (pathIndex + 1) % path.Length;
            yield return new WaitForSeconds(_waitTime);
        }
    }

    public void MoveToVacumCleaner()
    {
        /*
        if (Input.GetMouseButtonUp(0))
        {
            _canMove = true;
            StartCoroutine(Move());
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 5f * Time.deltaTime);
        }
        */
        // ---------------------------------------------------------------------------------------------------------------------------------------------------
        // once a ghost is being sucked up by the vacuum, they should not be able to escape or else this will mess up their synchronisation with the time loop
        // ---------------------------------------------------------------------------------------------------------------------------------------------------
        while (!caught && !locked) transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 5f * Time.deltaTime);
    }

    public void SetGhostFields(Transform suckUpPosition)
    {
        _target = suckUpPosition;
        _canMove = false;
    }

    public void SetPath(Vector3[] p)
    {
        if (p == null) return;
        List<Vector3> pathList = new List<Vector3>(p);
        pathList.Add(transform.position);
        path = pathList.ToArray();
    }

    public void ResetTimeLoop()
    {
        transform.position = path[path.Length - 1];
        pathIndex = 0;
        if (!locked && !caught) StartCoroutine(Move());
    }

    public void SetLocked(bool l)
    {
        locked = l;
    }

    public void SetCaught(bool c)
    {
        caught = c;
    }
}
