using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class LhamasDoForroh : MonoBehaviour
{
    private TankAI _tank;
    private float _minDistance;

    private void Awake()
    {
        _tank = GetComponent<TankAI>();
    }

    [Task]
    public void PickRandomDestination()
    {
        //Clear Current Path
        _tank.Agent.ResetPath();

        Vector3 randomDirection = Random.insideUnitSphere * 20.0f;
        Vector3 destination = Vector3.zero;

        //NavmeshHit
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(randomDirection, out navMeshHit, 20.0f, NavMesh.AllAreas);

        //Set Destination
        destination = navMeshHit.position;
        _tank.Agent.SetDestination(destination);

        //Debug
        Debug.DrawLine(_tank.Position, destination, Color.blue);

        Task.current.Succeed();
    }

    [Task]
    public void MoveDestination()
    {
        //Arrived at destinantion
        if (_tank.Agent.remainingDistance <= _tank.Agent.stoppingDistance && !_tank.Agent.pathPending)
        {
            Task.current.Succeed();
        }
    }


    [Task]
    public void GetDistance()
    {
        if (!DistanceToShoot(10.0f))
        {
            // _tank.Agent.isStopped = true;
            // Vector3 disToEnemy = _tank.Targets[0] - _tank.Position;
            // Vector3 targetPos = disToEnemy.normalized * -3.0f;
            // _tank.Agent.SetDestination(targetPos);
            Fire();
        }

        _tank.Agent.isStopped = false;
        _tank.Agent.ResetPath();
        Task.current.Succeed();
    }

    [Task]
    public void TakeCover()
    {
        _tank.StopFire();
        Vector3 awayFromTarget = (_tank.Position - _tank.Targets[0]).normalized;
        Vector3 destination = _tank.Position + awayFromTarget * 5;
        _tank.Agent.SetDestination(-destination);
        Task.current.Succeed();
    }

    [Task]
    public void Fire()
    {
        _tank.StartFire();
        Task.current.Succeed();
    }

    [Task]
    public void StopFire()
    {
        _tank.StopFire();
        Task.current.Succeed();
    }

    private void Update()
    {
        if (HasTargetInRange())
        {
            var lookRotation = Quaternion.LookRotation((_tank.Targets[0] - _tank.Position).normalized);
            _tank.transform.rotation = Quaternion.Slerp(_tank.transform.rotation, lookRotation, Time.deltaTime * 2.0f);
        }
    }

    [Task]
    public bool ShootLinedUp()
    {
        float angle = _tank.Angle(_tank.Targets[0]);
        Debug.Log(angle);
        if (Mathf.Abs(angle) <= 10)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [Task]
    public bool HasTargetInRange()
    {
        return _tank.HasTargetInRange;
    }

    [Task]
    public bool InDanger(float _minDistance)
    {
        return _tank.DistanceToTarget(_tank.Targets[0]) < _minDistance;
    }

    [Task]
    public bool DistanceToShoot(float _minDistance)
    {
        return _tank.DistanceToTarget(_tank.Targets[0]) > _minDistance;
    }

    [Task]
    public bool IsHealthLessThan(float health)
    {
        return _tank.Health < health;
    }
}