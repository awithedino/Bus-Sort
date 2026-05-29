using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    // Size of the car grid
    public int width;
    public int height;

    private int[,] gridState;

    public Dictionary<int, Vehicle> vehicles = new Dictionary<int, Vehicle>();
    private int nextCarId = 1;
    
    public Transform gridParent;
    public Transform vehicleParent;
    public ParkingManager parkingManager;
    
    public Vector3 gridOffset = new Vector3(0, 0, -3f);
    
    [Header("Display Settings")]
    public float baseGridSize = 6f;
    
    public void Start()
    {
    }

    // ------------ GRID SETUP -----------------
    public void InitializeGrid(int w, int h)
    {
        width = w;
        height = h;
        gridState = new int[width, height];
        vehicles.Clear();
        AutoScaleBoard(width, height);
    }
    
    public Vector3 GridToWorld(int x, int y) {
        float startX = -((width - 1) / 2f) + gridOffset.x;
        float startZ = -((height - 1) / 2f) + gridOffset.z; 
        return new Vector3(startX + x, 0, startZ + y);
    }
    
    public Vector2Int WorldToGrid(Vector3 worldPos) {
        float startX = -((width - 1) / 2f) + gridOffset.x;
        float startZ = -((height - 1) / 2f) + gridOffset.z;
    
        int x = Mathf.RoundToInt(worldPos.x - startX);
        int y = Mathf.RoundToInt(worldPos.z - startZ); // y reads from z!
    
        return new Vector2Int(x, y);
    }
    
    private void AutoScaleBoard(int currentWidth, int currentHeight)
    {
        float maxDimension = Mathf.Max(currentWidth, currentHeight);
        
        if (maxDimension > baseGridSize)
        {
            float scaleFactor = baseGridSize / maxDimension;
            transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
        else
        {
            transform.localScale = Vector3.one; // Keep it normal size
        }
    }
    
    // ---------------- VEHICLE REGISTER, POSITION, SPAWNING ---------------------

    public void RegisterVehicle(Vehicle newVehicle, int startX, int startY) {
        vehicles.Add(newVehicle.id, newVehicle);
    
        for (int i = 0; i < newVehicle.length; i++) {
            int cellX = startX;
            int cellY = startY;
            
            // Vehicles taking slot 
            switch (newVehicle.moveDirection) {
                case Direction.Up: cellY += i; break;
                case Direction.Right: cellX += i; break;
                case Direction.Down: cellY -= i; break;
                case Direction.Left: cellX -= i; break;
            }

            gridState[cellX, cellY] = newVehicle.id;
        }
    }

    public void SpawnVehicle(Vehicle newVehicle, int startX, int startY, Direction dir)
    {
        Vector3 spawnPos = GridToWorld(startX, startY); 
        Vehicle spawnedCar = Instantiate(newVehicle, spawnPos, newVehicle.transform.rotation, vehicleParent);
    
        spawnedCar.id = nextCarId;
        nextCarId++;
    
        spawnedCar.SetupDirection(dir);
        RegisterVehicle(spawnedCar, startX, startY);
    }

    public Vehicle GetVehicleAt(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return null;

        int cellId = gridState[x, y];

        // If it is not empty, grab a car from Dictionary
        if (cellId > 0 && vehicles.ContainsKey(cellId))
        {
            return vehicles[cellId];
        }

        return null;
    }
    
    public bool EscapeVehicle(Vehicle car) {
        Transform targetSlot = parkingManager.GetNextAvailableSlot(car);
        
        if (targetSlot != null) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (gridState[x, y] == car.id) {
                        gridState[x, y] = 0; 
                    }
                }
            }
            Vector3 exitPoint = car.transform.position + (car.transform.forward * 3f);
            car.GetComponent<VehicleAnimator>().AnimateEscape(targetSlot, exitPoint);
            
            car.arrowVisual.gameObject.SetActive(false);
            return true;
        } else {
            ShakeVehicle(car);
            return false;
        }
    }
    
    public void BumpVehicle(Vehicle car, int spaces) {  
        car.GetComponent<VehicleAnimator>().AnimateBump(car.moveDirection, spaces);
    }
    
    public void ShakeVehicle(Vehicle car) {
        car.GetComponent<VehicleAnimator>().AnimateShake();
    }

    public bool IsCellEmpty(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return false;
        return gridState[x, y] == 0;
    }

    public bool CheckPath(Vehicle vehicle, int headX, int headY, out int spaces)
    {
        int stepX = 0; int stepY = 0;
        switch (vehicle.moveDirection) {
            case Direction.Up: stepY = 1; break;
            case Direction.Down: stepY = -1; break;
            case Direction.Left: stepX = -1; break;
            case Direction.Right: stepX = 1; break;
        }

        int currX = headX + stepX;
        int currY = headY + stepY;
        spaces = 0;

        while (currX >= 0 && currY >= 0 && currX < width && currY < height)
        {
            if (gridState[currX, currY] != 0 && gridState[currX, currY] != vehicle.id) {
                return false;
            }
            
            if (gridState[currX, currY] == 0) {
                spaces++; 
            }
            
            currX += stepX;
            currY += stepY;
        }
        return true;
    }
    
    public void ClearBoard()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}