using System;
using Windows.Storage;

namespace Plugin.AudioRecorder
{
	public partial class AudioRecorderService
	{
		async partial void Init()
		{
			if (FilePath == null)
			{
				StorageFolder temporaryFolder = ApplicationData.Current.TemporaryFolder;
				StorageFile tempFile = await temporaryFolder.CreateFileAsync(DefaultFileName, CreationCollisionOption.ReplaceExisting);

				FilePath = tempFile.Path;
			}
		}
	}
}
