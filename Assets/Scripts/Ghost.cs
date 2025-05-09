using System.Diagnostics;
using System.Collections;
using UnityEngine;

public class Ghost : MonsterController
{
    public int health = 1;
    private bool isDead = false;

    public float followRange = 5f;
    public float attackingRange = 2f;
    private float lastAttackTime;
    private float attackCooldown = 0.7f;

    public AudioClip moanSound;
    public AudioClip deathSound;

    private float lastMoanTime = 0f;
    private float moanCooldown = 20f;

    protected override void Start()
    {
        base.Start();
        SetGhostTransparency(0.5f); 
    }

    public override void TakeDamage(int damage)
    {
        if (isDead) return;

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
        if (player == null || isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < followRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            transform.localScale = new Vector3(direction.x < 0 ? 1 : -1, 1, 1);

            animator?.SetBool("isMoving", true);
        }
        else
        {
            animator?.SetBool("isMoving", false);
        }

        if (distance < attackingRange && Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;

            PlayerController playerScript = player.GetComponent<PlayerController>();
            playerScript?.TakeDamage(1);
        }


        if (moanSound != null && Time.time - lastMoanTime > moanCooldown)
        {
            MonsterAudioController.Instance?.PlaySound(moanSound);
            lastMoanTime = Time.time;
        }
    }

    private IEnumerator Die()
    {
        animator?.SetTrigger("Die");

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        if (deathSound != null)
        {
            MonsterAudioController.Instance?.PlaySound(deathSound);
        }

        if (Mplayer != null)
        {
            Mplayer.AddKill();
        }


        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    private void SetGhostTransparency(float alpha)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }
}
