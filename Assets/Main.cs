using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Main : MonoBehaviour
{
	public static int population = 100;
	public static int best = 10;
	public static float time = 0f;
	public static int generation = 0;

	public GameObject headPrefab;
	public GameObject tailPrefab;
	public float generationTime = 120f;

	private List<Creature> creatures = new List<Creature>();
	private string date;

	// Start is called before the first frame update
	void Start()
	{
		Application.targetFrameRate = 300;
		for (int i = 0; i < population; i++)
		{
			creatures.Add(new Creature(headPrefab, tailPrefab, (i - population / 2) * 15, 0));
		}
		date = System.DateTime.Now.ToString("yyyyMMddHHmmss");
	}

	// Update is called once per frame
	void Update()
	{
		for (int i = 0; i < population; i++)
		{
			creatures[i].Update();
		}
		if(time > generationTime)
		{
			creatures.Sort((a, b) => b.GetFitness().CompareTo(a.GetFitness()));
			using (StreamWriter w = File.AppendText("stats" + date + ".txt"))
			{
				w.WriteLine((int)creatures[0].GetFitness());
			}
			NN[] bestNNs = new NN[best];
			for (int i = 0; i < best; i++)
			{
				bestNNs[i] = creatures[i].nn;
				if(Random.value < 0.1) bestNNs[i].Mutate(0.5f, 0.4f);
				else bestNNs[i].Mutate(0.01f, 0.1f);
			}
			for (int i = 0; i < population; i++)
			{
				creatures[i].CreateSegments();
				creatures[i].nn = new NN(bestNNs[Random.Range(0, best)]);
			}
			time = 0;
			generation++;
		}
		time += Time.deltaTime;
	}

}