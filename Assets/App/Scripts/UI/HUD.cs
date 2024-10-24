using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;



public class HUD : MonoBehaviour
{
    public TextMeshProUGUI horizontalSpeedometer;
    public TextMeshProUGUI verticalSpeedometer;
    public TextMeshProUGUI state;
    PlayerMovementAbility playerMovement;

    

    private void Awake()
    {
        playerMovement= FindAnyObjectByType<PlayerMovementAbility>();
        playerMovement.OnSpeedUpdate += UpdateSpeedText;
        playerMovement.OnStateUpdate += UpdateState;
    }

    void UpdateSpeedText(float vertical, float horizontal)
    {
        verticalSpeedometer.text = "Vertical:   " + System.Math.Round(vertical, 2);
        horizontalSpeedometer.text = "Horizontal: " + System.Math.Round(horizontal, 2);
    }

    void UpdateState(PlayerMovementAbility.MovementState mState){
        state.text = mState.ToString();
    }

    private void OnDisable()
    {
        playerMovement.OnSpeedUpdate -= UpdateSpeedText;
        playerMovement.OnStateUpdate -= UpdateState;
    }
}
