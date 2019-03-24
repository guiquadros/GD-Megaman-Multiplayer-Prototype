#pragma warning disable 0168 //variable declared but not used
#pragma warning disable 0219 //variable assigned but not used
#pragma warning disable 0414 //private field assigned but not used

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Megaman.Player
{
    /// <summary>
    /// Esta classe é responsável por captar o Input de um player particular (servindo tanto para
    /// o player 1, quanto para o player 2) e fazendo o seu personagem mover e saltar.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : BaseController
    {
        protected bool isLookingLeft = false;

        //Definição dos layers
        private int LAYER_MASK_PLAYER;

        [Header("Input")]
        [SerializeField]
        private PlayerKeysSO playerKeysSO;

        [Header("Movement")]
        [SerializeField]
        [Range(1, 400)]
        private float moveForce = 200f;
        [SerializeField]
        [Range(1, 20)]
        private float maxMovSpeed = 6f;
        [SerializeField]
        [Range(-20, -1)]
        private float minFallingVelocity = -7.5f;
        [SerializeField]
        private bool snapXVelocity = true;

        [Header("Jump")]
        [SerializeField]
        [Range(1, 30)]
        private float jumpForce = 15f;
        [SerializeField]
        [Range(.1f, 1)]
        private float wallJumpForce = 1f;
        [SerializeField]
        [Range(0f, 1f)]
        private float wallJumpDelay = .22f;

        [Header("Healing")]
        [SerializeField]
        private float AutoHealingDelay = 1;
        [SerializeField]
        private int autoHealingPoints = 3;
        private float autoHealingCounter = 0;

        private float wallJumpCounter = 0;
        
        //Internal logic variables
        private bool canMove = true;
        private bool IsLeaningAWall, IsLeaningTheRightWall, IsLeaningTheLeftWall;
        private bool isAutoHealing = false, canDash = false;

        #region DASH
        [Header("Dash")]
        [SerializeField]
        private float dashXVelocity = 17.5f;
        [SerializeField]
        private float dashDelay = .26f;

        private bool isDashing = false;

        private float dashDelayCounter = 0;
        #endregion

        /// <summary>
        /// Determina posição em que o personagem quer se mover (left == -1 e Right == 1)
        /// </summary>
        float hAxis;

        private new ParticleSystem particleSystem;

        protected override void Awake()
        {
            base.Awake();

            particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        protected override void Start()
        {
            base.Start();

            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);

            LAYER_MASK_PLAYER = LayerMask.NameToLayer(playerKeysSO.otherPlayerLayer);

            cameraBehavior.Tranformers.Add(this.transform);

            //Habilita a colisão entre os players sempre que o player é reiniciado
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(playerKeysSO.otherPlayerLayer), false);

            baseAtributes.OnLifeChange += (healthPoints) =>
            {
                if (healthPoints == 0)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            };
        }

        private void OnEnable()
        {
            isAutoHealing = false;

            dashDelayCounter = 0;
        }

        private void OnDisable()
        {
            cameraBehavior.Tranformers.Remove(this.transform);
        }

        private void Update()
        {
            //Auto Heal
            if (isAutoHealing)
            {
                autoHealingCounter += Time.deltaTime;

                if (autoHealingCounter > AutoHealingDelay)
                {
                    baseAtributes.Heal(autoHealingPoints);

                    autoHealingCounter = 0;
                }
            }

            ReadInput();

            CheckCollisionTriggers();

            if (IsGrounded && !canDash)
                canDash = true;

            if (!canMove)
            {
                //Impossibilita que o player se mova durante um pequeno intervalo de tempo, logo após o salto
                wallJumpCounter += Time.deltaTime;

                if (wallJumpCounter < wallJumpDelay)
                    return;

                //Reseta o contador
                wallJumpCounter = 0;

                canMove = true;
            }

            if (isInvulnerable)
            {
                this.invulnerableCounter += Time.deltaTime;
                this.invulnerableAnimationCounter += Time.deltaTime;

                if (this.invulnerableCounter > this.maxInvulnerableCounter)
                {
                    isInvulnerable = false;
                    this.invulnerableCounter = 0;

                    this.invulnerableAnimationCounter = 0;

                    spriteRenderer.enabled = true;

                    Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
                }

                if (this.invulnerableAnimationCounter > this.maxAnimationInvulnerableCounter)
                {
                    this.invulnerableAnimationCounter = 0;

                    spriteRenderer.enabled = !spriteRenderer.enabled;
                }
            }
        }

        private void FixedUpdate()
        {
            if (canMove)
            {
                if (snapXVelocity)
                {
                    if (hAxis == 0)
                    {
                        Vector2 tempNewVelocity = rigidbody2D.velocity;

                        tempNewVelocity.x = 0;

                        rigidbody2D.velocity = tempNewVelocity;
                    }
                }

                // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
                if (hAxis * rigidbody2D.velocity.x < maxMovSpeed)
                {
                    // ... Adiciona força no moveimtno do personagem
                    rigidbody2D.AddForce(Vector2.right * hAxis * moveForce);
                }
            } else if (!isDashing && Mathf.Abs(rigidbody2D.velocity.x) > maxMovSpeed)
            {
                // ... define a velocidade do player como sendo sua velocidade máxima, no eixo X.
                rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxMovSpeed,
                                                   rigidbody2D.velocity.y);
            }

            //Se a velocidade vertical, quando o player está caindo, é menor do que sua velocidade mínima...
            if (rigidbody2D.velocity.y < minFallingVelocity)
            {
                // ... define a velocidade Y do player como sendo sua velocidade mínima, no eixo X.
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, minFallingVelocity);
            }
        }

        protected override void CheckCollisionTriggers()
        {
            //Verifica se o personagem está no chão
            IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckCircleRadius,
                                                 1 << LAYER_MASK_SCENERY)
                                                 
                         || Physics2D.OverlapCircle(groundCheck.position, groundCheckCircleRadius,
                                                 1 << LAYER_MASK_PLAYER);

            //Verifica se o personagem está encostado em alguma parede
            IsLeaningTheLeftWall =
                Physics2D.OverlapPoint(leftWallCheck.position, 1 << LAYER_MASK_SCENERY);

            IsLeaningTheRightWall =
                Physics2D.OverlapPoint(rightWallCheck.position, 1 << LAYER_MASK_SCENERY);

            IsLeaningAWall = IsLeaningTheLeftWall || IsLeaningTheRightWall;
        }

        private void ReadInput()
        {
            hAxis = Input.GetAxisRaw(playerKeysSO.horizontalAxis);

            if (canMove && !isDashing)
            {
                if (hAxis < 0)
                    LookLeft();
                else if (hAxis > 0)
                    LookRight();

                if (canDash && Input.GetKeyDown(playerKeysSO.dash))
                    DoDash();
            }
            else if (isDashing)
            {
                dashDelayCounter += Time.deltaTime;

                if (dashDelayCounter > dashDelay)
                {
                    dashDelayCounter = 0;
                    isDashing = false;
                    canMove = true;

                    rigidbody2D.velocity = Vector3.zero;

                    rigidbody2D.gravityScale = 1;

                    Physics2D.IgnoreLayerCollision(gameObject.layer, LAYER_MASK_PLAYER, false);
                }
            }

            //Permite que o personagem salte apenas quando está no chão
            if (!isDashing && Input.GetKeyDown(playerKeysSO.jumpKey) && (IsGrounded || IsLeaningAWall))
            {
                Vector2 jumpVelocity = new Vector2(0f, jumpForce);

                //Permite ao personagem saltar quando encostado na parede
                if (IsLeaningAWall && !IsGrounded)
                {
                    jumpVelocity.x = (IsLeaningTheRightWall ? -jumpForce : jumpForce) * wallJumpForce;

                    //Anula a velocidade atual do rigidbody antes de forçar o salto
                    rigidbody2D.velocity = Vector2.zero;

                    canMove = false;
                }

                // Add a vertical force to the player.
                rigidbody2D.AddForce(jumpVelocity, ForceMode2D.Impulse);

                isDashing = false;
            }

            if (Input.GetKeyDown(playerKeysSO.fire))
            {
                Attack();
            }
        }

        private void DoDash()
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LAYER_MASK_PLAYER, true);

            canDash = false;

            isDashing = true;

            rigidbody2D.velocity = new Vector3(isLookingLeft ? -dashXVelocity : dashXVelocity, 0);

            //Desabilita a gravidade
            rigidbody2D.gravityScale = 0;

            dashDelayCounter = 0;
        }

        protected virtual void LookRight()
        {
            isLookingLeft = false;
        }

        protected virtual void LookLeft()
        {
            isLookingLeft = true;
        }

        protected virtual void Attack()
        {
            
        }

        private void OnDrawGismoz()
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(groundCheck.position, groundCheckCircleRadius);
        }

        /// <summary>
        /// BLABLABLA
        /// </summary>
        /// <param name="collider"></param>
        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            if (!this.isInvulnerable && collider.gameObject.CompareTag("enemyShoot"))
            {
                baseAtributes.Damage(4);

                this.isInvulnerable = true;

                Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);
            }
            else if (collider.gameObject.CompareTag("healer"))
            {
                isAutoHealing = true;

                autoHealingCounter = 0;

                particleSystem.Play();
            }
            else if (collider.gameObject.CompareTag("spikes"))
            {
                baseAtributes.Die();
            }
        }

        protected void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("healer"))
            {
                isAutoHealing = false;

                particleSystem.Stop();
            }
        }

        protected override void OnCollisionStay2D(Collision2D collision2D)
        {
            if (collision2D.gameObject.CompareTag("enemy") && !this.isInvulnerable)
            {
                baseAtributes.Damage(4);

                Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

                this.isInvulnerable = true;
            }
        }
    }
}