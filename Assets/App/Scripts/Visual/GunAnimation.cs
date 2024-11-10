using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class GunAnimation : MonoBehaviour
{
    [SerializeField]
    private Transform m_visualTransform;

    [SerializeField]
    private Transform m_swapTransform;

    [SerializeField]
    private bool      m_isFullRotation;

    [SerializeField]
    private Vector3   m_fullRotationAxis;

    [SerializeField]
    private int m_lap;

    [SerializeField]
    GunAnimationSO    m_animationData;

    [SerializeField]
    private float duration;


    [Header("Charging Animation")]
    public Vector3 shakeStrength;

    public Vector3Int vibrate;

    public Vector3 randomness;

    [Header("Shoot Animation")]
    public Vector3 recoilPosOffset;
    public Ease recoilPosEase;
    public float   recoilPosDurationOffset;

    public Vector3 recoilDegOffset;
    public Ease recoilDegEase;

    public float durationBeforeGoToOrigin;

    public float pullRecoilDuration;

    public float goBackToOriginDuration;

    public Ease posBackToOriginEase;
    public Ease rotationBackToOriginEase;

    [Header("Camera Shake")]
    public float shakeDuration;
    public Vector3 pivotOffset;
    public float amplitude;
    public float frequency;

    [Header("Swap Animation")]
    public float swapDuration;
    public RotateMode rotateMode;


    public float TotalDuration { get; set;}

    public UnityAction<Vector3, float, float, float> OnShooting;
    public UnityAction OnFinshSwap;

    private Vector3 m_defaultPos;
    private Vector3 m_defaultRot;

    Sequence shakeTween;
    Sequence swapTween;
    Sequence shootTween;

    private void Awake()
    {
        m_defaultPos = m_visualTransform.localPosition;
        m_defaultRot = m_visualTransform.localEulerAngles;

        //m_animationData.Initialize(m_visualTransform, m_defaultPos, m_defaultRot, m_swapTransform, duration, isFullRotation ? lap : 0,  fullRotationAxis);
    }

    public void Shoot(){
        if(shakeTween != null)
            shakeTween.Kill();

        //reset pos
        m_visualTransform.localEulerAngles = m_defaultRot;
        m_visualTransform.localPosition = m_defaultPos;

        shootTween = AnimateShooting();

        OnShooting?.Invoke(m_animationData.pivotOffset, m_animationData.amplitude, m_animationData.frequency, m_animationData.shakeDuration);
    }

    public void Charge(){
        shakeTween = AnimateCharging();
    }

    public void Swap(){
        swapTween = AnimateSwap();
    }

    public void MoveToSwapPos()
    {
        m_visualTransform.position = m_swapTransform.position;
        m_visualTransform.rotation = m_swapTransform.rotation;
    }

    public Sequence AnimateCharging()
    {
        Sequence shakeTween = DOTween.Sequence();

        //shakeTween.Append(AnimationTransform.DOShakePosition(0.05f, new Vector3(0.02f, 0.015f, 0.015f), 10, 60, false, true, ShakeRandomnessMode.Full));
        shakeTween.Append(m_visualTransform.DOShakePosition(0.05f, new Vector3(shakeStrength.x, 0, 0), vibrate.x, randomness.x, false, true, ShakeRandomnessMode.Full));
        shakeTween.Join(m_visualTransform.DOShakePosition(0.05f, new Vector3(0, shakeStrength.y, 0), vibrate.y, randomness.y, false, true, ShakeRandomnessMode.Full));
        shakeTween.Join(m_visualTransform.DOShakePosition(0.05f, new Vector3(0, 0, shakeStrength.z), vibrate.z, randomness.z, false, true, ShakeRandomnessMode.Full));
        shakeTween.SetLoops(-1, LoopType.Yoyo);

        return shakeTween;
    }

    public Sequence AnimateShooting()
    {
        Sequence seq = DOTween.Sequence();


        seq.Append(m_visualTransform.DOLocalRotate(m_defaultRot + recoilDegOffset, pullRecoilDuration).SetEase(Ease.OutExpo));

        seq.SetRelative(true);

        seq.Insert(recoilPosDurationOffset, m_visualTransform.DOBlendableLocalMoveBy(recoilPosOffset, pullRecoilDuration));

        seq.SetRelative(false);




        seq.AppendInterval(durationBeforeGoToOrigin);

        seq.Append(m_visualTransform.DOLocalMove(m_defaultPos, goBackToOriginDuration).SetEase(Ease.OutQuart));
        seq.Join(m_visualTransform.DOLocalRotate(m_defaultRot, goBackToOriginDuration).SetEase(Ease.OutQuart));

        return seq;
    }


    public Sequence AnimateSwap()
    {
        Sequence seq = DOTween.Sequence();


        seq.Append(m_visualTransform.DOLocalMove(m_defaultPos, swapDuration));
        seq.Join(m_visualTransform.DOLocalRotate(m_defaultRot + m_fullRotationAxis.normalized * (-360 * m_lap), swapDuration, rotateMode));
        seq.OnComplete( () => { OnFinshSwap?.Invoke();});
        return seq;
    }

    public void CancelAll(){
        shakeTween.Kill();
        shootTween.Kill();
        swapTween.Kill();

    }
}
