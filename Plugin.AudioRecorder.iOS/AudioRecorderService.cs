using System.IO;

namespace Plugin.AudioRecorder
{
	public partial class AudioRecorderService
	{
		partial void Init()
		{
			if (FilePath == null)
			{
				FilePath = Path.Combine(Path.GetTempPath(), DefaultFileName);
			}
		}
	}
}
