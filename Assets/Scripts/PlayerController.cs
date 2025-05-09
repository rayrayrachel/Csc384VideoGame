using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using TMPro;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    //Audio
    public AudioClip shotgunSound;
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip healSound;

    private AudioSource audioSource;

    //UI
    public GameObject arrow;

    public GameObject bulletPrefab;
    public float bulletSpeed = 40f;

    public TextMeshProUGUI noBulletText;
    public TextMeshProUGUI saltShotsText;

    public UnityEngine.UI.Image[] heartImages;

    //PlayerProfile
    public int damage = 1;
    public int maxHealth = 5;
    public int currentHealth;
    private bool isDead = false;

    //Inventory
    public int saltShots = 10;
    public bool hasRunningShoe = false;
    public float runningShoeDuration = 5f; 

    public GameObject runningShoeText;
    public GameObject runningShoeStar;

    //Score
    public int killCount = 0;
    public TextMeshProUGUI killCountText;

    // Timer for running shoes
    private Coroutine runningShoeCoroutine;

    // Button 

    public Button returnButton;
    public GameObject EndScene;
    public TextMeshProUGUI EndSceneText;
    public int MonsterAmount = 15;

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
        UpdateHearts();

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

    public void AddHealth(int heart)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += heart; 
        }

        UpdateHearts();

        animator.SetTrigger("Hurt");
        if (healSound != null && PlayerAudioController.Instance != null)
        {
            PlayerAudioController.Instance.PlaySound(healSound); 
        }
    }

    public void AddSalt(int salt)
    {
        saltShots += 10;

        if (healSound != null && PlayerAudioController.Instance != null)
        {
            PlayerAudioController.Instance.PlaySound(healSound);
        }
    }

    public void AddRunningShoe()
    {
        if (!hasRunningShoe)
        {
            hasRunningShoe = true;
            walkSpeed = 4f;
            moveSpeed = walkSpeed;

            if (runningShoeText != null) runningShoeText.SetActive(true);
            if (runningShoeStar != null) runningShoeStar.SetActive(true);

            if (runningShoeCoroutine != null)
            {
                StopCoroutine(runningShoeCoroutine); 
            }
            runningShoeCoroutine = StartCoroutine(RunningShoeDuration());

        }

        if (healSound != null && PlayerAudioController.Instance != null)
        {
            PlayerAudioController.Instance.PlaySound(healSound);
        }
    }

    private IEnumerator RunningShoeDuration()
    {
        yield return new WaitForSeconds(runningShoeDuration); 

        walkSpeed = 2f;
        moveSpeed = walkSpeed;

        hasRunningShoe = false;

        if (runningShoeText != null) runningShoeText.SetActive(false);
        if (runningShoeStar != null) runningShoeStar.SetActive(false);
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

        yield return new WaitForSeconds(2f);

        if (EndScene != null) EndScene.SetActive(true);
        EndScenee();

    }

    
    void EndScenee()
    {
        EndSceneText.text = "SCORE: " + killCount;
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(OnReturnButtonClicked);
        }
        SaveScore(killCount);
    }

    //private void SaveScore()
    //{
    //    PlayerPrefs.SetInt("PlayerScore", killCount);  
    //    PlayerPrefs.Save();  
    //}

    private void SaveScore(int score)
    {
        PlayerPrefs.DeleteKey("RecentScore");

        PlayerPrefs.SetInt("RecentScore", score);  
        PlayerPrefs.Save();

        string savedScores = PlayerPrefs.GetString("PlayerScores", "");
        List<int> highScores = new List<int>();

        if (!string.IsNullOrEmpty(savedScores))
        {
            string[] scoreStrings = savedScores.Split(',');

            foreach (string scoreStr in scoreStrings)
            {
                if (int.TryParse(scoreStr, out int parsedScore))
                {
                    highScores.Add(parsedScore);
                }
            }
        }

        highScores.Add(score);  
        highScores.Sort((a, b) => b.CompareTo(a)); 

        if (highScores.Count > 5)
        {
            highScores.RemoveAt(highScores.Count - 1); 
        }

        string updatedScores = string.Join(",", highScores);
        PlayerPrefs.SetString("PlayerScores", updatedScores);
        PlayerPrefs.Save();
    }


    void OnReturnButtonClicked()
    {

        if (!isDead) 
        {
            StopAllCoroutines();
        }
        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        if(killCount == MonsterAmount)
        {
            StartCoroutine(Die());
        }
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
            if (saltShots > 0)
            {
                Shoot();
                isFiring = true;
                animator.SetBool("isFiring", true);
            }
            else
            {
                StartCoroutine(ShowNoBulletMessage());
            }

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
        if (saltShots > 0 && !isFiring)
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

            saltShots--;

            UpdateBulletCount();

            StartCoroutine(ResetFiringState());
        }
        else
        {
            StartCoroutine(ShowNoBulletMessage());
        }
    }

    public void UpdateBulletCount()
    {
        if (saltShotsText != null)
        {
            saltShotsText.text = saltShots.ToString();
        }
    }

    private IEnumerator ShowNoBulletMessage()
    {
        if (noBulletText != null)
        {
            noBulletText.text = "Insufficient bullets, open more chests.";
            noBulletText.gameObject.SetActive(true);
            yield return new WaitForSeconds(5f);
            noBulletText.gameObject.SetActive(false);
        }
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

    void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = i < currentHealth;
        }
    }

    //kill Monster
    public void AddKill()
    {
        killCount++;
        killCountText.text = "Kills: " + killCount;
    }
}
