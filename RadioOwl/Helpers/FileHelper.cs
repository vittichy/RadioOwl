using Dtc.Common.Extensions;
using RadioOwl.Parsers.Data;
using System;
using System.IO;

namespace RadioOwl.Helpers
{
    public class FileHelper
    {
        /// <summary>
        /// Jméno pro uložení části pořadu
        /// </summary>
        /// <param name="radioDataPart"></param>
        /// <returns></returns>
        public string GenerateMp3FileName(RadioDataPart radioDataPart)
        {
            return GenerateFullPartFilename(radioDataPart, ".mp3");
        }

        /// <summary>
        /// Nageruji vhodné filename pro readme k části pořadu
        /// </summary>
        public string GenerateReadmeFilename(RadioDataPart radioDataPart)
        {
            return GenerateFullPartFilename(radioDataPart, ".txt");
        }

        /// <summary>
        /// Nageruji vhodné filename pro readme k celému pořadu
        /// </summary>
        public string GenerateReadmeFilename(RadioData radioData)
        {
            var path = GetFolderPath(radioData);
            var fileName = "readme.txt";
            return Path.Combine(path, fileName);
        }

        private string GenerateFullPartFilename(RadioDataPart radioDataPart, string extension)
        {
            var path = GetFolderPath(radioDataPart?.RadioData);
            var fileName = GeneratePartFilename(radioDataPart, extension);
            return Path.Combine(path, fileName);
        }

        /// <summary>
        /// Nageruji vhodné filename dle img pořadu
        /// </summary>
        public string GenerateImageFilename(RadioData radioData)
        {
            var path = GetFolderPath(radioData);
            var filename = GetImageFilename(radioData);
            return Path.Combine(path, filename);
        }

        /// <summary>
        /// Nageruji vhodný adresář pro uložení pořadu
        /// </summary>
        private string GetFolderPath(RadioData radioData)
        {
            var subPath = radioData?.SiteDocumentPath?.Trim();
            if (string.IsNullOrEmpty(subPath))
            {
                subPath = radioData?.ContentId?.Trim();
                if (string.IsNullOrEmpty(subPath))
                {
                    subPath = $"RADIOOWL_DOWNLOAD_{Guid.NewGuid()}";
                }
            }

            // nebezpecne znaky z filename + max delka jmena souboru
            var paths = subPath.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
            for (var i = 0; i < paths.Length; i++)
            {
                paths[i] = paths[i].RemovePathInvalidChars().TrimToMaxLen(60);
            }
            var subPathFinal = string.Empty;
            for (var i = 0; i < paths.Length; i++)
            {
                subPathFinal = Path.Combine(subPathFinal, paths[i]);
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), subPathFinal);
        }

        /// <summary>
        /// Nageruji vhodné filename dle části pořadu
        /// </summary>
        private string GeneratePartFilename(RadioDataPart radioDataPart, string extension)
        {
            var filename = GetBaseFileName(radioDataPart?.Title);

            // skládá se z jednotlivých dílů? přidám tedy do filename
            if (radioDataPart.RadioData.ContentSerialAllParts.HasValue && radioDataPart.RadioData.ContentSerialAllParts.Value > 0)
            {
                string partNo = null;
                if (radioDataPart.PartNo.HasValue && radioDataPart.PartNo.Value > 0)
                {
                    // ideální situace znám vše
                    partNo = radioDataPart.RadioData.ContentSerialAllParts.Value > 99 ? $"{radioDataPart.PartNo:000}" : $"{radioDataPart.PartNo:00}";
                }
                else
                {
                    // divná situace, znám počet částí, ale neznám číslo části 
                    partNo = $"UNKOWN_PART_NO_{Guid.NewGuid()}";
                }
                filename = $"{partNo}-{radioDataPart.RadioData.ContentSerialAllParts}-{filename?.Trim()}";
            }

            filename = RemoveInvalidChars(filename.TrimToMaxLen(80));
            filename += extension;
            return filename;
        }

        private string GetReadmeFilename(string title)
        {
            var filename = GetBaseFileName(title);
            filename += ".readme";
            return filename;
        }

        private string GetImageFilename(RadioData radioData)
        {
            var filename = GetBaseFileName(radioData.SiteEntityLabel);
            filename += ".jpg";
            return filename;
        }

        /// <summary>
        /// Vrací základ filename pro uložení pořadu dle title
        /// </summary>
        private string GetBaseFileName(string title)
        {
            var filename = RemoveInvalidChars(title?.Trim());
            if (string.IsNullOrEmpty(filename))
            {
                filename = $"UNKOWN_NAME_{Guid.NewGuid()}";
            }
            filename = RemoveInvalidChars(filename.TrimToMaxLen(80));
            return filename;
        }

        private string RemoveInvalidChars(string fileName)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char ch in invalidChars)
            {
                fileName = fileName.Replace(ch.ToString(), "");
            }
            return fileName;
        }

        public DirectoryInfo EnsureDirectoryCreated(string fileName)
        {
            var path = Path.GetDirectoryName(fileName);
            return Directory.CreateDirectory(path);
        }
    }
}
