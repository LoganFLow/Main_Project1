using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDamageAndRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    public float fallThreshold = -500f; // Height below which the player respawns
    public Vector3 respawnPosition = Vector3.zero; // Position to respawn at

    private CharacterController controller;  // Reference to the CharacterController
    private Vector3 velocity;  // To reset velocity upon respawn

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("CharacterController not found on this GameObject.  FallDamageAndRespawn script will not function.", this);
            enabled = false; // Disable the script if there's no CharacterController
            return;
        }
    }

    private void Update()
    {
        // Check for falling below the threshold
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Find the object tagged "Player"

        if (player != null)
        {
            CharacterController playerController = player.GetComponent<CharacterController>(); // Get the CharacterController

            if (playerController != null)
            {
                playerController.enabled = false; // Disable controller during teleport
                player.transform.position = respawnPosition;
                // Reset velocity (optional, but often desirable)
                PlayerMove gtaMovement = player.GetComponent<PlayerMove>(); //Get the GTAStyleMovement Component
                if (gtaMovement != null)
                {
                    gtaMovement.ResetVelocity(); // Reset the velocity of the character, if it has a GTAStyleMovement script
                }
                playerController.enabled = true; // Re-enable controller
            }
            else
            {
                Debug.LogError("Player object does not have a CharacterController component.  Respawn will not work.", player);
            }

        }
        else
        {
            Debug.LogError("No GameObject with tag 'Player' found in the scene. Respawn will not work.", this);
        }
    }
}