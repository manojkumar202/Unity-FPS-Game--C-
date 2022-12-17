using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class enemy2gun : MonoBehaviour {

    public GameObject end, start; // The gun start and end point
    public GameObject gun;
    public Animator animator;
    
    public GameObject spine;
    public GameObject handMag;
    public GameObject gunMag;
    public int[] shotProbablitylist = new int[]{0,1,0,0,0};
    float gunShotTime = 0.5f;
    float gunReloadTime = 1.0f;
    //Quaternion previousRotation;
    //public float health = 100;
    public bool isDead;
    public GameObject shotSound;
    public GameObject muzzleFlash;
    public Text magBullets;
    public Text remainingBullets;
    public GameObject bulletHole;
    int magBulletsVal = 30;
    int remainingBulletsVal = 90;
    int magSize = 30;
    //public GameObject headMesh;
    public int firststartshoot = 0;
    public static bool leftHanded { get; private set; }

    // Use this for initialization
    void Start() {
        //headMesh.GetComponent<SkinnedMeshRenderer>().enabled = false; // Hiding player character head to avoid bugs :)
    }

    // Update is called once per frame
    void Update() {
        
        // Cool down times
        if (gunShotTime >= 0.0f)
        {
            gunShotTime -= Time.deltaTime;
        }
        if (gunReloadTime >= 0.0f)
        {
            gunReloadTime -= Time.deltaTime;
        }
        if(GetComponent<Enemy2Movement>().player.GetComponent<Gun>().health>0)
        {
            StartCoroutine(enemyfiring());
        }
        
        
        updateText();
       
    }

    IEnumerator enemyfiring()
    {
        if((GetComponent<Enemy2Movement>().startplayerShoot == 0))
        {
            firststartshoot = 0;
        }
        else if((GetComponent<Enemy2Movement>().startplayerShoot == 1) && firststartshoot==0)
        {
            yield return new WaitForSeconds(2);
            firststartshoot = 1;
        }
        else 
        {
            if ( gunShotTime <= 0 && gunReloadTime <= 0.0f && magBulletsVal > 0 && !isDead)
            { 
                shotDetection(); // Should be completed

                addEffects(); // Should be completed

                //animator.SetBool("fire", true);
                gunShotTime = 1.0f;
                
                // Instantiating the muzzle prefab and shot sound
                
                magBulletsVal = magBulletsVal - 1;
                if (magBulletsVal <= 0 && remainingBulletsVal > 0)
                {
                    //animator.SetBool("reloadAfterFire", true);
                    gunReloadTime = 2.5f;
                    Invoke("reloaded", 2.5f);
                }
            }
            else
            {
                //animator.SetBool("fire", false);
            }

            if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.R)) && gunReloadTime <= 0.0f && gunShotTime <= 0.1f && remainingBulletsVal > 0 && magBulletsVal < magSize && !isDead )
            {
                animator.SetBool("reload", true);
                gunReloadTime = 2.5f;
                Invoke("reloaded", 2.0f);
            }
            else
            {
                animator.SetBool("reload", false);
            }
        }
    }

    public void ReloadEvent(int eventNumber) // appearing and disappearing the handMag and gunMag
    {
        if(eventNumber==1)
        {
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
        if(eventNumber==2)
        {
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
    }

    void reloaded()
    {
        int newMagBulletsVal = Mathf.Min(remainingBulletsVal + magBulletsVal, magSize);
        int addedBullets = newMagBulletsVal - magBulletsVal;
        magBulletsVal = newMagBulletsVal;
        remainingBulletsVal = Mathf.Max(0, remainingBulletsVal - addedBullets);
        animator.SetBool("reloadAfterFire", false);
    }

    void updateText()
    {
        //magBullets.text = magBulletsVal.ToString() ;
        //remainingBullets.text = remainingBulletsVal.ToString();
    }

    void shotDetection() // Detecting the object which player shot 
    {
        RaycastHit rayHit;
        int layerMask = 1<<8;
        layerMask = ~layerMask;
        if(Physics.Raycast(end.transform.position, (end.transform.position - start.transform.position).normalized, out rayHit, 100.0f, layerMask))
        {
            if(rayHit.collider.tag=="Player")
            {
                
                int index = Random.Range(0,5);
                int shootingProbablity = shotProbablitylist[index];
                if(shootingProbablity==1)
                {
                    rayHit.collider.GetComponent<Gun>().playerBeing_shot();
                }
            }
            //GameObject bulletHoleObject = Instantiate(bulletHole, rayHit.point + rayHit.collider.transform.up*0.01f, rayHit.collider.transform.rotation);
            //Destroy(bulletHoleObject, 4.0f);
        }
        GameObject muzzleFlashObject = Instantiate(muzzleFlash, end.transform.position, end.transform.rotation);
        muzzleFlashObject.GetComponent<ParticleSystem>().Play();
        Destroy(muzzleFlashObject, 1.0f);
        Destroy((GameObject)Instantiate(shotSound, transform.position, transform.rotation), 1.0f);
    }

    void addEffects() // Adding muzzle flash, shoot sound and bullet hole on the wall
    {

    }

}
