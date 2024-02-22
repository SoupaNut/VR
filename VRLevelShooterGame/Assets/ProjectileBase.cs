using UnityEngine;
using UnityEngine.Events;

namespace Unity.Game.Base
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 InheritedMuzzleVelocity { get; private set; }

        public UnityAction OnShoot;

        public void Shoot(WeaponController controller)
        {
            InitialPosition = transform.position;
            InitialDirection = transform.forward;
            InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;

            OnShoot?.Invoke();
        }
    }
}

