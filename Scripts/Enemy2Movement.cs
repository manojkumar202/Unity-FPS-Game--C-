using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Movement : MonoBehaviour
{
    Animator animator;
    public GameObject[] targets;
    public GameObject player, gunobject;
    public int count = 0;
    public float distance, playerEnemyDistance, pedistance = 10.0f;
    public int run_initiated = 0, stop_initiated = 0, first_run = 0, startplayerShoot = 0;   
    // Start is called before the first frame update
    public float enemyHealth = 100, angleENP;
    public int[] shotProbablitylist = new int[]{0,1,0,0,0};
    public int iselseoccured = 0, gotshot=0;
    //public Random rnd = new Random();

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Being_shot() // getting hit from player
    {
        //Debug.Log("SUCESSSSSSSSSSS");
        gotshot=1;
        enemyHealth = enemyHealth - 20;
        if(enemyHealth==0)
        {
            startplayerShoot=0;
            animator.SetTrigger("enemydead");
            gunobject.transform.parent = null;
            //gunobject.AddComponent<Rigidbody>();
            Vector3 m_Input = gunobject.transform.position;
            //Debug.Log(m_Input);
            gunobject.transform.position = new Vector3(m_Input.x, 0, m_Input.z);
            //Debug.Log(gunobject.transform.position);
            //gunobject.isKinematic = true;
        }
        /*
        int index = Random.Range(0,5);
        int shootingProbablity = shotProbablitylist[index];
        if(shootingProbablity==1)
        {
            enemyHealth = enemyHealth - 20;
            if(enemyHealth==0)
            {
                animator.SetTrigger("enemydead");
            }
        }*/
    }
    // Update is called once per frame 
    void Update()
    {
        if(enemyHealth>0)
        {
            playerEnemyDistance = Vector3.Distance(player.transform.position, transform.position);
            //Debug.Log(distance);
            //Debug.Log(playerEnemyDistance);
            angleENP = Vector3.Angle(transform.forward,player.transform.position-transform.position);
            //Debug.Log(angleENP);
            if(playerEnemyDistance>15.0 && gotshot==0)
            {
                if(iselseoccured==1)
                {
                    animator.SetTrigger("walk");
                }
                distance = Vector3.Distance(targets[count].transform.position, transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targets[count].transform.position-transform.position), Time.deltaTime);
                //Debug.Log("playerEnemyDistance greater than");
                if(distance<2)
                {
                    count = count + 1;
                    if(count==2)
                    {
                        count = 0;
                    }
                }

            }
            else
            {
                if(angleENP<60)
                {
                    enemyfiedofView(playerEnemyDistance);
                }
                else
                {
                    if(gotshot==0)
                    {
                        if(iselseoccured==1)
                        {
                            animator.SetTrigger("walk");
                        }
                        distance = Vector3.Distance(targets[count].transform.position, transform.position);
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targets[count].transform.position-transform.position), Time.deltaTime);
                        //Debug.Log("playerEnemyDistance greater than");
                        if(distance<2)
                        {
                            count = count + 1;
                            if(count==2)
                            {
                                count = 0;
                            }
                        }
                    }   
                    else
                    {
                        enemyfiedofView(playerEnemyDistance);
                    }
                }
                
            }  
        }
        
    }
    void enemyfiedofView(float playerEnemyDistance)
    {
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.transform.position-transform.position), Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(player.transform.position-transform.position);
        //Debug.Log("playerEnemyDistance less than");
        if(playerEnemyDistance>pedistance && run_initiated==0 && stop_initiated==0 && first_run==0 && startplayerShoot==0)
        {
            transform.rotation = Quaternion.LookRotation(player.transform.position-transform.position);
            animator.SetTrigger("enemyrun");
            run_initiated = 1;
            first_run = 1;
        }
        else if(playerEnemyDistance<=pedistance && run_initiated==1 && stop_initiated==0)
        {
            transform.rotation = Quaternion.LookRotation(player.transform.position-transform.position);
            animator.SetTrigger("enemyidle");
            startplayerShoot = 1;
            run_initiated = 0;
            stop_initiated = 1;
        }
        else if(playerEnemyDistance>pedistance && run_initiated==0 && stop_initiated==1)
        {
            transform.rotation = Quaternion.LookRotation(player.transform.position-transform.position);
            animator.SetTrigger("eidletorun");
            stop_initiated = 0;
            run_initiated = 1;
            startplayerShoot = 0;
        }
        else if(playerEnemyDistance<=pedistance && startplayerShoot == 0)
        {
            transform.rotation = Quaternion.LookRotation(player.transform.position-transform.position);
            animator.SetTrigger("walktoidle");
            startplayerShoot = 1;
            run_initiated = 0;
            stop_initiated = 1;
        }
    }    
}
