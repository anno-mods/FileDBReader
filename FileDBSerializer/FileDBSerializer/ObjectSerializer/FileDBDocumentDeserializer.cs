using FileDBSerializing.EncodingAwareStrings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileDBSerializing.ObjectSerializer
{
    public class FileDBDocumentDeserializer<T> where T : class, new()
    {
        private FileDBSerializerOptions Options;
        private PrimitiveTypeConverter PrimitiveConverter = new PrimitiveTypeConverter();
        private T TargetObject;
        private Type TargetType;

        public FileDBDocumentDeserializer(FileDBSerializerOptions options)
        {
            Options = options;
            TargetObject = new T();
            TargetType = TargetObject.GetType();
        }

        public T GetObjectStructureFromFileDBDocument(IFileDBDocument doc)
        {
            IEnumerable<PropertyInfo> Properties = TargetType.GetProperties();
            IEnumerable<FileDBNode> NodeCollection = doc.Roots;

            DeserializeNodeCollection(NodeCollection, TargetObject);

            return TargetObject;
        }

        private void DeserializeNodeCollection(IEnumerable<FileDBNode> nodes, object parentObject)
        {
            foreach (FileDBNode node in nodes)
            {
                BuildProperty(node, parentObject);
            }
        }

        private void BuildProperty(FileDBNode node, object parentObject)
        {
            String PropertyName = node.GetName();

            //try to find the property this needs to go for
            Type parentType = parentObject!.GetType();
            PropertyInfo? propertyInfo = parentType.GetProperty(PropertyName);
            if (propertyInfo is null) throw new FileDBSerializationException($"Property not found: {PropertyName}");

            Type PropertyType = propertyInfo.PropertyType;

            if (PropertyType.IsArray())
                BuildArrayProperty(node, propertyInfo as PropertyInfo, parentObject);
            else
                BuildSingleValue(node, propertyInfo as PropertyInfo, parentObject);
        }

        private void BuildSingleValue(FileDBNode node, PropertyInfo PropertyInfo, object parentObject)
        {
            //convert node.Content and set the value accordingly.
            if (node is Attrib)
            {
                //single value 
                BuildSinglePropertyFromAttrib((Attrib)node, parentObject, PropertyInfo);
            }
            //worry about the children if we have a tag, we wont need to set anything now!
            else if (node is Tag)
            {
                BuildSinglePropertyFromTag((Tag)node, parentObject, PropertyInfo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ParentType"></typeparam>
        /// <param name="node">The node to construct the array from</param>
        /// <param name="PropertyInfo"></param>
        /// <param name="parentObject"></param>
        private void BuildArrayProperty(FileDBNode node, PropertyInfo PropertyInfo, object parentObject)
        {
            Type ArrayContentType = PropertyInfo.PropertyType.GetElementType()!;

            Array? ArrayInstance = null;
            if (node is Attrib) ArrayInstance = BuildPrimitiveArray(((Attrib)node).Content, ArrayContentType);

            else if (node is Tag)
            {
                Tag Tag = (Tag)node;
                ArrayInstance = ArrayContentType.IsStringType() ? BuildStringArray(Tag.Children, ArrayContentType) : BuildReferenceArray(Tag.Children, ArrayContentType);
            }

            if (ArrayInstance is not null) PropertyInfo.SetArray(parentObject, ArrayInstance);
        }

        private Array BuildPrimitiveArray(byte[] content, Type ContentType)
        {
            int contentSize = Marshal.SizeOf(ContentType);
            if (content.Length % contentSize != 0) throw new FileDBSerializationException($"Failed to create array because of not matching bytesizes: Content Bytesize: {content.Length}, Bytesize of Array Element: {contentSize}");

            var ArrayLength = content.Length / contentSize;
            var ArrayInstance = Array.CreateInstance(ContentType, ArrayLength);

            ReadOnlySpan<byte> ContentSpan = new ReadOnlySpan<byte>(content);
            for (int i = 0; i < ArrayLength; i++)
            {
                byte[] SpanSlice = ContentSpan.Slice(i * contentSize, contentSize).ToArray();
                var ArrayEntry = PrimitiveConverter.GetObject(ContentType, SpanSlice);
                ArrayInstance.SetValue(ArrayEntry, i);
            }

            return ArrayInstance;
        }

        private Array BuildMultiNodeArray(IEnumerable<FileDBNode> Nodes, Func<FileDBNode, object?> ChildCreationFunction, Type ArrayContentType)
        {
            int ArrayLength = Nodes.Count();

            Array ArrayInstance = Array.CreateInstance(ArrayContentType, ArrayLength);
            for (int i = 0; i < ArrayLength; i++)
            {
                object? ArrayEntry = ChildCreationFunction.Invoke(Nodes.ElementAt(i));
                ArrayInstance.SetValue(ArrayEntry, i);
            }
            return ArrayInstance;
        }

        private Array BuildStringArray(IEnumerable<FileDBNode> Nodes, Type TargetType)
        {
            return BuildMultiNodeArray(Nodes,
                node => {
                    if (node is Tag) throw new FileDBSerializationException($"Cannot create Array Entry from Tag: {node.GetName()}");
                    return InstanceString(TargetType, ((Attrib)node).Content);
                },
                TargetType
            );
        }

        private Array BuildReferenceArray(IEnumerable<FileDBNode> Nodes, Type TargetType)
        {
            return BuildMultiNodeArray(Nodes, MakeInstance, TargetType);

            object MakeInstance(FileDBNode node)
            {
                if (node is Attrib) throw new FileDBSerializationException($"Cannot create Array Entry from Attrib: {node.GetName()}");
                return MakeInstanceFromTag((Tag)node, TargetType);
            }
        }

        private object MakeInstanceFromTag(Tag Tag, Type PropertyType)
        {
            if (PropertyType.IsPrimitiveType()) throw new FileDBSerializationException("Cannot instantiate primitive from tag");
            try
            {
                //this is for a single reference instance
                object? PropertyInstance = Activator.CreateInstance(PropertyType);

                if (PropertyInstance is null) throw new FileDBSerializationException($"Could not create instance of Type {PropertyType.Name}");

                //deserialize the children into the property instance
                DeserializeNodeCollection(Tag.Children, PropertyInstance);
                return PropertyInstance;
            }
            catch (Exception e)
            {
                throw new FileDBSerializationException($"Could not create instance of Type {PropertyType.Name}", e);
            }
        }

        private void BuildSinglePropertyFromTag(Tag tag, object parentObject, PropertyInfo Property)
        {
            Type PropertyType = Property.PropertyType;
            object PropertyInstance = MakeInstanceFromTag(tag, PropertyType);
            //set the value
            Property.SetValue(parentObject, PropertyInstance);
        }

        private void BuildSinglePropertyFromAttrib(Attrib attrib, object parentObject, PropertyInfo Property)
        {
            //target type
            var PropertyType = Property.PropertyType;

            object? PropertyInstance = null;

            if (PropertyType.IsPrimitiveType())
                PropertyInstance = PrimitiveConverter.GetObject(PropertyType, attrib.Content);
            else if (PropertyType.IsStringType())
                PropertyInstance = InstanceString(PropertyType, attrib.Content);

            Property.SetValue(parentObject, PropertyInstance);
        }

        private object? InstanceString(Type PropertyType, byte[] StringBytes)
        {
            //special strings
            if (PropertyType.IsSubclassOf(typeof(EncodingAwareString)))
            {
                return Activator.CreateInstance(PropertyType, StringBytes);
            }
            else if (PropertyType.Equals(typeof(String)))
            {
                return Options.DefaultEncoding.GetString(StringBytes);
            }
            return null;
        }
    }
}
