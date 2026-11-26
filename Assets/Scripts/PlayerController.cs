using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] 
    private float speed;

    [Header("Vida")]
    public float maxLife;
    public float currentLife;

    [Header("Ataque")]
    public float baseDamage;
    public float damage;

    /*[Header("Disparo")]               
    public GameObject bulletPrefab;
    public Transform firePoint;   
    public float fireRate = 0.3f;  
    private float fireTime;*/

    /*[Header("Stats")]
    [SerializeField] 
    private float espadaNivel;
    private float armaduraNivel;*/

    /*[Header("Audio")]
    [SerializeField] 
    private AudioClip attackSound;
    [SerializeField] 
    private AudioClip deathSound;*/

    [Header("UI Stats")]
    [SerializeField]
    private Text lifeText;
    [SerializeField]
    private Image lifeBar;
    [SerializeField]
    private GameObject gameOverPanel; 

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private WeaponController weapon;
    private LevelManager levelManager;
    public bool knockback;


    private void Start()
    {
        damage = baseDamage;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        levelManager = FindObjectOfType<LevelManager>();
        weapon = GetComponentInChildren<WeaponController>();

        UpdateLife();
    }

    private void Update()
    {
        if (currentLife <=0)
        {
            return;
        }

        //Movimiento 

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        bool lados = movement.x != 0;
        bool arriba = movement.x == 0 && movement.y > 0;
        bool abajo = movement.x == 0 && movement.y < 0;

        animator.SetBool("Run", lados);     
        animator.SetBool("RunUp", arriba);  
        animator.SetBool("RunDown", abajo);
        
        // Flip para los lados
        
        if (movement.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (movement.x > 0)
        {
            transform.eulerAngles = Vector3.zero;
        }

        Attack();
    }

    private void FixedUpdate()
    {
        if (knockback == true)
        {
            return;  
        }

        rb.linearVelocity = movement.normalized * speed;
    }

    // ATAQUE //

    private void Attack()
    {
        if (currentLife <= 0)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            //AudioManager.Instance.PlaySFX(attackSound);
            weapon.Attack();      
        }
    }

    public void KnockBack(float _knockbackForce, Vector2 _direct)
    {
        knockback = true;
        rb.AddForce(_direct * _knockbackForce);
        Invoke("KnockbackEnd", 0.5f);
    }

    private void KnockbackEnd()
    {
        knockback = false;
    }

    public void TakeDamage(float damageTaken)
    {
        currentLife -= damageTaken;

        if (currentLife <= 0)
        {
            Die();
        }

        UpdateLife();
        animator.SetTrigger("Hit");
    }

    private void Die()
    {
        animator.SetTrigger("Death");
        rb.linearVelocity = Vector2.zero;
        gameOverPanel.SetActive(true);
        //AudioManager.Instance.PlaySFX(deathSound);
        enabled = false;
    }

    public void UpdateLife()
    {
        //lifeBar.fillAmount = currentLife / maxLife;
        //lifeText.text = "LIFE: " + currentLife + " / " + maxLife;
    }
}
