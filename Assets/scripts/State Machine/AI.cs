using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{

    NavMeshAgent agent;
    public Animator anim;
    public Transform player;
    State currentState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = new State.Idle(this.gameObject, agent, anim, player) ;
    }

    // Update is called once per frame
    void Update()
    {
        currentState = currentState.Process();
    }
}
