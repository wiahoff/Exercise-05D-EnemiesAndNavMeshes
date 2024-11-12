using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour
{
    public GameObject heart;

    public GameObject UIcontroller;

    public GameObject[] tiles;

    public GameObject player;

    public List<GameObject> enemyList;

    public GameObject lamp;

    public List<Material> floor;

    public List<Material> wall;

    private int enemyCount = 0;

    // From 0 to 100 the percentage chance of an enemy spawning in a tile
    public int enemySpawnChance;

    const int N = 1;
    const int E = 2;
    const int S = 4;
    const int W = 8;

    Dictionary<Vector2, int> cell_walls = new Dictionary<Vector2, int>();

    float tile_size = 10;
    public int width = 10;   // Width of map  
    public int height = 10;  // Height of map

    List<List<int>> map = new List<List<int>>();

    int floorMat = 0;
    int wallMat = 0;

    // Start is called before the first frame update
    void Start()
    {
        floorMat = UnityEngine.Random.Range(0, floor.Count()-1);
        wallMat = UnityEngine.Random.Range(0, wall.Count()-1);


        cell_walls[new Vector2(0, -1)] = N;
        cell_walls[new Vector2(1, 0)] = E;
        cell_walls[new Vector2(0, 1)] = S;
        cell_walls[new Vector2(-1, 0)] = W;

        MakeMaze();

        GameObject p = GameObject.Instantiate(player);
        p.transform.position = new Vector3(2.91f, 1f, 4.6f);
    }

    private List<Vector2> CheckNeighbors(Vector2 cell, List<Vector2> unvisited) {
        // Returns a list of cell's unvisited neighbors
        List<Vector2> list = new List<Vector2>();

        foreach (var n in cell_walls.Keys)
        {
            if (unvisited.IndexOf((cell + n)) != -1) { 
                list.Add(cell+ n);
            }
                    
        }
        return list;
    }


    private void MakeMaze()
    {
        List<Vector2> unvisited = new List<Vector2>();
        List<Vector2> stack = new List<Vector2>();

        // Fill the map with #15 tiles
        for (int i = 0; i < width; i++)
        {
            map.Add(new List<int>());
            for (int j = 0; j < height; j++)
            {
                map[i].Add(N | E | S | W);
                unvisited.Add(new Vector2(i, j));
            }

        }

        Vector2 current = new Vector2(0, 0);

        unvisited.Remove(current);

        while (unvisited.Count > 0) {
            List<Vector2> neighbors = CheckNeighbors(current, unvisited);

            if (neighbors.Count > 0)
            {
                Vector2 next = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
                stack.Add(current);

                Vector2 dir = next - current;

                int current_walls = map[(int)current.x][(int)current.y] - cell_walls[dir];

                int next_walls = map[(int)next.x][(int)next.y] - cell_walls[-dir];

                map[(int)current.x][(int)current.y] = current_walls;

                map[(int)next.x][(int)next.y] = next_walls;

                current = next;
                unvisited.Remove(current);

            }
            else if (stack.Count > 0) { 
                current = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
            
            }

            
        }

        for (int i = 0; i < width; i++)
        {
            
            for (int j = 0; j < height; j++)
            {
                GameObject tile = GameObject.Instantiate(tiles[map[i][j]]);
                tile.transform.parent = gameObject.transform;
                tile.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material = floor[floorMat];
                tile.transform.GetChild(1).GetChild(0).gameObject.GetComponent<MeshRenderer> ().material = wall[wallMat];
                tile.transform.GetChild(1).GetChild(1).gameObject.GetComponent<MeshRenderer> ().material = wall[wallMat];
                tile.transform.GetChild(1).GetChild(2).gameObject.GetComponent<MeshRenderer> ().material = wall[wallMat];
                tile.transform.GetChild(1).GetChild(3).gameObject.GetComponent<MeshRenderer> ().material = wall[wallMat];

                //spawn enemy
                int enemyType = UnityEngine.Random.Range(0, enemyList.Count);
                int enemyRoll = UnityEngine.Random.Range(0, 100);
                Debug.Log("Spawing: " + enemyType);
                if(enemyRoll <= enemySpawnChance){
                    enemyCount++;
                    Debug.Log("Spawned enemy: " + enemyCount);
                    GameObject enemy = GameObject.Instantiate(enemyList[enemyType]);
                    enemy.transform.Translate(new Vector3((j+1)*tile_size, 1, i*tile_size));
                }
                if(enemyRoll > enemySpawnChance){
                    int special = UnityEngine.Random.Range(0, 100);
                    if(special <= 10){
                        GameObject heartDecor = GameObject.Instantiate(heart);
                        heartDecor.transform.Translate(new Vector3((j+1)*tile_size, 1, i*tile_size));
                    }
                    if(special > 10){
                        GameObject lampDecor = GameObject.Instantiate(lamp);
                        lampDecor.transform.Translate(new Vector3((j+1)*tile_size, 1, i*tile_size));
                    }
                }



                tile.transform.Translate(new Vector3 (j*tile_size, 0, i * tile_size));
                tile.name += " " + i.ToString() + ' ' + j.ToString();
                tile.GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
               
            }

        }

        Variables.Object(UIcontroller).Set("enemyCount", enemyCount);

    }

    
}
