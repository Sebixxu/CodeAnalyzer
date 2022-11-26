using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalyzerBase.Common
{
    static class StringExtension
    {
        static List<string> _primitiveTypes = new List<string> { "Boolean", "Byte", "SByte", "Int16", "UInt16", "Int32", "UInt32", "Int64", "UInt64", "Char", "Double", "Single" };
        
        static List<string> _integerTypes = new List<string> { "Int16", "Int32", "Int64"};
        static List<string> _unsignedIntegerTypes = new List<string> { "UInt16", "UInt32", "UInt64" };
        
        static List<string> _booleanTypes = new List<string> { "Boolean" };

        static List<string> _byteTypes = new List<string> { "Byte"};
        static List<string> _shortByteTypes = new List<string> { "SByte" };

        static List<string> _charTypes = new List<string> { "Char" };

        static List<string> _doubleTypes = new List<string> { "Double" };

        static List<string> _singleTypes = new List<string> { "Single" };

        internal static bool IsPrimitveType(this string name)
        {
            var a = 1; 
            if (!_primitiveTypes.Contains(name))
            {
                return false;
            }

            return true;
        }

        internal static string ParseVariableType(this string name)
        {
            if (_integerTypes.Contains(name))
            {
                return "int";
            }
            else if (_unsignedIntegerTypes.Contains(name))
            {
                return "uint";
            }
            else if (_booleanTypes.Contains(name))
            {
                return "bool";
            }
            else if (_byteTypes.Contains(name))
            {
                return "byte";
            }
            else if (_shortByteTypes.Contains(name))
            {
                return "sbyte";
            }
            else if (_charTypes.Contains(name))
            {
                return "char";
            }
            else if (_doubleTypes.Contains(name))
            {
                return "double";
            }
            else if (_singleTypes.Contains(name))
            {
                return "Single";
            }
            else
            {
                throw new ArgumentOutOfRangeException("Recived type is not supported to parse");
            }
        }
    }
}
