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
    /// A structure that holds atom data for input or output from InChI 
    /// library functions. Be sure to inital all arrays to the size
    /// specified by the MarshalAs attribute.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
    public unsafe struct InChIAtom
    {
        /// <summary>
        /// Creates a new InChIAtom with the specified symbol and leaves setting the 
        /// HCounts to the InChI library.
        /// </summary>
        /// <param name="symbol"></param>
        public InChIAtom(string symbol) : this(symbol,-1)
        {
          
        }

        /// <summary>
        /// Creates a new InChI atom using the symbol and implicit H count.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="impHCount"></param>
        public InChIAtom(string symbol, int impHCount)
            : this()
        {
            ElementName = symbol;
            num_iso_H[0] = (sbyte)impHCount;
        }

        /// <summary>
        /// Creates a new InChIAtom with the specified symbol and coordinates. 
        /// Implicit HCounts are set by InChI library.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public InChIAtom(string symbol,double x,double y, double z) 
            : this(symbol,x,y,z,-1)
        { }

        /// <summary>
        /// Creates a new InChIAtom with the specified symbol and coordinates. 
        /// Implicit HCounts are set by InChI library.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// /// <param name="impHCount"></param>
        public InChIAtom(string symbol, double x, double y, double z, int impHCount)
            : this(symbol,impHCount)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Creates a new InChIAtom with all possible values explicitly specified.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="neighborIndices"></param>
        /// <param name="numBonds"></param>
        /// <param name="bondTypes"></param>
        /// <param name="bondStereo"></param>
        /// <param name="hCounts"></param>
        /// <param name="isotopicMass"></param>
        /// <param name="radical"></param>
        /// <param name="charge"></param>
        public InChIAtom(string symbol, double x, double y, double z, short[] neighborIndices, short numBonds, sbyte[] bondTypes, sbyte[] bondStereo, sbyte[] hCounts,
            byte isotopicMass, sbyte radical, sbyte charge) : this(symbol)
        {

            Neighbors = neighborIndices;

            BondStereo = bondStereo;
            NumBonds = numBonds;

            HCounts = hCounts;
            BondTypes = bondTypes;
            Charge = charge;
            Radical = radical;
            IsotopicMass = isotopicMass;
        }

        public double x;
        public double y;
        public double z;
        public fixed short neighbor[20];
        public fixed sbyte bond_type[20];    /* inchi_BondType */
        public fixed sbyte bond_stereo[20];
        public fixed sbyte elname[6];
        short num_bonds;
        public fixed sbyte num_iso_H[4];
        public short IsotopicMass;
        public sbyte Radical;
        public sbyte Charge;

        

        /// <summary>
        /// An array containing the indices of neighboring atoms.
        /// The array must always have a length of 20 if it is initalized in managed code.
        /// </summary>
        public short[] Neighbors
        {
            get
            {
                fixed (short* n = neighbor)
                {
                    return new Span<short>(n, 20).ToArray();
                }
            }
            set
            { 
                if(value.Length != 20)
                    throw new ArgumentException("The array 'Neighbors' must have a length of 20 to be correctly marshalled");
                
                bool okayInput = Array.TrueForAll(value, delegate(short val)
                {
                    if (val > 1023 || val < 0)
                        return false;
                    return true;
                });

                fixed (short* n = neighbor)
                {
                    value.AsSpan().CopyTo(new Span<short>(n, 20));
                }
            }
        }

        /// <summary>
        /// An array containing the data on the types of bonds
        /// the atom is involved in. Values are defined in the
        /// InChI_Bond_Type enumeration. The array must always 
        /// have a length of 20 if it is initalized in managed code.
        /// </summary>
        public sbyte[] BondTypes
        {
            get
            {
                fixed (sbyte* n = bond_type)
                {
                    return new Span<sbyte>(n, 20).ToArray();
                }
            }
            set
            {
                if (value.Length != 20)
                    throw new ArgumentException("The array 'BondTypes' must have a length of 20 to be correctly marshalled");

                fixed (sbyte* n = bond_type)
                {
                    value.AsSpan().CopyTo(new Span<sbyte>(n, 20));
                }
            }
        }
       
        /// <summary>
        ///The stereo data if for the bonds the atom is involved in.
        ///The types are defined in the InChIBondStereo2D enumeration.
        ///The array must always have a length of 20 if it is initalized
        ///in managed code.
        /// </summary>
        public sbyte[] BondStereo
        {

            get
            {
                fixed (sbyte* n = bond_stereo)
                {
                    return new Span<sbyte>(n, 20).ToArray();
                }
            }
            set
            {
                if (value.Length != 20)
                    throw new ArgumentException("The array 'bond_stereo' must have a length of 20 to be correctly marshalled");

                fixed (sbyte* n = bond_stereo)
                {
                    value.AsSpan().CopyTo(new Span<sbyte>(n, 20));
                }
            }
        }

        /// <summary>
        /// The element name. A maximum of 6 characters.
        /// </summary>
        public string ElementName
        {
            get
            {
                fixed (sbyte* e = elname)
                {
                    return new string(e);
                }
            }
            set
            {
                if (value.Length > 6)
                    throw new ArgumentException("The maximum length of the element name is 6");

                for (int i = 0; i < value.Length; i++)
                {
                    elname[i] = (sbyte)value[i];
                }
            }
        }

        /// <summary>
        /// The number of bonds the atom is involved in.
        /// </summary>
        public short NumBonds
        {
            get { return num_bonds; }
            set
            {
                if (num_bonds > 20)
                    throw new ArgumentException("The maximum number of bonds an InChIAtom can have is 20");

                num_bonds = value;
            }

        }

        /// <summary>
        /// The number of implicit hydrogen atoms. The array must always 
        /// have a length of 4.
        ///  [0]: number of implicit non-isotopic H
        ///      (exception: HCounts[0]=-1 means INCHI
        ///      adds implicit H automatically),
        ///  [1]: number of implicit isotopic 1H (protium),
        ///  [2]: number of implicit 2H (deuterium),
        ///  [3]: number of implicit 3H (tritium) */
        /// </summary>
        /// 
        public sbyte[] HCounts
        {
            get
            {
                fixed (sbyte* n = num_iso_H)
                {
                    return new Span<sbyte>(n, 4).ToArray();
                }
            }
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The array 'num_iso_H' must have a length of 4 to be correctly marshalled");

                fixed (sbyte* n = num_iso_H)
                {
                    value.AsSpan().CopyTo(new Span<sbyte>(n, 4));
                }
            }
        }

    }//end struct InChIAtom

   
    /// <summary>
    /// A structure that holds stereo data for an InChIAtom.
    /// </summary>
    /// <seealso cref="StereoParity0D"/>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct InChIStereo0D 
    {
        /// <summary>
        /// An array containg the indices of the neighbors of a stereo center.
        /// The array must always have a length of 4.
        /// </summary>
        public short[] Neighbors
        {
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The array 'Neighbors' must have length 4");

                neighbors[0] = value[0];
                neighbors[1] = value[1];
                neighbors[2] = value[2];
                neighbors[3] = value[3];
            }
        }

        public short CentralAtom
        {
            get { return centAtom; }

            set
            {
                if (value > 1023 || value < -1)
                    throw new ArgumentException("The maximum allowable atom index is 1023");

                centAtom = value;
            }
        }

        public fixed short neighbors[4];
        public short centAtom;
        public sbyte Type;
        public sbyte Parity;
    }//end struct InChIStereo0D
}
