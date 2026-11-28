using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPrueba : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] 
    private float speed;

    [Header("Vida")]
    public float maxLife;
    public float currentLife;
    public bool isDead = false; 

    [Header("Ataque")]
    public float baseDamage;
    public float damage;

    [Header("Arma")]                              // NUEVO
    [SerializeField]
    private Transform weaponPivot;                // el "brazo invisible"
    [SerializeField]
    private float turnSpeed = 900f;               // grados por segundo al girar

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

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private WeaponController weapon;
    private LevelManager levelManager;
    public bool knockback;

    private Camera mainCamera;                    // NUEVO
    private float targetAngle;                    // NUEVO: hacia dónde QUIERE mirar el arma
    private float currentAngle;                   // NUEVO: hacia dónde mira AHORA
    private bool pendingAttack;                   // NUEVO: "golpea cuando termines de girar"


    private void Start()
    {
        damage = baseDamage;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        levelManager = FindObjectOfType<LevelManager>();
        weapon = GetComponentInChildren<WeaponController>();
        mainCamera = Camera.main;                 // NUEVO

        UpdateLife();
    }

    private void Update()
    {
        if (isDead == true)
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
        
        Face(movement.x);                         // CAMBIO: el flip ahora está en un método

        // NUEVO: si corres, el arma apunta hacia donde vas
        if (movement != Vector2.zero)
        {
            targetAngle = GetAngle(movement);
        }

        Attack();
        RotateWeapon();                           // NUEVO
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
        if (Input.GetMouseButtonDown(1) && pendingAttack == false)
        {
            //AudioManager.Instance.PlaySFX(attackSound);

            if (movement != Vector2.zero)
            {
                // Corriendo: el arma ya apunta hacia donde vas, golpea directamente
                weapon.Attack();
            }
            else
            {
                // Quieto: girar hacia el lado del clic y golpear al llegar
                Vector2 clickDirection = GetClickDirection();
                targetAngle = GetAngle(clickDirection);
                Face(clickDirection.x);
                pendingAttack = true;
            }
        }
    }

    // ARMA //  (todo NUEVO)

    private void RotateWeapon()
    {
        // Girar poco a poco hacia el ángulo objetivo (esta es la "mini anim")
        currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turnSpeed * Time.deltaTime);
        weaponPivot.rotation = Quaternion.Euler(0, 0, currentAngle);

        // Que el arma no quede boca abajo al apuntar a la izquierda
        if (Mathf.Abs(Mathf.DeltaAngle(0, currentAngle)) > 90)
        {
            weaponPivot.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            weaponPivot.localScale = Vector3.one;
        }

        // Si estaba esperando para golpear y ya ha llegado, golpea
        if (pendingAttack == true && Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 1f)
        {
            weapon.Attack();
            pendingAttack = false;
        }
    }

    // Lado del clic: arriba, abajo, izquierda o derecha
    private Vector2 GetClickDirection()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - transform.position;

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            return new Vector2(Mathf.Sign(direction.x), 0);   // izquierda o derecha
        }
        else
        {
            return new Vector2(0, Mathf.Sign(direction.y));   // arriba o abajo
        }
    }

    // Pasa una dirección a grados (derecha = 0, arriba = 90, izquierda = 180, abajo = -90)
    private float GetAngle(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    // Flip del sprite
    private void Face(float x)
    {
        if (x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (x > 0)
        {
            transform.eulerAngles = Vector3.zero;
        }
    }

    // DAÑO //

    public void KnockBack(float _knockbackForce, Vector2 _direct)
    {
        if (isDead == true)     
        {
            return;
        }

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
        if (isDead == true)    
        {
            return;
        }

        currentLife -= damageTaken;
        UpdateLife();

        if (currentLife <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");   
        }
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        //AudioManager.Instance.PlaySFX(deathSFX);
        animator.SetTrigger("Death");
        enabled = false; 
    }

    public void UpdateLife()
    {
        //lifeBar.fillAmount = currentLife / maxLife;
        //lifeText.text = "LIFE: " + currentLife + " / " + maxLife;
    }
}
