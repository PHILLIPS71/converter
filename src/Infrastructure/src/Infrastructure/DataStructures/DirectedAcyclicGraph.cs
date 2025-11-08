namespace Giantnodes.Infrastructure;

/// <summary>
/// Represents a directed acyclic graph (DAG) data structure.
/// </summary>
/// <typeparam name="T">The type of data stored in the graph nodes.</typeparam>
public class DirectedAcyclicGraph<T> where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> _adjacencies;

    public DirectedAcyclicGraph()
    {
        _adjacencies = [];
    }

    /// <summary>
    /// Gets the number of nodes in the graph.
    /// </summary>
    public int NodeCount => _adjacencies.Count;

    /// <summary>
    /// Gets the number of edges in the graph.
    /// </summary>
    public int EdgeCount => _adjacencies.Sum(kvp => kvp.Value.Count);

    /// <summary>
    /// Gets all nodes in the graph.
    /// </summary>
    public IEnumerable<T> Nodes => _adjacencies.Keys;

    /// <summary>
    /// Adds a node to the graph if it doesn't already exist.
    /// </summary>
    /// <param name="node">The node to add.</param>
    /// <returns>True if the node was added; false if it already existed.</returns>
    public bool AddNode(T node)
    {
        if (_adjacencies.ContainsKey(node))
            return false;

        _adjacencies[node] = [];
        return true;
    }

    /// <summary>
    /// Adds an edge from the source node to the target node.
    /// </summary>
    /// <param name="source">The source node.</param>
    /// <param name="target">The target node.</param>
    /// <returns>True if the edge was added; false if adding the edge would create a cycle or if the edge already exists.</returns>
    public bool AddEdge(T source, T target)
    {
        AddNode(source);
        AddNode(target);

        // Check if the edge already exists
        if (_adjacencies[source].Contains(target))
            return false;

        if (CreatesCycle(source, target))
            return false;

        _adjacencies[source].Add(target);
        return true;
    }

    /// <summary>
    /// Removes a node and all its associated edges from the graph.
    /// </summary>
    /// <param name="node">The node to remove.</param>
    /// <returns>True if the node was found and removed; otherwise, false.</returns>
    public bool RemoveNode(T node)
    {
        if (!_adjacencies.ContainsKey(node))
            return false;

        // Remove all edges where this node is the target
        foreach (var sourceNode in _adjacencies.Keys)
        {
            _adjacencies[sourceNode].Remove(node);
        }

        // Remove the node and its outgoing edges
        _adjacencies.Remove(node);
        return true;
    }

    /// <summary>
    /// Removes an edge from the graph.
    /// </summary>
    /// <param name="source">The source node.</param>
    /// <param name="target">The target node.</param>
    /// <returns>True if the edge was found and removed; otherwise, false.</returns>
    public bool RemoveEdge(T source, T target)
    {
        if (!_adjacencies.TryGetValue(source, out var adjacency))
            return false;

        return adjacency.Remove(target);
    }

    /// <summary>
    /// Gets all nodes that have an outgoing edge to the specified target node (parents).
    /// </summary>
    /// <param name="node">The target node.</param>
    /// <returns>An enumerable collection of nodes that have an edge to the target node.</returns>
    public IEnumerable<T> GetParents(T node)
    {
        if (!_adjacencies.ContainsKey(node))
            yield break;

        foreach (var parent in _adjacencies.Keys)
        {
            if (_adjacencies[parent].Contains(node))
                yield return parent;
        }
    }

    /// <summary>
    /// Gets all nodes that have an incoming edge from the specified source node (children).
    /// </summary>
    /// <param name="node">The source node.</param>
    /// <returns>An enumerable collection of nodes that have an edge from the source node.</returns>
    public IEnumerable<T> GetChildren(T node)
    {
        if (!_adjacencies.TryGetValue(node, out var adjacency))
            yield break;

        foreach (var child in adjacency)
            yield return child;
    }

    /// <summary>
    /// Gets all root nodes in the graph (nodes with no incoming edges).
    /// </summary>
    /// <returns>An enumerable collection of root nodes.</returns>
    public IEnumerable<T> GetRoots()
    {
        var dependents = new HashSet<T>();

        // Collect all nodes that have incoming edges
        foreach (var node in _adjacencies.Keys)
        {
            foreach (var child in _adjacencies[node])
                dependents.Add(child);
        }

        // Return nodes that don't have incoming edges
        foreach (var node in _adjacencies.Keys)
        {
            if (!dependents.Contains(node))
                yield return node;
        }
    }

    /// <summary>
    /// Gets all leaf nodes in the graph (nodes with no outgoing edges).
    /// </summary>
    /// <returns>An enumerable collection of leaf nodes.</returns>
    public IEnumerable<T> GetLeaves()
    {
        foreach (var node in _adjacencies.Keys)
        {
            if (_adjacencies[node].Count == 0)
                yield return node;
        }
    }

    /// <summary>
    /// Checks if the graph is empty.
    /// </summary>
    /// <returns>True if the graph contains no nodes; otherwise, false.</returns>
    public bool IsEmpty()
    {
        return _adjacencies.Count == 0;
    }

    /// <summary>
    /// Clears all nodes and edges from the graph.
    /// </summary>
    public void Clear()
    {
        _adjacencies.Clear();
    }

    /// <summary>
    /// Performs a topological sort of the graph.
    /// </summary>
    /// <returns>An enumerable collection of nodes in topological order.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the graph contains a cycle.</exception>
    public IEnumerable<T> Sort()
    {
        var inDegree = new Dictionary<T, int>();

        // Initialize in-degree for all nodes to 0
        foreach (var node in _adjacencies.Keys)
            inDegree[node] = 0;

        // Calculate in-degree for each node
        foreach (var source in _adjacencies.Keys)
        {
            foreach (var target in _adjacencies[source])
                inDegree[target] = inDegree.TryGetValue(target, out var degree) ? degree + 1 : 1;
        }

        // Add all nodes with in-degree 0 to the queue
        var queue = new Queue<T>();
        foreach (var node in _adjacencies.Keys)
        {
            if (inDegree.TryGetValue(node, out var degree) && degree == 0)
                queue.Enqueue(node);
        }

        var result = new List<T>(_adjacencies.Count);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);

            // Reduce in-degree of all neighbors
            if (!_adjacencies.TryGetValue(current, out var adjacency))
                continue;

            foreach (var child in adjacency)
            {
                inDegree[child]--;

                // If in-degree becomes 0, add to the queue
                if (inDegree[child] == 0)
                    queue.Enqueue(child);
            }
        }

        // If all nodes are not included in the result, there must be a cycle
        if (result.Count != _adjacencies.Count)
            throw new InvalidOperationException("the graph contains a cycle and cannot be topologically sorted.");

        return result;
    }

    /// <summary>
    /// Determines whether adding an edge from source to target would create a cycle.
    /// </summary>
    /// <param name="source">The source node.</param>
    /// <param name="target">The target node.</param>
    /// <returns>True if adding the edge would create a cycle; otherwise, false.</returns>
    private bool CreatesCycle(T source, T target)
    {
        // If source and target are the same, it would create a self-loop
        if (EqualityComparer<T>.Default.Equals(source, target))
            return true;

        // Check if there's a path from target back to source (which would create a cycle)
        var visited = new HashSet<T>();
        return HasPath(target, source, visited);
    }

    /// <summary>
    /// Determines whether there is a path from source to target in the graph using DFS.
    /// </summary>
    /// <param name="current">The current node being examined.</param>
    /// <param name="target">The target node to find.</param>
    /// <param name="visited">Set of already visited nodes.</param>
    /// <returns>True if there is a path from current to target; otherwise, false.</returns>
    private bool HasPath(T current, T target, HashSet<T> visited)
    {
        if (EqualityComparer<T>.Default.Equals(current, target))
            return true;

        if (!visited.Add(current))
            return false;

        if (!_adjacencies.TryGetValue(current, out var adjacency))
            return false;

        foreach (var neighbor in adjacency)
        {
            if (HasPath(neighbor, target, visited))
                return true;
        }

        return false;
    }
}
