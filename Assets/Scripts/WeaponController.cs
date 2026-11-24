using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private float knockBackForce;
    private Animator animator;
    private LevelManager levelManager;
    private PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        levelManager = FindObjectOfType<LevelManager>();
        player = GetComponentInParent<PlayerController>();
    }
    // Update is called once per frame
    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();

            if (enemy != null)
            {
                //enemy.TakeDamage(player.damage);
                //enemy.KnockBack(knockBackForce, (enemy.transform.position - transform.position).normalized);
            }
        }
        else if (collision.CompareTag("Wood"))
        {
            Destroy(collision.gameObject);
            //levelManager.AddItem("Wood", 1);
        }
    }
}
