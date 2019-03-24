using UnityEngine;
using System.Collections;

namespace Megaman.Player
{
    public class ZeroController : PlayerController
    {
        [Header("Zero")]
        [SerializeField]
        private Transform swordFlipper;
        [SerializeField]
        private Animator animator;

        protected override void Attack()
        {
            animator.SetTrigger("attack");
        }

        protected override void LookRight()
        {
            base.LookRight();

            Vector3 newScale = swordFlipper.localScale;

            newScale.x = -1;

            swordFlipper.localScale = newScale;
        }

        protected override void LookLeft()
        {
            base.LookLeft();

            Vector3 newScale = swordFlipper.localScale;

            newScale.x = 1;

            swordFlipper.localScale = newScale;
        }
    }
}