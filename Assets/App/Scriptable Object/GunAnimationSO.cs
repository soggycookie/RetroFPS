using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

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

    [Header("Swap Animation")]
    public float swapDuration;
    public RotateMode rotateMode;


#if UNITY_EDITOR

    private bool       m_isInit = false;

#endif

    private Transform m_animationTransform;
    private Transform m_swapTransform;
    private float m_totalDuration;

    private Vector3 m_position;
    private Vector3 m_rotation;

    private int m_lap;
    private Vector3 m_axis;

    public void Initialize(Transform animTransform, Vector3 defaultPos, Vector3 defaultRot, Transform swap, float totalDuration, int lap, Vector3 axis){
        m_animationTransform = animTransform;
        m_totalDuration = totalDuration;
        m_position = defaultPos;
        m_rotation = defaultRot;
        m_swapTransform = swap;
        m_lap = lap;
        m_axis = axis;

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



        seq.SetRelative(true);
        
        seq.Append(m_animationTransform.DOBlendableLocalMoveBy(recoilPosOffset, pullRecoilDuration));

        seq.SetRelative(false);



        seq.Join(m_animationTransform.DOLocalRotate(m_rotation + recoilDegOffset, pullRecoilDuration).SetEase(Ease.OutExpo));

        seq.AppendInterval(durationBeforeGoToOrigin);

        seq.Append(m_animationTransform.DOLocalMove(m_position, goBackToOriginDuration).SetEase(Ease.OutQuart));
        seq.Join(m_animationTransform.DOLocalRotate(m_rotation, goBackToOriginDuration).SetEase(Ease.OutQuart));
    }


    public Sequence AnimateSwap(){
        Sequence seq = DOTween.Sequence();


        seq.Append(m_animationTransform.DOLocalMove(m_position, swapDuration));
        seq.Join(m_animationTransform.DOLocalRotate(m_rotation + m_axis.normalized *  (-360 * m_lap), swapDuration, rotateMode));
    
        return seq;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if(!m_isInit) return;

        float duration = durationBeforeGoToOrigin + pullRecoilDuration + goBackToOriginDuration;


        if(duration > m_totalDuration){
            Debug.LogError("Combination of all durations is greater than total gun duration");
        }
    }
    #endif
}
