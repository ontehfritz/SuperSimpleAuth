using System;
using System.Collections.Generic;

namespace SSAManager
{
    public static class Helpers
    {
        /// <summary>
        /// Values the specified array.
        /// </summary>
        /// <param name="array">Array.</param>
        public static string Values(IEnumerable<string> array)
        {
            if (array != null) {
                return string.Join (",", array);
            }

            return "";
        }
    }
}

