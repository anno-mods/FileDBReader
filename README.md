# FileDBReader

FileDB Unpacking done by @VeraAtVersus, based on filedb reverse engineering by @lysannschlegel

A simple command line unpacker, repacker and interpreter for Anno 1800 (and 2205?) filedb compression. Currently WIP.


when decompressing, all data will be represented in hex strings which can be interpreted using an interpreter file. This is done by selecting XML Nodes using xpath. After you are done editing the decompressed and interpreted xml file, you have to convert it to he using the same interpreter and compress it again.
 >you can also create xml patches and apply changes using meow's [XML Tools](https://github.com/xforce/anno1800-mod-loader/releases/tag/v0.7.12) which works like the modloader.

# Internal Compression 
Be aware that filedb compressed files can contain other filedb compressed files which you have to decompress while interpreting. When decompressing, the xml node structure always looks like this: 

```xml
<None>
    <ByteCount />
    <Data />
</None>
```

> When using the internal decompression in your interpreter file, ByteCount is automatically overwritten. 

# How to use

```
decompress -f <inputfiles> -c <CompressionVersion>
compress -f <inputFiles> -o <outputFileExtension> -c <CompressionVersion>
interpret -f <inputFiles> -i <interpreterFile>
toHex -f <inputFiles> -i <interpreterFile>
decompress_interpret -f <inputfiles> -i <interpreterFile> -c <CompressionVersion>
recompress_export -f <inputfiles> -i <interpreterFile> -o <outputFileExtension> -c <CompressionVersion>
check_fileversion -f <inputfiles>
```

included converters

> infotip file ````(data/infotips/export.bin)````

> Island Gamedata 

> Island Rd3d 

> Maptemplate 

> a7minfo

# Compression Versions

There are two versions of this compression
 
> Version 1 is what you find in files up to Anno 1800, GU 12 (31.08.2021)

> Version 2 is used for new or updated files after this date. 

# Sample interpreter file

```xml
<Converts>
    <Default Type ="Int32" />
    <InternalCompression>
        <Element Path="//AreaManagerData/None/Data" CompressionVersion = "2"/>
    </InternalCompression>
    <Converts>
        <Convert Path ="//VegetationPropSetName" Type="String" Encoding="UTF-8" />
        <Convert Path ="//GlobalAmbientName" Type="String" />
        <Convert Path ="//HeightMap/HeightMap" Type="UInt16" Structure ="List" />

        <Convert Path="//MapTemplate/TemplateElement/Element/Size" Type="Int16" UseEnum ="True">
            <!-- This Enum will map the converted value 0 to Small, 1 to Medium and 2 to Large-->
            <Enum>
                <Entry Value ="0" Name ="Small" />
                <Entry Value ="1" Name ="Medium" />
                <Entry Value ="2" Name ="Large" />
            </Enum>
        </Convert>
    </Converts>
</Converts>
```

Internal Compression Args
- Path: Path that contains the filedb file
- CompressionVersion: Compression Version of the inner file

Convert Args
- Path: Xpath that selects nodes to be converted. 
- Type: primitive as it occurs in [.NET system](https://docs.microsoft.com/de-de/dotnet/csharp/language-reference/builtin-types/built-in-types)
- Encoding: Encoding as in [.NET encoding](https://docs.microsoft.com/de-de/dotnet/api/system.text.encoding?view=net-5.0)
- UseEnum: Boolean Value that specifies wether values of the conversion should be mapped to the a specified enum. This only works on single nodes.

> the same can be used for Default. 

# What you typically may find in Anno 1800 fileformats
- zlib Compression
- FileDB Compression
- RDA archives (can be accessed using [RDA explorer by Lysann Schlegel](https://github.com/lysannschlegel/RDAExplorer))

# Island files

For a detailed explaination, have a look at the [Wiki](https://github.com/anno-mods/FileDBReader/wiki/How-Island-Files-work)

Island files consist of
- an rda v2.2 archive containing two gamedata.data and rd3d.data that are both in filedb compression.
- chunk-meshes (.tmc) in filedb compression 
- an .a7minfo file in filedb compression 
- an .a7me file that is in xml. 
- a .ctt file that contains normalmaps in multiple resolutions. This one is a zlib compressed filedb file. (compression level 1) 

# Map Templates 

Map Template files consist of
- an rda v2.2 archive containing gamedata.data that is in filedb compression. 
- an .a7te file that is plain xml 
- an .a7tinfo file that is in filedb compression. 

# Future plans 
- full support for zlib compressed files
- automated conversion routines
- support for rda archives (maybe)






