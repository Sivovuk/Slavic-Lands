using System;
using Gameplay.Interaction;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities
{
    public class NPCMovement : EntityMovement
    {
        public enum MovementState
        {
            Idle,
            Walking
        }

        [Header("Villager Movement")]
        [SerializeField, Min(0f)] private float _wanderRadius = 3f;
        [SerializeField, Min(0f)] private float _minimumIdleDuration = 1f;
        [SerializeField, Min(0f)] private float _maximumIdleDuration = 3f;

        private float _homeX;
        private float _targetX;
        private float _stateTimer;
        private bool _isPaused;
        private MovementState _stateBeforePause;
        private DialogueManager _dialogueManager;

        public MovementState State { get; private set; } = MovementState.Idle;
        public event Action<MovementState> StateChanged;

        protected override void Start()
        {
            _homeX = transform.position.x;
            SetState(MovementState.Idle);
            _stateTimer = RandomIdleDuration();
            StateChanged?.Invoke(State);
            SubscribeToDialogueManager();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SubscribeToDialogueManager();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            UnsubscribeFromDialogueManager();
            if (_isPaused)
                SetPaused(false);
        }

        protected override void FixedUpdate()
        {
            if (_isPaused)
                return;

            if (State == MovementState.Idle)
            {
                _stateTimer -= Time.fixedDeltaTime;
                if (_stateTimer <= 0f)
                    StartWalking();

                return;
            }

            float nextX = Mathf.MoveTowards(
                transform.position.x,
                _targetX,
                Mathf.Max(0f, WalkSpeed) * Time.fixedDeltaTime
            );
            transform.position = new Vector3(nextX, transform.position.y, transform.position.z);

            if (Mathf.Approximately(nextX, _targetX))
            {
                SetState(MovementState.Idle);
                _stateTimer = RandomIdleDuration();
            }
        }

        public void SetPaused(bool isPaused)
        {
            if (_isPaused == isPaused)
                return;

            _isPaused = isPaused;
            if (isPaused)
            {
                _stateBeforePause = State;
                SetState(MovementState.Idle);
            }
            else
            {
                SetState(_stateBeforePause);
            }
        }

        private void StartWalking()
        {
            float radius = Mathf.Max(0f, _wanderRadius);
            float minX = _homeX - radius;
            float maxX = _homeX + radius;

            _targetX = Random.Range(minX, maxX);
            if (Mathf.Abs(_targetX - transform.position.x) < 0.1f && radius > 0f)
                _targetX = _targetX < _homeX ? maxX : minX;

            SetState(MovementState.Walking);
        }

        private float RandomIdleDuration()
        {
            float minimum = Mathf.Max(0f, _minimumIdleDuration);
            float maximum = Mathf.Max(minimum, _maximumIdleDuration);
            return Random.Range(minimum, maximum);
        }

        private void SetState(MovementState state)
        {
            if (State == state)
                return;

            State = state;
            StateChanged?.Invoke(State);
        }

        private void HandleDialogueStarted(GameObject source)
        {
            if (source == gameObject)
                SetPaused(true);
        }

        private void HandleDialogueClosed(GameObject source)
        {
            if (source == gameObject)
                SetPaused(false);
        }

        private void SubscribeToDialogueManager()
        {
            if (DialogueManager.Instance == null || _dialogueManager == DialogueManager.Instance)
                return;

            UnsubscribeFromDialogueManager();
            _dialogueManager = DialogueManager.Instance;
            _dialogueManager.DialogueStarted += HandleDialogueStarted;
            _dialogueManager.DialogueClosed += HandleDialogueClosed;

            if (_dialogueManager.IsDialogueActive && _dialogueManager.DialogueSource == gameObject)
                SetPaused(true);
        }

        private void UnsubscribeFromDialogueManager()
        {
            if (_dialogueManager == null)
                return;

            _dialogueManager.DialogueStarted -= HandleDialogueStarted;
            _dialogueManager.DialogueClosed -= HandleDialogueClosed;
            _dialogueManager = null;
        }
    }
}