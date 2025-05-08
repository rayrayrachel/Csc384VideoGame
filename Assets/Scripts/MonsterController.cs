using UnityEngine;

public abstract class MonsterController : MonoBehaviour
{

public PlayerController Mplayer;

    public float speed = 1f;
    protected Transform player;
    protected Animator animator;
    private AudioSource audioSource;

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        Move();
    }

    protected abstract void Move();

    public virtual void TakeDamage(int damage)
    {
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
    }

    protected virtual void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        if (Mplayer != null)
        {
            Mplayer.AddKill();
        }
        Destroy(gameObject, 1f); 
    }
}
