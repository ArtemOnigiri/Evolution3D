using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature
{
	private GameObject headPrefab;
	private GameObject cubePrefab;
	private GameObject foodPrefab;
	private float xOffset;
	private float zOffset;
	public List<GameObject> segments;
	private float fitness;
	private float minDistToTarget;
	private GameObject food;
	public GameObject[] sensorsX = new GameObject[2];
	public GameObject[] sensorsY = new GameObject[2];
	public GameObject[] sensorsZ = new GameObject[2];

	public NN nn;

	public Creature(GameObject headPrefab, GameObject cubePrefab, GameObject foodPrefab, float xOffset, float zOffset)
	{
		this.headPrefab = headPrefab;
		this.cubePrefab = cubePrefab;
		this.foodPrefab = foodPrefab;
		this.xOffset = xOffset;
		this.zOffset = zOffset;
		this.segments = new List<GameObject>();
		this.fitness = 0f;
		CreateSegments();
		UpdateFood();
		nn = new NN(segments.Count);
	}

	public void Update()
	{
		float[] sensorsDistsX = new float[2];
		float[] sensorsDistsY = new float[2];
		float[] sensorsDistsZ = new float[2];
		for (int i = 0; i < 2; i++) sensorsDistsX[i] = Vector3.Distance(sensorsX[i].transform.position, food.transform.position);
		for (int i = 0; i < 2; i++) sensorsDistsY[i] = Vector3.Distance(sensorsY[i].transform.position, food.transform.position);
		for (int i = 0; i < 2; i++) sensorsDistsZ[i] = Vector3.Distance(sensorsZ[i].transform.position, food.transform.position);
		float avgX = (sensorsDistsX[0] + sensorsDistsX[1]) / 2f;
		float avgY = (sensorsDistsY[0] + sensorsDistsY[1]) / 2f;
		float avgZ = (sensorsDistsZ[0] + sensorsDistsZ[1]) / 2f;
		nn.neurons[0, 0] = (sensorsDistsX[0] - avgX) / 2f;
		nn.neurons[0, 1] = (sensorsDistsY[0] - avgY) / 2f;
		nn.neurons[0, 2] = (sensorsDistsZ[0] - avgZ) / 2f;
		nn.Update();
		for (int i = 1; i < segments.Count; i++)
		{
			JointSpring spring = segments[i].GetComponent<HingeJoint>().spring;
			spring.targetPosition = 60 * nn.neurons[nn.layers - 1, i];
			segments[i].GetComponent<HingeJoint>().spring = spring;
		}
		minDistToTarget = Mathf.Min(minDistToTarget, Vector3.Distance(segments[0].transform.position, food.transform.position));
		if(minDistToTarget < 4)
		{
			UpdateFood();
			fitness += 20;
		}
	}

	public float GetFitness()
	{
		// return fitness - minDistToTarget + 15f;
		return segments[0].transform.position.z;
	}

	public void CreateSegments()
	{
		fitness = 0f;
		if(segments.Count > 0)
		{
			for (int i = 0; i < segments.Count; i++)
			{
				GameObject.Destroy(segments[i]);
			}
		}
		segments.Clear();
		GameObject head = GameObject.Instantiate(headPrefab, new Vector3(xOffset, 1.5f, zOffset), Quaternion.identity);
		for (int i = 0; i < 2; i++) sensorsX[i] = head.transform.Find("X" + i).gameObject;
		for (int i = 0; i < 2; i++) sensorsY[i] = head.transform.Find("Y" + i).gameObject;
		for (int i = 0; i < 2; i++) sensorsZ[i] = head.transform.Find("Z" + i).gameObject;
		Worm(head);
		// UpdateFood();
	}

	private void Worm(GameObject head)
	{
		segments.Add(head);
/*		{
			GameObject newSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset, 1.5f, zOffset - 1f), Quaternion.identity);
			Connect(newSegment, segments[0]);
			segments.Add(newSegment);
		}
		{
			GameObject newSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset, 1.5f, zOffset - 2f), Quaternion.identity);
			newSegment.GetComponent<HingeJoint>().axis = new Vector3(0f, 1f, 0f);
			Connect(newSegment, segments[1]);
			segments.Add(newSegment);
		}*/
		for (int i = 0; i < 5; i++)
		{
			GameObject newSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset, 1.5f, zOffset - 1f - i), Quaternion.identity);
			Connect(newSegment, segments[i]);
			segments.Add(newSegment);
		}
	}

	private void Crab(GameObject head)
	{
		segments.Add(head);
		{
			GameObject upperSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset - 2f, 3f, zOffset - 2.5f), Quaternion.identity);
			Connect(upperSegment, head);
			upperSegment.GetComponent<HingeJoint>().axis = new Vector3(0f, 1f, 0f);
			segments.Add(upperSegment);
			GameObject middleSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset - 2f, 2f, zOffset - 2.5f), Quaternion.identity);
			Connect(middleSegment, upperSegment);
			segments.Add(middleSegment);
			GameObject lowerSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset - 2f, 1f, zOffset - 2.5f), Quaternion.identity);
			Connect(lowerSegment, middleSegment);
			segments.Add(lowerSegment);
		}
		{
			GameObject upperSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset + 2f, 3f, zOffset - 2.5f), Quaternion.identity);
			Connect(upperSegment, head);
			upperSegment.GetComponent<HingeJoint>().axis = new Vector3(0f, 1f, 0f);
			segments.Add(upperSegment);
			GameObject middleSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset + 2f, 2f, zOffset - 2.5f), Quaternion.identity);
			Connect(middleSegment, upperSegment);
			segments.Add(middleSegment);
			GameObject lowerSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset + 2f, 1f, zOffset - 2.5f), Quaternion.identity);
			Connect(lowerSegment, middleSegment);
			segments.Add(lowerSegment);
		}
		{
			GameObject upperSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset - 2f, 3f, zOffset + 2.5f), Quaternion.identity);
			Connect(upperSegment, head);
			upperSegment.GetComponent<HingeJoint>().axis = new Vector3(0f, 1f, 0f);
			segments.Add(upperSegment);
			GameObject middleSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset - 2f, 2f, zOffset + 2.5f), Quaternion.identity);
			Connect(middleSegment, upperSegment);
			segments.Add(middleSegment);
			GameObject lowerSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset - 2f, 1f, zOffset + 2.5f), Quaternion.identity);
			Connect(lowerSegment, middleSegment);
			segments.Add(lowerSegment);
		}
		{
			GameObject upperSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset + 2f, 3f, zOffset + 2.5f), Quaternion.identity);
			Connect(upperSegment, head);
			upperSegment.GetComponent<HingeJoint>().axis = new Vector3(0f, 1f, 0f);
			segments.Add(upperSegment);
			GameObject middleSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset + 2f, 2f, zOffset + 2.5f), Quaternion.identity);
			Connect(middleSegment, upperSegment);
			segments.Add(middleSegment);
			GameObject lowerSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset + 2f, 1f, zOffset + 2.5f), Quaternion.identity);
			Connect(lowerSegment, middleSegment);
			segments.Add(lowerSegment);
		}
	}

	private void UpdateFood()
	{
		bool farEnough = false;
		Vector3 targetPosition = Vector3.one;
		while(!farEnough)
		{
			minDistToTarget = 15f;
			Vector2 target = Random.insideUnitCircle * 15;
			targetPosition = new Vector3(target.x + xOffset, 2f, target.y + zOffset);
			minDistToTarget = Mathf.Min(minDistToTarget, Vector3.Distance(segments[0].transform.position, targetPosition));
			if(minDistToTarget > 5) farEnough = true;
		}
		if(food != null)
		{
			GameObject.Destroy(food);
		}
		food = GameObject.Instantiate(foodPrefab, targetPosition, Quaternion.identity);
	}

	private void Connect(GameObject branch, GameObject root)
	{
		branch.GetComponent<HingeJoint>().connectedBody = root.GetComponent<Rigidbody>();
	}

}