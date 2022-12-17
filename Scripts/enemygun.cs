using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class enemygun : MonoBehaviour {

    public GameObject end, start; // The gun start and end point
    public GameObject gun;
    public Animator animator;
    
    public GameObject spine;
    public GameObject handMag;
    public GameObject gunMag;
    public float[] shotProbablitylist;
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
    int magSize = 30, numberofshots =0 ;
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
        if(GetComponent<EnemyMovement>().player.GetComponent<Gun>().health>0)
        {
            StartCoroutine(enemyfiring());
        }
        
        
        updateText();
       
    }

    IEnumerator enemyfiring()
    {
        if((GetComponent<EnemyMovement>().startplayerShoot == 0))
        {
            firststartshoot = 0;
        }
        else if((GetComponent<EnemyMovement>().startplayerShoot == 1) && firststartshoot==0)
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
    public void probshuffle()
    {
        for (var i = 0; i < 5; ++i)
        {
            var r = UnityEngine.Random.Range(i, 5);
            var tmp = shotProbablitylist[i];
            shotProbablitylist[i] = shotProbablitylist[r];
            shotProbablitylist[r] = tmp;
        }
    }
    void shotDetection() // Detecting the object which player shot 
    {
        if(numberofshots%5==0)
        {
            probshuffle();
            numberofshots = 0;
        }
        string result = "List contents: ";
        foreach (var item in shotProbablitylist)
        {
            result += item.ToString() + ", ";
        }
        //Debug.Log(result);
        
        RaycastHit rayHit;
        int layerMask = 1<<8;
        layerMask = ~layerMask;
        //(end.transform.position - start.transform.position).normalized
        Vector3 t_end = end.transform.position;
        //Debug.Log("**");
        //Debug.Log(t_end);
        //int[] t = new int[]{0,1.3,2,-1.3,3};
        //int index = Random.Range(0,5);
        //int shootingProbablity = t[index];
        //Debug.Log("**");
        //Debug.Log(numberofshots);
        float shootingProbablityValue = shotProbablitylist[numberofshots];
        //Debug.Log(shootingProbablityValue);
        if(Physics.Raycast(end.transform.position, (new Vector3(t_end.x+shootingProbablityValue, t_end.y, t_end.z) - start.transform.position).normalized, out rayHit, 100.0f, layerMask))
        {
            if(rayHit.collider.tag=="Player")
            {
                //int index = Random.Range(0,5);
                //int shootingProbablity = shotProbablitylist[index];
                rayHit.collider.GetComponent<Gun>().playerBeing_shot();

                //if(shotProbablitylist[numberofshots]==1)
                //{
                  //  rayHit.collider.GetComponent<Gun>().playerBeing_shot();
                //}
            }
            //GameObject bulletHoleObject = Instantiate(bulletHole, rayHit.point + rayHit.collider.transform.up*0.01f, rayHit.collider.transform.rotation);
            //Destroy(bulletHoleObject, 4.0f);
        }
        GameObject muzzleFlashObject = Instantiate(muzzleFlash, end.transform.position, end.transform.rotation);
        muzzleFlashObject.GetComponent<ParticleSystem>().Play();
        Destroy(muzzleFlashObject, 1.0f);
        Destroy((GameObject)Instantiate(shotSound, transform.position, transform.rotation), 1.0f);
        numberofshots = numberofshots + 1;
    }

    void addEffects() // Adding muzzle flash, shoot sound and bullet hole on the wall
    {

    }

}
