# FileDBReader

[![example workflow](https://github.com/anno-mods/FileDBReader/actions/workflows/main.yml/badge.svg)](https://github.com/anno-mods/FileDBReader/actions/workflows/main.yml)

A simple command line unpacker, repacker and interpreter for proprietary Anno 1800 compression. Currently Work in Progress.

- Decompressing: all data will be represented in hex strings which can be interpreted with an interpreter file. 

- Interpreting: This tool uses Xpath to select xmlNodes in the decompressed documents and converts them. 

After you are done editing the decompressed and interpreted xml file, you have to convert it to he using the same interpreter and compress it again.

> Tip: you can also create xml patches and apply changes using meow's [xmltest](https://github.com/xforce/anno1800-mod-loader/releases/latest/download/xmltest.zip) which works like the modloader.
 
Credits: First version of FileDB unpacking done by @VeraAtVersus, based on reverse engineering by @lysannschlegel

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
decompress -f <inputfiles> -c <CompressionVersion> -i <interpreterFile>
compress -f <inputFiles> -o <outputFileExtension> -c <CompressionVersion> -i <interpreterFile>
interpret -f <inputFiles> -i <interpreterFile>
toHex -f <inputFiles> -i <interpreterFile>
check_fileversion -f <inputfiles>
fctohex -f <inputfiles> -i <interpreterFile>
hextofc -f <inputfiles> -i <interpreterFile>
```

> Note that i is optional on decompress, compress, fctohex and hextofc verbs. If provided, the program will directly convert from compressed to interpreted / from interpreted to recompressed.

included converters

- a7tinfo
- ctt
- fc Files (2205 & 1800)
- fc Files (1404 & 2070)
- infotip 
- a7s island archives: gamedata.data
- a7s island archives: RD3D.data
- a7t map: gamedata.data
- tmc
- rdp

# Compression Versions

There are two versions of this compression
 
- Version 1 is what you find in files up to Anno 1800, GU 12 (31.08.2021) -> [documentation](https://github.com/lysannschlegel/RDAExplorer/wiki/file.db-format)

- Version 2 is used for new or updated files after this date.

> The compressor can autodetect versions while decompressing. Alternativly, you can use the check_fileversion verb.

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
        <Convert Path ="//GuidVariationList" Type = "Int32" Structure="Cdata">
        <Convert Path="//MapTemplate/TemplateElement/Element/Size" Type="Int16">
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
- Structure: Can be either Default, List or Cdata. 
- Enum: Define your own mapping for IDs as seen in the sample interpreter

> Type, Encoding and Structure can also be used for Default. 

# Binary Data in xml-based Anno files
Some xml-based file formats, especially in Anno 1701/1404/2070, do not use filedb compression, but they still have binary parts: 

```XML
<binary>CDATA[<bytesize><content>]</binary>
```

This tool can also convert those parts into:
```XML
<binary>CDATA[<hex_representation_of_content>]</binary>
```
Which in turn can be interpreted using the default interpreter steps. CDATA nodes must be marked in the interpreter with 

```XML
<Convert Structure = "Cdata">
```

## Invalid XML
Anno accepts </> as an xml closing tag. While reading, any of these closing tags are autocorrected, 
and since Anno also understands the valid xml syntax, you can use them ingame right away. 

# File format explanation (will be moved to wiki soon)

## What you typically may find in Anno 1800 fileformats
- zlib Compression 
- FileDB Compression (this tool)
- RDA archives (can be accessed using [RDA explorer by Lysann Schlegel](https://github.com/lysannschlegel/RDAExplorer))


## Island files

For a detailed explaination, have a look at the [Wiki](https://github.com/anno-mods/FileDBReader/wiki/How-Island-Files-work)

Island files consist of
- an rda v2.2 archive containing two gamedata.data and rd3d.data that are both in filedb compression.
- chunk-meshes (.tmc) in filedb compression 
- an .a7minfo file in filedb compression 
- an .a7me file that is in xml. 
- a .ctt file that contains normalmaps in multiple resolutions. This one is a zlib compressed filedb file. (compression level 1) 

## Map Templates 

Map Template files consist of
- an rda v2.2 archive containing gamedata.data that is in filedb compression. 
- an .a7te file that is plain xml 
- an .a7tinfo file that is in filedb compression. 

# Future plans 
- FileDB Serialization library (coming soon! )

also on the list:
- full support for zlib compressed files
- automated conversion routines
- support for rda archives (maybe)






