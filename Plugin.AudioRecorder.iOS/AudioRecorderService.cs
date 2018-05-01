using System.IO;
using System.Threading.Tasks;

namespace Plugin.AudioRecorder
{
	public partial class AudioRecorderService
	{
		Task Init()
		{
			if (FilePath == null)
			{
				FilePath = Path.Combine(Path.GetTempPath(), DefaultFileName);
			}
			return Task.CompletedTask;
		}
	}
}
