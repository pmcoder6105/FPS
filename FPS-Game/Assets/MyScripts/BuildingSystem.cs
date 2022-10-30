using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class BuildingSystem : MonoBehaviourPunCallbacks
{
    public Transform blockShootingPoint;
    public GameObject blockPrefab;

    public Material highlightedColour;

    bool canBuild = true;
    bool canDestroy = true;

    [HideInInspector] public bool isInBuildMode = false;

    public PhotonView PV;

    public Material lit;

    public Texture proBuilderTexture;

    GameObject blockInstantiated;

    int blockID;

    public AudioClip placeBlock;

    public AudioClip destroyBlock;

    public GameObject blockDestructionVFX;

    public bool isDestroyed = false;

    public GameObject blockCrosshair;

    public GameObject handHeldBlock;

    public GameObject blockIndictorUIImage;

    public PlayerController controller;

    void Update()
    {
        if (!PV.IsMine || controller.isDead)
            return;        

        if (Input.GetKeyDown(KeyCode.B))
        {
            isInBuildMode = !isInBuildMode;
        }

        if (isInBuildMode && controller.isDead == false)
        {
            if (PV.IsMine)
            {
                if (Input.GetMouseButton(0) && canBuild)
                {
                    StartCoroutine(nameof(BuildBlockAndWait));
                }
                if (Input.GetMouseButton(1))
                {
                    StartCoroutine(nameof(DestroyBlockAndWait));
                }
                blockCrosshair.GetComponent<SpriteRenderer>().enabled = true;
                handHeldBlock.SetActive(true);
                PV.RPC(nameof(DisplayHandHeldBlockColour), RpcTarget.All);
                if (controller.inventoryEnabled == true)
                {
                    blockIndictorUIImage.SetActive(true);
                }                
            }
        }
        else if (isInBuildMode == false && controller.isDead == false)
        {
            if (PV.IsMine)
            {
                blockCrosshair.GetComponent<SpriteRenderer>().enabled = false;
                handHeldBlock.SetActive(false);
                if (controller.inventoryEnabled == false)
                {
                    blockIndictorUIImage.SetActive(false);
                }
            }            
        }              
    }

    IEnumerator DestroyBlockAndWait()
    {
        DestroyBlock();
        canDestroy = false;
        yield return new WaitForSeconds(0.1f);
        canDestroy = true;
    }

    IEnumerator BuildBlockAndWait()
    {
        BuildBlock();
        canBuild = false;
        yield return new WaitForSeconds(0.1f);
        canBuild = true;
    }    


    void BuildBlock()
    {
        if (Physics.Raycast(blockShootingPoint.position, blockShootingPoint.forward, out RaycastHit hitInfo, 10)) 
        {
            if (hitInfo.transform.tag == "BuildingBlock")
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x + hitInfo.normal.x/2), Mathf.RoundToInt(hitInfo.point.y + hitInfo.normal.y/2), Mathf.RoundToInt(hitInfo.point.z + hitInfo.normal.z/2));                
                blockInstantiated = PhotonNetwork.Instantiate("BuildingBlockPrefab", spawnPosition, Quaternion.identity);
                PV.RPC(nameof(DisplayBlockConstruction), RpcTarget.All, blockInstantiated.GetComponent<PhotonView>().ViewID);
            }
            else //if is the ground
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y) + 0.001f, Mathf.RoundToInt(hitInfo.point.z));
                blockInstantiated = PhotonNetwork.Instantiate("BuildingBlockPrefab", spawnPosition, Quaternion.identity);
                PV.RPC(nameof(DisplayBlockConstruction), RpcTarget.All, blockInstantiated.GetComponent<PhotonView>().ViewID);
            }
        }
        
    }

    void DestroyBlock()
    {
        if (Physics.Raycast(blockShootingPoint.position, blockShootingPoint.forward, out RaycastHit hitInfo, 10) && canDestroy)
        {
            if (hitInfo.transform.tag == "BuildingBlock")
            {
                PV.RPC(nameof(DisplayBlockDestruction), RpcTarget.All, hitInfo.transform.gameObject.GetComponent<PhotonView>().ViewID);
            }            
        }
    }


    [PunRPC]
    void DisplayBlockConstruction(int _blockInstantiatedViewID)
    {

        Material blockMaterial = new Material(lit);
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("BeanPlayerColor"), out Color beanColor))
        {
            blockMaterial.color = beanColor;
        }
        blockMaterial.mainTexture = proBuilderTexture;
        PhotonView.Find(_blockInstantiatedViewID).gameObject.GetComponent<MeshRenderer>().material = blockMaterial;
        PhotonView.Find(_blockInstantiatedViewID).gameObject.GetComponent<AudioSource>().PlayOneShot(placeBlock);
        PhotonView.Find(_blockInstantiatedViewID).gameObject.GetComponent<Animator>().SetBool("isActive", true);
        blockID = _blockInstantiatedViewID;

        if (PV.IsMine)
        {
            Hashtable hash = new();
            hash.Add("blockColour", PlayerPrefs.GetString("BeanPlayerColor"));
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        StartCoroutine(nameof(AutoDestructCountdownTimer), _blockInstantiatedViewID);
    }


    [PunRPC]
    void DisplayHandHeldBlockColour()
    {

        Material blockMaterial = new Material(lit);
        if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("BeanPlayerColor"), out Color beanColor))
        {
            blockMaterial.color = beanColor;
        }
        blockMaterial.mainTexture = proBuilderTexture;
        handHeldBlock.gameObject.GetComponent<MeshRenderer>().material = blockMaterial;      

        if (PV.IsMine)
        {
            Hashtable hash = new();
            hash.Add("handHeldBlockColour", PlayerPrefs.GetString("BeanPlayerColor"));
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }


    [PunRPC]
    void DisplayBlockDestruction(int hitID)
    {
        PhotonNetwork.Destroy(PhotonView.Find(hitID).gameObject);
        this.gameObject.GetComponent<AudioSource>().Stop();
        this.gameObject.GetComponent<AudioSource>().PlayOneShot(destroyBlock);

        StartCoroutine(nameof(DisplayDestroyVFX), hitID);
    }

    IEnumerator DisplayDestroyVFX(int _hitID)
    {
        GameObject destroyVFX = Instantiate(blockDestructionVFX, PhotonView.Find(_hitID).gameObject.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(2);
        Destroy(destroyVFX);
    }

    IEnumerator AutoDestructCountdownTimer(int __blockInstantiatedViewID)
    {
        yield return new WaitForSeconds(4);
        isDestroyed = true;
        if (PhotonView.Find(__blockInstantiatedViewID).gameObject.activeInHierarchy == true)
        {
            Destroy(PhotonView.Find(__blockInstantiatedViewID).gameObject);
        }        

        StartCoroutine(nameof(DisplayDestroyVFX), __blockInstantiatedViewID);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("blockColour") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            Material blockMaterial = new Material(lit);
            if (ColorUtility.TryParseHtmlString("#" + changedProps["blockColour"], out Color beanColor))
            {
                blockMaterial.color = beanColor;
            }
            blockMaterial.mainTexture = proBuilderTexture;
            PhotonView.Find(blockID).gameObject.GetComponent<MeshRenderer>().material = blockMaterial;
        }

        if (changedProps.ContainsKey("handHeldBlockColour") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            Material blockMaterial = new Material(lit);
            if (ColorUtility.TryParseHtmlString("#" + changedProps["blockColour"], out Color beanColor))
            {
                blockMaterial.color = beanColor;
            }
            blockMaterial.mainTexture = proBuilderTexture;
            handHeldBlock.gameObject.GetComponent<MeshRenderer>().material = blockMaterial;
            handHeldBlock.gameObject.GetComponent<MeshRenderer>().material = blockMaterial;
        }
    }
}
