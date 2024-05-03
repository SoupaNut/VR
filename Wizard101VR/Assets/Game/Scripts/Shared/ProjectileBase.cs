using UnityEngine;
using UnityEngine.Events;

namespace Unity.Game.Shared
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        public GameObject Owner { get; private set; }
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public float Speed { get; private set; }
        public float Damage { get; private set; }

        public UnityAction onShoot;

        public void Shoot(WeaponController controller)
        {
            Owner = controller.Owner;
            InitialPosition = transform.position;
            InitialDirection = controller.WeaponMuzzle.forward;
            Speed = controller.ProjectileSpeed;
            Damage = controller.Damage;

            onShoot?.Invoke();
        }
    }
}