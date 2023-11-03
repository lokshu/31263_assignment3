using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class PacStudentController : MonoBehaviour
{
    private Vector2 initalPosition = new Vector2(-5.96f, 4.16f);
    public Tilemap levelTilemap;
    public float moveSpeed = 3.0f;
    private Vector2 targetPosition;
    private Vector2 lastInput;
    private Vector2 currentInput;
    private bool isLerping;
    private bool isHitWallSoundPlayed;
    private int[] walkableTiles = {0, 5, 6};
    private float cellSize = 0.32f;
    private Animator animator;
    public TileBase emptyTile;
    public AudioClip eatPelletSound;
    public AudioClip moveEmptySound;
    private AudioSource audioSource; 
    private Vector2 previousPosition;
    private int dustGas=0; // add some gas to dust system each time when pac student move
    public ParticleSystem dustParticles;
    public AudioClip wallBumpSound; 
    public ParticleSystem wallBumpParticles;
    private int GameScore = 0;
    public TextMeshProUGUI ScoreTextBox;
    public TextMeshProUGUI ScaredTimer;
    public TextMeshProUGUI GameTimer;
    public TextMeshProUGUI GameOverTextBox;
    private int GhostScaredSec;
    public AudioClip normalBackgroundMusic;
    public AudioClip scaredBackgroundMusic; 
    public GameObject backgroundMusic;
    private AudioSource backgroundMusicAudioSource;
    private int ScorePellets = 10;
    private int ScoreCherry = 100;
    private int KillScaredGhost = 300;
    private float timer = 1f;
    private int ghostScaredSec = 0;
    private int totalGameSec=0;
    private int lifeLost=0;
    private bool isGamePause = true;
    private int gameOverCountDown = 3; 
    public GameObject[] lifeIndictor;
    public GameObject dustEffectPrefab;
    private int firstCountDown=5;
    GameObject[] ghosts;

    void Start()
    {
        transform.position = initalPosition;
        targetPosition = transform.position; // Initialize targetPosition to the start position of PacStudent.
        previousPosition = targetPosition;
        Application.targetFrameRate = 60;
        animator = GetComponent<Animator>();
        animator.Play("Right");
        PacStudentIsStop(true);
        audioSource = GetComponent<AudioSource>();
        QualitySettings.vSyncCount = 0;
        dustParticles.Play();
        wallBumpParticles.Play();
        
        backgroundMusicAudioSource = backgroundMusic.GetComponent<AudioSource>();

        ghosts = GameObject.FindGameObjectsWithTag("Ghost");
    }
    
    void Update()
    {
        if(!isGamePause){
            GatherInput();
            TryMove();
            GetPoint();
            MovementDust();
        }
        timer -= Time.deltaTime; 
        if (timer <= 0)
        {
            OnSecondPassed();
            timer = 1f; 
        }
    }
    void MovementDust(){
        if(dustGas>0){
            if (!dustParticles.isPlaying)
            {
                dustParticles.Play();
            }
            dustGas--;      
        }else{
            if (dustParticles.isPlaying)
            {
                dustParticles.Stop();
                wallBumpParticles.Stop();
            }     
        }
        
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
                GameScore = GameScore + ScorePellets;
                ScoreTextBox.text = GameScore.ToString();
                if(currentTile.name == "5"){
                    
                }else if (currentTile.name == "6"){
                    GhostScaredStart();
                }
                
            }else{
                if (!audioSource.isPlaying){
                    audioSource.PlayOneShot(moveEmptySound);
                }
            }
            if(dustGas<10){
                dustGas = dustGas + 5;
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        float offset = cellSize*21 -cellSize/2;
        if (other.CompareTag("TeleportRight")){
            transform.position = new Vector3(transform.position.x - offset, transform.position.y, transform.position.z);
            isLerping = false;
        }else if (other.CompareTag("TeleportLeft")){
            transform.position = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);
            isLerping = false;
        }else if (other.CompareTag("Cherry")){
            GameScore = GameScore + ScoreCherry;
            ScoreTextBox.text = GameScore.ToString();
        }else if(other.CompareTag("Ghost")){
            if(ghostScaredSec>0){
                GameScore = GameScore + KillScaredGhost;
                ScoreTextBox.text = GameScore.ToString();
            }else{
                PacStudentDie();
            }
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
                    wallBumpParticles.Stop();
                    return true; 
                } else {
                    if(!isHitWallSoundPlayed){
                        audioSource.PlayOneShot(wallBumpSound);
                        isHitWallSoundPlayed = true;
                        wallBumpParticles.Play();
                    }
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
        isHitWallSoundPlayed = false;
    }

    void LerpToPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if ((Vector2)transform.position == targetPosition)
        {
            isLerping = false; 
        }
    }
    private void OnSecondPassed(){
        if(ghostScaredSec>0){
            ScaredTimer.text = ghostScaredSec.ToString();
            ghostScaredSec--;
        }else if(ghostScaredSec==0){
            GhostScaredFinished();
            ScaredTimer.text = "";
        }
        if(!isGamePause){
            totalGameSec++;
            GameTimer.text = SecondsToFormattedTime(totalGameSec);
        }    

        if(isGamePause){
            if(firstCountDown>0){
                firstCountDown--;
                GameOverTextBox.text = (firstCountDown-1).ToString();
                if(firstCountDown==1){
                    GameOverTextBox.text = "GO!";
                }else if(firstCountDown==0){
                    isGamePause = false;
                    GameOverTextBox.text = "";
                }
            }else{
                if(gameOverCountDown>0){
                    gameOverCountDown--;
                }else{
                    SceneManager.LoadScene("StartScene");
                }
            }
        }
    }
    private void GhostScaredFinished(){
        backgroundMusicAudioSource.clip = normalBackgroundMusic;
        backgroundMusicAudioSource.loop = true;
        backgroundMusicAudioSource.Play(); 

        foreach (GameObject ghost in ghosts)
        {
            Animator ghostAnimator = ghost.GetComponent<Animator>();
            if (ghostAnimator != null) 
            {
                ghostAnimator.Play("Left"); 
            }
        }
    }
    private void GhostScaredStart(){
        backgroundMusicAudioSource.clip = scaredBackgroundMusic;
        backgroundMusicAudioSource.loop = true;
        backgroundMusicAudioSource.Play(); 
        ghostScaredSec = 10;

        foreach (GameObject ghost in ghosts)
        {
            Animator ghostAnimator = ghost.GetComponent<Animator>();
            if (ghostAnimator != null) 
            {
                ghostAnimator.Play("Scared"); 
            }
        }
    }
    private void PacStudentDie(){
        if(lifeLost<2){
            lifeIndictor[lifeLost].SetActive(false);
            lifeLost++;
            Instantiate(dustEffectPrefab, transform.position, Quaternion.identity);
            transform.position = initalPosition;
            targetPosition = transform.position; 
            previousPosition = targetPosition;
        }else{
            GameOver();
        }
    }
    private void GameOver(){
        isGamePause = true;
        GameOverTextBox.text = "Game Over!!";
        if (PlayerPrefs.HasKey("HighScore")) 
        {
            int savedHighScore = PlayerPrefs.GetInt("HighScore");
            if(GameScore>savedHighScore){
                PlayerPrefs.SetInt("HighScore", GameScore);
                PlayerPrefs.SetString("GameTime", SecondsToFormattedTime(totalGameSec));
                PlayerPrefs.Save();
            }
        }else{
            PlayerPrefs.SetInt("HighScore", GameScore);
            PlayerPrefs.SetString("GameTime", SecondsToFormattedTime(totalGameSec));
            PlayerPrefs.Save();
        }
    }
    private  string SecondsToFormattedTime(int seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);
    }
}
