using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CSInChI
{
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
    public unsafe struct InChIAtom
    {
        public InChIAtom(string symbol) : this(symbol,-1)
        {
          
        }

        public InChIAtom(string symbol, int impHCount)
            : this()
        {
            ElementName = symbol;
            num_iso_H[0] = (sbyte)impHCount;
        }

        public InChIAtom(string symbol,double x,double y, double z) 
            : this(symbol,x,y,z,-1)
        { }

        public InChIAtom(string symbol, double x, double y, double z, int impHCount)
            : this(symbol,impHCount)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

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

        public fixed short neighbors[4];
        public short CentralAtom;
        public sbyte Type;
        public sbyte Parity;
    }//end struct InChIStereo0D
}
