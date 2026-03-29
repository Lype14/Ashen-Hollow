using System;
using UnityEngine;
using UnityEngine.InputSystem; //import que permite usar o nome input system da unity


public class player_moviment : MonoBehaviour
{
    public GameObject slashEffect;
    public Animator attackAnim;
    public bool teste = true;


    [Header("Components")]
    public PlayerInput playerInput;
    public Rigidbody2D rb;
    public Animator anim;

    
    [Header("Movement Variable")]
    private bool moving = false;
    private bool idle = false;
    private bool isAttacking = false;
    private bool jumping = false; 
    public float velocity;
    public float jumpForce;
    public float jumpCutMultiplier = .9f;
    public float normalGravity;
    public float fallGravity;
    public float jumpGravity;
    private int facingDirection = 1;


    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public bool isGrounded;


    [Header("Slide Settings")]
    public float slideDuration = .6f;
    private bool isSliding;
    private float slideTimer;
    public Vector2 moveInput;
    private bool jumpPressed;
    private bool jumpReleased;




    void Start()
{
    rb.gravityScale = normalGravity;
}

    void Update()
    {
        Flip();
        HandleAnimations();
        HandleSlide();
    }

    void FixedUpdate()
    {   
        ApplyVariableGravity();
        CheckGrounded();

        if (!isSliding)
        {
            HandleMoviment();
        }
        HandleJump();     
    }

     void ApplyVariableGravity()
    {
        if(rb.linearVelocity.y < -0.3f){
            rb.gravityScale = fallGravity;
        }
        else if(rb.linearVelocity.y > 0.3f)
        {
            rb.gravityScale = jumpGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position,groundCheckRadius,groundLayer);
    }

    void HandleAnimations()
    {
        anim.SetBool("sliding",isSliding && isGrounded);
        anim.SetBool("isAttacking",isAttacking && !isSliding);
        anim.SetBool("isIdle", Math.Abs(moveInput.x) < .1f && isGrounded && !isAttacking && !isSliding);
        anim.SetBool("moving", Math.Abs(moveInput.x) > .1f && isGrounded && !isAttacking && !isSliding);
        anim.SetBool("jumping", isGrounded == false && !isAttacking);
    }

    private void HandleSlide()
        {
            if (isSliding)
            {
                slideTimer -= Time.deltaTime;
                if(slideTimer <= 0)
                {
                    isSliding = false;
                }   
            }
    }

    private void HandleMoviment()
    {
        {
            float targetSpeed = moveInput.x * velocity;
            rb.linearVelocity = new Vector2(targetSpeed,rb.linearVelocity.y);
            if(rb.linearVelocity.x != 0 && isGrounded == true )
            {
                if (isAttacking)
                {
                    rb.linearVelocity = new Vector2(targetSpeed,rb.linearVelocity.y) * 0;
                } 
            }
        }
        
    }

    private void HandleJump()
    {
        if(jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,jumpForce);
            jumpPressed = false;
            jumpReleased = false;
        }
        if (jumpReleased)
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x,rb.linearVelocity.y * jumpCutMultiplier);
            }
            jumpReleased = false;
        }
    }
    public void SpawnSlash()
{
     slashEffect.SetActive(true);
     attackAnim.SetBool("firstAttack", true);
}

 public void OnAttack(InputValue value)
{
    if (value.isPressed)
    {
                isAttacking = true;
        
    }
}

    public void finishAttack()
    {
        isAttacking = false;
        attackAnim.SetBool("firstAttack",false);
        slashEffect.SetActive(false);
    }

    public void OnJump (InputValue value)
    {
         if(value.isPressed)
        {
         jumpPressed = true;
         jumpReleased = false;
        }
        else
        {
            jumpReleased = true;
        }
    }

    public void OnSlide(InputValue value)
    {
        if (isGrounded && value.isPressed && !isSliding)
        {
            isSliding = true;
            slideTimer = slideDuration;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color= Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }


    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    

    private void Flip()
    {
        if(moveInput.x < 0 )
        {
            facingDirection = -1;
        }
        else if(moveInput.x > 0)
        {
            facingDirection = 1;
        }

        transform.localScale = new Vector3(facingDirection * 1.6f,1.6f,1);
    }
}
