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

namespace ThinkAway.Drawing.Barcode.client.result
{
	
	/// <author>  Sean Owen
	/// </author>
	/// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
	/// </author>
	public sealed class GeoParsedResult:ParsedResult
	{
	    private string _geoUri;

	    /// <summary>
	    /// 
	    /// </summary>
	    public string GeoURI
	    {
	        get { return _geoUri; }
	        private set { _geoUri = value; }
	    }

	    private double _latitude;

	    /// <returns> latitude in degrees
	    /// </returns>
	    public double Latitude
	    {
	        get { return _latitude; }
	        private set { _latitude = value; }
	    }

	    private double _longitude;

	    /// <returns> longitude in degrees
	    /// </returns>
	    public double Longitude
	    {
	        get { return _longitude; }
	        private set { _longitude = value; }
	    }

	    private double _altitude;

	    /// <returns> altitude in meters. If not specified, in the geo URI, returns 0.0
	    /// </returns>
	    public double Altitude
	    {
	        get { return _altitude; }
	        private set { _altitude = value; }
	    }

	    /// <summary>
        /// 
        /// </summary>
	    override public System.String DisplayResult
		{
			get
			{
				System.Text.StringBuilder result = new System.Text.StringBuilder(50);
				result.Append(Latitude);
				result.Append(", ");
				result.Append(Longitude);
				if (Altitude > 0.0f)
				{
					result.Append(", ");
					result.Append(Altitude);
					result.Append('m');
				}
				return result.ToString();
			}

            /*
			public String getGoogleMapsURI() {
			StringBuffer result = new StringBuffer(50);
			result.append("http://maps.google.com/?ll=");
			result.append(latitude);
			result.append(',');
			result.append(longitude);
			if (altitude > 0.0f) {
			// Map altitude to zoom level, cleverly. Roughly, zoom level 19 is like a
			// view from 1000ft, 18 is like 2000ft, 17 like 4000ft, and so on.
			double altitudeInFeet = altitude * 3.28;
			int altitudeInKFeet = (int) (altitudeInFeet / 1000.0);
			// No Math.log() available here, so compute log base 2 the old fashioned way
			// Here logBaseTwo will take on a value between 0 and 18 actually
			int logBaseTwo = 0;
			while (altitudeInKFeet > 1 && logBaseTwo < 18) {
			altitudeInKFeet >>= 1;
			logBaseTwo++;
			}
			int zoom = 19 - logBaseTwo;
			result.append("&z=");
			result.append(zoom);
			}
			return result.toString();
			}
			*/
			
		}
		
		//UPGRADE_NOTE: Final was removed from the declaration of 'geoURI '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	    //UPGRADE_NOTE: Final was removed from the declaration of 'latitude '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	    //UPGRADE_NOTE: Final was removed from the declaration of 'longitude '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	    //UPGRADE_NOTE: Final was removed from the declaration of 'altitude '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"

	    internal GeoParsedResult(System.String geoURI, double latitude, double longitude, double altitude):base(ParsedResultType.GEO)
		{
			this.GeoURI = geoURI;
			this.Latitude = latitude;
			this.Longitude = longitude;
			this.Altitude = altitude;
		}
	}
}