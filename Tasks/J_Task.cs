using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Conditions.Tasks
{
    /// <summary>
    /// A task is a condition that might be completed
    /// </summary>
    public abstract class J_Task : J_ReactiveCondition
    {
        #region FIELDS AND PROPERTIES
        private JGenericDelegate<J_Task> OnComplete;
        // --------------- TRIGGERS --------------- //
        [InfoBox("Null => Auto Start"), BoxGroup("Setup - Triggers", true, true, -15), SerializeField, AssetsOnly]
        private J_ReactiveCondition _startTrigger;
        [InfoBox("Null => Never Go Dormant"), BoxGroup("Setup - Triggers", true, true, -15), SerializeField, AssetsOnly]
        private J_ReactiveCondition _dormantTrigger;
        [InfoBox("Null => Auto Complete"), BoxGroup("Setup - Triggers", true, true, -15), SerializeField, AssetsOnly]
        private J_ReactiveCondition _completeTrigger;

        // --------------- EVENTS --------------- //
        [BoxGroup("Setup - Events", true, true, -10), SerializeField] private JUnityEvent _unityEvents_AtActivation;
        [BoxGroup("Setup - Events", true, true, -10), SerializeField] private JUnityEvent _unityEvents_AtDormant;
        [BoxGroup("Setup - Events", true, true, -10), SerializeField] private JUnityEvent _unityEvents_AtComplete;
        // --------------- BEHAVIOUR --------------- //
        [BoxGroup("Setup - Behaviour", true, true, -5), SerializeField] private bool _reactivateIfDormant = true;
        [BoxGroup("Setup - Behaviour", true, true, -5), SerializeField] private bool _requiresOneActivation = false;
        [BoxGroup("Setup - Behaviour", true, true, -5), SerializeField] private float _activationDelay = 0f;

        // --------------- STATE --------------- //
        [BoxGroup("State", true, true, 25), ShowInInspector, ReadOnly] protected J_TaskChunk _chunk;
        [BoxGroup("State", true, true, 25), ShowInInspector, ReadOnly] private bool _activatedOnce = false;
        [BoxGroup("State", true, true, 25), ShowInInspector, ReadOnly] private J_Collection_DormantTasks _Dormants => _chunk.Dormants;
        [BoxGroup("State", true, true, 25), ShowInInspector, ReadOnly] private TaskState _state = TaskState.WaitStartCondition;
        public TaskState State
        {
            get => _state;
            private set
            {
                JConsole.Log($"{name} enters {value}", JLogTags.Task, this);
                _state = value;
                if (State == TaskState.Complete) OnComplete?.Invoke(this);
            }
        }
        #endregion

        #region TUTORIAL STARTERS
        internal void InjectChunk(J_TaskChunk chunk) { _chunk = chunk; }

        // launch the tasks. wait for triggers otherwise we directly start
        protected override void StartCheckingCondition()
        {
            Assert.IsFalse(CurrentValue, $"{name} should start as false => not completed");
            _activatedOnce = false;
            //start or wait
            if (ActivationValid()) ConfirmActivation();
            else WaitingActivation();

            //subscribe to all triggers if the task is not started, otherwise only to complete
            if (_completeTrigger != null) _completeTrigger.SubscribeToCondition(TriggerComplete);

            if (State == TaskState.Active ||
                State == TaskState.ActivationWaiting) return;
            if (_startTrigger   != null) _startTrigger.SubscribeToCondition(TriggerStart);
            if (_dormantTrigger != null) _dormantTrigger.SubscribeToCondition(TriggerDormant);
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
            if (_Dormants.Contains(this)) RemoveFromDormants();
            Timing.CallDelayed(_activationDelay, TryActivating);
        }

        private void TryActivating()
        {
            if (State != TaskState.ActivationWaiting)
            {
                JConsole.Log($"{name} is no more active. State: {State}", JLogTags.Task, this);
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
            AddToDormants();
            State = TaskState.Dormant;
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
            if (_Dormants.Contains(this)) RemoveFromDormants();
            CompleteTutorialStep();
            CurrentValue = true;
            State        = TaskState.Complete;
        }
        #endregion

        #region CHECKS
        private bool ActivationValid() { return _startTrigger  == null || _startTrigger.CurrentValue; }
        private bool CompleteReady() { return _completeTrigger == null || _completeTrigger.CurrentValue; }
        #endregion

        #region DORMANT STATE AND ACTIVATION
        internal void ReactivateTask()
        {
            Assert.IsTrue(State == TaskState.Dormant, $"{name} - State {State} is not valid to be reactivated");
            if (ActivationValid()) ConfirmActivation();
            else State = TaskState.WaitStartCondition;
        }

        private void AddToDormants()
        {
            Assert.IsTrue(!_Dormants.Contains(this), $"{name} is dormant already {_Dormants.name}");
            _Dormants.Add(this);
        }

        private void RemoveFromDormants()
        {
            Assert.IsTrue(_Dormants.Contains(this), $"{name} is not yet dormant {_Dormants.name}");
            _Dormants.Remove(this);
        }
        #endregion

        #region ABSTRACT IMPLEMENTATION
        protected virtual void RunTask() { _unityEvents_AtActivation.Invoke(); }
        protected virtual void SetDormant() { _unityEvents_AtDormant.Invoke(); }
        protected virtual void CompleteTutorialStep() { _unityEvents_AtComplete.Invoke(); }
        #endregion

        #region SUBSCRIBERS
        public void SubscribeToComplete(JGenericDelegate<J_Task> actionToAdd) { OnComplete += actionToAdd; }
        public void UnSubscribeToComplete(JGenericDelegate<J_Task> actionToRemove) { OnComplete -= actionToRemove; }
        #endregion
    }

    //the states related to this tutorial
    public enum TaskState
    {
        NotInitialized = -100,
        WaitStartCondition = 0,
        ActivationWaiting = 50,
        Active = 100,
        Dormant = 200,
        Complete = 300
    }
}
