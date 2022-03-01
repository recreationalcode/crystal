using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Airspace : MonoBehaviour
{
    public Cell cell;

    private Renderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void SetActive(bool isActive) {
        _renderer.enabled = isActive;
    }

    private void OnTriggerEnter(Collider other)
    {
        Ship ship = other.transform.parent.gameObject.GetComponent<Ship>();

        if (ship == null) return;

        if (cell.crystallizedBy == null && other.gameObject.CompareTag("Fractal"))
        {    
            cell.crystallizedBy = ship;   
        }
        else if (!cell.IsProtected() && other.gameObject.CompareTag("Shadow"))
        {    
            cell.crystallizedBy = null;   
        }

        if (ship is Player)
        {
            Player player = ship as Player;

            NetworkRunner runner = NetworkRunner.GetRunnerForGameObject(player.gameObject);

            if(cell.GetFaction() == player.faction &&
                player.HasAvailableTowers() &&
                !cell.IsProtected() &&
                GameObject.ReferenceEquals(player.gameObject, runner.GetPlayerObject(runner.LocalPlayer).gameObject))
            {
                player.EnableTowerPlacement(this);
            }
        }
        // TODO Remove when AI is no longer required for demo purposes
        else if (ship is PlayerAI)
        {
            if(cell.GetFaction() == ship.faction && !cell.IsProtected() && Random.value <= 0.04f)
            {
                GridManager.currentGridManager.PlaceTower(cell.GetAxialCoordinates(), ship.GetTowerPrefab());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Ship ship = other.transform.parent.gameObject.GetComponent<Ship>();

        if (ship == null) return;

        if (ship is Player)
        {
            Player player = ship as Player;

            NetworkRunner runner = NetworkRunner.GetRunnerForGameObject(player.gameObject);

            if(GameObject.ReferenceEquals(player.gameObject, runner.GetPlayerObject(runner.LocalPlayer).gameObject))
            {
                player.DisableTowerPlacement(this);
            }
        }
    }
}
