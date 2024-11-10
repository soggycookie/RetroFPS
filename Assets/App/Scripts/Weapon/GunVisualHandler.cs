using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunVisualHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject visual;
    
    [SerializeField]
    private GunAnimation m_gunAnimation;

    private Gun m_gun;



    private void Awake()
    {
        m_gun = GetComponent<Gun>();
        m_gunAnimation.OnFinshSwap += () => { m_gun.IsGunDisable = false; };    
    }

    public void Disable()
    {
        m_gunAnimation.CancelAll();
        m_gunAnimation.MoveToSwapPos();

        visual.SetActive(false);
    }

    public void Enable(){
        m_gunAnimation.Swap();
        visual.SetActive(true);
    }

    public void PlayShootAnimation(){
        m_gunAnimation.Shoot();
    }

    public void PlayChargeAnimation(){
        m_gunAnimation.Charge();
    }

}
