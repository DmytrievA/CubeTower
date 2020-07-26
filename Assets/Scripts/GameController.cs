using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private CubePosition currentCube = new CubePosition(0,1,0);
    private bool continuePlaying = true;
    private Coroutine showCubePlaceCoroutine = null;

    public GameObject restartButton;
    public Transform cubeToPlace;
    public GameObject cubeToCreate;
    public GameObject allCubes;
    public float nextCubePositionUpdateSpeed = 0.7f;
    public bool continueShowNextCube = true;

    private List<Vector3> allCubesPositions = new List<Vector3>
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(0,0,1),
        new Vector3(1,0,1),
        new Vector3(-1,0,0),
        new Vector3(-1,0,-1),
        new Vector3(-1,0,1),
        new Vector3(1,0,-1),
        new Vector3(0,1,0)
    };

    private void OnEnable()
    {
        ExplodeCubes.OnCubeExploded += new EventHandler(this.OnCubesExploded);
    }

    private void OnDisable()
    {
        ExplodeCubes.OnCubeExploded -= new EventHandler(this.OnCubesExploded);
    }

    private void OnCubesExploded(object sender, EventArgs args)
    {
        if(this.restartButton != null)
            this.restartButton.SetActive(true);

        Camera.main.transform.position += new Vector3(0, 0, 3);
    }

    private void Start()
    {
        //start infinite spawning of new cube position
        this.showCubePlaceCoroutine = this.StartCoroutine(this.ShowCubePlace());
    }

    private void Update()
    {
        if (this.continuePlaying && (Input.GetMouseButtonDown(0/*left button number*/) || Input.touchCount > 0))
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif
            //creating new cube
            GameObject newCube =  Instantiate(this.cubeToCreate, cubeToPlace.position, Quaternion.identity) as GameObject;
            //make it part of allCubes rigidbody
            newCube.transform.SetParent(this.allCubes.transform);

            //update position if last cube
            currentCube.setVector(newCube.transform.position);
            //save new occupied(занятое) position
            this.allCubesPositions.Add(currentCube.getVector());

            //draw cube to place in new position
            this.SpawnPositions();

            //to update rigid body to check is tower stable
            this.allCubes.GetComponent<Rigidbody>().isKinematic = true;
            this.allCubes.GetComponent<Rigidbody>().isKinematic = false;
        }

        //check if main cube is moving
        if(this.continuePlaying && this.allCubes.GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {
            //if tower is falling we delete cube to place from scene
            Destroy(cubeToPlace.gameObject);
            //stop position spawning
            this.StopCoroutine(this.showCubePlaceCoroutine);

            this.continuePlaying = false;
        }
    }

    private IEnumerator ShowCubePlace()
    {
        while (continueShowNextCube)
        {
            this.SpawnPositions();
            yield return new WaitForSeconds(this.nextCubePositionUpdateSpeed);
        }
    }

    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (var item in this.currentCube.getNeighbours())
        {
            if (cubeToPlace.position == item)
                continue;

            if(this.IsPositionEmpty(item))
            {
                positions.Add(item);
            }
        }

        cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
    }

    private bool IsPositionEmpty(Vector3 toCheck)
    {
        if(toCheck.y <=0 )
        {
            return false;
        }
        if(allCubesPositions.Any(x=>x == toCheck))
        {
            return false;
        }
        return true;
    }
}

public enum NeighbourPosition
{
    Top = 0, Bottom, Left, Right, Front, Back
}

public struct CubePosition
{
    public int x, y, z;

    public CubePosition(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 getVector()
    {
        return new Vector3(x, y, z);
    }

    public Vector3 getVector(NeighbourPosition position)
    {
        switch (position)
        {
            case NeighbourPosition.Top:
                return new Vector3(x, y + 1, z);
            case NeighbourPosition.Bottom:
                return new Vector3(x, y - 1, z);
            case NeighbourPosition.Left:
                return new Vector3(x - 1, y, z);
            case NeighbourPosition.Right:
                return new Vector3(x + 1, y, z);
            case NeighbourPosition.Front:
                return new Vector3(x, y, z + 1);
            case NeighbourPosition.Back:
                return new Vector3(x, y, z - 1);
            default:
                return new Vector3(x, y, z);
        }
    }

    public IEnumerable<Vector3> getNeighbours()
    {
        var _this = this;
        return Enum.GetValues(typeof(NeighbourPosition)).OfType<NeighbourPosition>().Select(x => _this.getVector(x)).ToArray();
    }

    public void setVector(Vector3 src)
    {
        this.x = Convert.ToInt32(src.x);
        this.y = Convert.ToInt32(src.y);
        this.z = Convert.ToInt32(src.z);
    }
}
