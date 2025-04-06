using UnityEngine;
using UnityEngine.AI;

public class State
{

    public enum STATE
    {
        IDLE, PURSUE, ATTACK, SLEEP, PATROL
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    public Transform player;
    protected State nextState;
    protected NavMeshAgent agent;

    float visDist = 10.0f;
    float visAngle = 30.0f;
    float attackDist = 0.5f;

    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    {
        this.npc = _npc;
        this.agent = _agent;
        this.anim = _anim;
        this.player = _player;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
         
        }
        return this;
    }

    public bool canSeePlayer()
    {
        Vector3 direction = player.position - npc.transform.position;

        float angle = Vector3.Angle(direction, npc.transform.position);

        if (direction.magnitude < visDist && angle < visAngle)
        {
            return true;
        }
        return false;
    }


    public bool canAttackPlayer()
    {
        Vector3 direction = player.position - npc.transform.position;

        if (direction.magnitude < attackDist)
        {
            return true;
        }

        return false;
    }

    public class Idle : State
    {
        public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) : base(_npc, _agent, _anim, _player)
        {
            name = STATE.IDLE;
        }

        public override void Enter()
        {
            //anim.SetTrigger("isIdle");
            Debug.Log("entered idle");
            base.Enter();
        }


        public override void Update()
        {
            Debug.Log("Idling");
            if (player != null)
            {
                Debug.Log("starting to chase the player ");
                nextState = new Pursue(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }
        }

        public override void Exit()
        {
            anim.ResetTrigger("isIdle");
            Debug.Log("exiting idle");
            base.Exit();

        }
    }



    public class Patrol : State
    {
        public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) : base(_npc, _agent, _anim, _player)
        {
            Debug.Log("Patrolling");
        }

        public override void Enter()
        {
            anim.SetTrigger("Walking");
            base.Enter();
        }


        public override void Update()
        {
            if (Random.Range(0, 100) < 10)
            {
                nextState = new Patrol(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }


        }

        public override void Exit()
        {
            anim.ResetTrigger("Walking");
            base.Exit();

        }
    }

    public class Pursue : State
    {
        public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) : base(_npc, _agent, _anim, _player)
        {
            name = STATE.PURSUE;
            agent.speed = 5;
            agent.isStopped = false;
        }

        public override void Enter()
        {
            anim.SetBool("isRunning", true);
            Debug.Log("entered pursue state");
            base.Enter();
        }


        public override void Update()
        {
            agent.SetDestination(player.position);
            if (agent.hasPath)
            {
                if (canAttackPlayer())
                {
                    nextState = new Attack(npc, agent, anim, player);
                    stage = EVENT.EXIT;
                }
                else if (!canSeePlayer())
                {
                    nextState = new Idle(npc, agent, anim, player);
                    stage = EVENT.EXIT;
                }

            }

        }

        public override void Exit()
        {
            anim.SetBool("isRunning", false);
            base.Exit();

        }

    }

    public class Attack : State
    {
        float rotationSpeed = 2.0f;
        public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) : base(_npc, _agent, _anim, _player)
        {
            name = STATE.ATTACK;
        }

        public override void Enter()
        {
            anim.Play("Chain1");
            agent.isStopped = true;
            Debug.Log("entered attack state");
            base.Enter();


        }


        public override void Update()
        {
            Vector3 direction = player.position - npc.transform.position;

            float angle = Vector3.Angle(direction, npc.transform.position);

            direction.y = 0;

            npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);

            if (!canAttackPlayer())
            {
                nextState = new Idle(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }

        }

        public override void Exit()
        {
            //anim.SetBool("isRunning", false);

            base.Exit();

        }




        }


    }
