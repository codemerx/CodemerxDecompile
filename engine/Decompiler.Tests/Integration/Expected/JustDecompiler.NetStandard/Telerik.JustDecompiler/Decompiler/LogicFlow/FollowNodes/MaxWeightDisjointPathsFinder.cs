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
			base();
			this.graph = bipartiteGraph;
			this.size = this.graph.GetLength(0);
			this.u = new Int32[this.size];
			this.v = new Int32[this.size];
			this.excessMatrix = new int[this.size, this.size];
			this.equalityGraph = new List<int>[this.size * 2];
			V_0 = 0;
			while (V_0 < this.size * 2)
			{
				this.equalityGraph[V_0] = new List<int>();
				V_0 = V_0 + 1;
			}
			this.pair = new Int32[this.size * 2 + 1];
			this.dist = new Int32[this.size * 2 + 1];
			this.nilVertex = this.size * 2;
			return;
		}

		private List<KeyValuePair<int, int>> FindMaximumCardianlityMatching()
		{
			V_1 = 0;
			while (V_1 < (int)this.pair.Length)
			{
				this.pair[V_1] = this.nilVertex;
				V_1 = V_1 + 1;
			}
			while (this.HopcroftKarpBFS())
			{
				V_2 = 0;
				while (V_2 < this.size)
				{
					if (this.pair[V_2] == this.nilVertex)
					{
						dummyVar0 = this.HopcroftKarpDFS(V_2);
					}
					V_2 = V_2 + 1;
				}
			}
			V_0 = new List<KeyValuePair<int, int>>();
			V_3 = 0;
			while (V_3 < this.size)
			{
				if (this.pair[V_3] != this.nilVertex)
				{
					V_0.Add(new KeyValuePair<int, int>(V_3, this.pair[V_3]));
				}
				V_3 = V_3 + 1;
			}
			return V_0;
		}

		private void GenerateExcessMatrixAndEqualitySubgraph()
		{
			V_0 = 0;
			while (V_0 < this.size * 2)
			{
				this.equalityGraph[V_0].Clear();
				V_0 = V_0 + 1;
			}
			V_1 = 0;
			while (V_1 < this.size)
			{
				V_2 = 0;
				while (V_2 < this.size)
				{
					this.excessMatrix[V_1, V_2] = this.u[V_1] + this.v[V_2] - this.graph[V_1, V_2];
					if (this.excessMatrix[V_1, V_2] == 0)
					{
						this.equalityGraph[V_1].Add(V_2 + this.size);
						this.equalityGraph[V_2 + this.size].Add(V_1);
					}
					V_2 = V_2 + 1;
				}
				V_1 = V_1 + 1;
			}
			return;
		}

		private int GetMinElement(bool[] restriction)
		{
			V_0 = 0x3b9aca00;
			V_1 = 0;
			while (V_1 < this.size)
			{
				if (!restriction[V_1])
				{
					V_2 = 0;
					while (V_2 < this.size)
					{
						if (!restriction[V_2 + this.size] && this.excessMatrix[V_1, V_2] < V_0)
						{
							V_0 = this.excessMatrix[V_1, V_2];
						}
						V_2 = V_2 + 1;
					}
				}
				V_1 = V_1 + 1;
			}
			return V_0;
		}

		public static List<KeyValuePair<int, int>> GetOptimalEdgesInDAG(int[,] adjacencyMatrix)
		{
			return (new MaxWeightDisjointPathsFinder(adjacencyMatrix)).MaximumWeightBipartiteGraphMatching();
		}

		private bool HopcroftKarpBFS()
		{
			V_0 = new Queue<int>();
			V_1 = 0;
			while (V_1 < this.size)
			{
				if (this.pair[V_1] != this.nilVertex)
				{
					this.dist[V_1] = 0x3b9aca00;
				}
				else
				{
					this.dist[V_1] = 0;
					V_0.Enqueue(V_1);
				}
				V_1 = V_1 + 1;
			}
			this.dist[this.nilVertex] = 0x3b9aca00;
			while (V_0.get_Count() > 0)
			{
				V_2 = V_0.Dequeue();
				if (V_2 == this.nilVertex)
				{
					continue;
				}
				V_3 = this.equalityGraph[V_2].GetEnumerator();
				try
				{
					while (V_3.MoveNext())
					{
						V_4 = V_3.get_Current();
						if (this.dist[this.pair[V_4]] != 0x3b9aca00)
						{
							continue;
						}
						this.dist[this.pair[V_4]] = this.dist[V_2] + 1;
						V_0.Enqueue(this.pair[V_4]);
					}
				}
				finally
				{
					((IDisposable)V_3).Dispose();
				}
			}
			return this.dist[this.nilVertex] != 0x3b9aca00;
		}

		private bool HopcroftKarpDFS(int vertex)
		{
			if (vertex == this.nilVertex)
			{
				return true;
			}
			V_0 = this.equalityGraph[vertex].GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (this.dist[this.pair[V_1]] != this.dist[vertex] + 1 || !this.HopcroftKarpDFS(this.pair[V_1]))
					{
						continue;
					}
					this.pair[V_1] = vertex;
					this.pair[vertex] = V_1;
					V_2 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			this.dist[vertex] = 0x3b9aca00;
			return false;
		}

		private void Initialization()
		{
			V_0 = 0;
			while (V_0 < this.size)
			{
				V_1 = -1;
				V_2 = 0;
				while (V_2 < this.size)
				{
					if (this.graph[V_0, V_2] > V_1)
					{
						V_1 = this.graph[V_0, V_2];
					}
					V_2 = V_2 + 1;
				}
				this.u[V_0] = V_1;
				V_0 = V_0 + 1;
			}
			return;
		}

		private void KonigDFS(int vertex, bool[] traversed)
		{
			traversed[vertex] = true;
			V_0 = 0;
			while (V_0 < this.equalityGraph[vertex].get_Count())
			{
				if (!traversed[this.equalityGraph[vertex].get_Item(V_0)])
				{
					this.KonigDFS(this.equalityGraph[vertex].get_Item(V_0), traversed);
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private List<KeyValuePair<int, int>> MaximumWeightBipartiteGraphMatching()
		{
			this.Initialization();
			while (true)
			{
				this.GenerateExcessMatrixAndEqualitySubgraph();
				V_0 = this.FindMaximumCardianlityMatching();
				if (V_0.get_Count() == this.size)
				{
					break;
				}
				V_1 = this.MinimumVertexCover(V_0);
				V_2 = this.GetMinElement(V_1);
				V_6 = 0;
				while (V_6 < this.size)
				{
					if (!V_1[V_6])
					{
						stackVariable40 = &this.u[V_6];
						stackVariable40 = stackVariable40 - V_2;
					}
					if (V_1[V_6 + this.size])
					{
						stackVariable33 = &this.v[V_6];
						stackVariable33 = stackVariable33 + V_2;
					}
					V_6 = V_6 + 1;
				}
			}
			V_3 = new List<KeyValuePair<int, int>>();
			V_4 = V_0.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if (this.graph[V_5.get_Key(), V_5.get_Value() - this.size] == 0)
					{
						continue;
					}
					V_3.Add(new KeyValuePair<int, int>(V_5.get_Key(), V_5.get_Value() - this.size));
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			return V_3;
		}

		private bool[] MinimumVertexCover(List<KeyValuePair<int, int>> maxCardinalMatching)
		{
			V_0 = new Boolean[this.size];
			V_1 = new Boolean[this.size * 2];
			V_3 = maxCardinalMatching.GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					V_5 = V_4.get_Key();
					V_6 = V_4.get_Value();
					V_0[V_5] = true;
					dummyVar0 = this.equalityGraph[V_5].Remove(V_6);
					this.equalityGraph[V_6] = new List<int>();
					this.equalityGraph[V_6].Add(V_5);
				}
			}
			finally
			{
				((IDisposable)V_3).Dispose();
			}
			V_7 = 0;
			while (V_7 < this.size)
			{
				if (!V_0[V_7])
				{
					this.KonigDFS(V_7, V_1);
				}
				V_7 = V_7 + 1;
			}
			V_2 = new Boolean[this.size * 2];
			V_8 = 0;
			while (V_8 < this.size)
			{
				V_2[V_8] = !V_1[V_8];
				V_8 = V_8 + 1;
			}
			V_9 = 0;
			while (V_9 < this.size)
			{
				V_2[V_9 + this.size] = V_1[V_9 + this.size];
				V_9 = V_9 + 1;
			}
			return V_2;
		}
	}
}