using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private TrailRenderer _trailrenderer;
    private Collider2D _collider;

    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundCheckRadius = 0.05f;
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float jumpForce = 10f;
    [SerializeField]
    private float _yVelJumpReleaseMod = 2f;
    [SerializeField]
    private LayerMask collisionMask;

    [SerializeField] private float _dashingVelocity = 14f;
    [SerializeField] private float _dashingTime = 0.5f;
    [SerializeField] private bool _active = true;

    private Vector2 _dashingDir;
    private bool _isDashing;
    private bool _canDash = true;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _trailrenderer = GetComponent<TrailRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var inputX = Input.GetAxisRaw("Horizontal");
        var jumpInput = Input.GetButtonDown("Jump");
        var jumpInputReleased = Input.GetButtonUp("Jump");
        var dashInput = Input.GetButtonDown("Dash");

        
        if (!_active)
        {
            return;
        }

        if(dashInput && _canDash)
        {
            _isDashing = true;
            _canDash = false;
            _trailrenderer.emitting = true;
            _dashingDir = new Vector2(inputX, Input.GetAxisRaw("Vertical"));
            if(_dashingDir == Vector2.zero)
            {
                _dashingDir = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(StopDashing());
        }

        if (_isDashing)
        {
            _rigidbody.velocity = _dashingDir.normalized * _dashingVelocity;
            return;
        }

        if (IsGrounded())
        {
            _canDash = true;
        }

        _rigidbody.velocity = new Vector2(inputX * speed, _rigidbody.velocity.y);

        if(jumpInput && IsGrounded())
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce);
        }

        if(jumpInputReleased && _rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y / _yVelJumpReleaseMod);
        }

        if(inputX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(inputX), 1, 1);
        }

        _animator.SetBool("IsJumping", !IsGrounded());
        _animator.SetBool("IsRunning", inputX != 0);
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionMask);
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(_dashingTime);
        _trailrenderer.emitting = false;
        _isDashing = false;
    }

    private void MiniJump()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce / 2);
    }

    private void Die()
    {
        _active = false;
        _collider.enabled = false;
        MiniJump();
    }
}
