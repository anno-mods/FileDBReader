using System.IO;

namespace FileDBReader {

  internal class Program {

    #region Methods

    private static void Main(string[] args) {
        var reader = new FileReader();
        //reader.ReadFile("export.infotip");
            
       
        var writer = new FileWriter();
            writer.Export("export-test.xml");
        }

        #endregion Methods
    }
}