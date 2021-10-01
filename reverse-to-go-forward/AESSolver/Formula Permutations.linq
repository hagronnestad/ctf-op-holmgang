<Query Kind="Program" />

// Taken from: https://www.geeksforgeeks.org/print-all-combinations-of-given-length/

// C# program to print all
// possible strings of length k
class GFG {

	// The method that prints all
	// possible strings of length k.
	// It is mainly a wrapper over
	// recursive function printAllKLengthRec()
	static public void printAllKLength(char[] set, int k) {
		int n = set.Length;
		printAllKLengthRec(set, "", n, k);
	}

	// The main recursive method
	// to print all possible
	// strings of length k
	static void printAllKLengthRec(char[] set, String prefix, int n, int k) {
		// Base case: k is 0,
		// print prefix
		if (k == 0) {
			//prefix.Dump();
			f.WriteLine(prefix);
			return;
		}

		// One by one add all characters
		// from set and recursively
		// call for k equals to k-1
		for (int i = 0; i < n; ++i) {
			// Next character of input added
			String newPrefix = prefix + set[i];

			// k is decreased, because
			// we have added a new character
			printAllKLengthRec(set, newPrefix, n, k - 1);
		}
	}
	
}

static StreamWriter f = File.AppendText(@"allcombinations6.txt");

// Driver Code
static public void Main()
{
	char[] set1 = { '−', '+', '÷', '×', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'c', 'o', 's', 't', 'a', 'n', 's', 'i', 'n', '(', ')' };
	int k = 6;
	GFG.printAllKLength(set1, k);
}