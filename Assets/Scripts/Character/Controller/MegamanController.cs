using UnityEngine;
using System.Collections;

namespace Megaman.Player
{
    public class MegamanController : PlayerController
    {

        [Header("Shoot")]
        [SerializeField]
        private float shootVelocity;
        [SerializeField]
        private GameObject prefabShoot;
        public static GameObject PrefabShoot;

        protected override void Awake()
        {
            base.Awake();

            PrefabShoot = prefabShoot;
        }

        protected override void Attack()
        {
            GameObject gameObject = Lean.LeanPool.Spawn(PrefabShoot);

            gameObject.transform.position = isLookingLeft ? shootLeft.position : shootRight.position;

            gameObject.GetComponent<ShootController>().AddVelocity(isLookingLeft ? -shootVelocity : shootVelocity);
        }
    }
}