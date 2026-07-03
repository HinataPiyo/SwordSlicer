using System.Collections.Generic;
using UnityEngine;

public class ParticleEndDestroy : MonoBehaviour
{
    List<ParticleSystem> particles = new List<ParticleSystem>();

    void Awake()
    {
        var p = GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in p) particles.Add(particle);
    }

    void Update()
    {
        for(int i = particles.Count - 1; i >= 0; i--)
        {
            if (!particles[i].IsAlive())
            {
                OnDisableParticle(particles[i]);
            }

            if(particles.Count == 0)
            {
                OnParticleEnd();
            }
        }
    }

    void OnDisableParticle(ParticleSystem target)
    {
        particles.Remove(target);
    }

    void OnParticleEnd()
    {
        Destroy(transform.parent.gameObject);    // 親オブジェクトを削除する
    }
}