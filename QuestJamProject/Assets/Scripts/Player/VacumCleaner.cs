using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacumCleaner : MonoBehaviour
{
    #region Fields
    [Header("Fields")]

    [SerializeField] private GameManager _gameManager;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] GameObject _particles;

    [SerializeField] private PlayerMovement _playerMovement;

    [SerializeField] bool _ghostInZone;

    [SerializeField] LayerMask _ghostLayerMap;

    [SerializeField] Transform _checkGhost;

    [SerializeField] private float _attackCooldown;

    private GameObject _player;

    private float GroundCheckRadius = 1.7f;

    private bool _canAttack = true;

    private Vector3 deffaultPos;
    private Vector3 deffaultScale;

    private float _deffaultAttackCooldown;

    #endregion
    #region Monobehaviour Functions
    private void Start()
    {
        _player = transform.parent.gameObject;

        deffaultPos = _checkGhost.transform.localPosition;
        deffaultScale = _checkGhost.localScale;
        _deffaultAttackCooldown = _attackCooldown;
    }

    void Update()
    {
        RoatateWeapon();

        if (Input.GetMouseButton(0))
        {
            Attack();
            _particles.SetActive(true);
        }
        else
        {
            _particles.SetActive(false);
            _spriteRenderer.enabled = false;
        }
    }

    #endregion

    #region Attack Functions

    void Attack()
    {
        _ghostInZone = Physics2D.OverlapCircle(_checkGhost.position, GroundCheckRadius, _ghostLayerMap);
        _spriteRenderer.enabled = true;
        if (_ghostInZone)
        {
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(_checkGhost.position, GroundCheckRadius))
            {
                if (collider.CompareTag("Enemy"))
                {
                    if (_canAttack)
                    {
                        IEnemy ghostScript = collider.GetComponent<IEnemy>();
                        if (ghostScript != null)
                        {
                            ghostScript.SetGhostFields(transform);
                        }

                        StartCoroutine(AttackCooldown());
                    }
                }
            }

            //StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }
    #endregion
    #region Rotate Weapon 
    void RoatateWeapon()
    {

        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        mousePosition = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        transform.right = Vector3.Lerp(transform.right, mousePosition, Time.deltaTime * 7);

        Vector2 scale = transform.localScale;
        if (Mathf.Abs(transform.eulerAngles.z - 180) < 90)
        {
            scale.y = -1;
        }
        else
        {
            scale.y = 1;
        }
        transform.localScale = scale;
    }
    #endregion

    #region Ghost Effects

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ghost ghost = collision.transform.GetComponent<Ghost>();
        if (ghost != null)
        {
            if (ghost._canMove == false)
            {
                switch (ghost._typeOfGhosts)
                {
                    case Ghost.TypesOfGhosts.Anger:
                        StartCoroutine(AngerGhostEffect());
                        //increases your vacuum range
                        break;

                    case Ghost.TypesOfGhosts.Depression:
                        StartCoroutine(_playerMovement.BoostPlayerSpeed(5f, 5f)); // or _playerMovement.StartCoroutine(_playerMovement.BoostPlayerSpeed(7f, 5f));
                        //increases your walking speed
                        break;

                    case Ghost.TypesOfGhosts.Anxiety:
                        StartCoroutine(AnxietyGhostEffect());
                        //makes your vacuum suck up ghosts faster (you cannot move while sucking up a ghost)
                        break;


                    case Ghost.TypesOfGhosts.Envy:
                        //undecided on effect
                        break;
                }

                //StartCoroutine(AttackCooldown());
                Destroy(ghost.gameObject);
                _gameManager.AddScore(5);
            }
        }
    }


    IEnumerator AngerGhostEffect()
    {
        _checkGhost.localScale += new Vector3(0.3f, 0.3f, 0f);
        _checkGhost.transform.localPosition += new Vector3(0.68f, 0f, 0f);

        yield return new WaitForSeconds(4f);

        _checkGhost.localScale = deffaultScale;
        _checkGhost.transform.localPosition = deffaultPos;
    }

    IEnumerator AnxietyGhostEffect()
    {
        _attackCooldown -= _attackCooldown / 2;
        yield return new WaitForSeconds(4f);
        _attackCooldown = _deffaultAttackCooldown;
    }

    #endregion
}
