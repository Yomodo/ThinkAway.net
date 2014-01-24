/*
* Copyright 2008 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using ByteMatrix = ThinkAway.Drawing.Barcode.Common.ByteMatrix;
using EAN13Writer = ThinkAway.Drawing.Barcode.oned.EAN13Writer;
using EAN8Writer = ThinkAway.Drawing.Barcode.oned.EAN8Writer;
using QRCodeWriter = ThinkAway.Drawing.Barcode.qrcode.QRCodeWriter;
namespace ThinkAway.Drawing.Barcode
{
	
	/// <summary> This is a factory class which finds the appropriate Writer subclass for the BarcodeFormat
	/// requested and encodes the barcode with the supplied contents.
	/// 
	/// </summary>
	/// <author>  dswitkin@google.com (Daniel Switkin)
	/// </author>
	/// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
	/// </author>

	public sealed class MultiFormatWriter : IWriter
	{
		
		public ByteMatrix Encode(System.String contents, BarcodeFormat format, int width, int height)
		{
			
			return Encode(contents, format, width, height, null);
		}
		
		public ByteMatrix Encode(System.String contents, BarcodeFormat format, int width, int height, System.Collections.Hashtable hints)
		{
		    if (format == BarcodeFormat.EAN_8)
			{
				return new EAN8Writer().Encode(contents, format, width, height, hints);
			}
		    if (format == BarcodeFormat.EAN_13)
		    {
		        return new EAN13Writer().Encode(contents, format, width, height, hints);
		    }
		    if (format == BarcodeFormat.QR_CODE)
		    {
		        return new QRCodeWriter().Encode(contents, format, width, height, hints);
		    }
		    throw new System.ArgumentException("No encoder available for format " + format);
		}
	}
}