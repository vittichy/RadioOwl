using System.Windows;

namespace RadioOwl.Windows
{
    /// <summary>
    /// Extension metody pro DragEventArgs
    /// </summary>
    public static class DragEventArgsExtension
    {
        /// <summary>
        /// Z DragEventArgs získá data ve požadovaném formátu jako string
        /// </summary>
        public static string GetTextData(this DragEventArgs e, string dragDropFormat)
        {
            if (e?.Data != null && e.Data.GetDataPresent(dragDropFormat))
            {
                var data = e.Data.GetData(dragDropFormat) as string;
                return data?.Trim();
            }
            return null;
        }

        /// <summary>
        /// Z DragEventArgs získá data ve formátu unicode
        /// </summary>
        public static string GetUnicodeData(this DragEventArgs e)
        {
            return GetTextData(e, "UnicodeText");
        }
    }
}
