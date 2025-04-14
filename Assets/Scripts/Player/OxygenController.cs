using System;
using System.Collections;
using UnityEngine;
using Wolfheat.StartMenu;

public class OxygenController : MonoBehaviour
{

    private const float delay = 0.1f;
    private WaitForSeconds coroutineDelay = new WaitForSeconds(delay);

    private float oxygen = 11;
    private int maxOxygen = 20;
    private bool IsDead { get; set; } = false;

    private const int StartOxygen = 10;
    private const int OxygenUsage = 1;
    private const int OxygenWarningLevel = 10;
    private const int SecondOxygenWarningLevel = 2;
    private const int OxygenRefillSpeed = 10;

    private float noOxygenSurvival = 8f;
    private const float NoOxygenSurvivalMax = 8f;

    // Event handlers
    public Action<float, float> OxygenUpdated;
    public Action<float, int> HealthUpdated;

    public Action PlayerDied;

    private void Start()
    {
        Debug.Log("Oxygen Coroutine started.");
        StartCoroutine(UseOxygen());
    }

    // Oxygen Usage
    private IEnumerator UseOxygen()
    {
        // This routine always runs
        while (true) {
            //Debug.Log("Oxygen Coroutine runs.");
            // This routines effect is "paused" when dead
            while (IsDead) {
                Debug.Log("Player is dead.");
                yield return coroutineDelay;
            }
            yield return coroutineDelay;
            float startOxygen = oxygen;

            if (IsPlayerUsingOxygen()) { // Check if player are outside refill area
                // Only use oxygen when in vaccuum = outside
                if (oxygen > 0) {
                    bool aboveWarningBeforeDecrease = oxygen >= OxygenWarningLevel;
                    bool aboveSecondWarningBeforeDecrease = oxygen >= SecondOxygenWarningLevel;
                    oxygen -= OxygenUsage * delay;
                    Debug.Log("Oxygen is now "+oxygen);

                    // Give warning of low oxygen
                    //
                    //if (aboveWarningBeforeDecrease && oxygen < OxygenWarningLevel)
                    //    HUDMessage.Instance.ShowMessage("Oxygen!", sound: SoundName.LowOxygen);
                    //else if (aboveSecondWarningBeforeDecrease && oxygen < SecondOxygenWarningLevel)
                    //    HUDMessage.Instance.ShowMessage("Quickly Now!", sound: SoundName.LowOxygen);
                }
                else {
                    // All oxygen is used, player starts to drown
                    // No oxygen Survival stat is monitoring how long the player survives without oxygen, separate timer
                    if (noOxygenSurvival == NoOxygenSurvivalMax) {
                        // Starting to drown
                        //SoundMaster.Instance.PlaySound(SoundName.Drowning);
                        SoundMaster.Instance.FadeMusic();
                    }
                    if (noOxygenSurvival > 0)
                        noOxygenSurvival -= delay;
                    else {
                        // Kill player if not allready dead
                        if (!IsDead) {
                            Debug.Log("PlayerDIED");
                            IsDead = true;
                            PlayerDied?.Invoke();
                            //uiController.ShowDeathScreen();
                        }
                    }
                }
            }
            else {
                //Debug.Log("Player are not using oxygen.");
                // Only regain oxygen when inside
                if (noOxygenSurvival < NoOxygenSurvivalMax) {
                    // Stops drowning clip from playing
                    if (oxygen <= 0) {
                        Debug.Log("Stop drowning sound");
                        //SoundMaster.Instance.StopSound(SoundName.Drowning);
                    }
                    // Refills the drowning timer at same rate as the oxygen (not visible to player)
                    noOxygenSurvival = Math.Min(NoOxygenSurvivalMax, noOxygenSurvival + OxygenRefillSpeed * delay);
                }
                // Refills oxygen tank 
                if (oxygen < maxOxygen)
                    oxygen += OxygenRefillSpeed * delay;
                if (oxygen > maxOxygen)
                    oxygen = maxOxygen;

            }

            // Set distortioneffect and darkening from noOxygenSurvival value
            //
            //uiController.UpdateScreenDarkening(1 - noOxygenSurvival / NoOxygenSurvivalMax);
            //uiController.SetOxygen(oxygen, maxOxygen);

            // Check if the current oxygen is the same as we started with i.e no uptdates event dispatch needed
            if (oxygen != startOxygen)
                OxygenUpdated?.Invoke(oxygen, maxOxygen);
        }
    }

    private bool IsPlayerUsingOxygen()
    {
        // Check if player are inside refill area
        return !PlayerColliderController.IsPlayerInRegainArea;
    }
}
