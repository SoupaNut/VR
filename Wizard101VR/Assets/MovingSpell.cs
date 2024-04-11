using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpell : BasicSpell
{
    public Vector3 MovingAxis;
    public float Speed = 5f;
    public float CollisionRadius = 0.2f;
    public LayerMask CollisionLayer;

    public ParticleSystem Bolt;
    public ParticleSystem Trail;
    public ParticleSystem Explosion;


    Vector3 MovingWorldAxis;
    bool m_HasExploded;
    public override void Initialize(Transform wandTip)
    {
        base.Initialize(wandTip);
        MovingWorldAxis = wandTip.TransformDirection(MovingAxis);
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * MovingWorldAxis * Speed;
    }

    private void FixedUpdate()
    {
        if (!m_HasExploded)
        {
            Collider[] results = Physics.OverlapSphere(transform.position, CollisionRadius, CollisionLayer);
            if (results.Length > 0)
            {
                Explode();
            }
        }
    }

    public void Explode()
    {
        m_HasExploded = true;

        Explosion.Play();
        Trail.Stop();
        Bolt.Stop();

        Destroy(gameObject, 1f);
    }
}
