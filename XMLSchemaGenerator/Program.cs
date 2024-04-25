using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace XMLSchemaGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.Write("Unesite putanju do XML datoteke (pronađite datoteku u file exploreru i kopirajte path CTRL + Shift + C: ");
            string inputPath = Console.ReadLine();

            // Ukloni dvostruke navodnike ako postoje
            inputPath = inputPath.Trim('"');

            string normalizedPath = Path.GetFullPath(inputPath);

            Console.WriteLine($"Unesena putanja: {normalizedPath}");

            try
            {
                // Učitaj XML sadržaj iz tekstualne datoteke
                string xmlContent = File.ReadAllText(normalizedPath);

                // Stvori novi XmlReader iz sadržaja XML-a
                using (XmlReader reader = XmlReader.Create(new StringReader(xmlContent)))
                {
                    // Koristi XmlSchemaInference za generiranje sheme
                    XmlSchemaSet schemaSet = new XmlSchemaSet();
                    XmlSchemaInference schemaInference = new XmlSchemaInference();
                    schemaSet = schemaInference.InferSchema(reader);

                    // Generiraj naziv filea i putanju na koju se sprema (u isti direktorij kao i ulazna datoteka)
                    string fileNameBezEkstenzije = Path.GetFileNameWithoutExtension(normalizedPath);
                    string outputFileName = $"{fileNameBezEkstenzije}.xsd";
                    string directory = Path.GetDirectoryName(normalizedPath);

                    // Spoji putanju i ime filea
                    string schemaPath = Path.Combine(directory, outputFileName);

                    // Stvaranje novog XmlWritera za zapisivanje sheme u datoteku
                    using (XmlWriter writer = XmlWriter.Create(schemaPath))
                    {
                        // Pisanje XML sheme u konzolu i u datoteku
                        foreach (XmlSchema schema in schemaSet.Schemas())
                        {
                            // Ispiši generiranu shemu u konzolu
                            schema.Write(Console.Out);

                            // Zapiši shemu u datoteku
                            schema.Write(writer);
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine($"Shema je uspješno spremljena u {schemaPath}.");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Došlo je do pogreške prilikom obrade datoteke:");
                Console.WriteLine(ex.Message);
            }

        }
    }
}
