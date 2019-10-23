using System.Collections;
using System.Collections.Generic;
using Panda;
using UnityEngine;
using UnityEngine.AI;

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
        // _tank.Agent.isStopped = true;
        // Vector3 disToEnemy = _tank.Targets[0] - _tank.Position;
        // Vector3 targetPos = disToEnemy.normalized * -3.0f;
        // _tank.Agent.SetDestination(targetPos);

        _tank.Move(-1f);
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
    public void GetSafeDistance()
    {
        _tank.Agent.ResetPath();
        Fire();
        _tank.LookAt(_tank.Targets[0]);
        if (_tank.DistanceToTarget(_tank.Targets[0]) < 5.0f)
        {
            _tank.Agent.Move(new Vector3(0, -1f, 0));
        }

        if (_tank.DistanceToTarget(_tank.Targets[0]) > 15.0f)
        {
            _tank.Agent.Move(new Vector3(0, 1f, 0));
        }
    }

    [Task]
    public void Fire()
    {
        _tank.StartFire();
        _tank.Move(1f);
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
            _tank.TurretLookAt(_tank.Targets[0]);
        }

        if (HitSomethingInFront())
            Debug.Log("Hitou");
        else
        {
            Debug.Log("nothing happens feijoada");
        }

        if (HitSomethingInBack())
            Debug.Log("Hitou");
        else
        {
            Debug.Log("nothing happens feijoada");
        }
        if (TorretHitSomething())
            Debug.Log("Hitou");
        else
        {
            Debug.Log("nothing happens feijoada");
        }
        // RaycastHit hit;
        // //Frente (Layer 10: Obstacles)
        // if (Physics.Raycast(_tank.Position, transform.TransformDirection(Vector3.forward), out hit, 8.0f, 10))
        //     Debug.DrawRay(_tank.Position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        // else
        //     Debug.DrawRay(_tank.Position, transform.TransformDirection(Vector3.forward) * 100, Color.white);
        // //Tras (Layer 10: Obstacles)
        // if (Physics.Raycast(_tank.Position, transform.TransformDirection(-Vector3.forward), out hit, 8.0f, 10))
        //     Debug.DrawRay(_tank.Position, transform.TransformDirection(-Vector3.forward) * hit.distance, Color.yellow);
        // else
        //     Debug.DrawRay(_tank.Position, transform.TransformDirection(-Vector3.forward) * 100, Color.white);
        // //Torreta (Layer 9: Player)
        // if (Physics.Raycast(_tank.TurretDirection, transform.TransformDirection(Vector3.forward), out hit, 10.0f, 9))
        //     Debug.DrawRay(_tank.TurretDirection, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        // else
        //     Debug.DrawRay(_tank.TurretDirection, transform.TransformDirection(Vector3.forward) * 100, Color.white);

    }

    [Task]
    public bool HitSomethingInFront()
    {
        //RaycastHit hit;
        int layer = LayerMask.GetMask("Obstacles");
        Ray ray = new Ray(_tank.Position, transform.forward);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 6.0f, Color.blue);
        return Physics.Raycast(ray, 6.0f, layer);
    }

    [Task]
    public bool HitSomethingInBack()
    {
        //RaycastHit hit;
        int layer = LayerMask.GetMask("Obstacles");
        Ray ray = new Ray(_tank.Position, -transform.forward);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 6.0f, Color.blue);
        return Physics.Raycast(ray, 6.0f, layer);
    }

    [Task]
    public bool TorretHitSomething()
    {
        //RaycastHit hit;
        int layer = LayerMask.GetMask("Players");
        Ray ray = new Ray(_tank.Position, _tank.TurretDirection);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 15.0f, Color.yellow);
        return Physics.Raycast(ray, 6.0f, layer);
    }

    [Task]
    public bool ShootLinedUp()
    {
        float angle = _tank.Angle(_tank.Targets[0]);
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
    public bool InDanger(float _minDistance, float _maxDistance)
    {
        return _tank.DistanceToTarget(_tank.Targets[0]) < _minDistance || _tank.DistanceToTarget(_tank.Targets[0]) > _maxDistance;
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