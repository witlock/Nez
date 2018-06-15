using Nez.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nez.ECS
{
    public class HeadlessScene : Scene
    {

        /// <summary>
        /// Stoes and manages all entity processors
        /// </summary>
        public readonly EntityProcessorList entityProcessors;

        public HeadlessScene()
        {
            //entities = new EntityList(this);
            //content = new NezContentManager();

            //if (Core.entitySystemsEnabled)
            //    entityProcessors = new EntityProcessorList();


            //initialize();
        }
    }
}
