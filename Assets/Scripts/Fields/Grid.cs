using System.Collections.Generic;
using UnityEngine;

namespace Fields
{
    public class Grid
    {
        private Node[,] m_Nodes;

        private int m_Width;
        private int m_Height;
        private float m_NodeSize;

        private Vector2Int m_StartCoordinate;
        private Vector2Int m_TargetCoordinate;
        private Vector3 m_Offset;

        private Node m_SelectedNode = null;

        private FlowFieldPathfinding m_Pathfinding;

        public int Width => m_Width;

        public int Height => m_Height;
        

        public Grid(int width, int height, Vector3 offset, float nodeSize, Vector2Int start, Vector2Int target)
        {
            m_Width = width;
            m_Height = height;

            m_StartCoordinate = start;
            m_TargetCoordinate = target;

            m_Offset = offset;
            m_NodeSize = nodeSize;
            

            m_Nodes = new Node[m_Width, m_Height];

            for (int i = 0; i < m_Nodes.GetLength(0); i++)
            {
                for (int j = 0; j < m_Nodes.GetLength(1); j++)
                {
                    m_Nodes[i, j] = new Node(offset + new Vector3(i + .5f,0f,j + .5f) * nodeSize);
                }
            }
            
            m_Pathfinding = new FlowFieldPathfinding(this, target, start);
            
            UpdatePathfinding();
        }

        public Node GetStartNode()
        {
            return GetNode(m_StartCoordinate);
        }
        
        public Node GetTargetNode()
        {
            return GetNode(m_TargetCoordinate);
        }

        public void SelectCoordinate(Vector2Int coordinate)
        {
            m_SelectedNode = GetNode(coordinate);
        }

        public void UnselectNode()
        {
            m_SelectedNode = null;
        }

        public bool HasSelectedNode()
        {
            return m_SelectedNode != null;
        }

        public Node GetSelectedNode()
        {
            return m_SelectedNode;
        }

        public Node GetNode(Vector2Int coordinate)
        {
            return GetNode(coordinate.x, coordinate.y);
        }
        
        public Node GetNodeAtPoint(Vector3 point)
        {
            Vector3 difference = point - m_Offset;

            int x = (int) (difference.x / m_NodeSize);
            int z = (int) (difference.z / m_NodeSize);
            
            return GetNode(x, z);
        }

       public List<Node> GetNodesInCircle(Vector3 point, float radius)
       {
           float sqrRadius = radius * radius; 

           List<Node> nodes = new List<Node>();
           foreach (Node node in EnumerateAllNodes())
           {
               if (node.Position.x < point.x - radius - m_NodeSize * .5f 
                   || node.Position.x > point.x + radius + m_NodeSize * .5f
                   || node.Position.z < point.z - radius - m_NodeSize * .5f
                   || node.Position.z > point.z + radius + m_NodeSize * .5f)
               {
                   continue;
               }
               
               if ((point - new Vector3(node.Position.x + m_NodeSize * .5f,0f,node.Position.z)).sqrMagnitude < sqrRadius 
                   || (point - new Vector3(node.Position.x - m_NodeSize * .5f,0f,node.Position.z)).sqrMagnitude < sqrRadius
                   || (point - new Vector3(node.Position.x,0f,node.Position.z + m_NodeSize * .5f)).sqrMagnitude < sqrRadius
                   || (point - new Vector3(node.Position.x,0f,node.Position.z - m_NodeSize * .5f)).sqrMagnitude < sqrRadius
                   || (point - new Vector3(node.Position.x + m_NodeSize * .5f,0f,node.Position.z + m_NodeSize * .5f)).sqrMagnitude < sqrRadius
                   || (point - new Vector3(node.Position.x + m_NodeSize * .5f,0f,node.Position.z - m_NodeSize * .5f)).sqrMagnitude < sqrRadius
                   || (point - new Vector3(node.Position.x - m_NodeSize * .5f,0f,node.Position.z + m_NodeSize * .5f)).sqrMagnitude < sqrRadius
                   || (point - new Vector3(node.Position.x - m_NodeSize * .5f,0f,node.Position.z - m_NodeSize * .5f)).sqrMagnitude < sqrRadius)
               {
                   nodes.Add(node);
               }
           }

           return nodes;
       }

        public Node GetNode(int i, int j)
        {
            if (i < 0 || i >= m_Width)
            {
                return null;
            }

            if (j < 0 || j >= m_Height)
            {
                return null;
            }
            
            return m_Nodes[i, j];
        }

        public IEnumerable<Node> EnumerateAllNodes()
        {
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    yield return GetNode(i, j);
                }
            }
        }

        public void UpdatePathfinding()
        {
            m_Pathfinding.UpdateField();
        }

        public void TryOccupyNode(Node node, bool occupy)
        {
            if (occupy)
            {
                node.IsOccupied = true;
            }
            UpdatePathfinding();
        }
        
        public bool CanOccupy(Node node)
        {
            Node currentNode = GetNode(m_StartCoordinate);

            if (node.m_OccupationAvailability == OccupationAvailability.CanOccupy)
            {
                return true;
            }
            
            if (node.m_OccupationAvailability == OccupationAvailability.CanNotOccupy)
            {
                return false;
            }
            
            if (node.m_OccupationAvailability == OccupationAvailability.Undefined)
            {
                node.IsOccupied = !node.IsOccupied;
                UpdatePathfinding();
                while (currentNode != GetNode(m_TargetCoordinate))
                {
                    if (currentNode.IsOccupied)
                    {
                        node.m_OccupationAvailability = OccupationAvailability.CanNotOccupy;
                        node.IsOccupied = !node.IsOccupied;
                        return false;
                    }
                    currentNode = currentNode.NextNode;
                }
                node.IsOccupied = !node.IsOccupied;
            }

            node.m_OccupationAvailability = OccupationAvailability.CanOccupy;
            return true;
        }
    }
}