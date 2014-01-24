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

namespace ThinkAway.Drawing.Barcode.Common
{
	
	/// <summary> Superclass of classes encapsulating types ECIs, according to "Extended Channel Interpretations"
	/// 5.3 of ISO 18004.
	/// 
	/// </summary>
	/// <author>  Sean Owen
	/// </author>
	/// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
	/// </author>
	public abstract class ECI
	{
		virtual public int Value
		{
			get
			{
				return _valueRenamed;
			}
			
		}
		
		//UPGRADE_NOTE: Final was removed from the declaration of 'value '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		private readonly int _valueRenamed;
		
		internal ECI(int valueRenamed)
		{
			this._valueRenamed = valueRenamed;
		}

	    /// <param name="valueRenamed"></param>
	    /// <returns> {@link ECI} representing ECI of given value, or null if it is legal but unsupported
	    /// </returns>
	    /// <throws>  IllegalArgumentException if ECI value is invalid </throws>
	    public static ECI getECIByValue(int valueRenamed)
		{
			if (valueRenamed < 0 || valueRenamed > 999999)
			{
				throw new System.ArgumentException("Bad ECI value: " + valueRenamed);
			}
			if (valueRenamed < 900)
			{
				// Character set ECIs use 000000 - 000899
				return CharacterSetECI.GetCharacterSetEciByValue(valueRenamed);
			}
			return null;
		}
	}
}