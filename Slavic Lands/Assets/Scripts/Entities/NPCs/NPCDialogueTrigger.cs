using Data.Interaction;
using Gameplay.Interaction;
using Gameplay.Player;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Enables NPCs to trigger dialogue when the player interacts nearby.
    /// Uses Unity's trigger detection and listens for interaction input to start/close dialogue.
    /// </summary>
    public class NPCDialogueTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueDataSO _dialogueData; // The dialogue script assigned to this NPC
        private PlayerInputSystem _playerInputSystem;          // Handles player interaction input

        private bool _playerInRange; // Whether the player is currently within interaction range

        private void Awake()
        {
            // Assumes the same object has the input system component
            _playerInputSystem = GetComponent<PlayerInputSystem>();
        }

        private void OnEnable()
        {
            // Listen for interaction input
            _playerInputSystem.OnInteractionClick += OnInteraction;
        }

        private void OnDisable()
        {
            // Stop listening to avoid errors on disable/destroy
            _playerInputSystem.OnInteractionClick -= OnInteraction;
        }

        /// <summary>
        /// Triggered when the player presses the interaction button while near the NPC.
        /// </summary>
        private void OnInteraction()
        {
            if (_playerInRange)
            {
                DialogueManager.Instance.StartDialogue(_dialogueData);
            }
        }

        /// <summary>
        /// Detects when the player enters the NPC's interaction zone.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                _playerInRange = true;
        }

        /// <summary>
        /// Detects when the player leaves the NPC's interaction zone.
        /// Closes the dialogue if it was open.
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
                DialogueManager.Instance.CloseDialogue();
            }
        }
    }
}
