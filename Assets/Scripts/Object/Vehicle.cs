using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public enum Direction { Up, Down, Left, Right }

public class Vehicle : MonoBehaviour
{
    [Header("Car information")]
    public int id;
    public int length;
    
    [Header("True/False checker")]
    public bool isHorizontal;
    public bool isParked = false;
    
    [Header("Car direction, color")]
    public Direction moveDirection;
    public ColorType carColor;
    public Transform arrowVisual;
    
    [Header("Capacity")]
    public int capacity = 16;
    public int currentCapacity;
    public int boardedCapacity = 0;

    [Header("UI")] 
    public TMP_Text capacityText;
    
    void Start()
    {
        UpdateCapacityUI();
        SetTextVisible(false);
    }
    
    public void SetupDirection(Direction dir) {
        moveDirection = dir;
        GetComponent<VehicleAnimator>().UpdateVisualRotation(dir); 
    }

    public void SetTextVisible(bool isVisible)
    {
        if (capacityText != null)
        {
            capacityText.gameObject.SetActive(isVisible);
        }
    }
    
    public void UpdateCapacityUI()
    {
        if (capacityText != null)
        {
            capacityText.text = $"{boardedCapacity}/{capacity}"; 
        }
    }

    public bool isFull()
    {
        return currentCapacity >= capacity;
    }
}
