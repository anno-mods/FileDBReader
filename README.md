# FileDBReader

[![example workflow](https://github.com/anno-mods/FileDBReader/actions/workflows/main.yml/badge.svg)](https://github.com/anno-mods/FileDBReader/actions/workflows/main.yml)
[![Create Release](https://github.com/anno-mods/FileDBReader/actions/workflows/release.yml/badge.svg)](https://github.com/anno-mods/FileDBReader/actions/workflows/release.yml)

A simple command line unpacker, repacker and interpreter for proprietary Anno 1800 compression.

- Decompressing: Convert BlueByte's documents to XML, representing Data as HexStrings 

- Interpreting: Uses Xpath to select xmlNodes in documents and converts them. 

After you are done editing the decompressed and interpreted xml file, you have to reinterpret it to hex (using the same interpreter) and compress it again.

> Tip: you can also create xml patches and apply changes using meow's [xmltest](https://github.com/jakobharder/anno1800-mod-loader/releases/latest/download/xmltest.zip) which works like the modloader.
 
Credits: First version of FileDB unpacking done by @VeraAtVersus, based on reverse engineering by @lysannschlegel

# TLDR Usage

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

> The FileFormats folder contains a few more-or-less correct interpreter documents that have been collected over time. Don't let that stop you from creating your own. For this, refer to the [Wiki](https://github.com/anno-mods/FileDBReader/wiki/Writing-Interpreters).

# AnnoMods.BBDom 

This project also includes a library for use in your own projects. [Documentation](https://github.com/anno-mods/FileDBReader/wiki/Using-AnnoMods.BBDom)

To get started, simply add the [Nuget Package](https://www.nuget.org/packages/AnnoMods.BBDom/) to your project

> Supported .NET versions: .NET 6.0 and above

# Compression Versions

There are three versions of this compression
 
- Version 1 is what you find in files up to Anno 1800, GU 12 (31.08.2021) -> [documentation](https://github.com/lysannschlegel/RDAExplorer/wiki/file.db-format)
- Version 2 is used for new or updated files after this date -> [documentation](https://github.com/anno-mods/FileDBReader/wiki/compression-version-2)
- Version 3 is Version 2 with extra steps, introduced shortly after Version 2 -> [documentation](https://github.com/anno-mods/FileDBReader/wiki/compression-version-2-&-3#version-3)

> The compressor autodetects versions while decompressing. You can use the check_fileversion verb to determine yourself.

# Binary Data in xml-based Anno files
Some xml-based file formats, especially in Anno 1701/1404/2070, do not use BlueByte's compression, but they still have binary parts: 

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






