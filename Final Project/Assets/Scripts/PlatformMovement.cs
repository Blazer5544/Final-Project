using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private TrailRenderer _trailrenderer;
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
    private Vector2 _dashingDir;
    private bool _isDashing;
    private bool _canDash;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _trailrenderer = GetComponent<_trailrenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var inputX = Input.GetAxisRaw("Horizontal");
        var jumpInput = Input.GetButtonDown("Jump");
        var jumpInputReleased = Input.GetButtonUp("Jump");
        var dashInput = Input.GetButtonDown("Dash");

        if(dashInput && _canDash)
        {
            _isDashing = true;
            _canDash = false;
            _trailrenderer.emitting = true;
            _dashingDir = new Vector2(inputX, Input.GetAxisRaw("Vertical"));
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
}
