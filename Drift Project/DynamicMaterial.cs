using UnityEngine;

public class DynamicMaterial : MonoBehaviour
{
    public GameObject[] targetObjects; // Los GameObjects a los que se les cambiará el material
    public Material originalMaterial; // Material original
    public Material brakeMaterial; // Material para aplicar cuando se está frenando

    private MeshRenderer[] meshRenderers;

    void Start()
    {
        // Asegúrate de tener MeshRenderers en los GameObjects
        meshRenderers = new MeshRenderer[targetObjects.Length];
        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (targetObjects[i] == null || targetObjects[i].GetComponent<MeshRenderer>() == null)
            {
                Debug.LogError($"Se requiere un GameObject con MeshRenderer para cambiar el material. GameObject #{i} es nulo o no tiene MeshRenderer.");
                enabled = false;
                return;
            }
            meshRenderers[i] = targetObjects[i].GetComponent<MeshRenderer>();
            meshRenderers[i].material = originalMaterial; // Establece el material original al inicio
        }
    }

    void Update()
    {
        // Verifica si se está frenando
        bool isBraking = Input.GetAxis("BrakePS4") > 0;

        // Cambia el material basándose en si se está frenando o no para cada objeto
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = isBraking ? brakeMaterial : originalMaterial;
        }
    }
}
