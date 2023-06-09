using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FiniteStateMachine;

public class EnemyPathfinding : MonoBehaviour
{
	//reference to the NavMeshAgent attached to object
	private NavMeshAgent agent;

	//object the Ai is trying to get to 
	[SerializeField] GameObject[] navPoint;
	[SerializeField] GameObject TargetNavPoint;

	[SerializeField] GameObject player;

	//floats relating to detection distance and speed of objects
	[SerializeField]
	float stoppingDistance,
		  detectionDistance,
		  runSpeed,
		  walkSpeed;

	float idleTimer;
	[SerializeField] Vector3 GeneratedNavPoint;


	public StateMachine StateMachine { get; private set; }

	private Animator animC;

	private void Awake()
	{
		StateMachine = new StateMachine();

		if (!TryGetComponent<NavMeshAgent>(out agent))
		{
			Debug.LogError("This object needs a navmesh agent attached to it");
		}

		animC = GetComponent<Animator>();

	}

	//Start is called before the first frame update
	void Start()
	{
		StateMachine.SetState(new IdleState(this));
		agent.isStopped = true;
	}

	//Update is called once per frame
	void Update()
	{
		StateMachine.OnUpdate();
	}

	private Vector3 UpdateTargetPosition(GameObject targetPos)
	{
		return targetPos.transform.position;
	}

	public abstract class EnemyMoveState : IState
	{
		protected EnemyPathfinding instance;

		public EnemyMoveState(EnemyPathfinding _instance)
		{
			instance = _instance;
		}

		public virtual void OnEnter()
		{

		}

		public virtual void OnExit()
		{

		}

		public virtual void OnUpdate()
		{

		}
	}

	/* idle state
	 * chase player if in range
	 * idle timer that has a random range between 3-10 seconds that counts down
	 * idle timer reaches 0 and goes back to default state	 
	 */
	public class IdleState : EnemyMoveState
	{
		public IdleState(EnemyPathfinding _instance) : base(_instance)
		{
		}

		public override void OnEnter()
		{
			Debug.Log("Entering Idle State!");
			instance.agent.isStopped = true;

			instance.animC.SetTrigger("Idle");
			instance.idleTimer = Random.Range(3, 10);
			Debug.Log(instance.idleTimer);
		}

		public override void OnUpdate()
		{
			if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.detectionDistance)
			{
				instance.StateMachine.SetState(new ChaseState(instance));
			}
			else if (instance.idleTimer <= 0)
			{
				instance.StateMachine.SetState(new WanderState(instance));
			}
			else if (instance.idleTimer > 0)
			{
				instance.idleTimer -= 1 * Time.deltaTime;
			}
		}

		public override void OnExit()
		{
			instance.animC.ResetTrigger("Idle");
		}
	}
	/* wander state
	 * determine available spot on navmesh and move toward it
	 * enter chase state if player in range
	 * idle state when point reached.
	 */
	public class WanderState : EnemyMoveState
	{
		public WanderState(EnemyPathfinding _instance) : base(_instance)
		{

		}

		public override void OnEnter()
		{
			//set the agent to 'stopped'
			instance.agent.isStopped = false;
			Debug.Log("Entering Wander State!");

			instance.agent.speed = instance.walkSpeed;

			instance.RandomNavmeshLocation(instance.detectionDistance * 2);
			Debug.Log(instance.GeneratedNavPoint);

			instance.animC.SetTrigger("Walk");


		}

		public override void OnUpdate()
		{
			//update the position of the target object
			//move towards it
			if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.detectionDistance)
			{
				instance.StateMachine.SetState(new ChaseState(instance));
			}
			else if (Vector3.Distance(instance.transform.position, instance.GeneratedNavPoint) > instance.stoppingDistance)
			{
				instance.agent.SetDestination(instance.GeneratedNavPoint);
			}
			else
			{
				//set the state to IdleState
				instance.StateMachine.SetState(new IdleState(instance));
			}
		}

		public override void OnExit()
		{

			instance.animC.ResetTrigger("Walk");
		}
	}
	/* chase state
	 * chase the player when it gets in detection range
	 * becomes the default state when the object is picked up
	 */
	public class ChaseState : EnemyMoveState
	{
		public ChaseState(EnemyPathfinding _instance) : base(_instance)
		{
		}

		public override void OnEnter()
		{
			Debug.Log("Entering Chase State!");
			instance.agent.isStopped = false;

			instance.agent.speed = instance.runSpeed;

			instance.animC.SetTrigger("Run");
		}

		public override void OnUpdate()
		{
			if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.detectionDistance)
			{
				instance.agent.SetDestination(instance.player.transform.position);
			}
			else
			{
				instance.StateMachine.SetState(new IdleState(instance));
			}
		}

		public override void OnExit()
		{
			instance.animC.ResetTrigger("Run");
		}

	}
	
	/* patrol state
	 * chase player if in range
	 * move between different patrol points that are pre determined IN ORDER
	 */
	public class PatrolState : EnemyMoveState
	{

		public PatrolState(EnemyPathfinding _instance) : base(_instance)
		{
		}

		public override void OnEnter()
		{
			Debug.Log("Entering Patrol State!");
			instance.agent.isStopped = false;

			instance.agent.speed = instance.walkSpeed;

			instance.animC.SetTrigger("Walk");
			instance.TargetNavPoint = instance.navPoint[Random.Range(0, instance.navPoint.Length)];
		}

		public override void OnUpdate()
		{
			//update the position of the target object
			//move towards it

			if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.detectionDistance)
			{
				instance.StateMachine.SetState(new ChaseState(instance));
			}
			else if (Vector3.Distance(instance.transform.position, instance.TargetNavPoint.transform.position) > instance.stoppingDistance)
			{
				instance.agent.SetDestination(instance.TargetNavPoint.transform.position);
			}
			else
			{
				//set the state to IdleState
				instance.StateMachine.SetState(new IdleState(instance));
			}
		}

		public override void OnExit()
		{
			instance.animC.ResetTrigger("Run");
		}

	}

	public Vector3 RandomNavmeshLocation(float radius)  // generates a random point on the nav mesh by creating a random postion in a sphere around the agent
	{
		Vector3 randomDirection = Random.insideUnitSphere * radius;
		randomDirection += transform.position;
		NavMeshHit hit;
		if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
		{
			GeneratedNavPoint = hit.position;
		}
		return GeneratedNavPoint;
	}
}




	//	private void onDrawGizmos()
	//	{
	//		Gizmos.color = Color.green;
	//		Gizmos.DrawWireSphere(transform.position, detectionDistance);
	//	}