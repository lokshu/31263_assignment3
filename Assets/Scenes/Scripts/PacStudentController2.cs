using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PacStudentController2 : MonoBehaviour
{
    public Tilemap[] levelTilemaps; // Assign your level tilemaps in the inspector
    public float moveSpeed = 5.0f;
    private Vector3Int currentGridPosition;
    private Vector3 targetWorldPosition;
    private bool isMoving = false;
    private Vector2 lastInput;
    private Vector2 currentInput;

    void Start()
    {
        currentGridPosition = levelTilemaps[0].WorldToCell(transform.position);
        targetWorldPosition = transform.position;
    }

    void Update()
    {
        GatherInput();
        TryMove();
    }

    void GatherInput()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (input != Vector2.zero)
        {
            lastInput = input;
        }
    }

    void TryMove()
    {
        if (!isMoving)
        {
            Vector3Int direction = Vector3Int.RoundToInt(lastInput);
            if (direction != Vector3Int.zero)
            {
                Vector3Int newGridPosition = currentGridPosition + direction;
                if (CanMoveTo(newGridPosition))
                {
                    currentInput = lastInput;
                    StartCoroutine(MoveToNewPosition(newGridPosition));
                }
                else if (CanMoveTo(currentGridPosition + Vector3Int.RoundToInt(currentInput)))
                {
                    StartCoroutine(MoveToNewPosition(currentGridPosition + Vector3Int.RoundToInt(currentInput)));
                }
            }
        }
    }

    IEnumerator MoveToNewPosition(Vector3Int newGridPosition)
    {
        isMoving = true;
        // We're assuming all tilemaps have the same orientation and size
        targetWorldPosition = levelTilemaps[0].GetCellCenterWorld(newGridPosition);
        float elapsedTime = 0;

        while (elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(transform.position, targetWorldPosition, (elapsedTime / moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetWorldPosition; // Ensures PacStudent is exactly at the target position
        currentGridPosition = newGridPosition; // Update current grid position
        isMoving = false;
    }

    bool CanMoveTo(Vector3Int gridPosition)
    {
        foreach (var tilemap in levelTilemaps)
        {
            TileBase tile = tilemap.GetTile(gridPosition);
            if (tile != null && IsWalkable(tile))
            {
                return true;
            }
        }
        return false;
    }

    bool IsWalkable(TileBase tile)
    {
        // Your logic to determine if the tile is walkable goes here
        string tileName = tile.name;
        return tileName == "0" || tileName == "1" || tileName == "5";
    }
}
