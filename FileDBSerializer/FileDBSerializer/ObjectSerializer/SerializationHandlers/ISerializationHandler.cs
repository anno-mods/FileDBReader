﻿using FileDBSerializing;
using FileDBSerializing.ObjectSerializer;
using System.Collections.Generic;
using System.Reflection;

namespace FileDBSerializer.ObjectSerializer.SerializationHandlers
{
    public interface ISerializationHandler
    {
        public IEnumerable<FileDBNode> Handle(object? item, string tagName, IFileDBDocument workingDocument, FileDBSerializerOptions options);
    }
}
