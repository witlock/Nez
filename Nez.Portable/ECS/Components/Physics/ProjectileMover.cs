using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Nez
{
	/// <summary>
	/// moves taking collision into account only for reporting to any ITriggerListeners. The object will always move the full amount so it is up
	/// to the caller to destroy it on impact if desired.
	/// </summary>
	public class ProjectileMover : Component
	{

        List<ITriggerListener> _tempTriggerList = new List<ITriggerListener>();
        List<Collider> _colliders = new List<Collider>();

        public override void OnAddedToEntity()
        {
            _colliders = Entity.GetComponents<Collider>();
            Debug.WarnIf(_colliders.Count == 0, "ProjectileMover has no Collider. ProjectilMover requires a Collider!");
        }


        /// <summary>
		/// moves the entity taking collisions into account
		/// </summary>
		/// <returns><c>true</c>, if move actor was newed, <c>false</c> otherwise.</returns>
		/// <param name="motion">Motion.</param>
		public bool Move(Vector2 motion)
		{
			if (_colliders.Count == 0)
				return false;

			var didCollide = false;

			// fetch anything that we might collide with at our new position
			Entity.Transform.Position += motion;

            foreach (var collider in _colliders)
            {
                // fetch anything that we might collide with us at our new position
                var neighbors = Physics.BoxcastBroadphase(collider.Bounds, collider.CollidesWithLayers);
                foreach (var neighbor in neighbors)
                {
                    if (collider.Overlaps(neighbor))
                    {
                        didCollide = true;
                        NotifyTriggerListeners(collider, neighbor);
                    }
                }
            }

			return didCollide;
		}

        void NotifyTriggerListeners(Collider self, Collider other)
        {
            // notify any listeners on the Entity of the Collider that we overlapped
            other.Entity.GetComponents(_tempTriggerList);
            for (var i = 0; i < _tempTriggerList.Count; i++)
                _tempTriggerList[i].OnTriggerEnter(self, other);

            _tempTriggerList.Clear();

            // notify any listeners on this Entity
            Entity.GetComponents(_tempTriggerList);
            for (var i = 0; i < _tempTriggerList.Count; i++)
                _tempTriggerList[i].OnTriggerEnter(other, self);

            _tempTriggerList.Clear();
        }
    }

}