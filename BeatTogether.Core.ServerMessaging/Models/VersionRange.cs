using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatTogether.Core.Models
{
	public class VersionRange
	{
		public string MinVersion { get; set; } = "0.0.0";
		public string MaxVersion { get; set; } = new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue).ToString();

		public static VersionRange? FindVersionRange(List<VersionRange> versionRanges, Version version)
		{
			foreach (var range in versionRanges)
				if (VersionRangeSatisfies(range, version))
					return range;
			return null;
		}

		public static VersionRange? FindVersionRange(List<VersionRange> versionRanges, string version)
		{
			return FindVersionRange(versionRanges, version);
		}
		public static bool VersionRangeSatisfies(VersionRange range, Version version)
		{
			var minVersion = Version.Parse(range.MinVersion);
			var maxVersion = Version.Parse(range.MaxVersion);
			return version >= minVersion && version <= maxVersion;
		}

		public static bool VersionRangeSatisfies(VersionRange range, string version)
		{
			return VersionRangeSatisfies(range, Version.Parse(version));
		}

		public static VersionStatus CheckVersionRange(VersionRange range, Version version)
		{
			var minVersion = Version.Parse(range.MinVersion);
			var maxVersion = Version.Parse(range.MaxVersion);
			if (version < minVersion) return VersionStatus.TooLow;
			if (version > maxVersion) return VersionStatus.TooHigh;
			return VersionStatus.Ok;
		}

		public enum VersionStatus
		{
			Ok,
			TooHigh,
			TooLow
		}
	}
}
