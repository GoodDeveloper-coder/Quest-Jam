using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Keybuttons")]
    [SerializeField] private InputActionReference _moveInputX;
    [SerializeField] private InputActionReference _moveInputY;
    [Space]

    [Header("Sfx")]
    [SerializeField] private AudioSource _findGhostSound;
    [SerializeField] private List<AudioSource> _footSteps;

    [SerializeField] float minTimeBetweenFootsteps = 0.3f; // Minimum time between footstep sounds
    [SerializeField] float maxTimeBetweenFootsteps = 0.6f; // Maximum time between footstep sounds
    private float timeSinceLastFootstep; // Time since the last footstep sound

    [SerializeField] private float walkSpeed = 2f;

    private Animator _anim;
    private Vector2 _moveVector;
    private Rigidbody2D _rb;

    private bool faceRight = true;

    [SerializeField] private GameObject[] vacuumSprites;
    [SerializeField] private GameObject vacuumBeamSprite;

    private GhostMovement[] ghostsCaught;
    private Vector2 origin;
    private Vector2 startPosition;
    private Vector2 pMoveVector;
    private bool moving;
    private bool locked;
    private int direction;
    private bool vacuumOn;
    private bool catchingGhost;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        ghostsCaught = new GhostMovement[0];
        origin = _rb.position;
        startPosition = origin;
        SetVacuum();
    }

    private void Update()
    {
        if (locked) return;
        if (catchingGhost)
        {
            if (Input.GetKeyDown(KeyCode.Space)) vacuumOn = true;
            else if (Input.GetKeyUp(KeyCode.Space)) vacuumOn = false;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            vacuumOn = true;
            SetVacuum();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            vacuumOn = false;
            SetVacuum();
        }
        if (vacuumOn)
        {
            float vacuumRange = 5;
            Vector2 offset = (new Vector3[]{ Vector2.right, Vector2.up, -Vector2.right, -Vector2.up })[direction] * vacuumRange / 2f;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(_rb.position + offset, direction % 2 == 0 ? new Vector2(vacuumRange, 0.2f) : new Vector2(0.2f, vacuumRange), 0);
            GhostMovement closestGhost = null;
            float minDistance = 0;
            Vector3 target = Vector3.right * _rb.position.x + Vector3.up * _rb.position.y;
            foreach (Collider2D c in colliders)
            {
                GhostMovement currentGhost = c.gameObject.GetComponent<GhostMovement>();
                if (currentGhost != null)
                {
                    if (closestGhost == null) closestGhost = currentGhost;
                    else
                    {
                        float distance = (c.transform.position - target).sqrMagnitude;
                        if (distance < minDistance)
                        {
                            closestGhost = currentGhost;
                            minDistance = distance;
                        }
                    }
                    
                }
            }
            if (closestGhost != null) StartCoroutine(CatchGhost(closestGhost));
        }

        _moveVector.x = _moveInputX.action.ReadValue<float>();
        _moveVector.y = _moveInputY.action.ReadValue<float>();

        Move(walkSpeed);

        /*
        if (moving) return;
        if (Mathf.Abs(_moveVector.x) > Mathf.Abs(_moveVector.y))
        {
            if (_moveVector.x > 0 && pMoveVector.x <= 0)
            {
               Collider2D[] colliders = Physics2D.OverlapBoxAll(_rb.position + Vector2.right, Vector2.one / 2, 0);
               bool wall = false;
               foreach (Collider2D c in colliders) wall |= c.gameObject.tag == "Wall";
               if (!wall) StartCoroutine(IMove(0));
            }
            else if (_moveVector.x < 0 && pMoveVector.x >= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(_rb.position - Vector2.right, Vector2.one / 2, 0);
                bool wall = false;
                foreach (Collider2D c in colliders) wall |= c.gameObject.tag == "Wall";
                if (!wall) StartCoroutine(IMove(2));
            }
        }
        else if(Mathf.Abs(_moveVector.x) < Mathf.Abs(_moveVector.y))
        {
            if (_moveVector.y > 0 && pMoveVector.y <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(_rb.position + Vector2.up, Vector2.one / 2, 0);
                bool wall = false;
                foreach (Collider2D c in colliders) wall |= c.gameObject.tag == "Wall";
                if (!wall) StartCoroutine(IMove(1));
            }
            else if (_moveVector.y < 0 && pMoveVector.y >= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(_rb.position - Vector2.up, Vector2.one / 2, 0);
                bool wall = false;
                foreach (Collider2D c in colliders) wall |= c.gameObject.tag == "Wall";
                if (!wall) StartCoroutine(IMove(3));
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = 0;
            SetVacuum();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = 1;
            SetVacuum();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = 2;
            SetVacuum();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = 3;
            SetVacuum();
        }
        pMoveVector = _moveVector;
        */

        Reflect();
        /*
        _anim.SetFloat("Horizontal", _moveVector.x);
        _anim.SetFloat("Vertical", _moveVector.y);
        _anim.SetFloat("Speed", _moveVector.sqrMagnitude);
        */

        /*
        if (_moveVector.sqrMagnitude > 0)
        {
            if (Time.time - timeSinceLastFootstep >= Random.Range(minTimeBetweenFootsteps, maxTimeBetweenFootsteps))
            {
                // Play a random footstep sound from the array
                _footSteps[Random.Range(0, _footSteps.Count)].Play();

                timeSinceLastFootstep = Time.time; // Update the time since the last footstep sound
            }
        }
        */
    }

    private void Move(float speed)
    {
        // _anim.SetFloat("moveX", Mathf.Abs(_moveVector.x));
        _rb.velocity = new Vector2(_moveVector.x * speed, _rb.velocity.y);
        _rb.velocity = new Vector2(_rb.velocity.x, _moveVector.y * speed);
    }

    private void Reflect()
    {
        if ((_moveVector.x > 0 && !faceRight) || (_moveVector.x < 0 && faceRight))
        {
            if (faceRight)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }

            if (!faceRight)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }

            faceRight = !faceRight;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    public void FindGhostSound()
    {
        _findGhostSound.Play();
    }

    public void SetLocked(bool l)
    {
        locked = l;
        if (l)
        {
            StopAllCoroutines();
            moving = false;
        }
    }

    public void ResetPosition()
    {
        _rb.position = origin;
        ghostsCaught = new GhostMovement[0];
    }

    private IEnumerator IMove(int direction)
    {
        moving = true;
        Vector2 target = _rb.position + (new Vector2[] { Vector2.right, Vector2.up, -Vector2.right, -Vector2.up })[direction];
        float distance = (_rb.position - target).sqrMagnitude;
        while (distance > float.Epsilon)
        {
            _rb.MovePosition(Vector2.MoveTowards(_rb.position, target, walkSpeed * Time.deltaTime));
            distance = (_rb.position - target).sqrMagnitude;
            yield return null;
        }
        _rb.MovePosition(target);
        moving = false;
        startPosition = target;
    }

    private void SetVacuum()
    {
        for (int i = 0; i < vacuumSprites.Length; i++) vacuumSprites[i].SetActive(i == direction);
        vacuumBeamSprite.SetActive(vacuumOn);
        if (vacuumOn)
        {
            float beamLength = 5;
            vacuumBeamSprite.transform.localScale = Vector3.right * (direction % 2 == 0 ? beamLength : 0.2f) + Vector3.up * (direction % 2 == 0 ? 0.2f : beamLength) + Vector3.forward;
            vacuumBeamSprite.transform.position = transform.position + Vector3.right * (direction == 0 ? (beamLength - 1) / 2f : direction == 2 ? (1 - beamLength) / 2f : 0) + Vector3.up * (direction == 1 ? (beamLength - 1) / 2f : direction == 3 ? (1 - beamLength) / 2f : -0.2f);
        }
    }

    private IEnumerator CatchGhost(GhostMovement ghost)
    {
        catchingGhost = true;
        ghost.SetMoving(false);
        float distance = (ghost.transform.position - transform.position).sqrMagnitude;

        float suckSpeed = 2.5f;

        while (distance > float.Epsilon)
        {
            ghost.transform.position = Vector3.MoveTowards(ghost.transform.position, transform.position, suckSpeed * Time.deltaTime);
            float beamLength = (ghost.transform.position - transform.position).magnitude;
            vacuumBeamSprite.transform.localScale = Vector3.right * (direction % 2 == 0 ? beamLength : 0.2f) + Vector3.up * (direction % 2 == 0 ? 0.2f : beamLength) + Vector3.forward;
            vacuumBeamSprite.transform.position = transform.position + Vector3.right * (direction == 0 ? beamLength / 2f : direction == 2 ? -beamLength / 2f : 0) + Vector3.up * (direction == 1 ? beamLength / 2f : direction == 3 ? -beamLength / 2f : -0.2f);
            distance = (ghost.transform.position - transform.position).sqrMagnitude;
            yield return null;
        }
        SetVacuum();
        ghost.SetCaught(true);
        List<GhostMovement> ghostList = new List<GhostMovement>(ghostsCaught);
        ghostList.Add(ghost);
        ghostsCaught = ghostList.ToArray();
        ghost.gameObject.SetActive(false);
        catchingGhost = false;
    }
}
