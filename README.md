# FileDBReader

FileDB Unpacking done by @VeraAtVersus, based on filedb reverse engineering by @lysannschlegel

A simple command line unpacker, repacker and interpreter for Anno 1800 (and 2205?) filedb compression. Currently WIP.


when decompressing, all data will be represented in hex strings which can be interpreted using an interpreter file. This is done by selecting XML Nodes using xpath. After you are done editing the decompressed and interpreted xml file, you have to convert it to he using the same interpreter and compress it again.
 >you can also create xml patches and apply changes using meow's [XML Tools](https://github.com/xforce/anno1800-mod-loader/releases/tag/v0.7.12) which works like the modloader.

> be aware that these files can contain other filedb compressed files which you have to decompress while interpreting. 

How to use: 

```
decompress -f <inputfiles>
compress -f <inputFiles> -o <outputFileExtension>
interpret -f <inputFiles> -i <interpreterFile>
toHex -f <inputFiles> -i <interpreterFile>
```

included converters

> infotip file ````(data/infotips/export.bin)````

> island gamedata file ````(gamedata.data in a7m files)````

# Sample interpreter file

```xml
<Converts>
    <Default Type ="Int32" />
    <InternalCompression>
        <Element Path="//AreaManagerData/None/Data" />
    </InternalCompression>
    <Converts>
        <Convert Path ="//VegetationPropSetName" Type="String" Encoding="UTF-8" />
        <Convert Path ="//GlobalAmbientName" Type="String" />
        <Convert Path ="//HeightMap/HeightMap" Type="UInt16" Structure ="List" />
        <Convert Path ="//val" Type="Byte" Structure ="List" />
        <Convert Path ="//bits" Type="Byte" Structure ="List" />
        <Convert Path ="//PlayableArea" Type="Int32" Structure ="List" />
        <Convert Path ="//WorldSize" Type="Int32" Structure ="List" />
        <Convert Path ="//m_AreaRect/None" Type="Int32" Structure ="List" />
        <Convert Path ="//m_AreaPointGrid/None" Type="Int32" Structure ="List" />
        <Convert Path ="//ID" Type="Int64" />
        <Convert Path ="//Position" Type="Single" Structure="List" />
        <Convert Path ="//Orientation" Type="Single" Structure="List" />
        <Convert Path ="//TangentScale" Type="Single" />
        <Convert Path ="//BezierPath/Path/Minimum" Type="Single" Structure="List"/>
        <Convert Path ="//BezierPath/Path/Maximum" Type="Single" Structure="List"/>
        <Convert Path ="//BezierPath/Path/BezierCurve/None/*" Type="Single" Structure="List"/>
        <Convert Path ="//None[guid]/Direction" Type="Single"/>
        <Convert Path ="//None[guid]/Mesh/Scale" Type="Single"/>
        <Convert Path ="//Sequence/Name" Type="String" Encoding ="UTF-8"/>
        <Convert Path ="//AllKeyframes/None/None/*[self::Value or self::TangentIn or self::TangentOut]" Type="Single"/>
        <Convert Path ="//View/*" Type="Single" Structure="List"/>
        <Convert Path ="//Projection/NearClip" Type="Single"/>
        <Convert Path ="//BreakableFirstLastTangents" Type="Byte" Structure="List"/>
        <Convert Path ="//HerdAreas/None/RandomPoint" Type="Int32" Structure="List"/>
        <Convert Path ="//HerdAreas/None/DangerPoints/None" Type="Int32" Structure="List"/>
        <Convert Path ="//BBox" Type="Int32" Structure="List"/>
        <Convert Path ="//ObjectGroups/None[not(GameObjects)]" Type="String" Encoding="UTF-8"/>
        <Convert Path ="//GameObjects/None" Type="Int32" Structure ="List"/>
    </Converts>
</Converts>
```

Internal Compression Args
- Path: Path that contains the filedb file

Convert Args
- Path: Xpath that selects nodes to be converted. 
- Type: primitive as it occurs in [.NET system](https://docs.microsoft.com/de-de/dotnet/csharp/language-reference/builtin-types/built-in-types)
- Encoding: Encoding as in [.NET encoding](https://docs.microsoft.com/de-de/dotnet/api/system.text.encoding?view=net-5.0)

> the same can be used for Default. 


# How to access island files

Island files consist of

- an rda v2.2 archive containing two gamedata.data and rd3d.data that are both in filedb compression. you can access those with the RDA explorer and also use it as a repacker. 
- chunk-meshes (.tmc) in filedb compression 
- an .a7minfo file in filedb compression 
- an .a7me file that is in xml. 










