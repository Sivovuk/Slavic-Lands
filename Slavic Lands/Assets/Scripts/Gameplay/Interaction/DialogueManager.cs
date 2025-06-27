using System;
using System.Collections;
using Data.Interaction;
using Gameplay.Player;
using TMPro;
using UnityEngine;

namespace Gameplay.Interaction
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private GameObject _dialogueBox;
        [SerializeField] private TMP_Text _dialogueText;
        
        private string[] _sentences;
        private int _index;
        private bool _isTyping;
        
        private Coroutine _typingCoroutine;
        
        public static DialogueManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        public void StartDialogue(DialogueDataSO dialogueData)
        {
            _dialogueBox.SetActive(true);
            _sentences = dialogueData.Sentences;
            _index = 0;
            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);
            _typingCoroutine = StartCoroutine(TypeSentence(_sentences[_index]));
        }

        public void CloseDialogue()
        {
            _sentences = null;
            _index = 0;
            _dialogueText.text = "";
            _dialogueBox.SetActive(false);
            StopCoroutine(_typingCoroutine);
        }

        public void SkipAnimation()
        {
            if (_dialogueBox.activeInHierarchy)
            {
                if (_isTyping)
                {
                    StopAllCoroutines();
                    _dialogueText.text = _sentences[_index];
                    _isTyping  = false;
                }
                else
                    NextSentence();
            }
        }

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
                _dialogueBox.SetActive(false);
        }

        private IEnumerator TypeSentence(string sentence)
        {
            _isTyping = true;
            _dialogueText.text = "";
            foreach (var letter in sentence)
            {
                _dialogueText.text += letter;
                yield return new WaitForSeconds(0.02f);
            }
            _isTyping = false;
        }
    }
}