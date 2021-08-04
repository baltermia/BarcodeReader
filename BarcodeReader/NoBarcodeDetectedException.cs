using System;
using System.Drawing;

namespace speyck.BarcodeReader
{
    /// <summary>
    /// Exception for when the Decode() method has a error/exception
    /// </summary>
    public class NoBarcodeDetectedException : Exception
    {
        /// <summary>
        /// Bitmap of the image that doesn't contain a barcode
        /// </summary>
        public Bitmap Image { get; }

        /// <summary>
        /// Creates a new instance of the NoBarcodeDetectedException class
        /// </summary>
        /// <param name="message"></param>
        /// <param name="bmp"></param>
        public NoBarcodeDetectedException(string message, Bitmap bmp) : base(message)
        {
            Image = bmp;
        }
    }
}
