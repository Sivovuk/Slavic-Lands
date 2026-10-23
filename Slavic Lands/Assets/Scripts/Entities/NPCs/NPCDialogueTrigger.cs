using System.Collections.Generic;
using Data.Interaction;
using Gameplay.Interaction;
using Gameplay.Player;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Starts this NPC's dialogue when it is the closest available NPC to the interacting player.
    /// </summary>
    public class NPCDialogueTrigger : MonoBehaviour
    {
        [SerializeField] private string _npcName = "Mještanin";
        [SerializeField] private DialogueDataSO _dialogueData; // The dialogue script assigned to this NPC
        [SerializeField] private Data.Shop.ShopSO _shopData;   // Optional shop assigned to this NPC
        private static readonly HashSet<NPCDialogueTrigger> ActiveTriggers = new HashSet<NPCDialogueTrigger>();

        private readonly HashSet<Collider2D> _playerColliders = new HashSet<Collider2D>();
        private PlayerInputSystem _playerInputSystem;

        private void OnEnable()
        {
            ActiveTriggers.Add(this);
        }

        private void OnDisable()
        {
            ActiveTriggers.Remove(this);
            UnsubscribeFromPlayerInput();

            if (DialogueManager.Instance != null &&
                DialogueManager.Instance.DialogueSource == gameObject)
                DialogueManager.Instance.CloseDialogue();

            if (UI.Interaction.NPCInteractionChoiceUI.Instance != null &&
                UI.Interaction.NPCInteractionChoiceUI.Instance.IsOpen)
                UI.Interaction.NPCInteractionChoiceUI.Instance.CloseChoices();

            _playerColliders.Clear();
        }

        /// <summary>
        /// Triggered when the player presses the interaction button while near the NPC.
        /// </summary>
        private void OnInteraction()
        {
            if (UI.Interaction.NPCInteractionChoiceUI.Instance != null && UI.Interaction.NPCInteractionChoiceUI.Instance.IsOpen)
                return;

            if (Gameplay.Shop.ShopManager.Instance != null && Gameplay.Shop.ShopManager.Instance.IsShopOpen)
                return;

            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager == null)
                return;

            if (dialogueManager.IsDialogueActive)
            {
                if (dialogueManager.DialogueSource == gameObject)
                    dialogueManager.SkipAnimation();

                return;
            }

            if (FindClosestInRange() != this)
                return;

            // If NPC offers a shop, show the choice panel (Talk / Shop / Leave)
            if (_shopData != null && UI.Interaction.NPCInteractionChoiceUI.Instance != null)
            {
                UI.Interaction.NPCInteractionChoiceUI.Instance.OpenChoices(
                    _npcName,
                    onTalk: () =>
                    {
                        if (_dialogueData != null)
                            dialogueManager.StartDialogue(_dialogueData, gameObject);
                    },
                    onShop: () =>
                    {
                        if (Gameplay.Shop.ShopManager.Instance != null)
                            Gameplay.Shop.ShopManager.Instance.OpenShop(_shopData);
                    },
                    onLeave: null,
                    hasShop: true
                );
                return;
            }

            if (_dialogueData == null)
                return;

            dialogueManager.StartDialogue(_dialogueData, gameObject);
        }

        /// <summary>
        /// Detects when the player enters the NPC's interaction zone.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || !_playerColliders.Add(other))
                return;

            if (_playerInputSystem == null)
            {
                _playerInputSystem = other.GetComponentInParent<PlayerInputSystem>();
                if (_playerInputSystem == null)
                {
                    Debug.LogError("Player entered an NPC interaction zone without a PlayerInputSystem.", this);
                    _playerColliders.Remove(other);
                    return;
                }

                _playerInputSystem.OnInteractionClick += OnInteraction;
            }
        }

        /// <summary>
        /// Detects when the player leaves the NPC's interaction zone.
        /// Closes the dialogue if it was open.
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || !_playerColliders.Remove(other))
                return;

            if (_playerColliders.Count > 0)
                return;

            UnsubscribeFromPlayerInput();

            if (DialogueManager.Instance != null &&
                DialogueManager.Instance.DialogueSource == gameObject)
                DialogueManager.Instance.CloseDialogue();

            if (UI.Interaction.NPCInteractionChoiceUI.Instance != null &&
                UI.Interaction.NPCInteractionChoiceUI.Instance.IsOpen)
                UI.Interaction.NPCInteractionChoiceUI.Instance.CloseChoices();
        }

        private static NPCDialogueTrigger FindClosestInRange()
        {
            NPCDialogueTrigger closest = null;
            float closestDistance = float.PositiveInfinity;

            foreach (NPCDialogueTrigger trigger in ActiveTriggers)
            {
                if (trigger == null || trigger._playerColliders.Count == 0 ||
                    trigger._playerInputSystem == null ||
                    (trigger._dialogueData == null && trigger._shopData == null))
                    continue;

                float distance = (trigger.transform.position - trigger._playerInputSystem.transform.position).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closest = trigger;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        private void UnsubscribeFromPlayerInput()
        {
            if (_playerInputSystem == null)
                return;

            _playerInputSystem.OnInteractionClick -= OnInteraction;
            _playerInputSystem = null;
        }
    }
}
