using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public float life;
    public float speed;
    public float damage;

    [Header("Knockback")]
    [SerializeField]
    private float knockBackForce;
    public bool knockback;

    /*[Header("Drop")]
    public string itemDrop;*/
    
    [Header("Ataque")]
    public float stopDistance;      
    public bool attacking;  

    [Header("Audio")]
    [SerializeField]
    private AudioClip hit;

    public bool playerDetected;
    public bool isDead = false;
    public Transform player;
    private PlayerController playerController;   
    public Rigidbody2D rb;
    public Animator animator;
    public LevelManager levelManager;


    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    public void Update()
    {
        if (isDead == true || knockback == true)
        {
            return;
        }

        if (playerController != null && playerController.currentLife <= 0)
        {
            attacking = false;
            rb.linearVelocity = Vector2.zero;
            UpdateAnimations(Vector2.zero);
            return;
        }

        if (playerDetected == true && attacking == false)
        {
            // Ir hacia el player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;

            // Girar sprite
            if (direction.x > 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (direction.x < 0)
            {
                transform.eulerAngles = Vector3.zero;
            }

            UpdateAnimations(direction);

            float distanceSqr = (player.position - transform.position).sqrMagnitude;

            if (distanceSqr <= stopDistance * stopDistance)
            {
                attacking = true;
                rb.linearVelocity = Vector2.zero;
            }
        }
        else if (playerDetected == true && attacking == true)  
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimations(Vector2.zero);

            // Si el player se aleja, vuelve a perseguirle
            float distanceSqr = (player.position - transform.position).sqrMagnitude;

            if (distanceSqr > stopDistance * stopDistance)
            {
                attacking = false;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimations(Vector2.zero);
        }
    }

    // ANIMACIONES //

    private void UpdateAnimations(Vector2 direction)
    {
        bool moviendose = direction != Vector2.zero;

        bool lados = moviendose && Mathf.Abs(direction.x) >= Mathf.Abs(direction.y);
        bool arriba = moviendose && lados == false && direction.y > 0;
        bool abajo = moviendose && lados == false && direction.y < 0;

        animator.SetBool("Run", lados);
        animator.SetBool("RunUp", arriba);
        animator.SetBool("RunDown", abajo);
    }

    // DETECTAR //

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.transform;
            playerController = collision.GetComponent<PlayerController>();
            Invoke("StartMoving", animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    private void StartMoving()
    {
        playerDetected = true;
    }

    // ATACAR //

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead == true)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            //AudioManager.Instance.PlaySFX(hit);

            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(damage);
            player.KnockBack(knockBackForce, (collision.transform.position - transform.position).normalized);

            Vector2 direccionRebote = (transform.position - collision.transform.position).normalized;
            KnockBack(knockBackForce, direccionRebote);
        }
    }

    // RECIBIR DAÑO //

    public void TakeDamage(float _damage)
    {
        if (isDead == true)
        {
            return;
        }

        life -= _damage;
        animator.SetTrigger("Hit");

        if (life <= 0)
        {
            isDead = true;
            //AudioManager.Instance.PlaySFX(dead);
            Destroy(gameObject);
        }
    }

    public void KnockBack(float _knockbackForce, Vector2 _direct)
    {
        knockback = true; 
        rb.AddForce(_direct * _knockbackForce);
        Invoke("KnockbackEnd", 1f);
    }

    private void KnockbackEnd()
    {
        knockback = false;
    }
}
