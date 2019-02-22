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
    public struct InChIStrucInput : IDisposable
    {
        /// <summary>
        /// Creates a new instance of this structure with no
        /// stereo data and an empty options string.
        /// </summary>
        /// <param name="atoms"></param>
        public InChIStrucInput(IEnumerable<InChIAtom> atoms) : this(atoms,"")
        { 
            
        }

        /// <summary>
        /// Creates a new instance from the specified inputs. See inchi_options.txt for a complete list of options.
        /// </summary>
        /// <param name="atoms">the atoms contained by this structure</param>
        /// <param name="stereoData">the stereo data</param>
        /// <param name="options">the space delimited options string</param>
        public InChIStrucInput(IEnumerable<InChIAtom> atoms, IEnumerable<InChIStereo0D> stereoData, string options)
            : this()
        {
            SetAtoms(atoms);
            SetStereoData(stereoData);
            Options = options;
        }

        /// <summary>
        /// Creates a new InChIStrucInput with no stereo data. See inchi_options.txt for a complete list of options.
        /// </summary>
        /// <param name="atoms">an IEnumerable of atoms to add to this structure</param>
        /// <param name="options">the space delimited options string</param>
        public InChIStrucInput(IEnumerable<InChIAtom> atoms, string options)
            : this(atoms,null,options)
        {}

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
        /// The space delimited string of options. Options start with
        /// '-' or '/' depending on the platform.
        /// </summary>
        public string Options
        {
            get { return opt; }
            set { opt = value; }
        }

        private string opt;

        /// <summary>
        /// The number of AtomsPtr in the structure. Max value is
        /// 1023.
        /// </summary>
        public short NumAtoms
        {
            get { return numAtoms; }
            set { numAtoms = value; }
        }

        private short numAtoms;

        /// <summary>
        /// The number of Stereo0D structures.
        /// </summary>
        public short NumStereo0D
        {
            get { return numStereo; }
            set { numStereo = value; }
        }

        private short numStereo;

        /// <summary>
        /// Converts an IEnumerable of InChI_Atoms into a series of pointers
        /// in unmanaged memory and sets the internal pointer used
        /// when this structure is marshaled 
        /// </summary>
        /// <param name="atoms">the array of InChI_Atoms to set.</param>
        public void SetAtoms(IEnumerable<InChIAtom> atoms)
        {
            foreach (InChIAtom a in atoms)
                AddAtom(a);
        }

        /// <summary>
        /// Converts an IEnumerable of InChIStereo0D structures into a series
        /// of pointers in unmanaged memory and sets the internal pointer used
        /// when this structure is marshaled. 
        /// </summary>
        /// <param name="stereoData">the array of InChIStereo0D structures</param>
        public void SetStereoData(IEnumerable<InChIStereo0D> stereoData)
        {
            if (stereoData != null)
            {
                foreach (InChIStereo0D st in stereoData)
                {
                    AddStereo0D(st);
                }
            }
        }

        /// <summary>
        /// Adds an InChIStereo0D to this structure.
        /// </summary>
        /// <param name="stereo"></param>
        public void AddStereo0D(InChIStereo0D stereo)
        {
            int stSize = Marshal.SizeOf(stereo);

            if (StereoDataPtr == IntPtr.Zero)
            {
                StereoDataPtr = Marshal.AllocHGlobal(stSize);
                Marshal.StructureToPtr(stereo, StereoDataPtr, false);
            }
            else
            {
                StereoDataPtr = Marshal.ReAllocHGlobal(StereoDataPtr, new IntPtr((NumStereo0D + 1) * stSize));
                IntPtr p = new IntPtr(StereoDataPtr.ToInt64() + NumStereo0D * stSize);
                Marshal.StructureToPtr(stereo, p, true);
            }
            numStereo++;
        }

        /// <summary>
        /// Adds an InChIAtom to this structure. 
        /// </summary>
        /// <param name="atom"></param>
        public void AddAtom(InChIAtom atom)
        {
            int atomSize = Marshal.SizeOf(atom);

            if (AtomsPtr == IntPtr.Zero)
            {
                AtomsPtr = Marshal.AllocHGlobal(atomSize);
                Marshal.StructureToPtr(atom, AtomsPtr, false);
            }
            else
            {
                AtomsPtr = Marshal.ReAllocHGlobal(AtomsPtr, new IntPtr((NumAtoms + 1) * atomSize));
                IntPtr p = new IntPtr(AtomsPtr.ToInt64() + NumAtoms * atomSize);
                Marshal.StructureToPtr(atom, p, true);
            }
            numAtoms++;
        }

        /// <summary>
        /// Releases all unmanaged resources used by this structure.
        /// </summary>
        public void Dispose()
        {
            if (AtomsPtr != IntPtr.Zero || this.StereoDataPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(AtomsPtr);
                Marshal.FreeHGlobal(StereoDataPtr);
                
                AtomsPtr = IntPtr.Zero;
                StereoDataPtr = IntPtr.Zero;
                
                GC.SuppressFinalize(this);
            }
        }
    }//end struct InChIStrucInput
}
