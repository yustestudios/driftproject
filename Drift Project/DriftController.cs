using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using LeanTween;

public class DriftController : MonoBehaviour
{
    public Rigidbody rb;
    public WheelSkid[] wheelSkids;
    public Text driftText;
    public float POINTS_PER_SECOND = 100.0f;
    public float MIN_SPEED_FOR_DRIFT = 40.0f;
    public float INTENSITY_THRESHOLD = 0.5f;
    private float driftPoints;
    private bool isSkidding;
    private bool addedPoints = false;
    private float driftStartTime;
    const float MAX_SKID_INTENSITY = 2.0f;
    public EnterRaceTrack enterRacingTrack;

    private void FixedUpdate()
    {
        float carSpeed = rb.velocity.magnitude * 3.6f;

        if (carSpeed >= MIN_SPEED_FOR_DRIFT)
        {
            float skidTotal = 0f;
            bool isSkiddingAtLeastOne = false;

            foreach (var wheelSkid in wheelSkids)
            {
                skidTotal += wheelSkid.GetSkidIntensity();

                if (wheelSkid.GetSkidIntensity() >= INTENSITY_THRESHOLD)
                {
                    isSkiddingAtLeastOne = true;
                }
            }

            if (isSkiddingAtLeastOne && skidTotal >= MAX_SKID_INTENSITY)
            {
                if (!isSkidding)
                {
                    StartDrift();
                }

                UpdateDriftPoints();
            }
            else
            {
                EndDrift();
            }
        }
        else
        {
            EndDrift();
        }
    }

    private void StartDrift()
    {
        driftStartTime = Time.time;
        isSkidding = true;
        addedPoints = false;
    }

    private void UpdateDriftPoints()
    {
        float elapsedTime = Time.time - driftStartTime;
        driftPoints = elapsedTime * POINTS_PER_SECOND;

        driftText.text = Mathf.Floor(driftPoints).ToString();
    }

    private void EndDrift()
    {
        isSkidding = false;

        if (!string.IsNullOrEmpty(driftText.text))
        {
            if (enterRacingTrack.raceInProgress && !addedPoints)
            {
                addedPoints = true;
                enterRacingTrack.AddDriftPoints(Mathf.Floor(driftPoints));
            }

            // Agregar animación de escalado al texto
            AnimateDriftTextScale();
        }
    }

    private void AnimateDriftTextScale()
    {
        LeanTween.scale(driftText.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.2f)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() =>
            {
                // Restablecer el escalado original
                LeanTween.scale(driftText.gameObject, new Vector3(1.0f, 1.0f, 1.0f), 0.2f);
                driftText.text = "";
            });
    }
}
