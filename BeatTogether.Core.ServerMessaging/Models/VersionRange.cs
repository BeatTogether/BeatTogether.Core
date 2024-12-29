using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatTogether.Core.Models
{
	public class VersionRange
	{
		public string MinVersion { get; set; } = string.Empty;
		public string MaxVersion { get; set; } = new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue).ToString();

		public static VersionRange? FindVersionRange(List<VersionRange> versionRanges, string version)
		{
			foreach (var range in versionRanges)
				if (VersionRangeSatisfies(range, version))
					return range;
			return null;
		}

		public static bool VersionRangeSatisfies(VersionRange range, string version)
		{
			var minVersion = Version.Parse(range.MinVersion);
			var maxVersion = Version.Parse(range.MaxVersion);
			var parsedVersion = Version.Parse(version);
			return parsedVersion >= minVersion && parsedVersion <= maxVersion;
		}
	}
}
