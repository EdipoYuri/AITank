using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class TankRapariga : MonoBehaviour
{
    private TankAI tankai;
    
    Vector3 lookPos;
    Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        tankai = GetComponent<TankAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(tankai.Targets != null)
        {
            tankai.TurretLookAt(tankai.Targets[0]);
        }
    }

    [Task]
    public void PickRandomDestination()
    {
        Vector3 destination = new Vector3(Random.Range(-35.0f, 35.0f), 0.0f, Random.Range(-35.0f, 35.0f));
        tankai.Agent.SetDestination(destination);
        Task.current.Succeed();
    }

    [Task]
    public void PickDestination(float x, float z)
    {
        Vector3 destination = new Vector3(x, 0.0f, z);
        tankai.Agent.SetDestination(destination);
        Task.current.Succeed();
    }

    [Task]
    public bool IsHealthLessThan(float health)
    {
        return tankai.Health < health;
    }

    [Task]
    public void TakeCover()
    {
        tankai.Agent.isStopped = true;
        Vector3 awayFromTarget = (transform.position - tankai.Targets[0]).normalized;
        Vector3 destination = transform.position + awayFromTarget * 5;
        tankai.Agent.isStopped = false;
        tankai.Agent.SetDestination(destination);
        Task.current.Succeed();
    }

    [Task]
    public bool Turn(float angle)
    {
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up) * transform.rotation;
        return true;
    }

    [Task]
    public bool InDanger(float minDistance)
    {
        return Vector3.Distance(tankai.Targets[0], transform.position) < minDistance;
    }

    [Task]
    public bool NotDanger(float minDistance)
    {
        return Vector3.Distance(tankai.Targets[0], transform.position) > minDistance;
    }

    [Task]
    public void explosao()
    {
        tankai.SelfDestruction();
        Task.current.Succeed();
    }

    [Task]
    public bool ninguem()
    {
        return tankai.Targets.Count == 0;
    }

    [Task]
    public bool doisoumais()
    {
        return tankai.Targets.Count >= 2;
    }

    [Task]
    public void Atacar()
    {
        tankai.StartFire();
        Task.current.Succeed();
    }

    [Task]
    public void Parar()
    {
        tankai.StopFire();
        Task.current.Succeed();

    }

    [Task]
    public void MoveDestination()
    {
        //Arrived at destinantion
        if (tankai.Agent.remainingDistance <= tankai.Agent.stoppingDistance && !tankai.Agent.pathPending)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public bool Parede(float min)
    {
        RaycastHit hit;
        return Physics.Raycast(tankai.Position, tankai.transform.forward ,out hit, min);   
    }

    [Task]
    public void Stop()
    {
        tankai.Agent.isStopped = true;
        Task.current.Succeed();
    }
}

