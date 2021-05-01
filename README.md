# FileDBReader

FileDB Unpacking done by @VeraAtVersus, based on filedb reverse engineering by LysannSchlegel

A simple command line unpacker, repacker and interpreter for Anno 1800 (and 2205?) filedb compression. 

when decompressing, all data will be represented in hex strings which can be interpreted using an interpreter file. You may then change anything you want, you can also create xml patches using meow's xml tools to patch the xml files: https://github.com/xforce/anno1800-mod-loader/releases/tag/v0.7.12 which works exactly like the modloader.

After you are done editing the decompressed and interpreted xml file, you have to convert it to hex and compress it again.

How to use: 

```
decompress -f <inputfiles>
compress -f <inputFiles> -o <outputFileExtension>
interpret -f <inputFiles> -i <interpreterFile>
toHex -f <inputFiles> -i interpreterFiles
```


Some notes about file formats:
island files (a7t) and map templates (a7m) are 2.2 RDA archives containing .data files which are compressed with filedb compression. 
the .data files can also contain files in filedb compression (i.e. AreaManagerData)
Have fun making the map- and island editors. 

infotips are in filedb compression

bfg files used for global illumination are in filedb compression. 








