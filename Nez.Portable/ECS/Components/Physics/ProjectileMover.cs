﻿using Microsoft.Xna.Framework;
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


        public override void onAddedToEntity()
        {
            _colliders = entity.getComponents<Collider>();
            Insist.isNotNull(_colliders, "null Collider. ProjectilMover requires a Collider!");
        }


        /// <summary>
        /// moves the entity taking collisions into account
        /// </summary>
        /// <returns><c>true</c>, if move actor was newed, <c>false</c> otherwise.</returns>
        /// <param name="motion">Motion.</param>
        public bool move(Vector2 motion)
        {
            var didCollide = false;

            // fetch anything that we might collide with at our new position
            entity.transform.position += motion;

            foreach (var collider in _colliders)
            {

                // fetch anything that we might collide with us at our new position
                var neighbors = Physics.boxcastBroadphase(collider.bounds, collider.collidesWithLayers);
                foreach (var neighbor in neighbors)
                {
                    if (collider.overlaps(neighbor))
                    {
                        didCollide = true;
                        notifyTriggerListeners(collider, neighbor);
                    }
                }

            }

            return didCollide;
        }


        void notifyTriggerListeners(Collider self, Collider other)
        {
            // notify any listeners on the Entity of the Collider that we overlapped
            other.entity.getComponents(_tempTriggerList);
            for (var i = 0; i < _tempTriggerList.Count; i++)
                _tempTriggerList[i].onTriggerEnter(self, other);

            _tempTriggerList.Clear();

            // notify any listeners on this Entity
            entity.getComponents(_tempTriggerList);
            for (var i = 0; i < _tempTriggerList.Count; i++)
                _tempTriggerList[i].onTriggerEnter(other, self);

            _tempTriggerList.Clear();
        }
    }
}