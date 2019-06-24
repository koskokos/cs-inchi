/*
 * CSInChI
 * v0.5
 * 
 * A class library for using the IUPAC InChI library from the 
 * .Net framework and the Mono Runtime.
 * 
 * CSInChI is a product of the ChemSharp Project.
 * 
 * Copyright (C) 2008 Matt Sprague <mesprague@users.sf.net>
 * 
 * Contact: chemsharp-csinchi@lists.sourceforge.net
 * 
 * This software requires the IUPAC InChI toolkit , which is distributed
 * under the GNU LGPL:
 * Copyright (C) 2005 The International Union of Pure and Applied Chemistry
 * IUPAC International Chemical Identifier (InChI) (contact:secretariat@iupac.org)
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA. 
 */

using System;
using System.Runtime.InteropServices;

namespace CSInChI
{
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
    public unsafe struct InChIStrucOutput : IDisposable
    {
        private void* AtomsPtr;
        private IntPtr StereoDataPtr;
        public short NumAtoms;
        public short NumStereo0D;
        private IntPtr errMsg;
        private IntPtr log;


        public string ErrorMsg => Marshal.PtrToStringAnsi(errMsg);


        /// <summary>
        /// A string containing a list of recognized options and 
        /// possibly an Error/warning message. 
        /// </summary>
        public string OutputLog => Marshal.PtrToStringAnsi(log);


        //marshaling nested arrays is not supported

        /// <summary>
        /// An array of warning flags. Due to the lack of support
        /// for marshaling nested arrays this field has been converted
        /// to a 1-D array with the same total capacity.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] WarningFlags;

        public Span<InChIAtom> GetAtoms() => new Span<InChIAtom>(AtomsPtr, NumAtoms);

        public InChIStereo0D[] GetStereoData()
        {
            if (NumStereo0D == 0)
                return null;

            InChIStereo0D[] stereoInfo = new InChIStereo0D[NumStereo0D];
            IntPtr pStereo = StereoDataPtr;
            int stereoSize = Marshal.SizeOf(typeof(InChIStereo0D));
            InChIStereo0D stereo;

            for (int i = 0; i < NumStereo0D; i++)
            {
                stereo = (InChIStereo0D)Marshal.PtrToStructure(new IntPtr(pStereo.ToInt64() + i * stereoSize), typeof(InChIStereo0D));
                stereoInfo[i] = stereo;
            }
            return stereoInfo;
        }

        public void Dispose()
        {
            if (log != IntPtr.Zero)
            {
                LibInChI.DeallocateOutputStruct(ref this);
                GC.SuppressFinalize(this);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct InChIStringOutput : IDisposable
    {
        internal sbyte* inchi;
        internal sbyte* auxinfo;
        internal sbyte* errMsg;
        internal sbyte* outLog;

        public string InChI => new string(inchi);
        public string ErrorMessage => new string(errMsg);
        public string OutputLog => new string(outLog);

        public void Dispose()
        {
            if (outLog != default)
            {
                LibInChI.DeallocateInChIString(ref this);
            }
        }
    }
}
