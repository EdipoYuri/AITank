using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class LhamasDoForroh : MonoBehaviour
{
    private TankAI _tank;
    private float _minDistance;
    Vector3 destination;

    private void Awake()
    {
        _tank = GetComponent<TankAI>();
    }

    [Task]
    public void PickRandomDestination()
    {
        _tank.Agent.ResetPath();
        Vector3 randomDirection = Random.insideUnitSphere * 20.0f;

        //NavmeshHit
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(randomDirection, out navMeshHit, 20.0f, NavMesh.AllAreas);

        destination = navMeshHit.position;

        _tank.Agent.SetDestination(destination);
        
        Task.current.Succeed();
    }

    [Task]
    public void MoveDestination()
    {
        //Raycast para frente do tanque.
        

        RaycastHit hit;
        if (Physics.Raycast(_tank.Position, _tank.transform.forward, out hit, 3f))
        {
            Task.current.Fail();
        }

        if (_tank.Agent.remainingDistance <= _tank.Agent.stoppingDistance && !_tank.Agent.pathPending)
        {
            Task.current.Succeed();
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
    public void GetDistance()
    {
        // _tank.Agent.Stop();
        // Vector3 distToEnemy = _tank.Targets[0] - _tank.Position;
        // Vector3 targetPos = distToEnemy.normalized * -3.0f;
        // _tank.Agent.destination = targetPos;
        // _tank.Agent.Resume();



        if (!DistanceToShoot(15.0f))
        {
            _tank.Agent.Stop();
            _tank.Agent.ResetPath();
            //_tank.Move(-1f);
        }

        //Raycast para trás do tanque.
        // RaycastHit hit;
        // if (Physics.Raycast(_tank.Position, -_tank.transform.forward, out hit, 1.0f))
        // {
        //     Task.current.Fail();
        // }

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
    public bool IsHealthLessThan(float health)
    {
        return _tank.Health < health;
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
            //_tank.Agent.Stop();
            //_tank.Agent.ResetPath();
            _tank.LookAt(_tank.Targets[0]);
        }

        Debug.DrawLine(_tank.Position, destination, Color.blue);
    }
}