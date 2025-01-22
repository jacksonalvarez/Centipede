using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WormPhase {
    InitialEncounter,
    MidFightChaos,
    FinalStand,
    Defeated,
    Inactive
}

public class Worm : MonoBehaviour {
    public int Health = 50000;
    public WormPhase CurrentPhase { get; private set; } = WormPhase.Inactive;
    public bool Inactive = true;
    public Player player;
    private void Start() {
        Debug.Log("You feel deeply unsettled");
    }

    private void Update() {
        UpdatePhase();
        PerformAction();
    }

    private void UpdatePhase() {
        if (Health <= 0 && CurrentPhase != WormPhase.Defeated) {
            TransitionToPhase(WormPhase.Defeated);
        } else if (Health < 15000 && CurrentPhase != WormPhase.FinalStand) {
            TransitionToPhase(WormPhase.FinalStand);
        } else if (Health < 30000 && CurrentPhase != WormPhase.MidFightChaos) {
            TransitionToPhase(WormPhase.MidFightChaos);
        } else if (CurrentPhase == WormPhase.Inactive && Inactive == false) {
            TransitionToPhase(WormPhase.InitialEncounter);
        }
    }

    private void TransitionToPhase(WormPhase newPhase) {
        if (CurrentPhase == newPhase) return;
        CurrentPhase = newPhase;

        Debug.Log($"Worm transitioned to phase: {newPhase}");

        switch (newPhase) {
            case WormPhase.Inactive:
                break;
            case WormPhase.InitialEncounter:
                StartInitialEncounter();
                break;
            case WormPhase.MidFightChaos:
                StartMidFightChaos();
                break;
            case WormPhase.FinalStand:
                StartFinalStand();
                break;
            case WormPhase.Defeated:
                StartDefeatSequence();
                break;
        }
    }

    private void PerformAction() {
        // Phase-based behavior in each frame
        switch (CurrentPhase) {
            case WormPhase.Inactive:
                if(player.drinksWater == true){
                    player.StartIntroCutscene();
                    Inactive = false;
                }
            break;
            case WormPhase.InitialEncounter:
                //Charge && Ranged Toxic spores
                if(player.HitType == "Prefers at a range"){
                    Charge();
                    ReleaseToxicSpores();
                    Charge();

                }
                if(player.HitType == "Prefers being Close"){
                    ReleaseToxicSpores();
                    Charge();
                    ReleaseToxicSpores();
                }
                break;

            case WormPhase.MidFightChaos:
                if(player.HitType == "Prefers at a range"){
                    Dig();
                    SummonTornado();
                    Charge();
                    SummonTornado();
                    Charge();
                    SummonTornado();
                    Charge();
                    ReleaseToxicSpores();

                }
                if(player.HitType == "Prefers being Close"){
                    Dig();
                    ReleaseToxicSpores();
                    Charge();
                    SummonTornado();
                    Charge();
                    ReleaseToxicSpores();
                    Charge();
                    SummonTornado();
                }
                break;

            case WormPhase.FinalStand:
                // Actions like Sandstorm or Laser
                ActivateSandstorm();
                FireLaser();
                if(player.HitType == "Prefers at a range"){
                    FireLaser();
                    Dig();
                    ReleaseToxicSpores();
                    Charge();
                    FireLaser();
                    Dig();
                    ReleaseToxicSpores();
                    FireLaser();

                }
                if(player.HitType == "Prefers being Close"){
                    Dig();
                    ReleaseToxicSpores();
                    FireLaser();
                    FireLaser();
                    Charge();
                    ReleaseToxicSpores();
                    Charge();
                    SummonTornado();
                }
                break;

            case WormPhase.Defeated:
                // Play Death Cinematic
                break;
        }
    }

    // Phase-Specific Methods
    private void StartInitialEncounter() {
        Debug.Log("Starting Initial Encounter phase!");
    }

    private void StartMidFightChaos() {
        Debug.Log("Starting Mid-Fight Chaos phase!");
    }

    private void StartFinalStand() {
        Debug.Log("Starting Final Stand phase!");
    }

    private void StartDefeatSequence() {
        Debug.Log("Worm has been defeated!");
        DeathAnimation();
    }

    // Action Methods
    public void Charge() {
        Debug.Log("Worm charges forward!");
        StartCoroutine(WaitAndExecute(5f)); 
    }

    public void ReleaseToxicSpores() {
        Debug.Log("Worm releases toxic spores!");
        StartCoroutine(WaitAndExecute(5f)); 
    }

    public void Dig() {
        Debug.Log("Worm digs underground.");
        StartCoroutine(WaitAndExecute(5f)); 
    }

    public void SummonTornado() {
        Debug.Log("Worm summons a tornado!");
        StartCoroutine(WaitAndExecute(5f)); 
    }

    public void ActivateSandstorm() {
        
        Debug.Log("Worm activates a sandstorm!");
        StartCoroutine(WaitAndExecute(5f)); 
    }

    public void FireLaser() {
        
        Debug.Log("Worm fires a laser beam!");
        StartCoroutine(WaitAndExecute(5f)); 
    }

    public void DeathAnimation() {
        Debug.Log("Worm performs a dramatic death animation!");
    }

    // Example Method to Reduce Health
    public void TakeDamage(int damage) {
        Health -= damage;
        Debug.Log($"Worm took {damage} damage! Remaining health: {Health}");
    }

    private IEnumerator WaitAndExecute(float seconds) {
        Debug.Log("Waiting for " + seconds + " seconds...");
        yield return new WaitForSeconds(seconds); 
        Debug.Log("Time's up! Execute action here.");
    }
}
