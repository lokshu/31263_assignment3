using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PacStudentController : MonoBehaviour
{
    public Tilemap levelTilemap;
    public float moveSpeed = 3.0f;
    private Vector2 targetPosition;
    private Vector2 lastInput;
    private Vector2 currentInput;
    private bool isLerping;
    private int[] walkableTiles = {0, 5, 6};
    private float cellSize = 0.32f;
    private Animator animator;
    public TileBase emptyTile;
    public AudioClip eatPelletSound;
    public AudioClip moveEmptySound;
    private AudioSource audioSource; 
    private Vector2 previousPosition;
    
    void Start()
    {
        targetPosition = transform.position; // Initialize targetPosition to the start position of PacStudent.
        previousPosition = targetPosition;
        Application.targetFrameRate = 60;
        animator = GetComponent<Animator>();
        animator.Play("Right");
        PacStudentIsStop(true);
        audioSource = GetComponent<AudioSource>();
        QualitySettings.vSyncCount = 0;
       

    }
    
    void Update()
    {
        GatherInput();
        TryMove();
        GetPoint();
    }
    void GetPoint(){
        Vector3Int currentCellPosition = levelTilemap.WorldToCell(transform.position);
        TileBase currentTile = levelTilemap.GetTile(currentCellPosition);
        Vector2 currentPosition = transform.position;
        if(previousPosition != currentPosition){
            if (currentTile != null && (currentTile.name == "5" || currentTile.name == "6")) 
            {
                levelTilemap.SetTile(currentCellPosition, emptyTile);
                audioSource.PlayOneShot(eatPelletSound);
                
            }else{
                if (!audioSource.isPlaying){
                    audioSource.PlayOneShot(moveEmptySound);
                }
            }
        }
        previousPosition = currentPosition;
    }
    void GatherInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) {
            lastInput = Vector2.up;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            lastInput = Vector2.left;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            lastInput = Vector2.down;
        }
        if (Input.GetKeyDown(KeyCode.D)) { 
            lastInput = Vector2.right;
        }
        
    }

    void TryMove()
    {
        if (!isLerping)
        {
            if (CanMoveTo(lastInput))
            {
                currentInput = lastInput;
                StartLerp(currentInput);
            }
            else if (CanMoveTo(currentInput))
            {
                StartLerp(currentInput);
            }
        }
        else
        {
            LerpToPosition();
        }
    }

    bool CanMoveTo(Vector2 direction)
    {
        Vector3 worldPosition = transform.position + ((Vector3)direction * cellSize);
        Vector3Int gridPosition = levelTilemap.WorldToCell(worldPosition);
        
        TileBase tile = levelTilemap.GetTile(gridPosition);
        if (tile != null)
        {
            int tileType;
            if (int.TryParse(tile.name, out tileType))
            {
                if (System.Array.Exists(walkableTiles, element => element == tileType))
                {
                    PacStudentIsStop(false);
                    if(direction == Vector2.down){
                        animator.Play("Down");
                    }else if(direction == Vector2.up){
                        animator.Play("Up");
                    }else if(direction == Vector2.left ){
                        animator.Play("Left");
                    }else if( direction == Vector2.right){
                        animator.Play("Right");
                    }
                    
                    return true; 
                }
            }
        }
        
        PacStudentIsStop(true);
        return false;
    }

    void PacStudentIsStop(bool isStopped){
           if(isStopped){
                animator.enabled = false;
           }else{
                animator.enabled = true;
           } 
    }
    void StartLerp(Vector2 direction)
    {
        direction  = direction * cellSize;
        targetPosition = (Vector2)transform.position + direction; 
        isLerping = true;
    }

    void LerpToPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if ((Vector2)transform.position == targetPosition)
        {
            isLerping = false; 
        }
    }
}
