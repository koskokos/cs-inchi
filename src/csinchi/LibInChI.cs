using System;
using System.Runtime.InteropServices;
using System.Security;

namespace CSInChI
{
    [SuppressUnmanagedCodeSecurity]
    public unsafe static class LibInChI
    {
        const string libInchiName = "inchi";

        public const int ISOTOPIC_SHIFT_FLAG = 10000;
        public const int ISOTOPIC_SHIFT_MAX = 100;
        public const int MAX_BONDS = 20;
        public const int NUM_H_ISOTOPES = 3;
        public const int ATOM_EL_LEN = 6;
        public const short NO_ATOM = -1;

        [DllImport(libInchiName, EntryPoint = "GetStructFromINCHI")]
        public static extern int ParseInChI(ref InChIStringInput input, out InChIStrucOutput output);

        [DllImport(libInchiName, EntryPoint = "GetStdINCHI")]
        public static extern int GetInChI(ref InChIStrucInput structData, out InChIStringOutput output);

        [DllImport(libInchiName, EntryPoint = "GetStdINCHIKeyFromStdINCHI")]
        static extern int GetInChIKey(sbyte* inchi, sbyte* sb);

        [DllImport(libInchiName, EntryPoint = "CheckINCHIKey")]
        public static extern int CheckInChIKey(string inchiKey);

        [DllImport(libInchiName, EntryPoint = "FreeStructFromINCHI")]
        internal static extern void DeallocateOutputStruct(ref InChIStrucOutput outputStruct);

        [DllImport(libInchiName, EntryPoint = "FreeINCHI")]
        internal static extern void DeallocateInChIString(ref InChIStringOutput inchiOut);

        [DllImport(libInchiName, EntryPoint = "GetINCHIfromINCHI")]
        public static extern int GetInChIFromInChI(ref InChIStringInput inp, out InChIStringOutput outStruct);

        public static int ParseInChI(string inchi, string options, out InChIStrucOutput outStruct)
        {
            InChIStringInput inp = new InChIStringInput(inchi, options);

            int retCode = ParseInChI(ref inp, out outStruct);

            return retCode;
        }

        public static int ParseInChI(string inchi, out InChIStrucOutput outStruct)
        {
            return ParseInChI(inchi, "", out outStruct);
        }

        public static string GetInChI(ref InChIStrucInput inp) => GetInChI(ref inp, out int ret);

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

        public static string GetInChIKey(string inchi)
        {
            sbyte* inchiKeyPtr = stackalloc sbyte[28];
            sbyte* input = stackalloc sbyte[inchi.Length + 1];
            for (int i = 0; i < inchi.Length; i++)
            {
                input[i] = (sbyte)inchi[i];
            }
            int retCode = GetInChIKey(input, inchiKeyPtr);

            if (retCode != 0) throw new ApplicationException($"GetInChIKey returned code {retCode}");

            return new string(inchiKeyPtr, 0, 27);
        }

        public static string GetInChIKey(ref InChIStrucInput inp)
        {
            var retCode = GetInChI(ref inp, out InChIStringOutput output);
            if(retCode != 0)
            {
                throw new ApplicationException($"GetInChI returned code {retCode}. Internal error: {output.ErrorMessage}");
            }
            try
            {
                sbyte* inchiKeyPtr = stackalloc sbyte[28];
                retCode = GetInChIKey(output.inchi, inchiKeyPtr);
                if (retCode != 0)
                {
                    throw new ApplicationException($"GetInChIKey returned code {retCode}");
                }
                return new string(inchiKeyPtr, 0, 27);
            }
            finally
            {
                output.Dispose();
            }
        }

        public static string GetInChIKey(string inchi, out int retCode)
        {
            sbyte* inchiKeyPtr = stackalloc sbyte[28];
            sbyte* input = stackalloc sbyte[inchi.Length + 1];
            for (int i = 0; i < inchi.Length; i++)
            {
                input[i] = (sbyte)inchi[i];
            }
            retCode = GetInChIKey(input, inchiKeyPtr);

            return new string(inchiKeyPtr, 0, 27);
        }

        public static string GetInChIFromInChI(string inputInChI, string options) 
            => GetInChIFromInChI(inputInChI, options, out int retCode);


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

    }
}
