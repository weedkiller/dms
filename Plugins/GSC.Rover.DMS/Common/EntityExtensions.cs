﻿using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSC.Rover.DMS.BusinessLogic.Common
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Checks if entity attribute exists gets its value of an entity. returns default value if attribute does not exist
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attribute"></param>
        /// <returns>Guid</returns>
        public static Guid GetEntityReferenceGuid(this Entity entity, string attribute)
        {
            if (entity.Attributes.Contains(attribute))
            {
                return entity.GetAttributeValue<EntityReference>(attribute).Id;
            }

            return default(Guid);
        }

        /// <summary>
        /// Checks if entity attribute exists gets its value of an entity. returns default value if attribute does not exist
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attribute"></param>
        /// <returns>T</returns>
        public static T GetEntityAttributeValueSafely<T>(this Entity entity, string attribute)
        {
            if (entity.Attributes.Contains(attribute))
            {
                return entity.GetAttributeValue<T>(attribute);
            }
            return default(T);
        }


        /// <summary>
        /// Checks if entity attribute exists gets its value of an option set. returns 0 if attribute does not exist
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attribute"></param>
        /// <returns>int</returns>
        public static int GetEntityOptionSetValueSafely(this Entity entity, string attribute)
        {
            if (entity.Attributes.Contains(attribute))
            {
                return entity.GetAttributeValue<OptionSetValue>(attribute).Value;
            }
            return 0;
        }

        /// <summary>
        /// Clones entity attribute collection from one to another and removes unecessary keys
        /// </summary>
        /// <param name="source"></param>
        /// <param name="attributes"></param>
        /// <param name="destination"></param>
        /// <returns>Entity</returns>
        public static void MapEntityAttributes(Entity source,  Entity destination, string[] attributes = null)
        {
            Entity sourceEntity = source;

            foreach (var item in source.Attributes)
            {
                if (attributes.Contains(item.Key))
                {
                    source.Attributes.Remove(item.Key);
                }
            }

            destination.Attributes = sourceEntity.Attributes;
            //if (entity.Attributes.Contains(attribute))
            //{
            //    return entity.GetAttributeValue<OptionSetValue>(attribute).Value;
            //}
           
        }
    }
}
