using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class ConBelt : MonoBehaviour
{

    public GameObject cubePrefab;
    public Transform spawnPoint;
    public float spawnInterval = 1f;
    public float speed = 2f;
    public float destroyZ = 5f;
    public float cubeLifetime = 2f;

    private float timer;
    private List<GameObject> cubes = new List<GameObject>();

    void Update()
    {
       
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            GameObject cube = Instantiate(cubePrefab, spawnPoint.position, Quaternion.identity);
            cubes.Add(cube);
            Destroy(cube, cubeLifetime); 
            timer = 0f;
        }

      
        for (int i = cubes.Count - 1; i >= 0; i--)
        {
            GameObject cube = cubes[i];
            if (cube == null) { cubes.RemoveAt(i); continue; }

            cube.transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (cube.transform.position.z >= destroyZ)
            {
                Destroy(cube);
                cubes.RemoveAt(i);
            }
        }
    }

    void OnDisable()
    {
       
        for (int i = 0; i < cubes.Count; i++)
        {
            if (cubes[i] != null)
                Destroy(cubes[i]);
        }
        cubes.Clear();
    }
}


