using System.Collections.Generic;
using Unity.Game.Shared;
using UnityEngine;
using System.Collections;

namespace Unity.Game.Gameplay.Spells
{
    public class BasicSpell : MonoBehaviour
    {
        // **************************************************************************************
        // -------------------------------------- GENERAL
        // **************************************************************************************
        [Header("General")]
        [Tooltip("Max amount of time the object stays active in the scene.")]
        public float MaxLifeTime = 5f;

        [Tooltip("Sound clip to play upon casting. The sound will play at the wandtip position.")]
        public AudioClip OnCastSfx;

        [Tooltip("How much damage the projectile deals upon impact")]
        public float Damage = 10f;

        protected Coroutine DestroyCoroutine { get; private set; }

        
        public GameObject Owner { get; private set; }
        public List<Collider> IgnoredColliders { get; private set; }

        protected virtual void Awake()
        {
        }

        public virtual void Cast(SpellcastManager manager)
        {
            transform.position = manager.WandTip.position;
            transform.forward = manager.WandTip.forward;

            Owner = manager.Owner;
            IgnoredColliders = manager.IgnoredColliders;


            if (OnCastSfx)
                AudioUtility.CreateSfx(OnCastSfx, transform.position, AudioUtility.AudioGroups.Spells, 1f);

            DestroyCoroutine = StartCoroutine(DestroySpell(MaxLifeTime));
        }

        IEnumerator DestroySpell(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
}