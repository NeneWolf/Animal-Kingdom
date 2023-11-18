using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Multi_PlayerWeapon : MonoBehaviour, IPunObservable
{
    Multi_PlayerLocomotion playerLocomotion;
    Multi_AnimatorManager animatorManager;
    Multi_PlayerManager playerManager;

    [Header("Projectile information")]
    [SerializeField] private GameObject WeaponOrbStorage;

    [Header("Info")]
    [SerializeField] private List<GameObject> weapon;
    public int currentWeapons;


    public float projectileAttackRange;

    [Header("Target Check")]
    [SerializeField] private Transform TargetCheck;
    GameObject target;
    public GameObject currentTarget;
    bool findTarget;

    [Header("Reloading")]
    [SerializeField] GameObject reloadFullChargeSkin;
    bool isReloading;
    [SerializeField] private int reloadTime;

    GameObject self;

    PhotonView photonView;  

    private void Awake()
    {
        playerManager = GetComponent<Multi_PlayerManager>();
        animatorManager = GetComponent<Multi_AnimatorManager>();
        playerLocomotion = GetComponent<Multi_PlayerLocomotion>();
        photonView = GetComponent<PhotonView>();

        self = this.gameObject;
    }

    void Start()
    {
        if (WeaponOrbStorage.activeInHierarchy == false)
            WeaponOrbStorage.SetActive(true);

        foreach (Transform child in WeaponOrbStorage.transform)
        {
            weapon.Add(child.gameObject);
        }

        currentWeapons = weapon.Count;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (!playerManager.ReportDead())
            {
                WeaponOrbStorage.SetActive(true);

                if (currentWeapons == 0 && !isReloading)
                {
                    reloadFullChargeSkin.gameObject.SetActive(false);
                    StartCoroutine(WeaponReload());
                }

                // Deals with turning off or on the "full reloaded skin"
                if (!playerLocomotion.isInvisible && !isReloading)
                {
                    reloadFullChargeSkin.gameObject.SetActive(true);
                }

                if (playerLocomotion.isInvisible)
                    reloadFullChargeSkin.gameObject.SetActive(false);
            }
            else
            {
                WeaponOrbStorage.SetActive(false);
            }
        }
    }


    public void Shoot(bool autoTarget)
    {
        if (photonView.IsMine)
        {
            if (isReloading == false && currentWeapons > 0)
            {
                findTarget = autoTarget;

                if (autoTarget)
                {
                    FindAutoTarget();
                }
                else
                    currentTarget = null;

                photonView.RPC("PlayTargetAnimation", RpcTarget.AllViaServer, "PrimaryAttack", false);
                //animatorManager.PlayTargetAnimation("PrimaryAttack", false);
                weapon[currentWeapons - 1].GetComponent<MeshRenderer>().enabled = false;
                weapon[currentWeapons - 1].GetComponent<SphereCollider>().enabled = false;

                weapon[currentWeapons - 1].GetPhotonView().RPC("FireBullet",
                    RpcTarget.AllViaServer,
                    self.GetComponent<Multi_PlayerManager>().cameraObject.transform.rotation);

                currentWeapons--;
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(TargetCheck.position, projectileAttackRange);
    }

    void FindAutoTarget()
    {
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(TargetCheck.position, projectileAttackRange, transform.forward,0.5f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.tag == "Player" && hit.transform.gameObject != self.gameObject)
            {
                Debug.Log("target:" + hit.transform.gameObject.name);

                target = hit.transform.gameObject;

                //Compare it with the previous distance
                if (currentTarget == null)
                {
                    currentTarget = target;
                }
                else
                {
                    float distanceToNewTarget = Vector3.Distance(transform.position, target.transform.position);
                    float distanceToCurrentTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

                    if (distanceToNewTarget < distanceToCurrentTarget)
                    {
                        currentTarget = target;
                    }
                }
                break;
            }
            else
            {
                target = null;
            }
        }
    }

    IEnumerator WeaponReload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);
        foreach (GameObject orb in weapon)
        {
            orb.GetComponent<MeshRenderer>().enabled = true;
            orb.GetComponent<SphereCollider>().enabled = true;
            yield return new WaitForSeconds(reloadTime/weapon.Count);
        }
        if(!playerLocomotion.isInvisible)
            reloadFullChargeSkin.gameObject.SetActive(true);

        currentWeapons = weapon.Count;
        isReloading = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(reloadFullChargeSkin.activeInHierarchy);

            stream.SendNext(isReloading);
            stream.SendNext(currentWeapons);


            //stream.SendNext(weapon);

            //foreach (GameObject orb in weapon)
            //{
            //    stream.SendNext(orb.GetComponent<MeshRenderer>().enabled);
            //    stream.SendNext(orb.GetComponent<SphereCollider>().enabled);
            //}

            //stream.SendNext(WeaponOrbStorage.activeInHierarchy);
        }
        else
        {
            reloadFullChargeSkin.SetActive((bool)stream.ReceiveNext());

            isReloading = (bool)stream.ReceiveNext();
            currentWeapons = (int)stream.ReceiveNext();

            //weapon = (List<GameObject>)stream.ReceiveNext();
            //foreach (GameObject orb in weapon)
            //{
            //    orb.GetComponent<MeshRenderer>().enabled = (bool)stream.ReceiveNext();
            //    orb.GetComponent<SphereCollider>().enabled = (bool)stream.ReceiveNext();
            //}

            //WeaponOrbStorage.SetActive((bool)stream.ReceiveNext());
        }
    }
}