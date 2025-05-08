using System.Diagnostics;
using UnityEngine;
using System.Collections;


public class Zombie : MonsterController
{
    public int health = 3;
    private bool isDead = false;

    public float followRange = 3f;
    public float attackingRange = 2f;
    private bool isMoving = false;
    private bool isAttacking = false;
    private float attackCooldown = 1f;
    private float lastAttackTime;



    public AudioClip roarSound;
    public AudioClip hitSound;
    public AudioClip deathSound;
    private float lastRoarTime = 0f;
    private float roarCooldown = 3f;

    protected override void Start()
    {
        base.Start();
    }

    public override void TakeDamage(int damage)
    {
        if (isDead) return;

        if (hitSound != null && MonsterAudioController.Instance != null)
        {
            MonsterAudioController.Instance.PlaySound(hitSound);
        }


        base.TakeDamage(damage); 
        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            StartCoroutine(Die());
        }
    }



    protected override void Move()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < followRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Flip Zombie
            if (direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);

            isMoving = true;

            if (animator != null)
                animator.SetBool("isMoving", true);
        }
        else
        {
            isMoving = false;
            if (animator != null)
                animator.SetBool("isMoving", false);
        }


        if (!isMoving && roarSound != null && MonsterAudioController.Instance != null)
        {
            if (Time.time - lastRoarTime > roarCooldown)
            {
                lastRoarTime = Time.time;
                MonsterAudioController.Instance.PlaySound(roarSound);
            }
        }


        if (distance < attackingRange)
        {
            animator.SetBool("isAttacking", true);

            if (Time.time - lastAttackTime > attackCooldown)
            {
                lastAttackTime = Time.time;

                PlayerController playerScript = player.GetComponent<PlayerController>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(1); 
                }
            }
        }
        else
        {
            animator.SetBool("isAttacking", false);
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

        if (hitSound != null && MonsterAudioController.Instance != null)
        {
            float hurtSoundDuration = hitSound.length; 
            yield return new WaitForSeconds(hurtSoundDuration);
        }
        if (deathSound != null && MonsterAudioController.Instance != null)
        {
            MonsterAudioController.Instance.PlaySound(deathSound);
        }


        yield return new WaitForSeconds(5f); 

        Destroy(gameObject);


    }

}

