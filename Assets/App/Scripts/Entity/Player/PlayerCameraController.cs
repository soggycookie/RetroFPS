using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField]
    private PlayerMovementAbility              m_playerMovement;
                                  
    [SerializeField]
    private GunAnimation[]                     m_gunAnimations;

    [SerializeField]                           
    private CinemachineCamera                  m_POVCam;

    [SerializeField]
    private CinemachineBasicMultiChannelPerlin m_cameraNoise;

    [SerializeField]
    private CinemachinePanTilt                 m_panTilt;



    private float m_currentAmplitude;
    private bool  m_isShaking;

    private void Awake()
    {
        m_gunAnimations = GetComponentsInChildren<GunAnimation>();

        foreach(GunAnimation anim in m_gunAnimations){
            anim.OnShooting += CameraShake;
        }

        m_isShaking = false;

        m_playerMovement.OnSlam     += CameraShake;
        //m_playerMovement.OnDash     += IncreaseFOV;
        //m_playerMovement.OnExitDash += DecreaseFOV;
         

        m_panTilt.PanAxis.Value = m_playerMovement.transform.eulerAngles.y;
    }

    public Vector3 GetCameraYRotation()
    {
        Vector3 rotation = new Vector3(0, transform.eulerAngles.y, 0);

        return rotation;
    }

    public void CameraShake(Vector3 pivotOffset, float amplitude, float frequency, float duration)
    {
        if(m_isShaking) return;

        m_isShaking = true;
        m_cameraNoise.PivotOffset = pivotOffset;
        m_cameraNoise.AmplitudeGain = amplitude;
        m_cameraNoise.FrequencyGain = frequency;

        DOVirtual.Float(amplitude, 0f, duration, v =>{
            m_cameraNoise.AmplitudeGain = v;
        })
            .OnComplete(() =>{
                m_isShaking = false;
            });

    }

    private void OnDisable()
    {
        m_playerMovement.OnSlam -= CameraShake;

        foreach (GunAnimation anim in m_gunAnimations)
        {
            anim.OnShooting -= CameraShake;
        }

    }
}
