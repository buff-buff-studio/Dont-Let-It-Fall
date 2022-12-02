using System.Collections;
using System.Collections.Generic;
using DLIFR.Props;
using UnityEngine;

public class LeviathanManager : MonoBehaviour
{
    public bool leviathanActive = true;
    public Vector2 timeBetweenSpawns = new Vector2(30,120);
    public Vector2 timeBetweenAppearances = new Vector2(5, 10);
    public Transform leviathanTransform;
    
    public Transform[] apearingPoints;
    public Transform[] spawnPoints;
    
    public float leviathanSpeed = 10;
    
    public Spawner spawner;
    
    public float timeToSpawn;
    public float stateTime;
    
    public bool leviathanIsSpawning = false;
    public bool dropCargo = false;
    
    public int leviathanState = 0;
    
    private Animator leviathanAnimator => GetComponent<Animator>();
    private Rigidbody leviathanRigidbody => leviathanTransform.GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        if (!leviathanActive) return;
        
        if (Time.time > timeToSpawn)
        {
            timeToSpawn = float.MaxValue;

            leviathanIsSpawning = true;
            leviathanState = 0;
        }

        if (!leviathanIsSpawning) return;

        switch (leviathanState)
        {
            case 0:
                leviathanTransform.eulerAngles = new Vector3(0, 90, 0);
                leviathanTransform.position = apearingPoints[0].position;
                leviathanRigidbody.velocity = Vector3.zero;
                leviathanAnimator.Play("Enter");
                leviathanState = 1;
                break;
            case 1:
                leviathanTransform.position = Vector3.MoveTowards(leviathanTransform.position, apearingPoints[1].position, leviathanSpeed *2* Time.fixedDeltaTime);
                if (Vector3.Distance(leviathanTransform.position, apearingPoints[1].position) <= 0.1f)
                    leviathanState = 2;
                break;
            case 2:
                leviathanAnimator.Play("Exit");
                leviathanRigidbody.AddForce(leviathanTransform.forward * leviathanSpeed, ForceMode.VelocityChange);
                stateTime = Time.time + Random.Range(timeBetweenAppearances.x, timeBetweenAppearances.y) + 1;
                leviathanState = 3;
                break;
            case 3:
                if (Time.time > stateTime)
                {
                    leviathanRigidbody.velocity = Vector3.zero;
                    leviathanAnimator.Play("Enter");
                    leviathanTransform.eulerAngles = new Vector3(0, -90, 0);
                    leviathanTransform.position = spawnPoints[0].position;
                    dropCargo = true;
                    leviathanState = 4;
                }
                break;
            case 4:
                leviathanTransform.position = Vector3.MoveTowards(leviathanTransform.position, spawnPoints[1].position, leviathanSpeed * Time.fixedDeltaTime);
                if(Vector3.Distance(leviathanTransform.position, spawnPoints[1].position) <= 0.1f) 
                    leviathanState = 5;
                if(dropCargo && Vector3.Distance(leviathanTransform.position, transform.position) <= 0.1f)
                {
                    spawner.Spawn(leviathanTransform.GetChild(2).GetComponentsInChildren<Transform>());
                    dropCargo = false;
                }
                break;
            case 5:
                leviathanAnimator.Play("Exit");
                leviathanRigidbody.AddForce(leviathanTransform.forward * leviathanSpeed, ForceMode.VelocityChange);
                leviathanIsSpawning = false;
                timeToSpawn = Time.time + Random.Range(timeBetweenSpawns.x, timeBetweenSpawns.y);
                break;
        }
    }
}
