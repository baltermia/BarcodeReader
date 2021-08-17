using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

namespace speyck.BarcodeReader
{
    /*
    *  Needed Nuget Libraries:
    *  - ZXing.Net
    *  - System.Drawing.Common
    */

    public class BarcodeReader : IDisposable
    {
        #region Events & Delegates
        /// <summary>
        /// Represents the method that will handle the event raised when a barcode is detected
        /// </summary>
        /// <param name="sender">The object which raises the event</param>
        /// <param name="e">Arguments such as the barcode value and bitmap are given through the EventArgs</param>
        public delegate void BarcodeDetectedEventHandler(object sender, BarcodeEventArgs e);

        /// <summary>
        /// Event which is raised when a barcode is detected
        /// </summary>
        public event BarcodeDetectedEventHandler DetectedBarcode;
        #endregion

        #region Public variables
        /// <summary>
        /// Any exceptions that orrcure will be written in this variable
        /// </summary>
        public Exception Error { get; private set; } = null;

        /// <summary>
        /// Bitmap that is currently being decoded
        /// </summary>
        public Bitmap Image { get; private set; }

        /// <summary>
        /// Value that was decoded out of the barcode in the provided bitmap
        /// </summary>
        public string FoundValue { get; private set; }

        /// <summary>
        /// Whether a barcode was found in the provided bitmap or not
        /// </summary>
        public bool Successful { get; private set; }
        #endregion

        #region Private variables
        /// <summary>
        /// Token which cancels the reader task
        /// </summary>
        private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();

        /// <summary>
        /// Task holding the decoding code
        /// </summary>
        private Task readerTask;

        /// <summary>
        /// ZXing BarcodeReader object which is being used for decoding possible barcodes in a bitmap
        /// </summary>
        private readonly ZXing.BarcodeReader reader;
        #endregion

        #region Constructors -> Public
        /// <summary>
        /// Creates a new instance of the BarcodeReader class. The Constructor will set any settings for decoding barcodes with the Decode() method
        /// </summary>
        /// <param name="type">What type of barcode the provided bitmap should be searched for. By defualt all one-dimensional barcodes will be detected</param>
        /// <param name="performanceMode">The decoder will take less resources to search for the barcodes. By default the mode is set to performance</param>
        public BarcodeReader(bool performanceMode = true, params BarcodeFormat[] types)
        {
            reader = new ZXing.BarcodeReader()
            {
                AutoRotate = !performanceMode,
                Options = new DecodingOptions()
                {
                    TryHarder = !performanceMode,
                    PossibleFormats = types.Length >= 1 ? types : new BarcodeFormat[] { BarcodeFormat.All_1D }
                }
            };
        }

        /// <summary>
        /// Creates a new instance of the BarcodeReader class. The Constructor will set any settings for decoding barcodes with the Decode() method
        /// </summary>
        /// <param name="tryHarder">The decoder will try harder decoding a barcode from the bitmap, thus also using more resources.</param>
        /// <param name="autoRotate">The decoder will rotate the bitmap to look for any barcodes. This will take more resources if enabled</param>
        /// <param name="type">What type of barcode the provided bitmap should be searched for. By defualt all one-dimensional barcodes will be detected</param>
        public BarcodeReader(bool tryHarder, bool autoRotate, params BarcodeFormat[] types)
        {
            reader = new ZXing.BarcodeReader()
            {
                AutoRotate = autoRotate,
                Options = new DecodingOptions()
                {
                    TryHarder = tryHarder,
                    PossibleFormats = types.Length >= 1 ? types : new BarcodeFormat[] { BarcodeFormat.All_1D }
                }
            };
        }
        #endregion

        #region Public Methods
        public bool Decode(Bitmap bmp)
        {
            try
            {
                readerTask = Task.Run(() =>
                {
                    using (bmp)
                    {
                        Image = bmp;

                        Result result = reader.Decode(bmp);

                        FoundValue = result?.Text;

                        if (Successful = result != null)
                        {
                            DetectedBarcode(this, new BarcodeEventArgs(FoundValue, bmp));
                        }
                        else
                        {
                            Error = new NoBarcodeDetectedException("The provided bitmap did not contain a readable barcode.", bmp);
                        }
                    }
                });

                return Successful;
            }
            catch (Exception ex)
            {
                bmp?.Dispose();

                Error = ex;

                return false;
            }
        }

        public Task<bool> DecodeAsync(Bitmap bmp)
        {
            return Task.Run(() =>
            {
                try
                {
                    readerTask = Task.Run(() =>
                    {
                        using (bmp)
                        {
                            Image = bmp;

                            Result result = reader.Decode(bmp);

                            FoundValue = result?.Text;

                            if (Successful = result != null)
                            {
                                DetectedBarcode(this, new BarcodeEventArgs(FoundValue, bmp));
                            }
                            else
                            {
                                Error = new NoBarcodeDetectedException("The provided bitmap did not contain a readable barcode.", bmp);
                            }
                        }
                    });

                    return Successful;
                }
                catch (Exception ex)
                {
                    bmp?.Dispose();

                    Error = ex;

                    return false;
                }
            });
        }

        /// <summary>
        /// Stops decoding and releases any managed/unmanaged memory the class uses
        /// </summary>
        public void Dispose()
        {
            cancelSource?.Cancel();

            readerTask?.Dispose();

            Image?.Dispose();
        }
        #endregion

        #region Public static Methods
        /// <summary>
        /// Saves a bitmap as jpeg file
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="path">Path where the file should be saved and with what name</param>
        /// <param name="format">What format the image should be saved in</param>
        /// <returns>Wether the image could be saved or not</returns>
        public static void SaveBitmap(Bitmap bmp, string path, ImageFormat format)
        {
            using (bmp)
            {
                string ending = "." + format.ToString().ToLower();
                int endLength = ending.Length;

                if (path.Length <= endLength)
                {
                    path += ending;
                }
                else if (path.Substring(path.Length - endLength, endLength) != ending)
                {
                    path += ending;
                }

                bmp.Save(path, format);
            }
        }
        #endregion
    }
}