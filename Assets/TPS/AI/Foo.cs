using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Foo : MonoBehaviour
{
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;
    [SerializeField] private Transform target;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    private void Update()
    {
       // agent.SetDestination(target.position);
        var corners = agent.path.corners;
        lineRenderer.SetPositions(corners);
    }

}
