using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorylineTrigger : MonoBehaviour
{
    public StorylineManager storylineManager;
    private bool storylinePlayed = false; // Variable para rastrear si la historia ya se reprodujo

    // Se llama cuando otro collider entra en el trigger
    private void OnTriggerEnter(Collider other)
    {
        // Comprueba si el objeto que entró tiene el tag "Player" (o el que necesites)
        if (!storylinePlayed && other.CompareTag("Player"))
        {
            // Llamar a la función que inicia la historia o cualquier otra lógica
            StartStoryline();
        }
    }

    // Se llama cuando otro collider sale del trigger
    private void OnTriggerExit(Collider other)
    {
        // Puedes agregar lógica de salida si es necesario
    }

    // Función para iniciar la historia
    private void StartStoryline()
    {
        storylineManager.PlayFirstCutscene();
        storylinePlayed = true; // Marcar la historia como reproducida
    }
}
