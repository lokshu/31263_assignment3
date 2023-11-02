using UnityEngine;
using System.Collections;

public class CherryController : MonoBehaviour
{
    public float respawnTime = 10f;
    public float moveSpeed = 1f;
    private Camera mainCamera;
    private Vector2 targetPosition;
    private bool isMoving = false;

    private void Awake()
    {
        mainCamera = Camera.main;
       
        StartCoroutine(RespawnAndMoveCherryCycle());
    }

    private IEnumerator RespawnAndMoveCherryCycle()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(respawnTime);
         
            SetRandomStartPosition();
            SetTargetPosition();

            gameObject.SetActive(true);

            isMoving = true;
            while (isMoving)
            {
                MoveCherryTowardsTarget();
                yield return null;
            }
        }
    }

    private void SetRandomStartPosition()
    {
        
        float offset = 1f; // To make sure it starts off-screen.
        float randomValue = Random.value;
        if (randomValue < 0.25f)
        {
            // Start from left
            transform.position = mainCamera.ViewportToWorldPoint(new Vector3(-offset, Random.value, 0));
        }
        else if (randomValue < 0.5f)
        {
            // Start from right
            transform.position = mainCamera.ViewportToWorldPoint(new Vector3(1 + offset, Random.value, 0));
        }
        else if (randomValue < 0.75f)
        {
            // Start from bottom
            transform.position = mainCamera.ViewportToWorldPoint(new Vector3(Random.value, -offset, 0));
        }
        else
        {
            // Start from top
            transform.position = mainCamera.ViewportToWorldPoint(new Vector3(Random.value, 1 + offset, 0));
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, 0); 
    }

    private void SetTargetPosition()
    {
        
        targetPosition = mainCamera.ViewportToWorldPoint(new Vector3(1 - mainCamera.WorldToViewportPoint(transform.position).x,
                                                                     1 - mainCamera.WorldToViewportPoint(transform.position).y,
                                                                     0));
        targetPosition = new Vector2(targetPosition.x, targetPosition.y);
    }

    private void MoveCherryTowardsTarget()
    {
        
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        
        if ((Vector2)transform.position == targetPosition)
        {
            isMoving = false;
            gameObject.SetActive(false);
        }
    }
}
