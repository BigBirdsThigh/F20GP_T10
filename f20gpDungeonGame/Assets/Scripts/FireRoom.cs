using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FireRoom : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private GameObject[] fireTiles;
    private bool fireScriptGoing = false;
    bool running = false;
    bool fireOn = false;
    bool warningStarted = false;
    bool behaviourRunning = false;

    void Start()
    {
        //get all the fire tiles.
        fireTiles = GameObject.FindGameObjectsWithTag("FireTile");
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(FireBehaviour());
    }

    private IEnumerator FireBehaviour()
    {
        if (running == true || warningStarted == true)
        {
            yield break;
        }
        running = true;

        // time between fire turning off and on, can change later
        int delay = Random.Range(5, 7);
        yield return new WaitForSeconds(delay);

        // turn fire off or on
        if (fireOn)
        {
            StopFire();
        }
        else
        {
            StartCoroutine(fireWarning());
        }
        running = false;
    }


    private IEnumerator fireWarning()
    {
        if (warningStarted == true)
        {
            yield break;
        }

        print("starting warning");

        warningStarted = true;

        ParticleSystem ps;

        // get particle system components and allow particles to be spawned again.
        foreach (GameObject tile in fireTiles)
        {
            ps = tile.GetComponentInChildren<ParticleSystem>();

            var mainModule = ps.main;
            var emission = ps.emission;

            mainModule.startLifetime = 0.1f;
            emission.enabled = true;
        }

        yield return new WaitForSeconds(2.5f);
        print("warning over");
        StartFire();
        warningStarted = false;
    }

    private void StartFire()
    {
        print("starting big fire");
        fireOn = true;
        ParticleSystem ps;

        // get particle system components and allow particles to be spawned again.
        foreach (GameObject tile in fireTiles)
        {
            ps = tile.GetComponentInChildren<ParticleSystem>();
            var mainModule = ps.main;
            mainModule.startLifetime = 0.7f;
        }
    }

    private void StopFire()
    {
        fireOn = false;
        ParticleSystem ps;

        //get partcile system components and stop new particles being spawned
        foreach (GameObject tile in fireTiles)
        {
            ps = tile.GetComponentInChildren<ParticleSystem>();
            var emission = ps.emission;
            emission.enabled = false;
        }
    }
}
