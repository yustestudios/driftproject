using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Asegúrate de importar el espacio de nombres de UI
using EVP;

public class StorylineManager : MonoBehaviour
{
    public EnterRaceTrack enterRaceTrack;
    public GameObject camera;
    public GameObject player;
    public VehicleController vehicleController;
    public Animator animatorFade;
    public Text actorText; // Asegúrate de asignar esto en el inspector
    public Text dialogueText; // Asegúrate de asignar esto en el inspector
    public Text infoText; // Asegúrate de asignar esto en el inspector

    private WaitForSeconds dialogueDelay = new WaitForSeconds(2.0f);
    private WaitForSeconds dialogueDelayFaster = new WaitForSeconds(0.5f);
    private WaitForSeconds letterDelay = new WaitForSeconds(0.05f); // Ajusta según tu preferencia
    private string[] dialogues;
    private string[] dialogues_0;
    private bool storylineThrottle = false;

    // Start is called before the first frame update
    void Start()
    {
        dialogues = new string[]
        {
            "This is going to be my last race, now that I have a kid I don't want to risk my life anymore...",
            "I dedicated all my life to illegal racing and car culture and now that I am starting to have more responsibilities it's time to stop...",
            "I don't know why but this Nexus RS is not behaving properly",
            "I told the car mechanic to change the brakes after this lap, this is dangerous"
        };

        dialogues_0 = new string[]
        {
            "Ey, BRAKES ARE NOT WORKING!!",
            "NO, NOOO, NOoOooOoOoooOOO"
        };

        StartCoroutine(StartRaceWithDelay());
    }

    private IEnumerator StartRaceWithDelay()
    {
        yield return new WaitForSeconds(1.0f);

        enterRaceTrack.StartRaceFromExternal();
        camera.GetComponent<VehicleCameraController>().enabled = false;
        camera.transform.position = new Vector3(0, 0, 0);
        camera.GetComponent<VehicleCameraController>().enabled = true;
        animatorFade.enabled = true;

        yield return dialogueDelay;

        foreach (string dialogue in dialogues)
        {
            yield return StartCoroutine(ShowDialogue(dialogue));
            yield return dialogueDelay;
            dialogueText.text = "";
            actorText.text = "";
            yield return dialogueDelay;
        }
    }

    private IEnumerator ShowDialogue(string dialogue)
    {
        dialogueText.text = "";
        actorText.text = "Paul";
        
        foreach (char letter in dialogue)
        {
            dialogueText.text += letter;
            yield return letterDelay;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Add any additional logic you need in the update
        if(storylineThrottle)
        {
            vehicleController.steerInput = 0f;
            vehicleController.throttleInput = 1f;
            vehicleController.brakeInput = 0f;
            vehicleController.handbrakeInput = 0f;
        }
    }

    public void PlayFirstCutscene()
    {
        StartCoroutine(PlayFirstCutsceneScene());
    }

    private IEnumerator PlayFirstCutsceneScene()
    {
        animatorFade.SetBool("fade", !animatorFade.GetBool("fade"));
        player.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
        yield return new WaitForSeconds(1.5f);
        vehicleController.enabled = false;
        camera.GetComponent<VehicleCameraController>().enabled = false;
        camera.transform.position = new Vector3(662.7f, 3.27f, 243.4f);
        camera.GetComponent<VehicleCameraController>().mode = VehicleCameraController.Mode.LookAt;
        camera.GetComponent<VehicleCameraController>().enabled = true;

        player.GetComponent<PlayerInputCar>().enabled = false;
        player.transform.position = new Vector3(618.46f, 0.2f, 174.36f);
        player.transform.rotation = Quaternion.Euler(0f, 21f, 0f);
        
        vehicleController.enabled = true;

        storylineThrottle = true;
        animatorFade.SetBool("fade", !animatorFade.GetBool("fade"));
        
        yield return new WaitForSeconds(0.5f);

        foreach (string dialogue in dialogues_0)
        {
            yield return StartCoroutine(ShowDialogue(dialogue));
            yield return dialogueDelayFaster;
            dialogueText.text = "";
            actorText.text = "";
            yield return dialogueDelayFaster;
        }

        animatorFade.SetBool("fade", !animatorFade.GetBool("fade"));
        yield return new WaitForSeconds(0.5f);
        
        StartCoroutine(ChangeUITextWithAnimation("One week later..."));

        enterRaceTrack.cancelRace = true;

        yield return new WaitForSeconds(2.5f);

        enterRaceTrack.cancelRace = false;
        player.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
        camera.GetComponent<VehicleCameraController>().enabled = true;
        camera.GetComponent<VehicleCameraController>().mode = VehicleCameraController.Mode.SmoothFollow;
        player.GetComponent<PlayerInputCar>().enabled = true;
        storylineThrottle = false;

        StartCoroutine(ChangeUITextWithAnimation(""));
        
        animatorFade.SetBool("fade", !animatorFade.GetBool("fade"));
    }

    private IEnumerator ChangeUITextWithAnimation(string newText)
    {
        // Asegúrate de configurar la animación en tu Text de UI (añadir una propiedad "fade", por ejemplo)
        infoText.CrossFadeAlpha(0f, 0.5f, false); // Desvanecer el texto actual
        yield return new WaitForSeconds(0.5f); // Esperar a que se desvanezca

        // Cambiar el texto y restaurar la opacidad
        infoText.text = newText;
        infoText.CrossFadeAlpha(1f, 0.5f, false); // Desvanecer el nuevo texto en
    }
}