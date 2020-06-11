using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.FollowNodes
{
    internal class MaxWeightDisjointPathsFinder
    {
        private const int INFINITY = 1000000000;

        private readonly int[,] graph;
        private readonly int size;
        private readonly int[] u;
        private readonly int[] v;
        private readonly int[,] excessMatrix;
        private readonly List<int>[] equalityGraph;

        /// <summary>
        /// Finds the optimal vertex-disjoint path cover of maximum total weight of the given directed acyclic graph.
        /// </summary>
        /// <remarks>
        /// Let G = (V, E) be the graph represented by the specified <paramref name="adjacencyMatrix"/>, with weight function - W((vi, vj)) = adjacencyMatrix[i, j].
        /// Let Gb = (X, Y, Eb) is a bipartite graph where:
        /// X = { xi | vi from V }, Y = { yi | vi from V }
        /// Eb = { (xi, yj) | (vi, vj) from E }
        /// Wb((xi, yj)) = W((vi, vj)) for each (vi, vj) from E
        /// 
        /// As stated in chapter 4 in "Approximation Algorithms for Covering a Graph by Vertex-Disjoint Paths of Maximum Total Weight":
        /// The problem of findig the vertex-disjoint path cover of maximum total weight of G,
        /// can be reduced to the problem of finding a maximum weight bipartite matching of Gb.
        /// Since G is a DAG then the produced cover will be optimal. (mentioned at end of chapter 4)
        /// 
        /// Implementation details:
        /// In the implementation of the algorithms below we often need a set of all of the nodes of the graph.
        /// So we define the set Z with the following properties:
        /// |V| = |X| = |Y| = n
        /// Z = { zi | xi from X } U { z(i + n) | yi from Y }
        /// </remarks>
        /// <param name="adjacencyMatrix"></param>
        /// <returns></returns>
        public static List<KeyValuePair<int, int>> GetOptimalEdgesInDAG(int[,] adjacencyMatrix)
        {
            MaxWeightDisjointPathsFinder pathsFinder = new MaxWeightDisjointPathsFinder(adjacencyMatrix);
            return pathsFinder.MaximumWeightBipartiteGraphMatching();
        }

        private MaxWeightDisjointPathsFinder(int[,] bipartiteGraph)
        {
            this.graph = bipartiteGraph;
            this.size = graph.GetLength(0);
            this.u = new int[size];
            this.v = new int[size];
            this.excessMatrix = new int[size, size];
            this.equalityGraph = new List<int>[size * 2];

            for (int i = 0; i < size * 2; i++)
            {
                this.equalityGraph[i] = new List<int>();
            }

            this.pair = new int[size * 2 + 1];
            this.dist = new int[size * 2 + 1];
            this.nilVertex = size * 2;
        }

        #region Hungarian algorithm
        /// <summary>
        /// Implementation of the hungarian algorithm for finding a maximum weight matching in a bipartite graph.
        /// </summary>
        /// <remarks>
        /// The implementation is taken from "Maximum Weight Matching in a Bipartite Graph"
        /// </remarks>
        /// <returns></returns>
        private List<KeyValuePair<int, int>> MaximumWeightBipartiteGraphMatching()
        {
            Initialization();

            while (true)
            {
                GenerateExcessMatrixAndEqualitySubgraph();
                List<KeyValuePair<int, int>> matching = FindMaximumCardianlityMatching();

                if (matching.Count == size)
                {
                    List<KeyValuePair<int, int>> maxPositiveWeightBipartiteMatching = new List<KeyValuePair<int, int>>();
                    foreach (KeyValuePair<int, int> edge in matching)
                    {
                        if (graph[edge.Key, edge.Value - size] != 0)
                        {
                            maxPositiveWeightBipartiteMatching.Add(new KeyValuePair<int, int>(edge.Key, edge.Value - size));
                        }
                    }

                    return maxPositiveWeightBipartiteMatching;
                }

                bool[] minVertexCoverMarkUp = MinimumVertexCover(matching);

                int e = GetMinElement(minVertexCoverMarkUp);

                for (int i = 0; i < size; i++)
                {
                    if (!minVertexCoverMarkUp[i])
                    {
                        u[i] -= e;
                    }

                    if (minVertexCoverMarkUp[i + size])
                    {
                        v[i] += e;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the structures used by the Hungarian algorithm.
        /// </summary>
        private void Initialization()
        {
            for (int i = 0; i < size; i++)
            {
                int maxElement = -1;
                for (int j = 0; j < size; j++)
                {
                    if (graph[i, j] > maxElement)
                    {
                        maxElement = graph[i, j];
                    }
                }

                u[i] = maxElement;
            }
        }

        /// <summary>
        /// Generates the excess matrix and the equality subgraph used by the Hungarian algorithm.
        /// </summary>
        private void GenerateExcessMatrixAndEqualitySubgraph()
        {
            for (int i = 0; i < size * 2; i++)
            {
                equalityGraph[i].Clear();
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    excessMatrix[i, j] = u[i] + v[j] - graph[i, j];
                    if (excessMatrix[i, j] == 0)
                    {
                        equalityGraph[i].Add(j + size);
                        equalityGraph[j + size].Add(i);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the minimum element from the excess matrix.
        /// </summary>
        /// <remarks>
        /// Skips the check for vertices with indices that are marked in the <paramref name="restriction"/> array.
        /// </remarks>
        /// <param name="restriction"></param>
        /// <returns></returns>
        private int GetMinElement(bool[] restriction)
        {
            int min = INFINITY;
            for (int i = 0; i < size; i++)
            {
                if (restriction[i])
                {
                    continue;
                }

                for (int j = 0; j < size; j++)
                {
                    if (restriction[j + size])
                    {
                        continue;
                    }

                    if (excessMatrix[i, j] < min)
                    {
                        min = excessMatrix[i, j];
                    }
                }
            }

            return min;
        }
        #endregion

        #region Hopcroft–Karp algorithm
        private readonly int[] pair;
        private readonly int[] dist;
        private readonly int nilVertex; //NIL == vertex with index - 2 * size.
        
        /// <summary>
        /// Finds the maximum cardinality matching in the equality subgraph of Gb, by using the Hopcroft–Karp algorithm.
        /// </summary>
        /// <remarks>
        /// The implementation of the algorithm is from the pseudo code in the article in wikipedia:
        /// http://en.wikipedia.org/wiki/Hopcroft%E2%80%93Karp_algorithm
        /// </remarks>
        /// <returns></returns>
        private List<KeyValuePair<int, int>> FindMaximumCardianlityMatching()
        {
            for (int i = 0; i < pair.Length; i++)
            {
                pair[i] = nilVertex;
            }

            while (HopcroftKarpBFS())
            {
                for (int i = 0; i < size; i++)
                {
                    if (pair[i] == nilVertex)
                    {
                        HopcroftKarpDFS(i);
                    }
                }
            }

            List<KeyValuePair<int, int>> matching = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < size; i++)
            {
                if (pair[i] != nilVertex)
                {
                    matching.Add(new KeyValuePair<int, int>(i, pair[i]));
                }
            }

            return matching;
        }

        /// <summary>
        /// The modified BFS used by the Hopcroft–Karp algorithm.
        /// </summary>
        /// <returns></returns>
        private bool HopcroftKarpBFS()
        {
            Queue<int> bfsQueue = new Queue<int>();

            for (int i = 0; i < size; i++)
            {
                if (pair[i] == nilVertex)
                {
                    dist[i] = 0;
                    bfsQueue.Enqueue(i);
                }
                else
                {
                    dist[i] = INFINITY;
                }
            }

            dist[nilVertex] = INFINITY;
            while (bfsQueue.Count > 0)
            {
                int currentVertex = bfsQueue.Dequeue();
                if (currentVertex == nilVertex)
                {
                    continue;
                }
                foreach (int neighbour in equalityGraph[currentVertex])
                {
                    if (dist[pair[neighbour]] == INFINITY)
                    {
                        dist[pair[neighbour]] = dist[currentVertex] + 1;
                        bfsQueue.Enqueue(pair[neighbour]);
                    }
                }
            }

            return dist[nilVertex] != INFINITY;
        }

        /// <summary>
        /// The modified DFS used by the Hopcroft–Karp algorithm.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private bool HopcroftKarpDFS(int vertex)
        {
            if (vertex != nilVertex)
            {
                foreach (int neighbour in equalityGraph[vertex])
                {
                    if (dist[pair[neighbour]] == dist[vertex] + 1 && HopcroftKarpDFS(pair[neighbour]))
                    {
                        pair[neighbour] = vertex;
                        pair[vertex] = neighbour;
                        return true;
                    }
                }
                dist[vertex] = INFINITY;
                return false;
            }

            return true;
        }
        #endregion

        #region König's theorem
        /// <summary>
        /// Finds the minimum vertex cover from the maximum cardinality matching of the equality subgraph of Gb, by using König's theorem
        /// </summary>
        /// <remarks>
        /// Modifies the equalityGraph collection.
        /// Uses the algorithm descibed by the article in wikipedia: "http://en.wikipedia.org/wiki/K%C3%B6nig%27s_theorem_(graph_theory)"
        /// </remarks>
        /// <param name="maxCardinalMatching"></param>
        /// <returns></returns>
        private bool[] MinimumVertexCover(List<KeyValuePair<int, int>> maxCardinalMatching)
        {
            bool[] notDFSStarts = new bool[size];
            bool[] konigT = new bool[size * 2]; //Represents the set T used in the algorithm.
            //We can now change the equality graph since it will not be used until the next iteration of the Hungarian algorithm.
            foreach (KeyValuePair<int, int> edge in maxCardinalMatching)
            {
                //We mark the start of each edge in the maximum cardinality matching and we reverse the edge in the equality subgraph.
                int start = edge.Key;
                int end = edge.Value;

                notDFSStarts[start] = true;

                equalityGraph[start].Remove(end);

                equalityGraph[end] = new List<int>();
                equalityGraph[end].Add(start);
            }

            for (int i = 0; i < size; i++)
            {
                //Start the DFS from all the nodes in L that are not in the maximum cardinal matching.
                if (!notDFSStarts[i])
                {
                    KonigDFS(i, konigT);
                }
            }

            bool[] result = new bool[size * 2];

            for (int i = 0; i < size; i++)
            {
                result[i] = !konigT[i];
            }

            //result = L \ T

            for (int i = 0; i < size; i++)
            {
                result[i + size] = konigT[i + size];
            }

            //result = (L \ T) U (R n T) = minimum vertex cover
            return result;
        }

        /// <summary>
        /// The DFS used in the algorithm of König's theorem.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="traversed"></param>
        private void KonigDFS(int vertex, bool[] traversed)
        {
            traversed[vertex] = true;

            for (int i = 0; i < equalityGraph[vertex].Count; i++)
            {
                if (!traversed[equalityGraph[vertex][i]])
                {
                    KonigDFS(equalityGraph[vertex][i], traversed);
                }
            }
        }
        #endregion
    }
}
