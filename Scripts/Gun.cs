using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Gun : MonoBehaviour {

    public GameObject end, start, enemymovescript; // The gun start and end point
    public GameObject gun;
    public Animator animator;
    public Text remainingplayerhealth;
    public GameObject spine;
    public GameObject handMag, ammorone, ammortwo, ammorthree;
    public GameObject gunMag;
    public Text gameover;
    float gunShotTime = 0.1f;
    float gunReloadTime = 1.0f;
    Quaternion previousRotation;
    public float health = 100;
    public bool isDead;
    public GameObject shotSound;
    public GameObject muzzleFlash;
    public Text magBullets;
    public Text remainingBullets;
    public GameObject bulletHole, bloodhole;
    int magBulletsVal = 30;
    int remainingBulletsVal = 60;
    int magSize = 30, counter=10;
    public GameObject headMesh;
    public static bool leftHanded { get; private set; }
    public float arp1, arp2, arp3;
    // Use this for initialization
    void Start() {
        headMesh.GetComponent<SkinnedMeshRenderer>().enabled = false; // Hiding player character head to avoid bugs :)
    }

    // Update is called once per frame
    void Update() {
        arp1 = Vector3.Distance(ammorone.transform.position, transform.position);
        arp2 = Vector3.Distance(ammortwo.transform.position, transform.position);
        arp3 = Vector3.Distance(ammorthree.transform.position, transform.position);
        if(arp1<1.2 || arp2<1.2 || arp3<1.2)
        {
            remainingBulletsVal=60;
            magBulletsVal=30;
            magBullets.text = magBulletsVal.ToString() ;
            remainingBullets.text = remainingBulletsVal.ToString();
        }
        // Cool down times
        if (gunShotTime >= 0.0f)
        {
            gunShotTime -= Time.deltaTime;
        }
        if (gunReloadTime >= 0.0f)
        {
            gunReloadTime -= Time.deltaTime;
        }


        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && gunShotTime <= 0 && gunReloadTime <= 0.0f && magBulletsVal > 0 && !isDead)
        { 
            shotDetection(); // Should be completed

            addEffects(); // Should be completed

            animator.SetBool("fire", true);
            gunShotTime = 0.5f;
            
            // Instantiating the muzzle prefab and shot sound
            
            magBulletsVal = magBulletsVal - 1;
            if (magBulletsVal <= 0 && remainingBulletsVal > 0)
            {
                animator.SetBool("reloadAfterFire", true);
                gunReloadTime = 2.5f;
                Invoke("reloaded", 2.5f);
            }
        }
        else
        {
            animator.SetBool("fire", false);
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
        updateText();
       
    }

  

    public void playerBeing_shot() // getting hit from enemy
    {
        health = health - 20;
        //Debug.Log(health);
        remainingplayerhealth.text = health.ToString();
        if(health<10)
        {
            enemymovescript.GetComponent<EnemyMovement>().startplayerShoot = 0;
            GetComponent<CharacterMovement>().isDead = true;    
            gameover.enabled = true;
            GetComponent<CharacterMovement>().restart.enabled = true;
            GetComponent<CharacterMovement>().threedot.enabled = true;
            GetComponent<CharacterMovement>().seconds.enabled = true;    
            StartCoroutine(Restartgame());
        }
    }
    private IEnumerator Restartgame()
    {
        GetComponent<CharacterMovement>().seconds.text = counter.ToString();
        yield return new WaitForSeconds(1);
        counter = counter - 1;
        GetComponent<CharacterMovement>().seconds.text = counter.ToString();
        yield return new WaitForSeconds(1);
        counter = counter - 1;
        GetComponent<CharacterMovement>().seconds.text = counter.ToString();
        yield return new WaitForSeconds(1);
        counter = counter - 1;
        GetComponent<CharacterMovement>().seconds.text = counter.ToString();
        yield return new WaitForSeconds(1);
        counter = counter - 1;
        GetComponent<CharacterMovement>().seconds.text = counter.ToString();
        yield return new WaitForSeconds(1);
        counter = counter - 1;
        GetComponent<CharacterMovement>().seconds.text = counter.ToString();
        yield return new WaitForSeconds(1);
        counter = counter - 1;
        GetComponent<CharacterMovement>().seconds.text = counter.ToString();
        yield return new WaitForSeconds(1);
        counter = counter - 1;
        GetComponent<CharacterMovement>().seconds.text = counter.ToString();
        yield return new WaitForSeconds(1);
        counter = counter - 1;
        GetComponent<CharacterMovement>().seconds.text = counter.ToString();
        yield return new WaitForSeconds(1);
        counter = counter - 1;
        GetComponent<CharacterMovement>().seconds.text = counter.ToString();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

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
        magBullets.text = magBulletsVal.ToString() ;
        remainingBullets.text = remainingBulletsVal.ToString();
    }

    void shotDetection() // Detecting the object which player shot 
    {
        RaycastHit rayHit;
        int layerMask = 1<<8;
        layerMask = ~layerMask;
        if(Physics.Raycast(end.transform.position, (end.transform.position - start.transform.position).normalized, out rayHit, 100.0f, layerMask))
        {
            //Debug.Log(rayHit.transform.gameObject.layer);
            //Debug.Log(rayHit.collider.tag);
            if(rayHit.collider.tag=="enemy")
            {
                rayHit.collider.GetComponent<EnemyMovement>().Being_shot(20);
                GameObject bloodObject = Instantiate(bloodhole, rayHit.point + rayHit.collider.transform.up*0.01f, rayHit.collider.transform.rotation);
                Destroy(bloodObject, 4.0f);
            }
            else if(rayHit.collider.tag=="chest")
            {
                rayHit.collider.GetComponent<EnemyMovement>().Being_shot(30);
                GameObject bloodObject = Instantiate(bloodhole, rayHit.point + rayHit.collider.transform.up*0.01f, rayHit.collider.transform.rotation);
                Destroy(bloodObject, 4.0f);
            }
            else if(rayHit.collider.tag=="head")
            {
                rayHit.collider.GetComponent<EnemyMovement>().Being_shot(100);
                GameObject bloodObject = Instantiate(bloodhole, rayHit.point + rayHit.collider.transform.up*0.01f, rayHit.collider.transform.rotation);
                Destroy(bloodObject, 4.0f);
            }
            else
            {
                GameObject bulletHoleObject = Instantiate(bulletHole, rayHit.point + rayHit.collider.transform.up*0.01f, rayHit.collider.transform.rotation);
                Destroy(bulletHoleObject, 4.0f);
            }
            
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
