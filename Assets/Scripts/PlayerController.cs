using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private float moveInputX;
    private float moveInputY;

    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    private float moveSpeed;
    private bool isRunning = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = walkSpeed;
    }

    void Update()
    {
        moveInputX = Input.GetAxisRaw("Horizontal");  
        moveInputY = Input.GetAxisRaw("Vertical");  

        Vector2 moveDirection = new Vector2(moveInputX, moveInputY).normalized;
        rb.linearVelocity = moveDirection * moveSpeed;

        // Toggle Run
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && moveDirection.magnitude > 0)
        {
            isRunning = true;
            moveSpeed = runSpeed;
        }

        // Set animation
        if (moveDirection.magnitude > 0)
        {
            animator.SetBool("isWalking", !isRunning);
            animator.SetBool("isRunning", isRunning);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            isRunning = false;
            moveSpeed = walkSpeed;
        }

        // Flip 
        if (moveInputX > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInputX < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
