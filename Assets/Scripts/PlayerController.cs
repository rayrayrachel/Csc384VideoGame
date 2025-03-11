using UnityEngine;

public class playerController : MonoBehaviour
{

    private Animator animator;
    private Rigidbody2D rb;
    private float moveInput;

    public float moveSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        // Flip
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1); 
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1); 

        //test commit

    }
}
