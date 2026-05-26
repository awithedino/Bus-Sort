using System;
using UnityEngine;

public enum Direction { Up, Down, Left, Right }

public class Vehicle : MonoBehaviour
{
    public int id;
    public int length;
    public bool isHorizontal;
    public Direction moveDirection;
    public ColorType carColor;
    public int capacity = 16;
    public int currentCapacity;

    public Transform arrowVisual;
    
    public void SetupDirection(Direction dir) {
        moveDirection = dir;
        GetComponent<VehicleAnimator>().UpdateVisualRotation(dir); 
    }

    public bool isFull()
    {
        return currentCapacity >= capacity;
    }
}
