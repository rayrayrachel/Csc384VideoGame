using UnityEngine;
using System.Collections;  

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private float moveInputX;
    private float moveInputY;

    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    private float moveSpeed;
    private bool isRunning = false;

    private bool isAiming = false;
    private bool isFired = false;
    private bool isFiring = false;

    public AudioClip shotgunSound;
    public AudioClip hitSound;
    public AudioClip deathSound;


    private AudioSource audioSource;


    public GameObject arrow;

    public GameObject bulletPrefab;
    public float bulletSpeed = 40f;

    public int damage = 1;

    public int maxHealth = 5;
    private int currentHealth;
    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = walkSpeed;
        audioSource = GetComponent<AudioSource>();  

    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
            if (hitSound != null && PlayerAudioController.Instance != null)
            {
                PlayerAudioController.Instance.PlaySound(hitSound);
            }

        }

        if (currentHealth <= 0)
        {
            isDead = true;
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        if (hitSound != null && PlayerAudioController.Instance != null)
        {
            float hurtSoundDuration = hitSound.length;
            yield return new WaitForSeconds(hurtSoundDuration);
        }
        if (deathSound != null && PlayerAudioController.Instance != null)
        {
            PlayerAudioController.Instance.PlaySound(deathSound);
        }


        yield return new WaitForSeconds(5f);
    }

    // Update is called once per frame
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

        if (arrow != null)
        {
            arrow.SetActive(isAiming);
        }

        if (isAiming && arrow != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (transform.localScale.x < 0)
            {
                angle += 180f;
            }

            arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
        }



        // Flip 
        if (moveInputX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }


        else if (moveInputX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);

        }

    }
    void Shoot()
    {
        animator.SetBool("isFired", true);
        isFired = true;

        PlayerAudioController.Instance.PlaySound(shotgunSound);

        if (arrow != null && bulletPrefab != null)
        {
            Vector3 fireDirection = arrow.transform.right;

            if (transform.localScale.x < 0)
            {
                fireDirection = -fireDirection;
            }

            Vector3 spawnPosition = arrow.transform.position;

            StartCoroutine(SpawnBulletWithDelay(spawnPosition, fireDirection));
        }

        StartCoroutine(ResetFiringState());
    }

    private IEnumerator SpawnBulletWithDelay(Vector3 spawnPosition, Vector3 fireDirection)
    {
        yield return new WaitForSeconds(0.2f); 

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        SpriteRenderer sr = bullet.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = fireDirection.normalized * bulletSpeed;
        }

        Destroy(bullet, 5f / bulletSpeed);
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
