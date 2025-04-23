using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpiderPlayerTrigger : MonoBehaviour
{
    private SpiderNPCController spiderController;

    public void Initialize(SpiderNPCController controller)
    {
        spiderController = controller;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Damageable player = other.GetComponent<Damageable>();
            if (player != null)
            {
                spiderController.OnPlayerHit(player);
            }
        }
    }
}
