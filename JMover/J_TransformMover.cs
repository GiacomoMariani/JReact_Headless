using MEC;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace JReact.Mover
{
    /// <summary>
    /// this class is used to move the transform and offers different ways to achieve the movement
    /// </summary>
    public class J_TransformMover : MonoBehaviour
    {
        #region VALUES AND PROPERTIES
        //a delegate to track the movement
        public delegate void MoveAction(Vector3 newPosition);
        private event MoveAction OnMove;
        private event MoveAction OnFixedPositionReached;

        //a tag for the movement
        private const string COROUTINE_MoveTransform = "COROUTINE_MoveTransform";
        //the starting constant id
        private const int INT_StartID = -1;

        //the reference to the transform
        [BoxGroup("Mover", true, true, 0), ReadOnly] protected Transform _ThisTransform => this.transform;
        //the id of the mover, it starts at -1, and then get identified
        [BoxGroup("Mover", true, true, 0), ReadOnly] private int _transformId = INT_StartID;
        [BoxGroup("Mover", true, true, 0), ReadOnly] private int _TransformId
        {
            get
            {
                if (_transformId == INT_StartID) _transformId = GetInstanceID();
                return _transformId;
            }
        }
        //the current position of the mover, used also to move it
        [BoxGroup("Mover", true, true, 0), ReadOnly] protected virtual Vector2 _CurrentPosition
        {
            get => _ThisTransform.position;
            set => _ThisTransform.position = value;
        }

        //movement speed
        [BoxGroup("Move State", true, true, 5), ShowInInspector] public float MoveSpeed { get; private set; } = 1f;
        //movement direction
        [BoxGroup("Move State", true, true, 5), ShowInInspector] public Vector2 MoveDirection { get; private set; } = new Vector2();
        //a bool to track movement
        [BoxGroup("Move State", true, true, 5) , ReadOnly] public bool IsMoving { get; private set; } = false;
        #endregion

        #region DIRECTION MOVEMENT - NORMALIZED
        /// <summary>
        /// this is used to start moving the object, given a speed and a direction
        /// </summary>
        /// <param name="direction">the direction we want to reach</param>
        /// <param name="speed">the speed of movement</param>
        public void StartMovingIntoDirection(Vector2 direction, float speed = 1f)
        {
            //set the direction and the speed
            ChangeDirection(direction);
            ChangeSpeed(speed);
            if (!IsMoving) BeginDirectionMoving();
        }

        /// <summary>
        /// this method set the direction we want to reach and normalize it.
        /// This method won't let the mover start moving, but just set the direction
        /// </summary>
        /// <param name="direction">the direction we want to reach</param>
        public void ChangeDirection(Vector2 direction) { MoveDirection = direction.normalized; }

        /// <summary>
        /// this is used to set the speed of the mover
        /// This method won't let the mover start moving, but just set the speed
        /// </summary>
        /// <param name="speed">the speed we want to set</param>
        public void ChangeSpeed(float speed) { MoveSpeed = speed; }

        //this method is used to start moving
        private void BeginDirectionMoving()
        {
            //set the bool as moving
            IsMoving = true;
            //start the coroutine
            Timing.RunCoroutine(DirectionMove().CancelWith(gameObject), Segment.FixedUpdate, _TransformId, COROUTINE_MoveTransform);
        }

        //this method is used to keep moving on the direction we set
        private IEnumerator<float> DirectionMove()
        {
            //stop the moving if requested
            if (!IsMoving) yield break;
            //move the transform on the given direction
            _CurrentPosition += (MoveDirection * MoveSpeed * Time.deltaTime);
            //send the event
            OnMove?.Invoke(_CurrentPosition);
            //wait one frame, then run again
            yield return Timing.WaitForOneFrame;
            Timing.RunCoroutine(DirectionMove().CancelWith(gameObject), Segment.FixedUpdate, _TransformId, COROUTINE_MoveTransform);
        }

        /// <summary>
        /// this will stop the transform movement, by stopping the move coroutine
        /// </summary>
        public void StopMoveTransform()
        {
            Timing.KillCoroutines(_TransformId, COROUTINE_MoveTransform);
            IsMoving = false;
        }
        #endregion

        #region DIRECTION MOVEMENT FIXED
        /// <summary>
        /// this is used to start moving the object, given only direction
        /// this method is more advanced and consider the speed inside the direction
        /// please consider to use StartMovingIntoDirection instead
        /// </summary>
        /// <param name="direction">the direction we want to reach</param>
        public void StartMovingFixedDirection(Vector2 direction)
        {
            //set the direction and the speed
            SetFixedDirection(direction);
            if (!IsMoving) BeginDirectionMoving();
        }

        /// <summary>
        /// this method is used to set a fixed direction, it consider that the direction will also have the intensity 
        /// (so speed will be set to 1)
        /// this method is more advanced and consider the speed inside the direction
        /// please consider to use SetDirection instead
        /// </summary>
        /// <param name="direction"></param>
        public void SetFixedDirection(Vector2 direction)
        {
            MoveDirection = direction;
            MoveSpeed     = 1f;
        }
        #endregion

        #region MOVE TO FIXED POSITION
        /// <summary>
        /// move the transform from a position to another
        /// </summary>
        /// <param name="start">the start coordinates</param>
        /// <param name="end">the end coordinates</param>
        /// <param name="timeToReach">the time to reach</param>
        public void MoveFromToPosition(Vector2 start, Vector2 end, float timeToReach)
        {
            _CurrentPosition = start;
            MoveTransformToPosition(end, timeToReach);
        }
        
        /// <summary>
        /// used to move a given transform on a specific point within a specific time
        /// </summary>
        /// <param name="positionToReach">the position to reach</param>
        /// <param name="timeToReach">the time to reach this position</param>
        public void MoveTransformToPosition(Vector2 positionToReach, float timeToReach)
        {
            //we run the move coroutine giving it a layer and a tag
            Timing.RunCoroutine(MoveToPosition(positionToReach, timeToReach).CancelWith(gameObject),
                                Segment.FixedUpdate, _TransformId, COROUTINE_MoveTransform);
        }

        //this is the coroutine to move the transform
        private IEnumerator<float> MoveToPosition(Vector2 positionToReach, float timeToReach)
        {
            //start moving
            IsMoving = true;
            //the time passed for this current step
            var elapsedTime = 0f;
            //the starting position is the position of the transform at start
            Vector2 startingPos = _CurrentPosition;
            //keep moving until we reach the time
            while (elapsedTime < timeToReach)
            {
                //moving with a lerp
                _CurrentPosition = Vector2.Lerp(startingPos, positionToReach, (elapsedTime / timeToReach));
                //adding the elapsed time
                elapsedTime += Time.deltaTime;
                //send the event
                OnMove?.Invoke(_CurrentPosition);
                //wait a frame
                yield return Timing.WaitForOneFrame;
            }

            //stop moving at the end
            IsMoving = false;
            //send the event for the fixed position reached
            OnFixedPositionReached?.Invoke(_CurrentPosition);
        }
        #endregion

        #region SUBSCRIBERS
        public void SubscribeToMovement(MoveAction actionToAdd) { OnMove                                += actionToAdd; }
        public void UnSubscribeToMovement(MoveAction actionToRemove) { OnMove                           -= actionToRemove; }
        public void SubscribeToReachFixedPosition(MoveAction actionToAdd) { OnFixedPositionReached      += actionToAdd; }
        public void UnSubscribeToReachFixedPosition(MoveAction actionToRemove) { OnFixedPositionReached -= actionToRemove; }
        #endregion

        #region TEST
        //a test method to start moving with direction and speed
        [BoxGroup("Test", true, true, 100), Button(ButtonSizes.Medium)]
        private void MoveWithDirectionAndSpeed() { StartMovingIntoDirection(MoveDirection, MoveSpeed); }

        //a method to just add a direction
        [BoxGroup("Test", true, true, 100), Button(ButtonSizes.Medium)]
        private void MoveWithJustDirection() { StartMovingFixedDirection(MoveDirection); }
        #endregion
    }
}
