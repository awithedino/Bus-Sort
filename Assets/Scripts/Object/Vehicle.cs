using System;
using UnityEngine;

public enum Direction { Up, Down, Left, Right }

public class Vehicle : MonoBehaviour
{
    // Car info
    public int id;
    public int length;
    // Bool check
    public bool isHorizontal;
    public bool isParked = false;
    // Inside maze
    public Direction moveDirection;
    public ColorType carColor;
    // Car's passenger capacity
    public int capacity = 16;
    public int currentCapacity;
    public int boardedCapacity = 0;

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
