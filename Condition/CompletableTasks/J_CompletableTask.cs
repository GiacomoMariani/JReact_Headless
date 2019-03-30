using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Conditions.Tasks
{
    /// <summary>
    /// A task is a condition that might be completed
    /// </summary>
    public class J_CompletableTask : J_ReactiveCondition, iObservable<J_CompletableTask>
    {
        #region FIELDS AND PROPERTIES
        private JGenericDelegate<J_CompletableTask> OnTaskUpdate;
        // --------------- TRIGGERS --------------- //
        [InfoBox("Null => Auto Start"), BoxGroup("Setup - Triggers", true, true, -15), SerializeField, AssetsOnly]
        private J_ReactiveCondition _startTrigger;
        [InfoBox("Null => Never Go Dormant"), BoxGroup("Setup - Triggers", true, true, -15), SerializeField, AssetsOnly]
        private J_ReactiveCondition _dormantTrigger;
        [InfoBox("Null => Auto Complete"), BoxGroup("Setup - Triggers", true, true, -15), SerializeField, AssetsOnly]
        private J_ReactiveCondition _completeTrigger;

        // --------------- EVENTS --------------- //
        [BoxGroup("Setup - Events", true, true, -10), SerializeField] private JUnityEvent _unityEvents_AtActivation
            = new JUnityEvent();
        [BoxGroup("Setup - Events", true, true, -10), SerializeField] private JUnityEvent _unityEvents_AtDormant
            = new JUnityEvent();
        [BoxGroup("Setup - Events", true, true, -10), SerializeField] private JUnityEvent _unityEvents_AtComplete
            = new JUnityEvent();
        // --------------- BEHAVIOUR --------------- //
        [BoxGroup("Setup - Behaviour", true, true, -5), SerializeField] private bool _reactivateIfDormant = true;
        [BoxGroup("Setup - Behaviour", true, true, -5), SerializeField] private bool _requiresOneActivation;
        [BoxGroup("Setup - Behaviour", true, true, -5), SerializeField] private float _activationDelay;

        // --------------- STATE --------------- //
        [BoxGroup("State", true, true, 25), ShowInInspector, ReadOnly] private bool _activatedOnce;
        [BoxGroup("State", true, true, 25), ShowInInspector, ReadOnly] private TaskState _state = TaskState.NotInitialized;
        public TaskState State
        {
            get => _state;
            private set
            {
                JLog.Log($"{name} enters {value}", JLogTags.Task, this);
                _state = value;
                OnTaskUpdate?.Invoke(this);
            }
        }
        #endregion

        #region IMPLEMENTORS
        public static J_CompletableTask CreateTask<T>(J_ReactiveCondition start, J_ReactiveCondition complete,
                                                      J_ReactiveCondition dormant = null, bool autoReactivate = true,
                                                      bool required = false)
            where T : J_CompletableTask
        {
            var task = CreateInstance<T>();
            task._startTrigger          = start;
            task._dormantTrigger        = dormant;
            task._completeTrigger       = complete;
            task._reactivateIfDormant   = autoReactivate;
            task._requiresOneActivation = required;
            return task;
        }
        #endregion

        #region ACTIVATORS
        // launch the tasks. wait for triggers otherwise we directly start
        protected override void StartCheckingCondition()
        {
            Assert.IsFalse(CurrentValue, $"{name} should start as false => not completed");

            // --------------- TRACKING --------------- //
            //subscribe to all triggers if the task is not started, otherwise only to complete
            if (_completeTrigger != null) _completeTrigger.SubscribeToCondition(TriggerComplete);
            if (_dormantTrigger  != null) _dormantTrigger.SubscribeToCondition(TriggerDormant);
            if (_startTrigger != null) _startTrigger.SubscribeToCondition(TriggerStart);
            
            // --------------- STARTUP --------------- //
            State = TaskState.Startup; 
            if (AlreadyComplete()) return;
            _activatedOnce = false;

            // --------------- ACTIVATION --------------- //
            if (ActivationValid()) ConfirmActivation();
            else WaitingActivation();
        }

        private bool AlreadyComplete()
        {
            if (_requiresOneActivation || !CompleteReady()) return false;
            ConfirmComplete();
            return true;
        }

        //we reset on disable, when the scriptable object goes out of scope
        protected override void StopCheckingCondition()
        {
            _state = TaskState.NotInitialized;
            if (_startTrigger    != null) _startTrigger.UnSubscribeToCondition(TriggerStart);
            if (_completeTrigger != null) _completeTrigger.UnSubscribeToCondition(TriggerComplete);
            if (_dormantTrigger  != null) _dormantTrigger.UnSubscribeToCondition(TriggerDormant);
        }

        private void WaitingActivation() { State = TaskState.WaitStartCondition; }
        #endregion

        #region ACTIVE STATE
        private void TriggerStart(bool item)
        {
            if (!item) return;
            //ignore if already active, completed
            if (State == TaskState.Active   ||
                State == TaskState.Complete ||
                State == TaskState.ActivationWaiting) return;

            //a trigger start can be valid if we want to reactivate the task
            if (State == TaskState.Dormant &&
                !_reactivateIfDormant) return;

            ConfirmActivation();
        }

        private void ConfirmActivation()
        {
            State = TaskState.ActivationWaiting;
            if (_activationDelay > 0f) Timing.CallDelayed(_activationDelay, TryActivating);
            else TryActivating();
        }

        private void TryActivating()
        {
            if (State != TaskState.ActivationWaiting)
            {
                JLog.Log($"{name} is no more active. State: {State}", JLogTags.Task, this);
                return;
            }

            // --------------- ACTIVATION --------------- //
            ProcessActivation();

            //auto complete if requested
            if (CompleteReady()) ConfirmComplete();
        }

        private void ProcessActivation()
        {
            _activatedOnce = true;
            RunTask();
            State = TaskState.Active;
        }
        #endregion

        #region DORMANT STATE
        private void TriggerDormant(bool item)
        {
            if (!item) return;
            if (State != TaskState.Active &&
                State != TaskState.ActivationWaiting) return;

            ConfirmDormant();
        }

        private void ConfirmDormant()
        {
            SetDormant();
            State = TaskState.Dormant;
        }

        internal void ReactivateTask()
        {
            Assert.IsTrue(State == TaskState.Dormant, $"{name} - State {State} is not valid to be reactivated");
            if (ActivationValid()) ConfirmActivation();
            else State = TaskState.WaitStartCondition;
        }
        #endregion

        #region COMPLETE STATE
        private void TriggerComplete(bool item)
        {
            if (!item) return;
            ConfirmComplete();
        }

        private void ConfirmComplete()
        {
            if (State == TaskState.NotInitialized) return;
            if (_requiresOneActivation && !_activatedOnce) return;
            StopCheckingCondition();
            CompleteTutorialStep();
            CurrentValue = true;
            State        = TaskState.Complete;
        }
        #endregion

        #region CHECKS
        private bool ActivationValid() => _startTrigger  == null || _startTrigger.CurrentValue;
        private bool CompleteReady() => _completeTrigger == null || _completeTrigger.CurrentValue;
        #endregion

        #region VIRTUAL IMPLEMENTATION
        protected virtual void RunTask() { _unityEvents_AtActivation.Invoke(); }
        protected virtual void SetDormant() { _unityEvents_AtDormant.Invoke(); }
        protected virtual void CompleteTutorialStep() { _unityEvents_AtComplete.Invoke(); }
        #endregion

        #region SUBSCRIBERS
        public void SubscribeToTaskChange(JGenericDelegate<J_CompletableTask> action) { Subscribe(action); }
        public void UnSubscribeToTaskChange(JGenericDelegate<J_CompletableTask> action) { UnSubscribe(action); }
        public void Subscribe(JGenericDelegate<J_CompletableTask> action) { OnTaskUpdate   += action; }
        public void UnSubscribe(JGenericDelegate<J_CompletableTask> action) { OnTaskUpdate -= action; }
        #endregion
    }

    //the states related to this tutorial
    public enum TaskState
    {
        NotInitialized = -100, Startup = -50, WaitStartCondition = 0, ActivationWaiting = 50, Active = 100, Dormant = 200, Complete = 300
    }
}
