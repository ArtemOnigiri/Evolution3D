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
	public GameObject cubePrefab;
	public GameObject foodPrefab;
	public float generationTime = 120f;
	public bool crossover = true;

	private List<Creature> creatures = new List<Creature>();
	private string date;

	void Start()
	{
		// spawn in grid
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				creatures.Add(new Creature(headPrefab, cubePrefab, foodPrefab, (i - 10 / 2) * 50, (j - 10 / 2) * 50));
			}
		}
		// spawn in line
		/*for (int i = 0; i < population; i++)
		{
			creatures.Add(new Creature(headPrefab, cubePrefab, foodPrefab, (i - population / 2) * 20, 0));
		}*/
		date = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
	}

	void FixedUpdate()
	{
		for (int i = 0; i < population; i++)
		{
			creatures[i].Update();
		}
		if(time > generationTime)
		{
			creatures.Sort((a, b) => b.GetFitness().CompareTo(a.GetFitness()));
			float avgFitness = 0f;
			for (int i = 0; i < population; i++)
			{
				avgFitness += creatures[i].GetFitness();
			}
			avgFitness /= population;
			using (StreamWriter w = File.AppendText("stats" + date + ".txt"))
			{
				w.WriteLine((int)creatures[0].GetFitness() + " " + (int)avgFitness);
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
				if(crossover)
				{
					creatures[i].nn = new NN(bestNNs[Random.Range(0, best)], bestNNs[Random.Range(0, best)], 0.5f);
				}
				else
				{
					creatures[i].nn = new NN(bestNNs[Random.Range(0, best)]);
				}
			}
			time = 0;
			generation++;
			if(generation == 200)
			{
				for (int i = 0; i < population; i++)
				{
					creatures[i].nn = new NN(creatures[i].segments.Count);
				}
				generation = 0;
				using (StreamWriter w = File.AppendText("stats" + date + ".txt"))
				{
					w.WriteLine("=====");
				}
			}
		}
		time += Time.deltaTime;
	}

}