using Unity.Netcode;
using UnityEngine;

public class ChainLogic : NetworkBehaviour
{
    private LineRenderer line;
    private GameObject otherPlayer;

    [SerializeField] private float maxDistance = 4f; // How long the chain is
    [SerializeField] private float pullStrength = 8f; // How hard it pulls you back

    private void Start()
    {
        // This links the code to the LineRenderer component you added
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
    }

    void Update()
    {
        // Search for the partner until found
        if (otherPlayer == null)
        {
            FindOtherPlayer();
            return;
        }

        // 1. DRAW THE VISUAL LINE (Runs on both screens)
        // Position 0 is Me, Position 1 is Partner
        line.SetPosition(0, transform.position + Vector3.up);
        line.SetPosition(1, otherPlayer.transform.position + Vector3.up);

        // 2. PHYSICS PULL (Only runs for the person owning this character)
        if (!IsOwner) return;

        float currentDistance = Vector3.Distance(transform.position, otherPlayer.transform.position);

        if (currentDistance > maxDistance)
        {
            Vector3 direction = (otherPlayer.transform.position - transform.position).normalized;

            // Move this player toward the partner
            GetComponent<CharacterController>().Move(direction * pullStrength * Time.deltaTime);
        }
    }

    private void FindOtherPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length < 2) return; // Stop if it's just you (Solo mode)

        foreach (var p in players)
        {
            if (p != gameObject)
            {
                otherPlayer = p;
                line.enabled = true; // Only show chain if partner is found
                break;
            }
        }
    }
}