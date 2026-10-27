using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object;

public class LocalPlayerSetup : NetworkBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private AudioListener audioListener;

    public override void OnStartClient()
    {
        bool isLocalPlayer = IsOwner;

        if (isLocalPlayer)
        {
            playerInput.gameObject.layer = 7;
        }

        if (playerInput != null)
            playerInput.enabled = isLocalPlayer;

        if (playerCamera != null && !isLocalPlayer)
            Destroy(playerCamera);

        if (audioListener != null)
            audioListener.enabled = isLocalPlayer;
    }
}
