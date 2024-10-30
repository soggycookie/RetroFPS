using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    [SerializeField]
    private PlayerMovementAbility              m_playerMovement;
                                               
    [SerializeField]                           
    private CinemachineCamera                  m_POVCam;

    [SerializeField]
    private CinemachineBasicMultiChannelPerlin m_cameraNoise;

    [SerializeField]
    private CinemachinePanTilt                 m_panTilt;




    [Space(10)]
    [SerializeField]
    private float m_slamShakeTime = 1f;

    [SerializeField]
    private Vector3 m_shakeStrength;

    [SerializeField]
    private float m_amplitude;

    [SerializeField]
    private float m_frequency;


    [Header("FOV config")]
    [SerializeField]
    private float m_lerpTimeFOV   = 1f;

    [SerializeField]
    private float m_maxValueFOV   = 120f;

    [SerializeField]
    private AnimationCurve m_FOVLerpCurve;

    private float m_FOV;
    private float m_defaultFOV;

    private float m_currentAmplitude;

    private Coroutine m_increaseFOV;

    private void Awake()
    {
        ResetAmplitude();


        m_FOV        = m_POVCam.Lens.FieldOfView;
        m_defaultFOV = m_FOV;


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

    public void CameraShake()
    {
        m_cameraNoise.PivotOffset = m_shakeStrength;
        m_cameraNoise.AmplitudeGain = m_amplitude;
        m_cameraNoise.FrequencyGain = m_frequency;

        DOVirtual.Float(m_amplitude, 0f, m_slamShakeTime, v =>{
            m_cameraNoise.AmplitudeGain = v;
        });

        //StartCoroutine(StopCameraShaking());
    }

    IEnumerator StopCameraShaking() {
        yield return new WaitForSeconds(m_slamShakeTime);
        ResetAmplitude() ;
    }

    void IncreaseFOV(){
        m_increaseFOV = StartCoroutine(IncreaseOverTime());
    }

    void DecreaseFOV(){
        float lerpTime = (m_POVCam.Lens.FieldOfView / m_maxValueFOV) * m_lerpTimeFOV;

        if(m_increaseFOV !=null){
            StopCoroutine(m_increaseFOV);
        }
            
        StartCoroutine(DecreaseOverTime(lerpTime, m_POVCam.Lens.FieldOfView));
    }

    IEnumerator IncreaseOverTime(){
        float time = 0;

        while(true){
            time+= Time.deltaTime;

            m_FOV = Mathf.Lerp(m_defaultFOV, m_maxValueFOV,  m_FOVLerpCurve.Evaluate(time / m_lerpTimeFOV));
            SetFOV(m_FOV);

            if(time >= m_lerpTimeFOV){
                break;
            }

            yield return null;
        }
    }

    IEnumerator DecreaseOverTime(float lerpTime, float FOV){
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;

            m_FOV = Mathf.Lerp(FOV, m_defaultFOV, m_FOVLerpCurve.Evaluate(time / lerpTime));
            SetFOV(m_FOV);

            if (time >= lerpTime)
            {
                break;
            }

            yield return null;
        }
    }

    void SetFOV(float fieldOfView){
        m_POVCam.Lens.FieldOfView = fieldOfView;
    }

    void ResetAmplitude()
    {
        m_cameraNoise.AmplitudeGain = 0f;
    }

    private void OnDisable()
    {
        m_playerMovement.OnSlam -= CameraShake;
        //m_playerMovement.OnDash -=IncreaseFOV;
        //m_playerMovement.OnExitDash -=DecreaseFOV;
    }
}
