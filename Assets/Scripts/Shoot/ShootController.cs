using System;
using UnityEngine;
using Megaman.Player;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class ShootController : MonoBehaviour
{
    private const float DEFAULT_CIRCLE_RADIUS = 0.015f;

    private new Rigidbody2D rigidbody2D;
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    private bool isEnemyShoot;
    /// <summary>
    /// Determina por quanto o raio do CircleCollider2D deve ser multiplicado quando colide com algum objeto.
    /// Basicamente, aumenta o raio da colisão para dar o efeito da bomba explodindo e causando dano em outro player 
    /// próximo à explosão.
    /// </summary>
    [Header("Bomb Setup")]
    [SerializeField]
    [Range(1, 5)]
    private float circleRadiusMultiplier = 1.5f;
    [SerializeField]
    private float dieDelay = .04f;

    private CircleCollider2D circleCollider2D;

    /// <summary>
    /// Desabilita a colisão entre os demais objetos 
    /// </summary>
    private bool isBeingDestroyed = false;

    public bool hasColidedWithZero = false;

    /// <summary>
    /// Sempre que o objeto é reativado, reseta a bomba para o estado original.
    /// </summary>
    private void OnEnable()
    {
        this.hasColidedWithZero = false;

        if (!isEnemyShoot)
            return;

        isBeingDestroyed = false;

        circleCollider2D.radius = DEFAULT_CIRCLE_RADIUS;
    }

    private void OnDisable()
    {
        if (!isEnemyShoot)
        {
            this.tag = "shoot";
            spriteRenderer.color = Color.blue;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidbody2D = this.GetComponent<Rigidbody2D>();

        if (!isEnemyShoot)
            return;

        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        //Verifica se o Shoot não está mais sendo visto pela câmera e o destrói
        if (Vector2.Distance(transform.position, Camera.main.transform.position) > 
            Camera.main.orthographicSize * 2)
        {
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }

    public void AddVelocity(float vel)
    {
        rigidbody2D.velocity = new Vector2(vel, 0);
    }

    public void AddVelocity(Vector2 velocity)
    {
        rigidbody2D.velocity = velocity;
        rigidbody2D.gravityScale = 1;
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        Lean.LeanPool.Despawn(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (!isEnemyShoot)
        {
            if (collider2D.transform.CompareTag("zero"))
            {
                if (hasColidedWithZero)
                    return;

                float swordx = collider2D.transform.parent.parent.localScale.x;
                float bulletx = rigidbody2D.velocity.x;
                if (swordx < 0 && bulletx > 0 || swordx > 0 && bulletx < 0)
                {
                    this.tag = "superShoot";
                    spriteRenderer.color = Color.red;
                }
                else
                {
                    GameObject newShoot = Lean.LeanPool.Spawn(MegamanController.PrefabShoot);
                    newShoot.transform.position = this.transform.position;
                    newShoot.GetComponent<ShootController>().hasColidedWithZero = true;
                    newShoot.GetComponent<Rigidbody2D>().velocity = new Vector3(rigidbody2D.velocity.x, Mathf.Abs(rigidbody2D.velocity.x));
                    rigidbody2D.velocity = new Vector3(rigidbody2D.velocity.x, -Mathf.Abs(rigidbody2D.velocity.x));

                    this.hasColidedWithZero = true;
                }

            }
        }
        else
        {
            if (isBeingDestroyed)
                return;

            //Faz a bomba parar onde tenha colidido
            rigidbody2D.gravityScale = 0;

            rigidbody2D.velocity = Vector2.zero;

            circleCollider2D.radius = circleCollider2D.radius * circleRadiusMultiplier;

            isBeingDestroyed = true;

            //Destroi a bomba após 1 segundo
            Lean.LeanPool.Despawn(this.gameObject, dieDelay);
        }
    }

}
