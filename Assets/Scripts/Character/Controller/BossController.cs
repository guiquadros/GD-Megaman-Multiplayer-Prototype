using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;
using Megaman.Player;
using System.Linq;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Megaman
{
    public class BossController : BaseController
    {
        [Header("Bomb")]
        [SerializeField]
        private GameObject  prefabEnemyShoot;
        [SerializeField]
        private Vector2 bombVelocity, enemyJumpVelocity;

        [Header("Triggers")]
        [SerializeField]
        private Transform leftWallTransform;
        [SerializeField]
        private Transform rightWallTransform;

        [Header("IA Delay")]
        [SerializeField]
        private float minThrowBombDelay;
        [SerializeField]
        private float maxThrowBombDelay;
        private float throwBombDelay, throwBombCounter;
        [SerializeField]
        private float minJumpDelay, maxJumpDelay;
        private float jumpCounter, jumpDelay;

        private List<Transform> players;

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            if (isInvulnerable)
                return;

            if (collider.gameObject.CompareTag("zero"))
            {
                baseAtributes.Damage(4);
            }
            else if (collider.gameObject.CompareTag("superShoot"))
            {
                baseAtributes.Damage(5);
            }
            else
            {
                baseAtributes.Damage(1);
            }

            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Megaman"), true);
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Zero"), true);

            isInvulnerable = true;
        }

        protected override void OnCollisionStay2D(Collision2D collision2D)
        {
            
        }

        protected override void Awake()
        {
            base.Awake();
            players = GameObject.FindObjectsOfType(typeof(PlayerController)).Select(o => (o as PlayerController).transform).ToList();
        }

        protected override void Start()
        {
            base.Start();

            throwBombCounter = jumpDelay = .5f;

            base.baseAtributes.Reset();

            baseAtributes.OnLifeChange += (damage) =>
            {
                if (damage == 0)
                {
                    SceneManager.LoadScene("TheEnd");
                };
            };
        }

        protected override void CheckCollisionTriggers()
        {
            //Verifica se o personagem está no chão
            IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckCircleRadius,
                                                 1 << LAYER_MASK_SCENERY);
        }

        void Update()
        {
            CheckCollisionTriggers();

            if (isInvulnerable)
            {
                invulnerableCounter += Time.deltaTime;
                this.invulnerableAnimationCounter += Time.deltaTime;

                if (invulnerableCounter > maxInvulnerableCounter)
                {
                    isInvulnerable = false;
                    invulnerableCounter = 0;

                    invulnerableAnimationCounter = 0;

                    spriteRenderer.enabled = true;

                    Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Megaman"), false);
                    Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Zero"), false);
                }

                if (invulnerableAnimationCounter > maxAnimationInvulnerableCounter)
                {
                    invulnerableAnimationCounter = 0;

                    spriteRenderer.enabled = !spriteRenderer.enabled;
                }
            }

            throwBombCounter += Time.deltaTime;

            if (IsGrounded)
                jumpCounter += Time.deltaTime;

            if (throwBombCounter > throwBombDelay)
            {
                throwBombCounter = 0;

                throwBombDelay = Random.Range(minThrowBombDelay, maxThrowBombDelay);

                ThrowBomb();
            }

            if (jumpCounter > jumpDelay)
            {
                jumpCounter = 0;

                jumpDelay = Random.Range(minJumpDelay, maxJumpDelay);

                Jump();
            }
        }

        private void Jump()
        {
            Transform selectedWall = Vector2.Distance(transform.position, leftWallTransform.position)
                                     > Vector2.Distance(transform.position, rightWallTransform.position) ?
                                        leftWallTransform : rightWallTransform;

            bool jumpRight = selectedWall.position.x > transform.position.x;

            float newXVelocity = Random.Range(4, 20);

            enemyJumpVelocity.x = jumpRight ? newXVelocity : -newXVelocity;

            rigidbody2D.velocity = enemyJumpVelocity;
        }

        private void ThrowBomb()
        {
            int randomPlayer = Random.Range(0, 2);

            float newDistance = Vector2.Distance(transform.position, players[randomPlayer].position);

            bool isLeft = transform.position.x > players[randomPlayer].position.x; ;

            GameObject gameObject = Lean.LeanPool.Spawn(prefabEnemyShoot);
            gameObject.transform.position = isLeft ? base.shootLeft.position : base.shootRight.position;
            bombVelocity.x = isLeft ? -newDistance : newDistance;
            gameObject.GetComponent<ShootController>().AddVelocity(bombVelocity);
        }

        void OnDisable()
        {
            //cameraBehavior.Tranformers.Remove(this.transform);
        }

    }
}
