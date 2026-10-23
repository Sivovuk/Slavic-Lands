using System;
using System.Collections;
using Data.Interaction;
using Gameplay.Player;
using TMPro;
using UnityEngine;

namespace Gameplay.Interaction
{
    /// <summary>
    /// Singleton class that manages displaying dialogue lines on the screen.
    /// Handles typing effect, sentence progression, and UI visibility.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private GameObject _dialogueBox;     // UI container for dialogue
        [SerializeField] private TMP_Text _dialogueText;      // Text component for showing sentences

        private string[] _sentences;      // Sentences from the dialogue script
        private int _index;               // Tracks current sentence index
        private bool _isTyping;           // Is a sentence currently being typed?

        private Coroutine _typingCoroutine;

        // Singleton instance for global access
        public static DialogueManager Instance { get; private set; }

        public bool IsDialogueActive { get; private set; }
        public GameObject DialogueSource { get; private set; }
        public event Action<GameObject> DialogueStarted;
        public event Action<GameObject> DialogueClosed;

        private void Awake()
        {
            // Simple singleton enforcement
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        /// <summary>
        /// Begins a new dialogue session by activating UI and starting the typing effect.
        /// </summary>
        public void StartDialogue(DialogueDataSO dialogueData, GameObject source = null)
        {
            if (dialogueData == null || dialogueData.Sentences == null || dialogueData.Sentences.Length == 0)
            {
                Debug.LogError("Cannot start a dialogue without at least one sentence.", this);
                return;
            }

            if (IsDialogueActive)
                CloseDialogue();

            _dialogueBox.SetActive(true);
            _sentences = dialogueData.Sentences;
            _index = 0;
            DialogueSource = source;
            IsDialogueActive = true;
            _typingCoroutine = StartCoroutine(TypeSentence(_sentences[_index]));

            if (PlayerController.Instance != null && PlayerController.Instance.PlayerMovement != null)
            {
                PlayerController.Instance.PlayerMovement.SetMovementLock(true);
            }

            DialogueStarted?.Invoke(DialogueSource);
        }

        /// <summary>
        /// Ends the dialogue, resets state, and hides the UI.
        /// </summary>
        public void CloseDialogue()
        {
            if (!IsDialogueActive)
                return;

            GameObject source = DialogueSource;
            IsDialogueActive = false;
            DialogueSource = null;

            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
            _isTyping = false;
            _sentences = null;
            _index = 0;
            _dialogueText.text = "";
            _dialogueBox.SetActive(false);

            if (PlayerController.Instance != null && PlayerController.Instance.PlayerMovement != null)
            {
                PlayerController.Instance.PlayerMovement.SetMovementLock(false);
            }

            DialogueClosed?.Invoke(source);
        }

        /// <summary>
        /// Called when player presses interaction button during dialogue.
        /// Skips typing or proceeds to next sentence.
        /// </summary>
        public void SkipAnimation()
        {
            if (IsDialogueActive)
            {
                if (_isTyping)
                {
                    if (_typingCoroutine != null)
                        StopCoroutine(_typingCoroutine);

                    _dialogueText.text = _sentences[_index];
                    _isTyping = false;
                    _typingCoroutine = null;
                }
                else
                    NextSentence();
            }
        }

        /// <summary>
        /// Advances to the next sentence or ends dialogue if finished.
        /// </summary>
        private void NextSentence()
        {
            _index++;
            if (_index < _sentences.Length)
            {
                if (_typingCoroutine != null)
                    StopCoroutine(_typingCoroutine);

                _typingCoroutine = StartCoroutine(TypeSentence(_sentences[_index]));
            }
            else
            {
                CloseDialogue();
            }
        }

        /// <summary>
        /// Types out a sentence character by character for a typing effect.
        /// </summary>
        private IEnumerator TypeSentence(string sentence)
        {
            _isTyping = true;
            _dialogueText.text = "";

            foreach (var letter in sentence)
            {
                _dialogueText.text += letter;
                yield return new WaitForSeconds(0.02f); // Adjust speed here
            }

            _isTyping = false;
            _typingCoroutine = null;
        }
    }
}
