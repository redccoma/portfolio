using UnityEngine;

namespace Game.GameStage1
{
    public class Node
    {
        public Node leftNode;
        public Node rightNode;
        public Node parentNode;
        public RectInt nodeRect;
        public Node(RectInt rect)
        {
            this.nodeRect = rect;
        }
    }    
}