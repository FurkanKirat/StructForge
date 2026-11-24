using StructForge.Collections;

namespace StructForge.Enumerators
{
    /// <summary>
    /// A zero-alloc, stack-allocated enumerator for Depth-First Search.
    /// </summary>
    public ref struct SfDfsEnumerator<T>
    {
        private readonly SfGraph<T> _graph;
        private readonly SfStack<T> _stack;
        private readonly SfHashSet<T> _visited;
        private T _current;

        internal SfDfsEnumerator(SfGraph<T> graph, T start)
        {
            _graph = graph;
            _stack = new SfStack<T>(32); 
            _visited = new SfHashSet<T>(graph.Count, graph.Comparer);
            
            _stack.Push(start);
            _current = default;
        }

        public readonly T Current => _current;

        public bool MoveNext()
        {
            while (_stack.Count > 0)
            {
                T node = _stack.Pop();

                if (!_visited.TryAdd(node))
                    continue;

                _current = node;
                
                var neighbors = _graph.GetNeighbors(node);
                foreach (var edge in neighbors)
                {
                    _stack.Push(edge.Target);
                }

                return true;
            }

            return false;
        }

        public void Dispose() { }
    }
    
    /// <summary>
    /// A zero-alloc, stack-allocated enumerator for Breadth-First Search.
    /// </summary>
    public ref struct SfBfsEnumerator<T>
    {
        private readonly SfGraph<T> _graph;
        private readonly SfQueue<T> _queue;
        private readonly SfHashSet<T> _visited;
        private T _current;

        internal SfBfsEnumerator(SfGraph<T> graph, T start)
        {
            _graph = graph;
            _queue = new SfQueue<T>(32);
            _visited = new SfHashSet<T>(graph.Count, graph.Comparer);

            _queue.Enqueue(start);
            _visited.Add(start);
            _current = default;
        }

        public readonly T Current => _current;

        public bool MoveNext()
        {
            if (_queue.Count == 0)
                return false;

            _current = _queue.Dequeue();

            var neighbors = _graph.GetNeighbors(_current);
            foreach (var edge in neighbors)
            {
                if (_visited.TryAdd(edge.Target))
                {
                    _queue.Enqueue(edge.Target);
                }
            }

            return true;
        }

        public void Dispose() { }
    }
    
    public readonly ref struct SfDfsEnumerable<T>
    {
        private readonly SfGraph<T> _graph;
        private readonly T _start;

        internal SfDfsEnumerable(SfGraph<T> graph, T start)
        {
            _graph = graph;
            _start = start;
        }

        public SfDfsEnumerator<T> GetEnumerator()
        {
            return new SfDfsEnumerator<T>(_graph, _start);
        }
        
        public SfList<T> ToList() 
        {
            var list = new SfList<T>();
            foreach(var item in this) list.Add(item);
            return list;
        }
    
        public int Count()
        {
            int c = 0;
            foreach(var item in this) c++;
            return c;
        }
    }

    public readonly ref struct SfBfsEnumerable<T>
    {
        private readonly SfGraph<T> _graph;
        private readonly T _start;

        internal SfBfsEnumerable(SfGraph<T> graph, T start)
        {
            _graph = graph;
            _start = start;
        }

        public SfBfsEnumerator<T> GetEnumerator()
        {
            return new SfBfsEnumerator<T>(_graph, _start);
        }
        
        public SfList<T> ToList() 
        {
            var list = new SfList<T>();
            foreach(var item in this) list.Add(item);
            return list;
        }
    
        public int Count()
        {
            int c = 0;
            foreach(var item in this) c++;
            return c;
        }
    }
}