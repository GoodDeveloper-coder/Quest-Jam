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

    [SerializeField] Animator _ghostAnimator;

    [SerializeField] RuntimeAnimatorController _angerGhostAnimator, _depressionGhostAnimator, _anxietyGhostAnimator, _envyGhostAnimator;
    #endregion

    #region Fields
    [SerializeField] SpriteRenderer _minimapSpriteRenderer;

    [SerializeField] float _speed;
    [SerializeField] float _waitTime;
    public List<Transform> _moveSpots;

    private int randomSpot;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Transform _target;

    public bool _canMove = true;

    private Vector3 _startPos;

    private Vector3 _deffaultScale;

    private Vector3[] path;
    private int pathIndex;

    #endregion

    #region Monobehaviour Functions

    void Start()
    {
        _startPos = transform.position;
        _deffaultScale = transform.localScale;
        _ghostAnimator = GetComponent<Animator>();
        _minimapSpriteRenderer.enabled = true;
        //InitializeGhost();
        StartCoroutine(Move());
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
        else
        {
            if (transform.localScale.x < _deffaultScale.x || transform.localScale.y < _deffaultScale.y)
            {
                transform.localScale += new Vector3(0.005f, 0.005f, 0f);
            }
        }
    }

    #endregion
    #region Initialize Ghost
    public void InitializeGhost()
    {
        switch (_typeOfGhosts)
        {
            case TypesOfGhosts.Anger:
                _ghostAnimator.runtimeAnimatorController = _angerGhostAnimator;
                break;

            case TypesOfGhosts.Depression:
                _ghostAnimator.runtimeAnimatorController = _depressionGhostAnimator;
                break;

            case TypesOfGhosts.Anxiety:
                _ghostAnimator.runtimeAnimatorController = _anxietyGhostAnimator;
                break;


            case TypesOfGhosts.Envy:
                _ghostAnimator.runtimeAnimatorController = _envyGhostAnimator;
                break;
        }

        StartCoroutine(Move());
    }
    #endregion

    public IEnumerator Move()
    {
        /*
        while (_canMove)
        {
            randomSpot = Random.Range(0, _moveSpots.Count);
            while (Vector3.Distance(transform.position, _moveSpots[randomSpot].position) > 0.2f)
            {
                if (_canMove)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _moveSpots[randomSpot].position, _speed * Time.deltaTime);
                    _ghostAnimator.SetFloat("Speed", 1f);
                }
                yield return null;
            }

            if (Vector3.Distance(transform.position, _moveSpots[randomSpot].position) < 0.2f)
            {
                _ghostAnimator.SetFloat("Speed", 0f);
            }

            yield return new WaitForSeconds(_waitTime);
        }
        */
        while (_canMove)
        {
            if (path == null) yield return null;
            else
            {
                Vector3 target = path[pathIndex];
                while (Vector3.Distance(transform.position, target) > float.Epsilon)
                {
                    if (_canMove) transform.position = Vector3.MoveTowards(transform.position, target, _speed * Time.deltaTime);
                    yield return null;
                }
                pathIndex = (pathIndex + 1) % path.Length;
                yield return new WaitForSeconds(_waitTime);
            }
        }
    }

    public void MoveToVacumCleaner()
    {
        /*
        if (Input.GetMouseButtonUp(0))
        {
            _canMove = true;
            StopAllCoroutines();
            StartCoroutine(Move());
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 5f * Time.deltaTime);

            if (Vector2.Distance(transform.position, _target.transform.position) <= 5f)
            {
                if (transform.localScale.x <= 0.4f && transform.localScale.y <= 0.4f)
                {

                }
                else
                {
                    transform.localScale -= new Vector3(0.005f, 0.005f, 0f);
                }
            }
        }
        */
        transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 5f * Time.deltaTime);
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

    private void OnTriggerExit2D(Collider2D other)
    {
        /*
        if (other.name == "Trigger")
        {
            _canMove = true;
            StopAllCoroutines();
            StartCoroutine(Move());
        }
        */
    }

    public void SetPath(Vector3[] p)
    {
        if (p == null) return;
        path = p;
    }
}
