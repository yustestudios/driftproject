using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public EnterRaceTrack[] races;
    public bool enteredTriggerEvent = false;

    public void EnteredTriggerEventManager(EnterRaceTrack enterRaceTrack)
    {
        print("EnteredTriggerEventManager: " + enterRaceTrack.gameObject.name);

        // Desactivar todas las carreras excepto la proporcionada
        foreach (EnterRaceTrack race in races)
        {
            if (race != enterRaceTrack)
            {
                race.gameObject.SetActive(false);
            }
        }
    }

    public void Restart()
    {
        // Activar todas las carreras
        foreach (EnterRaceTrack race in races)
        {
            race.gameObject.SetActive(true);
        }
    }
}
