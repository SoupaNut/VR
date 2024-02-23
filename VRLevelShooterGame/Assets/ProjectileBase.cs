using UnityEngine;
using UnityEngine.Events;

namespace Unity.Game.Shared
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 Speed { get; private set; }
        public float Damage { get; private set; }

        public UnityAction OnShoot;

        public void Shoot(WeaponController controller)
        {
            InitialPosition = transform.position;
            InitialDirection = controller.WeaponMuzzle.forward;
            Speed = controller.ProjectileSpeed;
            Damage = controller.Damage;

            OnShoot?.Invoke();
        }
    }
}

