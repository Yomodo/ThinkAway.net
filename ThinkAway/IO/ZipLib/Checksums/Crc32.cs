// CRC32.cs - Computes CRC32 data checksum of a data stream
// Copyright (C) 2001 Mike Krueger
//
// This file was translated from java, it was part of the GNU Classpath
// Copyright (C) 1999, 2000, 2001 Free Software Foundation, Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// Linking this library statically or dynamically with other modules is
// making a combined work based on this library.  Thus, the terms and
// conditions of the GNU General Public License cover the whole
// combination.
// 
// As a special exception, the copyright holders of this library give you
// permission to link this library with independent modules to produce an
// executable, regardless of the license terms of these independent
// modules, and to copy and distribute the resulting executable under
// terms of your choice, provided that you also meet, for each linked
// independent module, the terms and conditions of the license of that
// module.  An independent module is a module which is not derived from
// or based on this library.  If you modify this library, you may extend
// this exception to your version of the library, but you are not
// obligated to do so.  If you do not wish to do so, delete this
// exception statement from your version.

using System;
using ThinkAway.Security;

namespace ThinkAway.IO.ZipLib.Checksums 
{
	
	/// <summary>
	/// Generate a table for a byte-wise 32-bit CRC calculation on the polynomial:
	/// x^32+x^26+x^23+x^22+x^16+x^12+x^11+x^10+x^8+x^7+x^5+x^4+x^2+x+1.
	///
	/// Polynomials over GF(2) are represented in binary, one bit per coefficient,
	/// with the lowest powers in the most significant bit.  Then adding polynomials
	/// is just exclusive-or, and multiplying a polynomial by x is a right shift by
	/// one.  If we call the above polynomial p, and represent a byte as the
	/// polynomial q, also with the lowest power in the most significant bit (so the
	/// byte 0xb1 is the polynomial x^7+x^3+x+1), then the CRC is (q*x^32) mod p,
	/// where a mod b means the remainder after dividing a by b.
	///
	/// This calculation is done using the shift-register method of multiplying and
	/// taking the remainder.  The register is initialized to zero, and for each
	/// incoming bit, x^32 is added mod p to the register if the bit is a one (where
	/// x^32 mod p is p+x^32 = x^26+...+1), and the register is multiplied mod p by
	/// x (which is shifting right by one and adding x^32 mod p if the bit shifted
	/// out is a one).  We start with the highest power (least significant bit) of
	/// q and repeat for all eight bits of q.
	///
	/// The table is simply the CRC of all possible eight bit values.  This is all
	/// the information needed to generate CRC's on data a byte at a time for all
	/// combinations of CRC register values and incoming bytes.
	/// </summary>
    public sealed class Crc32Checksum : CRC, IChecksum
	{
		const uint CrcSeed = 0xFFFFFFFF;
		
		internal static uint ComputeCrc32(uint oldCrc, byte value)
		{
			return CrcTable[(oldCrc ^ value) & 0xFF] ^ (oldCrc >> 8);
		}
		
		/// <summary>
		/// The crc data checksum so far.
		/// </summary>
		uint crc;
		
		/// <summary>
		/// Returns the CRC32 data checksum computed so far.
		/// </summary>
		public long Value {
			get {
				return crc;
			}
			set {
				crc = (uint)value;
			}
		}
		
		/// <summary>
		/// Resets the CRC32 data checksum as if no update was ever called.
		/// </summary>
		public void Reset() 
		{ 
			crc = 0; 
		}
		
		/// <summary>
		/// Updates the checksum with the int bval.
		/// </summary>
		/// <param name = "value">
		/// the byte is taken as the lower 8 bits of value
		/// </param>
		public void Update(int value)
		{
			crc ^= CrcSeed;
			crc  = CrcTable[(crc ^ value) & 0xFF] ^ (crc >> 8);
			crc ^= CrcSeed;
		}
		
		/// <summary>
		/// Updates the checksum with the bytes taken from the array.
		/// </summary>
		/// <param name="buffer">
		/// buffer an array of bytes
		/// </param>
		public void Update(byte[] buffer)
		{
			if (buffer == null) {
				throw new ArgumentNullException("buffer");
			}
			
			Update(buffer, 0, buffer.Length);
		}
		
		/// <summary>
		/// Adds the byte array to the data checksum.
		/// </summary>
		/// <param name = "buffer">
		/// The buffer which contains the data
		/// </param>
		/// <param name = "offset">
		/// The offset in the buffer where the data starts
		/// </param>
		/// <param name = "count">
		/// The number of data bytes to update the CRC with.
		/// </param>
		public void Update(byte[] buffer, int offset, int count)
		{
			if (buffer == null) {
				throw new ArgumentNullException("buffer");
			}
			
			if ( count < 0 ) {
#if NETCF_1_0
				throw new ArgumentOutOfRangeException("count");
#else
				throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero");
#endif				
			}
			
			if (offset < 0 || offset + count > buffer.Length) {
				throw new ArgumentOutOfRangeException("offset");
			}
			
			crc ^= CrcSeed;
			
			while (--count >= 0) {
				crc = CrcTable[(crc ^ buffer[offset++]) & 0xFF] ^ (crc >> 8);
			}
			
			crc ^= CrcSeed;
		}
	}
}
