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
    /// <summary>
    /// A structure that holds structural data from the
    /// LibInChI.ParseInChI method. This structure should be 
    /// used with the try/finally pattern to ensure deallocation
    /// of the unmanaged memory.
    /// </summary>
    /// <seealso cref="LibInChI.ParseInChI"/>
    /// 
    /// <example>This example illustrates the use of this structure.
    /// <code>
    ///class Program
    ///{
    ///    static void Main(string[] args)
    ///    {
    ///        string inchi = "InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1+";
    ///        InChIStringInput inp = new InChIStringInput(inchi);
    ///        InChIStrucOutput outStruct = new InChIStrucOutput();
    ///
    ///        try
    ///        {
    ///            int ret = LibInChI.ParseInChI(ref inp, out outStruct);
    ///            string retCode = InChIRetVal.GetStringVal(ret);
    ///            Console.WriteLine(retCode);
    ///
    ///            foreach (InChIAtom atom in outStruct.GetAtoms())
    ///            {
    ///                Console.WriteLine("{0} {1}", atom.ElementName, atom.NumBonds);
    ///            }
    ///        }
    ///        finally
    ///        {
    ///            //free the unmanaged memory
    ///            outStruct.Dispose();
    ///        }
    ///    }
    ///}
    /// </code></example>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
    public struct InChIStrucOutput : IDisposable
    {
        /// <summary>
        /// A pointer to the first atom in the array of atoms
        /// </summary>
        private IntPtr AtomsPtr;

        /// <summary>
        /// A pointer to the first Stereo0D structure in the array
        /// of Stereo0D structures.
        /// </summary>
        private IntPtr StereoDataPtr;

        /// <summary>
        /// The number of atoms in the structure. Max value is
        /// 1023.
        /// </summary>
        public short NumAtoms;

        /// <summary>
        /// The number of Stereo0D structures.
        /// </summary>
        public short NumStereo0D;

        /// <summary>
        /// A string containing error/warning messages.
        /// </summary>
        public string ErrorMsg
        {
            get { return Marshal.PtrToStringAnsi(errMsg); }
        }

        private IntPtr errMsg;

        /// <summary>
        /// A string containing a list of recognized options and 
        /// possibly an Error/warning message. 
        /// </summary>
        public string OutputLog
        {
            get { return Marshal.PtrToStringAnsi(log); }
        }

        private IntPtr log;

        //marshaling nested arrays is not supported

        /// <summary>
        /// An array of warning flags. Due to the lack of support
        /// for marshaling nested arrays this field has been converted
        /// to a 1-D array with the same total capacity.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] WarningFlags;

        /// <summary>
        /// Converts the AtomsPtr to an array of InChIAtom structures.
        /// </summary>
        /// <returns>An array of InChIAtom structures</returns>
        public InChIAtom[] GetAtoms()
        {
            int atomSize = Marshal.SizeOf(typeof(InChIAtom));
            InChIAtom[] iAtoms = new InChIAtom[NumAtoms];

            InChIAtom a;
            IntPtr pAtom = AtomsPtr;
            for (int i = 0; i < iAtoms.Length; i++)
            {
                a = (InChIAtom)Marshal.PtrToStructure(pAtom, typeof(InChIAtom));
                iAtoms[i] = a;
                pAtom = new IntPtr(pAtom.ToInt32() + atomSize);
            }
            
            return iAtoms;
        }

        /// <summary>
        /// Converts the StereoDataPtr to an array of InChIStereo0D structures.
        /// </summary>
        /// <returns>An array of InChIStereo0D structures</returns>
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
                stereo = (InChIStereo0D)Marshal.PtrToStructure(new IntPtr(pStereo.ToInt32() + i * stereoSize), typeof(InChIStereo0D));
                stereoInfo[i] = stereo;
            }
            return stereoInfo;
        }

        /// <summary>
        /// Releases all unmanaged resources used by this structure.
        /// </summary>
        public void Dispose()
        {
            //the output log pointer is always initalized even if the
            //unmanaged function does not produce any structure data
            if (log != IntPtr.Zero)
            {
                LibInChI.DeallocateOutputStruct(ref this);
                GC.SuppressFinalize(this);
            }
        }

    }// end struct InChIStrucOutput

    /// <summary>
    /// A structure that holds the output data from methods
    /// that create an InChI. If the fields are initalized
    /// by the GetInChI(ref InChIStrucInput structData, out InChIStringOutput output)
    /// method the try/finally pattern should be used to ensure deallocation
    /// of the unmanaged memory. Note that the properties of this structure convert pointers to strings
    /// when accessed. Repeatedly accessing these fields will lead to a loss of
    /// performance.
    /// </summary>
    /// <example></example>
    /// <seealso cref="LibInChI.GetInChI"/>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
    public struct InChIStringOutput : IDisposable
    { 
        private IntPtr inchi;

        /// <summary>
        /// A read only property that returns the InChI string.
        /// </summary>
        public string InChI
        { 
            get
            {
                return Marshal.PtrToStringAnsi(inchi);
            }
        }

        private IntPtr auxinfo;

        /// <summary>
        /// A read only string containg the aux info.
        /// </summary>
        public string AuxInfo
        {
            get
            {
                return Marshal.PtrToStringAnsi(auxinfo);
            }
        }

        private IntPtr errMsg;

        /// <summary>
        /// A read only string containing error/warning messages.
        /// </summary>
        public string ErrorMessage
        {
            get 
            { 
                return Marshal.PtrToStringAnsi(errMsg); 
            }
        }

        private IntPtr outLog;

        /// <summary>
        /// A read only string containing a list of recognized options and 
        /// possibly an Error/warning message. 
        /// </summary>
        public string OutputLog
        {   get 
            { 
                return Marshal.PtrToStringAnsi(outLog); 
            }
        }

        /// <summary>
        /// Releases all resources used by this structure.
        /// </summary>
        public void Dispose()
        {
            //the output log pointer will be initalized
            //even if the call to GetInChI fails
            if(outLog != IntPtr.Zero)
            {
                LibInChI.DeallocateInChIString(ref this);
                GC.SuppressFinalize(this);
            }
        
        }
        
    }//end struct InChIStringOutput

}//end namespace csinchi
