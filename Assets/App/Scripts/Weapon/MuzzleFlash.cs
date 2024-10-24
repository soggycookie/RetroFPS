using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//unused yet
public class MuzzleFlash : MonoBehaviour
{
    public MuzzleFlashSO muzzleFlashSO;
    MeshRenderer muzzleFlashRenderer;

    private float m_countTime;

    public void FadeOutFlash(){
        transform.localScale = (muzzleFlashSO.scale - m_countTime/muzzleFlashSO.fadeOutTime * muzzleFlashSO.scale) * Vector3.one;
    }

    private void Update()
    {
        FadeOutFlash();
        m_countTime += Time.deltaTime;
        if(m_countTime >= muzzleFlashSO.fadeOutTime){
            m_countTime = 0;
            gameObject.SetActive(false);
        }

    }

    private void Awake()
    {

        gameObject.SetActive(false);
    }

    private void ResetValue(){
        transform.localScale = Vector3.one * muzzleFlashSO.scale;
    }

    public void Activate(){
        gameObject.SetActive(true);
    }
}
