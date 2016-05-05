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
 * of the License; or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful;
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not; write to the Free Software
 * Foundation; Inc.; 51 Franklin St; Fifth Floor; Boston; MA 02110-1301 USA. 
 */

using System;

namespace CSInChI
{
    /// <summary>
    /// A set of constants for the error codes returned by the
    /// LibInChI.GetInChI, GetInChIFromInChI, and ParseInChI methods. 
    /// </summary>
    /// <seealso cref="LibInChI.GetInChI"/>
    /// <seealso cref="LibInChI.ParseInChI"/>
    public static class InChIRetVal
    {
        /// <summary>
        /// Not used in the InChI library. The value is included only for completeness.
        /// </summary>
        public const int SKIP = -2;
        
        /// <summary>
        /// -1  = No structural data has been provided
        /// </summary>
        public const int EOF = -1;
        
        /// <summary>
        /// 0 = Success; no errors or warnings 
        /// </summary>
        public const int OKAY = 0;
        
        /// <summary>
        /// 1 = Success; warning(s) issued
        /// </summary>
        public const int WARNING = 1;
        
        /// <summary>
        /// 2 = Error: no InChI has been created 
        /// </summary>
        public const int ERROR = 2;

        /// <summary>
        /// 3 = Severe error: no InChI has been created (typically; memory allocation failure) 
        /// </summary>
        public const int FATAL = 3;

        /// <summary>
        /// 4 = Unknown program error 
        /// </summary>
        const int UNKNOWN = 4;
        
        /// <summary>
        /// 5 = A previous call to InChI has not returned yet.
        /// </summary> 
        const int BUSY = 5;

        /// <summary>
        /// Converts a return code from the ParseInChI and GetInChI
        /// methods to a string representation of the value.
        /// </summary>
        /// <param name="retVal"></param>
        /// <returns></returns>
        public static string GetStringVal(int retVal)
        {
            switch (retVal)
            {
                case -1: return "EOF_NO_STRUCT_DATA";
                case 0: return "OKAY";
                case 1: return "OKAY_WARNING(S)_ISSUED";
                case 2: return "ERROR_NO_VAL_RETURNED";
                case 3: return "FATAL_ERROR";
                case 4: return "UNKNOWN ERROR";
                case 5: return "LIBRARY_BUSY";
                default : return "Not a valid return code";
            }
               
        }
    }//end class InChIRetVal

    /// <summary>
    /// A set of constants corresponding to InChI bond types.
    /// </summary>
    /// <seealso cref="InChIAtom"/>
    public static class BondType
    {
        /// <summary>
        /// 0 = No bond type specified.
        /// </summary>
        const int NONE = 0;

        /// <summary>
        /// 1 = A single bond.
        /// </summary>
        public const int SINGLE = 1;
        
        /// <summary>
        /// 2 = A double bond.
        /// </summary>
        public const int DOUBLE = 2;
        
        /// <summary>
        /// 3 = A triple bond.
        /// </summary>
        public const int TRIPLE = 3;

        /// <summary>
        /// 4 = An unspecified bond type.
        /// Avoid using this constant by all means.
        /// </summary>
        public const int ALTERN = 4;
    }//end class BondType

    /// <summary>
    /// A set of constants corresponding to InChI Radical types.
    /// </summary>
    /// <seealso cref="InChIAtom"/>
    public static class RadicalType
    {
        /// <summary></summary>
        public const int NONE    = 0;
        /// <summary></summary>
        public const int SINGLET = 1;
        /// <summary></summary>
        public const int DOUBLET = 2;
        /// <summary></summary>
        public const int TRIPLET = 3;
    }

    
    /// <summary>
    /// A set of constants defining the 0D stereo parities
    /// used by the InChI library.
     ///
    ///  Stereo_Parties.txt for complete details on how to use
    ///  0D stereo parities.
    /// </summary>
    /// <seealso cref="InChIStereo0D"/>
    public static class StereoParity0D
    {
        /// <summary>
        /// 
        /// </summary>
        public const int INCHI_PARITY_NONE = 0;
        
        /// <summary>
        /// 
        /// </summary>
        public const int INCHI_PARITY_ODD = 1; 
        public const int INCHI_PARITY_EVEN = 2;
        public const int INCHI_PARITY_UNKNOWN = 3;

        /// <summary>
        /// Stereo parity is undefined. This value should
        /// only be used when only disconnected structure parity exists.
        /// </summary>
        public const int INCHI_PARITY_UNDEFINED = 4;
    }//end class StereoParity0D

    /// <summary>
    /// A set of constants defining the types of stereo center
    /// handled by the InChI library.
    /// </summary>
    /// <seealso cref="InChIStereo0D"/>
    public static class StereoType0D
    {
        /// <summary>
        /// 
        /// </summary>
        public const int NONE = 0;
        
        /// <summary>
        /// Indicates stereo chemistry about a double bond.
        /// The central atom index should be set to -1 when
        /// using this stereo type.
        /// </summary>
        public const int DOUBLEBOND = 1;
     
        /// <summary>
        /// Indicates a tetrahedral the stereo center 
        /// </summary>
        public const int TETRAHEDRAL = 2;

        /// <summary>
        /// Stereo type used for allenes.
        ///  The central atom index should be set to -1 when
        /// using this stereo type.
        /// </summary>
        public const int ALLENE = 3;
    }//end class StereoType0D

    /// <summary>
    /// A set of constants defining the types of 2D stereo
    /// bond used by this InChI library. These values are used only if all neighbors of 
    /// this atom have same z-coordinate.
    /// as this atom
    /// </summary>
    public static class InChIBondStereo2D
    {
        /* stereocenter-related; positive: the sharp end points to this atom  */
       public const int  INCHI_BOND_STEREO_NONE           =  0;
        public const int INCHI_BOND_STEREO_SINGLE_1UP     =  1;
        public const int INCHI_BOND_STEREO_SINGLE_1EITHER =  4;
        public const int INCHI_BOND_STEREO_SINGLE_1DOWN   =  6;
         /* stereocenter-related; negative: the sharp end points to the opposite atom  */
        public const int INCHI_BOND_STEREO_SINGLE_2UP     = -1;
        public const int INCHI_BOND_STEREO_SINGLE_2EITHER = -4;
        public const int INCHI_BOND_STEREO_SINGLE_2DOWN   = -6;
        
        /// <summary>
        /// Unknown stereobond geometry
        /// </summary>
        public const int INCHI_BOND_STEREO_DOUBLE_EITHER = 3;
    }

    /// <summary>
    /// A set of constants corresponding to error codes
    /// returned by the GetINCHIKey
    /// </summary>
    /// <seealso cref="LibInChI.GetInChIKey"/>
    public static class GetInChIKeyRetVal
    {
        /// <summary>
        /// 0 = The InChI key was successully generated. 
        /// </summary>
        public const int OKAY =0;
        
        /// <summary>
        /// An error code return when an unspecified error
        /// occurs when calculating the InChI key. 
        /// </summary>
        public const int UNKNOWN_ERROR =1;
        
        /// <summary>
        /// The error code returned when input string had a null value.
        /// </summary>
        public const int EMPTY_INPUT =2;

        /// <summary>
        /// The error code returned when the input string is not a
        /// valid InChI string.
        /// </summary>
        public const int NOT_INCHI_INPUT =3;
        
        /// <summary>
        /// An error code returned when the InChI library
        /// function cannot allocate the memory required
        /// to compute an InChI key.
        /// </summary>
        public const int NOT_ENOUGH_MEMORY =4;
        
        /// <summary>
        /// The error code returned if the length of the input InChI
        /// is less than 9. 
        /// </summary>
        public const int ERROR_IN_FLAG_CHAR =5;

        /// <summary>
        /// Gets a string representation of the return code
        /// from the GetInChIKey method
        /// </summary>
        /// <param name="retVal"></param>
        /// <returns></returns>
        /// <seealso cref="LibInChI.GetInChIKey"/>
        public static string GetStringVal(int retVal)
        {
            switch (retVal)
            {
                case 0: return "OKAY";
                case 1: return "UNKNOWN_ERROR";
                case 2: return "EMPTY_INPUT";
                case 3: return "NOT_INCHI_INPUT";
                case 4: return "NOT_ENOUGH_MEMORY";
                case 5: return "ERROR_IN_FLAG_CHAR";
                default: return "Not a valid return code";
            }
        }
    }//end class GetInChIKeyRetVal

    /// <summary>
    /// A set of constants for the return values of the
    /// CheckInChIKey method
    /// </summary>
    /// <seealso cref="LibInChI.CheckInChIKey"/>
    public static class CheckInChIKeyResult
    { 
        /// <summary>
        /// The input value was a valid InChI key.
        /// </summary>
        public const int VALID_KEY =0;
        
        /// <summary>
        /// Error code indicating that the length of the input key was not 25
        /// characters.
        /// </summary>
        public const int INVALID_LENGTH =1;

        /// <summary>
        /// Error code indicating that the key had 1 or more
        /// invalid characters. This return code indicates that one of the following is true
        /// 
        /// The 14th char is not a '-'
        /// A character other than an upper case letter is present in the rest of the string 
        /// The key has an 'E' at position 0, 3, 6, 9, 15, or 18
        /// </summary>
        public const int INVALID_LAYOUT =2;

        /// <summary>
        /// Error code indicating that the check character (last character in the key)
        /// does not match the check sum calculated by CheckInChIKey.
        /// </summary>
        public const int INVALID_CHECKSUM =3;

        /// <summary>
        /// Converts a return code from the CheckInChIKey
        /// method to a string representation of the value.
        /// </summary>
        /// <param name="retVal"></param>
        /// <returns></returns>
        public static string GetStringVal(int retVal)
        {
            switch (retVal)
            { 
                case 0 : return "VALID_KEY";
                case 1 : return "INVALID_LENGTH";
                case 2 : return "INVALID_LAYOUT";
                case 3 : return "INVALID_CHECKSUM";
                default : return "Not a valid return code";
            }
        }

    }//end class CheckInChIKeyResult

}
