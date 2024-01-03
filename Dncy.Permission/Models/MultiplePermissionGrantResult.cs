using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnetydd.Permission.Models
{
    public class MultiplePermissionGrantResult
    {
        public MultiplePermissionGrantResult()
        {
            Result = new Dictionary<string, PermissionGrantResult>();
        }


        public MultiplePermissionGrantResult(string[] names,
            PermissionGrantResult grantResult = PermissionGrantResult.Prohibited) : this()
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            Array.ForEach(names, name => Result.Add(name, grantResult));
        }

        public Dictionary<string, PermissionGrantResult> Result { get; }


        public bool AllGranted => Result.Count > 0 && Result.Values.All(x => x == PermissionGrantResult.Granted);

        public bool AllProhibited => Result.Count > 0 && Result.Values.All(x => x == PermissionGrantResult.Prohibited);
    }
}