using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public enum NPCState
    {
        Idle_Up = 0,
        Idle_Down = 1,
        Idle_Left = 2,
        Idle_Right = 3,
        Walk_Up = 4,
        Walk_Down = 5,
        Walk_Left = 6,
        Walk_Right = 7,
        Idle_Book = 8,
        Idle_Phone = 9
    }

    [RequireComponent(typeof(Animator))]
    public class Script_NPCAnimationManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private GameManager _gameManager; 
        [SerializeField] private Script_NPCActionsManager _actionsManager;
        [SerializeField] private Script_NPCMovementManager _movementManager;

        [Header("Components")]
        [SerializeField] private Animator _animator;

        [Header("Line of Sight")]
        [SerializeField] private Script_NPCLineOfSight _los;

        [Header("Animation Clip Names")]
        [SerializeField] private List<string> _animationNames;

        [Header("Interactions")]
        [SerializeField] private float _minInteractTime;
        [SerializeField] private float _maxInteractTime;
        Waypoint _waypointProperties = null;

        // State
        private NPCState _currentState = NPCState.Idle_Down;
        private bool _isReading;



        // Start is called before the first frame update
        void Start()
        {
            _gameManager = GameManager._instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_actionsManager.IsInteracting)
            {
                if (_movementManager.IsMoving) PlayMovementAnimations();
                else PlayIdleAnimations();
            }
        }

        private void PlayMovementAnimations()
        {
            // Priorities direction of greater influence
            if (_movementManager.IsHorizontalGreater)
            {
                //Debug.Log("Horizontal is greater");
                if (_movementManager.IsWalkingRight)
                {
                    PlayAnimation(NPCState.Walk_Right);
                }
                else
                {
                    PlayAnimation(NPCState.Walk_Left);
                }
            }
            else
            {
                //Debug.Log("Vertical is greater");
                if (_movementManager.IsWalkingUp)
                {
                    PlayAnimation(NPCState.Walk_Up);
                }
                else
                {
                    PlayAnimation(NPCState.Walk_Down);
                }
            }
        }

        private void PlayIdleAnimations()
        {
            //_animator.SetBool("isMoving", _movementManager.IsMoving);
            PlayAnimation(_waypointProperties.NPCBehaviour);
        }

        private void PlayAnimation(NPCState toState)
        {
            if (toState == _currentState) return;

            Debug.Log("State " + toState + " resolves to " + (int) toState);
            _animator.Play(_animationNames[(int) toState], 0);
            _currentState = toState;
        }

        public void InteractAtWaypoint(GameObject waypoint)
        {
            _waypointProperties = waypoint.GetComponent<Waypoint>();
            StartCoroutine(HoldPosition());
            if (_waypointProperties.ShouldInteract)
            {
                //_los.ShrinkLOS();
            }

            IEnumerator HoldPosition()
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(_minInteractTime, _maxInteractTime));
                
                _gameManager.LetNPCWalk(_movementManager);
                //if (_waypointProperties.ShouldInteract) _los.ExpandLOS();
            }
        }
    }
}
