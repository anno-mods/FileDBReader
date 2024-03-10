using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AnnoMods.BBDom.ObjectSerializer.SerializationHandlers
{
    internal class FlatArrayHandler : ISerializationHandler
    {
        public IEnumerable<BBNode> Handle(object? item, string tagName, IBBDocument workingDocument, BBSerializerOptions options)
        {
            var listInstance = item as IList;
            if (listInstance is null) 
                yield break;

            Type listContentType = listInstance.GetType().GetNullableType().GetGenericArguments().Single();

            foreach (var listEntry in listInstance)
            {
                var itemHandler = HandlerProvider.GetHandlerFor(listContentType);
                var created = itemHandler.Handle(listEntry, tagName, workingDocument, options);
                foreach(BBNode tag in created)
                {
                    yield return tag;
                }
            }
        }
    }
}
