using UnityEngine;

namespace Game.GameStage1
{
    public class Room
    {
        public RectInt rect;
        public Room partitionA;
        public Room partitionB;
        public Room parents;

        public Vector2Int center2Int => new Vector2Int(rect.x + rect.width / 2, rect.y + rect.height / 2);
        public Vector2 center => new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f);

        public Room(RectInt rect)
        {
            this.rect = rect;
        }

        public void PartitionRoom(int split)
        {
            if (rect.width >= rect.height)
            {
                partitionA = new Room(new RectInt(rect.x, rect.y, split, rect.height));
                partitionB = new Room(new RectInt(rect.x + split, rect.y, rect.width - split, rect.height));
            }
            else
            {
                partitionA = new Room(new RectInt(rect.x, rect.y, rect.width, split));
                partitionB = new Room(new RectInt(rect.x, rect.y + split, rect.width, rect.height - split));
            }

            partitionA.parents = this;
            partitionB.parents = this;
        }
    }
}