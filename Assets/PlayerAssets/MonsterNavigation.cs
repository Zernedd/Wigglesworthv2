using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class MonsterNavigation : MonoBehaviour
{
    public float DetectionRange = 5f;
    public float MonsterSpeedWander = 5f;
    public float MonsterSpeedChase = 7.5f;
    public NavMeshAgent agent;
    public Transform[] points;
    public string tagString = "User";

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            //Debug.LogError("NavMeshAgent component not found on this GameObject.");
            return;
        }

      //  Debug.Log("MonsterNavigation started. Setting speed to wander.");
        agent.speed = MonsterSpeedWander;
        Wander();
    }

    void Update()
    {
        if (agent == null)
            return;

        GameObject[] players = GameObject.FindGameObjectsWithTag(tagString);

        GameObject target = null;

        if (players.Length > 0)
        {
            float minDistance = float.MaxValue;
            foreach (GameObject player in players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
               // Debug.Log($"Distance to player {player.name}: {distance}");

                if (distance < DetectionRange && distance < minDistance)
                {
                    minDistance = distance;
                    target = player;
                }
            }

            if (target != null)
            {
               // Debug.Log($"Target found: {target.name}, chasing...");
                agent.speed = MonsterSpeedChase;
                Chase(target.transform);
            }
            else if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
               // Debug.Log("No target found, wandering again.");
                agent.speed = MonsterSpeedWander;
                Wander();
            }
        }

        if (target != null)
        {
           // Debug.Log($"Setting destination to target: {target.transform.position}");
            agent.destination = target.transform.position;
        }
    }

    void Chase(Transform target)
    {
        if (agent.enabled)
        {
          //  Debug.Log("Chase() called.");
            agent.destination = target.position;
        }
    }

    void Wander()
    {
        if (points.Length == 0)
        {
          //  Debug.LogWarning("No wander points assigned.");
            return;
        }

        int destPoint = Random.Range(0, points.Length);
        Vector3 wanderPos = points[destPoint].position;
       // Debug.Log($"Wandering to point {destPoint} at position {wanderPos}");
        agent.destination = wanderPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }
}
