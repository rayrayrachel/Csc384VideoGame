using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        MonsterController monster = collision.GetComponent<MonsterController>();
        if (monster != null)
        {
            monster.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
