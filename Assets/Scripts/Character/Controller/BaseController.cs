using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Megaman
{
    public abstract class BaseController : MonoBehaviour
    {
        [Header("Attributes")]
        [SerializeField]
        protected BaseAtributesSO baseAtributes;

        [Header("Shoot")]
        [SerializeField]
        protected Transform shootRight;
        [SerializeField]
        protected Transform shootLeft;

        protected int LAYER_MASK_SCENERY;

        

        /// <summary>
        /// Raio do circulo utilizado para verificar colisão com o cenário.
        /// O ideal é manter este valor em .21f.
        /// </summary>
        protected const float groundCheckCircleRadius = .21f;

        protected new Rigidbody2D rigidbody2D;

        protected SpriteRenderer spriteRenderer;

        protected CameraBehavior cameraBehavior;

        protected bool IsGrounded, isInvulnerable = false;

        //Collision detection Transforms
        protected Transform groundCheck, leftWallCheck, rightWallCheck;

        [Header("Invunerable")]
        [SerializeField]
        protected float maxInvulnerableCounter = 4;
        protected float invulnerableCounter;

        protected float invulnerableAnimationCounter;
        [SerializeField]
        protected float maxAnimationInvulnerableCounter = .13f;

        protected abstract void CheckCollisionTriggers();

        protected virtual void Start()
        {
            LAYER_MASK_SCENERY = LayerMask.NameToLayer("Scenery");
            baseAtributes.Reset();
        }

        protected virtual void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            rigidbody2D = GetComponent<Rigidbody2D>();
            groundCheck = transform.FindChild("groundCheck");

            cameraBehavior = FindObjectOfType<CameraBehavior>();

            if (groundCheck == null)
                throw new MissingComponentException("groundCheck não encontrado!");

            leftWallCheck = transform.FindChild("leftWallCheck");

            if (leftWallCheck == null)
                throw new MissingComponentException("leftWallCheck não encontrado!");

            rightWallCheck = transform.FindChild("rightWallCheck");

            if (rightWallCheck == null)
                throw new MissingComponentException("rightWallCheck não encontrado!");

            shootRight = transform.FindChild("shootRight");

            shootLeft = transform.FindChild("shootLeft");
        }

        protected abstract void OnTriggerEnter2D(Collider2D collider);
        protected abstract void OnCollisionStay2D(Collision2D collision2D);

    }
}