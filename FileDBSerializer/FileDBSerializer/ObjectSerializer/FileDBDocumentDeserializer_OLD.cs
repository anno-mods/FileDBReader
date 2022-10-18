using FileDBSerializing.EncodingAwareStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FileDBSerializing.ObjectSerializer
{
    public class FileDBDocumentDeserializer_OLD<T> where T : class, new()
    {
        private FileDBSerializerOptions Options;
        private T TargetObject;
        private Type TargetType;

        public FileDBDocumentDeserializer_OLD(FileDBSerializerOptions options)
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
            //get OnSerialized Method of parentObject
            MethodInfo? on_serialized = parentObject.GetType().GetMethod("OnSerialized", new Type[0]);
            if (on_serialized is not null) on_serialized?.Invoke(parentObject, new object[0]);
        }

        private void BuildProperty(FileDBNode node, object parentObject)
        {
            String PropertyName = node.GetName();

            //try to find the property this needs to go for
            Type parentType = parentObject!.GetType();
            PropertyInfo? propertyInfo = parentType.GetProperty(PropertyName);

            if (propertyInfo is null)
            {
                if (Options.IgnoreMissingProperties) return;
                throw new FileDBSerializationException($"Property not found: {PropertyName}");
            }

            Type PropertyType = propertyInfo.GetNullablePropertyType();

            if (PropertyType.IsArray())
                BuildArrayProperty(node, propertyInfo as PropertyInfo, parentObject);
            else
                BuildSingleValue(node, propertyInfo as PropertyInfo, parentObject);
        }

        #region SingleValue_Parent
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
        #endregion

        #region Array_Parent
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ParentType"></typeparam>
        /// <param name="node">The node to construct the array from</param>
        /// <param name="PropertyInfo"></param>
        /// <param name="parentObject"></param>
        private void BuildArrayProperty(FileDBNode node, PropertyInfo PropertyInfo, object parentObject)
        {
            Type ArrayContentType = PropertyInfo.GetNullablePropertyType().GetElementType()!;
            Array? ArrayInstance = null;

            if (node is Attrib attrib)
                ArrayInstance = BuildArrayProperty_Attrib(ArrayContentType, attrib, PropertyInfo);
            else if (node is Tag tag)
                ArrayInstance = BuildArrayProperty_Tag(ArrayContentType, tag, PropertyInfo);
            else
                //we should not get here.
                throw new InvalidOperationException($"PropertyType {PropertyInfo.PropertyType.Name} could not be resolved to a FileDB document element.");

            if (ArrayInstance is not null) PropertyInfo.SetArray(parentObject, ArrayInstance);
        }

        private Array BuildArrayProperty_Attrib(Type ArrayContentType, Attrib attrib, PropertyInfo PropertyInfo)
        {
            if (ArrayContentType.IsPrimitiveType())
                return BuildPrimitiveArray(attrib.Content, ArrayContentType);

            else if (ArrayContentType.IsStringType() /* flat strings are Attribs */)
            {
                IEnumerable<FileDBNode> collection = attrib.Siblings.Where(s => s.ID == attrib.ID);
                return BuildStringArray(collection, ArrayContentType);
            }
            else if (ArrayContentType.IsPrimitiveListType())
            {
                IEnumerable<FileDBNode> collection = attrib.Siblings.Where(s => s.ID == attrib.ID);
                return BuildPrimitiveListArray(collection, ArrayContentType);
            }
            throw new InvalidOperationException($"PropertyType {PropertyInfo.PropertyType.Name} could not be resolved to a FileDB document element.");
        }

        private Array? BuildArrayProperty_Tag(Type ArrayContentType, Tag tag, PropertyInfo PropertyInfo)
        {
            bool isFlat = PropertyInfo.HasAttribute<FlatArrayAttribute>();
            IEnumerable<FileDBNode> collection = isFlat ? tag.Siblings.Where(s => s.ID == tag.ID) : tag.Children;

            if (ArrayContentType.IsStringType() /* non-flat strings are Tags */)
                return BuildStringArray(collection, ArrayContentType);
            else if (ArrayContentType.IsPrimitiveListType() /* non-flat primitive lists are Tags */)
                return BuildPrimitiveListArray(collection, ArrayContentType);
            else
                return BuildReferenceArray(collection, ArrayContentType, PropertyInfo.GetCustomAttributes<PolymorphAttribute>());
        }
        #endregion

        #region Array_Construction

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
                var ArrayEntry = PrimitiveTypeConverter.GetObject(ContentType, SpanSlice);
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

        private Array BuildStringArray(IEnumerable<FileDBNode> nodes, Type targetType)
        {
            return BuildMultiNodeArray(nodes,
                node =>
                {
                    if (node is not Attrib attrib)
                        throw new FileDBSerializationException($"Array entry must be Attrib: {node.GetName()}");
                    return InstanceString(targetType, attrib.Content);
                },
                targetType
            );
        }

        private Array BuildPrimitiveListArray(IEnumerable<FileDBNode> nodes, Type targetType)
        {
            Type? contentTargetType = targetType.GetElementType();
            if (contentTargetType is null)
                throw new FileDBSerializationException($"Array entry type must be Array: {nodes.FirstOrDefault()?.GetName()}");

            return BuildMultiNodeArray(nodes,
                node =>
                {
                    if (node is not Attrib attrib)
                        throw new FileDBSerializationException($"Array entry must be Attrib: {node.GetName()}");
                    return BuildPrimitiveArray(attrib.Content, contentTargetType);
                },
                targetType
            );
        }

        private Array BuildReferenceArray(IEnumerable<FileDBNode> Nodes, Type TargetType, IEnumerable<PolymorphAttribute> polymorphAttributes)
        {
            return BuildMultiNodeArray(Nodes, MakeInstance, TargetType);

            object MakeInstance(FileDBNode node)
            { 
                if (node is Tag tag)
                {
                    Type? polyTarget = polymorphAttributes.FirstOrDefault((x) => x.IsApplicable(tag))?.TargetType;
                    return MakeInstanceFromTag(tag, polyTarget ?? TargetType);
                }
                throw new FileDBSerializationException($"Cannot create Array Entry from Attrib: {node.GetName()}");
            }
        }
        #endregion

        #region Node_Construction

        private object MakeInstanceFromTag(Tag Tag, Type PropertyType)
        {
            if (PropertyType.IsPrimitiveType()) throw new FileDBSerializationException("Cannot instantiate primitive from tag");
            try
            {
                //this is for a single reference instance
                object? PropertyInstance = Activator.CreateInstance(PropertyType);

                if (PropertyInstance is null) throw new FileDBSerializationException($"Could not create instance of Type {PropertyType.Name}. Missing a parameterless constructor.");

                //deserialize the children into the property instance
                DeserializeNodeCollection(Tag.Children, PropertyInstance);
                return PropertyInstance;
            }
            catch (Exception e)
            {
                throw new FileDBSerializationException($"Could not create instance of Type {PropertyType.Name}. Missing a parameterless constructor.", e);
            }
        }

        private void BuildSinglePropertyFromTag(Tag tag, object parentObject, PropertyInfo Property)
        {
            Type PropertyType = Property.GetNullablePropertyType();
            Type? polyTarget = Property.GetCustomAttributes<PolymorphAttribute>().FirstOrDefault((x) => x.IsApplicable(tag))?.TargetType;
            object PropertyInstance = MakeInstanceFromTag(tag, polyTarget ?? PropertyType);
            //set the value
            Property.SetValue(parentObject, PropertyInstance);
        }

        private void BuildSinglePropertyFromAttrib(Attrib attrib, object parentObject, PropertyInfo Property)
        {
            //target type
            var PropertyType = Property.GetNullablePropertyType();

            object? PropertyInstance = null;

            if (PropertyType.IsPrimitiveType())
                PropertyInstance = PrimitiveTypeConverter.GetObject(PropertyType, attrib.Content);
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

        #endregion
    }
}
