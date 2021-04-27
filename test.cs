using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FileDBReader {

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public partial class Content {

    #region Properties

    public ContentBase Base { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBase {

    #region Properties

    public ContentBaseTerrain Terrain { get; set; }

    public ContentBaseWaterInfos WaterInfos { get; set; }

    public ContentBaseWaterFlow WaterFlow { get; set; }

    public ContentBasePropGrid PropGrid { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrain {

    #region Properties

    public byte Version { get; set; }

    public ushort GridWidth { get; set; }

    public ushort GridHeight { get; set; }

    public byte NodeSize { get; set; }

    public uint UnitScale { get; set; }

    public ushort MaterialMapWidth { get; set; }

    public ushort MaterialMapHeight { get; set; }

    public byte MaterialIDBlockSize { get; set; }

    public ushort TintMapWidth { get; set; }

    public ushort TintMapHeight { get; set; }

    public ushort GritMapWidth { get; set; }

    public ushort GritMapHeight { get; set; }

    public ushort PageSize { get; set; }

    public byte MinMeshLevel { get; set; }

    public ushort TintPageSize { get; set; }

    public byte NodesPerMeshCluster { get; set; }

    public byte UsingSplitImageHeightMaps { get; set; }

    public byte HeightPageBorderTexels { get; set; }

    public byte NodeSizeWS { get; set; }

    [XmlArrayItem("None", IsNullable = false)]
    public ContentBaseTerrainNone[] Levels { get; set; }

    public string MaterialSetFileName { get; set; }

    public string MaterialIDs2 { get; set; }

    public string MaterialAlphas2 { get; set; }

    public ContentBaseTerrainTintsMap TintsMap { get; set; }

    public ContentBaseTerrainGritMap2 GritMap2 { get; set; }

    public ContentBaseTerrainSlushMap SlushMap { get; set; }

    public ContentBaseTerrainActiveMap ActiveMap { get; set; }

    public ContentBaseTerrainAllNormalMapLevels AllNormalMapLevels { get; set; }

    public ContentBaseTerrainTintMapLevels TintMapLevels { get; set; }

    public ContentBaseTerrainCoarseHeightMap CoarseHeightMap { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainNone {

    #region Properties

    public byte Version { get; set; }

    public ContentBaseTerrainNoneMinYArray MinYArray { get; set; }

    public ContentBaseTerrainNoneMaxYArray MaxYArray { get; set; }

    public ContentBaseTerrainNoneHasMeshArray HasMeshArray { get; set; }

    public ushort LevelSizeX { get; set; }

    public ushort LevelSizeZ { get; set; }

    public ushort NodeSizeWS { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainNoneMinYArray {

    #region Properties

    public ushort width { get; set; }

    public ushort height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainNoneMaxYArray {

    #region Properties

    public ushort width { get; set; }

    public ushort height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainNoneHasMeshArray {

    #region Properties

    public ushort x { get; set; }

    public ushort y { get; set; }

    public string bits { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainTintsMap {

    #region Properties

    public ushort width { get; set; }

    public ushort height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainGritMap2 {

    #region Properties

    public byte width { get; set; }

    public byte height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainSlushMap {

    #region Properties

    public byte width { get; set; }

    public byte height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainActiveMap {

    #region Properties

    public byte x { get; set; }

    public byte y { get; set; }

    public string bits { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainAllNormalMapLevels {

    #region Properties

    [XmlArrayItem("None", IsNullable = false)]
    public ContentBaseTerrainAllNormalMapLevelsNone[] Levels { get; set; }

    public ContentBaseTerrainAllNormalMapLevelsBaseTextureData BaseTextureData { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainAllNormalMapLevelsNone {

    #region Properties

    public ContentBaseTerrainAllNormalMapLevelsNoneFlags Flags { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainAllNormalMapLevelsNoneFlags {

    #region Properties

    public byte width { get; set; }

    public byte height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainAllNormalMapLevelsBaseTextureData {

    #region Properties

    public ushort width { get; set; }

    public ushort height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainTintMapLevels {

    #region Properties

    [XmlArrayItem("None", IsNullable = false)]
    public ContentBaseTerrainTintMapLevelsNone[] Levels { get; set; }

    public ContentBaseTerrainTintMapLevelsBaseTextureData BaseTextureData { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainTintMapLevelsNone {

    #region Properties

    public ContentBaseTerrainTintMapLevelsNoneFlags Flags { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainTintMapLevelsNoneFlags {

    #region Properties

    public byte width { get; set; }

    public byte height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainTintMapLevelsBaseTextureData {

    #region Properties

    public ushort width { get; set; }

    public ushort height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseTerrainCoarseHeightMap {

    #region Properties

    public ushort width { get; set; }

    public ushort height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseWaterInfos {

    #region Properties

    public byte width { get; set; }

    public byte height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBaseWaterFlow {

    #region Properties

    public byte width { get; set; }

    public byte height { get; set; }

    public string map { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBasePropGrid {

    #region Properties

    [XmlArrayItem("None", IsNullable = false)]
    public string[] FileNames { get; set; }

    [XmlArrayItem("None", IsNullable = false)]
    public ContentBasePropGridNone[] Instances { get; set; }

    #endregion Properties
  }

  [Serializable()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class ContentBasePropGridNone {

    #region Properties

    public byte Index { get; set; }

    public string Position { get; set; }

    public string Rotation { get; set; }

    public string Scale { get; set; }

    public string Color { get; set; }

    public byte AdaptTerrainHeight { get; set; }

    [XmlIgnore()]
    public bool AdaptTerrainHeightSpecified { get; set; }

    #endregion Properties
  }
}