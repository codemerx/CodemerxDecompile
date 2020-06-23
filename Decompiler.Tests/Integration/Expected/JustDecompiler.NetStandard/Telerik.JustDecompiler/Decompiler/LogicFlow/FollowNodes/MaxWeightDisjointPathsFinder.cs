using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.FollowNodes
{
	internal class MaxWeightDisjointPathsFinder
	{
		private const int INFINITY = 0x3b9aca00;

		private readonly int[,] graph;

		private readonly int size;

		private readonly int[] u;

		private readonly int[] v;

		private readonly int[,] excessMatrix;

		private readonly List<int>[] equalityGraph;

		private readonly int[] pair;

		private readonly int[] dist;

		private readonly int nilVertex;

		private MaxWeightDisjointPathsFinder(int[,] bipartiteGraph)
		{
			this.graph = bipartiteGraph;
			this.size = this.graph.GetLength(0);
			this.u = new Int32[this.size];
			this.v = new Int32[this.size];
			this.excessMatrix = new int[this.size, this.size];
			this.equalityGraph = new List<int>[this.size * 2];
			for (int i = 0; i < this.size * 2; i++)
			{
				this.equalityGraph[i] = new List<int>();
			}
			this.pair = new Int32[this.size * 2 + 1];
			this.dist = new Int32[this.size * 2 + 1];
			this.nilVertex = this.size * 2;
		}

		private List<KeyValuePair<int, int>> FindMaximumCardianlityMatching()
		{
			for (int i = 0; i < (int)this.pair.Length; i++)
			{
				this.pair[i] = this.nilVertex;
			}
			while (this.HopcroftKarpBFS())
			{
				for (int j = 0; j < this.size; j++)
				{
					if (this.pair[j] == this.nilVertex)
					{
						this.HopcroftKarpDFS(j);
					}
				}
			}
			List<KeyValuePair<int, int>> keyValuePairs = new List<KeyValuePair<int, int>>();
			for (int k = 0; k < this.size; k++)
			{
				if (this.pair[k] != this.nilVertex)
				{
					keyValuePairs.Add(new KeyValuePair<int, int>(k, this.pair[k]));
				}
			}
			return keyValuePairs;
		}

		private void GenerateExcessMatrixAndEqualitySubgraph()
		{
			for (int i = 0; i < this.size * 2; i++)
			{
				this.equalityGraph[i].Clear();
			}
			for (int j = 0; j < this.size; j++)
			{
				for (int k = 0; k < this.size; k++)
				{
					this.excessMatrix[j, k] = this.u[j] + this.v[k] - this.graph[j, k];
					if (this.excessMatrix[j, k] == 0)
					{
						this.equalityGraph[j].Add(k + this.size);
						this.equalityGraph[k + this.size].Add(j);
					}
				}
			}
		}

		private int GetMinElement(bool[] restriction)
		{
			int num = 0x3b9aca00;
			for (int i = 0; i < this.size; i++)
			{
				if (!restriction[i])
				{
					for (int j = 0; j < this.size; j++)
					{
						if (!restriction[j + this.size] && this.excessMatrix[i, j] < num)
						{
							num = this.excessMatrix[i, j];
						}
					}
				}
			}
			return num;
		}

		public static List<KeyValuePair<int, int>> GetOptimalEdgesInDAG(int[,] adjacencyMatrix)
		{
			return (new MaxWeightDisjointPathsFinder(adjacencyMatrix)).MaximumWeightBipartiteGraphMatching();
		}

		private bool HopcroftKarpBFS()
		{
			Queue<int> nums = new Queue<int>();
			for (int i = 0; i < this.size; i++)
			{
				if (this.pair[i] != this.nilVertex)
				{
					this.dist[i] = 0x3b9aca00;
				}
				else
				{
					this.dist[i] = 0;
					nums.Enqueue(i);
				}
			}
			this.dist[this.nilVertex] = 0x3b9aca00;
			while (nums.Count > 0)
			{
				int num = nums.Dequeue();
				if (num == this.nilVertex)
				{
					continue;
				}
				foreach (int num1 in this.equalityGraph[num])
				{
					if (this.dist[this.pair[num1]] != 0x3b9aca00)
					{
						continue;
					}
					this.dist[this.pair[num1]] = this.dist[num] + 1;
					nums.Enqueue(this.pair[num1]);
				}
			}
			return this.dist[this.nilVertex] != 0x3b9aca00;
		}

		private bool HopcroftKarpDFS(int vertex)
		{
			bool flag;
			if (vertex == this.nilVertex)
			{
				return true;
			}
			List<int>.Enumerator enumerator = this.equalityGraph[vertex].GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					int current = enumerator.Current;
					if (this.dist[this.pair[current]] != this.dist[vertex] + 1 || !this.HopcroftKarpDFS(this.pair[current]))
					{
						continue;
					}
					this.pair[current] = vertex;
					this.pair[vertex] = current;
					flag = true;
					return flag;
				}
				this.dist[vertex] = 0x3b9aca00;
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private void Initialization()
		{
			for (int i = 0; i < this.size; i++)
			{
				int num = -1;
				for (int j = 0; j < this.size; j++)
				{
					if (this.graph[i, j] > num)
					{
						num = this.graph[i, j];
					}
				}
				this.u[i] = num;
			}
		}

		private void KonigDFS(int vertex, bool[] traversed)
		{
			traversed[vertex] = true;
			for (int i = 0; i < this.equalityGraph[vertex].Count; i++)
			{
				if (!traversed[this.equalityGraph[vertex][i]])
				{
					this.KonigDFS(this.equalityGraph[vertex][i], traversed);
				}
			}
		}

		private List<KeyValuePair<int, int>> MaximumWeightBipartiteGraphMatching()
		{
			List<KeyValuePair<int, int>> keyValuePairs;
			this.Initialization();
			while (true)
			{
				this.GenerateExcessMatrixAndEqualitySubgraph();
				keyValuePairs = this.FindMaximumCardianlityMatching();
				if (keyValuePairs.Count == this.size)
				{
					break;
				}
				bool[] flagArray = this.MinimumVertexCover(keyValuePairs);
				int minElement = this.GetMinElement(flagArray);
				for (int i = 0; i < this.size; i++)
				{
					if (!flagArray[i])
					{
						this.u[i] -= minElement;
					}
					if (flagArray[i + this.size])
					{
						this.v[i] += minElement;
					}
				}
			}
			List<KeyValuePair<int, int>> keyValuePairs1 = new List<KeyValuePair<int, int>>();
			foreach (KeyValuePair<int, int> keyValuePair in keyValuePairs)
			{
				if (this.graph[keyValuePair.Key, keyValuePair.Value - this.size] == 0)
				{
					continue;
				}
				keyValuePairs1.Add(new KeyValuePair<int, int>(keyValuePair.Key, keyValuePair.Value - this.size));
			}
			return keyValuePairs1;
		}

		private bool[] MinimumVertexCover(List<KeyValuePair<int, int>> maxCardinalMatching)
		{
			bool[] flagArray = new Boolean[this.size];
			bool[] flagArray1 = new Boolean[this.size * 2];
			foreach (KeyValuePair<int, int> keyValuePair in maxCardinalMatching)
			{
				int key = keyValuePair.Key;
				int value = keyValuePair.Value;
				flagArray[key] = true;
				this.equalityGraph[key].Remove(value);
				this.equalityGraph[value] = new List<int>();
				this.equalityGraph[value].Add(key);
			}
			for (int i = 0; i < this.size; i++)
			{
				if (!flagArray[i])
				{
					this.KonigDFS(i, flagArray1);
				}
			}
			bool[] flagArray2 = new Boolean[this.size * 2];
			for (int j = 0; j < this.size; j++)
			{
				flagArray2[j] = !flagArray1[j];
			}
			for (int k = 0; k < this.size; k++)
			{
				flagArray2[k + this.size] = flagArray1[k + this.size];
			}
			return flagArray2;
		}
	}
}