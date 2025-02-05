using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WormEffects : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;
    public WormAI bossAI;

    [Header("Effects")]
    [SerializeField] ParticleSystem dirtEffect = default;
    [SerializeField] ParticleSystem waterEffect = default;
    ParticleSystem enterParticle, exitParticle;
    [SerializeField] LayerMask terrainLayer = default;

void Start()
{
    bossAI = GetComponent<WormAI>();
    impulseSource = GetComponent<CinemachineImpulseSource>();
    bossAI.GroundContact.AddListener((boolA, boolB) => GroundContact(boolA, boolB));
    bossAI.GroundDetection.AddListener((x, y) => GroundParticleChange(x, y));

}



    void Update()
    {
    RaycastHit hitInfo;
if (Physics.Raycast(bossAI.startPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
    {
        Debug.Log($"Raycast hit at: {hitInfo.point}, Object: {hitInfo.transform.name}");
        Debug.DrawRay(bossAI.startPosition, Vector3.down * 1000, Color.red, 5f); // Draw the ray in the editor
        enterParticle = hitInfo.transform.CompareTag("Terrain") ? dirtEffect : waterEffect;
    }


    // Debugging end position raycast
    if (Physics.Raycast(bossAI.endPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
    {
        Debug.Log($"Raycast hit at: {hitInfo.point}, Object: {hitInfo.transform.name}");
        Debug.DrawRay(bossAI.endPosition, Vector3.down * 1000, Color.blue, 5f); // Draw the ray in the editor
        exitParticle = hitInfo.transform.CompareTag("Terrain") ? dirtEffect : waterEffect;
    }
    impulseSource.GenerateImpulse();


    }

    void GroundParticleChange(bool start, int particle)
    {
        if(start)
            enterParticle = particle == 0 ? dirtEffect : waterEffect;
        else
            exitParticle = particle == 0 ? dirtEffect : waterEffect;
    }

    void GroundContact(bool state, bool start)
    {
        if (start)
        {
            if (state)
            {
                enterParticle.transform.position = Vector3.Lerp(bossAI.startPosition, bossAI.endPosition, .1f);
                enterParticle.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
                enterParticle.Play();
            }
            else
            {
                enterParticle.Stop();
            }
        }
        else
        {
            if (state)
            {
                exitParticle.transform.position = Vector3.Lerp(bossAI.endPosition, bossAI.startPosition, .22f);
                exitParticle.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
                exitParticle.Play();
            }
            else
            {
                exitParticle.Stop();
            }
        }
    }

}
