using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacumCleaner : MonoBehaviour
{
    #region Fields
    [Header("Fields")]

    [SerializeField] private GameManager _gameManager;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] ParticleSystem _particleSystem;

    [SerializeField] bool _ghostInZone;

    [SerializeField] LayerMask Ground;

    [SerializeField] Transform GroundCheck;

    [SerializeField] private float attackCooldown;

    private GameObject _player;

    private float GroundCheckRadius = 1.7f;

    private bool _canAttack = true;
    #endregion
    #region Monobehaviour Functions
    private void Start()
    {
        _player = transform.parent.gameObject;
    }

    void Update()
    {
        RoatateWeapon();

        if (Input.GetMouseButton(0))
        {
            Attack();
        }
        else _spriteRenderer.enabled = false;

        if (Input.GetMouseButtonDown(0)) _particleSystem.Play(); else _particleSystem.Stop();
    }
    #endregion
    #region Attack Functions
    void Attack()
    {
        _ghostInZone = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, Ground);
        _spriteRenderer.enabled = true;
        if (_ghostInZone)
        {
            if (_canAttack)
            {
                foreach (Collider2D collider in Physics2D.OverlapCircleAll(GroundCheck.position, GroundCheckRadius))
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        Ghost ghostScript = collider.GetComponent<Ghost>();
                        if (ghostScript != null)
                        {
                            ghostScript.SetGhostFields(transform);
                        }
                    }
                }

                StartCoroutine(AttackCooldown());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ghost ghost = collision.transform.GetComponent<Ghost>();
        if (ghost != null)
        {
            if (ghost._canMove == false)
            {
                Destroy(ghost.gameObject);
                _gameManager.AddScore(5);
            }
        }
    }

    IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
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
}
