using UnityEngine;

public class LifeIndicator : MonoBehaviour
{
    public GameObject pacManPrefab; // Assign your Pac-Man prefab in the inspector
    public int numberOfLives = 3; // Set the initial number of lives

    private float spacing = 0.4f; // Spacing between each Pac-Man

    void Start()
    {
        CreateLives();
    }

    void CreateLives()
    {
        for (int i = 0; i < numberOfLives; i++) // Start at 1 to account for the original Pac-Man
        {
            // Instantiate a new Pac-Man prefab and set its position
            GameObject newPacMan = Instantiate(pacManPrefab, new Vector3(transform.position.x + spacing * i, transform.position.y, transform.position.z), Quaternion.identity);
            newPacMan.transform.parent = transform; // Optional: Set the new Pac-Man as a child of the script's GameObject
        }
    }
}
