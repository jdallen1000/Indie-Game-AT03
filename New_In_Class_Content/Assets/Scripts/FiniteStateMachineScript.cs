namespace FiniteStateMachine
{
	public interface IState
	{
		void OnEnter();

		void OnUpdate();

		void OnExit();
	}

    public class StateMachine
		{
			public IState CurrentState { get; private set; }

			public StateMachine() { }

			public StateMachine(IState initState)
			{
				SetState(initState);
			}

			public void OnUpdate()
			{
				if (CurrentState != null)
				{
					CurrentState.OnUpdate();
				}
			}

			public void SetState(IState newState)
			{
				if (CurrentState != null)
				{
					CurrentState.OnExit();
				}

				CurrentState = newState;

				CurrentState.OnEnter();
			}

			public StateType GetCurrentStateAsType<StateType>() where StateType : class, IState
			{
				return CurrentState as StateType;
			}
		}
	}