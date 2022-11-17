# FileDBReader

[![example workflow](https://github.com/anno-mods/FileDBReader/actions/workflows/main.yml/badge.svg)](https://github.com/anno-mods/FileDBReader/actions/workflows/main.yml)
[![Create Release](https://github.com/anno-mods/FileDBReader/actions/workflows/release.yml/badge.svg)](https://github.com/anno-mods/FileDBReader/actions/workflows/release.yml)

A simple command line unpacker, repacker and interpreter for proprietary Anno 1800 compression.

- Decompressing: all data will be represented in hex strings which can be interpreted with an interpreter file. 

- Interpreting: This tool uses Xpath to select xmlNodes in the decompressed documents and converts them. 

After you are done editing the decompressed and interpreted xml file, you have to convert it to he using the same interpreter and compress it again.

> Tip: you can also create xml patches and apply changes using meow's [xmltest](https://github.com/xforce/anno1800-mod-loader/releases/latest/download/xmltest.zip) which works like the modloader.
 
Credits: First version of FileDB unpacking done by @VeraAtVersus, based on reverse engineering by @lysannschlegel

# Usage

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

- Version 2 is used for new or updated files after this date -> [documentation](https://github.com/anno-mods/FileDBReader/wiki/compression-version-2)

> The compressor can autodetect versions while decompressing. Alternativly, you can use the check_fileversion verb.

# Internal Compression 
Be aware that filedb compressed files can contain other filedb compressed files which you have to decompress while interpreting. When decompressing, the xml node structure may look like this, in which case the bytesize is stored along with the file: 

```xml
<None>
    <ByteCount />
    <Data />
</None>
```

> When using the internal decompression in your interpreter file, the ByteCount is automatically overwritten if it exists 

> notable examples of this are AreaManagerData and SessionData/BinaryData

# Sample interpreter file

```xml
<Converts>
    <Default Type ="Int32" />
    <InternalCompression>
        <Element Path="//AreaManagerData/None/Data" CompressionVersion="2">
            <ReplaceTagNames>
                <!-- ensure that Original and Replacement are both unique! -->
                <Entry Original="Delayed Construction" Replacement="DelayedConstruction"/>
            </ReplaceTagNames>
        <Element>
    </InternalCompression>
    <Converts>
        <Convert Path ="//VegetationPropSetName" Type="String" Encoding="UTF-8" />
        <Convert Path ="//GlobalAmbientName" Type="String" />
        <Convert Path ="//HeightMap/HeightMap" Type="UInt16" Structure="List" />
        <Convert Path ="//GuidVariationList" Type="Int32" Structure="Cdata" />
        <Convert Path="//MapTemplate/TemplateElement/Element/Size" Type="Int16">
            <!-- This Enum will map the converted value 0 to Small, 1 to Medium and 2 to Large. Ensure that Name and Value are both unique -->
            <Enum>
                <Entry Value="0" Name="Small" />
                <Entry Value="1" Name="Medium" />
                <Entry Value="2" Name="Large" />
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
Which also can be interpreted using an interpreter file. CDATA nodes must be marked in the interpreter with 

```XML
<Convert Structure = "Cdata">
```

Bytesizes are automatically adjusted.

> The most common example of this are .fc files for visual feedback.

To use this functionality, run 

```Shell
fctohex -f <inputfiles> -i <interpreterFile>
```

## Invalid XML
Anno accepts </> as an xml closing tag. While reading, any of these closing tags are autocorrected, 
and since Anno also understands the valid xml syntax, you can use them ingame right away. 

# FileDBSerializer Library 

The build also produces a Library to include in your own projects. A detailed explanation is in the [Wiki](https://github.com/anno-mods/FileDBReader/wiki/Using-the-FileDB-Library-in-C%23)






