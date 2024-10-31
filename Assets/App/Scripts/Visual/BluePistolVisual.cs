using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePistolVisual : GunAnimation
{
    Sequence shakeTween;

    [Header("Shake")]
    [SerializeField]
    private Vector3 m_shakeStrength;

    [SerializeField]
    private Vector3Int m_vibrate;

    [SerializeField]
    private Vector3 m_randomness;

    [Header("Shoot Animation")]
    [SerializeField]
    private Vector3 m_shootPosOffset;

    [SerializeField]
    private Vector3 m_shootDegOffset;

    [SerializeField]
    private float m_durationBeforeToOrigin;

    [SerializeField]
    private float m_pullDuration;

    [SerializeField]
    private float m_goBackDuration;

    //void Shake()
    //{
    //    shakeTween = DOTween.Sequence();

    //    //shakeTween.Append(m_visualTransform.DOShakePosition(0.05f, new Vector3(0.02f, 0.015f, 0.015f), 10, 60, false, true, ShakeRandomnessMode.Full));
    //    shakeTween.Append(m_visualTransform.DOShakePosition(0.05f, new Vector3(m_shakeStrength.x, 0, 0), m_vibrate.x, m_randomness.x, false, true, ShakeRandomnessMode.Full));
    //    shakeTween.Join( m_visualTransform.DOShakePosition(0.05f, new Vector3(0, m_shakeStrength.y, 0), m_vibrate.y, m_randomness.y, false, true, ShakeRandomnessMode.Full));
    //    shakeTween.Join( m_visualTransform.DOShakePosition(0.05f, new Vector3(0, 0, m_shakeStrength.z), m_vibrate.z, m_randomness.z, false, true, ShakeRandomnessMode.Full));
    //    shakeTween.SetLoops(-1, LoopType.Yoyo);
    //}

    //public override void OnShooting()
    //{
    //    m_isCharging = false;
    //    shakeTween.Kill();

    //    //reset pos
    //    m_visualTransform = m_defaultRootTransform;

    //    Sequence seq = DOTween.Sequence();


    //    seq.Append(m_visualTransform.DOLocalMove(m_defaultRootTransform.localPosition + m_shootPosOffset, m_pullDuration).SetEase(Ease.OutExpo));
    //    seq.Join(m_visualTransform.DOLocalRotate(m_defaultRootTransform.localEulerAngles + m_shootDegOffset, m_pullDuration).SetEase(Ease.OutExpo));
    //    seq.AppendInterval(m_durationBeforeToOrigin);
    //    seq.Append(m_visualTransform.DOLocalMove(m_defaultRootTransform.localPosition, m_goBackDuration).SetEase(Ease.OutQuart));
    //    seq.Join( m_visualTransform.DOLocalRotate(m_defaultRootTransform.localPosition, m_goBackDuration).SetEase(Ease.OutQuart));


        
    //}

    //public override void OnCharging()
    //{
    //    Shake();
    //}

}
