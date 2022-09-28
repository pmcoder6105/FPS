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


    //public Material normalColour;
    public Material highlightedColour;

    GameObject lastHighlightedBlock;

    bool canBuild = true;
    bool canDestroy = true;

    [HideInInspector] public bool isInBuildMode = false;

    PhotonView PV;

    public Material lit;

    float elapsedTime;
    float desiredDuration = 10;

    public Texture proBuilderTexture;

    GameObject blockInstantiated;

    int blockID;

    public AudioClip placeBlock;

    public AudioClip destroyBlock;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {

    }

    void Update()
    {
        if (!PV.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.B))
        {
            isInBuildMode = !isInBuildMode;
        }

        if (isInBuildMode)
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
                
            }
                
            
            //HighlightBlock();
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
        blockID = _blockInstantiatedViewID;

        if (PV.IsMine)
        {
            Hashtable hash = new();
            hash.Add("blockColour", PlayerPrefs.GetString("BeanPlayerColor"));
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }        
    }
    [PunRPC]
    void DisplayBlockDestruction(int hitID)
    {
        Destroy(PhotonView.Find(hitID).gameObject);
        this.gameObject.GetComponent<AudioSource>().Stop();
        this.gameObject.GetComponent<AudioSource>().PlayOneShot(destroyBlock);
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
    }

        void HighlightBlock()
        {
        if (Physics.Raycast(blockShootingPoint.position, blockShootingPoint.forward, out RaycastHit hitInfo, 10))
        {
            if (hitInfo.transform.tag == "BuildingBlock")
            {
                if (lastHighlightedBlock == null)
                {
                    lastHighlightedBlock = hitInfo.transform.gameObject;
                    hitInfo.transform.gameObject.GetComponent<MeshRenderer>().material = highlightedColour;
                }
                else if (lastHighlightedBlock != hitInfo.transform.gameObject)
                {
                    Material blockColour = new Material(lit);
                    lastHighlightedBlock.GetComponent<MeshRenderer>().material = blockColour;
                    hitInfo.transform.gameObject.GetComponent<MeshRenderer>().material = highlightedColour;

                    lastHighlightedBlock = hitInfo.transform.gameObject;
                }
            }
        }
    }
}
