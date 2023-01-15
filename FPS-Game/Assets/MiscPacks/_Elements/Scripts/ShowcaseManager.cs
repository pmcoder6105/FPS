using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class ShowcaseManager : MonoBehaviour
{
    public GameObject[] allToShowcase;
    public string[] qualitySettings;
    public TextMeshProUGUI qualityText;
    public int currSetting;
    public GameObject selectedType;
    public GameObject selectedEffect;
    public TextMeshProUGUI effectName, modeName;
    public int currEffectSelected, currTypeSelected;
    public CinemachineVirtualCamera vCam;
    public bool showCaseMode;
    public GameObject projectileStand;
    public Shooter shooter;
    public Transform[] viewSpots;
    public Transform[] projectileViewSpots;
    public int currViewSpot, currProjectileViewSpot;
    // Start is called before the first frame update
    void Start()
    {
        modeName.text = "Mode: " + "VFX Showcase";
        qualityText.text = "Graphics: " + "\n" + qualitySettings[currSetting];
        ActivateType();
        ActivateEffect();
    }

    public void ToggleQuality()
    {
        currSetting = GenericScroll(currSetting, qualitySettings.Length-1, true);
        QualitySettings.SetQualityLevel(currSetting);
        qualityText.text = "Graphics: " +"\n"+ qualitySettings[currSetting];
    }

    public void NextType(bool isForward)
    {
        currEffectSelected = 0;
        currTypeSelected = GenericScroll(currTypeSelected, allToShowcase.Length - 1, isForward);
        ActivateType();
        ActivateEffect();
    }

    public void NextEffect(bool isForward)
    {
        currEffectSelected = GenericScroll(currEffectSelected, selectedType.transform.childCount - 1, isForward);
        ActivateEffect();

    }

    public void ActivateEffect()
    {
        selectedType = allToShowcase[currTypeSelected];
        int myNum = 0;
        foreach (Transform t in selectedType.transform)
        {

            if (currEffectSelected == myNum)
            {
                selectedEffect = t.gameObject;
                selectedEffect.SetActive(true);
                effectName.text = "Name: " + selectedEffect.name + "\n" + "Type: " + allToShowcase[currTypeSelected].name.Replace("=","");
                vCam.m_LookAt = selectedEffect.transform;
            }
            else
            {
                t.gameObject.SetActive(false);
            }
            myNum++;
        }
    }

    public void ActivateType()
    {
        foreach (GameObject g in allToShowcase)
        {
            g.SetActive(false);
        }
        allToShowcase[currTypeSelected].SetActive(true);
    }

    public int GenericScroll(int currNum, int maxNum, bool isForward)
    {
        if (!isForward)
        {
            if (currNum > 0)
                currNum--;
            else
                currNum = maxNum;
        }
        else
        {
            if (currNum < maxNum)
                currNum++;
            else
                currNum = 0;
        }
        return currNum;
    }

    public void PlayEffect()
    {
        if (selectedEffect != null)
        {
            if (selectedEffect.GetComponent<ParticleSystem>() != null)
            {
                selectedEffect.GetComponent<ParticleSystem>().Stop();
                selectedEffect.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    public void ChangeViewSpot(bool goForward)
    {
        if (showCaseMode)
        {
            currViewSpot = GenericScroll(currViewSpot, viewSpots.Length - 1, goForward);
            vCam.m_Follow = viewSpots[currViewSpot];
        }
        else
        {
            currProjectileViewSpot = GenericScroll(currProjectileViewSpot, projectileViewSpots.Length - 1, goForward);
            vCam.m_Follow = projectileViewSpots[currProjectileViewSpot];
        }
    }

    public void ShowcaseControls()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            NextType(true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            NextType(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            NextEffect(true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            NextEffect(false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayEffect();
        }
    }

    public void ProjectileControls()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            shooter.SwitchProjectile(true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            shooter.SwitchProjectile(false);
        }
        effectName.text = "Name: " + shooter.vfxToShoot[shooter.selectedVFX].name + "\n" + "Type: " + projectileStand.name.Replace("=", "");
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleShowcase();
        }
        if(showCaseMode)
        {
            ShowcaseControls();
        }
        else
        {
            ProjectileControls();
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeViewSpot(false);
        }
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeViewSpot(true);
        }
    }

    public void ToggleShowcase()
    {
        showCaseMode = !showCaseMode;
        if(showCaseMode)
        {
            projectileStand.SetActive(false);
            modeName.text = "Mode: " + "VFX Showcase";
            shooter.StopAllCoroutines();
            shooter.ResetInfo();
            ActivateType();
            ActivateEffect();
        }
        else
        {
            modeName.text = "Mode: " + "Projectile Showcase";
            foreach (GameObject g in allToShowcase)
            {
                g.SetActive(false);
            }
            selectedEffect = null;
            selectedType = null;
            projectileStand.SetActive(true);
            vCam.m_LookAt = shooter.viewpoint.transform;
            vCam.m_Follow = projectileViewSpots[currProjectileViewSpot];
        }
    }
}
