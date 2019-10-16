using System.Collections;
using System.Collections.Generic;
using Panda;
using UnityEngine;
using UnityEngine.AI;

public class LhamasDoForroh : MonoBehaviour {
    [SerializeField]
    private TankAI m_Tank;
    private float minDistance;
    NavMeshHit hit;
    private void Awake () {
        m_Tank = GetComponent<TankAI> ();
    }

    [Task]
    public void PickRandomDestination () {
        Vector3 destination = new Vector3 (Random.Range (-20.0f, 20.0f), 0.0f, Random.Range (-20.0f, 20.0f));
        Vector3 randomDirection = Random.insideUnitSphere * 20.0f;
        randomDirection += transform.position;
        NavMesh.SamplePosition (randomDirection, out hit, 20.0f, NavMesh.AllAreas);
        Vector3 finalPosition = hit.position;
        m_Tank.Agent.SetDestination (destination);
        Task.current.Succeed ();
    }

    [Task]
    public void MoveDestination () {
        Vector3 myPosition = m_Tank.transform.position;
        Vector3 rayDirection = m_Tank.transform.forward;
        int rayLengthMeters = 3;
        RaycastHit hitInfo;

        if (Physics.Raycast (myPosition, rayDirection, out hitInfo, rayLengthMeters)) {
            Task.current.Fail ();
        }

        if (m_Tank.Agent.remainingDistance <= m_Tank.Agent.stoppingDistance && !m_Tank.Agent.pathPending) {
            Task.current.Succeed ();
        }
    }

    [Task]
    public bool SeeEnemy () {
        bool retorno = false;
        if (m_Tank.Targets.Count > 0) {
            for (int i = 0; i < m_Tank.Targets.Count; i++) {
                if (m_Tank.Targets[i] != null) {
                    retorno = true;
                }
            }
        }
        return retorno;
    }

    [Task]
    public bool InDanger (float minDistance) {
        return Vector3.Distance (m_Tank.Targets[0], transform.position) < minDistance;
    }

    [Task]
    public bool DistanceToShoot (float minDistance) {
        return Vector3.Distance (m_Tank.Targets[0], transform.position) > minDistance;
    }

    [Task]
    public void GetDistance () {
        if(!DistanceToShoot(15.0f)){
            m_Tank.Move (-1.0f);
        }
        Task.current.Succeed ();
    }

    [Task]
    public void TakeCover () {
        m_Tank.StopFire ();
        Vector3 awayFromTarget = (transform.position - m_Tank.Targets[0]).normalized;
        Vector3 destination = transform.position + awayFromTarget * 5;
        m_Tank.Agent.SetDestination (destination);
        Task.current.Succeed ();
    }

    [Task]
    public bool IsHealthLessThan (float health) {
        return m_Tank.Health < health;
    }

    [Task]
    public void AlignTank () {
        if(!ShootLinedUp()){
            m_Tank.LookAt (m_Tank.Targets[0]);
        }else{
            Task.current.Succeed();
        }
    }

    [Task]
    public bool ShootLinedUp () {
        Vector3 dir = transform.position - m_Tank.Targets[0];
        float angle=Vector3.Angle(transform.position, dir);
        Debug.Log(angle);
        if (Mathf.Abs(angle)<5) {
            return true;
        } else {
            return false;
        }
    }

    [Task]
    public void Fire () {
        m_Tank.StartFire ();
        Task.current.Succeed ();
    }

    [Task]
    public void StopFire () {
        m_Tank.StopFire ();
        Task.current.Succeed ();
    }
}