using Unity.Netcode.Components;

// This script allows Clients to sync their own animations to the Host
public class OwnerNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}