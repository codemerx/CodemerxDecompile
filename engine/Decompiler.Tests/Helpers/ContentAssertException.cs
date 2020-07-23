//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.
//    To use a different set of GPL versions, you would modify the end of the first long paragraph.For instance, to license under version 2 or later, you would replace “3” with “2”.

//    This statement should go near the beginning of every source file, close to the copyright notices. When using the Lesser GPL, insert the word “Lesser” before “General” in all three places.When using the GNU AGPL, insert the word “Affero” before “General” in all three places.

using System;

namespace Decompiler.Tests.Helpers
{
    internal class ContentAssertException : Exception
    {
        public ContentAssertException(string message, Exception innerException) : base(message, innerException) { }
    }
}
