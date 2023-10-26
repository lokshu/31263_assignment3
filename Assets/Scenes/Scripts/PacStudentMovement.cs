using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PacStudentMovement : MonoBehaviour
{
    public float speed = 0.5f; 
    
    private Vector3[] waypoints = new Vector3[4];
    private int currentWaypointIndex = 0;
    private Animator animator;
    private AudioSource audioSource;
    public Tilemap pelletTilemap; 
    public TileBase emptyTile;
    public AudioClip eatPelletSound;    

    void Start()
    {
        gameObject.transform.position = new Vector3(-3.64f, 4.08f, 0);
        Vector3 startPos = transform.position;
        waypoints[0] = startPos;
        waypoints[1] = startPos + new Vector3(1.64f, 0, 0);
        waypoints[2] = startPos + new Vector3(1.64f, -1.38f, 0);
        waypoints[3] = startPos + new Vector3(0, -1.38f, 0);

        QualitySettings.vSyncCount = 0;  // Disable v-sync
        Application.targetFrameRate = 30;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
       
        
    }

    void Update()
    {
        MovePacStudent();
        Vector3Int currentCellPosition = pelletTilemap.WorldToCell(transform.position);
        if (IsPelletTile(currentCellPosition))
        {
            pelletTilemap.SetTile(currentCellPosition, emptyTile);
            audioSource.PlayOneShot(eatPelletSound);
        }
    }

    void MovePacStudent()
    {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex], speed * Time.deltaTime);
        
        // If PacStudent reaches a waypoint, set the next waypoint as the target.
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex]) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % 4; // Cycle back to the first waypoint after the last one.
        }
        //turn the pacstudent
        if(currentWaypointIndex==0){
            animator.Play("Up");
        }else if(currentWaypointIndex==1){
            animator.Play("Right");
        }else if(currentWaypointIndex==2){
            animator.Play("Down");
        }else if(currentWaypointIndex==3){
            animator.Play("Left");
        }
    }
    //switch tile
    bool IsPelletTile(Vector3Int position)
    {
        TileBase currentTile = pelletTilemap.GetTile(position);
        if (currentTile != null && (currentTile.name == "5" || currentTile.name == "6")) 
        {
            return true;
        }else{
            return false;
        }
    }
}
