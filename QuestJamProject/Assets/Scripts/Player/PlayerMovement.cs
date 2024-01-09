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

    [SerializeField] private float _walkSpeed = 2f;

    private Animator _anim;
    private Vector2 _moveVector;
    private Rigidbody2D _rb;

    private bool faceRight = true;

    private Vector2 origin;
    private bool locked;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        origin = _rb.position;
        locked = true;
    }

    private void Update()
    {
        if (locked) return;

        _moveVector.x = _moveInputX.action.ReadValue<float>();
        _moveVector.y = _moveInputY.action.ReadValue<float>();

        Move(_walkSpeed);

        Reflect();
        
        _anim.SetFloat("Horizontal", _moveVector.x);
        _anim.SetFloat("Vertical", _moveVector.y);
        _anim.SetFloat("Speed", _moveVector.sqrMagnitude);
        

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
        Vector2 normalisedMV = _moveVector.normalized;
        _rb.velocity = new Vector2(normalisedMV.x * speed, _rb.velocity.y);
        _rb.velocity = new Vector2(_rb.velocity.x, normalisedMV.y * speed);
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

    public IEnumerator BoostPlayerSpeed(float boostSpeed, float time)
    {
        float defaultSpeed = _walkSpeed;
        _walkSpeed = boostSpeed;
        yield return new WaitForSeconds(time);
        _walkSpeed = defaultSpeed;
    }

    public void FindGhostSound()
    {
        _findGhostSound.Play();
    }

    public void SetLocked(bool l)
    {
        locked = l;
        if (l) _rb.velocity = Vector2.zero;
    }

    public void ResetPosition()
    {
        _rb.position = origin;
    }
}
