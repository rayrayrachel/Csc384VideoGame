using UnityEngine;
using System.Collections;  

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

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    private bool isAiming = false;
    private bool isFired = false;
    private bool isFiring = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        moveInputX = Input.GetAxisRaw("Horizontal");
        moveInputY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveInputX, moveInputY).normalized;
        rb.velocity = moveDirection * moveSpeed;

        // Toggle Run
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && moveDirection.magnitude > 0)
        {
            isRunning = true;
            moveSpeed = runSpeed;
        }

        // Aiming (Right Click)
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            isAiming = true;
            animator.SetBool("isAiming", true);
            isFired = false;
            animator.SetBool("isFired", false);
        }
        if (Input.GetMouseButtonUp(1)) // Release right click
        {
            isAiming = false;
            animator.SetBool("isAiming", false);
            isFired = false;
            animator.SetBool("isFired", false);
        }

        // Shooting (Left Click)
        if (isAiming && Input.GetMouseButtonDown(0) && !isFiring) 
        {
            Shoot();
            isFiring = true;
            animator.SetBool("isFiring", true);
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

    void Shoot()
    {

        animator.SetBool("isFired", true); 
        isFired = true;

        StartCoroutine(ResetFiringState());
    }

    private IEnumerator ResetFiringState()
    {
        yield return new WaitForSeconds(0.5f); 

        isFiring = false;
        animator.SetBool("isFiring", false);
        isFired = false;
        animator.SetBool("isFired", false);
    }
}
