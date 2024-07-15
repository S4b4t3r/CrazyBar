using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] props;
    [SerializeField] float spawnInterval;
    [SerializeField] float updateDifficultyEvery;
    [SerializeField] float updateSpeedBy;
    [SerializeField] AudioSource freefall;
    [SerializeField] BarmanController barman;
    float currentIntervalRemaining;

    float counter = 0f;
    bool enableSpawning = true;

    void Start()
    {
        currentIntervalRemaining = spawnInterval;
    }

    void Update()
    {
        if (enableSpawning)
        {
            if(currentIntervalRemaining < 0f)
            {
                barman.DoThrowAnimation();
                
                if(counter == updateDifficultyEvery) {
                    counter = 0f;
                    spawnInterval = spawnInterval - updateSpeedBy;
                    barman.MultiplyAnimatorSpeed(updateSpeedBy);
                } else {
                    counter += 1f;
                }
                currentIntervalRemaining = spawnInterval;
            } else {
                currentIntervalRemaining -= Time.deltaTime;
            }
        }
    }

    public void SpawnObject()
    {
        Instantiate(GetRandomProp(), new Vector2(Random.Range(-6f, 6f), 6f), Quaternion.identity);
        freefall.PlayDelayed(0.3f);
    }

    public void StopSpawning()
    {
        enableSpawning = false;
    }

    public GameObject GetRandomProp()
    {
        return props[Random.Range(0, props.Length)];
    }
}
