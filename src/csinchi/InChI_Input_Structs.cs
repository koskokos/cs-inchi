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
 * This software requires the IUPAC InChI toolkit which is distributed
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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CSInChI
{
    /// <summary>
    /// A struct that holds the input data for the ParseInChI(ref InChIStringInput input, out InChIStrucOutput output) method.
    /// </summary>
    ///<seealso cref="LibInChI.ParseInChI"/>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
    public struct InChIStringInput
    {
        /// <summary>
        /// Creates a new InChIStringInput struct with the specified
        /// space delimited options string. See inchi_options.txt for a complete list of options.
        /// </summary>
        /// <param name="inchiString">the inchi string</param>
        /// <param name="options">the string containing the list of options</param>
        public InChIStringInput(string inchiString, string options)
        {
            if (inchiString == "")
                throw new ArgumentException("Cannot create an InChIStringInput with an empty InChI");
        
            inchi = inchiString;
            opt = options;
        }

        /// <summary>
        /// Creates a new InChIStringInput with an empty options
        /// string.
        /// </summary>
        /// <param name="inchiString">the inchi string</param>
        public InChIStringInput(string inchiString)
            : this(inchiString, "")
        { }

        //these properties were added because Iron Python does not allow you to
        //change the value of public fields on value types

        /// <summary>
        /// The InChI string.
        /// </summary>
        public string InChI
        {
            get { return inchi; }
            set { inchi = value; }
        }

        private string inchi;
        
        /// <summary>
        /// The space delimited string of options. Options start with
        /// '/' in Windows or '-' for other platforms.
        /// </summary>
        public string Options
        {
            get { return opt; }
            set { opt = value; }
        }

        private string opt;

    }//end struct InChIStringInput


    /// <summary>
    /// A struct that holds input structural data for the GetInChI
    /// method. The user is responsible for calling the Dispose method to ensure
    /// that unmanaged resources are deallocated. This structure should be used with the try/finally
    /// pattern to ensure that the Dispose method is called even if an exception occurs.  
    /// </summary>
    /// <seealso cref="LibInChI.GetInChI"/>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
    public unsafe struct InChIStrucInput : IDisposable
    {
        readonly static int _atomSize = Marshal.SizeOf<InChIAtom>();
        readonly static int _stereo0DSize = Marshal.SizeOf<InChIStereo0D>();

        public InChIStrucInput(InChIAtom[] atoms) : this(atoms,"")
        { 
            
        }

        public InChIStrucInput(InChIAtom[] atoms, InChIStereo0D[] stereoData, string options)
            : this()
        {
            {
                var atomsLength = atoms.Length;
                NumAtoms = (short)atomsLength;
                var atomsPtr = Marshal.AllocHGlobal(new IntPtr(atomsLength * _atomSize));
                try
                {
                    long addr = atomsPtr.ToInt64();
                    for (int i = 0; i < atomsLength; i++, addr += _atomSize)
                    {
                        Marshal.StructureToPtr(atoms[i], new IntPtr(addr), true);
                    }
                }
                catch
                {
                    Marshal.FreeHGlobal(atomsPtr);
                    throw;
                }
                AtomsPtr = atomsPtr.ToPointer();
            }

            {
                var stereo0DLength = stereoData?.Length ?? 0;
                if (stereo0DLength != 0)
                {
                    NumStereo0D = (short)stereo0DLength;
                    var stereoDataPtr = Marshal.AllocHGlobal(new IntPtr(stereo0DLength * _stereo0DSize));
                    try
                    {
                        long addr = stereoDataPtr.ToInt64();
                        for (int i = 0; i < stereo0DLength; i++, addr += _stereo0DSize)
                        {
                            Marshal.StructureToPtr(stereoData[i], new IntPtr(addr), true);
                        }
                    }
                    catch
                    {
                        Marshal.FreeHGlobal(stereoDataPtr);
                        throw;
                    }
                    StereoDataPtr = stereoDataPtr.ToPointer();
                }
            }
            if (!string.IsNullOrEmpty(options))
            {
                Options = Marshal.StringToHGlobalAnsi(options).ToPointer();
            }
        }
        
        public InChIStrucInput(InChIAtom[] atoms, string options)
            : this(atoms,null,options)
        {}
        
        private readonly void* AtomsPtr;
        private readonly void* StereoDataPtr;
        public readonly void* Options;
        public readonly short NumAtoms;
        public readonly short NumStereo0D;

        /// <summary>
        /// Releases all unmanaged resources used by this structure.
        /// </summary>
        public void Dispose()
        {
            var atomsPtr = new IntPtr(AtomsPtr);
            if (atomsPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(atomsPtr);
            }

            var stereoDataPtr = new IntPtr(StereoDataPtr);
            if (stereoDataPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(stereoDataPtr);
            }

            var optionsPtr = new IntPtr(Options);
            if (optionsPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(optionsPtr);
            }
        }
    }//end struct InChIStrucInput
}
