CSInChI v0.5:

A class library for using the IUPAC InChI library from the .Net framework and the Mono Runtime. Please note that that this library is not currently thread safe.

Requirements:
	version 2.0 or later of the .Net framework or an equivalent version of Mono.

	The IUPAC InChI library. Full functionality requires ver. 1.0.2 or later.
	It can be downloaded from http://www.iupac.org/inchi or the newer InChI website at
	http://www.inchi.info.

Release Files:

	CsInChI.dll - the assembly
	CSInChI_Doc.chm - searchable documentation
	CSInChI.xml - An XML doc file used to generate the documentation. This file must be in the same directory
		      as the dll for the help() function in Iron Python to display the documentation comments. 
	Stereo_Parties.txt - contains the InChI library documentation for using 0D stereo parities
	inchi_options.txt - contains a complete list of input options
	license.txt - contains a copy of the LGPL license

Source Files:
	LibInChI.cs : contains the definition of the class that exposes the external methods that access the InChI library
	
	InChI_Input_Structs.cs : contains definitions for the structures used as inputs to InChI library functions

	InChI_Output_Structs.cs : contains definitions for the structures used to hold output from InChI library functions
	
	InChI_Atom.cs : contains the definition of the structures used to hold atom and stereo data in the InChI library

	InChI_Enums.cs : a set of static classes containing constants that correspond to the enumerations defined 				 	 in the InChI library

	

Deallocating Unmanaged Memory:

	The InChI library provides the functions FreeINCHI and FreeStructFromInChI to deallocate unmanaged memory. In 			CSInChI these methods are not exposed. Instead they are called by the Dispose method of the the InChIStringOutput 		and InChIStructOutput structures. The InChIStructInput also requires explicit disposal because it allocates 			unmanaged memory.



For more information about the InChI see the InChI Technical Manual and The Unofficial InChI FAQ located at http://wwmm.ch.cam.ac.uk/inchifaq/



CSInChI is a part of the ChemSharp Project
	http://sourceforge.net/projects/chemsharp

Questions, comments, and bug reports should be directed to the CSInChI Mailing List:
	chemsharp-csinchi@lists.sourceforge.net


I'd like extend a special thanks to Matt Baldwin for his invaluable debugging help and expert advice on the ins and outs of unmanaged code.

