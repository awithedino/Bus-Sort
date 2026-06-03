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
    
    [Header("Visuals")]
    public Renderer carBodyRenderer;

    [Header("UI")] 
    public TMP_Text capacityText;
    public Transform capacityCanvas;
    
    [HideInInspector] 
    public Vehicle originalPrefab;
    
    [Header("Effects")]
    public ParticleSystem fullParticle;
    public ParticleSystem sparkParticle;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip fullSound;
    public AudioClip leaveSound;
    public AudioClip hopInSound;
    
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
        if (capacityCanvas != null) 
        {
            capacityCanvas.gameObject.SetActive(isVisible);
        }
    }
    
    public void UpdateCapacityUI()
    {
        if (capacityText != null)
        {
            int val = capacity - boardedCapacity;
            capacityText.text = $"{val}"; 
        }
    }
    
    public void SetColor(Material newMat)
    {
        if (carBodyRenderer != null && newMat != null)
        {
            carBodyRenderer.material = newMat;
        }
    }
    
    public void ApplyColor(ColorType chosenColor)
    {
        carColor = chosenColor; 
        Material assignedMat = ColorManager.Instance.GetMaterial(chosenColor);

        if (assignedMat != null)
        {
            if (carBodyRenderer != null) carBodyRenderer.sharedMaterial = assignedMat;
        }
    }

    public bool isFull()
    {
        return currentCapacity >= capacity;
    }
}
