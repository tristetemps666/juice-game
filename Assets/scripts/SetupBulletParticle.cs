using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupBulletParticle : MonoBehaviour
{
    public Rigidbody2D bulletRB;

    private ParticleSystem particleSyst;
    // Start is called before the first frame update
    void Start()
    {
        particleSyst = GetComponent<ParticleSystem>();
        particleSyst.startSpeed= Vector2.SqrMagnitude(bulletRB.velocity);
        //particleSyst.startRotation = bulletRB.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
