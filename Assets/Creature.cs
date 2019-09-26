using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature
{
	private GameObject headPrefab;
	private GameObject cubePrefab;
	private float xOffset;
	private float zOffset;
	private List<GameObject> segments;

	public NN nn;

	public Creature(GameObject headPrefab, GameObject cubePrefab, float xOffset, float zOffset)
	{
		this.headPrefab = headPrefab;
		this.cubePrefab = cubePrefab;
		this.xOffset = xOffset;
		this.zOffset = zOffset;
		this.segments = new List<GameObject>();
		CreateSegments();
		nn = new NN(segments.Count);
	}

	public void Update()
	{
		nn.Update();
		for (int i = 1; i < segments.Count; i++)
		{
			JointSpring spring = segments[i].GetComponent<HingeJoint>().spring;
			spring.targetPosition = 30 * nn.neurons[nn.layers - 1, i];
			segments[i].GetComponent<HingeJoint>().spring = spring;
		}
	}

	public float GetFitness()
	{
		return segments[0].transform.position.z;
	}

	public void CreateSegments()
	{
		if(segments.Count > 0)
		{
			for (int i = 0; i < segments.Count; i++)
			{
				GameObject.Destroy(segments[i]);
			}
		}
		segments.Clear();
		GameObject head = GameObject.Instantiate(headPrefab, new Vector3(xOffset, 1, zOffset), Quaternion.identity);
		segments.Add(head);
		for (int i = 0; i < 8; i++)
		{
			GameObject newSegment = GameObject.Instantiate(cubePrefab, new Vector3(xOffset, 1, zOffset - i - 1), Quaternion.identity);
			Connect(newSegment, segments[i]);
			segments.Add(newSegment);
		}
	}

	private void Connect(GameObject branch, GameObject root)
	{
		branch.GetComponent<HingeJoint>().connectedBody = root.GetComponent<Rigidbody>();
	}

}