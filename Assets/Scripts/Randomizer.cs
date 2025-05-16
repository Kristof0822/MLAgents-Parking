using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : MonoBehaviour
{

    [Header("Positions")]
    public List<Transform> parkingSpots;     // 6 spot
    public List<Transform> parkedCars;       // 5 autó
    public Transform currentTargetSpot;      // a kiválasztott üres célhely
    public Material defaultMat;
    public Material targetMat;

    float backOffset = 2.4f; // hátrafelé (a falhoz)
    float upOffset = 0.5f;    // felfelé (ne lógjon bele az aszfaltba)

    public void RandomizePositions()
    {
        // Shuffle the parking spaces
        List<Transform> shuffledSpots = new List<Transform>(parkingSpots);
        Shuffle(shuffledSpots);

        // Place cars on the first N spot.
        for (int i = 0; i < parkedCars.Count; i++)
        {
            parkedCars[i].position = shuffledSpots[i].position;
            parkedCars[i].rotation = shuffledSpots[i].rotation;
        }

        // Bollard placed on the last spot.
        Transform bollardSpot = shuffledSpots[parkedCars.Count];
        
        Vector3 offsetPosition = bollardSpot.position
                        + bollardSpot.forward * backOffset
                        + bollardSpot.up * upOffset;

        currentTargetSpot = shuffledSpots[parkedCars.Count];

        foreach (var spot in parkingSpots)
        {
            var rend = spot.GetComponent<Renderer>();
            if (rend != null)
                rend.material = defaultMat;
        }

        // A kiválasztott célspot → cél szín
        var targetRenderer = currentTargetSpot.GetComponent<Renderer>();
        if (targetRenderer != null)
            targetRenderer.material = targetMat;
    }

    private void Shuffle(List<Transform> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            var temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}
