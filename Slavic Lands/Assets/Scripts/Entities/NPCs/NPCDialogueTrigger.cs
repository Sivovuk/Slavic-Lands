using System;
using Data.Interaction;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Interaction
{
    public class NPCDialogueTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueDataSO _dialogueData;
        private PlayerInputSystem _playerInputSystem;
        
        private bool _playerInRange;

        private void Awake()
        {
            _playerInputSystem = GetComponent<PlayerInputSystem>();
        }

        private void OnEnable()
        {
            _playerInputSystem.OnInteractionClick += OnInteraction;
        }

        private void OnDisable()
        {
            _playerInputSystem.OnInteractionClick -= OnInteraction;
        }


        private void OnInteraction()
        {
            if (_playerInRange)
            {
                DialogueManager.Instance.StartDialogue(_dialogueData);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                _playerInRange = true;
        }

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