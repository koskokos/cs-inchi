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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;


namespace CSInChI
{

    /// <summary>
    /// A class providing access to the functions in the IUPAC InChI library.
    /// </summary>
    /// <example>This examples demonstrates how to use the various methods defined by the LibInChI class.
    /// This code takes an InChI string, calculates the InChI key, and outputs the structure as an sdf file 
    /// with the InChI key as a property. Note the use of the try/finally pattern that ensures the disposal
    /// of unmanaged memory even if an exception is thrown.
    /// <code>

    /// /*The output is:
    ///
    ///Structure #1
    ///   InChI v1 SDfile Output
    ///   
    /// 12 11  0  0  0  0  0  0  0  0  2 V2000
    ///        0.0000    0.0000    0.0000 C   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 C   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 C   0  5  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 C   0  3  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  5  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  5  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 H   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 H   0  0  0     0  0  0  0  0  0
    ///  1 11  1  0  0  0  0
    ///  1  2  1  0  0  0  0
    ///  1  3  1  0  0  0  0
    ///  1  5  1  0  0  0  0
    ///  2 12  1  0  0  0  0
    ///  2  4  1  0  0  0  0
    ///  2  6  1  0  0  0  0
    ///  3  7  1  0  0  0  0
    ///  3  8  1  0  0  0  0
    ///  4  9  1  0  0  0  0
    ///  4 10  1  0  0  0  0
    ///M  CHG  4   3  -1   4   1   8  -1  10  -1
    ///M  END
    ///> &lt;INCHIKEY&gt;
    ///YMKUERMSEJFJHY-XIXRPRMCBM
    ///$$$$
    ///*/
    ///</code>
    ///<code>
    ///import clr
    ///
    ///clr.AddReference("CSInChI.dll")
    ///
    ///from clr import Reference
    ///
    ///from System.IO import File
    ///from CSInChI import *
    ///
    ///inchi = "InChI=1/C4H6O6/c5-1(3(7)8)2(6)4(9)10/h1-2,5-7,9H/q-2/t1-,2+"
    ///
    ///#create a Reference object to hold a structure passed with the out keyword
    ///#in CLR languages
    ///outStructRef = Reference[InChIStrucOutput]()
    ///inStruct = InChIStrucInput()
    ///
    ///try:
    ///
    ///    LibInChI.ParseInChI(inchi, outStructRef)
    ///
    ///    #get the InChI key
    ///    key = LibInChI.GetInChIKey(inchi)
    ///
    ///    #create structure data to generate the sdf file text
    ///    inStruct.SetAtoms(outStructRef.Value.GetAtoms())
    ///    inStruct.SetStereoData(outStructRef.Value.GetStereoData())
    ///    inStruct.Options = "/outputsdf"
    ///
    ///    inStrucRef = Reference[InChIStrucInput](inStruct)
    ///    sdfText = LibInChI.GetInChI(inStrucRef)
    ///
    ///    #append the key to the file text as a property
    ///    key = "> &lt;INCHIKEY&gt;\n" + key +"\n"
    ///
    ///    sdfText = sdfText[:-4] + key + sdfText[-4:]
    ///
    ///    File.WriteAllText("struc1.sdf",sdfText);
    ///
    ///
    ///finally:
    ///    outStructRef.Value.Dispose()
    ///    inStruct.Dispose()
    ///
    ///
    ///"""
    /// The output is:
    /// 
    ///Structure #1
    ///   InChI v1 SDfile Output
    ///   
    /// 12 11  0  0  0  0  0  0  0  0  2 V2000
    ///        0.0000    0.0000    0.0000 C   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 C   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 C   0  5  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 C   0  3  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  5  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 O   0  5  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 H   0  0  0     0  0  0  0  0  0
    ///        0.0000    0.0000    0.0000 H   0  0  0     0  0  0  0  0  0
    ///  1 11  1  0  0  0  0
    ///  1  2  1  0  0  0  0
    ///  1  3  1  0  0  0  0
    ///  1  5  1  0  0  0  0
    ///  2 12  1  0  0  0  0
    ///  2  4  1  0  0  0  0
    ///  2  6  1  0  0  0  0
    ///  3  7  1  0  0  0  0
    ///  3  8  1  0  0  0  0
    ///  4  9  1  0  0  0  0
    ///  4 10  1  0  0  0  0
    ///M  CHG  4   3  -1   4   1   8  -1  10  -1
    ///M  END
    ///> &lt;INCHIKEY&gt;
    ///YMKUERMSEJFJHY-XIXRPRMCBM
    ///$$$$
    ///"""
    /// </code>
    ///</example>
    public static class LibInChI
    {
#if WINDOWS
        const string libInchiName = "libinchi.dll";
#else
        const string libInchiName = "libinchi.so.1.05.00";
#endif

        static void Main(string[] args)
        {
            string inchi = "InChI=1S/C4H6O6/c5-1(3(7)8)2(6)4(9)10/h1-2,5-7,9H/q-2/t1-,2+";
            InChIStrucOutput outStruc = new InChIStrucOutput();
            InChIStrucInput inStruc = new InChIStrucInput();

            try
            {
                LibInChI.ParseInChI(inchi, out outStruc);

                //get the InChI key
                string key = LibInChI.GetInChIKey(inchi);

                //create structure data to generate the sdf file text
                var a = outStruc.GetAtoms();
                var b = a.ToArray().Select(s => s.ElementName).ToArray();
                inStruc.SetAtoms(a.ToArray());
                var stereo = outStruc.GetStereoData();
                inStruc.SetStereoData(stereo);
                //inStruc.Options = "/outputsdf";

                string sdfText = LibInChI.GetInChI(ref inStruc);

                //append the key to the file text as a property
                key = "> &lt;INCHIKEY&gt;\n" + key + "\n";
                sdfText = sdfText.Insert(sdfText.Length - 4, key);

                File.WriteAllText("struc1.sdf", sdfText);
            }
            finally
            {
                //free the unmanaged memory
                outStruc.Dispose();
                inStruc.Dispose();
            }
        }

        /// <summary>
        /// Constant defined in inchi_api.h
        /// 
        ///  Add to isotopic mass if isotopic_mass =     
        ///  (isotopic mass - average atomic mass)
        /// </summary>
        public const int ISOTOPIC_SHIFT_FLAG = 10000;

        /// <summary>
        /// Constant defined in inchi_api.h
        ///
        /// max abs(isotopic mass - average atomic mass)
        /// </summary>
        public const int ISOTOPIC_SHIFT_MAX = 100;

        /// <summary>
        /// Constant defined in inchi_api.h (MAXVAL)
        /// The maximum number of bonds an atom can be involved in.
        /// </summary>
        public const int MAX_BONDS = 20;

        /// <summary>
        /// Constant defined in inchi_api.h
        /// The number of isotopes of hydrogen
        /// </summary>
        public const int NUM_H_ISOTOPES = 3;

        /// <summary>
        ///  Constant defined in inchi_api.h
        ///  The length of the char array which stores the
        ///  symbol for an element.
        /// </summary>
        public const int ATOM_EL_LEN = 6;

        /// <summary>
        /// Constant defined in inchi_api.h.
        /// The value to use for the central atom
        /// of an InChIStereo0D structure if the stereo
        /// type is not allene or tetrahedral.
        /// </summary>
        public const short NO_ATOM = -1;

        /// <summary>
        /// An external method that calls the GetStructFromINCHI function
        /// in the IUPAC InChI library. The unmanaged memory is deallocated
        /// when the Dispose method of the InChIStrucOutput is called.
        /// </summary>
        /// <param name="input">the structure that holds the input data</param>
        /// <param name="output">the structure that holds the output</param>
        /// <returns>an error code indicating the success or failure of the function call</returns>
        /// <seealso cref="LibInChI.ParseInChI"/>
        /// <example>This example illustrates the use of this method.
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
        ///            // Console.WriteLine(InChIRetVal.GetStringVal(ret));
        ///            
        ///            foreach (InChIAtom atom in outStruct.GetAtoms())
        ///            {
        ///                // Console.WriteLine(atom.ElementName + " " + atom.NumBonds);
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
        [DllImport(libInchiName, EntryPoint = "GetStructFromINCHI")]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ParseInChI(ref InChIStringInput input, out InChIStrucOutput output);

        /// <summary>
        /// An external method that calls the GetINCHI function
        /// in the IUPAC InChI library. The unmanaged memory is deallocated 
        /// when the Dispose method of the InChIStringOutput is called. It is recommended
        /// that programmers use one of the overloads that returns the InChI string as these
        /// will automatically deallocate the InChIStringOutput structure.
        /// </summary>
        /// <param name="structData">the structure that holds the input structure data</param>
        /// <param name="output">the structure that holds the InChI output</param>
        /// <returns>an error code indicating the success or failure of the function call</returns>
        [DllImport(libInchiName, EntryPoint = "GetStdINCHI")]
        [SuppressUnmanagedCodeSecurity]
        public static extern int GetInChI(ref InChIStrucInput structData, out InChIStringOutput output);

        /// <summary>
        /// An external method that calls the GetINCHIKeyFromINCHI function
        /// in the IUPAC InChI library. The StringBuilder that holds the output
        /// should be initialized with a capacity of 25. Requires version 1.0.2 or later of
        /// the library. 
        /// </summary>
        /// <param name="inchi">the inchi string</param>
        /// <param name="sb">a StringBuilder to hold the InChI key output</param>
        /// <returns>an error code indicating the success or failure of the function call</returns>
        [DllImport(libInchiName, EntryPoint = "GetStdINCHIKeyFromStdINCHI")]
        public static extern int GetInChIKey([MarshalAs(UnmanagedType.LPStr)]string inchi, [MarshalAs(UnmanagedType.LPStr)]StringBuilder sb);

        /// <summary>
        /// An external method that calls the CheckInChIKey function
        /// in the IUPAC InChI toolkit. Requires version 1.0.2 or later of
        /// the library.
        /// </summary>
        /// <seealso cref="CheckInChIKeyResult"/>
        /// <param name="inchiKey">the inchi key</param>
        /// <returns>an error code indicating the success or failure of the function call</returns>
        ///<example>This example illustrates the different possible return codes from this
        ///method.
        ///<code>
        ///static void Main()
        ///{
        ///    List&lt;string&gt; inchiKeys = new List&lt;string&gt;()
        ///    {
        ///        //valid key
        ///        "WFLOTYSKFUPZQB-OWOJBTEDBV",
        ///        //invalid length - leading and trailing white space
        ///        "  WFLOTYSKFUPZQB - OWOJBTEDBV  ",
        ///        //invalid check character 'A' should be 'V'
        ///        "WFLOTYSKFUPZQB-OWOJBTEDBA",
        ///        //key is lowercase
        ///        "wflotyskfupzqb-owojbtedbv"
        ///    };
        ///
        ///    int retCode;
        ///    string checkVal;
        ///    foreach (string key in inchiKeys)
        ///    {
        ///        retCode = LibInChI.CheckInChIKey(key);
        ///        checkVal = CheckInChIKeyResult.GetStringVal(retCode);
        ///        // Console.WriteLine("{0} -> {1}\n", key, checkVal);
        ///    }
        ///} 
        ///
        ///  
        /// /*Console output:  
        ///   WFLOTYSKFUPZQB-OWOJBTEDBV -> VALID_KEY
        ///
        ///   WFLOTYSKFUPZQB - OWOJBTEDBV   -> INVALID_LENGTH
        ///
        ///   WFLOTYSKFUPZQB-OWOJBTEDBA -> INVALID_CHECKSUM
        ///
        ///   wflotyskfupzqb-owojbtedbv -> INVALID_LAYOUT
        /// */
        /// </code>
        /// <code>
        /// import clr
        ///
        ///clr.AddReference("CSInChI.dll")
        ///
        ///from CSInChI import LibInChI
        ///from CSInChI import CheckInChIKeyResult
        ///
        ///inchiKeys =[#valid key
        ///           "WFLOTYSKFUPZQB-OWOJBTEDBV",
        ///           #invalid length - leading and trailing white space
        ///           "  WFLOTYSKFUPZQB - OWOJBTEDBV  ",
        ///           #invalid check character
        ///           "WFLOTYSKFUPZQB-OWOJBTEDBA",
        ///           #key is lowercase
        ///           "wflotyskfupzqb-owojbtedbv"]
        ///
        ///for key in inchiKeys:
        ///    checkVal = LibInChI.CheckInChIKey(key)
        ///    print key + " -> " + CheckInChIKeyResult.GetStringVal(checkVal) + "\n"
        ///    
        /// """ Output:
        ///   WFLOTYSKFUPZQB-OWOJBTEDBV -> VALID_KEY
        ///
        ///   WFLOTYSKFUPZQB - OWOJBTEDBV   -> INVALID_LENGTH
        ///
        ///   WFLOTYSKFUPZQB-OWOJBTEDBA -> INVALID_CHECKSUM
        ///
        ///   wflotyskfupzqb-owojbtedbv -> INVALID_LAYOUT
        /// """
        /// 
        /// </code>
        /// </example>
        /// <seealso cref="CheckInChIKeyResult"/>
        [DllImport(libInchiName, EntryPoint = "CheckINCHIKey")]
        public static extern int CheckInChIKey(string inchiKey);

        /// <summary>
        /// Frees the unmanaged memory used allocated by the
        /// ParseInChI method. It is unecessary to explicitly call this method
        /// as it is called in the Dispose method of the InChIStrucOutput structure.
        /// </summary>
        /// <param name="outputStruct">the InChIStrucOutput to deallocate </param>
        [DllImport(libInchiName, EntryPoint = "FreeStructFromINCHI")]
        internal static extern void DeallocateOutputStruct(ref InChIStrucOutput outputStruct);

        /// <summary>
        /// Frees the unmanaged memory allocated by the GetInChI method.It is unecessary to explicitly call this method
        /// as it is called in the Dispose method of the InChIStringOutput structure.
        /// </summary>
        /// <param name="inchiOut">the InChIStringOutput to deallocate</param>
        [DllImport(libInchiName, EntryPoint = "FreeINCHI")]
        internal static extern void DeallocateInChIString(ref InChIStringOutput inchiOut);

        /// <summary>
        /// An external method that calls the GetINCHIFromINCHI function
        /// in the IUPAC InChI library.
        /// </summary>
        /// <param name="inp">the input inchi</param>
        /// <param name="outStruct">the struct that will hold the output</param>
        /// <returns>an error code indicating the success or failure of the function call</returns>
        [DllImport(libInchiName, EntryPoint = "GetINCHIfromINCHI")]
        public static extern int GetInChIFromInChI(ref InChIStringInput inp, out InChIStringOutput outStruct);

        /// <summary>
        /// A convenience overload of ParseInChI that takes 2 strings
        /// rather than a InChIStringInput. The user is responsible for
        /// calling the Dispose method of the InChIStrucOutput structure to free
        /// unmanaged memory. Options start with '/' in Windows or 
        /// '-' for other platforms. See inchi_options.txt for a complete list of options.
        /// </summary>
        /// <param name="inchi">the inchi string</param>
        /// <param name="options">the space delimited option string</param>
        /// <param name="outStruct">the structure that will hold the output</param>
        /// <returns>an error code indicating the success or failure of the function call</returns>
        ///    
        /// <example>This example illustrates the use of this method.
        /// <code>
        ///class Program
        ///{
        ///    static void Main(string[] args)
        ///    {
        ///        string inchi = "InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1+";
        ///        
        ///        InChIStrucOutput outStruct = new InChIStrucOutput();
        ///
        ///        try
        ///        {
        ///            int ret = LibInChI.ParseInChI(inchi, out outStruct);
        ///            // Console.WriteLine(InChIRetVal.GetStringVal(ret));
        ///
        ///            foreach (InChIAtom atom in outStruct.GetAtoms())
        ///            {
        ///                // Console.WriteLine(atom.ElementName + " " + atom.NumBonds);
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
        /// <example>The console output:
        /// <code>
        /// OKAY
        /// C 3
        /// C 3
        /// F 1
        /// F 1
        /// H 1
        /// H 1
        /// </code>
        /// </example>
        /// <seealso cref="InChIStrucOutput"/>
        public static int ParseInChI(string inchi, string options, out InChIStrucOutput outStruct)
        {
            InChIStringInput inp = new InChIStringInput(inchi, options);

            int retCode = ParseInChI(ref inp,out outStruct);

            return retCode;
        }

        /// <summary>
        /// A convenience overload of ParseInChI that passes an empty
        /// option string. The user is responsible for calling the 
        /// Dispose method of the InChIStrucOutput structure to free
        /// unmanaged memory.
        /// </summary>
        /// <param name="inchi">the inchi string</param>
        /// <param name="outStruct">the structure that will hold the output</param>
        /// <returns>an error code indicating the success or failure of the function call</returns>
        /// <seealso cref="InChIStrucOutput"/>
        public static int ParseInChI(string inchi, out InChIStrucOutput outStruct)
        {
            return ParseInChI(inchi, "", out outStruct);
        }

        /// <summary>
        /// A convenience overload of GetInChI that returns the inchi string
        /// or an empty string if the underlying platform invoke call fails.
        /// This method frees all unmanaged resources allocated by the 
        /// underlying platform invoke call.
        /// </summary>
        /// <param name="inp">the input structural data</param>
        /// <returns>an error code indicating the success or failure of the function call</returns>
        public static string GetInChI(ref InChIStrucInput inp)
        {
            int ret;
            // Console.WriteLine("Entering top level get inchi");
            return GetInChI(ref inp, out  ret);

        }

        /// <summary>
        /// A convenience overload of GetInChI that returns the inchi string
        /// or an empty string if the underlying platform invoke call fails.
        /// This method frees all unmanaged resources allocated by the underlying 
        /// platform invoke call. 
        /// </summary>
        /// <param name="inp">the structure holding the input data</param>
        /// <param name="retCode">an integer to store the return value from the underlying platform invoke call</param>
        /// <returns>the inchi code or the empty string</returns>
        public static string GetInChI(ref InChIStrucInput inp, out int retCode)
        {
            InChIStringOutput output = new InChIStringOutput();
            string ret;
            // Console.WriteLine("try getinchi starting");
            try
            {
                retCode = GetInChI(ref inp, out output);
                ret = output.InChI;
            }
            finally
            {
                 output.Dispose();
            }

            return ret;
        }

        /// <summary>
        /// A convenience overload of GetInChIKey that takes a string input. 
        /// The method returns an empty string or null if the underlying
        /// platform invoke call fails. 
        /// </summary>
        /// <param name="inchi">the input inchi</param>
        /// <returns>the inchi key</returns>
        /// <example>This example illustrates the use of this method from C# and Iron Python.
        /// <code>
        /// class Program
        /// {
        ///     public static void Main()
        ///     {
        ///         int retVal;
        ///         
        ///         string key = LibInChI.GetInChIKey("InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1+",retVal);
        ///         
        ///         // Console.WriteLine(key);
        ///     }
        /// }
        /// 
        /// //Output:
        /// //  WFLOTYSKFUPZQB-OWOJBTEDBV
        /// </code>
        /// <code>
        ///import clr
        ///
        ///clr.AddReference("CSInChI.dll")
        ///
        ///from CSInChI import LibInChI
        ///
        ///key = LibInChI.GetInChIKey("InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1+")
        ///
        ///print key
        ///
        ///#Output:
        ///#    WFLOTYSKFUPZQB-OWOJBTEDBV
        ///</code>
        /// </example>
        public static string GetInChIKey(string inchi)
        {
            StringBuilder sb = new StringBuilder(28);
            int retCode = GetInChIKey(inchi, sb);

            if (retCode != 0)
                return "";

            return sb.ToString();
        }

        /// <summary>
        /// A convenience overload of GetInChIKey that takes a string input. The method 
        /// returns an empty string or null if the underlying
        /// platform invoke call fails. The int parameter stores the return value
        /// from the unmanaged function.
        /// </summary>
        /// <param name="inchi">the inchi string</param>
        /// <param name="retCode">an int that holds the return value of the underlying function</param>
        /// <returns>the inchi key</returns>
        /// <example>This example illustrates using this method from C# and Iron Python.
        /// <code>
        /// class Program
        /// {
        ///     public static void Main()
        ///     {
        ///         int retVal;
        ///         string key = LibInChI.GetInChIKey("InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1+",retVal);
        ///         string outCode = GetInChIKeyRetVal.GetStringVal(retVal.Value);
        ///         // Console.WriteLine("{0} {1}",outCode,key);
        ///     }
        /// }
        /// 
        /// //Output:
        /// //  OKAY WFLOTYSKFUPZQB-OWOJBTEDBV
        /// </code>
        /// <code>
        ///import clr
        ///
        ///clr.AddReference("CSInChI.dll")
        ///
        ///#used to store properties passed with the ref or out
        ///#keywords in a CLR language
        ///from clr import Reference
        ///
        ///from CSInChI import LibInChI
        ///from CSInChI import GetInChIKeyRetVal
        ///
        ///#initalize the object that holds the return code
        ///retVal = Reference[int]()
        ///
        ///key = LibInChI.GetInChIKey("InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1+",retVal)
        ///outCode = GetInChIKeyRetVal.GetStringVal(retVal.Value)
        ///
        ///print outCode,key
        ///
        ///#Output:
        ///#    OKAY WFLOTYSKFUPZQB-OWOJBTEDBV
        /// </code>
        /// </example>
        public static string GetInChIKey(string inchi, out int retCode)
        {
            StringBuilder sb = new StringBuilder(25);
            retCode = GetInChIKey(inchi, sb);

            if (retCode != 0)
                return "";

            return sb.ToString();
        }

        /// <summary>
        /// A convenience overload of GetInChIFromInChI that takes two strings.
        /// invoke call. The method returns an empty string or null if the underlying
        /// platform invoke call fails. Options start with '/' in Windows or 
        /// '-' for other platforms. See inchi_options.txt for a complete list of options.
        /// This method frees all unmanaged resources allocated by the underlying platform
        /// invoke call.
        /// </summary>
        /// <param name="inputInChI">the input inchi</param>
        /// <param name="options">the space delimited options string</param>
        /// <returns>the output inchi or an empty string</returns>
        ///<example>This example shows how to use this method to convert
        /// an InChI string to its compressed equivalent and display the
        /// return code from the underlying unmanaged function.
        ///<code>
        /// class Program
        /// {
        ///     static void Main()
        ///     {
        ///         string inInChI = "InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H";
        ///         
        ///         string outInChI = LibInChI.GetInChIFromInChI(inInChI, "/compress");
        /// 
        ///         // Console.WriteLine(outInChI);
        ///     }
        /// }
        /// 
        /// //Output:
        /// //  InChI=1/C6H6/cAE1ABFD/hAF1
        ///</code>
        ///<code>
        ///import clr
        ///clr.AddReference("CSInChI.dll")
        ///
        ///from CSInChI import LibInChI
        ///
        ///inInChI = "InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H"
        ///
        ///compInChI = LibInChI.GetInChIFromInChI(inInChI,"/compress")
        ///
        ///print compInChI
        /// 
        ///#Output"
        ///#   InChI=1/C6H6/cAE1ABFD/hAF1
        ///</code>
        /// </example>
        public static string GetInChIFromInChI(string inputInChI, string options)
        {
            int retCode;
            return GetInChIFromInChI(inputInChI, options, out retCode);
        }

        /// <summary>
        /// A convenience overload of GetInChIFromInChI that takes two strings.
        /// The int parameter stores the return value of the underlying platform
        /// invoke call. The method returns null or an empty string if the underlying
        /// platform invoke call fails. Options start with '/' in Windows or '-' for 
        /// other platforms. See inchi_options.txt for a complete list of options. This method frees all unmanaged
        /// resources allocated by the underlying platform invoke call.
        /// </summary>
        /// <param name="inputInChI">the input inchi</param>
        /// <param name="options">the space delimited options string</param>
        /// <param name="retCode">an integer to store the return value of the unmanaged function</param>
        /// <returns>the output inchi, null, or an empty string</returns>
        /// <example>This example shows how to use this method to convert
        /// an InChI string to its compressed equivalent.
        /// <code>
        /// class Program
        /// {
        ///     static void Main(string[] args)
        ///     {
        ///         string inInChI = "InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H";
        ///         int retCode;
        ///         string outInChI = LibInChI.GetInChIFromInChI(inInChI, "/compress", out retCode);
        ///         string result = InChIRetVal.GetStringVal(retCode);
        ///         // Console.WriteLine(result);
        ///         // Console.WriteLine(outInChI);
        ///     }
        /// }
        /// 
        /// //Output:
        /// //    OKAY
        /// //    InChI=1/C6H6/cAE1ABFD/hAF1
        /// </code>
        /// <code>
        ///import clr
        ///
        ///clr.AddReference("CSInChI.dll")
        ///
        ///#used to store properties passed with the ref or out
        ///#keywords in a CLR language
        ///from clr import Reference
        ///
        ///from CSInChI import LibInChI
        ///
        ///inInChI = "InChI=1/C6H6/c1-2-4-6-5-3-1/h1-6H"
        ///
        ///#initalize the object that holds the return code
        ///retCode = Reference[int]()
        /// 
        ///compInChI = LibInChI.GetInChIFromInChI(inInChI, "/compress", retCode)
        ///result = InChIRetVal.GetStringVal(retCode.Value);
        ///
        ///print result
        ///print compInChI
        /// 
        ///#Output:
        ///#   OKAY
        ///#   InChI=1/C6H6/cAE1ABFD/hAF1 
        /// </code>
        /// </example>
        public static string GetInChIFromInChI(string inputInChI, string options, out int retCode)
        {
            InChIStringInput inp = new InChIStringInput(inputInChI, options);
            string ret;
            InChIStringOutput outInChI = new InChIStringOutput();
            try
            {
                retCode = GetInChIFromInChI(ref inp, out outInChI);

                ret = outInChI.InChI;
            }
            finally
            {
                outInChI.Dispose();
            }
            return ret;
        }

    }//end class LibInChI
}
