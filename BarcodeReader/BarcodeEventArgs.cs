using System;
using System.Drawing;

namespace BarcodeReader
{
    /// <summary>
    /// EventArgs for BarcodeDetectedEvents but also other events using Barcodes
    /// </summary>
    public class BarcodeEventArgs : EventArgs
    {
        /// <summary>
        /// Decoded value from the barcode
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Bitmap image of the decoded barcode
        /// </summary>
        public Bitmap Image { get; }

        /// <summary>
        /// Creates a new instance of the BarcodeEventArgs class 
        /// </summary>
        /// <param name="value">Decoded barcode-value</param>
        /// <param name="bmp">Bitmap of the decoded barcode</param>
        public BarcodeEventArgs(string value, Bitmap bmp)
        {
            Value = value;
            Image = bmp;
        }
    }
}
