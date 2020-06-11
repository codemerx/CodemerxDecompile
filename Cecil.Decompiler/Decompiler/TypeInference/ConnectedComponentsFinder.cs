using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
    class ConnectedComponentsFinder
    {
        ///Implements Gabow's algorithm for finding connected components.
        ///From <see cref="http://en.wikipedia.org/wiki/Cheriyan%E2%80%93Mehlhorn/Gabow_algorithm"/>:
        ///
        ///The algorithm performs a depth-first search of the given graph G, maintaining as it does two stacks S and P. Stack S contains 
        ///all the vertices that have not yet been assigned to a strongly connected component, in the order in which 
        ///the depth-first search reaches the vertices. Stack P contains vertices that have not yet been determined to belong to different 
        ///strongly connected components from each other. It also uses a counter C of the number of vertices reached so far, which it uses to compute 
        ///the preorder numbers of the vertices.
        ///When the depth-first search reaches a vertex v, the algorithm performs the following steps:
        ///1. Set the preorder number of v to C, and increment C.
        ///2. Push v onto S and also onto P.
        ///3. For each edge from v to a neighboring vertex w:
        ///     - If the preorder number of w has not yet been assigned, recursively search w;
        ///     - Otherwise, if w has not yet been assigned to a strongly connected component:
        ///         * Repeatedly pop vertices from P until the top element of P has a preorder number less than or equal to the preorder number of w.
        ///4. If v is the top element of P:
        ///     - Pop vertices from S until v has been popped, and assign the popped vertices to a new component.
        ///     - Pop v from P.
        ///The overall algorithm consists of a loop through the vertices of the graph, calling this recursive search on each vertex that does not yet have a preorder number assigned to it.
        
        private int preorderNumber;
        private int componentCount;
        private readonly Dictionary<ClassHierarchyNode, int> used;
        private readonly Stack<ClassHierarchyNode> s;
        private readonly Stack<ClassHierarchyNode> p;
        private readonly Dictionary<ClassHierarchyNode, int> nodeToComponent;
        private readonly ICollection<ClassHierarchyNode> inferenceGraph;

        public ConnectedComponentsFinder(ICollection<ClassHierarchyNode> inferenceGraph)
        {
            this.inferenceGraph = inferenceGraph;

            this.used = new Dictionary<ClassHierarchyNode, int>();
            this.s = new Stack<ClassHierarchyNode>();
            this.p = new Stack<ClassHierarchyNode>();
            this.nodeToComponent = new Dictionary<ClassHierarchyNode, int>();
            this.componentCount = 0;
            this.preorderNumber = 0;
        }

        /// <summary>
        /// The entry point of the method.
        /// </summary>
        /// <returns>Returns enumeration of all connected components. A single connected component is represented as collection of the nodes it contains.</returns>
        public IEnumerable<ICollection<ClassHierarchyNode>> GetConnectedComponents()
        {
            ClassHierarchyNode startNode;
            do
            {
                startNode = null;
                foreach (ClassHierarchyNode x in inferenceGraph)
                {
                    if (!used.ContainsKey(x))
                    {
                        startNode = x;
                        break;
                    }
                }
                if (startNode != null)
                {
                    RecursiveDfs(startNode);
                }
            } while (startNode != null);


            ICollection<ClassHierarchyNode>[] result = new ICollection<ClassHierarchyNode>[componentCount];

            foreach (ClassHierarchyNode node in nodeToComponent.Keys)
            {
                int component = nodeToComponent[node];
                if (result[component] == null)
                {
                    result[component] = new List<ClassHierarchyNode>();
                }
                ((List<ClassHierarchyNode>)result[component]).Add(node);
            }
            
            return result;
        }

        /// <summary>
        /// Realises the steps from 1 to 4 as described above.
        /// </summary>
        /// <param name="node">The node which connected component is to be found.</param>
        private void RecursiveDfs(ClassHierarchyNode node)
        {
            preorderNumber++;
            used.Add(node, preorderNumber);
            s.Push(node);
            p.Push(node);
            foreach (ClassHierarchyNode supernode in node.CanAssignTo)
            {
                if (!used.ContainsKey(supernode))
                {
                    RecursiveDfs(supernode);
                }
                else
                {
                    if (!nodeToComponent.ContainsKey(supernode))
                    {
                        int supernodeNumber = used[supernode];
                        while (supernodeNumber < used[p.Peek()])
                        {
                            p.Pop();
                        }
                    }
                }
            }
            if (p.Peek() == node)
            {
                while (s.Peek() != node)
                {
                    ClassHierarchyNode componentMember = s.Pop();
                    nodeToComponent.Add(componentMember, componentCount);
                }
                nodeToComponent.Add(p.Pop(), componentCount);
                s.Pop();
                componentCount++;
            }
        }
    }
}
