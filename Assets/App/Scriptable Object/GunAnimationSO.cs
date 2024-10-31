using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Weapon/Gun Animation")]
public class GunAnimationSO : ScriptableObject
{


    [Header("Charging Animation")]
    public Vector3    shakeStrength;

    public Vector3Int vibrate;

    public Vector3    randomness;

    [Header("Shoot Animation")]
    public Vector3    recoilPosOffset;
    public Ease       recoilPosEase;

    public Vector3    recoilDegOffset;
    public Ease       recoilDegEase;

    public float      durationBeforeGoToOrigin;
                      
    public float      pullRecoilDuration;
                      
    public float      goBackToOriginDuration;

    public Ease       posBackToOriginEase;
    public Ease       rotationBackToOriginEase;

    [Header("Camera Shake")]
    public float      shakeDuration;
    public Vector3    pivotOffset;
    public float      amplitude;
    public float      frequency;

    private Transform  m_animationTransform;
    private Transform  m_originTransform;
    private float      m_totalDuration;



    #if UNITY_EDITOR
    
    private bool       m_isInit = false;
    
    #endif



    public void Initialize(Transform animTransform, Transform origin, float totalDuration){
        m_animationTransform = animTransform;
        m_totalDuration = totalDuration;
        m_originTransform = origin;
        m_isInit =true;
    }

    public Sequence AnimateCharging()
    {
        Sequence shakeTween = DOTween.Sequence();

        //shakeTween.Append(AnimationTransform.DOShakePosition(0.05f, new Vector3(0.02f, 0.015f, 0.015f), 10, 60, false, true, ShakeRandomnessMode.Full));
        shakeTween.Append(m_animationTransform.DOShakePosition(0.05f, new Vector3(shakeStrength.x, 0, 0), vibrate.x, randomness.x, false, true, ShakeRandomnessMode.Full));
        shakeTween.Join(m_animationTransform.DOShakePosition(0.05f, new Vector3(0, shakeStrength.y, 0), vibrate.y, randomness.y, false, true, ShakeRandomnessMode.Full));
        shakeTween.Join(m_animationTransform.DOShakePosition(0.05f, new Vector3(0, 0, shakeStrength.z), vibrate.z, randomness.z, false, true, ShakeRandomnessMode.Full));
        shakeTween.SetLoops(-1, LoopType.Yoyo);

        return shakeTween;
    }

    public void AnimateShooting(){
        Sequence seq = DOTween.Sequence();

        //m_animationTransform.DOShakePosition(pullRecoilDuration, shootShakeStrength, shootVibrate, shootRandomness, false, true, ShakeRandomnessMode.Harmonic);

        //seq.Append(m_animationTransform.DOLocalMove(m_originTransform.localPosition + recoilPosOffset, pullRecoilDuration).SetEase(Ease.OutExpo));

        seq.SetRelative(true);
        
        seq.Append(m_animationTransform.DOBlendableLocalMoveBy(recoilPosOffset, pullRecoilDuration));

        seq.SetRelative(false);



        seq.Join(m_animationTransform.DOLocalRotate(m_originTransform.localEulerAngles + recoilDegOffset, pullRecoilDuration).SetEase(Ease.OutExpo));

        seq.AppendInterval(durationBeforeGoToOrigin);

        seq.Append(m_animationTransform.DOLocalMove(m_originTransform.localPosition, goBackToOriginDuration).SetEase(Ease.OutQuart));
        seq.Join(m_animationTransform.DOLocalRotate(m_originTransform.localPosition, goBackToOriginDuration).SetEase(Ease.OutQuart));

        //        float duration = durationBeforeGoToOrigin + pullRecoilDuration + goBackToOriginDuration;

        //        seq.Append(m_animationTransform.DOPunchPosition(recoilPosOffset, duration, shootVibrate, 2));
        //        seq.Join(m_animationTransform.DOPunchRotation(recoilDegOffset, duration, shootVibrate, 2));
    }

    public void Cancel(){
        m_animationTransform = m_originTransform;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if(!m_isInit) return;

        float duration = durationBeforeGoToOrigin + pullRecoilDuration + goBackToOriginDuration;

        if(duration > m_totalDuration){
            EditorApplication.isPlaying = false;
            Debug.LogWarning("Combination of all durations is greater than total gun duration");
        }
    }
    #endif
}
