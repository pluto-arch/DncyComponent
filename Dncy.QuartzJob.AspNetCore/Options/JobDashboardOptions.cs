using System.Diagnostics.CodeAnalysis;

namespace Dotnetydd.QuartzJob.AspNetCore.Options
{
    public class JobDashboardOptions
    {
        [NotNull] public PathString HomePath { get; set; } = new PathString("/job");

        [NotNull]
        public PathString APIPath { get; set; } = new PathString("/job-api");
    }
}
